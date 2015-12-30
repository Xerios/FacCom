using System.Collections.Generic;
using System.Text;
using System.Drawing;
using RageEngine.Utils;
using RageEngine.Graphics;
using SharpDX.Direct3D11;
using SharpDX;


namespace RageEngine.Rendering {
    public class StaticDecal {
        public DecalManager parent;

        private VertexTextureColor[] vertices;
        public BoundingSphere bounding;
        public Material material;

        public Buffer vertexBuffer;

        public StaticDecal(TerrainInfo map_info,Material mat,Vector3 pos,float size, int resolution,float rotation) {
            material = mat;
            map_info.GetHeight(pos, ref pos.Y);
            bounding = new BoundingSphere(pos, size);

            float x1 = -size * 0.5f;
            float z1 = -size * 0.5f;

            vertices = new VertexTextureColor[(resolution + 1) * (resolution + 1) * 3 * 2];

            int amount = 0;
            Vector2 texD1 = new Vector2(0, 0), texD2 = new Vector2(1, 0), texD3 = new Vector2(0, 1), texD4 = new Vector2(1, 1);
            Color4 color = material.color;
            for (float i = 0; i < resolution; i++) {
                for (float j = 0; j < resolution; j++) {
                    Vector3 position = new Vector3(x1, 0, z1);
                    Vector3 local_position;

                    local_position = rotatePoint(pos,position + new Vector3(0, 0, 0),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position, new Vector2(i/resolution, j/resolution),color);

                    local_position = rotatePoint(pos,position + new Vector3(size / resolution, 0, size / resolution),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position, new Vector2((i+1)/resolution, (j+1)/resolution),color);

                    local_position = rotatePoint(pos,position + new Vector3(0, 0, size / resolution),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position,  new Vector2(i/resolution, (j+1)/resolution),color);

                    local_position = rotatePoint(pos,position + new Vector3(0, 0, 0),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position,  new Vector2(i/resolution, j/resolution),color);

                    local_position = rotatePoint(pos,position + new Vector3(size / resolution, 0, 0),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position,  new Vector2((i+1)/resolution, j/resolution),color);

                    local_position = rotatePoint(pos,position + new Vector3(size / resolution, 0, size / resolution),rotation);
                    map_info.GetHeight(local_position, ref local_position.Y);
                    vertices[amount++] = new VertexTextureColor(local_position,  new Vector2((i+1)/resolution, (j+1)/resolution),color);
                    
                    z1 += size / resolution;
                }
                x1 += size / resolution;
                z1 = -size * 0.5f;
            }

            vertexBuffer = Buffer.Create<VertexTextureColor>(Display.device, vertices, new BufferDescription(vertices.Length*VertexTextureColor.SizeInBytes, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));
            
        }

        private Vector3 rotatePoint(Vector3 center,Vector3 offset,float rotation) {
            Vector3 finalPos = new Vector3();
            float rotCos = (float)System.Math.Cos(rotation);
            float rotSin = (float)System.Math.Sin(rotation);
            finalPos.X = rotCos * offset.X - rotSin * offset.Z;
            finalPos.Z = rotSin * offset.X + rotCos * offset.Z;
            finalPos.X += center.X;
            finalPos.Z += center.Z;
            return finalPos;
        }


        public void Draw(){
            if (SceneManager.Camera.Frustum.Contains(ref bounding) == ContainmentType.Disjoint) return;

            Display.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexTextureColor.SizeInBytes, 0));
            Display.context.Draw(vertices.Length, 0);
        }

        public void Dispose() {
            parent.Dispose(this);
        }

    }
        
}
