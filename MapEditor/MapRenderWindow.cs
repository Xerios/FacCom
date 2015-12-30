using SharpDX;
using System.IO;
using RageEngine.Utils;
using RageEngine;
using RageRTS.Map;
using RageRTS;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using RageEngine.Graphics;
using RageEngine.Rendering;
using RageEngine.ContentPipeline;
using RageEngine.Graphics.TwoD;
using Color = SharpDX.Color;

namespace MapEditor {
    public enum Tool { 
        NOTHING, 
        LANDSCAPE_UP, LANDSCAPE_DOWN, LANDSCAPE_SMOOTH, LANDSCAPE_SET,
        TEXTURE, 
        COLOR,
        OBJECT_EDIT,OBJECT_PLACE, 
        DECAL_EDIT,DECAL_PLACE,
        PATHFINDING,
        NODE_PLACE,NODE_SELECT
    }

    public static class InputHelper {
        public static int mouseX, mouseY, wheelDelta;
        public static bool rightDown, middleDown, leftDown, altDown,delDown;
    }


    public class MapRenderWindow : DXControl {

        public static MapRenderWindow instance;
        public bool mouseInside = false;

        public bool firstRun = true;

        public TerrainInfo Map;
        public EditorTerrain Terrain;
        public TerrainWater TerrainWater;

        MainWindow form;

        public bool loading = false;

        public EditorCamera camera;

        public static Texture2D brushTexture,brushTextureReadable;
        public static RenderTargetView brushRenderTarget;

        public static Texture2D drawableTexture;
        public static RenderTargetView drawableRenderTarget;

        Texture DefaultBrush;
        Texture defaultBrushShape;

        //Material blockWhiteMaterial;

        public Tool     currentTool = Tool.NOTHING;
        public Texture  CurrentBrush;
        public float    CurrentBrushSize=20, CurrentBrushSoftness=10, CurrentBrushIntensity=10, HeightLevelSetBrush;
        public Color4   CurrentBrushColor;

        public int showCompass=0;

        private bool updateBrush = true, updateWaterShader = false;

        public bool modifiedColorMap = false, modifiedTextureMap = false;

        private float lastBrushSize, lastBrushSoftness;

        public MapModel selectedObject = null;
        public MapNode selectedNode = null;
        public Vector3 lastDest,lastPos, lastScale;
        public Quaternion lastRot;
        //public List<MapModel> mapObjects = new List<MapModel>();

        List<float[]> listForSmoothing = new List<float[]>();

        public MapRenderWindow() : base() {

        }

        protected override void WinInitialize() {
            instance = this;

            form = (MainWindow)Main.Form;

            loading=true;
            if (Display.device.IsDisposed) return;

            //Display.device.SetRenderState(RenderState.FillMode,FillMode.Wireframe);

            ShaderManager.Add("VS_2D", Resources.GetShader("VS", "VS", "UI2D.fx")).SetInputLayout(Vertex2D.Elements);
            ShaderManager.Add("PS_2D", Resources.GetShader("PS", "PS", "UI2D.fx"));

            ShaderManager.Add("VS_Debug", Resources.GetShader("VS", "VS", "Debug.fx")).SetInputLayout(VertexColor.Elements);
            ShaderManager.Add("PS_Debug", Resources.GetShader("PS", "PS", "Debug.fx"));

            ShaderManager.Add("VS_Terrain", Resources.GetShader("VS", "VS", "TerrainDebug.fx")).SetInputLayout(TerrainTriangle.Elements);
            ShaderManager.Add("PS_Terrain", Resources.GetShader("PS", "PS", "TerrainDebug.fx"));

            ShaderManager.Add("VS_Water", Resources.GetShader("VS", "VS", "Water.fx")).SetInputLayout(VertexTexture.Elements);
            ShaderManager.Add("PS_Water", Resources.GetShader("PS", "PS", "Water.fx"));


            FontManager.Add("default", Resources.GetFont("Fonts/default"));

            SceneManager.Initialize();
            QuadRenderer.Initialize();


            /*------------------------------------------------------------------------------------------*/

            Texture2DDescription desc = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Width = 512,
                Height = 512,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            };

            brushTexture = new Texture2D(Display.device, desc);
            brushRenderTarget = new RenderTargetView(Display.device, brushTexture);

            Texture2DDescription descReadable = desc;
            descReadable.BindFlags = BindFlags.None;
            descReadable.CpuAccessFlags = CpuAccessFlags.Read;
            descReadable.Usage = ResourceUsage.Staging;
            brushTextureReadable = new Texture2D(Display.device, descReadable);

            defaultBrushShape =Resources.GetTexture("blob.png", false);
            DefaultBrush = defaultBrushShape;

            CurrentBrush = new Texture(DefaultBrush.Tex);

            CurrentBrushColor = new Color4(1,0, 0, 0);

            camera = new EditorCamera();
            SceneManager.Camera = camera;

            LoadMap(MainWindow.mapFile);

            firstRun=false;
            loading=false;
            return;
        }


        public void LoadMap(RawMap mapFile) {


            GlobalConstantVars.SunColor = mapFile.Sun_Color;

            GlobalConstantVars.SunDirection.X = (float)Math.Sin(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Y = (float)Math.Sin(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Z = (float)Math.Cos(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Normalize();

            GlobalConstantVars.AmbientColor = new Vector4(mapFile.Ambient_Color, mapFile.Ambient);

            ForegroundGame.Map = new TerrainInfo();
            ForegroundGame.Map.width=mapFile.Width;
            ForegroundGame.Map.height=mapFile.Height;

            ForegroundGame.Map.altitudeData=new float[mapFile.Width, mapFile.Height];
            for (int y=0; y<mapFile.Height; y++) {
                for (int x=0; x<mapFile.Width; x++) {
                    float h=mapFile.Data[x+y*mapFile.Width];
                    ForegroundGame.Map.altitudeData[x, y]=h;
                    if (h>ForegroundGame.Map.maxAltitude) ForegroundGame.Map.maxAltitude=h;
                }
            }

            ForegroundGame.Map.accessibilityArray = mapFile.Accessibilty_Array;

            GlobalConstantVars.SunColor = mapFile.Sun_Color;

            GlobalConstantVars.SunDirection.X = (float)Math.Sin(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Y = (float)Math.Sin(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Z = (float)Math.Cos(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Normalize();

            GlobalConstantVars.AmbientColor = new Vector4(mapFile.Ambient_Color, mapFile.Ambient);


            ForegroundGame.Map.waterColor=new Vector4(mapFile.Water_Color, mapFile.Water_Density);
            ForegroundGame.Map.waterLevel=mapFile.Water_Level;
            ForegroundGame.Map.WaterBump=Resources.GetTexture("Maps/Shared/wave");

            ForegroundGame.Map.ColorMap=Resources.GetTexture("Maps/"+mapFile.MainPath+"/_colorMap.png", false);
            ForegroundGame.Map.TextureMap=Resources.GetTexture("Maps/"+mapFile.MainPath+"/_textureMap.png", false);

            ForegroundGame.Map.Shore = RageEngine.ContentPipeline.Resources.GetTexture("Maps/Shared/shore.jpg");
            ForegroundGame.Map.Clouds = RageEngine.ContentPipeline.Resources.GetTexture("Maps/Shared/clouds.jpg");

            string[] texturesWithNormal=new string[mapFile.Textures.Length*2];
            for (uint i = 0; i < mapFile.Textures.Length; i++) {
                texturesWithNormal[i*2]="Maps/Shared/"+mapFile.Textures[i];
                texturesWithNormal[i*2+1]="Maps/Shared/"+mapFile.Textures[i]+"_bump";
            }
            ForegroundGame.Map.TextureArray=Resources.GetTextureArray(texturesWithNormal);

            ForegroundGame.Map.Load();

            //-----------------------------

            form.mapName.Text = mapFile.Name;
            form.mapAuthor.Text = mapFile.Author;
            form.mapDesc.Text = mapFile.Description;

            form.waterLevel.Value = (decimal)ForegroundGame.Map.waterLevel;
            form.waterDensity.Value = (decimal)mapFile.Water_Density * 100;

            form.AmbientLight.Value = (decimal)(mapFile.Ambient * 100);
            form.sunHeight.Value = (int)(mapFile.Sun_Height * 90 / GameUtils.PiOver2);
            form.sunRotation.Value = (int)(mapFile.Sun_Rotation * 180 / GameUtils.Pi);

            //--------------------------------------------------------------------------------------

            modifiedColorMap = modifiedTextureMap = false;
            
            //--------------------------------------------------------------------------------------

            Texture2DDescription descHM = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Width = ForegroundGame.Map.width,
                Height = ForegroundGame.Map.height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            };

            drawableTexture = new Texture2D(Display.device, descHM);
            drawableRenderTarget = new RenderTargetView(Display.device, drawableTexture);


            Terrain = new EditorTerrain(ForegroundGame.Map);
            TerrainWater = new TerrainWater(ForegroundGame.Map);
            ForegroundGame.TerrainDecals = new DecalManager(ForegroundGame.Map);

            //------------------
            ForegroundGame.TerrainDebug = new TerrainDebug(ForegroundGame.Map);
            SceneManager.Add(Terrain);
            SceneManager.Add(ForegroundGame.TerrainDebug);
            SceneManager.Add(ForegroundGame.TerrainDecals);
            SceneManager.Add(TerrainWater);



            camera.target = new Vector3(ForegroundGame.Map.width/2, 0, ForegroundGame.Map.height/2);
            ForegroundGame.Map.GetHeight(camera.target, ref camera.target.Y);
            camera.UpdatePosition();

        }

        public void LoadTextures(RawMap file) {
            string[] texturesWithNormal = new string[file.Textures.Length * 2];
            for (uint i = 0; i < file.Textures.Length; i++) {
                texturesWithNormal[i * 2] = "Maps/Shared/" + file.Textures[i];
                texturesWithNormal[i * 2 + 1] = "Maps/Shared/" + file.Textures[i] + "_bump";
            }
            ForegroundGame.Map.TextureArray = Resources.GetTextureArray(texturesWithNormal);
        }

        bool LastClick, LastRightClick, LastMiddleClick;

        public float deltaWheel;
        public Vector3 pinPos;
        public Point clickPos, deltaPos;

        public Vector3 destination;
        public static Vector3 eyePosition;


        protected override void WinUpdate() {

            if (ForegroundGame.Map == null) return;

            UpdateBrush();

            Point pnt = new Point(InputHelper.mouseX, InputHelper.mouseY);

            bool canZoom = true;

            float realWheel = InputHelper.wheelDelta-deltaWheel;
            deltaWheel = InputHelper.wheelDelta;

            camera.ZoomLevel -= realWheel;
            if (camera.ZoomLevel < camera.minZoom) {
                camera.ZoomLevel = camera.minZoom;
                canZoom = false;
            } else if (camera.ZoomLevel > 50) {
                camera.ZoomLevel = 50;
            }

            bool freezeDestination = false;

            if (updateWaterShader && !InputHelper.leftDown) {
                TerrainWater.Initialize();
                updateWaterShader=false;
            }

            //------------------------------------------------------------------------------------------------ SELECTION STUFF 
            if ((currentTool==Tool.OBJECT_EDIT || currentTool==Tool.DECAL_EDIT) && InputHelper.altDown) {
                freezeDestination = true;
            } else if (currentTool!=Tool.NOTHING && InputHelper.altDown && InputHelper.rightDown) {
                if (!LastRightClick) {
                    clickPos = deltaPos = pnt;
                    lastBrushSize = CurrentBrushSize;
                    lastBrushSoftness = CurrentBrushSoftness;
                }
                float dx = pnt.X - deltaPos.X;
                float dy = pnt.Y - deltaPos.Y;
                float dist = 1;

                if (destination != new Vector3(0, -99999, 0)) {
                    dist = Vector3.Distance(destination, camera.eyePosition);
                    dist /= ClientSize.Width /2;
                }

                CurrentBrushSize = lastBrushSize + (dx * dist);

                if (CurrentBrushSize < 1) CurrentBrushSize = 1; else if (CurrentBrushSize > 200) CurrentBrushSize = 200;
                form.brushSize.Value = (int)CurrentBrushSize;

                CurrentBrushSoftness = lastBrushSoftness - (dy * dist);
                if (CurrentBrushSoftness < 0) CurrentBrushSoftness = 0; else if (CurrentBrushSoftness > 100) CurrentBrushSoftness = 100;
                form.brushSoftness.Value = (int)CurrentBrushSoftness;

                ChangeBrushProperties(CurrentBrushSize, CurrentBrushSoftness, CurrentBrushIntensity);
                freezeDestination = true;

            } else CameraInputs(pnt);

            Ray ray = CameraHelper.RayFromScreen(pnt.X, pnt.Y);
            if (!freezeDestination) destination = ForegroundGame.Map.Pick(ray, false);
            Vector2 size = new Vector2(CurrentBrushSize / 512);
            Vector2 textPos = new Vector2(destination.X, destination.Z) + new Vector2(-256  + 256 * (1-size.X));

            //------------------------------------------------------------------------------------------------ MODIFICATION AND TOOLS AND STUFF 

            if (InputHelper.leftDown) {

                if (currentTool == Tool.PATHFINDING) {
                    #region Pathfinding
                    int roundedSize = (int)Math.Ceiling(CurrentBrushSize / 2);
                    int destX = (int)Math.Round(destination.X, MidpointRounding.ToEven);
                    int destY = (int)Math.Round(destination.Z, MidpointRounding.ToEven);

                    int width = (int)Math.Ceiling((float)Terrain.info.width/2);

                    for (int x = -roundedSize; x < roundedSize; x++) {
                        for (int y = -roundedSize; y < roundedSize; y++) {
                            float power = (Vector2.Distance(Vector2.Zero, new Vector2(x, y)) / CurrentBrushSize) / 0.5f;

                            float limit = 1 - (CurrentBrushSoftness / 100);
                            power = (power - limit) / (1 - limit);
                            power = 1 - GameUtils.Clamp(power, 0, 1);

                            if (power > 0 && Terrain.InsideMap(destX + x, destY + y)) {
                                Point2D point = new Point2D((int)((destX + x)/2), (int)((destY + y)/2));//ForegroundGame.Map_Space.WorldToSpace(new Vector3());
                                Terrain.info.accessibilityArray[point.X + point.Y * width] = (byte)(CurrentBrushColor.Green == 0 ? 0 : 1);
                            }
                        }
                    }
                    #endregion
                } else if (currentTool == Tool.TEXTURE) {
                    #region Texture
                    Color4 colorAlphad = CurrentBrushColor;
                    colorAlphad.Alpha = CurrentBrushIntensity / 100;

                    Display.context.OutputMerger.SetTargets(drawableRenderTarget);
                    Display.context.ClearRenderTargetView(drawableRenderTarget, Color.Black);

                    QuadRenderer.Begin();
                    Display.context.OutputMerger.BlendState = DeviceStates.blendStateDrawing;
                    QuadRenderer.Draw(Terrain.info.TextureMap, new Vector2(0), Color.White);

                    QuadRenderer.Draw(CurrentBrush, textPos, size, colorAlphad);

                    QuadRenderer.End();

                    Display.context.OutputMerger.SetTargets(Display.depthStencil, Display.renderTarget);
                    GraphicsManager.Reset();

                    Texture2DDescription descHM = new Texture2DDescription() {
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        Width = Terrain.info.width,
                        Height = Terrain.info.height,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                    };

                    Terrain.info.TextureMap.Tex.Dispose();
                    Terrain.info.TextureMap.ressource.Dispose();
                    Terrain.info.TextureMap.Tex = new Texture2D(Display.device, descHM);
                    Display.context.CopyResource(drawableTexture, Terrain.info.TextureMap.Tex);
                    Terrain.info.TextureMap.RecreateSRV();
                    modifiedTextureMap=true;
                    #endregion
                } else if (currentTool == Tool.COLOR) {
                    #region Color
                    Color4 colorAlphad = CurrentBrushColor;
                    colorAlphad.Alpha = CurrentBrushIntensity / 100;

                    Display.context.OutputMerger.SetTargets(drawableRenderTarget);
                    Display.context.ClearRenderTargetView(drawableRenderTarget, Color.Black);

                    QuadRenderer.Begin();
                    Display.context.OutputMerger.BlendState = DeviceStates.blendStateDrawing;
                    QuadRenderer.Draw(Terrain.info.ColorMap, new Vector2(0), Color.White);

                    QuadRenderer.Draw(CurrentBrush, textPos, size, colorAlphad);

                    QuadRenderer.End();

                    Display.context.OutputMerger.SetTargets(Display.depthStencil, Display.renderTarget);
                    GraphicsManager.Reset();

                    Texture2DDescription descHM = new Texture2DDescription() {
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        Width = Terrain.info.width,
                        Height = Terrain.info.height,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                    };

                    Terrain.info.ColorMap.Tex.Dispose();
                    Terrain.info.ColorMap.Tex = new Texture2D(Display.device, descHM);
                    Display.context.CopyResource(drawableTexture, Terrain.info.ColorMap.Tex);
                    Terrain.info.ColorMap.RecreateSRV();
                    modifiedColorMap=true;
                    #endregion
                }else if (currentTool == Tool.LANDSCAPE_DOWN || currentTool == Tool.LANDSCAPE_UP || currentTool == Tool.LANDSCAPE_SET|| currentTool == Tool.LANDSCAPE_SMOOTH) {
                    #region Terraforming

                    if (!LastClick && currentTool == Tool.LANDSCAPE_SET && !form.customSetValue.Checked) form.levelTo.Value = (decimal)destination.Y;

                    int destX = (int)Math.Round(destination.X, MidpointRounding.ToEven);
                    int destY = (int)Math.Round(destination.Z, MidpointRounding.ToEven);
                

                    int roundedSize = (int)Math.Ceiling(CurrentBrushSize / 2);

                    if (currentTool == Tool.LANDSCAPE_SMOOTH) {
                        listForSmoothing = new List<float[]>(roundedSize*roundedSize*2*2);
                    }

                    for (int x = -roundedSize; x < roundedSize; x++) {
                        for (int y = -roundedSize; y < roundedSize; y++) {
                            float power = (Vector2.Distance(Vector2.Zero, new Vector2(x, y)) / CurrentBrushSize) / 0.5f;

                            float limit = 1 - (CurrentBrushSoftness/100);
                            power = (power-limit)/(1-limit);
                            power = 1- GameUtils.Clamp(power, 0, 1);

                            if (power > 0 && Terrain.InsideMap(destX + x, destY + y)) {

                                float height = Terrain.vertices[destX + x + (destY + y) * Terrain.info.width].Position.Y;

                                switch (currentTool) {
                                    case Tool.LANDSCAPE_UP:
                                        height += CurrentBrushIntensity / 100 * power;
                                        Terrain.ChangeHeight(destX + x, destY + y, height);
                                        break;
                                    case Tool.LANDSCAPE_DOWN:
                                        height -= CurrentBrushIntensity / 100 * power;
                                        Terrain.ChangeHeight(destX + x, destY + y, height);
                                        break;
                                    case Tool.LANDSCAPE_SET:
                                        Terrain.ChangeHeight(destX + x, destY + y, GameUtils.Lerp(height, HeightLevelSetBrush, power));
                                        break;
                                    case Tool.LANDSCAPE_SMOOTH:
                                        listForSmoothing.Add(new float[] { destX + x, destY + y, CurrentBrushIntensity / 100 * power });
                                        break;
                                }
                            }

                        }
                    }
                    if (currentTool == Tool.LANDSCAPE_SMOOTH) {
                        Terrain.SmoothHeight(listForSmoothing);
                    }

                    
                    DataStream stream;
                    Display.context.MapSubresource(Terrain.vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out stream);
                    stream.WriteRange(Terrain.vertices);
                    Display.context.UnmapSubresource(Terrain.vertexBuffer, 0);

                    updateWaterShader=true;
                #endregion
                }
                
            }


            LastClick = InputHelper.leftDown;
            LastMiddleClick = InputHelper.middleDown;
            LastRightClick = InputHelper.rightDown;
        }


        private void CameraInputs(Point pnt) {
            if (InputHelper.rightDown) {
                if (!LastRightClick) clickPos = deltaPos = pnt;
                float dx = pnt.X - deltaPos.X;
                deltaPos.X = pnt.X;
                float dy = pnt.Y - deltaPos.Y;
                deltaPos.Y = pnt.Y;
                dx *= 0.01f*camera.ZoomLevel;
                dy *= 0.01f*camera.ZoomLevel;

                camera.target.Z -= ((float)Math.Cos(camera.Yaw) * dy - (float)Math.Sin(camera.Yaw) * dx);
                camera.target.X -= ((float)Math.Sin(camera.Yaw) * dy + (float)Math.Cos(camera.Yaw) * dx);

                float camLimits = 10;
                if (camera.target.X < camLimits) camera.target.X = camLimits;
                if (camera.target.Z < camLimits) camera.target.Z = camLimits;
                if (camera.target.X > ForegroundGame.Map.width - camLimits) camera.target.X = ForegroundGame.Map.width - camLimits;
                if (camera.target.Z > ForegroundGame.Map.height - camLimits) camera.target.Z = ForegroundGame.Map.height - camLimits;
                ForegroundGame.Map.GetHeight(camera.target, ref camera.target.Y);
                if (camera.target.Y < ForegroundGame.Map.waterLevel) camera.target.Y = ForegroundGame.Map.waterLevel;
            } else if (InputHelper.middleDown) {
                if (!LastMiddleClick) clickPos = deltaPos = pnt;
                float dx = pnt.X - deltaPos.X;
                deltaPos.X = pnt.X;
                float dy = pnt.Y - deltaPos.Y;
                deltaPos.Y = pnt.Y;

                camera.Yaw -= dx * 0.00085f * GameUtils.Pi;
                camera.Pitch += dy * 0.00085f * GameUtils.Pi;
                if (camera.Pitch < 0) camera.Pitch = 0;
                if (camera.Pitch > (GameUtils.PiOver2 - 0.0001f)) camera.Pitch = (GameUtils.PiOver2 - 0.0001f);
            } else if (!InputHelper.middleDown) {
                if (LastMiddleClick) {
                    if (Vector2.Distance(new Vector2(clickPos.X, clickPos.Y), new Vector2(pnt.X, pnt.Y)) < 1) {
                        camera.interpolatedYaw= GameUtils.AngleNormalize(camera.interpolatedYaw);
                        camera.Yaw = 0;
                        camera.Pitch = GameUtils.PiOver2 * 0.6f;
                    }
                }
            }
        }

        private void UpdateBrush() {

            if (updateBrush) {

                Display.context.OutputMerger.SetTargets(brushRenderTarget);
                Display.context.ClearRenderTargetView(brushRenderTarget, Color.Transparent);

                QuadRenderer.Begin();
                Display.context.OutputMerger.BlendState = DeviceStates.blendStateDrawing;

                int quality = 85;
                Color4 color = new Color4(1, 1, 1, 1);
                color.Alpha=0.012f;

                float size = 1;
                for (int i = 1; i <= quality; i++) {
                    size = ((100.0f-CurrentBrushSoftness)/100)+ ((float)i / quality) * (CurrentBrushSoftness/100);
                    QuadRenderer.Draw(DefaultBrush, new Vector2(256*(1-size)), new Vector2(size), color);
                }

                QuadRenderer.End();

                Display.context.OutputMerger.SetTargets(Display.depthStencil, Display.renderTarget);
                GraphicsManager.Reset();

                CurrentBrush.ressource = new ShaderResourceView(Display.device, brushTexture);

                Display.context.CopyResource(brushTexture, brushTextureReadable);

                Surface surf=brushTextureReadable.QueryInterface<Surface>();

                DataStream streamN;
                DataRectangle rect=surf.Map(SharpDX.DXGI.MapFlags.Read, out streamN);
                streamN.Position=0;
                 
                byte[] textureData = new byte[512 * 512 * 4];
                streamN.ReadRange(textureData, 0, textureData.Length);
                
                surf.Unmap();

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(
                               512, 512,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb
                             );

                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(
                               new System.Drawing.Rectangle(0, 0, 512, 512),
                               System.Drawing.Imaging.ImageLockMode.WriteOnly,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb
                             );

                IntPtr safePtr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
                bmp.UnlockBits(bmpData);

                form.brushPreview.Image = bmp;
                form.brushPreview.SizeMode = PictureBoxSizeMode.Zoom;


                updateBrush = false;
            }
        }

        protected override void WinRender() {
            if (loading) return;

            GlobalConstantVars.Time++;

            camera.Update();


            GlobalConstantVars.Time+=1;

            Display.context.ClearDepthStencilView(Display.depthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);
            Display.context.ClearRenderTargetView(Display.renderTarget, new Color(ForegroundGame.Map.waterColor));

            SceneManager.Camera.Update();
            // Game World -------------------------------------


            // Draw tool
            if (currentTool!=Tool.NOTHING) {
                DrawGroundCircle(destination, CurrentBrushSize*0.5f, CurrentBrushColor);
                DrawGroundCircle(destination, GameUtils.Max(CurrentBrushSize*0.5f*(1-(CurrentBrushSoftness*0.01f)),0.5f), new Color4(CurrentBrushColor.Red, CurrentBrushColor.Green, CurrentBrushColor.Blue, 0.5f));
            }

            int width = (int)Math.Ceiling((float)ForegroundGame.Map.width/2);
            int height = (int)Math.Ceiling((float)ForegroundGame.Map.height/2);

            if (currentTool==Tool.PATHFINDING) {
                for (int x=0; x<=(width-1); x++) {
                    for (int y=0; y<=(height-1); y++) {
                        int id = x  + (y * width);
                        if (ForegroundGame.Map.accessibilityArray[id]==0) ForegroundGame.TerrainDebug.Add(new Vector2(x*2, y*2), new Color4(1, 0, 0, 0.3f));
                    }
                }
            }
            
            if (showCompass!=0) {
                showCompass--;
                SceneManager.LineManager.AddSunCompass(camera.target + new Vector3(0, 0.5f, 0), GlobalConstantVars.SunDirection, 10);
            }


            SceneManager.Render();


            ScreenshotManager.Render();
        }


        private void DrawGroundCircle(Vector3 pos, float size, Color4 color) {
            // Compute our step around each circle
            float divide = 10+(int)Math.Ceiling((size/100)*60);
            float step = GameUtils.TwoPi / divide;

            // Next on the XZ plane
            for (float a = 0f; a <= GameUtils.TwoPi; a += step) {
                Vector3 from = pos + new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)) * size;
                Vector3 to = pos + new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step)) * size;

                ForegroundGame.Map.GetHeight(from, ref from.Y, 0.5f);
                ForegroundGame.Map.GetHeight(to, ref to.Y, 0.5f);

                SceneManager.LineManager.AddLine(from, to, color);
            }
        }



        public void TextureBrushEnable(int index) {
            switch (index) {
                case 0:
                    CurrentBrushColor.Red   = 1;
                    CurrentBrushColor.Green = 0;
                    CurrentBrushColor.Blue  = 0;
                    break;
                case 1:
                    CurrentBrushColor.Red   = 0;
                    CurrentBrushColor.Green = 1;
                    CurrentBrushColor.Blue  = 0;
                    break;
                case 2:
                    CurrentBrushColor.Red   = 0;
                    CurrentBrushColor.Green = 0;
                    CurrentBrushColor.Blue  = 1;
                    break;
                case 3:
                    CurrentBrushColor.Red   = 0;
                    CurrentBrushColor.Green = 0;
                    CurrentBrushColor.Blue  = 0;
                    break;
            }
            ChangeBrushProperties(CurrentBrushSize, CurrentBrushSoftness, CurrentBrushIntensity);
            //CurrentBrushColor.A = 255;
        }

        public void ChangeBrushProperties(float size, float softness, float intensity) {

            CurrentBrushSize = size;
            CurrentBrushSoftness = softness;
            CurrentBrushIntensity = intensity;

            updateBrush = true;
        }


        public string model_loaded = null;
        public void AddModel(string file) {
               model_loaded = file;
        }


        /*public bool isCopyAFoliage = false;
        public string copiedPath = null;
        private ModelPointer copiedObj = null;*/

        public void CopyPasteObject(bool copy) {
            /*if (copy) {
                if (selectedObject != null) {
                    copiedPath = selectedObject.file;
                    copiedObj = new ModelPointer(selectedObject.model.pointer, isCopyAFoliage);
                    copiedObj.Position = selectedObject.model.Position;
                    copiedObj.Orientation = selectedObject.model.Orientation;
                    copiedObj.Scale = selectedObject.model.Scale;
                    isCopyAFoliage = selectedObject.instanced;
                }
            } else {
                if (copiedObj==null) return;
                copiedObj.ApplyMaterialChanges();
                MapModel mod = new MapModel(copiedPath, copiedObj,isCopyAFoliage);
                MainWindow.mapFile.Models.Add(mod);
                Map.models.Add(copiedObj);
                selectedObject = mod;
                copiedObj = null;
            }*/
        }

        public static float UnpackUNorm(uint bitmask, uint value) {
            value &= bitmask;
            return (float)value / (float)bitmask;
        }
        public static Color4 ColorHSV(float h, float s, float v, float a) {
            float fh = h * 6.0f;
            float f = fh - (float)Math.Floor(fh);

            float p = v * (1.0f - s);
            float q = v * (1.0f - f * s);
            float t = v * (1.0f - (1.0f - f) * s);

            int ih = (int)fh;
            switch (ih) {
                case 0:
                    return new Color4(a, v, t, p);
                case 1:           
                    return new Color4(a, q, v, p);
                case 2:            
                    return new Color4(a, p, v, t);
                case 3:          
                    return new Color4(a, p, q, v);
                case 4:           
                    return new Color4(a, t, p, v);
                default:         
                    return new Color4(a, v, p, q);
            }
        }

    }


}
