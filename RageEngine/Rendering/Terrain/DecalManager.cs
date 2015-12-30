using System.Collections.Generic;
using System.Text;
using RageEngine.Utils;
using RageEngine.Graphics;
using SharpDX.Direct3D11;
using SharpDX;

namespace RageEngine.Rendering {
    public class DecalManager : Renderable {

        public TerrainInfo map;
        private Dictionary<Material, List<StaticDecal>> staticDecalList = new Dictionary<Material,List<StaticDecal>>(100);

        public int GetRenderPass() { return (int)RenderPass.Solid; }

        public DecalManager(TerrainInfo map) {
            this.map = map;
        }

        public StaticDecal New(Material material, Vector3 pos, float size) {
            int resolution = 1;
            if (size >= 3) resolution = (int)System.Math.Ceiling(size/3);
            return New(material, pos, size, resolution, GameUtils.RandomRotation());
        }

        public StaticDecal New(Material material,Vector3 pos,float size, int resolution) {
            return New(material, pos, size, resolution, GameUtils.RandomRotation());
        }

        public StaticDecal New(Material material,Vector3 pos,float size, int resolution,float rotation) {
            List<StaticDecal> list;
            if (!staticDecalList.TryGetValue(material, out list)) {
                list = new List<StaticDecal>(20);
                staticDecalList.Add(material, list);
            }
            StaticDecal decal = new StaticDecal(map,material, pos, size, resolution, rotation);
            decal.parent = this;
            list.Add(decal);
            return decal;
        }

        public void Dispose(StaticDecal decal) {
            List<StaticDecal> list;
            if (staticDecalList.TryGetValue(decal.material, out list)) {
                list.Remove(decal);
            }
        }


        public void Render() {
            GraphicsManager.SetDepthState(DeviceStates.depthWriteOff);
            GraphicsManager.SetBlendState(DeviceStates.blendStateTrans);
            GraphicsManager.SetPrimitiveTopology(SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            foreach (KeyValuePair<Material, List<StaticDecal>> keyPair in staticDecalList) {
                PrepareRendering(keyPair.Key);
                foreach (StaticDecal decal in keyPair.Value) {
                    decal.Draw();
                }
            }
        }

        void PrepareRendering(Material material) {
            Material.GetRenderState(material.mode);

            /*effect.Technique = material.technique;
            effect.SetTexture("Texture",material.texture.Texture);*/
            //DecalManager.effect.GetVariableByName("Texture").AsResource().SetResource(material.texture.ressource);
            //Display.context.PixelShader.SetShaderResource(material.textures[0].ressource, 0);
            //if (keyPair.Key.texture2!=null) StaticDecal.effect.Parameters["Texture2"].SetValue(keyPair.Key.texture2);
              
        }
    }

}
