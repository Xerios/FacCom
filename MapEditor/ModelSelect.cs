using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MapEditor {
    public partial class ModelSelect : Form {

        public static string selectedDirectory = "";

        public ModelSelect() {
            InitializeComponent();
        }

        private void ModelSelect_Load(object sender, EventArgs e) {
           string path =  @"..\data\";

           modelList.Nodes.Clear();

           string[] files = Directory.GetDirectories(path);

           foreach (String file in files) {
               string name = file.Substring(path.Length, file.Length - path.Length);
               if (!Directory.Exists(path+"/"+name+"/Models")) continue;

               modelList.Nodes.Add(name);
               PopulateTreeView(file+"/Models", modelList.Nodes[0]);
           }
           modelList.ExpandAll();
        }

        string substringDirectory;

        public void PopulateTreeView(string directoryValue, TreeNode parentNode) {
            string[] directoryArray = Directory.GetDirectories(directoryValue);

            try {
                if (directoryArray.Length != 0) {
                    foreach (string directory in directoryArray) {
                        substringDirectory = directory.Substring(
                        directory.LastIndexOf('\\') + 1,
                        directory.Length - directory.LastIndexOf('\\') - 1);

                        if (substringDirectory.Contains(".svn")) continue;

                        TreeNode myNode = new TreeNode(substringDirectory);
                        myNode.ToolTipText = directory;
                        parentNode.Nodes.Add(myNode);

                        PopulateTreeView(directory, myNode);
                    }
                }
            } catch (UnauthorizedAccessException) {
                parentNode.Nodes.Add("Access denied");
            } // end catch
        }

        private void modelList_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (modelList.SelectedNode != null && modelList.SelectedNode.Nodes.Count==0) {
                selectedDirectory = modelList.SelectedNode.ToolTipText;
                this.Dispose();
            }
        }

        private void ModelSelect_FormClosing(object sender, FormClosingEventArgs e) {
            if(e.CloseReason == CloseReason.UserClosing) 
                selectedDirectory="";
        }

    }
}
