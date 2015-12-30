using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rectangle = System.Drawing.Rectangle;

namespace RageRTS {
    public class InteractiveObject2D {
        public InteractiveObject entity;
        public int side;
        public int iconId=-1;
        public Vector2 position;
        public Rectangle rect;
        public int bracketSize;
        public float size;
        public bool onScreen, focused, selected;

        public string progress_text;
        public short progress=-1;

        public InteractiveObject2D(InteractiveObject iobj) {
            entity = iobj;
            side = entity.Side;
        }
    }
}
