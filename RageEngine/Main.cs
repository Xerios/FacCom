#region Using declarations
using System;
using System.Windows.Forms;
using System.ComponentModel.Design;
using RageEngine.Debug;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Windows;
using Buffer=SharpDX.Direct3D11.Buffer;
using Device=SharpDX.Direct3D11.Device;
using MapFlags=SharpDX.Direct3D11.MapFlags;
using SharpDX;
using RageEngine.Graphics;
using RageEngine.ContentPipeline;
using RageEngine.Input;
using RageEngine.Graphics.TwoD;
#endregion

namespace RageEngine {
    public class Main {
        public string Title="RageEngine";

        public static Form Form;

        private bool formIsResizing=false;

        public Main() {
            Logger.Log("-------------------------- Engine start ---------------------------");
#if DEBUG
            Logger.Log("Launching debug build "+RetrieveLinkerTimestamp().ToString());
#else
            Logger.Log("Launching release build "+RetrieveLinkerTimestamp().ToString());
#endif
        }

        public void Setup() {

            //if (ProcessChecker.IsOnlyProcess(Title)) throw new Exception("Unable to launch the game, it appears that an instance of the game is already running.");
            //PixHelper.AllowProfiling(false);

            Form=new RenderForm {
                ClientSize=new System.Drawing.Size((int)Display.Width, (int)Display.Height),
                StartPosition=FormStartPosition.CenterScreen
            };

            Form.Text=Title;
            Form.MaximizeBox=!Display.Fullscreen;
            if (Display.Fullscreen) Form.FormBorderStyle=FormBorderStyle.None;
            Form.FormClosed+=new FormClosedEventHandler(OnClose);

            Global.Timer=new Timer();
            Logger.Log("Render form initialized.");

            Display.SetupDisplay(Form);

            Main.Form.LostFocus+=(o, e) => {
                if (Display.Fullscreen) Main.Form.WindowState=FormWindowState.Minimized;
            };
            Main.Form.GotFocus+=(o, e) => {
                Display.swapChain.IsFullScreen=Display.Fullscreen;
            };

            Main.Form.Resize+=(o, e) => {
                if (Main.Form.WindowState==FormWindowState.Maximized||lastWindowState==FormWindowState.Maximized) WindowResize();
                lastWindowState=Main.Form.WindowState;
            };
            Main.Form.ResizeBegin+=(o, args) => {
                formIsResizing=true;
            };
            Main.Form.ResizeEnd+=(o, e) => {
                formIsResizing=false;
                WindowResize();
            };

        }


        private FormWindowState lastWindowState=FormWindowState.Normal;
        private void WindowResize() {
            if (Main.Form.ClientSize.Width==0||Main.Form.ClientSize.Height==0) return;
            if (Main.Form.WindowState==FormWindowState.Minimized) return;
            if (Display.Width==Main.Form.ClientSize.Width&&Display.Height==Main.Form.ClientSize.Height) return;

            lock (Display.device) {
                Display.ResizeDisplay(Form);
                Resize();
            }
        }

        public void Run() {
            Setup();
            Logger.Log("--- Game Init ---");
            Initialize();
            Logger.Log("Game initializing done.");
            Global.IsRunning=true;
            Global.Timer.Start();

            RenderLoop.Run(Form, InternalUpdate);

            Logger.Log("Loop ended.");
            Dispose();
            Exit();
            Logger.Log("All disposed.");
            Logger.Log("Game successfully exited");
        }

        private void InternalUpdate() {
            System.Threading.Thread.Yield();
            Loops=0;

            if (Limit_FPS) {
                while (Global.Timer.stopwatch.Elapsed.TotalMilliseconds>=NextFrame&&Loops<MaxFrameSkip) {
                    NextFrame+=SkipUpdate;
                    Loops++;
                }

                if (Global.Timer.stopwatch.Elapsed.TotalMilliseconds>=LastFrame+SkipFrame) {
                    LastFrame=Global.Timer.stopwatch.Elapsed.TotalMilliseconds;
                    Frames++;
                    OnUpdate();
                    OnRender();
                }
            } else {
                Frames++;
                OnUpdate();
                OnRender();
            }

            if (!Global.IsRunning) Application.Exit();
        }

        private int Frames;

        public static bool Limit_FPS=true;
        const int MaxFPS=60;

        const double MaxUPS=50;
        long MaxFrameSkip=10;

        double SkipUpdate=1000d/MaxUPS;
        double SkipFrame=1000d/MaxFPS;

        long Loops;
        double LastFrame=0;
        double NextFrame=0;

        private void OnUpdate() {

            // Update subsystems
            Global.Timer.Update();

            // Invoke application update
            Update();
        }

        protected virtual void Initialize() { }
        protected virtual void Update() { }
        protected virtual void Render() {
            Display.context.ClearRenderTargetView(Display.renderTarget, Color.CornflowerBlue);
        }
        protected virtual void Resize() { }
        protected virtual void Dispose() { }


        private void OnRender() {
            if (formIsResizing) return;
            GraphicsManager.Reset();

            Render();

            Display.swapChain.Present(0, PresentFlags.None);
        }

        private void OnClose(object t, EventArgs e) {
            Main.Exit();
        }

        public static void Exit() {
            Global.IsRunning=false;
        }

        private static void OnDispose() {

            Global.IsRunning=false;

            Logger.Log("Fullscreen disabled.");

            FontManager.Dispose();
            Logger.Log("FontManager disposed.");

            Resources.Dispose();
            Logger.Log("Resources disposed.");

            InputManager.Dispose();
            Logger.Log("Input Manager disposed.");

            ShaderManager.Dispose();
            Logger.Log("Shaders disposed.");

            Display.Dispose();

        }

        // Get Assembly TimeStamp
        public static DateTime RetrieveLinkerTimestamp() {
            string filePath=System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset=60;
            const int c_LinkerTimestampOffset=8;
            byte[] b=new byte[2048];
            System.IO.Stream s=null;

            try {
                s=new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            } finally {
                if (s!=null) s.Close();
            }

            int i=System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970=System.BitConverter.ToInt32(b, i+c_LinkerTimestampOffset);
            DateTime dt=new DateTime(1970, 1, 1, 0, 0, 0);
            dt=dt.AddSeconds(secondsSince1970);
            dt=dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
    }

}