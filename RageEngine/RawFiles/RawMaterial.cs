using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.Graphics;
using SharpDX;

namespace RageEngine.RawFiles
{
    public class RawMaterial
    {
        public ushort id = 0;

        public string technique;
        public MaterialMode mode;
        public string[] textures;
        public Vector4 color;
        public float delta;
    }
}
