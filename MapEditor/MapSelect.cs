using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RageEngine.Utils;

namespace MapEditor {
    public partial class MapSelect : Form {
        public MapSelect() {
            InitializeComponent();

        }

        private const string createNew ="--- New Map ---";

        private void MapSelect_Load(object sender, EventArgs e) {
            string direct = @"..\data\Maps\";
            string[] files = Directory.GetDirectories(direct);

            foreach (String file in files) {
                string name = file.Substring(direct.Length, file.Length - direct.Length);
                if (name=="Shared" || name==".svn") continue;
                mapList.Items.Add(name);
            }

            mapList.Items.Add(createNew);

            mapList.SelectedIndex = 0;
            this.AcceptButton = openButton;
        }

        private void openButton_Click(object sender, EventArgs e) {

            if (mapList.SelectedItem.Equals(createNew)) {
                MainWindow.OpenedFile = "";
                throw new Exception("SORRY BUT THIS HAS NOT YET IMPLEMENTED !");
            }else{
                MainWindow.OpenedFile = (string)(mapList.SelectedItem);
            }

            this.Dispose();
        }

        private void mapList_MouseDoubleClick(object sender, MouseEventArgs e) {
            openButton_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Dispose();
        }

        private void mapList_SelectedIndexChanged(object sender, EventArgs e) {
            if (mapList.SelectedItem.Equals(createNew)) {
                openButton.Text = "Create";
            } else {
                openButton.Text = "Open";
            }
        }
    }
}
