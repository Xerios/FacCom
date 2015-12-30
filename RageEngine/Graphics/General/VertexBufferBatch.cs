using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.Graphics {
    public class VertexBufferBatch<T> where T: struct {        
        private Buffer vertexBuffer;
        private BatchCollection<T> vertices;

        public int Count { 
            get {
                return vertices.Count;
            }
        }

        public VertexBufferBatch(int capacity) {
            vertices = new BatchCollection<T>(capacity);
            vertexBuffer = new Buffer(Display.device, new BufferDescription(capacity * VertexTextureColor.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
        }

        public void Add(T item) {
            vertices.Add(item);
        }

        public void Draw() {
            Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexTextureColor.SizeInBytes, 0));
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
