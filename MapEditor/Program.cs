using System;
using System.Windows.Forms;
using System.Threading;
using RageEngine.Utils;
using System.IO;

namespace MapEditor {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //ModelSelect map = new ModelSelect();
            //map.ShowDialog();

            MapSelect map = new MapSelect();
            map.ShowDialog();

            if (MainWindow.OpenedFile!=null) Application.Run(new MainWindow());

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread t = new Thread(new ThreadStart(FixDamnDragandDropIssue));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();*/
            

            //Console.WriteLine(PathHandling.RelativePath(Path.GetFullPath(Application.StartupPath+@"\..\data"),@"C:\Projects\FactionCommand_RTS\BIN\data\Game\Models\tree"));
        
        }

        /*static void FixDamnDragandDropIssue() {
            Application.Run(new MainWindow());
        }*/
    }
}

