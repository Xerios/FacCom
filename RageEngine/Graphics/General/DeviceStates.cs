using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX;

namespace RageEngine.Graphics {
    public static class DeviceStates {

        public static DepthStencilState depthDefaultState;
        public static DepthStencilState depthWriteOff;
        public static DepthStencilState depthDisabled;
        //public static DepthStencilState depthDecals;

        public static BlendState blendStateSolid;
        public static BlendState blendStateTrans;
        public static BlendState blendStateAdd;
        public static BlendState blendStateShadow;
        public static BlendState blendStateDrawing;
        public static BlendState blendStateDifference;

        public static RasterizerState rastStateSolid;
        public static RasterizerState rastStateDrawing;
        public static RasterizerState rastStateWire;
        public static RasterizerState rastStateNoCull;

        public static SamplerState samplerClamp;
        public static SamplerState samplerWrap;
        public static SamplerState samplerWrapAnistropy;

        public static void Initialize() {

            var sampler=new SamplerStateDescription() {
                Filter=Filter.MinMagMipLinear,
                AddressU=TextureAddressMode.Wrap,
                AddressV=TextureAddressMode.Wrap,
                AddressW=TextureAddressMode.Wrap,
                BorderColor=Color.Black,
                ComparisonFunction=Comparison.Never,
                MaximumAnisotropy=16,
                MipLodBias=0,
                MinimumLod=-float.MaxValue,
                MaximumLod=float.MaxValue
            };

            sampler.AddressU=TextureAddressMode.Clamp;
            sampler.AddressV=TextureAddressMode.Clamp;
            samplerClamp=new SamplerState(Display.device, sampler);

            sampler.AddressU=TextureAddressMode.Wrap;
            sampler.AddressV=TextureAddressMode.Wrap;
            samplerWrap=new SamplerState(Display.device, sampler);

            sampler.Filter=Filter.Anisotropic;
            samplerWrapAnistropy=new SamplerState(Display.device, sampler);

            var dssd = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };
            
            depthDefaultState = new DepthStencilState(Display.device, dssd);

            var dssdwriteOff = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero,
                DepthComparison = Comparison.Less,
            };

            depthWriteOff = new DepthStencilState(Display.device, dssdwriteOff);

            var dssdisabled = new DepthStencilStateDescription {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
            };

            depthDisabled = new DepthStencilState(Display.device, dssdisabled);

            var solidParentOp = new RenderTargetBlendDescription();
            solidParentOp.IsBlendEnabled=false;
            solidParentOp.RenderTargetWriteMask = ColorWriteMaskFlags.All;

            var transParentOp = new RenderTargetBlendDescription {
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Green | ColorWriteMaskFlags.Blue,
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,

                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.Zero,

                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
            };

            var addParentOp = new RenderTargetBlendDescription {
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Green | ColorWriteMaskFlags.Blue,
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,

                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.Zero,
                
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.One,
            };


            var shadowParentOp = new RenderTargetBlendDescription();
            shadowParentOp.IsBlendEnabled=false;
            shadowParentOp.RenderTargetWriteMask = 0;


            var drawingParentOp = new RenderTargetBlendDescription {
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.All,
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,

                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.One,

                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
            };


            var differenceParentOp = new RenderTargetBlendDescription {
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Green | ColorWriteMaskFlags.Blue,
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,

                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.One,

                SourceBlend = BlendOption.InverseDestinationColor,
                DestinationBlend = BlendOption.InverseSourceColor,
            };

            blendStateSolid = FromDesc(solidParentOp);
            blendStateTrans = FromDesc(transParentOp);
            blendStateAdd =   FromDesc(addParentOp);
            blendStateShadow = FromDesc(shadowParentOp);
            blendStateDrawing = FromDesc(drawingParentOp);
            blendStateDifference =  FromDesc(differenceParentOp);

            var rastState = new RasterizerStateDescription();
            rastState.CullMode = CullMode.Front;
            rastState.FillMode = FillMode.Solid;
            rastState.IsFrontCounterClockwise = true;
            rastState.IsMultisampleEnabled = true;
            rastState.IsDepthClipEnabled=true;
            rastState.IsAntialiasedLineEnabled = true;
            rastStateSolid = new RasterizerState(Display.device, rastState);

            rastState.CullMode = CullMode.None;
            //rastState.IsScissorEnabled=true;
            rastStateDrawing = new RasterizerState(Display.device, rastState);

            var rastStatewire = new RasterizerStateDescription();
            rastStatewire.CullMode = CullMode.None;
            rastStatewire.FillMode = FillMode.Wireframe;
            rastStatewire.IsFrontCounterClockwise = true;
            rastStatewire.IsDepthClipEnabled = true;
            rastStateWire = new RasterizerState(Display.device, rastStatewire);

            var rastStateNoCull1 = new RasterizerStateDescription();
            rastStateNoCull1.CullMode = CullMode.None;
            rastStateNoCull1.FillMode = FillMode.Solid;
            rastStateNoCull1.IsFrontCounterClockwise = true;
            rastStateNoCull1.IsDepthClipEnabled = true;
            rastStateNoCull1.IsDepthClipEnabled=true;
            rastStateNoCull = new RasterizerState(Display.device, rastStateNoCull1);
        }

        internal static BlendState FromDesc(RenderTargetBlendDescription desc) {
            var temp = new BlendStateDescription();
            temp.RenderTarget[0] = desc;

            return new BlendState(Display.device, temp);
        }


        /*public static BlendState Multiply = new BlendState {
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static BlendState Screen = new BlendState {
            ColorSourceBlend = Blend.InverseDestinationColor,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static BlendState Darken = new BlendState {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Min,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static BlendState Lighten = new BlendState {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Max,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static BlendState LinearDodge = new BlendState {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static BlendState LinearBurn = new BlendState {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        }; */
    }
}
