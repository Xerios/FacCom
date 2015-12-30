/*
 * Created by Javier Cantón Ferrero.
 * MVP Windows-DirectX 2007/2008
 * DotnetClub Sevilla
 * Date 20/03/2007
 * Web www.codeplex.com/XNACommunity
 * Email javiuniversidad@gmail.com
 * blog: mirageproject.blogspot.com
 */
using System.Collections.Generic;
using System.Text;
using Math = System.Math;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using RageEngine.Graphics;

namespace RageEngine.Rendering
{

    public class DotManager: Renderable
    {
        public int GetRenderPass() { return (int)RenderPass.Transparent; }

        Buffer vertexBuffer;
        BatchCollection<VertexColor> vertices;

        public DotManager()
        {
            vertices = new BatchCollection<VertexColor>(10000);

            vertexBuffer = new Buffer(Display.device, new BufferDescription(vertices.Capacity * VertexColor.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
        }

        public void AddDot(float x,float y,float z, Color4 color) {
            AddDot(new Vector3(x,y,z),color);
        }

        public void AddDot(Vector3 point,Color4 color){
            vertices.Add(new VertexColor(point, color));
        }


        public void Render()
        {

            if (vertices.Count>0){

                GraphicsManager.SetDepthState(DeviceStates.depthWriteOff);
                GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
                GraphicsManager.SetPrimitiveTopology(PrimitiveTopology.PointList);

                ShaderManager.Apply("VS_Debug");
                ShaderManager.Apply("PS_Debug");

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
