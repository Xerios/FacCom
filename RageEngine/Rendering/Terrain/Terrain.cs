#region Using Statements
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using RageEngine.Utils;
using RageEngine.Graphics;
using SharpDX.Direct3D11;
using SharpDX;
using Buffer=SharpDX.Direct3D11.Buffer;
using SharpDX.DXGI;
#endregion

namespace RageEngine.Rendering
{
    public class Terrain : Renderable {

        public int GetRenderPass() { return (int)RenderPass.Solid; }

        public TerrainInfo info;
        private VertexTexture[] vertices;
        private Vector3[] normals;
        private int[] indices;

        public int[] IndicesLength;
        public Buffer[] vertexBuffer;
        public Buffer[] indexBuffer;

        Buffer objectBuffer;
        ShaderResourceView normalView;


        public Terrain(TerrainInfo heightfield){

            info = heightfield;

            IndicesLength = new int[3];
            indexBuffer = new Buffer[3];
            vertexBuffer = new Buffer[3];
            

            RebuildTerrain(0, 1);

            normals = new Vector3[info.width * info.height];
            Vector3 normal;
            for (int i = 0; i < vertices.Length - info.width; i++) {
                Vector3 firstvec = vertices[i + 1].Position - vertices[i].Position;
                Vector3 secondvec = vertices[i].Position - vertices[i + info.width].Position;

                normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();

                normals[i] += normal;
                normals[i + 1] += normal;
                normals[i + info.width] += normal;
            }

            for (int i = 0; i < normals.Length; i++) {
                normals[i].Normalize();
                int potntialY=  (int)Math.Floor((float)i / info.width);
                int potntialX= i - potntialY * info.width;
                info.normalData[potntialX,potntialY] = normals[i];
            }

            RebuildTerrain(1, 4);
            RebuildTerrain(2, 8);

            var buff = new TerrainConstant() {
                WaterColor = info.waterColor,
                WaterHeight = info.waterLevel
            };
            objectBuffer = Buffer.Create(Display.device, ref buff, new BufferDescription(16*2, ResourceUsage.Immutable, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));


            Initialize();
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct TerrainConstant { // SYNC THIS WITH MAP EDITOR & ENGINE
            public Vector4 WaterColor;
            public float WaterHeight;
            // 3 float Space lost !!!
        }

        public void RebuildTerrain(int buffer,float quality) {

            int resX = (int)Math.Floor(info.width / quality);
            int resY = (int)Math.Floor(info.height / quality);

            float tileX = (float)info.width  / resX;
            float tileY = (float)info.height / resY;


            /********* calc vertices *************/
            
            vertices = new VertexTexture[resX * resY];
  
            int pos=0;
            for (int x = 0; x < resX; x++) {
                for (int y = 0; y < resY; y++) {
                    pos = x + y * resX;
                    vertices[pos].Position = new Vector3(tileX * x, 0, tileY * y);
                    info.GetHeight(vertices[pos].Position,ref vertices[pos].Position.Y);
                    vertices[pos].TexCoord.X = (float)x / resX;
                    vertices[pos].TexCoord.Y = (float)y / resY;

                    if (x==resX-1) {
                        vertices[pos].Position.X = info.width;
                        vertices[pos].TexCoord.X = 1;
                    }
                    if (y==resY-1) {
                        vertices[pos].Position.Z = info.height;
                        vertices[pos].TexCoord.Y = 1;
                    }
                }
            }

            //*********** calc indexes ********************

            indices = new int[(resX - 1) * (resY - 1) * 6];
            int counter = 0;
            for (uint y = 0; y < resY - 1; y++) {
                for (uint x = 0; x < resX - 1; x++) {
                    int topLeft = (int)(x + y * resX);
                    int topRight = (int)((x + 1) + y * resX);
                    int lowerLeft = (int)(x + (y + 1) * resX);
                    int lowerRight = (int)((x + 1) + (y + 1) * resX);

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;
                }
            }

            IndicesLength[buffer]=indices.Length;
            indexBuffer[buffer] = new Buffer(Display.device, DataStream.Create(indices, false, false), new BufferDescription(indices.Length*4, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));

            vertexBuffer[buffer] = Buffer.Create<VertexTexture>(Display.device, vertices, new BufferDescription(vertices.Length*VertexTexture.SizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
        }


        public void Initialize() {

            // Normal Map creation 
            //---------------------------------------------------------------------

            Texture2DDescription descHM = new Texture2DDescription() {
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

            Texture2D MappableTexture = new Texture2D(Display.device, descHM);

            Surface surf=MappableTexture.QueryInterface<Surface>();

            DataStream streamN;
            DataRectangle rect=surf.Map(SharpDX.DXGI.MapFlags.Write, out streamN);

            streamN.Position=0;

            for (int y = 0; y < info.height; y++) {
                for (int x = 0; x < info.width; x++) {
                    Vector3 data = info.normalData[x,y];
                    data.X = GameUtils.Clamp((data.X * 0.5f + 0.5f) * 255,0,255);
                    data.Y = GameUtils.Clamp((data.Y * 0.5f + 0.5f) * 255,0,255);
                    data.Z = GameUtils.Clamp((data.Z * 0.5f + 0.5f) * 255,0,255);
                    streamN.WriteByte((byte)(data.X));
                    streamN.WriteByte((byte)(data.Y));
                    streamN.WriteByte((byte)(data.Z));
                    streamN.WriteByte(0xFF);
                }
            }
            surf.Unmap();


            var desc = new Texture2DDescription();
            desc.Width = info.width;
            desc.Height = info.height;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.Usage = ResourceUsage.Default;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.SampleDescription = new SampleDescription(1, 0); 


            var newText = new Texture2D(Display.device,desc);
            Display.context.CopyResource(MappableTexture, newText);
            
            MappableTexture.Dispose();

            normalView = new ShaderResourceView(Display.device, newText);

        }


        public void Render() {

            int LOD_to_show=0;
            if (SceneManager.Camera.eyePosition.Y>500) LOD_to_show=2;
            else if (SceneManager.Camera.eyePosition.Y>250) LOD_to_show=1;
            else LOD_to_show=0;

            Display.context.PixelShader.SetConstantBuffer(1, objectBuffer);

            ShaderManager.Apply("VS_Terrain");
            ShaderManager.Apply("PS_Terrain");
            Display.context.PixelShader.SetSampler(0, DeviceStates.samplerClamp);
            Display.context.PixelShader.SetSampler(1, DeviceStates.samplerWrap);

            Display.context.PixelShader.SetShaderResource(0, info.TextureMap.ressource);
            Display.context.PixelShader.SetShaderResource(1, normalView);
            Display.context.PixelShader.SetShaderResource(2, info.ColorMap.ressource);
            Display.context.PixelShader.SetShaderResource(3, info.LightMap.ressource);
            Display.context.PixelShader.SetShaderResource(4, info.TextureArray.ressource);
            //Display.context.PixelShader.SetShaderResource(5, info.Clouds.ressource);

            GraphicsManager.SetBlendState(DeviceStates.blendStateSolid);
            GraphicsManager.SetDepthState(DeviceStates.depthDefaultState);
            GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            Display.context.InputAssembler.SetIndexBuffer(indexBuffer[LOD_to_show], Format.R32_UInt, 0);
            Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer[LOD_to_show], VertexTexture.SizeInBytes,0));

            Display.context.DrawIndexed(IndicesLength[LOD_to_show], 0, 0);
        }

    }


}
