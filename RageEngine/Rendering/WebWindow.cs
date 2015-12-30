using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.Graphics;
using RageEngine;
using System.Drawing;
using Awesomium.Core;
using SharpDX.Direct3D11;
using RageEngine.Input;
using SharpDX;
using RageEngine.Graphics.TwoD;
using RageEngine.ContentPipeline;

namespace RageEngine.Rendering
{

    public class WebWindow
    {
        public static void Initialize() {

            views = new List<WebWindow>();

            WebConfig config = new WebConfig() {
                AutoUpdatePeriod=30,
#if DEBUG
                LogLevel = LogLevel.Normal
#endif
            };

            WebCore.Initialize(config, true);
            //WebCore.PackagePath = System.IO.Directory.GetCurrentDirectory();

            session = WebCore.CreateWebSession(
                new WebPreferences() {
                    CustomCSS = "textarea, input { outline: none; }",
                    CanScriptsCloseWindows = false,
                    CanScriptsOpenWindows = false,
                    Plugins = false,
                    WebGL = false,
                    WebAudio = false,
                    SmoothScrolling = true,
                    FileAccessFromFileURL = false,
                    UniversalAccessFromFileURL = false,
                    AppCache = false,
                    Databases = false,
                    CanScriptsAccessClipboard = true,
                }
            );

        }

        public static void Update() {
            for (int i = 0; i < views.Count; i++) {
                views[i].Push();
            }
        }

        public static void DisposeAll() {
            for (int i = 0; i < views.Count; i++) {
                views[i].Dispose();
            }
            WebCore.Shutdown();
        }

        static WebSession session;
        static WebWindow focused = null;
        static List<WebWindow> views;

        public Vector2 position;
        public int width, height;

        private WebView webView;
        private JSObject app;
        private JSObject document;

        private BitmapSurface surface;
        private Texture webTex;
        private Texture2D mappableTexture;
        private SharpDX.DXGI.Surface mappableSurface;
        private Texture2DDescription descHM;


        bool initialized=false;

        private Stack<Tuple<string, JavascriptMethodEventHandler>> binds;
        private Stack<Tuple<string, JSValue[]>> invokes;

        public WebWindow(int width, int height, string url) {
            this.width  =width;
            this.height =height;

            views.Add(this);

            webView = WebCore.CreateWebView(width, height, session);

            webView.Source = new Uri("file:///"+System.IO.Directory.GetCurrentDirectory() +"/"+ Resources.FindFile(url));
            //webView.Source = new Uri("http://www.google.com");
            //webView.Source = new Uri("http://www.chiptune.com/starfield/starfield.html");

            webView.IsTransparent = true;

            webView.PropertyChanged+=webView_PropertyChanged;

            webView.Crashed += webView_Crashed;

            webView.DocumentReady +=webView_DocumentReady;

            descHM = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
            };

            mappableTexture = new Texture2D(Display.device, descHM);
            mappableSurface = mappableTexture.QueryInterface<SharpDX.DXGI.Surface>();

            descHM.Usage = ResourceUsage.Default;
            descHM.CpuAccessFlags = CpuAccessFlags.None;
            descHM.BindFlags = BindFlags.ShaderResource;

            InputManager.MouseDown +=_onMouseDown;
            InputManager.MouseUp += _onMouseUp;
            InputManager.MouseMove += _onMouseMove;
            InputManager.Wheel += _onMouseWheel;

            Texture2D webTexture = new Texture2D(Display.device, descHM);
            webTex = new Texture(webTexture);


            position = new Vector2(0,0);

            binds = new Stack<Tuple<string, JavascriptMethodEventHandler>>();
            invokes = new Stack<Tuple<string, JSValue[]>>();

        }

        public void Push(){
            if (!initialized || !webView.IsDocumentReady) return;
            if (binds.Count!=0) {
                Tuple<string, JavascriptMethodEventHandler> bind;
                while (binds.Count!=0) {
                    bind = binds.Pop();
                    app.Bind(bind.Item1, false, bind.Item2);
                }
            }
            if (invokes.Count!=0) {
                Tuple<string, JSValue[]> invoke;
                while (invokes.Count!=0) {
                    invoke = invokes.Pop();
                    document.Invoke(invoke.Item1,invoke.Item2);
                }
            }
        }

        internal void webView_Crashed(object sender, CrashedEventArgs e) {
            Error getLastError = webView.GetLastError();
            throw new Exception("WebKit Error :" + getLastError.ToString());
        }

        internal void webView_DocumentReady(object sender, UrlEventArgs e) {
            app = webView.ExecuteJavascriptWithResult("App");
            app.RemoveProperty("test");
            document = webView.ExecuteJavascriptWithResult("Remote");
        }

        internal void webView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName=="Surface") {
                if (surface!=null) surface.Dispose();
                surface = webView.Surface as BitmapSurface;
                initialized = true;
            }
        }

        internal bool IsInside(int x, int y) {
            if (x<position.X || y<position.Y || x>position.X+width || y>position.Y+height) return false;
            return (surface.GetAlphaAtPoint(-(int)position.X + x,-(int)position.Y + y) > 200);
        }

        internal void _onMouseDown(MouseInputEventArgs e){
            if (!initialized) return;

            if (IsInside(e.X, e.Y)) {
                focused=this;
                webView.FocusView();
                e.Handled = true;
                InputManager.textInput.RawMessage = _onKeyPress;
                if (e.IsLeftButtonDown) webView.InjectMouseDown(MouseButton.Left);
            } else {
                if (focused==this) {
                    focused=null;
                    webView.UnfocusView();
                    InputManager.textInput.RawMessage = null;
                }
            }
        }

        internal void _onMouseUp(MouseInputEventArgs e) {
            if (initialized) if (!e.IsLeftButtonDown) webView.InjectMouseUp(MouseButton.Left);
        }

        internal void _onMouseMove(MouseInputEventArgs e) {
            if (initialized) {
                webView.InjectMouseMove(-(int)position.X + e.X, -(int)position.Y + e.Y);
            }
        }

        internal void _onMouseWheel(MouseInputEventArgs e) {
            if (!initialized) return;
            if (e.X<position.X || e.Y<position.Y || e.X>position.X+descHM.Width || e.Y>position.Y+descHM.Height) return;
            e.Handled = true;
            webView.InjectMouseWheel((int)e.WheelDelta,0);
        }

        internal void _onKeyPress(int msg, int wparam, int lparam) {
            if (!initialized) return;
            if (focused==this &&
                (webView.FocusedElementType.HasFlag(FocusedElementType.TextInput)
                || webView.FocusedElementType.HasFlag(FocusedElementType.Input))) {
                webView.InjectKeyboardEvent(new WebKeyboardEvent((uint)msg, (IntPtr)wparam, (IntPtr)lparam, (Modifiers)0));
            }
        }

        public void Bind(string function,JavascriptMethodEventHandler callback) {
            binds.Push(new Tuple<string,JavascriptMethodEventHandler>(function,callback));
        }

        public void Invoke(string function, params JSValue[] values) {
            invokes.Push(new Tuple<string, JSValue[]>(function, values));
        }

        public void Dispose() {
            InputManager.MouseDown -=_onMouseDown;
            InputManager.MouseUp -= _onMouseUp;
            InputManager.MouseMove -= _onMouseMove;
            InputManager.Wheel -= _onMouseWheel;
            if (focused==this) InputManager.textInput.RawMessage = null;
            webView.Dispose();
        }

        public void Resize(int width, int height) {
            if (this.width==width && this.height==height) return;

            this.width  =width;
            this.height =height;

            mappableTexture.Dispose();
            webTex.Tex.Dispose();
            mappableSurface.Dispose();

            webView.Resize(width, height);
            //webView.Reload(false);
            

            //descHM
            descHM.Usage = ResourceUsage.Staging;
            descHM.CpuAccessFlags = CpuAccessFlags.Write;
            descHM.BindFlags = BindFlags.None;
            descHM.Width = width;
            descHM.Height = height;

            mappableTexture = new Texture2D(Display.device, descHM);
            mappableSurface = mappableTexture.QueryInterface<SharpDX.DXGI.Surface>();

            descHM.Usage = ResourceUsage.Default;
            descHM.CpuAccessFlags = CpuAccessFlags.None;
            descHM.BindFlags = BindFlags.ShaderResource;

            Texture2D webTexture = new Texture2D(Display.device, descHM);
            webTex.Tex = webTexture;
            webTex.Width=width;
            webTex.Height=height;
            webTex.RecreateSRV();
        }


        public void Draw() {
            if (!initialized) return;
#if DEBUG
            //bool wasDirty = surface.IsDirty;
#endif
            if (surface.IsDirty) {
                DataRectangle rect = mappableSurface.Map(SharpDX.DXGI.MapFlags.Write);
                surface.CopyTo(rect.DataPointer, rect.Pitch, 4, false, false);
                mappableSurface.Unmap();
                Display.context.CopyResource(mappableTexture, webTex.Tex);
            }
            QuadRenderer.Draw(webTex, position,new Color4(1,1,1,1));
#if DEBUG
            //FontRenderer.Draw(FontManager.Get("default"), "Dirty:"+wasDirty, new Vector2(position.X, position.Y), Color.White);
#endif
        }
    }
}
