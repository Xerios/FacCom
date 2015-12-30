using System;
using System.Collections.Generic;
using System.Text;
using RageEngine.RawFiles;
using SharpDX;
using RageEngine.ContentPipeline;
using System.Diagnostics.Contracts;

namespace RageEngine.Graphics {

    public enum MaterialMode { Solid = 1, Transparent = 2, Additif = 3 };

    public struct Material
    {
        public static ushort UNIQUE_ID;
        public ushort id;

        public static Material Current;
        public static Material Identity = new Material("Default",MaterialMode.Solid,null);


        public uint         guid;

        public string       technique;
        public MaterialMode mode;
        public Texture[]    textures;
        public Color4       color;
        public float        delta;

        public Material(RawMaterial mat) {
            guid = 0; // Defined by the model pointer
            if (mat.id==0) mat.id=++UNIQUE_ID;
            this.id = mat.id;
            this.mode = mat.mode;
            this.technique = mat.technique;

            this.textures = new Texture[mat.textures.Length];
            for (int i = 0; i < mat.textures.Length; i++) this.textures[i] = Resources.GetTexture(mat.textures[i]);

            this.delta = mat.delta;
            this.color = new Color4(mat.color);
        }

        public Material(string technique, MaterialMode mode1, Texture[] texture) {
            guid = 0; // Defined by the model pointer
            this.id = ++UNIQUE_ID;
            this.mode = mode1;
            this.technique = technique;
            this.textures = texture;
            this.delta = 0;
            this.color = new Color4(1,1,1,1);
        }

        public static SharpDX.Direct3D11.BlendState GetRenderState(MaterialMode mode) {
            if (mode == MaterialMode.Solid) {
                return DeviceStates.blendStateSolid;
            } else if (mode == MaterialMode.Additif) {
                return DeviceStates.blendStateAdd;
            } else if (mode == MaterialMode.Transparent) {
                return DeviceStates.blendStateTrans;
            }
            return DeviceStates.blendStateSolid;
        }


        //-------------------------------------------
        public static uint        lastRank;
        public static int         lastVertexDeclaration;
        public static string      lastPass;
        public static Texture lastTexture, lastTexture2, lastTexture3;

        public static void Reset() {
            lastVertexDeclaration = 0;
            lastPass= "";
            lastTexture = null;
            lastTexture2 = null;
            lastTexture3 = null;
            lastRank = 0;
        }

        public static void PrepareRendering(uint Rank, bool instanced, Material material) {
            GraphicsManager.SetBlendState(Material.GetRenderState(material.mode));

            /*if (material.technique=="Foliage") {
                GraphicsManager.SetRastState(DeviceStates.rastStateNoCull);
            } else {*/
                GraphicsManager.SetRastState(DeviceStates.rastStateSolid);
            //}

            if (SceneManager.Pass != RenderPass.Shadows) {
                if (!instanced) {
                    Contract.Requires(!material.technique.EndsWith("Instanced"));
                }

                if (Rank!=lastRank) {

                    if (material.technique!=lastPass) {
                        lastPass=material.technique;
                        //reset textures
                        lastTexture = null;
                        lastTexture2= null;
                    }

                    if (material.textures!=null) {
                        if (material.textures[0]!=lastTexture) {
                            Display.context.PixelShader.SetShaderResource(0, (material.textures[0]!=null?material.textures[0].ressource:null));
                            lastTexture=material.textures[0];
                        }
                    }
                }

                if (material.textures!=null && material.textures.Length>1 && material.textures[1] != lastTexture2) {
                    Display.context.PixelShader.SetShaderResource(1, (material.textures[1]!=null?material.textures[1].ressource:null));
                    lastTexture2 = material.textures[1];
                }
                if (material.textures!=null && material.textures.Length>2 && material.textures[2] != lastTexture2) {
                    Display.context.PixelShader.SetShaderResource(2, (material.textures[2]!=null?material.textures[2].ressource:null));
                    lastTexture2 = material.textures[2];
                }
            }

            lastRank = Rank;
        }

    }

}
