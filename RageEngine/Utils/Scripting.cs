using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using RageEngine.Debug;
using SharpDX;
using RageEngine.ContentPipeline;

namespace RageEngine {

    public class Script {
        private static Microsoft.CSharp.CSharpCodeProvider csProvider;
        private static CompilerParameters options;

        public Assembly assembly;
        public Dictionary<string,string> scripts;

        public Script() {
            scripts = new Dictionary<string,string>();
        }

        public void Load(string file) {
            StreamReader reader = Resources.GetStreamReader(file);
            Add(file, reader.ReadToEnd());
            reader.Close();
        }

        public void Add(string file,string script) {
            if (!scripts.ContainsKey(file)) scripts.Add(file, script);
        }

        public void Compile() {

            if (csProvider == null) {
                // you don't allow the use of c++, which is too volatile for scripting use - memory leaks anyone?)
                csProvider = new Microsoft.CSharp.CSharpCodeProvider();

                // Setup our options
                options = new CompilerParameters();

                options.CompilerOptions = "/optimize";
                options.WarningLevel = 1; // 3 = normal
                options.TreatWarningsAsErrors = false;
                options.GenerateExecutable = false; // we want a Dll (or "Class Library" as its called in .Net)
                options.GenerateInMemory = true; // Saves us from deleting the Dll when we are done with it, though you could set this to false and save start-up time by next time by not having to re-compile
                // And set any others you want, there a quite a few, take some time to look through them all and decide which fit your application best!

                // Add any references you want the users to be able to access, be warned that giving them access to some classes can allow
                // harmful code to be written and executed. I recommend that you write your own Class library that is the only reference it allows
                // thus they can only do the things you want them to.
                // (though things like "System.Xml.dll" can be useful, just need to provide a way users can read a file to pass in to it)
                // Just to avoid bloatin this example to much, we will just add THIS program to its references, that way we don't need another
                // project to store the interfaces that both this class and the other uses. Just remember, this will expose ALL public classes to
                // the "script"

                options.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Vector3)).Location);
                options.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Rectangle)).Location);

                options.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
                options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                options.IncludeDebugInformation=true;
            }

            //--------------

            // Remove duplicates
            /*scripts.Sort();
            Int32 index = 0;
            while (index < scripts.Count - 1) {
                if (scripts[index] == scripts[index + 1])
                    scripts.RemoveAt(index);
                else
                    index++;
            }*/

            string[] fileNames = new string[scripts.Count];
            scripts.Keys.CopyTo(fileNames, 0);

            string[] scriptsToCompile = new string[scripts.Count];
            scripts.Values.CopyTo(scriptsToCompile, 0);



            string str = Assembly.GetExecutingAssembly().Location;

            // Compile our code
            CompilerResults result;
            result = csProvider.CompileAssemblyFromSource(options, scriptsToCompile);

            if (result.Errors.HasErrors){
                string errorList = "";
                foreach(CompilerError error in result.Errors){
                    string[] file = error.FileName.Split('.');
                    int pathIndex;

                    int.TryParse(file[file.Length-2],out pathIndex);

                    errorList+= "   "+fileNames[pathIndex]+" : \t"+ (error.IsWarning?"Warning":"ERROR")+" on line " + error.Line + ":"+error.Column+" \t" +error.ErrorText + "\n";
                }
                throw new Exception("Scripting  ("+result.Errors.Count+") ERROR: \n" + errorList);
            }

            if (result.Errors.HasWarnings) {
                string errorList = "";
                foreach (CompilerError error in result.Errors) {
                    string[] file = error.FileName.Split('.');
                    int pathIndex;
                    int.TryParse(file[file.Length-2], out pathIndex);

                    errorList+= "   "+fileNames[pathIndex]+" : \t"+ (error.IsWarning?"Warning":"ERROR")+" on line " + error.Line + ":"+error.Column+" \t" +error.ErrorText + "\n";
                }
                GameConsole.Add("Scripting ("+result.Errors.Count+") WARNING: \n" + errorList);
            }

            assembly = result.CompiledAssembly;
        }

        public T Make<T>(string name) {
            return Make<T>(name, new Type[] { }, new object[] { });
        }

        public T Make<T>(string name,Type[] types, params object[] args) {

            foreach (Type type in assembly.GetExportedTypes()) {
                if (type.Name == name) {
                    ConstructorInfo constructor = type.GetConstructor(types);
                    if (constructor != null && constructor.IsPublic) {
                        T made = (T)Activator.CreateInstance(type, args);
                        return made;
                    }
                }
            }
            throw new Exception("Dynamic creation fail :'" + name+"' class not found");
        }
    }
}
