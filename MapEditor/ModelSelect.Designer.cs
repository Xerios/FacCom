namespace MapEditor {
    partial class ModelSelect {
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Empty");
            this.modelList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // modelList
            // 
            this.modelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelList.Location = new System.Drawing.Point(3, 2);
            this.modelList.Margin = new System.Windows.Forms.Padding(4);
            this.modelList.Name = "modelList";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Empty";
            this.modelList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.modelList.ShowLines = false;
            this.modelList.Size = new System.Drawing.Size(255, 278);
            this.modelList.TabIndex = 0;
            this.modelList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.modelList_MouseDoubleClick);
            // 
            // ModelSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 282);
            this.Controls.Add(this.modelList);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ModelSelect";
            this.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select a model";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelSelect_FormClosing);
            this.Load += new System.EventHandler(this.ModelSelect_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView modelList;
    }
}