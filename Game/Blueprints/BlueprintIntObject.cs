using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using RageEngine.Debug;
using RageEngine;
using System.IO;
using SharpDX;
using RageEngine.ContentPipeline;
using System.Collections;

namespace RageRTS {

    public struct BlueprintIntObjectPointer {
        public string name;
        public BlueprintIntObject pointer;

        public BlueprintIntObjectPointer(BlueprintIntObject tech) {
            name = tech.className;
            this.pointer = tech;
        }

        public BlueprintIntObjectPointer(string techName) {
            name = techName;
            this.pointer = BlueprintIntObject.Find(techName);
        }

        delegate object CallSiteDelegate(object[] args);

        public InteractiveObject CreateObject(InteractiveObject parent, int side,Vector3 pos,float rotation){
            InteractiveObject lastObject = BackgroundGame.Scripts_IObjects.Make<InteractiveObject>(name, new Type[2] { typeof(InteractiveObject), typeof(BlueprintIntObject) }, parent, pointer);//(InteractiveObject)Activator.CreateInstance(t, pointer);
            if (lastObject == null) throw new ArgumentNullException("TechPointer : Error creating " +  name);
            /*lastObject.Init(side, pos, rotation);
            BackgroundGame.async_IObjs.Push(lastObject);*/

            return lastObject;
        }
    }
    public class BlueprintIntObject {
        public static List<BlueprintIntObject> list = new List<BlueprintIntObject>(50);

        private static uint UNIQUE_ID = 0;

        public uint id;
        public string title = "";
        public string className = "";
        public int timeToBuild = 100;
        public int costPerTick = 0;
        //public int limit = 0;

        public string cameo="";

        public string[] unlocks;
        public string script;

        public bool wall = false;
        public bool placeable = false;

        public Point2D space;

        public BlueprintIntObject(string name) {
            id = ++BlueprintIntObject.UNIQUE_ID;
            className = name;
            title = name;
            list.Add(this);
            GameConsole.Add("BlueprintIntObject", "New blueprint added '"+className+"'");
        }

        public static BlueprintIntObject Find(string techName) {
            foreach (BlueprintIntObject tech in BlueprintIntObject.list) {
                if (tech.className == techName) return tech;
            }
            return null;
        }

        public static BlueprintIntObject Find(uint idd) {
            foreach (BlueprintIntObject tech in BlueprintIntObject.list) {
                if (tech.id == idd) return tech;
            }
            return null;
        }
        public static void Initialize() {

            Hashtable techList = SJSON.Load(Resources.FindFile("Data/TechData.fcd"));

            ArrayList techs = techList["techs"] as ArrayList;

            foreach (Hashtable tech in techs) {
                BlueprintIntObject o = new BlueprintIntObject(tech["class"] as string);
                o.title = tech["title"] as string;
                o.costPerTick = (int)(double)tech["cost_per_tick"];
                o.timeToBuild = (int)(double)tech["time_to_build"];
                o.title = tech["title"] as string;
            }

            BackgroundGame.Scripts_IObjects = new Script();
            string prefix = @"using RageRTS;
                            using RageRTS.Behaviors;
                            using RageEngine;
                            using RageEngine.Utils;
                            using RageEngine.Graphics;
                            using SharpDX;
            namespace intObjects { public class ";
            string prefix2 = ": InteractiveObject { public ";
            string prefix3 = "(InteractiveObject parent, BlueprintIntObject blueprint):base(parent,blueprint){}";
            string prefixend = "}}";

            //foreach (BlueprintIntObject tech in BlueprintIntObject.list) {
                /*tech.tickCost=(ushort)((float)tech.cost/tech.time);
                float tickCostFloat=(float)tech.cost/tech.time;
                if (tech.tickCost != tickCostFloat) GameConsole.Add("BlueprintIntObject", tech.name + " has incorrect time & cost value ("+tech.time+","+tech.cost+")");*/

            foreach (BlueprintIntObject tech in BlueprintIntObject.list) {
                string file = "Scripts\\" + (tech.className as string) + ".cs";
                if (file=="" && file.EndsWith(".cs")) continue;
                string name = Path.GetFileNameWithoutExtension(file);

                StreamReader reader = Resources.GetStreamReader(file);
                string fileScript = reader.ReadToEnd();
                BackgroundGame.Scripts_IObjects.Add(file, prefix + name + prefix2 + name + prefix3 + fileScript + prefixend);
                reader.Close();
            }
            BackgroundGame.Scripts_IObjects.Compile();
        }
    }
}
