using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.Graphics;
using SharpDX.Direct3D11;
using SharpDX;

namespace RageEngine.Rendering {
    public class TerrainDebug : Renderable {

        Buffer vertexBuffer;
        BatchCollection<VertexColor> vertices;

        TerrainInfo map_info;

        public int GetRenderPass() { return (int)RenderPass.Solid; }

        public TerrainDebug(TerrainInfo map){

            vertices = new BatchCollection<VertexColor>(6*10000);

            vertexBuffer = new Buffer(Display.device, new BufferDescription(vertices.Capacity * VertexColor.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

            map_info=map;
        }

        public void Add(Vector2 pos,Color4 color){


            float size = 2;

            Vector3 position = new Vector3(pos.X, 0, pos.Y);


            Vector3 d1, d2, d3;
            d1 = fixPointWater(map_info,position);

            Vector3 v=new Vector3(pos.X + (size * 0.5f), position.Y, pos.Y + (size * 0.5f));
            v = fixPointWater(map_info, v);

            if (SceneManager.Camera.Frustum.Contains(v)==ContainmentType.Disjoint) return;

            d2 = fixPointWater(map_info, position + new Vector3(size, 0, size));            
            d3 = fixPointWater(map_info,position + new Vector3(0, 0, size));
            vertices.Add(new VertexColor(d1, color));            
            vertices.Add(new VertexColor(d2, color));            
            vertices.Add(new VertexColor(d3, color));

            d3 = fixPointWater(map_info,position + new Vector3(size, 0, 0));         
            vertices.Add(new VertexColor(d1, color));
            vertices.Add(new VertexColor(d3, color));            
            vertices.Add(new VertexColor(d2, color));

            
        }

        private static Vector3 fixPointWater(TerrainInfo map_info,Vector3 pos) {
            map_info.GetHeightInacurate(pos, ref pos.Y);
            if (pos.Y < map_info.waterLevel) pos.Y = map_info.waterLevel;
            pos.Y+=0.1f;
            return pos;
        }


        public void Render() {

            if (vertices.Count>0) {
                ShaderManager.Apply("VS_Debug");
                ShaderManager.Apply("PS_Debug");

                GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
                GraphicsManager.SetDepthState(DeviceStates.depthWriteOff);
                GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

                Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexColor.SizeInBytes, 0));

                while (vertices.Seek()) {
                    DataStream stream;
                    Display.context.MapSubresource(vertexBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                    stream.WriteRange(vertices.Get());
                    Display.context.UnmapSubresource(vertexBuffer, 0);
                    Display.context.Draw(vertices.GetItemsCount(), 0);
                }
                vertices.Clear();
            }

        }

    }

}
