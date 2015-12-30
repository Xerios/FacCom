using System;
using System.Collections.Generic;
using System.IO;
using RageEngine.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace RageEngine.ContentPipeline {
    public static class Resources {
        public static string Main_Path="../data/";

        public static string FindFile(string ressource) {
            ressource=ressource.Replace(Main_Path, "");
            if (ressource.Length!=0 && ressource[1]!=':') ressource=Path.Combine(Main_Path, ressource);
            return ressource;
        }

        public static void Dispose() {
            foreach (KeyValuePair<string, Texture> tex in list) tex.Value.Dispose();
            list.Clear();
        }

        public static Font2D GetFont(string ressource) {
            Font2D font = new Font2D();
            String currentPath = FindFile(ressource);
            if (!File.Exists(currentPath)) {
                FontFile file = FontLoader.Load(currentPath+".fnt");
                font.LineSpacing = file.Common.LineHeight;

                foreach(FontChar fchar in file.Chars){
                    font.characterMap.Add((char)fchar.ID);
                    font.glyphData.Add(new Rectangle(fchar.X, fchar.Y, fchar.X+fchar.Width, fchar.Y+fchar.Height));
                    font.croppingData.Add(new Rectangle(fchar.XOffset, fchar.YOffset, fchar.XOffset+fchar.XAdvance, fchar.YOffset+fchar.YOffset));
                }
                foreach(FontKerning kern in file.Kernings){
                    font.kerning.Add(new Vector3(kern.First,kern.Second,kern.Amount));
                }
            }
            font.textureValue = GetTexture(currentPath+"_0",false);
            return font;
        }


        private static Texture whiteTexture=null;
        public static Texture GetEmptyTexture() {
            if (whiteTexture!=null) return whiteTexture;
            whiteTexture = GetTexture(FindFile("dot.png"), false, Format.R8G8B8A8_UNorm_SRgb);
            /*Texture2DDescription desc = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = Format.R8G8B8A8_UNorm,
                Width = Display.width,
                Height = Display.height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
            };
            Reso
            DataRectangle dr=new DataRectangle();

            ShaderResourceView res = new ShaderResourceView(Display.device, Texture2DArray, desc);
            whiteTexture = new Texture(res, 1, 1);*/
            return whiteTexture;
        }

        private static Dictionary<string, Texture> list = new Dictionary<string, Texture>(100);
        private static Dictionary<Texture, string> listReverse = new Dictionary<Texture, string>(100);


        public static string GetTextureFilePath(Texture tex) {
            string path;
            if (listReverse.TryGetValue(tex,out path)) return path;
            return null;
        }

        public static Texture GetTexture(string ressource, bool mipmap = true, Format format = Format.BC3_UNorm) {
            Texture returnTex;
            lock (list) {
                bool found = list.TryGetValue(ressource, out returnTex);

                if (!found) {
                    String currentPath = FindFile(ressource);

                    if (!File.Exists(currentPath)) {
                        if (File.Exists(currentPath + ".jpg")) currentPath = currentPath + ".jpg";
                        else if (File.Exists(currentPath + ".png")) currentPath = currentPath + ".png";
                        else if (File.Exists(currentPath + ".dds")) currentPath = currentPath + ".dds";
                        else currentPath = Main_Path + "error.jpg";
                    }

                    ImageLoadInformation loadinfo = new ImageLoadInformation();

                    loadinfo.BindFlags = BindFlags.ShaderResource;
                    loadinfo.CpuAccessFlags = CpuAccessFlags.None;
                    loadinfo.Depth = 0;
                    loadinfo.Filter = FilterFlags.None;
                    loadinfo.FirstMipLevel = 0;
                    loadinfo.Format = format;
                    loadinfo.MipFilter = FilterFlags.Linear;
                    loadinfo.MipLevels = (mipmap ? 0 : 1);
                    loadinfo.OptionFlags = ResourceOptionFlags.None;
                    loadinfo.Usage = ResourceUsage.Immutable;


                    Texture2D texture = Texture2D.FromFile<Texture2D>(Display.device, currentPath, loadinfo);

                    returnTex = new Texture(texture);
                    list.Add(ressource, returnTex);
                    listReverse.Add(returnTex, ressource);
                }
            }
            return returnTex;
        }

        public static Texture2D GetMappableTexture(string ressource) {
            String currentPath = FindFile(ressource);

            if (!File.Exists(currentPath)) {
                if (File.Exists(currentPath + ".jpg")) currentPath = currentPath + ".jpg";
                else if (File.Exists(currentPath + ".png")) currentPath = currentPath + ".png";
                else if (File.Exists(currentPath + ".dds")) currentPath = currentPath + ".dds";
                else currentPath = Main_Path + "error.jpg";//throw new ArgumentNullException("Texture '" + currentPath + "' not found.");
            }

            ImageLoadInformation loadinfo = new ImageLoadInformation();
            loadinfo.BindFlags = BindFlags.None;
            loadinfo.CpuAccessFlags = CpuAccessFlags.Read;
            loadinfo.Format = Format.R8G8B8A8_UNorm;
            loadinfo.Usage = ResourceUsage.Staging;
            loadinfo.MipFilter = FilterFlags.None;
            loadinfo.Filter = FilterFlags.None;
            loadinfo.MipLevels=1;


            return Texture2D.FromFile<Texture2D>(Display.device, currentPath, loadinfo);
        }


        public static Texture GetTextureArray(string[] ressources) {
            Texture returnTex = null;
            //bool found = list.TryGetValue(ressource,out returnTex);
            //if (!found) {
            Texture2D[] textures = new Texture2D[ressources.Length];
            for(int i=0;i<textures.Length;i++){
                String currentPath = FindFile(ressources[i]);

                if (!File.Exists(currentPath)) {
                    if (File.Exists(currentPath + ".jpg")) currentPath = currentPath + ".jpg";
                    else if (File.Exists(currentPath + ".png")) currentPath = currentPath + ".png";
                    else if (File.Exists(currentPath + ".dds")) currentPath = currentPath + ".dds";
                    else currentPath = Main_Path + "error.jpg";//throw new ArgumentNullException("Texture '" + currentPath + "' not found.");
                }

                ImageLoadInformation loadinfo = new ImageLoadInformation();
                loadinfo.BindFlags = BindFlags.None;
                loadinfo.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;
                loadinfo.Format = Format.BC3_UNorm;
                loadinfo.FirstMipLevel = 0;
                loadinfo.MipFilter = FilterFlags.Linear;
                loadinfo.Usage = ResourceUsage.Staging;

                textures[i] = Texture2D.FromFile<Texture2D>(Display.device, currentPath, loadinfo);
            }


            Texture2DDescription descHM = new Texture2DDescription() {
                ArraySize = ressources.Length,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.BC3_UNorm,
                Width = textures[0].Description.Width,
                Height = textures[0].Description.Height,
                MipLevels = textures[0].Description.MipLevels,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            };

            Texture2D Texture2DArray = new Texture2D(Display.device, descHM);
            int mipLevels = textures[0].Description.MipLevels;
            int Xsize = textures[0].Description.Width;
            int outSize;
            lock (Display.device) {
                for (int i = 0; i < textures.Length; ++i) {
                    // for each mipmap level...
                    for (int j = 0; j < mipLevels; ++j) {
                        DataBox databox = Display.context.MapSubresource(textures[i], j, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None, out outSize);
                        Display.context.UpdateSubresource(databox, Texture2DArray, SharpDX.Direct3D11.Resource.CalculateSubResourceIndex(j, i, mipLevels));
                        Display.context.UnmapSubresource(textures[i], j);
                    }
                }
            }
            for (int i = 0; i < textures.Length; ++i) textures[i].Dispose();

            ShaderResourceViewDescription viewDesc = new ShaderResourceViewDescription();
            viewDesc.Format = descHM.Format;
            viewDesc.Dimension = ShaderResourceViewDimension.Texture2DArray;
            viewDesc.Texture2DArray.ArraySize = textures.Length;
            viewDesc.Texture2DArray.MostDetailedMip = 0;
            viewDesc.Texture2DArray.FirstArraySlice = 0;
            viewDesc.Texture2DArray.MipLevels = mipLevels;

            ShaderResourceView Texture2DArrayView = new ShaderResourceView(Display.device, Texture2DArray, viewDesc);
            returnTex = new Texture(Texture2DArrayView,Xsize,Xsize);
            returnTex.Tex = Texture2DArray;

            return returnTex;
        }

        public static Shader GetShader(string type,string profile,string ressource) {
            String currentPath = FindFile(ressource);

            if (!File.Exists(currentPath)) 
                if (File.Exists(currentPath + ".fx")) 
                    currentPath = currentPath + ".fx";
                else
                    throw new ArgumentNullException("Shader '" + currentPath + "' not found.");

#if DEBUG
            ShaderFlags flags = ShaderFlags.Debug | ShaderFlags.SkipOptimization | ShaderFlags.PackMatrixColumnMajor | ShaderFlags.AvoidFlowControl;
#else
            ShaderFlags flags = ShaderFlags.OptimizationLevel3 | ShaderFlags.EnableBackwardsCompatibility | ShaderFlags.PackMatrixColumnMajor | ShaderFlags.AvoidFlowControl;
#endif
            Shader shader;


            if (type=="VS") {
                var vertexShaderByteCode = ShaderBytecode.CompileFromFile(currentPath, profile, "vs_4_0", flags, EffectFlags.None);
                shader = new ShaderVS(vertexShaderByteCode);
                vertexShaderByteCode.Dispose();
            } else {
                var pixelShaderByteCode = ShaderBytecode.CompileFromFile(currentPath, profile, "ps_4_0", flags, EffectFlags.None);
                shader = new ShaderPS(pixelShaderByteCode);
                pixelShaderByteCode.Dispose();
            }

            return shader;
        }


        public static StreamReader GetStreamReader(string ressource) {
            return new StreamReader(FindFile(ressource));
        }


    }
}
