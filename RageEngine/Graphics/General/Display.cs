using RageEngine.ContentPipeline;
using RageEngine.Debug;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Buffer=SharpDX.Direct3D11.Buffer;
using Device=SharpDX.Direct3D11.Device;
using MapFlags=SharpDX.Direct3D11.MapFlags;

namespace RageEngine.Graphics {
    public class Display {

        public static SwapChain swapChain;
        public static Device device;
        public static DeviceContext context;
        public static RenderTargetView renderTarget;
        public static DepthStencilView depthStencil;

        public static ShaderResourceView backBufferCopySRV;
        public static Texture2D backBuffer, backBufferCopy;

        protected static Texture2DDescription depthBufferDesc, backBufferCopyDesc;

        public static Matrix screenMatrix;
        public static Buffer screenBuffer;

        public static int Antialias = 8;
        public static int Width = 1024, Height = 768;
        public static bool Fullscreen = false;

        public static void SetupDisplay(Control control) {

            Display.Width=control.ClientSize.Width;
            Display.Height=control.ClientSize.Height;

            var description=new SwapChainDescription() {
                BufferCount=1,
                Usage=Usage.RenderTargetOutput,
                OutputHandle=control.Handle,
                IsWindowed=!Display.Fullscreen,
                ModeDescription=new ModeDescription(0, 0, new Rational(), Format.R8G8B8A8_UNorm),
                SampleDescription=new SampleDescription(Antialias, 0),
                Flags=SwapChainFlags.AllowModeSwitch,
                SwapEffect=SwapEffect.Discard
            };

            var factory=new Factory();
            Logger.Log("D3D11 Factory initialized.");

            int adapterCount=factory.GetAdapterCount();
            Logger.Log("D3D11 Amount of adapters: "+adapterCount);

            if (adapterCount==0) {
                Logger.Log("ERROR : No adapters found !");
                throw new Exception("ERROR : No adapters found !");
            }

            bool perfHUDenabled=false;
            Adapter adapter=null;
            for (int i=0; i<adapterCount; i++) {
                Adapter ad=factory.GetAdapter(i);
                Logger.Log("D3D11 Adapter "+i+" : ("+ad.Description.DeviceId+") "+ad.Description.Description);
                if (ad.Description.Description=="NVIDIA PerfHUD") {
                    perfHUDenabled=true;
                    adapter=ad;
                }
            }
            if (adapter==null) adapter=factory.GetAdapter(0);

            Logger.Log("D3D11 Adapter chosen : "+adapter.Description.Description);

            if (perfHUDenabled) {
                Device.CreateWithSwapChain(adapter, DeviceCreationFlags.None, description, out device, out swapChain);
            } else {
                try {
#if DEBUG
                    try {
                        device=new Device(adapter, DeviceCreationFlags.Debug);
                    } catch (Exception e) {
                        Logger.Log("! Warning : D3D11 Could not init DEBUG Device. ("+e.Message+")");
                        device=new Device(adapter, DeviceCreationFlags.None);
                    }
#else
			    device = new Device( adapter,    DeviceCreationFlags.None );
#endif
                } catch (Exception e) {
                    Logger.Log("! ERROR : D3D11 Could not init the Device at all ! ("+e.Message+")");
                    device=new Device(adapter, DeviceCreationFlags.None);
                }
            }
            Logger.Log("D3D11 Device initialized.");

            bool supConcurrentRess, supCommandList;
            device.CheckThreadingSupport(out supConcurrentRess, out supCommandList);
            Logger.Log("D3D11 Concurrent ressource load supported : "+supConcurrentRess);
            Logger.Log("D3D11 Command lists supported : "+supCommandList);

            // Main textures
            Logger.Log("D3D11 Format R8G8B8A8_UNorm supported : "+device.CheckFormatSupport(Format.R8G8B8A8_UNorm).HasFlag(FormatSupport.RenderTarget));
            Logger.Log("D3D11 Format D24_UNorm_S8_UInt supported : "+device.CheckFormatSupport(Format.D24_UNorm_S8_UInt).HasFlag(FormatSupport.DepthStencil));
            Logger.Log("D3D11 Format D32_Float supported : "+device.CheckFormatSupport(Format.D32_Float).HasFlag(FormatSupport.DepthStencil));

            //Shadow textures
            Logger.Log("D3D11 Format R16_Typeless supported : "+device.CheckFormatSupport(Format.R16_Typeless).HasFlag(FormatSupport.Texture2D));
            Logger.Log("D3D11 Format D16_UNorm supported : "+device.CheckFormatSupport(Format.D16_UNorm).HasFlag(FormatSupport.Texture2D));
            Logger.Log("D3D11 Format R16_UNorm supported : "+device.CheckFormatSupport(Format.R16_UNorm).HasFlag(FormatSupport.Texture2D));

            swapChain=new SwapChain(factory, device, description);
            factory.MakeWindowAssociation(Main.Form.Handle, WindowAssociationFlags.IgnoreAll);

            //device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 1);
            //swapChain.
            Logger.Log("D3D11 SwapChain initialized.");

            Display.context=device.ImmediateContext;

            //Display.width = Main.Form.ClientSize.Width;
            //Display.height = Main.Form.ClientSize.Height;
            //Global.Viewport = new Rectangle(0, 0, (int)Display.width, (int)Display.height);


            Result res=Display.device.DeviceRemovedReason;
            if (res.Failure) {
                Logger.Log("! ERROR : a weird one : "+res.Code);
                throw new Exception("ERROR, this is weird, here's the reason why : "+res.ToString());
            }

            if (device.IsDisposed) {
                Logger.Log("! ERROR : a REALLY weird one.");
                throw new Exception("DISPOSED ERROR, this is FUCKING weird ... ");
            }

            backBuffer=SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTarget=new RenderTargetView(device, backBuffer);

            Logger.Log("D3D11 Backbuffer initialized.");

            backBufferCopyDesc=new Texture2DDescription() {
                ArraySize=1,
                BindFlags=BindFlags.ShaderResource,
                CpuAccessFlags=CpuAccessFlags.None,
                Format=Format.R8G8B8A8_UNorm,
                Width=Display.Width,
                Height=Display.Height,
                MipLevels=1,
                OptionFlags=ResourceOptionFlags.None,
                SampleDescription=new SampleDescription(1, 0),
                Usage=ResourceUsage.Default,
            };

            backBufferCopy=new Texture2D(Display.device, backBufferCopyDesc);
            backBufferCopySRV=new ShaderResourceView(Display.device, backBufferCopy);

            Logger.Log("D3D11 Backbuffer copy initialized.");

            depthBufferDesc=new Texture2DDescription() {
                ArraySize=1,
                BindFlags=BindFlags.DepthStencil,
                CpuAccessFlags=CpuAccessFlags.None,
                Format=Format.D24_UNorm_S8_UInt,
                Width=Display.Width,
                Height=Display.Height,
                MipLevels=1,
                OptionFlags=ResourceOptionFlags.None,
                SampleDescription=description.SampleDescription,
                Usage=ResourceUsage.Default,
            };

            using (var depthBuffer=new Texture2D(device, depthBufferDesc))
                depthStencil=new DepthStencilView(device, depthBuffer);

            Logger.Log("D3D11 Depthbuffer initialized.");

            DeviceStates.Initialize();

            Logger.Log("D3D11 Device states initialized.");

            context.OutputMerger.DepthStencilState=DeviceStates.depthDefaultState;
            context.Rasterizer.State=DeviceStates.rastStateSolid;
            context.OutputMerger.BlendState=DeviceStates.blendStateSolid;

            context.OutputMerger.SetTargets(depthStencil, renderTarget);

            context.Rasterizer.SetViewports(new Viewport(0, 0, Display.Width, Display.Height, 0.0f, 1.0f));

            Logger.Log("D3D11 engine states set.");

            screenMatrix=Matrix.Scaling(2f/Display.Width, -2f/Display.Height, 0)*Matrix.Translation(-1, 1, 0);
            screenMatrix.Transpose();
            screenBuffer=new Buffer(device, DataStream.Create<Matrix>(new[] { screenMatrix }, false, false), 64, ResourceUsage.Immutable, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            Logger.Log("D3D11 screen buffer created.");
        }

        public static void ResizeDisplay(Control control) {
            if (renderTarget==null) return;

            renderTarget.Dispose();
            depthStencil.Dispose();
            
            backBuffer.Dispose();
           
            backBufferCopy.Dispose();
            backBufferCopySRV.Dispose();

            Display.Width=control.ClientSize.Width;
            Display.Height=control.ClientSize.Height;
            context.Rasterizer.SetViewports(new Viewport(0, 0, Display.Width, Display.Height, 0.0f, 1.0f));

            swapChain.ResizeBuffers(2, 0, 0, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);

            backBuffer=SharpDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTarget=new RenderTargetView(device, backBuffer);

            depthBufferDesc.Width=Display.Width;
            depthBufferDesc.Height=Display.Height;

            using (var depthBuffer=new Texture2D(device, depthBufferDesc))
                depthStencil=new DepthStencilView(device, depthBuffer);

            context.OutputMerger.SetTargets(depthStencil, renderTarget);

            backBufferCopyDesc.Width=Display.Width;
            backBufferCopyDesc.Height=Display.Height;

            backBufferCopy=new Texture2D(Display.device, backBufferCopyDesc);
            backBufferCopySRV=new ShaderResourceView(Display.device, backBufferCopy);

            screenMatrix=Matrix.Scaling(2f/Display.Width, -2f/Display.Height, 0)*Matrix.Translation(-1, 1, 0);
            screenMatrix.Transpose();
            screenBuffer.Dispose();
            screenBuffer=new Buffer(device, DataStream.Create<Matrix>(new[] { screenMatrix }, false, false), 64, ResourceUsage.Immutable, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

        }

        public static void Dispose() {

            renderTarget.Dispose();
            Logger.Log("Rendertarget disposed.");

            swapChain.Dispose();
            Logger.Log("SwapChain disposed.");

            device.Dispose();
            Logger.Log("Device disposed.");
        }

    }
}
