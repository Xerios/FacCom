using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine;
using System.IO;
using SharpDX;
using RageEngine.ContentPipeline;

namespace RageRTS
{
    public static class BlueprintProjectile
    {
        public static void Initialize(string folder) {

            string[] files = Directory.GetFiles(Resources.FindFile(folder));

            BackgroundGame.Scripts_Projectiles = new Script();
            string prefix = @"using RageRTS;
                            using RageEngine;
                            using RageEngine.Utils;
                            using RageEngine.Graphics;
                            using SharpDX;
            namespace Projectiles { public class ";
            string prefix2 =  ": Projectile { public ";
            string prefix3 = "(Vector3 initPos, Vector3 dest, InteractiveObject target1):base(initPos,dest,target1){}";
            string prefixend = "}}";


            foreach (String file in files) {
                if (file=="" && file.EndsWith(".cs")) continue;
                string name = Path.GetFileNameWithoutExtension(file);
                StreamReader reader = Resources.GetStreamReader(file);
                string fileScript = reader.ReadToEnd();
                BackgroundGame.Scripts_Projectiles.Add(file, prefix + name + prefix2 + name + prefix3 + fileScript + prefixend);
                reader.Close();
            }
            BackgroundGame.Scripts_Projectiles.Compile();
        }

        public static void CreatObject(string name, Vector3 initPos, Vector3 dest, InteractiveObject target1) {
            //Projectile proj = BackgroundGame.Scripts_Projectiles.Make<Projectile>(name, new Type[3] { typeof(Vector3), typeof(Vector3), typeof(InteractiveObject) }, new object[3] { initPos, dest, target1 });
            //BackgroundGame.Projectiles.Add(proj);
        }
    }
}
