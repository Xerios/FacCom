using RageEngine.ContentPipeline;
using RageEngine.Graphics;
using RageEngine.Graphics.TwoD;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rectangle = System.Drawing.Rectangle;

namespace RageRTS {
    public class InteractiveObject2DManager: Renderable {
        public bool StrategicMode = false;

        private List<InteractiveObject2D> list = new List<InteractiveObject2D>(1000);
        private VertexBufferBatch<VertexTextureColor> batch;

        public int GetRenderPass() { return (int)RenderPass.Transparent; }

        public InteractiveObject2DManager() {
            batch = new VertexBufferBatch<VertexTextureColor>(600);
        }

        public InteractiveObject2D Add(InteractiveObject iobj) {
            InteractiveObject2D obj = new InteractiveObject2D(iobj);
            list.Add(obj);
            return obj;
        }

        public void Remove(InteractiveObject2D obj) {
            list.Remove(obj);
        }

        public InteractiveObject2D Select(Vector2 point) {
            InteractiveObject2D returnObj = null;
            float nearestDist = float.MaxValue;
            foreach (InteractiveObject2D obj in list) {
                if (obj.rect.Contains((int)point.X, (int)point.Y)) {
                    float dist = (obj.position - point).LengthSquared();
                    if (dist<nearestDist) {
                        nearestDist = dist;
                        returnObj = obj;
                    }
                }
            }
            return returnObj;
        }
        public void Select(Vector2 point, ref List<InteractiveObject2D> result) {
            foreach (InteractiveObject2D obj in list) {
                if (obj.rect.Contains((int)point.X, (int)point.Y)) { result.Add(obj); return; }
            }
        }
        public void Select(ref Rectangle rect, ref List<InteractiveObject2D> result) {
            foreach (InteractiveObject2D obj in list) {
                if (rect.Contains(obj.rect) || rect.IntersectsWith(obj.rect)) result.Add(obj);
            }
        }
        public void Select(InteractiveObject2D selectedObj, ref List<InteractiveObject2D> result) {
            if (selectedObj == null) return;
            foreach (InteractiveObject2D obj in list) {
                if (!obj.onScreen) continue;
                if (selectedObj.entity.GetType() == obj.entity.GetType()) result.Add(obj);
            }
        }

        public void Render() {
            if (SceneManager.Pass != RenderPass.Transparent || StrategicMode) return;

            float height = 0.5f, bracketSizeDefault = 1, radiusSize = 0;
            float bracketSize;
            Vector2 texD1 = new Vector2(0.0f, 0.0f), texD2 = new Vector2(0.0f, 1.5f), texD3 = new Vector2(0.75f, 0.0f);
            Vector2 tex1, tex2, tex3, tex4;
            tex4 = new Vector2(1);
            Color4 color = Color.White;

            foreach (InteractiveObject2D obj in list) {
                if (obj.onScreen && (obj.focused || obj.selected)) {
                    radiusSize = obj.bracketSize- 1;
                    bracketSize = bracketSizeDefault  + radiusSize;
                    tex1 = texD1; tex2 = texD2; tex3 = texD3;
                    if (!obj.selected) {
                        tex1.X+=0.5f;
                        tex2.X+=0.5f;
                        tex3.X+=0.5f;
                    }
                    /*if (obj.side==Main.network.me.id) {
                        color = new Color4(0.75f, 1, 0,1);
                    } else {
                        color = Color.OrangeRed;
                    }*/

                    Vector3 pos = obj.entity.IntObj3D.Position;

                    batch.Add(new VertexTextureColor(pos + new Vector3(bracketSize, height, bracketSize), tex1, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(radiusSize, height, bracketSize), tex3, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(bracketSize, height, radiusSize), tex2, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-bracketSize, height, -bracketSize), tex1, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-radiusSize, height, -bracketSize), tex3, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-bracketSize, height, -radiusSize), tex2, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-bracketSize, height, bracketSize), tex1, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-bracketSize, height, radiusSize), tex2, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(-radiusSize, height, bracketSize), tex3, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(bracketSize, height, -bracketSize), tex1, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(bracketSize, height, -radiusSize), tex2, color));
                    batch.Add(new VertexTextureColor(pos + new Vector3(radiusSize, height, -bracketSize), tex3, color));
                }
            }

            if (batch.Count>0) {
                GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
                GraphicsManager.SetDepthState(DeviceStates.depthWriteOff);
                GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

                ShaderManager.Apply("VS_UI3D");
                ShaderManager.Apply("PS_UI3D");

                Display.context.PixelShader.SetShaderResource(0, ForegroundGame.selectionBrackets.ressource);

                batch.Draw();
            }
        }

        public void Render2D() {
            Vector2 pos;
            float length=0;
            Vector2 pos2D;

            for (int i = 0; i < list.Count; i++) {
                InteractiveObject2D obj=list[i];

                CameraHelper.Convert3DPointTo2D(obj.entity.IntObj3D.Position, out pos);
                obj.position.X = (int)pos.X;
                obj.position.Y = (int)pos.Y;
                if (!StrategicMode) {
                    length = Vector3.Distance(SceneManager.Camera.eyePosition, obj.entity.IntObj3D.Position);
                    obj.rect.Width = obj.rect.Height = (int)(16 * (obj.bracketSize * 120 / length));
                    obj.rect.X = (int)(pos.X - (float)obj.rect.Height / 2);
                    obj.rect.Y = (int)(pos.Y - (float)obj.rect.Height / 2);
                } else {
                    obj.rect.Width = obj.rect.Height =16;
                    obj.rect.X = (int)pos.X - 7;
                    obj.rect.Y = (int)pos.Y - 7;
                }
                obj.onScreen = obj.rect.Right > 0 && obj.rect.Bottom > 0 && obj.rect.Left < Display.Width && obj.rect.Top < Display.Height;
                if (!obj.onScreen) continue;

                //DEBUG SELECTION RECTANGLE
                //QuadRenderer.Draw(Resources.GetEmptyTexture(), new SharpDX.Rectangle(obj.rect.X, obj.rect.Y, obj.rect.Width, obj.rect.Height), new Color4(0, 1, 1, 0.2f));


                if (!StrategicMode) {
                    // DRAW PROGRESS BARS

                    /*if (obj.progress != -1) {
                        //if (obj.side != Main.network.me.id) continue;
                        float size = obj.bracketSize * 1000 / length;
                        Vector2 posBar = new Vector2(pos.X - size, pos.Y + 5 + size);
                        Vector2 sizeBar = new Vector2(size * 2, 2);
                        Vector2 progressBar = new Vector2(size * 2 * ((float)obj.progress / 10000), 2)*0.25f;
                        QuadRenderer.Draw(Resources.GetEmptyTexture(), posBar - new Vector2(1, 1), (sizeBar + new Vector2(2, 2))*0.25f, Color.Black);

                        QuadRenderer.Draw(Resources.GetEmptyTexture(), posBar, progressBar, Color.Cyan);
                        posBar.Y += 2;
                        FontRenderer.Draw("default", obj.progress_text, posBar + new Vector2(1, 1), Color.Black);
                        FontRenderer.Draw("default", obj.progress_text, posBar, Color.White);
                    }*/
                    if (obj.selected || obj.focused) {
                        SharpDX.Rectangle rect = new SharpDX.Rectangle(obj.rect.X, obj.rect.Y + obj.rect.Height, obj.rect.Width, 3);
                        QuadRenderer.Draw(Resources.GetEmptyTexture(), rect, Color.Black);

                        rect.Right = (int)(rect.Right * ((float)obj.entity.Health / obj.entity.HealthMax));
                        QuadRenderer.Draw(Resources.GetEmptyTexture(), rect, Color.LightGreen);
                    }
                } else {
                    // DRAW STRATEGIC ICON OUTLINES
                    pos2D.X = pos.X - 7;
                    pos2D.Y = pos.Y - 7;

                    if (obj.iconId != -1 && !obj.selected && !obj.focused) {
                        QuadRenderer.Draw(ForegroundGame.startegicIcons, new SharpDX.Rectangle((int)pos2D.X, (int)pos2D.Y, 16, 16), new SharpDX.Rectangle(obj.iconId, 32, 16, 16), Color.Black);
                    }
                }
            }

            if (!StrategicMode) return;

            // STRATEGIC ICONS with over the outlines
            //------------------------------------------

            foreach (InteractiveObject2D obj in list) {
                if (!obj.onScreen || obj.iconId == -1) continue;
                pos2D.X = obj.position.X - 7;
                pos2D.Y = obj.position.Y - 7;

                Color4 color = Color.LimeGreen;//Main.network.players.GetColor(obj.side);
                QuadRenderer.Draw(ForegroundGame.startegicIcons, new SharpDX.Rectangle((int)pos2D.X, (int)pos2D.Y, 16, 16), new SharpDX.Rectangle(obj.iconId, 0, 16, 16), color);
            }

            // SELECTED UNITS HAVE TO BE RENDERED ON TOP
            //------------------------------------------

            foreach (InteractiveObject2D obj in list) {
                if (!obj.onScreen || obj.iconId == -1 ||  (!obj.selected && !obj.focused)) continue;
                pos2D.X = obj.position.X - 7;
                pos2D.Y = obj.position.Y - 7;
                Color4 color = Color.Black;
                if (obj.selected) {
                    color = Color.White;
                } else if (obj.focused) {
                    color =  Color.LimeGreen;//Main.network.players.GetColor(obj.side);
                }
                QuadRenderer.Draw(ForegroundGame.startegicIcons, new SharpDX.Rectangle((int)pos2D.X, (int)pos2D.Y, 16, 16), new SharpDX.Rectangle(obj.iconId, 32, 16, 16), color);
                
            }
            foreach (InteractiveObject2D obj in list) {
                if (!obj.onScreen || obj.iconId == -1 ||  (!obj.selected && !obj.focused)) continue;
                pos2D.X = obj.position.X - 7;
                pos2D.Y = obj.position.Y - 7;

                Color4 color =  Color.LimeGreen;//Main.network.players.GetColor(obj.side);
                QuadRenderer.Draw(ForegroundGame.startegicIcons, new SharpDX.Rectangle((int)pos2D.X, (int)pos2D.Y, 16, 16), new SharpDX.Rectangle(obj.iconId, 0, 16, 16), color);
            }
        }

    }
}
