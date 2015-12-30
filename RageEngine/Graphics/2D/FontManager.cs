using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.ContentPipeline;
using RageEngine.Debug;

namespace RageEngine.Graphics.TwoD {
    public static class FontManager {

        private static Dictionary<string, Font2D> fonts = new Dictionary<string, Font2D>(20);

        public static void Dispose() {
            foreach (KeyValuePair<string, Font2D> keyPair in fonts) keyPair.Value.Dispose();
        }

        public static void Add(string fontLabel, Font2D font) {
            if (fontLabel != null && !fonts.ContainsKey(fontLabel)) {
                fonts.Add(fontLabel, font);
                GameConsole.Add("FontManager", "New font added '"+fontLabel+"'");
            } else GameConsole.Add("FontManager", "Font '"+fontLabel+"' already exists in the dictionnary");
        }

        public static Font2D Get(string fontLabel) {
            Font2D s;
            fonts.TryGetValue(fontLabel, out s);
            return s;
        }

    }
}
