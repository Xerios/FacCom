using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine;
using RageEngine.ContentPipeline;
using SharpDX;

namespace RageEngine.Graphics.TwoD {


    public static class FontRenderer {

        public static void Draw(string font, string text, Vector2 position, Color4 color, float size = 1) {
            Draw(FontManager.Get(font),text,position,color,size);
        }

        public static void Draw(Font2D font, string text, Vector2 position, Color4 color, float size = 1){
            if (font == null) throw new Exception("Font doesn't exist.");
            if (text == null) return;

            //bool newLine = true;
            Vector2 overallPosition = Vector2.Zero;
            Rectangle source;
            Rectangle destination;
            Vector2 charPosition;

            int lastChar = 0;

            for (int i = 0; i < text.Length; i++)
            {
                // Work out current character
                char character = text[i];

                switch (character)
                {
                    case '\t': 
                        {
                            i++;// skip #
                            float r = Byte.Parse(text[i++].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                            float g = Byte.Parse(text[i++].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                            float b = Byte.Parse(text[i].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                            color = new Color4(r/15, g/15, b/15,1);
                            break;
                        }
                    case '\n': 
                        {
                            //newLine = true;
                            overallPosition.X = 0.0f;
                            overallPosition.Y += (font.LineSpacing) * size;

                            break;
                        }
                    default:
                        {
                            int realIndex = font.GetCharacterIndex(character);
                            int characterIndex = realIndex;
                            if (characterIndex == -1) characterIndex = font.GetCharacterIndex('?');

                            //float charKerning = font.kerning.Find(letter => letter.X==lastChar && letter.Y==characterIndex).Z;

                            charPosition = overallPosition + position;

                            source = font.glyphData[characterIndex];

                            Rectangle cropData = font.croppingData[characterIndex];

                            destination = new Rectangle();
                            destination.Left= (int)Math.Ceiling(charPosition.X + cropData.X * size);
                            destination.Top = (int)Math.Ceiling(charPosition.Y + cropData.Y * size);
                            destination.Right=(int)Math.Ceiling(source.Width * size);
                            destination.Bottom=(int)Math.Ceiling(source.Height * size);

                            QuadRenderer.Draw(font.textureValue, destination, source, color);

                            overallPosition.X += cropData.Width * size;
                            //newLine = false;

                            lastChar = characterIndex;

                            break;
                        }
                }
            }
        }

    }
}
