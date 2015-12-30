namespace MapEditor {
    partial class MapSelect {
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
            this.mapList = new System.Windows.Forms.ListBox();
            this.openButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mapList
            // 
            this.mapList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mapList.FormattingEnabled = true;
            this.mapList.ItemHeight = 16;
            this.mapList.Location = new System.Drawing.Point(3, 3);
            this.mapList.Name = "mapList";
            this.mapList.Size = new System.Drawing.Size(318, 164);
            this.mapList.TabIndex = 0;
            this.mapList.SelectedIndexChanged += new System.EventHandler(this.mapList_SelectedIndexChanged);
            this.mapList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mapList_MouseDoubleClick);
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(227, 169);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(94, 25);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MapSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 196);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.mapList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MapSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Map";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MapSelect_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox mapList;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button button1;
    }
}