using RageRTS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class ResizeMap : Form
    {
        public ResizeMap() {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
            this.Dispose();
        }

        private void okButton_Click(object sender, EventArgs e) {

            // Do something!

            this.Close();
            this.Dispose();
        }

        private void ResizeMap_Load(object sender, EventArgs e) {
            widthValue.Value = (decimal)ForegroundGame.Map.width;
            heightValue.Value = (decimal)ForegroundGame.Map.height;
        }


    }
}
