using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RageRTS;
using SharpDX;
using System.IO;
using RageRTS.Map;
using RageEngine;
using RageEngine.Utils;
using ColorChooserCSharp;
using SharpDX.Direct3D11;
using RageEngine.ContentPipeline;
using RageEngine.Graphics;

namespace MapEditor {
    public partial class MainWindow : Form {

        public static string OpenedFile = null;
        public string Title;
        public static FileStream fileStream;
        public static RawMap mapFile;

        public MainWindow() {
            InitializeComponent();

            Title = this.Text;
            Main.Form = this;

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form_DragEnter);
            this.DragDrop += new DragEventHandler(Form_DragDrop);

            renderWindow1.MouseUp += new MouseEventHandler(Window_MouseUp);
            renderWindow1.MouseDown += new MouseEventHandler(Window_MouseDown);
            renderWindow1.MouseMove += new MouseEventHandler(Window_MouseMove);
            renderWindow1.MouseWheel += new MouseEventHandler(Window_MouseWheel);
            renderWindow1.MouseHover += new EventHandler(Window_MouseHover);
            renderWindow1.MouseEnter += new EventHandler(Window_MouseHover);
            renderWindow1.KeyDown += new KeyEventHandler(Window_KeyDown);
            renderWindow1.KeyUp += new KeyEventHandler(Window_KeyUp);

            mapFile = RawMap.Load(OpenedFile);

            this.Text = Title + "  -  " + OpenedFile;
        }

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData) {
            if (renderWindow1.currentTool == Tool.OBJECT_EDIT) {
                /*if (keyData == (Keys.F)) {
                    editStickTerrain.Checked=true;
                    renderWindow1.StickToTerrainOrAlign();
                    editStickTerrain.Checked=false;
                    return true;
                }
                if (keyData == (Keys.A)) {
                    editAlignTerrain.Checked=true;
                    renderWindow1.StickToTerrainOrAlign();
                    editAlignTerrain.Checked=false;
                }

                if (keyData == (Keys.Control | Keys.C)) {
                    renderWindow1.CopyPasteObject(true);
                    return true;
                }
                if (keyData == (Keys.Control | Keys.V)) {
                    renderWindow1.CopyPasteObject(false);
                    return true;
                }*/
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void Window_KeyDown(object sender, KeyEventArgs e) {
            InputHelper.altDown = (e.Alt);
            InputHelper.delDown = (e.KeyCode == Keys.Delete);

            if (renderWindow1.currentTool == Tool.OBJECT_EDIT && e.KeyCode == Keys.Z) {
                editLockX.Checked = true;
                editLockY.Checked = false;
                editLockZ.Checked = true;
            }
        }

        void Window_KeyUp(object sender, KeyEventArgs e) {
            InputHelper.altDown = (e.Alt);
            InputHelper.delDown = false;
            if (renderWindow1.currentTool == Tool.OBJECT_EDIT && e.KeyCode == Keys.Z) {
                editLockX.Checked = false;
                editLockY.Checked = false;
                editLockZ.Checked = false;
            }
        }

        void Window_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
            InputHelper.leftDown = InputHelper.rightDown = InputHelper.middleDown = false;
        }

        void Window_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            InputHelper.leftDown = (e.Button == System.Windows.Forms.MouseButtons.Left);
            InputHelper.rightDown = (e.Button == System.Windows.Forms.MouseButtons.Right);
            InputHelper.middleDown = (e.Button == System.Windows.Forms.MouseButtons.Middle);
        }

        void Window_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            InputHelper.mouseX = e.X;
            InputHelper.mouseY = e.Y;
        }

        void Window_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
            InputHelper.wheelDelta += (e.Delta>0 ? 1:-1);
        }

        void Window_MouseHover(object sender, EventArgs e) {
            renderWindow1.Focus();
        }

        void Form_DragEnter(object sender, DragEventArgs e) {
            if (renderWindow1.loading ) return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];

                if (Directory.Exists(file)) e.Effect = DragDropEffects.Copy; else e.Effect = DragDropEffects.None;
                
            }
        }

        void Form_DragDrop(object sender, DragEventArgs e) {
            if (renderWindow1.loading ) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string file = files[0];

            if (!Directory.Exists(file)) return;
            renderWindow1.AddModel(file);
        }

        private void new_Click(object sender, EventArgs e) {

        }

        private void open_Click(object sender, EventArgs e) {
            Application.Restart();
        }

        private void save_Click(object sender, EventArgs e) {
            if (OpenedFile == "") {
                saveAs_Click(sender, e);
            } else {
                SaveMap(OpenedFile);
            }
        }
        private void saveAs_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel) return;

            OpenedFile =  Path.GetFullPath(saveFileDialog.FileName);
            this.Text = Title + "  -  " + OpenedFile;
            SaveMap(OpenedFile);
        }


        private void SaveMap(string fileOpened) {
            mapFile.Data = new float[mapFile.Width * mapFile.Height];
            for (int y = 0; y < mapFile.Height; y++) {
                for (int x = 0; x < mapFile.Width; x++) {
                    mapFile.Data[x + y * mapFile.Width] = ForegroundGame.Map.altitudeData[x, y];
                }
            }
            string fullpath = Path.GetFullPath(Application.StartupPath+@"\..\data\Maps");
            
            /*foreach (MapModel mapmodel in mapFile.Models) {
                mapmodel.file = PathHandling.RelativePath(fullpath,mapmodel.file);
                mapmodel.Position = mapmodel.model.Position;
                mapmodel.Orientation = mapmodel.model.Orientation;
                mapmodel.Scale = mapmodel.model.Scale;
            }*/


            RawMap.Save(mapFile, fileOpened);

            Texture2DDescription desc = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                Width = mapFile.Width,
                Height =  mapFile.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
            };

            Texture2D tex = new Texture2D(Display.device, desc);
            string pathz;

            if (renderWindow1.modifiedTextureMap) {
                Display.context.CopyResource(ForegroundGame.Map.TextureMap.Tex, tex);
                pathz = Path.Combine(Resources.Main_Path, "Maps\\"+fileOpened+"\\_textureMap.png");
                Texture2D.ToFile(Display.context, tex, ImageFileFormat.Png, pathz);
                renderWindow1.modifiedTextureMap=false;
            }

            if (renderWindow1.modifiedColorMap) {
                Display.context.CopyResource(ForegroundGame.Map.ColorMap.Tex, tex);
                pathz = Path.Combine(Resources.Main_Path, "Maps\\"+fileOpened + "\\_colorMap.png");
                Texture2D.ToFile(Display.context, tex, ImageFileFormat.Png, pathz);
                renderWindow1.modifiedColorMap=false;
            }
            tex.Dispose();
        }

        private void brushChangeSettings_Scroll(object sender, EventArgs e) {
            float value = brushSize.Value;// 1 + ((float)Math.Pow(brushSize.Value, 2) / (float)Math.Pow(brushSize.Maximum, 2)) * brushSize.Maximum;
            renderWindow1.ChangeBrushProperties(value,brushSoftness.Value,brushIntensity.Value);
        }


        private void HeightTo_CheckedChanged(object sender, EventArgs e) {
            if (landToolUp.Checked) renderWindow1.currentTool = Tool.LANDSCAPE_UP;
            if (landToolDown.Checked) renderWindow1.currentTool = Tool.LANDSCAPE_DOWN;
            if (landToolSmooth.Checked) renderWindow1.currentTool = Tool.LANDSCAPE_SMOOTH;

            if (landToolSet.Checked) {
                renderWindow1.currentTool = Tool.LANDSCAPE_SET;
                renderWindow1.HeightLevelSetBrush = (float)levelTo.Value;
                renderWindow1.ChangeBrushProperties(renderWindow1.CurrentBrushSize, brushSoftness.Value, brushIntensity.Value); // intensity is diff when levelTo mode is enabled
            }
        }

        private void levelTo_ValueChanged(object sender, EventArgs e) {
            renderWindow1.HeightLevelSetBrush = (float)levelTo.Value;
        }

        private void mapSettings_ValueChanged(object sender, EventArgs e) {
            if (renderWindow1.loading) return;
            mapFile.Water_Level  = (float)waterLevel.Value;
            mapFile.Water_Density  = (float)waterDensity.Value/100;
            renderWindow1.Terrain.info.waterLevel = mapFile.Water_Level;
            renderWindow1.Terrain.info.waterColor.W = mapFile.Water_Density;
            renderWindow1.TerrainWater.BuildMesh();
            renderWindow1.TerrainWater.Initialize();
            renderWindow1.Terrain.GenerateCB();
        }

        private void pickWaterDepthColor_Click(object sender, EventArgs e) {

            ColorChooser2 frm =new ColorChooser2();
            int r= (int)(renderWindow1.Terrain.info.waterColor.X*255);
            int g= (int)(renderWindow1.Terrain.info.waterColor.Y*255);
            int b= (int)(renderWindow1.Terrain.info.waterColor.Z*255);
            int a= (int)(renderWindow1.Terrain.info.waterColor.W*255);
            frm.Color = System.Drawing.Color.FromArgb(a,r,g,b);
            if (frm.ShowDialog(this) == DialogResult.OK) {
                renderWindow1.Terrain.info.waterColor = new Vector4((float)frm.Color.R/255, (float)frm.Color.G/255, (float)frm.Color.B/255, mapFile.Water_Density);
                mapFile.Water_Color = renderWindow1.Terrain.info.waterColor.ToXYZ();
                renderWindow1.TerrainWater.BuildMesh();
                renderWindow1.TerrainWater.Initialize();
                renderWindow1.Terrain.GenerateCB();
            }
            frm.Dispose();

        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e) {

            switch(tabs.SelectedTab.ToolTipText){
                case "Landscape":
                    renderWindow1.currentTool=Tool.LANDSCAPE_UP;
                    renderWindow1.CurrentBrushColor = new Color4(1, 1, 1,1);
                    HeightTo_CheckedChanged(null, null);//refresh landscape tools
                    break;

                case "Textures":
                    renderWindow1.currentTool=Tool.TEXTURE;
                    renderWindow1.CurrentBrushColor = new Color4(0, 0, 0, 1);
                    textureList.SelectedIndex = 0;
                    textureList_SelectedIndexChanged(null, null);
                    break;
                case "Paint":
                    renderWindow1.currentTool = Tool.COLOR;
                    renderWindow1.CurrentBrushColor = new Color4(1, 1, 1, 1);
                    break;
                case "Decals":
                    decalSelectTool.Checked = true;
                    renderWindow1.currentTool=Tool.DECAL_EDIT;
                    break;
                case "Objects":
                    selectTool.Checked = true;
                    renderWindow1.currentTool=Tool.OBJECT_EDIT;
                    break;
                case "Path-F":
                    pathClearTool.Checked = true;
                    renderWindow1.currentTool = Tool.PATHFINDING;
                    renderWindow1.CurrentBrushColor = new Color4(1, 1, 1, 1);
                    break;
                case "Nodes":
                    nodeSelectTool.Checked = true;
                    renderWindow1.currentTool = Tool.NODE_SELECT;
                    break;

                default:
                    renderWindow1.currentTool=Tool.NOTHING;
                    break;
            }
        }

        private void textureList_SelectedIndexChanged(object sender, EventArgs e) {
            renderWindow1.TextureBrushEnable(textureList.SelectedIndex);
            textureFilePath.Text = mapFile.Textures[textureList.SelectedIndex];
        }

        private void replaceTexture_Click(object sender, EventArgs e) {
            mapFile.Textures[textureList.SelectedIndex] = textureFilePath.Text;
            renderWindow1.LoadTextures(mapFile);
        }

        private void MainInfoInputChange(object sender, EventArgs e) {
            if (renderWindow1.loading) return;
            mapFile.Name = mapName.Text;
            mapFile.Author = mapAuthor.Text;
            mapFile.Description = mapDesc.Text;
        }

        private void EnableMainInfo(object sender, EventArgs e) {
            bool val = !MainInfoCheckBox.Checked;
            mapAuthor.Enabled = mapDesc.Enabled = mapName.Enabled = val;
        }

        private void customSetValue_CheckedChanged(object sender, EventArgs e) {
            levelTo.Enabled = customSetValue.Checked;
        }

        private void selectTool_CheckedChanged(object sender, EventArgs e) {
            if (selectTool.Checked){
                renderWindow1.currentTool = Tool.OBJECT_EDIT;
            }
        }

        private void placeTool_CheckedChanged(object sender, EventArgs e) {
            if (placeTool.Checked){
                renderWindow1.currentTool = Tool.OBJECT_PLACE;
                ModelSelect select = new ModelSelect();
                select.ShowDialog();
                if (ModelSelect.selectedDirectory!="") renderWindow1.AddModel(ModelSelect.selectedDirectory);
            }
        }

        private void decalSelectTool_CheckedChanged(object sender, EventArgs e) {
            if (decalSelectTool.Checked) renderWindow1.currentTool = Tool.DECAL_EDIT; else renderWindow1.currentTool = Tool.DECAL_PLACE;
        }

        private void pathClearTool_CheckedChanged(object sender, EventArgs e) {
            if (pathClearTool.Checked) {
                renderWindow1.CurrentBrushColor = new Color4(1, 1, 1, 1);
            }else{
                renderWindow1.CurrentBrushColor = new Color4(1, 0, 0, 1);
            }
        }

        private void editTerrainAlign_CheckedChanged(object sender, EventArgs e) {

        }

        private void resizeButton_Click(object sender, EventArgs e) {
            ResizeMap resmap = new ResizeMap();
            resmap.ShowDialog();
        }

        private void nodePlaceTool_CheckedChanged(object sender, EventArgs e) {
            if (nodePlaceTool.Checked) {
                renderWindow1.currentTool = Tool.NODE_PLACE;
            } else {
                renderWindow1.currentTool = Tool.NODE_SELECT;
            }
        }


        private void sunPropertiesChange(object sender, EventArgs e) {
            if (renderWindow1.loading) return;

            float pitch = (float)sunHeight.Value /90 *  GameUtils.PiOver2;
            float rot = (float)sunRotation.Value /180 * GameUtils.Pi;

            renderWindow1.showCompass = 200;

            mapFile.Sun_Rotation = rot;
            mapFile.Sun_Height = pitch;
            mapFile.Ambient = (float)AmbientLight.Value / 100;
            mapFile.Ambient_Color = GlobalConstantVars.AmbientColor.ToXYZ();

            GlobalConstantVars.SunColor = mapFile.Sun_Color;

            GlobalConstantVars.SunDirection.X = (float)Math.Sin(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Y = (float)Math.Sin(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Z = (float)Math.Cos(mapFile.Sun_Rotation) * (float)Math.Cos(mapFile.Sun_Height);
            GlobalConstantVars.SunDirection.Normalize();

            GlobalConstantVars.AmbientColor = new Vector4(mapFile.Ambient_Color, mapFile.Ambient);
        }

        private void pathGenerate_Click(object sender, EventArgs e) {
            float heightOut, heightOut1=0, heightOut2=0, heightOut3=0, heightOut4=0;
            Vector3 normal;
            Vector3 normal1 = Vector3.Zero;
            Vector3 normal2 = Vector3.Zero;
            Vector3 normal3 = Vector3.Zero;
            Vector3 normal4 = Vector3.Zero;

            int width = (int)Math.Ceiling((float)ForegroundGame.Map.width/2);
            int height = (int)Math.Ceiling((float)ForegroundGame.Map.height/2);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    renderWindow1.Terrain.info.GetHeightAndNormal(new Vector2(x * 2 + 0.5f, y * 2 + 0.5f), ref heightOut1, true, ref normal1);
                    renderWindow1.Terrain.info.GetHeightAndNormal(new Vector2(x * 2 + 0.5f, y * 2 + 0.5f), ref heightOut2, true, ref normal2);
                    renderWindow1.Terrain.info.GetHeightAndNormal(new Vector2(x * 2 + 0.5f, y * 2 + 0.5f), ref heightOut3, true, ref normal3);
                    renderWindow1.Terrain.info.GetHeightAndNormal(new Vector2(x * 2 + 0.5f, y * 2 + 0.5f), ref heightOut4, true, ref normal4);
                    normal = (normal1 + normal2 + normal3 + normal4) / 4;
                    heightOut = (heightOut1 + heightOut2 + heightOut3 + heightOut4) / 4;
                    if (normal.Y < (0.75f+((float)pathBias.Value/400)) && (heightOut>renderWindow1.Terrain.info.waterLevel*0.9f)) {
                        ForegroundGame.Map.accessibilityArray[x + y * width] = 0;
                    } else {
                        ForegroundGame.Map.accessibilityArray[x + y * width] = 1;
                    }
                }
            }
        }

        private void PaintColorPicker_Click(object sender, EventArgs e) {
            ColorChooser2 frm = new ColorChooser2();
            int r= (int)(renderWindow1.CurrentBrushColor.Red*255);
            int g= (int)(renderWindow1.CurrentBrushColor.Green*255);
            int b= (int)(renderWindow1.CurrentBrushColor.Blue*255);
            int a= (int)(renderWindow1.CurrentBrushColor.Alpha*255);
            frm.Color = System.Drawing.Color.FromArgb(a, r, g, b);
            if (frm.ShowDialog(this) == DialogResult.OK) {
                renderWindow1.CurrentBrushColor = new Color4(new Vector4((float)frm.Color.R / 255, (float)frm.Color.G / 255, (float)frm.Color.B / 255, 1));
                PaintColorPicker.BackColor =frm.Color;
            }
            frm.Dispose();
        }

        private void PaintColorPickWhite_Click(object sender, EventArgs e) {
            renderWindow1.CurrentBrushColor = new Color4(1, 1, 1, 1);

            PaintColorPicker.BackColor = System.Drawing.Color.White;
        }

        private void SunColorPicker_Click(object sender, EventArgs e) {
            if (renderWindow1.loading) return;

            ColorChooser2 frm =new ColorChooser2();
            int r= (int)(GlobalConstantVars.SunColor.X*255);
            int g= (int)(GlobalConstantVars.SunColor.Y*255);
            int b= (int)(GlobalConstantVars.SunColor.Z*255);
            frm.Color = System.Drawing.Color.FromArgb(1, r, g, b);

            if (frm.ShowDialog(this) == DialogResult.OK) {
                GlobalConstantVars.SunColor = new Vector3((float)frm.Color.R/255, (float)frm.Color.G/255, (float)frm.Color.B/255);
                mapFile.Sun_Color = GlobalConstantVars.SunColor;
            }
            frm.Dispose();
        }

        private void ambientColorPicker_Click(object sender, EventArgs e) {

            if (renderWindow1.loading) return;

            ColorChooser2 frm =new ColorChooser2();
            int r= (int)(GlobalConstantVars.AmbientColor.X*255);
            int g= (int)(GlobalConstantVars.AmbientColor.Y*255);
            int b= (int)(GlobalConstantVars.AmbientColor.Z*255);
            frm.Color = System.Drawing.Color.FromArgb(1, r, g, b);

            if (frm.ShowDialog(this) == DialogResult.OK) {
                GlobalConstantVars.AmbientColor = new Vector4((float)frm.Color.R/255, (float)frm.Color.G/255, (float)frm.Color.B/255, mapFile.Ambient);
                mapFile.Ambient_Color = GlobalConstantVars.AmbientColor.ToXYZ();
            }
            frm.Dispose();
        }

    }
}
