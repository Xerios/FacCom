using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.Collections.Generic;
using RageEngine.ContentPipeline;

namespace RageEngine.Graphics.TwoD {


    public static class QuadRenderer {


        /*private struct QuadSprite {
            public Texture Texture;
            public Vector2 Position;
            public Vector2 Size;
            public SharpDX.Rectangle Source;
            public Color4 Color;
            public Vector2 Origin;
            public float Rotation;
        }*/

        static BatchCollection<Vertex2D> vertices;

        static VertexBufferBinding vertexBufferBind;
        static Buffer vertexBuffer,indexBuffer;
        static int vertexBufferSize;

        static int spritesDrawn=-1;

        public static void Initialize() {
            vertices = new BatchCollection<Vertex2D>(8192);
            vertexBufferSize = vertices.Capacity;
            vertexBuffer = new Buffer(Display.device, new BufferDescription(vertexBufferSize * Vertex2D.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
            vertexBufferBind = new VertexBufferBinding(vertexBuffer, Vertex2D.SizeInBytes, 0);


            
            short[] shArr = new short[12288];
            for (int i = 0; i < 2048; i++) {
                shArr[i * 6] = (short)(i * 4);
                shArr[(i * 6) + 1] = (short)((i * 4) + 1);
                shArr[(i * 6) + 2] = (short)((i * 4) + 2);
                shArr[(i * 6) + 3] = (short)((i * 4) + 2);
                shArr[(i * 6) + 4] = (short)((i * 4) + 1);
                shArr[(i * 6) + 5] = (short)((i * 4) + 3);
            }


            indexBuffer = new Buffer(Display.device, DataStream.Create(shArr,false,false), new BufferDescription(12288 * sizeof(short), ResourceUsage.Dynamic, BindFlags.IndexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

        }


        public static void Begin() {
#if DEBUG
            PixHelper.BeginEvent(Color.Orange, "QuadRenderer Batch");
#endif
            spritesDrawn=-1;
            vertices.Clear();

            Display.context.VertexShader.SetConstantBuffer(0, Display.screenBuffer);
            GraphicsManager.SetRastState(DeviceStates.rastStateDrawing);
            GraphicsManager.SetDepthState(DeviceStates.depthDisabled);
            GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
            GraphicsManager.SetPrimitiveTopology(PrimitiveTopology.TriangleList);

            ShaderManager.Apply("VS_2D");
            ShaderManager.Apply("PS_2D");

            Display.context.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R16_UInt, 0);
            Display.context.InputAssembler.SetVertexBuffers(0, vertexBufferBind);

            ResetCut();
            //Display.context.Rasterizer.SetScissorRectangles(new Rectangle(0, 0, Display.width, Display.height));
        }

        static bool resetCut = false;
        static Rectangle currentRectangle,lastRectangle;
        static Texture currentTexture,lastTexture;

        public static void ResetCut() {
            currentRectangle = new Rectangle(0, 0, Display.Width, Display.Height);
            Push();
            lastRectangle=currentRectangle;
        }
        public static void SetCut(Rectangle rect) {
            rect.Right += rect.Left;
            rect.Bottom += rect.Top;
            currentRectangle=rect;
            Push();
            lastRectangle=currentRectangle;
        }

        public static void Draw(Texture texture, Rectangle rect, Color4 color) {
            Draw(texture, new Vector2(rect.Left, rect.Top), new Vector2(1.0f / texture.Width * (rect.Right), 1.0f / texture.Height * (rect.Bottom)), null, color, Vector2.Zero);
        }

        public static void Draw(Texture texture, Rectangle rect, Rectangle cut, Color4 color) {
            Draw(texture, new Vector2(rect.Left, rect.Top), new Vector2(1.0f / cut.Width * (rect.Right), 1.0f / cut.Height * (rect.Bottom)), cut, color, Vector2.Zero);
        }

        public static void Draw(Texture texture, Vector2 position, Color4 color) {
            Draw(texture, position, new Vector2(1, 1), null, color, Vector2.Zero);
        }

        public static void Draw(Texture texture, Vector2 position, Rectangle cut, Color4 color) {
            Draw(texture, position, new Vector2(1, 1), cut, color, Vector2.Zero);
        }

        public static void Draw(Texture texture, Vector2 position, Vector2 size, Color4 color) {
            Draw(texture, position, size, null, color,Vector2.Zero);
        }

        public static void Draw(Texture texture, Vector2 position, Vector2 size, SharpDX.Rectangle? rect, Color4 color, Vector2 center, float rotation = 0) {

            if (lastTexture!=texture) {
                Push();
                lastTexture=texture;
            }

            Matrix trans = Matrix.Translation(-center.X,-center.Y,0) *
                           Matrix.Scaling(size.X,size.Y,0) *
                           Matrix.RotationZ(rotation) *
                           Matrix.Translation(position.X,position.Y,0);

            Vector2 sizeConverted = new Vector2();
            sizeConverted.X = texture.Width;
            sizeConverted.Y = texture.Height;

            Vector4 cut = new Vector4(0, 0, 1, 1);
            if (rect.HasValue) {
                cut.X = ((float)rect.Value.Left / texture.Width);
                cut.Y = ((float)rect.Value.Top / texture.Height);
                cut.Z = ((float)rect.Value.Right / texture.Width);
                cut.W = ((float)rect.Value.Bottom / texture.Height);

                sizeConverted.X = rect.Value.Width;
                sizeConverted.Y = rect.Value.Height;
            }

            vertices.Add(new Vertex2D(Vector2.Transform(new Vector2(0, 0), trans), new Vector2(cut.X, cut.Y), color));
            vertices.Add(new Vertex2D(Vector2.Transform(new Vector2(sizeConverted.X, 0), trans), new Vector2(cut.Z, cut.Y), color));
            vertices.Add(new Vertex2D(Vector2.Transform(new Vector2(0, sizeConverted.Y), trans), new Vector2(cut.X, cut.W), color));
            vertices.Add(new Vertex2D(Vector2.Transform(new Vector2(sizeConverted.X, sizeConverted.Y), trans), new Vector2(cut.Z, cut.W), color));

            spritesDrawn++;
        }

        public static void DrawFullScreenGradient(Color4 from, Color4 to) {
            DrawFullScreenGradient(from, from, to, to);
        }

        public static void DrawFullScreenGradient(Color4 one, Color4 two, Color4 three, Color4 four) {

            Vector2 pos = new Vector2(0, 0), size = new Vector2(Display.Width,Display.Height);

            Texture texture = Resources.GetEmptyTexture();

            if (lastTexture!=texture) {
                Push();
                lastTexture=texture;
            }

            vertices.Add(new Vertex2D(new Vector2(pos.X, pos.Y), new Vector2(0, 0), one));
            vertices.Add(new Vertex2D(new Vector2(pos.X + size.X, pos.Y), new Vector2(1, 0), two));
            vertices.Add(new Vertex2D(new Vector2(pos.X, pos.Y + size.Y), new Vector2(0, 1), three));
            vertices.Add(new Vertex2D(new Vector2(pos.X + size.X, pos.Y + size.Y), new Vector2(1, 1), four));
            spritesDrawn++;
        }



        static int verticesLastCount = 0, verticesLastPosition = 0;

        internal static void Push() {

            if (spritesDrawn==0) return;
            else if (spritesDrawn==-1) {
                spritesDrawn=0;
                return;
            }

            Display.context.PixelShader.SetShaderResource(0, lastTexture.ressource);
            //Display.context.Rasterizer.SetScissorRectangles(lastRectangle);

            while (vertices.Seek()) {
                int newVar = verticesLastPosition + vertices.GetItemsCount();
                if (newVar >= vertexBufferSize) {
                    DataStream stream;
                    Display.context.MapSubresource(vertexBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                    stream.WriteRange(vertices.Get());
                    Display.context.UnmapSubresource(vertexBuffer, 0);
                    Display.context.DrawIndexed(spritesDrawn*6, 0, 0);

                    verticesLastCount = spritesDrawn;
                    verticesLastPosition = vertices.GetItemsCount();
                } else {
                    DataStream stream;
                    Display.context.MapSubresource(vertexBuffer, MapMode.WriteNoOverwrite, MapFlags.None, out stream);
                    stream.Position = verticesLastPosition * Vertex2D.SizeInBytes;
                    stream.WriteRange(vertices.Get(),0,vertices.GetItemsCount());
                    Display.context.UnmapSubresource(vertexBuffer, 0);
                    Display.context.DrawIndexed(spritesDrawn*6, 0, verticesLastPosition);
                    verticesLastCount += spritesDrawn;
                    verticesLastPosition += vertices.GetItemsCount();
                }
            }
            vertices.Clear();
            spritesDrawn=0;

        }

        public static void End() {
            Push();
#if DEBUG
            PixHelper.EndEvent();
#endif
        }


        internal static Vector2 rotatePoint(Vector2 center, Texture texture, Vector2 offset, float rotation) {
            Vector2 finalPos = new Vector2();

            float rotCos = (float)System.Math.Cos(rotation);
            float rotSin = (float)System.Math.Sin(rotation);
            finalPos.X = rotCos * offset.X - rotSin * offset.Y;
            finalPos.Y = rotSin * offset.X + rotCos * offset.Y;

            finalPos.X = 2.0f * (finalPos.X * texture.Width) / Display.Width;
            finalPos.Y = 2.0f * (finalPos.Y * texture.Height) / Display.Height;

            finalPos.X = center.X + finalPos.X;
            finalPos.Y = center.Y - finalPos.Y;

            return finalPos;
        }
    }

}
