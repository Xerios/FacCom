using System.Collections.Generic;
using System.Text;
using RageEngine.Utils;
using SharpDX.Direct3D11;
using RageEngine.Rendering;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX;
using RageEngine.Graphics.TwoD;

namespace RageEngine.Graphics {
    public static class SceneManager {
        public static Camera Camera;

        private static Buffer Buffer;
		public static RenderPass Pass;

        private static Renderable[] Renderable_Solids;
        private static Renderable[] Renderable_Transparents;
        private static Renderable[] Renderable_Posts;
        private static Renderable[] Renderable_Shadows;


        // Debug stuff
        public static LineManager LineManager;
        public static DotManager DotManager;

        public static void Initialize() {
            Pass = RenderPass.None;
            Renderable_Solids = new Renderable[0];
		    Renderable_Transparents = new Renderable[0];
		    Renderable_Posts = new Renderable[0];
            Renderable_Shadows=new Renderable[0];

            Buffer=new Buffer(Display.device, Utilities.SizeOf<GlobalConstant>(), ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            //Debug
            DotManager=new DotManager();
            LineManager=new LineManager();
            Add(DotManager);
            Add(LineManager);
		}

        public static void Add(Renderable renderable) {
		    int pass = renderable.GetRenderPass();

		    if ((pass & (int)RenderPass.Solid        )== (int)RenderPass.Solid        ) AddToRenderList(ref Renderable_Solids       , ref renderable);
		    if ((pass & (int)RenderPass.Transparent  )== (int)RenderPass.Transparent  ) AddToRenderList(ref Renderable_Transparents , ref renderable);
		    if ((pass & (int)RenderPass.Post         )== (int)RenderPass.Post         ) AddToRenderList(ref Renderable_Posts        , ref renderable);
		    if ((pass & (int)RenderPass.Shadows      )== (int)RenderPass.Shadows      ) AddToRenderList(ref Renderable_Shadows      , ref renderable);
		}

        private static void AddToRenderList(ref Renderable[] list, ref Renderable renderable) {
			if (list == null) {
				list = new Renderable[1];
				list[0] = renderable;
			} else {
				System.Array.Resize(ref list, (list.Length + 1));
				list[list.Length - 1] = renderable;
			}
		}

        public static void Render() {
			int count;

            PrepareConstant();

			//*****************************
			#if DEBUG 
			PixHelper.BeginEvent(Color.Orange, "SM : Solid Rendering");
			#endif
			Pass = RenderPass.Solid;
			count = Renderable_Solids.Length;
			for (int i = 0; i < count; i++) Renderable_Solids[i].Render();
			#if DEBUG 
			PixHelper.EndEvent();
			#endif

			//*****************************
			#if DEBUG 
			PixHelper.BeginEvent(Color.Orange, "SM : Transparent Rendering");
			#endif
			Pass = RenderPass.Transparent;
			count = Renderable_Transparents.Length;
			for (int i = 0; i < count; i++) Renderable_Transparents[i].Render();
			#if DEBUG 
			PixHelper.EndEvent();
			#endif

			//*****************************
            Display.context.ResolveSubresource(Display.backBuffer, 0, Display.backBufferCopy, 0, Format.R8G8B8A8_UNorm);

			#if DEBUG 
			PixHelper.BeginEvent(Color.Orange, "SM : Special Rendering");
			#endif
			Pass = RenderPass.Post;
			count = Renderable_Posts.Length;
			for (int i = 0; i < count; i++) Renderable_Posts[i].Render();
			#if DEBUG 
			PixHelper.EndEvent();
			#endif

		}

        public static void PrepareConstant() {

            var globals = new GlobalConstant() {
                CameraPosition = new Vector4(Camera.eyePosition,0),
                ViewProjection = Matrix.Transpose(Camera.ViewProjection),
                Time = GlobalConstantVars.Time,
                AmbientColor = GlobalConstantVars.AmbientColor,
                SunColor = GlobalConstantVars.SunColor,
                SunDirection = GlobalConstantVars.SunDirection,
                EyeHeight = Camera.eye.Y,
            };

            DataStream mappedResource;
            Display.context.MapSubresource(Buffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
            mappedResource.Write(globals);
            Display.context.UnmapSubresource(Buffer, 0);

            Display.context.VertexShader.SetConstantBuffer(0, Buffer);
            Display.context.PixelShader.SetConstantBuffer(0, Buffer);

        }

	}
}
