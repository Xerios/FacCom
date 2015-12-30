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
using RageEngine.Utils;
using RageEngine.Graphics.TwoD;
using RageEngine.Graphics;

namespace RageEngine.Rendering
{

    public class LineManager: Renderable
    {
        public int GetRenderPass() { return (int)RenderPass.Transparent; }

        Buffer vertexBuffer;
        BatchCollection<VertexColor> vertices;

        public LineManager()
        {
            vertices = new BatchCollection<VertexColor>(10000);
            vertexBuffer = new Buffer(Display.device, new BufferDescription(vertices.Capacity * VertexColor.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
        }

        public void AddLine(Vector3 startPoint, Color4 startColor,Vector3 endPoint, Color4 endColor) 
        {
            if (startColor.Alpha == 0 && endColor.Alpha == 0) return;
            vertices.Add(new VertexColor(startPoint, startColor));
            vertices.Add(new VertexColor(endPoint, endColor));
        }

        public void AddLine(Vector3 startPoint, Vector3 endPoint, Color4 color)
        {
            AddLine(startPoint, color, endPoint, color);
        }

        //-------------------------------------------------------------------------------------

        public void Render() {

            if (vertices.Count>0) {

                GraphicsManager.SetDepthState(DeviceStates.depthWriteOff);
                GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
                GraphicsManager.SetPrimitiveTopology(PrimitiveTopology.LineList);

                ShaderManager.Apply("VS_Debug");
                ShaderManager.Apply("PS_Debug");

                Display.context.InputAssembler.SetVertexBuffers(0,  new VertexBufferBinding(vertexBuffer, VertexColor.SizeInBytes, 0));

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

        //------------------------------------------------------------------------------------- CUSTOM SHAPES

        public void AddCrossFlat(Vector3 origin, Vector3 direction, Color4 color) {
            // Build a vector pointing in an arbitrary direction that is perpendicular to
            // the direction the arrow is pointing at. This can be done by simply juggling
            // the vector elements around by one place.
            Vector3 normalizedDirection=Vector3.Normalize(direction);
            Vector3 up;
            GameUtils.GetPerpendicularVector(ref normalizedDirection, out up);
            Vector3 right=Vector3.Cross(normalizedDirection, up);

            // Right blade on arrowhead
            AddLine(origin-up*0.3f, origin+up*0.3f, color);
            // Left blade on arrowhead
            AddLine(origin-right*0.3f, origin+right*0.3f, color);
        }
        public void AddArrowFlat(Vector3 origin, Vector3 direction, Color4 color) {
            // Build a vector pointing in an arbitrary direction that is perpendicular to
            // the direction the arrow is pointing at. This can be done by simply juggling
            // the vector elements around by one place.
            Vector3 normalizedDirection=Vector3.Normalize(direction);
            Vector3 up;
            GameUtils.GetPerpendicularVector(ref normalizedDirection, out up);
            Vector3 right=Vector3.Cross(normalizedDirection, up);

            float length=direction.Length();
            up*=length;
            right*=length;

            Vector3 twoThird=origin+(direction*0.667f);
            Vector3 end=origin+direction;
            // Line origin to arrowhead
            AddLine(origin, end, color);
            // Right blade on arrowhead
            AddLine(end, twoThird+up*0.3f, color);
            // Left blade on arrowhead
            AddLine(end, twoThird+up*-0.3f, color);
        }
        public void AddArrow(Vector3 origin, Vector3 direction, Color4 color, bool small=false) {
            // Build a vector pointing in an arbitrary direction that is perpendicular to
            // the direction the arrow is pointing at. This can be done by simply juggling
            // the vector elements around by one place.
            Vector3 normalizedDirection=Vector3.Normalize(direction);
            Vector3 up;
            GameUtils.GetPerpendicularVector(ref normalizedDirection, out up);
            Vector3 right=Vector3.Cross(normalizedDirection, up);

            float length=direction.Length()*(small?0.15f:1);
            up*=length;
            right*=length;

            Vector3 twoThird=origin+(direction*0.667f)*(small?1.4f:1);
            Vector3 end=origin+direction;
            // Line origin to arrowhead
            AddLine(origin, end, color);
            // Upper blade on arrowhead
            AddLine(end, twoThird+up*0.3f, color);
            // Right blade on arrowhead
            AddLine(end, twoThird+right*0.3f, color);
            // Lower blade on arrowhead
            AddLine(end, twoThird+up*-0.3f, color);
            // Left blade on arrowhead
            AddLine(end, twoThird+right*-0.3f, color);
        }
        //------------------------------------------------------------------------------------- CUSTOM SHAPES

        public void AddBoundingBox(Vector3 min, Vector3 max, Color4 color) {
            AddLine(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, min.Y, min.Z), color);
            AddLine(new Vector3(max.X, min.Y, max.Z), new Vector3(min.X, min.Y, max.Z), color);
            AddLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, min.Y, min.Z), color);
            AddLine(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color);
            AddLine(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            AddLine(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            AddLine(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            AddLine(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);
            AddLine(new Vector3(max.X, max.Y, max.Z), new Vector3(max.X, max.Y, min.Z), color);
            AddLine(new Vector3(max.X, max.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            AddLine(new Vector3(min.X, max.Y, max.Z), new Vector3(min.X, max.Y, min.Z), color);
            AddLine(new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
        }
        public void AddBone(Matrix onePos, Matrix twoPos, Matrix realWorldPos, Color4 color) {
            Vector3 pos1 = new Vector3(onePos.M41, onePos.M42, onePos.M43);
            Vector3 pos2 = new Vector3(twoPos.M41, twoPos.M42, twoPos.M43);

            pos1 = Vector3.TransformCoordinate(pos1, realWorldPos);
            pos2 = Vector3.TransformCoordinate(pos2, realWorldPos);

            float size = 0.2f;
            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, -size), onePos), Vector3.TransformCoordinate(new Vector3(size, size, -size), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, -size), onePos), Vector3.TransformCoordinate(new Vector3(size, size, size), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, size), onePos), Vector3.TransformCoordinate(new Vector3(size, -size, size), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, size), onePos), Vector3.TransformCoordinate(new Vector3(size, -size, -size), onePos), color);

            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, -size), onePos), Vector3.TransformCoordinate(new Vector3(0, 0, 0), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, -size), onePos), Vector3.TransformCoordinate(new Vector3(0, 0, 0), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, size), onePos), Vector3.TransformCoordinate(new Vector3(0, 0, 0), onePos), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, size), onePos), Vector3.TransformCoordinate(new Vector3(0, 0, 0), onePos), color);

            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, -size), onePos), pos2, color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, -size), onePos), pos2, color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, size, size), onePos), pos2, color);
            AddLine(Vector3.TransformCoordinate(new Vector3(size, -size, size), onePos), pos2, color);
        }        
        
        //------------------------------------------------------------------------------------- CUSTOM SHAPES        
        
        public void AddCube(Vector3 pos, float size, Color4 color) {
            AddLine(pos + new Vector3(-size, 0, -size), pos + new Vector3(size, 0, -size), color);
            AddLine(pos + new Vector3(size, 0, -size), pos + new Vector3(size, 0, size), color);
            AddLine(pos + new Vector3(size, 0, size), pos + new Vector3(-size, 0, size), color);
            AddLine(pos + new Vector3(-size, 0, size), pos + new Vector3(-size, 0, -size), color);
        }

        public void AddCube(Matrix matrix, Vector3 min, Vector3 max, Color4 color) {
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, min.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, min.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, min.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, min.Y, max.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, min.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, min.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, min.Y, min.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, min.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, min.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, max.Y, max.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, min.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, max.Y, max.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, min.Y, min.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, max.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, min.Y, min.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, max.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, max.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, max.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(max.X, max.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, max.Y, max.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, max.Y, max.Z), matrix), Vector3.TransformCoordinate(new Vector3(min.X, max.Y, min.Z), matrix), color);
            AddLine(Vector3.TransformCoordinate(new Vector3(min.X, max.Y, min.Z), matrix), Vector3.TransformCoordinate(new Vector3(max.X, max.Y, min.Z), matrix), color);
        }

        //------------------------------------------------------------------------------------- CUSTOM SHAPES

        public void AddGroundCircle(Vector3 position, float size, Color4 color) {
            float divide = 32;
            if (size==1) divide = 16;
            float step = GameUtils.TwoPi / divide;

            for (float a = 0f; a < GameUtils.TwoPi; a += step) {
                AddLine(position + new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)) * size,
                        position + new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step)) * size, color);
            }
        }

        public void AddSphere(Vector3 position, float size, Color4 color) {
            // Compute our step around each circle
            float step = GameUtils.TwoPi / 32;

            // Create the loop on the XY plane first
            for (float a = 0f; a < GameUtils.TwoPi; a += step) {
                AddLine(position + new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f) * size,
                        position + new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f) * size, color);
            }

            // Next on the XZ plane
            for (float a = 0f; a < GameUtils.TwoPi; a += step) {
                AddLine(position + new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)) * size,
                        position + new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step)) * size, color);
            }

            // Finally on the YZ plane
            for (float a = 0f; a < GameUtils.TwoPi; a += step) {
                AddLine(position + new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a)) * size,
                        position + new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step)) * size, color);
            }
        }

        //------------------------------------------------------------------------------------- CUSTOM SHAPES

        public void AddSunCompass(Vector3 position, Vector3 dir, float size) {
            // Compute our step around each circle
            float divide = 32;
            if (size==1) divide = 16;
            float step = GameUtils.TwoPi / divide;

            Color4 color = Color.Black;
            // Next on the XZ plane
            for (float a = 0f; a < GameUtils.TwoPi; a += step) {
                AddLine(position + new Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)) * size,
                        position + new Vector3((float)Math.Cos(a + step), 0f, (float)Math.Sin(a + step)) * size, color);
            }

            SceneManager.LineManager.AddLine(position - new Vector3(0, 0, -size), position - new Vector3(0, 0, size), color);
            SceneManager.LineManager.AddLine(position - new Vector3(-size, 0, 0), position - new Vector3(size, 0, 0), color);

            Vector3 lineShadow = dir;
            lineShadow.Y=0;
            SceneManager.LineManager.AddLine(position, position - lineShadow * 10, color);


            color = Color.LightGreen;

            SceneManager.LineManager.AddLine(position, position - dir * 10, Color.LightGreen);

            color = Color.DarkRed;

            // Create the loop on the XY plane first
            for (float a = 0f; a < GameUtils.Pi; a += step) {
                AddLine(position + new Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f) * size,
                        position + new Vector3((float)Math.Cos(a + step), (float)Math.Sin(a + step), 0f) * size, color);
            }


            // Finally on the YZ plane
            for (float a = -GameUtils.PiOver2; a < (GameUtils.PiOver2-step); a += step) {
                AddLine(position + new Vector3(0f, (float)Math.Cos(a), (float)Math.Sin(a)) * size,
                        position + new Vector3(0f, (float)Math.Cos(a + step), (float)Math.Sin(a + step)) * size, color);
            }
        }

    }


}
