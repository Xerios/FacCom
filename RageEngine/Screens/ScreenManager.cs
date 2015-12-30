/************************************************************************/
/* Author : David Amador 
 * Web:      http://www.david-amador.com
 * Twitter : http://www.twitter.com/DJ_Link                             
 * 
 * You can use this for whatever you want. If you want to give me some credit for it that's cool but not mandatory
/************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RageEngine.Graphics.ScreenManager
{
    /// <summary>
    /// Screen Manager
    /// Keeps a list of available screens
    /// so you can switch between them, 
    /// ie. jumping from the start screen to the game screen 
    /// </summary>
    public static class ScreenManager
    {
        // Protected Members
        static private List<Screen> screens = new List<Screen>();
        static private Screen previous = null;
        // Public Members
        static public Screen ActiveScreen = null;

        /// <summary>
        /// Add new Screen
        /// </summary>
        /// <param name="screen">New screen, name must be unique</param>
        static public void AddScreen(Screen screen){
            foreach (Screen scr in screens){
                if (scr.Name == screen.Name){
                    return;
                }
            }
            screens.Add(screen);
        }

        static public void AddLayer(ScreenLayer screen) {
            if (ActiveScreen != null) {
                ActiveScreen.AddLayer(screen);
            }
        }


        /// <summary>
        /// Go to screen
        /// </summary>
        /// <param name="name">Screen name</param>
        static public void GotoScreen(string name){
            foreach (Screen screen in screens){
                if (screen.Name == name){
                    // Shutsdown Previous Screen         
                    previous = ActiveScreen;
                    if (ActiveScreen != null) ActiveScreen.Shutdown();
                    // Inits New Screen
                    ActiveScreen = screen;
                    ActiveScreen.Initialize();
                    return;
                }
            }
        }
        /// <summary>
        /// Falls back to previous selected screen if any
        /// </summary>
        static public void GoBack(){
            if (previous != null){
                GotoScreen(previous.Name);
                return;
            }
        }

        /// <summary>
        /// Updates Active Screen
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        static public void Update(){
            if (ActiveScreen!=null) ActiveScreen.Update();
        }

        static public void Draw(){
            if (ActiveScreen != null) ActiveScreen.Render();
        }

        static public void Resize() {
            if (ActiveScreen != null) ActiveScreen.Resize();
        }

        /*private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }*/
    }
}
