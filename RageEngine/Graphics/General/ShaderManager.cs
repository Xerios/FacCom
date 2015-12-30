using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using RageEngine.Debug;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

namespace RageEngine.Graphics {

    public static class ShaderManager 
    {
        private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>(20);
        private static Dictionary<string, InputLayout> inputlayouts = new Dictionary<string, InputLayout>(20);

        public static ShaderVS lastVS=null;
        public static ShaderPS lastPS=null;

        public static void Dispose() {
            foreach (KeyValuePair<string, Shader> keyPair in shaders) keyPair.Value.Dispose();
            shaders.Clear();
        }

        public static Shader Add(string shaderLabel, Shader newFx) {
            if (shaderLabel != null && !shaders.ContainsKey(shaderLabel)) {
                shaders.Add(shaderLabel, newFx);
                GameConsole.Add("ShaderManager","New shader added '"+shaderLabel+"'");
            } else GameConsole.Add("ShaderManager", "Shader '"+shaderLabel+"' already exists in the dictionnary");
            return newFx;
        }

        public static Shader Get(string shaderLabel) {
            Shader s;
            shaders.TryGetValue(shaderLabel ,out s);
            return s;
        }

        public static void Apply(string shaderLabel) {
            Shader s;
            if (shaders.TryGetValue(shaderLabel, out s)) s.Apply();
        }
    }

    public interface Shader {
        void Apply();
        void Dispose();
        void SetInputLayout(InputElement[] elements);
    }

    public class ShaderVS : Shader {
        public VertexShader value;
        private ShaderSignature sign;
        private InputLayout inputLayout;

        public ShaderVS(ShaderBytecode val) {
            value = new VertexShader(Display.device, val);
            sign = ShaderSignature.GetInputSignature(val);
        }

        public void SetInputLayout(InputElement[] elements) {
            inputLayout = new InputLayout(Display.device, sign, elements);
        }

        public void Apply() {
            GraphicsManager.SetInputLayout(inputLayout);
            if (ShaderManager.lastVS!=this) {
                ShaderManager.lastVS=this;
                Display.context.VertexShader.Set(value);
            }
        }
        public void Dispose() {
            value.Dispose();
        }
    }

    public class ShaderPS : Shader {
        public PixelShader value;
        public ShaderPS(ShaderBytecode val) {
            value = new PixelShader(Display.device, val);
        }

        public void SetInputLayout(InputElement[] elements) { }

        public void Apply() {
            if (ShaderManager.lastPS!=this) {
                ShaderManager.lastPS=this;
                Display.context.PixelShader.Set(value);
            }
        }
        public void Dispose() {
            value.Dispose();
        }
    }
}
