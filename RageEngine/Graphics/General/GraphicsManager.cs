using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace RageEngine.Graphics {
    public static class GraphicsManager {

        public static void Reset() {
            lastDepthState=null;
            lastBlendState=null;
            lastLayout=null;
            lastTopology = PrimitiveTopology.Undefined;
            ShaderManager.lastVS=null;
            ShaderManager.lastPS=null;
        }

        public static bool WireframeAlwaysOn = false;

        private static RasterizerState lastRastState;
        private static DepthStencilState lastDepthState;
        private static BlendState lastBlendState;
        private static InputLayout lastLayout;
        private static PrimitiveTopology lastTopology;



        public static void SetInputLayout(InputLayout state) {
            if (lastLayout != state) {
                lastLayout = state;
                Display.context.InputAssembler.InputLayout = state;
            }
        }

        public static void SetRastState(RasterizerState state) {
            if (WireframeAlwaysOn) state = DeviceStates.rastStateWire;
            if (lastRastState != state) {
                lastRastState = state;
                Display.context.Rasterizer.State = state;
            }
        }

        public static void SetDepthState(DepthStencilState state) {
            if (lastDepthState != state) {
                lastDepthState = state;
                Display.context.OutputMerger.DepthStencilState = state;
            }
        }

        public static void SetBlendState(BlendState state) {
            if (lastBlendState != state) {
                lastBlendState = state;
                Display.context.OutputMerger.BlendState = state;
            }
        }

        public static void SetPrimitiveTopology(PrimitiveTopology state) {
            if (lastTopology != state) {
                lastTopology = state;
                Display.context.InputAssembler.PrimitiveTopology = lastTopology;
            }
        }


    }
}
