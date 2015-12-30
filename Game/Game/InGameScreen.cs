using RageEngine;
using RageEngine.ContentPipeline;
using RageEngine.Debug;
using RageEngine.Graphics;
using RageEngine.Graphics.ScreenManager;
using RageEngine.Graphics.TwoD;
using RageEngine.Input;
using RageEngine.LQDB;
using RageEngine.Rendering;
using RageRTS.Map;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rectangle = System.Drawing.Rectangle;

namespace RageRTS {

    public static class DEBUG_VALUES {
        public static bool 
            ShowLQDB = false, 
            ShowTerrainAccessibility = true,
            ShowCorners = true;
    }

    class InGameScreen: Screen {
        public InGameScreen() : base("Game") { }

        public override void Initialize() {
            AddLayer(new InGameLayer());
        }
    }

    class InGameLayer: ScreenLayer {

        public static InGameLayer instance;

        public TopDownCamera camera;

        public Terrain terrain;

        private bool shiftMode, altMode, controlMode;
        private bool buildOn,selectionOn;
        private Vector2 selectionStart;
        private Rectangle selectionRectangle = new Rectangle();

        private InteractiveObject2D focusedObject;
        private List<InteractiveObject2D> dragSelection   = new List<InteractiveObject2D>(1000);
        private List<InteractiveObject2D> selectedObjects = new List<InteractiveObject2D>(1000);


        public override void Initialize() {
            instance = this;

            ShaderManager.Add("VS_Debug", Resources.GetShader("VS", "VS", "Debug.fx")).SetInputLayout(VertexColor.Elements);
            ShaderManager.Add("PS_Debug", Resources.GetShader("PS", "PS", "Debug.fx"));

            ShaderManager.Add("VS_UI3D", Resources.GetShader("VS", "VS", "UI3D.fx")).SetInputLayout(VertexTextureColor.Elements);
            ShaderManager.Add("PS_UI3D", Resources.GetShader("PS", "PS", "UI3D.fx"));

            ShaderManager.Add("VS_Model", Resources.GetShader("VS", "VS", "Model.fx")).SetInputLayout(VertexTextureColor.Elements);
            ShaderManager.Add("PS_Model", Resources.GetShader("PS", "PS", "Model.fx"));

            ShaderManager.Add("VS_Terrain", Resources.GetShader("VS", "VS", "Terrain.fx")).SetInputLayout(VertexTexture.Elements);
            ShaderManager.Add("PS_Terrain", Resources.GetShader("PS", "PS", "Terrain.fx"));

            ShaderManager.Add("VS_Water", Resources.GetShader("VS", "VS", "Water.fx")).SetInputLayout(VertexTexture.Elements);
            ShaderManager.Add("PS_Water", Resources.GetShader("PS", "PS", "Water.fx"));


            SceneManager.Initialize();

            InputManager.AssignKey("up", new[] { "Z" });
            InputManager.AssignKey("down", new[] { "S" });
            InputManager.AssignKey("left", new[] { "Q" });
            InputManager.AssignKey("right", new[] { "D" });

            InputManager.AssignKey("zoom_in", new[] { "Add" });
            InputManager.AssignKey("zoom_out", new[] { "Subtract" });

            //---------------------------------------------------------------------------------------------------
            RawMap mapFile;
            string mapname="(2) Germanium Island";
            mapFile=RawMap.Load(mapname);

            ForegroundGame.Map=new TerrainInfo();

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

            ForegroundGame.Map.ColorMap=Resources.GetTexture("Maps/"+mapname+"/_colorMap.png", false);
            ForegroundGame.Map.TextureMap=Resources.GetTexture("Maps/"+mapname+"/_textureMap.png", false);

            ForegroundGame.Map.Shore = RageEngine.ContentPipeline.Resources.GetTexture("Maps/Shared/shore.jpg");
            ForegroundGame.Map.Clouds = RageEngine.ContentPipeline.Resources.GetTexture("Maps/Shared/clouds.jpg");

            string[] texturesWithNormal=new string[mapFile.Textures.Length*2];
            for (uint i = 0; i < mapFile.Textures.Length; i++) {
                texturesWithNormal[i*2]="Maps/Shared/"+mapFile.Textures[i];
                texturesWithNormal[i*2+1]="Maps/Shared/"+mapFile.Textures[i]+"_bump";
            }
            ForegroundGame.Map.TextureArray=Resources.GetTextureArray(texturesWithNormal);

            ForegroundGame.Map.Load();

            var Map_Dimensions = new Point3D(ForegroundGame.Map.width, 1000, ForegroundGame.Map.height);
            var Map_Division = new Point3D(100, 1, 100);

            BackgroundGame.IntObjsLQDB = new LocalityQueryProximityDatabase<InteractiveObject>(Map_Dimensions, Map_Division);

            BackgroundGame.MapSpace = new MapSpace(ForegroundGame.Map);

            terrain = new Terrain(ForegroundGame.Map);
            SceneManager.Add(terrain);
            ForegroundGame.TerrainDebug = new TerrainDebug(ForegroundGame.Map);
            SceneManager.Add(ForegroundGame.TerrainDebug);
            SceneManager.Add(new TerrainWater(ForegroundGame.Map));
            //--------------------------------------------------------------------------------------------------

            ForegroundGame.startegicIcons = Resources.GetTexture("Textures/startegicIcon.png", false);
            ForegroundGame.selectionBrackets = Resources.GetTexture("Textures/selectionBrackets.png", false);
            ForegroundGame.unitPlaceholder = Resources.GetTexture("Textures/unitplaceholder.png", false);
            

            camera = new TopDownCamera();
            SceneManager.Camera=camera;

            uitest = new WebWindow(200, Display.Height, "UI/InGame/index.html");
            uitest.position.X=Display.Width-200;

            uitest.Bind("GlobalBuildClick", _globalBuildClick);
            uitest.Bind("BuildClick", _globalBuildClick);

            InputManager.MouseDown+=new Action<MouseInputEventArgs>(Input_MouseDown);
            InputManager.MouseUp+=new Action<MouseInputEventArgs>(Input_MouseUp);
            InputManager.MouseMove+=new Action<MouseInputEventArgs>(Input_MouseMove);
            InputManager.Wheel+=new Action<MouseInputEventArgs>(Input_Wheel);

            
            ForegroundGame.IntObjsManager = new InteractiveObject2DManager();

            SceneManager.Add(ForegroundGame.IntObjsManager);




            BlueprintIntObject.Initialize();
            //BlueprintProjectile.Initialize("Scripts/Projectiles/");


			int column = 0,line = 0;
			for (int i = 0; i < 200; i++) {
				if (line > 17) { column++; line = 0; }
                InteractiveObject obj = BackgroundGame.Scripts_IObjects.Make<InteractiveObject>("Engy", new Type[2] { typeof(InteractiveObject), typeof(BlueprintIntObject) }, null, BlueprintIntObject.Find("Engy"));//new Engy(null,BlueprintIntObject.list);
                obj.Init(0, new FVector2((fint)(150 + column * 2), (fint)(100 + line * 2)), 0);
				BackgroundGame.IntObjs.Add(obj);
				line++;
            }
            camera.ZoomLevel = 50;
            camera.Position=new Vector2(170, 120);

            Task.Factory.StartNew(UpdateThread, TaskCreationOptions.LongRunning);
            ForegroundGame.timer.Start();
        }


        void UpdateThread() {

            Stopwatch timer = new Stopwatch();
            timer.Start();
            double accumulatedTime=0, lastTime=0;

            while (Global.IsRunning) {
                double time = timer.Elapsed.TotalSeconds;
                accumulatedTime += (time - lastTime);
                lastTime = time;

                const double frameTime = 0.10; // frames per second
                while (accumulatedTime > frameTime) {
                    accumulatedTime -= frameTime;
                    //world.Step((float)frameTime);

                    lock (Display.device) {
                        BackgroundGame.GameTick++;

                        DebugPerfMonitor.BeginMark(0, "Update", Color.White);

                        /*NetOutgoingMessage om = Main.network.CreateMessage(NetworkDataType.SyncUpdate);
                        om.Write((Int32)(BackgroundGame.GameTick + 2));
                        om.Write((Int32)1);
                        foreach (byte[] order in BackgroundGame.OrderListToSend) {
                            byte[] data = order;
                            om.Write((byte)data.Length);
                            om.Write(data);
                        }
                        Main.network.SendSyncMessage(om);*/

                        foreach (byte[] o in BackgroundGame.OrderListToSend) {
                            Order.DecodeAndExecute(0, o, 0, o.Length);
                        }
                        BackgroundGame.OrderListToSend.Clear();

                        for (int i = BackgroundGame.IntObjs.Count - 1; i >= 0; i--) BackgroundGame.IntObjs[i].Update();
                        for (int i = BackgroundGame.IntObjs.Count - 1; i >= 0; i--) BackgroundGame.IntObjs[i].Prepare();

                        DebugPerfMonitor.EndMark(0, "Update");

                        BackgroundGame.TimeInterpolateDelta = timer.Elapsed.TotalSeconds - BackgroundGame.TimeInterpolateLast;
                        BackgroundGame.TimeInterpolateLast = timer.Elapsed.TotalSeconds;
                        ForegroundGame.timer.Restart();
                    }
                }
            }
            

            /*//if (float.IsNaN(minGameSpeed) || minGameSpeed < 100) minGameSpeed = 100;
            if (BackgroundGame.GameTick > BackgroundGame.GameTick_Last) continue;
            if (BackgroundGame.updateTimer.ElapsedMilliseconds < BackgroundGame.timeMax) continue;

            BackgroundGame.updateTimer.Restart();

            //BackgroundGame.timeMax = (int)BackgroundGame.updateTimer.ElapsedMilliseconds;
            ForegroundGame.timeInterpolateFull = ForegroundGame.timeInterpolate+1;
            ForegroundGame.timeInterpolate = 0;

            lock (Global.device) {
                BackgroundGame.GameTick++;

                SyncBucket timeBucket;
                if (Main.network.ticks.TryGetValue(BackgroundGame.GameTick, out timeBucket)) {
                    foreach (SyncData sdata in timeBucket.rawdata) {
                        int i = 0;
                        int length;
                        byte[] rdata = sdata.data;
                        while (i < rdata.Length) {
                            length = (int)rdata[i++];
                            Order.DecodeAndExecute(sdata.playerId, rdata, i, length);
                            i += length;
                            if (length == 0) break;
                        }
                    }
                }

                NetOutgoingMessage om = Main.network.CreateMessage(NetworkDataType.SyncUpdate);
                om.Write((Int32)(BackgroundGame.GameTick + 2));
                om.Write((Int32)1);
                foreach (byte[] order in BackgroundGame.OrderList) {
                    byte[] data = order;
                    om.Write((byte)data.Length);
                    om.Write(data);
                }
                Main.network.SendSyncMessage(om);
                BackgroundGame.OrderList.Clear();

                DebugPerfMonitor.BeginMark(1, "Update", Color.White);

                BackgroundGame.Map_Space.Update();

                DebugPerfMonitor.BeginMark(1, "Interactive Objects Update", Color.Orange);
                for (int i = BackgroundGame.IObjs.Count - 1; i >= 0; i--) BackgroundGame.IObjs[i].Update();
                DebugPerfMonitor.EndMark(1, "Interactive Objects Update");

                DebugPerfMonitor.BeginMark(1, "Intel Update", Color.Blue);
                BackgroundGame.intelManager.Update();
                DebugPerfMonitor.EndMark(1, "Intel Update");

                DebugPerfMonitor.BeginMark(1, "Resources Update", Color.Green);
                for (int i = BackgroundGame.ResObjs.Count - 1; i >= 0; i--) BackgroundGame.ResObjs[i].Update();
                DebugPerfMonitor.EndMark(1, "Resources Update");

                DebugPerfMonitor.BeginMark(1, "Projectiles Update", Color.Red);
                for (int i = BackgroundGame.Projectiles.Count - 1; i >= 0; i--) {
                    if (BackgroundGame.Projectiles[i].Update()) BackgroundGame.Projectiles.RemoveAt(i);
                }
                DebugPerfMonitor.EndMark(1, "Projectiles Update");

                if (BackgroundGame.async_IObjs.Count != 0) {
                    for (int i = BackgroundGame.async_IObjs.Count; i > 0; i--) {
                        InteractiveObject obj = BackgroundGame.async_IObjs.Pop();
                        if (obj == null) continue;
                        if (!obj.Initialized)
                            BackgroundGame.IObjs.Remove(obj);
                        else
                            BackgroundGame.IObjs.Add(obj);
                    }
                }

                DebugPerfMonitor.EndMark(1, "Update");
            }

            BackgroundGame.GameSpeed = BackgroundGame.GameSpeed_To; // Change it next turn
            */

        }


        public override void Resize() {
            uitest.Resize(200, Display.Height);
            uitest.position.X=Display.Width-200;
        }

        WebWindow uitest;
        void _globalBuildClick(object s, Awesomium.Core.JavascriptMethodEventArgs e) {
            int id = (int)e.Arguments[0];
            id = id;
            buildOn=true;
        }

        void Input_Wheel(MouseInputEventArgs e) {
            if (e.Handled) return;
            camera.Input_Wheel(null, e);
        }
        void Input_MouseDown(MouseInputEventArgs e) {
            if (e.Handled) return;

            if (!buildOn && !selectionOn && e.IsLeftButtonDown) {
                selectionOn = true;
                selectionStart.X = e.X;
                selectionStart.Y = e.Y;
                selectionRectangle.X = e.X;
                selectionRectangle.Y = e.Y;
                selectionRectangle.Width = selectionRectangle.Height = 0;
                Input_MouseMove(e);
            }

            if (!selectionOn) camera.Input_MouseDown(null, e);
        }
        void Input_MouseUp(MouseInputEventArgs e) {
            var cameraAngleChanged = camera.ChangingCameraAngle;
            camera.Input_MouseUp(null, e);

            shiftMode = InputManager.IsShiftDown();
            controlMode = InputManager.IsControlDown();
            altMode = InputManager.IsAltDown();

            if (cameraAngleChanged) {
                selectionOn = false;
                return;
            }

            if (buildOn) {
                selectionOn = false;

                if (!controlMode) buildOn = false;
                return;
            }

            if (selectionOn && !e.IsLeftButtonDown) {
                selectionOn = false;
                
                // If not shift or alt mode, deselect all previously selected units
                if (!shiftMode && !altMode && selectedObjects.Count != 0) {
                    foreach (InteractiveObject2D obj in selectedObjects) /*if (obj.side != Main.network.me.id)*/ obj.selected = false;
                    selectedObjects.Clear();
                }

                if (dragSelection.Count != 0) {

                    //debugText = "Current Type: " + dragSelection[0].entity.GetType();

                   /* if (focusedObject != null && focusedObject.side != Main.network.me.id) {  // Enemy seleciton else -> My units selection
                        foreach (InteractiveObject2D obj in selectedObjects) obj.selected = false;
                        selectedObjects.Clear();
                        focusedObject.focused = false;
                        focusedObject.selected = true;
                        selectedObjects.Add(focusedObject);
                    } else*/ 
                    {

                        if (altMode) {
                            var itemsToBeRemoved = new List<int>();

                            for (int i = selectedObjects.Count - 1; i >= 0; i--) {
                                InteractiveObject2D obj  = selectedObjects[i];
                                obj.focused = false;
                                obj.selected = false;
                                if (!selectedObjects.Contains(obj)) itemsToBeRemoved.Add(i);
                            }
                            for (int i = 0; i < itemsToBeRemoved.Count; i++) dragSelection.RemoveAt(itemsToBeRemoved[i]);
                            
                        } else {
                            if (e.IsDoubleClick && focusedObject != null) {
                                dragSelection.Clear();
                                ForegroundGame.IntObjsManager.Select(focusedObject, ref dragSelection);
                            }

                            int baseAmount = 0, junkAmount = 0, restAmount = 0;
                            foreach (InteractiveObject2D obj in dragSelection) {
                                //if (obj.side != Main.network.me.id) continue;
                                if (obj.entity.Type == IntObjType.BASE) baseAmount++;
                                else if (obj.entity.Type == IntObjType.WALL) junkAmount++;
                                else restAmount++;
                                obj.focused = false;
                                obj.selected = true;
                                if (!selectedObjects.Contains(obj)) /*if (obj.entity.Health == 0 || obj.side == Main.network.me.id)*/ selectedObjects.Add(obj);
                            }
                            if (dragSelection.Count != 1) {
                                var itemsToBeRemoved = new List<int>();

                                if (restAmount>0 && baseAmount >= 0 && junkAmount>=0) {
                                    for (int i = selectedObjects.Count - 1; i >= 0; i--) {
                                        if (selectedObjects[i].entity.Type == IntObjType.BASE || selectedObjects[i].entity.Type == IntObjType.WALL) {
                                            selectedObjects[i].selected = false;
                                            itemsToBeRemoved.Add(i);
                                        }
                                    }
                                } else if (baseAmount > 0 && restAmount==0 && junkAmount>0) {
                                    for (int i = selectedObjects.Count - 1; i >= 0; i--) {
                                        if (selectedObjects[i].entity.Type == IntObjType.WALL) {
                                            selectedObjects[i].selected = false;
                                            itemsToBeRemoved.Add(i);
                                        }
                                    }
                                }

                                for (int i = 0; i < itemsToBeRemoved.Count; i++) selectedObjects.RemoveAt(itemsToBeRemoved[i]);
                            }
                        }
                    }

                    dragSelection.Clear();
                }
                RefreshSelection();
                return;
            }

            if (!selectionOn && e.Button == System.Windows.Forms.MouseButtons.Right) {
                if (selectedObjects.Count != 0) {
                    RefreshSelection();
                    uint[] iobjects = new uint[selectedObjects.Count];
                    for (int i = selectedObjects.Count - 1; i >= 0; i--) iobjects[i] = selectedObjects[i].entity.id;

                    // AN OBJECT IS FOCUSED, COULD THIS BE AN ATTACK OR ASSIST ? ------------------
                    if (focusedObject != null) {
                        bool notFromSelection = !selectedObjects.Contains(focusedObject);
                        if (notFromSelection) {
                            /*if (focusedObject.entity.side == Main.network.me.id && !controlMode) {
                                Order_Assist newOrder = new Order_Assist();
                                newOrder.Init(focusedObject.entity);
                                newOrder.caller = iobjects;
                                newOrder.overwrite = !multiTaskMode;
                                lock (Global.device) BackgroundGame.OrderList.Add(newOrder.Encode());
                            } else {
                                Order_Target newOrder = new Order_Target();
                                newOrder.Init(focusedObject.entity);
                                newOrder.caller = iobjects;
                                newOrder.overwrite = !multiTaskMode;
                                lock (Global.device) BackgroundGame.OrderList.Add(newOrder.Encode());
                            }*/

                            return;
                        }
                    }

                    // MOVE COMMAND -------------------------------

                    Ray ray = CameraHelper.RayFromScreen(e.X, e.Y);
                    Vector3 destination = ForegroundGame.Map.Pick(ray);

                    Order_Move newOrderM = new Order_Move();
                    newOrderM.Init(new FVector2((fint)destination.X, (fint)destination.Z));
                    newOrderM.overwrite = !shiftMode;
                    newOrderM.caller = iobjects;
                    BackgroundGame.OrderListToSend.Add(newOrderM.Encode());
                }
            }
        }
        void Input_MouseMove(MouseInputEventArgs e) {

            camera.Input_MouseMove(null, e);

            if (selectionOn) {
                bool multi_select = false;
                if (Vector2.Distance(selectionStart, new Vector2(e.X, e.Y)) > 5) multi_select = true;
                float left = selectionStart.X, right = e.X, top = selectionStart.Y, bottom = e.Y;

                float temp;
                if (left > right) {
                    temp = right;
                    right = left;
                    left = temp;
                }
                if (top > bottom) {
                    temp = top;
                    top = bottom;
                    bottom = temp;
                }
                selectionRectangle.X = (int)left;
                selectionRectangle.Y = (int)top;
                selectionRectangle.Width = (int)(right - left);
                selectionRectangle.Height = (int)(bottom - top);

                foreach (InteractiveObject2D obj in dragSelection) obj.focused = false;

                dragSelection.Clear();
                if (!multi_select) {
                    InteractiveObject2D obj = ForegroundGame.IntObjsManager.Select(new Vector2(e.X, e.Y));
                    if (obj!=null) dragSelection.Add(obj);
                } else {
                    ForegroundGame.IntObjsManager.Select(ref selectionRectangle, ref dragSelection);
                    if (focusedObject != null) focusedObject.focused = false;
                    focusedObject = null;
                }
                if (dragSelection.Count != 0) {
                    foreach (InteractiveObject2D obj in dragSelection) {
                        //if (obj.side != Main.network.me.id) continue;
                        obj.focused = true;
                    }
                }

                return;
            }
            if (focusedObject != null) focusedObject.focused = false;
            focusedObject = ForegroundGame.IntObjsManager.Select(new Vector2(e.X, e.Y));
            if (focusedObject != null) focusedObject.focused = true;

        }

        public void RefreshSelection() {

            // List has been modified error when selecting already selected objects with new ones
            if (selectedObjects.Count!=1) {
                var itemsToBeRemoved = new List<int>();

                for (int i = selectedObjects.Count - 1; i >= 0; i--) {
                    InteractiveObject2D obj  = selectedObjects[i];
                    if (obj.entity.Health == 0/* || obj.side != Main.network.me.id*/) {
                        obj.focused = false;
                        obj.selected = false;
                        itemsToBeRemoved.Add(i);
                    }
                }
                for (int i = 0; i < itemsToBeRemoved.Count; i++) selectedObjects.RemoveAt(itemsToBeRemoved[i]);
            }
        }



        public override void Update() {

            GlobalConstantVars.Time+=1;

            //BackgroundGame.TimeInterpolateDelta = (float)(ForegroundGame.timer.Elapsed.TotalSeconds % (BackgroundGame.TimeInterpolate - BackgroundGame.TimeInterpolateLast));

            SceneManager.Camera.Update();

	        //GamePhysicsUtils::LerpState(interpolatedState, prevState, gameState, t);


            /*if (InputManager.IsDown("up")) Camera2D.Y+=1f;
            if (InputManager.IsDown("down")) Camera2D.Y-=1f;
            if (InputManager.IsDown("left")) Camera2D.X-=1f;
            if (InputManager.IsDown("right")) Camera2D.X+=1f;*/


            /*int radius = (int)GameUtils.Min(100, (500f/Camera2D.Zoom));
            for (int x = -radius; x < radius; x++) {
                for (int y = -radius; y < radius; y++) {
                    int xx = (int)player.position.X + x;
                    int yy = (int)player.position.Y + y;
                    if ((xx)%2!=0 || (yy)%2!=0 || (x*x+y*y)>radius*radius) continue;
                    if (!Map.CollisionPoint(xx,yy)) continue;
                    dotMgr.AddDot(new Vector2(xx, yy), new Color4(0, 1, 1, 0.5f));
                }
            }*/

            //dotMgr.AddDot(new Vector2(Camera2D.X, Camera2D.Y), new Color4(0, 1, 0, 1));

            //lineMgr.AddLine(new Vector2(0,0),new Vector2(Camera2D.X,Camera2D.Y),new Color4(1,0,1,1));
            //Camera2D.X=0;
            //Camera2D.Y=0;


        }


        public override void Render() {

            // Game World -------------------------------------

            /*SceneManager.LineManager.AddArrow(Vector3.Zero, new Vector3(10, 0, 0), Color.Red, true);
            SceneManager.LineManager.AddArrow(Vector3.Zero, new Vector3(0, 10, 0), Color.Green, true);// Z-Up style arrows
            SceneManager.LineManager.AddArrow(Vector3.Zero, new Vector3(0, 0, 10), Color.Blue, true);
            */

            lock (Display.device) {

                GlobalConstantVars.Time+=1;
                BackgroundGame.TimeInterpolate = (float)(ForegroundGame.timer.Elapsed.TotalSeconds / BackgroundGame.TimeInterpolateDelta);

                if (DEBUG_VALUES.ShowTerrainAccessibility) {
                    for (int x=0; x<=(BackgroundGame.MapSpace.Width-1); x++) {
                        for (int y=0; y<=(BackgroundGame.MapSpace.Height-1); y++) {
                            int id = x  + (y * BackgroundGame.MapSpace.Width);
                            if (ForegroundGame.Map.accessibilityArray[id]==0)
                                ForegroundGame.TerrainDebug.Add(new Vector2(x*2, y*2), new Color4(1, 0, 0, 0.5f));
                        }
                    }
                }

                if (DEBUG_VALUES.ShowLQDB) {
                    float stepX, stepY;
                    stepX = BackgroundGame.IntObjsLQDB.dimensions.X / BackgroundGame.IntObjsLQDB.divisions.X;
                    stepY = BackgroundGame.IntObjsLQDB.dimensions.Z / BackgroundGame.IntObjsLQDB.divisions.Z;
                    BoundingBox newb = new BoundingBox();
                    for (int x = 0; x < BackgroundGame.IntObjsLQDB.divisions.X; x++) {
                        for (int y = 0; y < BackgroundGame.IntObjsLQDB.divisions.Z; y++) {
                            newb.Minimum=new Vector3(x * stepX, 0, y * stepY);
                            newb.Maximum=new Vector3((x+1) * stepX, ForegroundGame.Map.maxAltitude, (y+1) * stepY);
                            if (camera.Frustum.Contains(ref newb)!=ContainmentType.Disjoint) SceneManager.LineManager.AddBoundingBox(newb.Minimum, newb.Maximum, Color.Red);
                        }
                    }
                }

                if (DEBUG_VALUES.ShowCorners) {
                    Vector3 dir = new Vector3(0);
                    for (int x = 0; x < BackgroundGame.MapSpace.Width; x++) {
                        for (int y = 0; y < BackgroundGame.MapSpace.Height; y++) {
                            byte bitmask = BackgroundGame.MapSpace.pathfinding[x + y * BackgroundGame.MapSpace.Width];

                            Vector3 tmppos = new Vector3(1 + x * 2, 0, 1 + y * 2);
                            ForegroundGame.Map.GetHeight(tmppos, ref tmppos.Y);
                            tmppos.Y+=0.5f;
                            if (SceneManager.Camera.Frustum.Contains(ref tmppos) != ContainmentType.Contains) continue;

                            if (bitmask == 0) continue;
                            if (bitmask == 255) continue;

                            for (int i = 0; i < 4; i++) {
                                if ((bitmask & MapSpace.DirectionDiagonalBlock[i]) == 0) {
                                    ForegroundGame.TerrainDebug.Add(new Vector2(x*2, y*2), new Color4(0, 0, 1, 0.4f));
                                }
                            }

                            /*for (int i = 0; i < 8; i++) {
                                if ((bitmask & (1 << i)) != (1 << i)) continue;

                                dir.X = MapSpace.Direction[i, 0];
                                dir.Z = MapSpace.Direction[i, 1];

                                int tmpX = x + (int)dir.X;
                                int tmpY = y + (int)dir.Z;
                                if (tmpX < 0 || tmpY < 0 || tmpX >= BackgroundGame.MapSpace.Width || tmpY >= BackgroundGame.MapSpace.Height) continue;
                                int tmpId = tmpX + tmpY * BackgroundGame.MapSpace.Width;

                                if (BackgroundGame.MapSpace.pathfinding[tmpId] < 255)
                                    SceneManager.LineManager.AddLine(tmppos, new Color4(0, 1, 0, 1), tmppos + dir, new Color4(1, 0, 0, 1));
                            }*/
                        }
                    }
                }



                if (buildOn) {
                    Ray ray = CameraHelper.RayFromScreen(InputManager.MouseState.X, InputManager.MouseState.Y);
                    Vector3 destination = ForegroundGame.Map.Pick(ray, false);

                    if (destination.Y != -99999) {
                        Point2D p = BackgroundGame.MapSpace.WorldToSpace(destination);
                        ForegroundGame.TerrainDebug.Add(new Vector2(p.X*2, p.Y*2), new Color4(0, 1, 0, 0.4f));
                    }
                }

                //BackgroundGame.intelManager.Render();
                for (int i = ForegroundGame.IntObjs3D.Count - 1; i >= 0; i--) ForegroundGame.IntObjs3D[i].Render();
                //for (int i = BackgroundGame.Projectiles.Count - 1; i >= 0; i--) BackgroundGame.Projectiles[i].Render();
                //for (int i = BackgroundGame.ResObjs.Count - 1; i >= 0; i--) BackgroundGame.ResObjs[i].Render();

                SceneManager.Render();
                InteractiveObject3D.RenderAll();

                // Interface -------------------------------------
                QuadRenderer.Begin();//(GC.GetTotalMemory(false)/1000000f).ToString("0.00 mb")
                ForegroundGame.IntObjsManager.StrategicMode = camera.StrategicView;
                ForegroundGame.IntObjsManager.Render2D();
                if (selectionOn && (selectionRectangle.Width > 2 || selectionRectangle.Height > 2)) {
                    QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width, selectionRectangle.Height), new Color4(1, 1, 1, 0.15f));
                    QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(selectionRectangle.X - 2, selectionRectangle.Y - 2, selectionRectangle.Width + 4, 2), new Color4(0, 0, 0, 0.7f));
                    QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(selectionRectangle.X - 2, selectionRectangle.Y, 2, selectionRectangle.Height + 2), new Color4(0, 0, 0, 0.7f));
                    QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(selectionRectangle.Right, selectionRectangle.Y, 2, selectionRectangle.Height + 2), new Color4(0, 0, 0, 0.7f));
                    QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(selectionRectangle.X, selectionRectangle.Bottom, selectionRectangle.Width, 2), new Color4(0, 0, 0, 0.7f));
                }
                uitest.Draw();
                FontRenderer.Draw("default", "FPS : " + Global.Timer.FramesPerSecond, new Vector2(0, 0), Color.White);
                //Thread.MemoryBarrier();
                FontRenderer.Draw("default", "Ticks : " + BackgroundGame.GameTick, new Vector2(0, 20), Color.White);
                FontRenderer.Draw("default", "Ticks : " + BackgroundGame.TimeInterpolate, new Vector2(0, 40), Color.White);
                QuadRenderer.End();

            }
        }
    }
}
