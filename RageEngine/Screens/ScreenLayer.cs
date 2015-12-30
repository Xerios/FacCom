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

namespace RageEngine.Graphics.ScreenManager
{
    public class ScreenLayer {


        public int ScreenOrder = 0;

        public string Name;

        /// <summary>
        /// Screen Constructor
        /// </summary>
        /// <param name="name">Must be unique since when you use ScreenManager is per name</param>
        public ScreenLayer(){

        }

        ~ScreenLayer(){

        }
        
        /// <summary>
        /// Virtual Function that's called when entering a Screen
        /// override it and add your own initialization code
        /// </summary>
        /// <returns></returns>
        public virtual void Initialize(){

        }

        /// <summary>
        /// Virtual Function that's called when exiting a Screen
        /// override it and add your own shutdown code
        /// </summary>
        /// <returns></returns>
        public virtual void Shutdown(){

        }

        /// <summary>
        /// Override it to have access to elapsed time
        /// </summary>
        /// <param name="elapsed">GameTime</param>
        public virtual void Update() {

        }

        public virtual void Render(){

        }

        public virtual void Resize() {

        }
    }

}
