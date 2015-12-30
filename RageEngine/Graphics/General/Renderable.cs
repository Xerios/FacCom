using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.Graphics {
    public enum RenderPass { 
        None       = 0, 
        Solid       = 1, 
        Transparent = 2, 
        Post        = 4, 
        Shadows     = 8,
        UnderWater     = 16,
        /*XXXXXXX1     = 0x8,
        XXXXXXX2     = 0x10,*/
    }

    public interface Renderable {
        int GetRenderPass();
        void Render();
    }
}
