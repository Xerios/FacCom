using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.Graphics;
using SharpDX;

namespace RageEngine.ContentPipeline {

    public class Font2D {
        public Texture textureValue;
        public List<char> characterMap;
        public List<Rectangle> croppingData;
        public List<Rectangle> glyphData;
        public List<Vector3> kerning;
        public int LineSpacing;
        public float spacing;

        public Font2D() {
            characterMap = new List<char>();
            croppingData = new List<Rectangle>();
            glyphData = new List<Rectangle>();
            kerning = new List<Vector3>();
        }

        public Vector2 MeasureString(string str) { // Doesn't properly support new lines
            //bool newLine = true;
            Vector2 size = new Vector2(0, 0);
            size.Y= LineSpacing;

            float biggestSizeX=0;
            int lastChar = 0;

            for (int i = 0; i < str.Length; i++) {
                // Work out current character
                char character = str[i];

                switch (character) {
                    case '\t': break;
                    case '\n':
                    //newLine = true;
                    size.X = 0;
                    size.Y += (LineSpacing);
                    break;
                    default:
                    int characterIndex = GetCharacterIndex(character);

                    if (characterIndex == -1) characterIndex = GetCharacterIndex('?');

                    //float charKerning = kerning.Find(letter => letter.X==lastChar && letter.Y==characterIndex).Z;

                    Rectangle cropData = croppingData[characterIndex];
                    size.X+=cropData.X + cropData.Width;
                    if (cropData.Height>size.Y) size.Y=cropData.Height;
                    if (size.X>biggestSizeX) biggestSizeX=size.X;


                    lastChar = characterIndex;

                    //newLine = false;
                    break;
                }
            }
            size.X=biggestSizeX;

            // int characterIndex = GetCharacterIndex(' ');
            //Rectangle destination = glyphData[characterIndex];
            return size;// new Vector2(destination.Width * str.Length, destination.Height);
        }

        public int GetCharacterIndex(char character) {
            int num2 = 0;
            int num3 = this.characterMap.Count - 1;

            while (num2 <= num3) {
                int num = num2 + ((num3 - num2) >> 1);
                char ch = this.characterMap[num];
                if (ch == character) {
                    return num;
                }
                if (ch < character) {
                    num2 = num + 1;
                } else {
                    num3 = num - 1;
                }
            }

            return -1;
        }

        public void Dispose() {
            textureValue.Dispose();
        }
    }

}
