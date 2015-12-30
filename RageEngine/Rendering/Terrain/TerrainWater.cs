using RageEngine.Graphics;
using RageEngine.Utils;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Text;
using Math = System.Math;

namespace RageEngine.Rendering {
    public class TerrainWater : Renderable{

        TerrainInfo info;
        short[] indices;


        public int IndicesLength;
        public Buffer vertexBuffer;
        public Buffer indexBuffer;
        public InputLayout layout;

        ShaderResourceView normalView,underwaterHeightView;

        public int GetRenderPass(){ return (int)RenderPass.Post; }

        public TerrainWater(TerrainInfo heightfield){
            info=heightfield;

            BuildMesh();
            Initialize();
        }


        public void Initialize() {

            // Heightmap Map to water level creation ( only required for water ) 
            //---------------------------------------------------------------------
            if (underwaterHeightView!=null) underwaterHeightView.Dispose();
            if (normalView!=null) normalView.Dispose();

            Texture2DDescription descHM = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = Format.R8_UNorm,
                Width = info.width,
                Height = info.height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
            };

            Texture2D MappableTexture = new Texture2D(Display.device, descHM);

            Surface surf=MappableTexture.QueryInterface<Surface>();

            DataStream stream;
            DataRectangle rect=surf.Map(SharpDX.DXGI.MapFlags.Write, out stream);

            stream.Position=0;

            float delta = 1;
	        //float level_clean = saturate((level-0.95f)/(1-0.95f));

            for (int y = 0; y < info.height; y++) {
                for (int x = 0; x < info.width; x++) {
                    float val = ((info.altitudeData[x, y]-(info.waterLevel-delta))/delta);

                    stream.WriteByte((byte)GameUtils.Clamp(val * 255, 0, 255));
                }
            }

            surf.Unmap();

            var desc = new Texture2DDescription();
            desc.Width = info.width;
            desc.Height = info.height;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.Format = SharpDX.DXGI.Format.R8_UNorm;
            desc.Usage = ResourceUsage.Default;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.SampleDescription = new SampleDescription(1, 0);


            Texture2D newText = new Texture2D(Display.device, desc);
            Display.context.CopyResource(MappableTexture, newText);

            MappableTexture.Dispose();

            underwaterHeightView = new ShaderResourceView(Display.device, newText);

            // Normal Map creation 
            //---------------------------------------------------------------------

            descHM = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = Format.R8G8B8A8_UNorm,
                Width = info.width,
                Height = info.height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
            };

            MappableTexture = new Texture2D(Display.device, descHM);

            surf=MappableTexture.QueryInterface<Surface>();

            DataStream streamN;
            rect=surf.Map(SharpDX.DXGI.MapFlags.Write, out streamN);

            streamN.Position=0;

            for (int y = 0; y < info.height; y++) {
                for (int x = 0; x < info.width; x++) {
                    Vector3 data = info.normalData[x, y];
                    data.X = Math.Abs(data.X);
                    data.Y = Math.Max(data.Y,0);
                    data.Z = Math.Abs(data.Z);

                    data.X = GameUtils.Clamp(data.X * 255, 0, 255);
                    data.Y = GameUtils.Clamp(data.Y * 255, 0, 255);
                    data.Z = GameUtils.Clamp(data.Z * 255, 0, 255);
                    streamN.WriteByte((byte)(data.X));
                    streamN.WriteByte((byte)(data.Y));
                    streamN.WriteByte((byte)(data.Z));
                    streamN.WriteByte(0xFF);
                }
            }
            surf.Unmap();

            desc = new Texture2DDescription();
            desc.Width = info.width;
            desc.Height = info.height;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.Usage = ResourceUsage.Default;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.SampleDescription = new SampleDescription(1, 0);


            newText = new Texture2D(Display.device, desc);
            Display.context.CopyResource(MappableTexture, newText);

            MappableTexture.Dispose();

            normalView = new ShaderResourceView(Display.device, newText);

        }

        public void BuildMesh() {

            if (indexBuffer!=null) {
                indexBuffer.Dispose();
                vertexBuffer.Dispose();
            }

            VertexTexture[] waterVertices = new VertexTexture[4];
            int i = 0;

            float x, y, width, height;

            x = 0;
            y = 0;
            width = info.width;
            height = info.height;

            float size = 1;

            waterVertices[i++] = new VertexTexture(new Vector3(x, info.waterLevel, y), new Vector2(0, 0));
            waterVertices[i++] = new VertexTexture(new Vector3(x, info.waterLevel, height), new Vector2(0, size));
            waterVertices[i++] = new VertexTexture(new Vector3(width, info.waterLevel, y), new Vector2(size, 0));
            waterVertices[i++] = new VertexTexture(new Vector3(width, info.waterLevel, height), new Vector2(size, size));

            indices = new short[] {0,3,1,0,2,3};
            IndicesLength= indices.Length;

            indexBuffer = new Buffer(Display.device, DataStream.Create(indices, false, false), new BufferDescription(indices.Length*4, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));

            vertexBuffer = Buffer.Create<VertexTexture>(Display.device, waterVertices, new BufferDescription(4*VertexTexture.SizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
            
        }


        public void Render() {

            if (info.waterLevel == 0) return;

            ShaderManager.Apply("VS_Water");
            ShaderManager.Apply("PS_Water");

            Display.context.PixelShader.SetSampler(0, DeviceStates.samplerClamp);
            Display.context.PixelShader.SetSampler(1, DeviceStates.samplerWrapAnistropy);

            Display.context.PixelShader.SetShaderResource(0, Display.backBufferCopySRV);
            Display.context.PixelShader.SetShaderResource(1, underwaterHeightView);
            Display.context.PixelShader.SetShaderResource(2, normalView);
            Display.context.PixelShader.SetShaderResource(3, info.WaterBump.ressource);
            Display.context.PixelShader.SetShaderResource(4, info.Shore.ressource);


            GraphicsManager.SetBlendState(DeviceStates.blendStateSolid);
            GraphicsManager.SetDepthState(DeviceStates.depthDefaultState);
            GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            Display.context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
            Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexTexture.SizeInBytes, 0));

            Display.context.DrawIndexed(IndicesLength, 0, 0);

            Display.context.ResolveSubresource(Display.backBuffer, 0, Display.backBufferCopy, 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm);

        }
    }
}
