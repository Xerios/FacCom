namespace MapEditor {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveasaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.renderWindow1 = new MapEditor.MapRenderWindow();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.brushPreview = new System.Windows.Forms.PictureBox();
            this.brushIntensity = new System.Windows.Forms.TrackBar();
            this.brushSoftness = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.brushSize = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.resizeButton = new System.Windows.Forms.Button();
            this.mapAuthor = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mapDesc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.MainInfoCheckBox = new System.Windows.Forms.CheckBox();
            this.mapName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.customSetValue = new System.Windows.Forms.CheckBox();
            this.landToolSmooth = new System.Windows.Forms.RadioButton();
            this.landToolSet = new System.Windows.Forms.RadioButton();
            this.landToolDown = new System.Windows.Forms.RadioButton();
            this.landToolUp = new System.Windows.Forms.RadioButton();
            this.levelTo = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pickWaterDepthColor = new System.Windows.Forms.Button();
            this.waterDensity = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.waterLevel = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.replaceTexture = new System.Windows.Forms.Button();
            this.textureFilePath = new System.Windows.Forms.TextBox();
            this.textureList = new System.Windows.Forms.ListBox();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.PaintColorPicker = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PaintColorPickWhite = new System.Windows.Forms.Button();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.ambientColorPicker = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.AmbientLight = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.clearLightmap = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.LightmapQuality = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.LightmapGenerateButton = new System.Windows.Forms.Button();
            this.LightmapDiffusion = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.sunHeight = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.sunRotation = new System.Windows.Forms.TrackBar();
            this.SunColorPicker = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.editAlignTerrain = new System.Windows.Forms.CheckBox();
            this.editRotateZ = new System.Windows.Forms.RadioButton();
            this.editRotateY = new System.Windows.Forms.RadioButton();
            this.editRotateX = new System.Windows.Forms.RadioButton();
            this.resetRotation = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.editStickTerrain = new System.Windows.Forms.CheckBox();
            this.editLockX = new System.Windows.Forms.CheckBox();
            this.editLockY = new System.Windows.Forms.CheckBox();
            this.editLockZ = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.placeRandomly = new System.Windows.Forms.CheckBox();
            this.isInstanced = new System.Windows.Forms.CheckBox();
            this.placeTool = new System.Windows.Forms.RadioButton();
            this.selectTool = new System.Windows.Forms.RadioButton();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.decalPlaceTool = new System.Windows.Forms.RadioButton();
            this.decalSelectTool = new System.Windows.Forms.RadioButton();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.pathGenerate = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.pathBias = new System.Windows.Forms.NumericUpDown();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.pathClearTool = new System.Windows.Forms.RadioButton();
            this.pathBlockTool = new System.Windows.Forms.RadioButton();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.nodePlaceTool = new System.Windows.Forms.RadioButton();
            this.nodeSelectTool = new System.Windows.Forms.RadioButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brushPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushSoftness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushSize)).BeginInit();
            this.tabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.levelTo)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waterDensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.waterLevel)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AmbientLight)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapDiffusion)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sunHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sunRotation)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pathBias)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(917, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.opernToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.saveasaToolStripMenuItem,
            this.toolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Enabled = false;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.new_Click);
            // 
            // opernToolStripMenuItem
            // 
            this.opernToolStripMenuItem.Name = "opernToolStripMenuItem";
            this.opernToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.opernToolStripMenuItem.Text = "&Open";
            this.opernToolStripMenuItem.Click += new System.EventHandler(this.open_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(135, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.save_Click);
            // 
            // saveasaToolStripMenuItem
            // 
            this.saveasaToolStripMenuItem.Name = "saveasaToolStripMenuItem";
            this.saveasaToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.saveasaToolStripMenuItem.Text = "Save &as";
            this.saveasaToolStripMenuItem.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.renderWindow1);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(917, 878);
            this.splitContainer1.SplitterDistance = 675;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // renderWindow1
            // 
            this.renderWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderWindow1.Location = new System.Drawing.Point(0, 25);
            this.renderWindow1.Name = "renderWindow1";
            this.renderWindow1.Size = new System.Drawing.Size(675, 853);
            this.renderWindow1.TabIndex = 2;
            this.renderWindow1.Text = "renderWindow1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(675, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.Click += new System.EventHandler(this.open_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            this.saveToolStripButton.Click += new System.EventHandler(this.save_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add(this.brushIntensity);
            this.splitContainer2.Panel1.Controls.Add(this.brushSoftness);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.brushSize);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabs);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(237, 878);
            this.splitContainer2.SplitterDistance = 226;
            this.splitContainer2.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.brushPreview);
            this.groupBox1.Location = new System.Drawing.Point(76, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(121, 129);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // brushPreview
            // 
            this.brushPreview.BackColor = System.Drawing.Color.Black;
            this.brushPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.brushPreview.Location = new System.Drawing.Point(9, 16);
            this.brushPreview.Name = "brushPreview";
            this.brushPreview.Size = new System.Drawing.Size(105, 105);
            this.brushPreview.TabIndex = 9;
            this.brushPreview.TabStop = false;
            // 
            // brushIntensity
            // 
            this.brushIntensity.Location = new System.Drawing.Point(69, 168);
            this.brushIntensity.Maximum = 100;
            this.brushIntensity.Minimum = 1;
            this.brushIntensity.Name = "brushIntensity";
            this.brushIntensity.Size = new System.Drawing.Size(135, 45);
            this.brushIntensity.TabIndex = 8;
            this.brushIntensity.TickFrequency = 5;
            this.brushIntensity.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.brushIntensity.Value = 10;
            this.brushIntensity.Scroll += new System.EventHandler(this.brushChangeSettings_Scroll);
            // 
            // brushSoftness
            // 
            this.brushSoftness.Location = new System.Drawing.Point(12, 25);
            this.brushSoftness.Maximum = 100;
            this.brushSoftness.Name = "brushSoftness";
            this.brushSoftness.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.brushSoftness.Size = new System.Drawing.Size(45, 107);
            this.brushSoftness.TabIndex = 7;
            this.brushSoftness.TickFrequency = 20;
            this.brushSoftness.Value = 10;
            this.brushSoftness.Scroll += new System.EventHandler(this.brushChangeSettings_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Size :";
            // 
            // brushSize
            // 
            this.brushSize.Location = new System.Drawing.Point(69, 138);
            this.brushSize.Maximum = 200;
            this.brushSize.Minimum = 1;
            this.brushSize.Name = "brushSize";
            this.brushSize.Size = new System.Drawing.Size(135, 45);
            this.brushSize.TabIndex = 6;
            this.brushSize.TickFrequency = 20;
            this.brushSize.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.brushSize.Value = 20;
            this.brushSize.Scroll += new System.EventHandler(this.brushChangeSettings_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Softness :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Intensity :";
            // 
            // tabs
            // 
            this.tabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabs.Controls.Add(this.tabPage1);
            this.tabs.Controls.Add(this.tabPage2);
            this.tabs.Controls.Add(this.tabPage3);
            this.tabs.Controls.Add(this.tabPage8);
            this.tabs.Controls.Add(this.tabPage7);
            this.tabs.Controls.Add(this.tabPage4);
            this.tabs.Controls.Add(this.tabPage5);
            this.tabs.Controls.Add(this.tabPage6);
            this.tabs.Controls.Add(this.tabPage9);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.ImageList = this.imageList1;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Multiline = true;
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.ShowToolTips = true;
            this.tabs.Size = new System.Drawing.Size(237, 648);
            this.tabs.TabIndex = 1;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.resizeButton);
            this.tabPage1.Controls.Add(this.mapAuthor);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.mapDesc);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.MainInfoCheckBox);
            this.tabPage1.Controls.Add(this.mapName);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 76);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(229, 568);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Tag = "0";
            this.tabPage1.ToolTipText = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // resizeButton
            // 
            this.resizeButton.Location = new System.Drawing.Point(15, 179);
            this.resizeButton.Name = "resizeButton";
            this.resizeButton.Size = new System.Drawing.Size(183, 27);
            this.resizeButton.TabIndex = 7;
            this.resizeButton.Text = "Resize";
            this.resizeButton.UseVisualStyleBackColor = true;
            this.resizeButton.Click += new System.EventHandler(this.resizeButton_Click);
            // 
            // mapAuthor
            // 
            this.mapAuthor.Enabled = false;
            this.mapAuthor.Location = new System.Drawing.Point(50, 52);
            this.mapAuthor.Name = "mapAuthor";
            this.mapAuthor.Size = new System.Drawing.Size(131, 18);
            this.mapAuthor.TabIndex = 6;
            this.mapAuthor.TextChanged += new System.EventHandler(this.MainInfoInputChange);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 12);
            this.label9.TabIndex = 5;
            this.label9.Text = "Author :";
            // 
            // mapDesc
            // 
            this.mapDesc.Enabled = false;
            this.mapDesc.Location = new System.Drawing.Point(50, 78);
            this.mapDesc.Multiline = true;
            this.mapDesc.Name = "mapDesc";
            this.mapDesc.Size = new System.Drawing.Size(131, 85);
            this.mapDesc.TabIndex = 4;
            this.mapDesc.TextChanged += new System.EventHandler(this.MainInfoInputChange);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "Desc :";
            // 
            // MainInfoCheckBox
            // 
            this.MainInfoCheckBox.AutoSize = true;
            this.MainInfoCheckBox.Checked = true;
            this.MainInfoCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MainInfoCheckBox.Location = new System.Drawing.Point(5, 6);
            this.MainInfoCheckBox.Name = "MainInfoCheckBox";
            this.MainInfoCheckBox.Size = new System.Drawing.Size(60, 16);
            this.MainInfoCheckBox.TabIndex = 2;
            this.MainInfoCheckBox.Text = "Locked";
            this.MainInfoCheckBox.UseVisualStyleBackColor = true;
            this.MainInfoCheckBox.CheckedChanged += new System.EventHandler(this.EnableMainInfo);
            // 
            // mapName
            // 
            this.mapName.Enabled = false;
            this.mapName.Location = new System.Drawing.Point(50, 28);
            this.mapName.Name = "mapName";
            this.mapName.Size = new System.Drawing.Size(131, 18);
            this.mapName.TabIndex = 1;
            this.mapName.TextChanged += new System.EventHandler(this.MainInfoInputChange);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Name :";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.ImageIndex = 5;
            this.tabPage2.Location = new System.Drawing.Point(4, 76);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(229, 568);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Tag = "1";
            this.tabPage2.ToolTipText = "Landscape";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.customSetValue);
            this.groupBox6.Controls.Add(this.landToolSmooth);
            this.groupBox6.Controls.Add(this.landToolSet);
            this.groupBox6.Controls.Add(this.landToolDown);
            this.groupBox6.Controls.Add(this.landToolUp);
            this.groupBox6.Controls.Add(this.levelTo);
            this.groupBox6.Location = new System.Drawing.Point(7, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(190, 110);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tool";
            // 
            // customSetValue
            // 
            this.customSetValue.AutoSize = true;
            this.customSetValue.Location = new System.Drawing.Point(86, 85);
            this.customSetValue.Name = "customSetValue";
            this.customSetValue.Size = new System.Drawing.Size(15, 14);
            this.customSetValue.TabIndex = 11;
            this.customSetValue.UseVisualStyleBackColor = true;
            this.customSetValue.CheckedChanged += new System.EventHandler(this.customSetValue_CheckedChanged);
            // 
            // landToolSmooth
            // 
            this.landToolSmooth.AutoSize = true;
            this.landToolSmooth.Location = new System.Drawing.Point(6, 61);
            this.landToolSmooth.Name = "landToolSmooth";
            this.landToolSmooth.Size = new System.Drawing.Size(61, 16);
            this.landToolSmooth.TabIndex = 3;
            this.landToolSmooth.Text = "Smooth";
            this.landToolSmooth.UseVisualStyleBackColor = true;
            this.landToolSmooth.CheckedChanged += new System.EventHandler(this.HeightTo_CheckedChanged);
            // 
            // landToolSet
            // 
            this.landToolSet.AutoSize = true;
            this.landToolSet.Location = new System.Drawing.Point(6, 83);
            this.landToolSet.Name = "landToolSet";
            this.landToolSet.Size = new System.Drawing.Size(58, 16);
            this.landToolSet.TabIndex = 2;
            this.landToolSet.Text = "Flatten";
            this.landToolSet.UseVisualStyleBackColor = true;
            this.landToolSet.CheckedChanged += new System.EventHandler(this.HeightTo_CheckedChanged);
            // 
            // landToolDown
            // 
            this.landToolDown.AutoSize = true;
            this.landToolDown.Location = new System.Drawing.Point(6, 39);
            this.landToolDown.Name = "landToolDown";
            this.landToolDown.Size = new System.Drawing.Size(66, 16);
            this.landToolDown.TabIndex = 1;
            this.landToolDown.Text = "Digg    \\/";
            this.landToolDown.UseVisualStyleBackColor = true;
            this.landToolDown.CheckedChanged += new System.EventHandler(this.HeightTo_CheckedChanged);
            // 
            // landToolUp
            // 
            this.landToolUp.AutoSize = true;
            this.landToolUp.Checked = true;
            this.landToolUp.Location = new System.Drawing.Point(6, 17);
            this.landToolUp.Name = "landToolUp";
            this.landToolUp.Size = new System.Drawing.Size(65, 16);
            this.landToolUp.TabIndex = 0;
            this.landToolUp.TabStop = true;
            this.landToolUp.Text = "Rise    /\\";
            this.landToolUp.UseVisualStyleBackColor = true;
            this.landToolUp.CheckedChanged += new System.EventHandler(this.HeightTo_CheckedChanged);
            // 
            // levelTo
            // 
            this.levelTo.DecimalPlaces = 2;
            this.levelTo.Enabled = false;
            this.levelTo.Location = new System.Drawing.Point(107, 83);
            this.levelTo.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.levelTo.Name = "levelTo";
            this.levelTo.Size = new System.Drawing.Size(77, 18);
            this.levelTo.TabIndex = 10;
            this.levelTo.ValueChanged += new System.EventHandler(this.levelTo_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pickWaterDepthColor);
            this.groupBox3.Controls.Add(this.waterDensity);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.waterLevel);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(8, 207);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(190, 97);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = " Water options";
            // 
            // pickWaterDepthColor
            // 
            this.pickWaterDepthColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.pickWaterDepthColor.Location = new System.Drawing.Point(7, 65);
            this.pickWaterDepthColor.Name = "pickWaterDepthColor";
            this.pickWaterDepthColor.Size = new System.Drawing.Size(177, 23);
            this.pickWaterDepthColor.TabIndex = 24;
            this.pickWaterDepthColor.Text = "Pick water color";
            this.pickWaterDepthColor.UseVisualStyleBackColor = true;
            this.pickWaterDepthColor.Click += new System.EventHandler(this.pickWaterDepthColor_Click);
            // 
            // waterDensity
            // 
            this.waterDensity.Location = new System.Drawing.Point(115, 39);
            this.waterDensity.Name = "waterDensity";
            this.waterDensity.Size = new System.Drawing.Size(69, 18);
            this.waterDensity.TabIndex = 17;
            this.waterDensity.ValueChanged += new System.EventHandler(this.mapSettings_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Water density :";
            // 
            // waterLevel
            // 
            this.waterLevel.DecimalPlaces = 2;
            this.waterLevel.Location = new System.Drawing.Point(115, 17);
            this.waterLevel.Name = "waterLevel";
            this.waterLevel.Size = new System.Drawing.Size(69, 18);
            this.waterLevel.TabIndex = 12;
            this.waterLevel.ValueChanged += new System.EventHandler(this.mapSettings_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "Water height :";
            // 
            // tabPage3
            // 
            this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Controls.Add(this.textureList);
            this.tabPage3.ImageIndex = 7;
            this.tabPage3.Location = new System.Drawing.Point(4, 76);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(228, 568);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Tag = "2";
            this.tabPage3.ToolTipText = "Textures";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.replaceTexture);
            this.groupBox4.Controls.Add(this.textureFilePath);
            this.groupBox4.Location = new System.Drawing.Point(4, 63);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(197, 71);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Properties";
            // 
            // replaceTexture
            // 
            this.replaceTexture.Location = new System.Drawing.Point(116, 41);
            this.replaceTexture.Name = "replaceTexture";
            this.replaceTexture.Size = new System.Drawing.Size(75, 23);
            this.replaceTexture.TabIndex = 3;
            this.replaceTexture.Text = "Replace";
            this.replaceTexture.UseVisualStyleBackColor = true;
            this.replaceTexture.Click += new System.EventHandler(this.replaceTexture_Click);
            // 
            // textureFilePath
            // 
            this.textureFilePath.Location = new System.Drawing.Point(6, 17);
            this.textureFilePath.Name = "textureFilePath";
            this.textureFilePath.Size = new System.Drawing.Size(184, 18);
            this.textureFilePath.TabIndex = 2;
            // 
            // textureList
            // 
            this.textureList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textureList.FormattingEnabled = true;
            this.textureList.IntegralHeight = false;
            this.textureList.ItemHeight = 12;
            this.textureList.Items.AddRange(new object[] {
            "Texture 1",
            "Texture 2",
            "Texture 3",
            "Texture 4"});
            this.textureList.Location = new System.Drawing.Point(4, 3);
            this.textureList.Name = "textureList";
            this.textureList.Size = new System.Drawing.Size(198, 54);
            this.textureList.TabIndex = 1;
            this.textureList.SelectedIndexChanged += new System.EventHandler(this.textureList_SelectedIndexChanged);
            // 
            // tabPage8
            // 
            this.tabPage8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage8.Controls.Add(this.PaintColorPicker);
            this.tabPage8.Controls.Add(this.label4);
            this.tabPage8.Controls.Add(this.PaintColorPickWhite);
            this.tabPage8.ImageIndex = 1;
            this.tabPage8.Location = new System.Drawing.Point(4, 76);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(228, 568);
            this.tabPage8.TabIndex = 9;
            this.tabPage8.Tag = "3";
            this.tabPage8.ToolTipText = "Paint";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // PaintColorPicker
            // 
            this.PaintColorPicker.BackColor = System.Drawing.Color.White;
            this.PaintColorPicker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PaintColorPicker.Location = new System.Drawing.Point(98, 12);
            this.PaintColorPicker.Name = "PaintColorPicker";
            this.PaintColorPicker.Size = new System.Drawing.Size(102, 58);
            this.PaintColorPicker.TabIndex = 21;
            this.PaintColorPicker.Text = "Pick";
            this.PaintColorPicker.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PaintColorPicker.Click += new System.EventHandler(this.PaintColorPicker_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 12);
            this.label4.TabIndex = 20;
            this.label4.Text = "Current color :";
            // 
            // PaintColorPickWhite
            // 
            this.PaintColorPickWhite.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PaintColorPickWhite.Location = new System.Drawing.Point(8, 87);
            this.PaintColorPickWhite.Name = "PaintColorPickWhite";
            this.PaintColorPickWhite.Size = new System.Drawing.Size(192, 29);
            this.PaintColorPickWhite.TabIndex = 19;
            this.PaintColorPickWhite.Text = "Pick pure white color";
            this.PaintColorPickWhite.UseVisualStyleBackColor = true;
            this.PaintColorPickWhite.Click += new System.EventHandler(this.PaintColorPickWhite_Click);
            // 
            // tabPage7
            // 
            this.tabPage7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage7.Controls.Add(this.groupBox14);
            this.tabPage7.Controls.Add(this.groupBox5);
            this.tabPage7.Controls.Add(this.groupBox2);
            this.tabPage7.ImageIndex = 3;
            this.tabPage7.Location = new System.Drawing.Point(4, 76);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(228, 568);
            this.tabPage7.TabIndex = 8;
            this.tabPage7.Tag = "0";
            this.tabPage7.ToolTipText = "Light";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.ambientColorPicker);
            this.groupBox14.Controls.Add(this.label12);
            this.groupBox14.Controls.Add(this.AmbientLight);
            this.groupBox14.Location = new System.Drawing.Point(11, 3);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(187, 103);
            this.groupBox14.TabIndex = 23;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Ambient Light";
            // 
            // ambientColorPicker
            // 
            this.ambientColorPicker.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ambientColorPicker.Location = new System.Drawing.Point(6, 51);
            this.ambientColorPicker.Name = "ambientColorPicker";
            this.ambientColorPicker.Size = new System.Drawing.Size(172, 39);
            this.ambientColorPicker.TabIndex = 24;
            this.ambientColorPicker.Text = "Pick ambient color\r\n( sky and shadows ) ";
            this.ambientColorPicker.UseVisualStyleBackColor = true;
            this.ambientColorPicker.Click += new System.EventHandler(this.ambientColorPicker_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "Ambient (%) :";
            // 
            // AmbientLight
            // 
            this.AmbientLight.Location = new System.Drawing.Point(109, 18);
            this.AmbientLight.Name = "AmbientLight";
            this.AmbientLight.Size = new System.Drawing.Size(69, 18);
            this.AmbientLight.TabIndex = 23;
            this.AmbientLight.ValueChanged += new System.EventHandler(this.sunPropertiesChange);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.clearLightmap);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.LightmapQuality);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.LightmapGenerateButton);
            this.groupBox5.Controls.Add(this.LightmapDiffusion);
            this.groupBox5.Location = new System.Drawing.Point(11, 292);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(188, 144);
            this.groupBox5.TabIndex = 22;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "LightMap";
            // 
            // clearLightmap
            // 
            this.clearLightmap.Location = new System.Drawing.Point(9, 107);
            this.clearLightmap.Name = "clearLightmap";
            this.clearLightmap.Size = new System.Drawing.Size(169, 26);
            this.clearLightmap.TabIndex = 29;
            this.clearLightmap.Text = "Clear lightmap";
            this.clearLightmap.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 49);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 12);
            this.label14.TabIndex = 27;
            this.label14.Text = "Diffusion :";
            // 
            // LightmapQuality
            // 
            this.LightmapQuality.Location = new System.Drawing.Point(107, 19);
            this.LightmapQuality.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.LightmapQuality.Name = "LightmapQuality";
            this.LightmapQuality.Size = new System.Drawing.Size(71, 18);
            this.LightmapQuality.TabIndex = 26;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 12);
            this.label13.TabIndex = 25;
            this.label13.Text = "Quality :";
            // 
            // LightmapGenerateButton
            // 
            this.LightmapGenerateButton.Location = new System.Drawing.Point(9, 76);
            this.LightmapGenerateButton.Name = "LightmapGenerateButton";
            this.LightmapGenerateButton.Size = new System.Drawing.Size(169, 26);
            this.LightmapGenerateButton.TabIndex = 24;
            this.LightmapGenerateButton.Text = "Generate lightmap";
            this.LightmapGenerateButton.UseVisualStyleBackColor = true;
            // 
            // LightmapDiffusion
            // 
            this.LightmapDiffusion.Location = new System.Drawing.Point(107, 47);
            this.LightmapDiffusion.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.LightmapDiffusion.Name = "LightmapDiffusion";
            this.LightmapDiffusion.Size = new System.Drawing.Size(71, 18);
            this.LightmapDiffusion.TabIndex = 28;
            this.LightmapDiffusion.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.sunHeight);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.sunRotation);
            this.groupBox2.Controls.Add(this.SunColorPicker);
            this.groupBox2.Location = new System.Drawing.Point(11, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 174);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sun options";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 105);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 12);
            this.label11.TabIndex = 22;
            this.label11.Text = "Height :";
            // 
            // sunHeight
            // 
            this.sunHeight.Location = new System.Drawing.Point(12, 127);
            this.sunHeight.Maximum = 0;
            this.sunHeight.Minimum = -90;
            this.sunHeight.Name = "sunHeight";
            this.sunHeight.Size = new System.Drawing.Size(172, 45);
            this.sunHeight.TabIndex = 21;
            this.sunHeight.TickFrequency = 2;
            this.sunHeight.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sunHeight.Scroll += new System.EventHandler(this.sunPropertiesChange);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 12);
            this.label10.TabIndex = 20;
            this.label10.Text = "Rotation :";
            // 
            // sunRotation
            // 
            this.sunRotation.Location = new System.Drawing.Point(6, 63);
            this.sunRotation.Maximum = 360;
            this.sunRotation.Name = "sunRotation";
            this.sunRotation.Size = new System.Drawing.Size(172, 45);
            this.sunRotation.TabIndex = 11;
            this.sunRotation.TickFrequency = 10;
            this.sunRotation.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sunRotation.Value = 20;
            this.sunRotation.Scroll += new System.EventHandler(this.sunPropertiesChange);
            // 
            // SunColorPicker
            // 
            this.SunColorPicker.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SunColorPicker.Location = new System.Drawing.Point(6, 16);
            this.SunColorPicker.Name = "SunColorPicker";
            this.SunColorPicker.Size = new System.Drawing.Size(172, 23);
            this.SunColorPicker.TabIndex = 18;
            this.SunColorPicker.Text = "Pick sun color";
            this.SunColorPicker.UseVisualStyleBackColor = true;
            this.SunColorPicker.Click += new System.EventHandler(this.SunColorPicker_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage4.Controls.Add(this.groupBox8);
            this.tabPage4.Controls.Add(this.groupBox7);
            this.tabPage4.ImageIndex = 4;
            this.tabPage4.Location = new System.Drawing.Point(4, 76);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(228, 568);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Tag = "4";
            this.tabPage4.ToolTipText = "Objects";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox10);
            this.groupBox8.Controls.Add(this.groupBox9);
            this.groupBox8.Location = new System.Drawing.Point(8, 137);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(192, 235);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Edit tool options";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.editAlignTerrain);
            this.groupBox10.Controls.Add(this.editRotateZ);
            this.groupBox10.Controls.Add(this.editRotateY);
            this.groupBox10.Controls.Add(this.editRotateX);
            this.groupBox10.Controls.Add(this.resetRotation);
            this.groupBox10.Location = new System.Drawing.Point(6, 133);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(176, 93);
            this.groupBox10.TabIndex = 5;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Rotation";
            // 
            // editAlignTerrain
            // 
            this.editAlignTerrain.AutoSize = true;
            this.editAlignTerrain.Location = new System.Drawing.Point(7, 40);
            this.editAlignTerrain.Name = "editAlignTerrain";
            this.editAlignTerrain.Size = new System.Drawing.Size(118, 16);
            this.editAlignTerrain.TabIndex = 4;
            this.editAlignTerrain.Text = "Align to the terrain";
            this.editAlignTerrain.UseVisualStyleBackColor = true;
            this.editAlignTerrain.CheckedChanged += new System.EventHandler(this.editTerrainAlign_CheckedChanged);
            // 
            // editRotateZ
            // 
            this.editRotateZ.AutoSize = true;
            this.editRotateZ.Location = new System.Drawing.Point(128, 18);
            this.editRotateZ.Name = "editRotateZ";
            this.editRotateZ.Size = new System.Drawing.Size(42, 16);
            this.editRotateZ.TabIndex = 6;
            this.editRotateZ.Text = "Roll";
            this.editRotateZ.UseVisualStyleBackColor = true;
            // 
            // editRotateY
            // 
            this.editRotateY.AutoSize = true;
            this.editRotateY.Location = new System.Drawing.Point(66, 18);
            this.editRotateY.Name = "editRotateY";
            this.editRotateY.Size = new System.Drawing.Size(49, 16);
            this.editRotateY.TabIndex = 5;
            this.editRotateY.Text = "Pitch";
            this.editRotateY.UseVisualStyleBackColor = true;
            // 
            // editRotateX
            // 
            this.editRotateX.AutoSize = true;
            this.editRotateX.Checked = true;
            this.editRotateX.Location = new System.Drawing.Point(7, 18);
            this.editRotateX.Name = "editRotateX";
            this.editRotateX.Size = new System.Drawing.Size(42, 16);
            this.editRotateX.TabIndex = 4;
            this.editRotateX.TabStop = true;
            this.editRotateX.Text = "Yaw";
            this.editRotateX.UseVisualStyleBackColor = true;
            // 
            // resetRotation
            // 
            this.resetRotation.Location = new System.Drawing.Point(35, 62);
            this.resetRotation.Name = "resetRotation";
            this.resetRotation.Size = new System.Drawing.Size(101, 22);
            this.resetRotation.TabIndex = 3;
            this.resetRotation.Text = "Reset rotation";
            this.resetRotation.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.editStickTerrain);
            this.groupBox9.Controls.Add(this.editLockX);
            this.groupBox9.Controls.Add(this.editLockY);
            this.groupBox9.Controls.Add(this.editLockZ);
            this.groupBox9.Location = new System.Drawing.Point(6, 17);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(179, 110);
            this.groupBox9.TabIndex = 4;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Position / Scale";
            // 
            // editStickTerrain
            // 
            this.editStickTerrain.AutoSize = true;
            this.editStickTerrain.Location = new System.Drawing.Point(6, 83);
            this.editStickTerrain.Name = "editStickTerrain";
            this.editStickTerrain.Size = new System.Drawing.Size(118, 16);
            this.editStickTerrain.TabIndex = 3;
            this.editStickTerrain.Text = "Stick to the terrain";
            this.editStickTerrain.UseVisualStyleBackColor = true;
            this.editStickTerrain.CheckedChanged += new System.EventHandler(this.editTerrainAlign_CheckedChanged);
            // 
            // editLockX
            // 
            this.editLockX.AutoSize = true;
            this.editLockX.Location = new System.Drawing.Point(6, 17);
            this.editLockX.Name = "editLockX";
            this.editLockX.Size = new System.Drawing.Size(58, 16);
            this.editLockX.TabIndex = 0;
            this.editLockX.Text = "Lock X";
            this.editLockX.UseVisualStyleBackColor = true;
            // 
            // editLockY
            // 
            this.editLockY.AutoSize = true;
            this.editLockY.Location = new System.Drawing.Point(6, 39);
            this.editLockY.Name = "editLockY";
            this.editLockY.Size = new System.Drawing.Size(58, 16);
            this.editLockY.TabIndex = 1;
            this.editLockY.Text = "Lock Y";
            this.editLockY.UseVisualStyleBackColor = true;
            // 
            // editLockZ
            // 
            this.editLockZ.AutoSize = true;
            this.editLockZ.Location = new System.Drawing.Point(6, 61);
            this.editLockZ.Name = "editLockZ";
            this.editLockZ.Size = new System.Drawing.Size(58, 16);
            this.editLockZ.TabIndex = 2;
            this.editLockZ.Text = "Lock Z";
            this.editLockZ.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.placeRandomly);
            this.groupBox7.Controls.Add(this.isInstanced);
            this.groupBox7.Controls.Add(this.placeTool);
            this.groupBox7.Controls.Add(this.selectTool);
            this.groupBox7.Location = new System.Drawing.Point(8, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(192, 110);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Tool";
            // 
            // placeRandomly
            // 
            this.placeRandomly.AutoSize = true;
            this.placeRandomly.Location = new System.Drawing.Point(13, 82);
            this.placeRandomly.Name = "placeRandomly";
            this.placeRandomly.Size = new System.Drawing.Size(160, 16);
            this.placeRandomly.TabIndex = 5;
            this.placeRandomly.Text = "Random Rotations and Size";
            this.placeRandomly.UseVisualStyleBackColor = true;
            // 
            // isInstanced
            // 
            this.isInstanced.AutoSize = true;
            this.isInstanced.Location = new System.Drawing.Point(13, 60);
            this.isInstanced.Name = "isInstanced";
            this.isInstanced.Size = new System.Drawing.Size(84, 16);
            this.isInstanced.TabIndex = 4;
            this.isInstanced.Text = "Instanced ?";
            this.isInstanced.UseVisualStyleBackColor = true;
            // 
            // placeTool
            // 
            this.placeTool.AutoSize = true;
            this.placeTool.Location = new System.Drawing.Point(6, 38);
            this.placeTool.Name = "placeTool";
            this.placeTool.Size = new System.Drawing.Size(73, 16);
            this.placeTool.TabIndex = 1;
            this.placeTool.TabStop = true;
            this.placeTool.Text = "Place tool";
            this.placeTool.UseVisualStyleBackColor = true;
            this.placeTool.CheckedChanged += new System.EventHandler(this.placeTool_CheckedChanged);
            // 
            // selectTool
            // 
            this.selectTool.AutoSize = true;
            this.selectTool.Checked = true;
            this.selectTool.Location = new System.Drawing.Point(6, 16);
            this.selectTool.Name = "selectTool";
            this.selectTool.Size = new System.Drawing.Size(101, 16);
            this.selectTool.TabIndex = 0;
            this.selectTool.TabStop = true;
            this.selectTool.Text = "Select/Edit tool";
            this.selectTool.UseVisualStyleBackColor = true;
            this.selectTool.CheckedChanged += new System.EventHandler(this.selectTool_CheckedChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage5.Controls.Add(this.groupBox13);
            this.tabPage5.ImageIndex = 2;
            this.tabPage5.Location = new System.Drawing.Point(4, 76);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(228, 568);
            this.tabPage5.TabIndex = 6;
            this.tabPage5.Tag = "4";
            this.tabPage5.ToolTipText = "Decals";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.decalPlaceTool);
            this.groupBox13.Controls.Add(this.decalSelectTool);
            this.groupBox13.Location = new System.Drawing.Point(8, 12);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(193, 60);
            this.groupBox13.TabIndex = 0;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Tool";
            // 
            // decalPlaceTool
            // 
            this.decalPlaceTool.AutoSize = true;
            this.decalPlaceTool.Location = new System.Drawing.Point(7, 36);
            this.decalPlaceTool.Name = "decalPlaceTool";
            this.decalPlaceTool.Size = new System.Drawing.Size(73, 16);
            this.decalPlaceTool.TabIndex = 1;
            this.decalPlaceTool.Text = "Place tool";
            this.decalPlaceTool.UseVisualStyleBackColor = true;
            this.decalPlaceTool.CheckedChanged += new System.EventHandler(this.decalSelectTool_CheckedChanged);
            // 
            // decalSelectTool
            // 
            this.decalSelectTool.AutoSize = true;
            this.decalSelectTool.Checked = true;
            this.decalSelectTool.Location = new System.Drawing.Point(7, 17);
            this.decalSelectTool.Name = "decalSelectTool";
            this.decalSelectTool.Size = new System.Drawing.Size(101, 16);
            this.decalSelectTool.TabIndex = 0;
            this.decalSelectTool.TabStop = true;
            this.decalSelectTool.Text = "Select/Edit tool";
            this.decalSelectTool.UseVisualStyleBackColor = true;
            this.decalSelectTool.CheckedChanged += new System.EventHandler(this.decalSelectTool_CheckedChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage6.Controls.Add(this.groupBox12);
            this.tabPage6.Controls.Add(this.groupBox11);
            this.tabPage6.ImageIndex = 6;
            this.tabPage6.Location = new System.Drawing.Point(4, 76);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(228, 568);
            this.tabPage6.TabIndex = 7;
            this.tabPage6.Tag = "5";
            this.tabPage6.ToolTipText = "Path-F";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.pathGenerate);
            this.groupBox12.Controls.Add(this.label15);
            this.groupBox12.Controls.Add(this.pathBias);
            this.groupBox12.Location = new System.Drawing.Point(8, 80);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(191, 72);
            this.groupBox12.TabIndex = 3;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Auto generator";
            // 
            // pathGenerate
            // 
            this.pathGenerate.Location = new System.Drawing.Point(10, 41);
            this.pathGenerate.Name = "pathGenerate";
            this.pathGenerate.Size = new System.Drawing.Size(175, 25);
            this.pathGenerate.TabIndex = 2;
            this.pathGenerate.Text = "Generate";
            this.pathGenerate.UseVisualStyleBackColor = true;
            this.pathGenerate.Click += new System.EventHandler(this.pathGenerate_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 19);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "Bias :";
            // 
            // pathBias
            // 
            this.pathBias.Location = new System.Drawing.Point(73, 17);
            this.pathBias.Name = "pathBias";
            this.pathBias.Size = new System.Drawing.Size(112, 18);
            this.pathBias.TabIndex = 0;
            this.pathBias.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.pathClearTool);
            this.groupBox11.Controls.Add(this.pathBlockTool);
            this.groupBox11.Location = new System.Drawing.Point(8, 5);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(192, 65);
            this.groupBox11.TabIndex = 2;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Tool";
            // 
            // pathClearTool
            // 
            this.pathClearTool.AutoSize = true;
            this.pathClearTool.Checked = true;
            this.pathClearTool.Location = new System.Drawing.Point(6, 17);
            this.pathClearTool.Name = "pathClearTool";
            this.pathClearTool.Size = new System.Drawing.Size(72, 16);
            this.pathClearTool.TabIndex = 0;
            this.pathClearTool.TabStop = true;
            this.pathClearTool.Text = "Clear tool";
            this.pathClearTool.UseVisualStyleBackColor = true;
            this.pathClearTool.CheckedChanged += new System.EventHandler(this.pathClearTool_CheckedChanged);
            // 
            // pathBlockTool
            // 
            this.pathBlockTool.AutoSize = true;
            this.pathBlockTool.Location = new System.Drawing.Point(6, 39);
            this.pathBlockTool.Name = "pathBlockTool";
            this.pathBlockTool.Size = new System.Drawing.Size(73, 16);
            this.pathBlockTool.TabIndex = 1;
            this.pathBlockTool.Text = "Block tool";
            this.pathBlockTool.UseVisualStyleBackColor = true;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.groupBox15);
            this.tabPage9.Controls.Add(this.nodePlaceTool);
            this.tabPage9.Controls.Add(this.nodeSelectTool);
            this.tabPage9.Location = new System.Drawing.Point(4, 76);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(228, 568);
            this.tabPage9.TabIndex = 10;
            this.tabPage9.Text = "Game Related Objects";
            this.tabPage9.ToolTipText = "Nodes";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            this.groupBox15.Location = new System.Drawing.Point(8, 58);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(200, 178);
            this.groupBox15.TabIndex = 4;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Selected Object";
            // 
            // nodePlaceTool
            // 
            this.nodePlaceTool.AutoSize = true;
            this.nodePlaceTool.Location = new System.Drawing.Point(8, 13);
            this.nodePlaceTool.Name = "nodePlaceTool";
            this.nodePlaceTool.Size = new System.Drawing.Size(73, 16);
            this.nodePlaceTool.TabIndex = 3;
            this.nodePlaceTool.TabStop = true;
            this.nodePlaceTool.Text = "Place tool";
            this.nodePlaceTool.UseVisualStyleBackColor = true;
            this.nodePlaceTool.CheckedChanged += new System.EventHandler(this.nodePlaceTool_CheckedChanged);
            // 
            // nodeSelectTool
            // 
            this.nodeSelectTool.AutoSize = true;
            this.nodeSelectTool.Checked = true;
            this.nodeSelectTool.Location = new System.Drawing.Point(8, 35);
            this.nodeSelectTool.Name = "nodeSelectTool";
            this.nodeSelectTool.Size = new System.Drawing.Size(101, 16);
            this.nodeSelectTool.TabIndex = 2;
            this.nodeSelectTool.TabStop = true;
            this.nodeSelectTool.Text = "Select/Edit tool";
            this.nodeSelectTool.UseVisualStyleBackColor = true;
            this.nodeSelectTool.CheckedChanged += new System.EventHandler(this.nodePlaceTool_CheckedChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1306768695_page_white_text.png");
            this.imageList1.Images.SetKeyName(1, "1306768725_paintbrush.png");
            this.imageList1.Images.SetKeyName(2, "1306768736_stock_3d-texture.png");
            this.imageList1.Images.SetKeyName(3, "1306768743_stock_3d-light-on.png");
            this.imageList1.Images.SetKeyName(4, "1306768746_stock_macro-objects.png");
            this.imageList1.Images.SetKeyName(5, "1306768749_pathing.png");
            this.imageList1.Images.SetKeyName(6, "1306768843_grid.png");
            this.imageList1.Images.SetKeyName(7, "1306768877_stock_3d-texture.png");
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 902);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RageEffect - Map Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.brushPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushSoftness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brushSize)).EndInit();
            this.tabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.levelTo)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waterDensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.waterLevel)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AmbientLight)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LightmapDiffusion)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sunHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sunRotation)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pathBias)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opernToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveasaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TrackBar brushSize;
        public System.Windows.Forms.TrackBar brushSoftness;
        public System.Windows.Forms.TrackBar brushIntensity;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.PictureBox brushPreview;
        public System.Windows.Forms.NumericUpDown levelTo;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.NumericUpDown waterLevel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox textureList;
        private System.Windows.Forms.TabPage tabPage5;
        public System.Windows.Forms.TextBox mapDesc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox MainInfoCheckBox;
        public System.Windows.Forms.TextBox mapName;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox mapAuthor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabPage6;
        public MapRenderWindow renderWindow1;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.NumericUpDown waterDensity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button pickWaterDepthColor;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton landToolSmooth;
        private System.Windows.Forms.RadioButton landToolSet;
        private System.Windows.Forms.RadioButton landToolDown;
        private System.Windows.Forms.RadioButton landToolUp;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.NumericUpDown LightmapQuality;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button LightmapGenerateButton;
        public System.Windows.Forms.NumericUpDown LightmapDiffusion;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.NumericUpDown AmbientLight;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TrackBar sunHeight;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TrackBar sunRotation;
        private System.Windows.Forms.Button SunColorPicker;
        public System.Windows.Forms.CheckBox customSetValue;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button clearLightmap;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Label PaintColorPicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button PaintColorPickWhite;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button replaceTexture;
        private System.Windows.Forms.TextBox textureFilePath;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton placeTool;
        private System.Windows.Forms.RadioButton selectTool;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button resetRotation;
        public System.Windows.Forms.CheckBox editLockZ;
        public System.Windows.Forms.CheckBox editLockY;
        public System.Windows.Forms.CheckBox editLockX;
        private System.Windows.Forms.GroupBox groupBox10;
        public System.Windows.Forms.RadioButton editRotateZ;
        public System.Windows.Forms.RadioButton editRotateY;
        public System.Windows.Forms.RadioButton editRotateX;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.RadioButton pathClearTool;
        private System.Windows.Forms.RadioButton pathBlockTool;
        private System.Windows.Forms.Button pathGenerate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown pathBias;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.RadioButton decalPlaceTool;
        private System.Windows.Forms.RadioButton decalSelectTool;
        private System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.CheckBox editAlignTerrain;
        public System.Windows.Forms.CheckBox editStickTerrain;
        public System.Windows.Forms.CheckBox isInstanced;
        public System.Windows.Forms.CheckBox placeRandomly;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Button ambientColorPicker;
        private System.Windows.Forms.Button resizeButton;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.RadioButton nodePlaceTool;
        private System.Windows.Forms.RadioButton nodeSelectTool;

    }
}