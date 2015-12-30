using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.Direct3D11;

namespace RageEngine.Graphics {

    public class Texture {
        public static ushort UNIQUE_ID;
        public ushort id;

        public ShaderResourceView ressource;
        public Texture2D Tex;

        public int Width;
        public int Height;

        public Texture() {
            id = UNIQUE_ID++;
        }

        public Texture(ShaderResourceView res, int w, int h) {
            id = UNIQUE_ID++;
            ressource = res; 
            Width = w;
            Height = h;
        }
        public Texture(Texture2D tex=null){
            id = UNIQUE_ID++;
            Tex = tex;
            Width =  Tex.Description.Width;
            Height = Tex.Description.Height;
            ressource = new ShaderResourceView(Display.device, Tex);
        }

        public void RecreateSRV(){
            if (ressource != null) ressource.Dispose();
            ressource = new ShaderResourceView(Display.device, Tex);
        }

        public void Dispose() {
            if (ressource != null) ressource.Dispose();
        }
    }
}
