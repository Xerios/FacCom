using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SharpDX;

//////////////////////////////////////////////////////////////
//                                                          //
//  Geographic Grid Reference Manager                       //
//                                                          //
//  This should realy derive from GameComponent but have    //
//  added graphical elements so you can see what is going   //
//  on. Remove them and inherit from GameComponent.         //
//                                                          //
//////////////////////////////////////////////////////////////

namespace RageEngine.LQD
{
    public class Grid2DManager 
    {

        private Vector2 gridDimensions = Vector2.Zero;
        private Rectangle gridElementSize = new Rectangle();

        public Dictionary<Vector2, Rectangle> theGrid;
        public Dictionary<Vector2, List<GridObject2D>> gridObjects;
        public List<GridObject2D> listObjects;
        public Dictionary<GridObject2D, List<Vector2>> objectRegisteredZones;
        public List<Rectangle> bounds = new List<Rectangle>();


        public Grid2DManager(Vector2 dimensions,Rectangle ElementSize)
        {
            gridDimensions = dimensions;
            gridElementSize = ElementSize;

            buildGrid();
        }

        
        private void buildGrid()
        {
            // Build the grid.
            gridObjects = new Dictionary<Vector2,List<GridObject2D>>();
            theGrid = new Dictionary<Vector2,Rectangle>();
            listObjects = new List<GridObject2D>();
            bounds = new List<Rectangle>();
            objectRegisteredZones = new Dictionary<GridObject2D,List<Vector2>>();
            
            Vector2 width = new Vector2(gridElementSize.Width,gridElementSize.Height);

            for (int x = 0; x < gridDimensions.X; x++)
            {
                for (int y = 0; y < gridDimensions.Y; y++)
                {                    
                    Vector2 gridRef = new Vector2(x, y);
                    Vector2 pos = gridRef * width;
                    Rectangle locBounds = new Rectangle((int)(gridElementSize.Left + pos.X),(int)(gridElementSize.Top + pos.Y),(int)gridElementSize.Width,(int)gridElementSize.Height);
                    theGrid.Add(gridRef, locBounds);
                    gridObjects.Add(gridRef, new List<GridObject2D>());
                    bounds.Add(locBounds);                
                }
            }
        }
        /*protected override void LoadContent()
        {            
            shader = new BasicEffect(Game.GraphicsDevice, null);
            pixel = new Texture2D(Game.GraphicsDevice, 1, 1, 0, TextureUsage.None, SurfaceFormat.Color);
            Color[] data = new Color[1];
            data[0] = Color.White;

            pixel.SetData<Color>(data);

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.LoadContent();
        }*/

        public void Update(GameTime gameTime)
        {
            // Check if object has moved. If it has, re register.
            for (int i = 0; i < listObjects.Count; i++)
            {
                if (listObjects[i].HasMoved)
                    RegisterObject(listObjects[i]);
            }
                        
            //base.Update(gameTime);
        }

        public void AddObject(GridObject2D obj)
        {
            listObjects.Add(obj);
            objectRegisteredZones.Add(obj, new List<Vector2>());
            RegisterObject(obj);
        }

        private Vector2 GetGridRef(GridObject2D obj)
        {
            Vector2 gridRef = Vector2.Zero;

            for (int x = 0; x < gridDimensions.X; x++)
            {
                for (int y = 0; y < gridDimensions.Y; y++)
                {
                    Vector2 checkRef = new Vector2(x,y);
                    if (theGrid[checkRef].Intersects(obj.Bounds))
                        return checkRef;                    
                }
            }
            return gridRef;
        }
        
        private void RegisterObject(GridObject2D obj)
        {
            bool registered = false;
            Vector2 gridRef = Vector2.Zero;
            List<GridObject2D> objList;

            // Get my zones.
            List<Vector2> registeredZones = new List<Vector2>();

            if (objectRegisteredZones.ContainsKey(obj))
                registeredZones = objectRegisteredZones[obj];

            // Am I already registered?
            if (registeredZones.Count == 0)
            {
                gridRef = GetGridRef(obj);
                registered = false;
            }
            else
                registered = true;

            // If not then find my initial place in the grid..
            if (!registered)
            {
                // Register with this grid ref.
                objList = gridObjects[gridRef];
                objList.Add(obj);
                gridObjects[gridRef] = objList;
                registeredZones.Add(gridRef);
                objectRegisteredZones[obj] = registeredZones;
            }
            else // If so, then am I still in these zones?
            {
                for (int r = 0; r < registeredZones.Count; r++)
                {
                    if (!theGrid[registeredZones[r]].Intersects(obj.Bounds))
                    {
                        // Remove from this ref
                        // Remove me from the object list in the ref.
                        objList = gridObjects[registeredZones[r]];
                        objList.Remove(obj);
                        gridObjects[registeredZones[r]] = objList;

                        // Remove my registration with this ref.
                        registeredZones.Remove(registeredZones[r]);
                        objectRegisteredZones[obj] = registeredZones;
                    }
                }
            }
            // Am I also in neigbouring zones?
            for (int r = 0; r < objectRegisteredZones[obj].Count; r++)
            {
                gridRef = objectRegisteredZones[obj][r];
                for (int x = (int)gridRef.X - 1; x < gridRef.X + 2; x++)
                {
                    for (int y = (int)gridRef.Y - 1; y < gridRef.Y + 2; y++)
                    {
                        if (x >= 0 && x < gridDimensions.X && y >= 0 && y < gridDimensions.Y)
                        {
                            Vector2 thisRef = new Vector2(x, y);
                            if (!registeredZones.Contains(thisRef))
                            {
                                if (theGrid[thisRef].Intersects(obj.Bounds))
                                {
                                    // Register with this grid ref.
                                    objList = gridObjects[thisRef];
                                    objList.Add(obj);
                                    gridObjects[thisRef] = objList;
                                    registeredZones.Add(thisRef);
                                    objectRegisteredZones[obj] = registeredZones;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<GridObject2D> GetObjectsInMyZone(GridObject2D obj)
        {
            List<Vector2> registeredZones = new List<Vector2>();
            List<GridObject2D> objList = new List<GridObject2D>();

            if (objectRegisteredZones.ContainsKey(obj))
                registeredZones = objectRegisteredZones[obj];

            if (registeredZones.Count > 0)
            {
                for (int z = 0; z < registeredZones.Count; z++)
                {
                    for (int o = 0; o < gridObjects[registeredZones[z]].Count; o++)
                    {
                        if (gridObjects[registeredZones[z]][o] != obj)
                        {
                            if (!objList.Contains(gridObjects[registeredZones[z]][o]))
                                objList.Add(gridObjects[registeredZones[z]][o]);
                        }                        
                    }
                }                
            }

            return objList;
        }
        /*
        public override void Draw(GameTime gameTime)
        {
            DrawBounds(base.Game.GraphicsDevice, bounds);
            base.Draw(gameTime);
        }

        protected VertexPositionColor[] points;
        protected short[] index;
        private void DrawBounds(GraphicsDevice myDevice, List<Rectangle> bounds)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            // Draw squares...
            for (int b = 0; b < bounds.Count; b++)
            {
                spriteBatch.Draw(pixel, bounds[b], Color.SkyBlue);
                // Draw top hriz
                spriteBatch.Draw(pixel, new Rectangle(bounds[b].X, bounds[b].Y, bounds[b].Width, 1), Color.White);
                // Draw left vert
                spriteBatch.Draw(pixel, new Rectangle(bounds[b].X, bounds[b].Y, 1, bounds[b].Height), Color.White);
                // Draw right vert
                spriteBatch.Draw(pixel, new Rectangle(bounds[b].X + bounds[b].Width, bounds[b].Y, 1, bounds[b].Height), Color.White);
                // Draw bottom hriz
                spriteBatch.Draw(pixel, new Rectangle(bounds[b].X, bounds[b].Y + bounds[b].Height, bounds[b].Width, 1), Color.White);

            }
            spriteBatch.End();
        }*/
    }
}
