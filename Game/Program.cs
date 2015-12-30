using System;
using System.Collections.Generic;
using System.Text;
using RageEngine;
using RageEngine.Debug;

namespace RageRTS {

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program {

        [STAThread]
        static void Main() {
            

#if !DEBUG
            try{
                Game game = new Game();
                game.Run();

            } catch (Exception e) {
                using (ErrorBox error = new ErrorBox(e))
                    error.ShowDialog();
            }
#else
            Game game = new Game();
            game.Run();
#endif
            Global.IsRunning = false;
        }
        public static bool AlmostEquals(this double double1, double double2, double precision) {
            return (Math.Abs(double1 - double2) <= precision);
        }
    }


}
