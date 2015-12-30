using RageEngine;
using RageEngine.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rectangle = System.Drawing.Rectangle;

namespace RageRTS {
    public class InteractiveObject3D: TimeEntity {
        public InteractiveObject entity;


        private static VertexBufferBatch<VertexTextureColor> batch = new VertexBufferBatch<VertexTextureColor>(600);

        public InteractiveObject3D(InteractiveObject iobj) {
            entity = iobj;
        }


        public virtual void Render() {
            if (!entity.Initialized) return;

            for (int i = 0; i < entity.Behaviors.Count; i++) entity.Behaviors[i].RenderDebug();

            //SceneManager.LineManager.AddGroundCircle(Position, IntObj2D.bracketSize, Color.Green);
            //SceneManager.LineManager.AddLine(Position, Position, Color.Green);
            //SceneManager.LineManager.AddLine(Position, Position+normal, Color.Green);

            Color color = Color.White;
            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(-1, 0, 0)), new Vector2(1, 1), color));
            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(1, 0, 0)), new Vector2(0, 1), color));
            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(-1, 3, 0)), new Vector2(1, 0), color));

            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(-1, 3, 0)), new Vector2(1, 0), color));
            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(1, 3, 0)), new Vector2(0, 0), color));
            batch.Add(new VertexTextureColor(RotateToScreen(Position, new Vector3(1, 0, 0)), new Vector2(0, 1), color));
        }

        private static Vector3 RotateToScreen(Vector3 pos, Vector3 offset) {

            Matrix mat = Matrix.Billboard(Vector3.Zero, SceneManager.Camera.eyePosition.ToX0Z()-pos.ToX0Z(), new Vector3(0, 1, 0), SceneManager.Camera.eyeDirection);

            return pos +  Vector3.TransformCoordinate(offset, mat);
        }

        public static void RenderAll() {
            if (batch.Count>0) {
                GraphicsManager.SetBlendState(DeviceStates.blendStateSolid);
                GraphicsManager.SetDepthState(DeviceStates.depthDefaultState);
                GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

                ShaderManager.Apply("VS_Model");
                ShaderManager.Apply("PS_Model");

                Display.context.PixelShader.SetShaderResource(0, ForegroundGame.unitPlaceholder.ressource);

                batch.Draw();
            }
        }
    }
}
