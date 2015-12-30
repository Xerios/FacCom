using System;
using System.Collections.Generic;
using System.Text;
using RageEngine.Utils;
using System.IO;
using SharpDX;
using RageEngine.ContentPipeline;
using RageEngine.Input;

namespace RageEngine.Graphics.TwoD.GUI {


    public static class GUIManager {
        public static Script scripts;
        public static Element hoveredOn,focusedOn;
        public static List<Element> list = new List<Element>();
        public static Dictionary<string, Element> nameList = new Dictionary<string, Element>();

        public static Element container;

        public static Element Get(string str) {
            return nameList[str];
        }

        public static void Add(Element element) {
            container.Add(element);
        }

        public static void AddIfNotExists(Element element) {
            if (!Exists(element)) container.Add(element);
        }

        public static bool Exists(Element element) {
            return (container.Elements.Find(e => e==element)!=null);
        }

        public static void Remove(Element element) {
            container.Remove(element);
        }

        public static void Update() {
            container.Size.X = Display.Width;
            container.Size.Y = Display.Height;

            hoveredOn = null;
            container.InnerUpdate();
            if (hoveredOn==container) 
                hoveredOn=null;
            else if (GUIManager.hoveredOn !=null) 
                GUIManager.hoveredOn.mouseOver=true;
        }

        public static void Draw() {
            container.InnerDraw();
        }

        public static void MouseDown(MouseInputEventArgs obj) {
            focusedOn=null;
            if (obj.IsLeftButtonDown && container.MouseDown()) obj.Handled=true;
        }

        public static void MouseUp(MouseInputEventArgs obj) {
            if (container.MouseUp()) obj.Handled=true;
        }

        //----------------------------------

        public static void Init(string folder="") {

            container = new Element();
            container.Unclickable = true;
            container.InnerAlign = GUIAlign.None;

            InputManager.MouseDown += MouseDown;
            InputManager.MouseUp   += MouseUp;

            if (folder!="") {
                string[] files = Directory.GetFiles(folder);

                scripts = new Script();
                string prefix = @"
using System;
using System.Collections.Generic;
using System.Text;
using RageEngine.Utils;
using System.IO;
using SharpDX;
using RageEngine.ContentPipeline;
using RageEngine.Input;
namespace Elements { public class ";
                string prefix2 = ": Element { public ";
                string prefix3 = "():base(){}";
                string prefixend = "}}";


                foreach (String file in files) {
                    if (file == "") continue;
                    string name = Path.GetFileNameWithoutExtension(file);
                    StreamReader reader = Resources.GetStreamReader(file);
                    string fileScript = reader.ReadToEnd();
                    scripts.Add(file, prefix + name + prefix2 + name + prefix3 + fileScript + prefixend);
                    reader.Close();
                }
                scripts.Compile();
            }
        }

        public static Element CreatElement(string type, string name = null) {
            return scripts.Make<Element>(type);
        }

    }

}
