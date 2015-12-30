using SharpDX;
using System;
using System.Collections.Generic;

//////////////////////////////////////////////////////////////
//                                                          //
//  GidObject                                               //
//                                                          //
//  Please don't use this object it is just givs the        //
//  minimum requirements an object should have to interact  //
//  with the grid.                                          //
//                                                          //
//////////////////////////////////////////////////////////////

namespace RageEngine.LQD
{
    public class GridObject2D
    {
        private static int ID = 0;
        private string myID;
        private int size = 50;

        private Vector2 lastPos;
        public Vector2 Position;
        public Vector2 Scale;
        public bool HasMoved;

        private Vector2 origin;

        public Rectangle Bounds
        {
            get
            {
                Vector2 pos = Position - origin;
                
                if (Texture != null)
                    return new Rectangle((int)pos.X,(int)pos.Y, (int)Texture.Width, (int)Texture.Height);
                else
                    return new Rectangle((int)pos.X, (int)pos.Y , 1, 1);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (lastPos != Position)
                HasMoved = true;
            else
                HasMoved = false;

            lastPos = Position;

            float ang = (float)gameTime.TotalGameTime.TotalSeconds;
            
            float x = ((float)Math.Cos(ang) / 1);
            float y = ((float)Math.Sin(ang) / 1);

            if(myID != "[1]")
                Position += new Vector2(x, y);   
            
        }

        public GridObject2D()
        {
            ID++;
            myID = string.Format("[{0}]", ID);
        }

        public void LoadContent(GraphicsDevice myDevice)
        {
            if (Texture == null)
            {
                boxColor = new Color(boxColor.R, boxColor.G, boxColor.B, (byte)127);
                Texture = new Texture2D(myDevice, size, size, 1, TextureUsage.None, SurfaceFormat.Color);
                Color[] data = new Color[size * size];

                for (int x = 0; x < size * size; x++)
                    data[x] = boxColor;

                Texture.SetData<Color>(data);

                origin = new Vector2(Texture.Width/2, Texture.Height/2);
            }
        }

        public void DrawBounds(GraphicsDevice myDevice,SpriteBatch spriteBatch,SpriteFont font)
        {
            // Draw objects texture.
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, size, size), new Rectangle(0, 0, Texture.Width,Texture.Height ), Color.Blue, 0, origin , SpriteEffects.None, 0);

            // Draw objects bounds.
            spriteBatch.Draw(Texture, Bounds, new Rectangle(0, 0, Texture.Width , Texture.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);

            // Draw ovjects ID.
            spriteBatch.DrawString(font, myID, new Vector2((int)Bounds.X + (25 - font.MeasureString(myID).X/2), (int)Bounds.Y + (25 - font.MeasureString(myID).Y/2)), Color.White);
        }
    }
}
