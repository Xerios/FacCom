using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine;
using RageEngine.Utils;
using RageEngine.Graphics;
using System.Runtime.InteropServices;
using SharpDX;
using RageEngine.Rendering;
using SharpDX.Direct3D11;
using SharpDX.DXGI;


namespace MapEditor {

    [StructLayout(LayoutKind.Sequential)]
    public struct TerrainTriangle {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;

        public TerrainTriangle(Vector3 position, Vector3 normal, Vector2 texCoord) {
            this.Position = position;
            this.Normal = normal;
            this.TexCoord = texCoord;
        }

        public static InputElement[] Elements =
        {
            new InputElement("POSITION" , 0, Format.R32G32B32_Float  , InputElement.AppendAligned, 0), 
            new InputElement("NORMAL"   , 0, Format.R32G32B32_Float  , InputElement.AppendAligned, 0), 
            new InputElement("TEXCOORD" , 0, Format.R32G32_Float     , InputElement.AppendAligned, 0),
        };
        public static int SizeInBytes = sizeof(float) * (3 + 3 + 2);
    }

    public class EditorTerrain : Renderable {
        public TerrainInfo info;
        public TerrainTriangle[] vertices;
        //VertexBuffer myVertexBuffer;

        /*uint[] indices;
        IndexBuffer myIndexBuffer;*/

        int[] indices;
        public int IndicesLength;
        public Buffer indexBuffer,vertexBuffer;
        public InputLayout layout;



        Buffer objectBuffer;
        //public DecalManager decals;
        //public List<ModelPointer> models = new List<ModelPointer>();

        public int GetRenderPass() { return (int)RenderPass.Solid; }


        public EditorTerrain(TerrainInfo heightfield){

            info = heightfield;

            RebuildTerrain();
            //decals = new DecalManager(info);
        }

        public void RebuildTerrain() {

            //SmoothTerrain(1);

            /********* calc vertices *************/
            vertices = new TerrainTriangle[info.width * info.height];
            for (int x = 0; x < info.width; x++) {
                for (int y = 0; y < info.height; y++) {
                    vertices[x + y * info.width].Position = new Vector3(x, info.altitudeData[x, y], y);
                    vertices[x + y * info.width].TexCoord.X = (float)x / info.width;
                    vertices[x + y * info.width].TexCoord.Y = (float)y / info.height;
                }
            }

            /*********** calc indexes ********************/
            indices = new int[(info.width - 1) * (info.height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < info.height - 1; y++) {
                for (int x = 0; x < info.width - 1; x++) {
                    int topLeft = (int)(x + y * info.width);
                    int topRight = (int)((x + 1) + y * info.width);
                    int lowerLeft = (int)(x + (y + 1) * info.width);
                    int lowerRight = (int)((x + 1) + (y + 1) * info.width);

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;
                }
            }

            Vector3 normal;
            for (int i = 0; i < vertices.Length - info.width; i++) {
                Vector3 firstvec = vertices[i + 1].Position - vertices[i].Position;
                Vector3 secondvec = vertices[i].Position - vertices[i + info.width].Position;

                normal = Vector3.Cross(firstvec, secondvec);
                normal.Normalize();

                vertices[i].Normal += normal;
                vertices[i + 1].Normal += normal;
                vertices[i + info.width].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal.Normalize();

                int potntialY = (int)System.Math.Floor((float)i / info.width);
                int potntialX = i - potntialY * info.width;
                info.normalData[potntialX, potntialY] = vertices[i].Normal;
            }

            /*
            // Faster but less accurate method
            for (int x = 1; x < info.width - 1; x++) {
                for (int y = 1; y < info.height - 1; y++) {
                    Vector3 normX = new Vector3((vertices[x - 1 + y * info.width].Position.Y - vertices[x + 1 + y * info.width].Position.Y) / 2, 0, 1);
                    Vector3 normY = new Vector3(0, 1, (vertices[x + (y - 1) * info.width].Position.Y - vertices[x + (y + 1) * info.width].Position.Y) / 2);
                    vertices[x + y * info.width].Normal = normX + normY;
                    vertices[x + y * info.width].Normal.Normalize();
                }
            }*/
            
            /*********** copy to buffer ******/

            IndicesLength=indices.Length;
            indexBuffer= new Buffer(Display.device, DataStream.Create(indices, false, false), new BufferDescription(indices.Length*4, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));

            vertexBuffer = Buffer.Create<TerrainTriangle>(Display.device, vertices, new BufferDescription(vertices.Length*TerrainTriangle.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

            GenerateCB();
        }

        public void GenerateCB() {
            if (objectBuffer!=null) objectBuffer.Dispose();

            var buff = new TerrainConstant() {
                WaterColor = info.waterColor,
                WaterHeight = info.waterLevel
            };
            objectBuffer = Buffer.Create(Display.device, ref buff, new BufferDescription(16*2, ResourceUsage.Immutable, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct TerrainConstant { // SYNC THIS WITH MAP EDITOR & ENGINE
            public Vector4 WaterColor;
            public float WaterHeight;
            // 3 float Space lost !!!
        }

        public void Render() {
            Display.context.PixelShader.SetConstantBuffer(1, objectBuffer);

            ShaderManager.Apply("VS_Terrain");
            ShaderManager.Apply("PS_Terrain");
            Display.context.PixelShader.SetSampler(0, DeviceStates.samplerClamp);
            Display.context.PixelShader.SetSampler(1, DeviceStates.samplerWrap);

            Display.context.PixelShader.SetShaderResource(0, info.TextureMap.ressource);
            Display.context.PixelShader.SetShaderResource(1, info.ColorMap.ressource);
            Display.context.PixelShader.SetShaderResource(2, info.LightMap.ressource);
            Display.context.PixelShader.SetShaderResource(3, info.TextureArray.ressource);

            GraphicsManager.SetBlendState(DeviceStates.blendStateSolid);
            GraphicsManager.SetDepthState(DeviceStates.depthDefaultState);
            GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            Display.context.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, TerrainTriangle.SizeInBytes, 0));

            Display.context.DrawIndexed(IndicesLength, 0, 0);
        }

        public bool InsideMap(int x, int y) {
            if (x < 0 || y < 0 || x >= info.width || y >=  info.height) return false;
            return true;
        }


        private void SmoothTerrain(int Passes) {
            float[,] newHeightData;
            float[,] HeightData = info.altitudeData;

            while (Passes > 0) {
                Passes--;

                // Note: MapWidth and MapHeight should be equal and power-of-two values 
                newHeightData = new float[info.width, info.height];

                for (int x = 0; x < info.width; x++) {
                    for (int y = 0; y < info.height; y++) {
                        int adjacentSections = 0;
                        float sectionsTotal = 0.0f;

                        if ((x - 1) > 0) {
                            sectionsTotal += HeightData[x - 1, y];
                            adjacentSections++;

                            if ((y - 1) > 0) {
                                sectionsTotal += HeightData[x - 1, y - 1];
                                adjacentSections++;
                            }

                            if ((y + 1) < info.height) {
                                sectionsTotal += HeightData[x - 1, y + 1];
                                adjacentSections++;
                            }
                        }

                        if ((x + 1) < info.width) {
                            sectionsTotal += HeightData[x + 1, y];
                            adjacentSections++;

                            if ((y - 1) > 0) {
                                sectionsTotal += HeightData[x + 1, y - 1];
                                adjacentSections++;
                            }

                            if ((y + 1) < info.height) {
                                sectionsTotal += HeightData[x + 1, y + 1];
                                adjacentSections++;
                            }
                        }

                        if ((y - 1) > 0) {
                            sectionsTotal += HeightData[x, y - 1];
                            adjacentSections++;
                        }

                        if ((y + 1) < info.height) {
                            sectionsTotal += HeightData[x, y + 1];
                            adjacentSections++;
                        }

                        newHeightData[x, y] = (HeightData[x, y] + (sectionsTotal / adjacentSections)) * 0.5f;
                    }
                }

                // Overwrite the HeightData info with our new smoothed info
                for (int x = 0; x < info.width; x++) {
                    for (int y = 0; y < info.height; y++) {
                        HeightData[x, y] = newHeightData[x, y];
                    }
                }
            }
        }


        public void ChangeHeight(int x, int y, float power){
            vertices[x +  y * info.width].Position.Y = power;
            //vertices[x +  y * info.width].TexCoord = new Vector2(0, 0);
            info.altitudeData[x, y] = power;
            if (power > info.maxAltitude) info.maxAltitude = power;

            CalculateNormal(x, y);
        }

        public void SmoothHeight(List<float[]> allPoints){//, float power) {

            float[,] HeightData = info.altitudeData;

            int x, y;
            float power;

            foreach(float[] point in allPoints){

                x = (int)point[0];
                y = (int)point[1];
                power = point[2];

                int adjacentSections = 1;
                float sectionsTotal = HeightData[x, y];


                if ((x - 1) > 0) {
                    sectionsTotal += HeightData[x - 1, y];
                    adjacentSections++;

                    if ((y - 1) > 0) {
                        sectionsTotal += HeightData[x - 1, y - 1];
                        adjacentSections++;
                    }

                    if ((y + 1) < info.height) {
                        sectionsTotal += HeightData[x - 1, y + 1];
                        adjacentSections++;
                    }
                }

                if ((x + 1) < info.width) {
                    sectionsTotal += HeightData[x + 1, y];
                    adjacentSections++;

                    if ((y - 1) > 0) {
                        sectionsTotal += HeightData[x + 1, y - 1];
                        adjacentSections++;
                    }

                    if ((y + 1) < info.height) {
                        sectionsTotal += HeightData[x + 1, y + 1];
                        adjacentSections++;
                    }
                }

                if ((y - 1) > 0) {
                    sectionsTotal += HeightData[x, y - 1];
                    adjacentSections++;
                }

                if ((y + 1) < info.height) {
                    sectionsTotal += HeightData[x, y + 1];
                    adjacentSections++;
                }


                vertices[x + y * info.width].Position.Y += (-HeightData[x, y] + (HeightData[x, y] + (sectionsTotal / adjacentSections)) * 0.5f) * power;
            }


            foreach (float[] point in allPoints) {
                x = (int)point[0];
                y = (int)point[1];
                info.altitudeData[x, y] = vertices[x + y * info.width].Position.Y;
                CalculateNormal(x, y);
            }
        }

        public void CalculateNormal(int x, int y) {

            Vector3[] corner = new Vector3[4];
            corner[0] = vertices[x + y * info.width].Position;

            Vector3 normalA = Vector3.Zero;
            Vector3 normalB = Vector3.Zero;

            int bitID1 = (x + 1) + y * info.width;
            int bitID2 = (x + 1) + (y + 1) * info.width;
            int bitID3 = x + (y + 1) * info.width;

            if (bitID1 >= 0 && bitID1 < vertices.Length)
                corner[1] = vertices[bitID1].Position;
            if (bitID2 >= 0 && bitID2 < vertices.Length)
                corner[2] = vertices[bitID2].Position;
            if (bitID3 >= 0 && bitID3 < vertices.Length)
                corner[3] = vertices[bitID3].Position;

            if (corner[1] != Vector3.Zero && corner[2] != Vector3.Zero && corner[3] != Vector3.Zero) {
                if (corner[1] != Vector3.Zero && corner[3] != Vector3.Zero)
                    normalA = new Plane(corner[0], corner[3], corner[1]).Normal;

                if (corner[2] != Vector3.Zero && corner[2] != Vector3.Zero && corner[3] != Vector3.Zero)
                    normalB = new Plane(corner[1], corner[3], corner[2]).Normal;
            }

            vertices[x + y * info.width].Normal = normalA + normalB;
            vertices[x + y * info.width].Normal.Normalize();
            info.normalData[x, y] = vertices[x + y * info.width].Normal;
        }


    }
}
