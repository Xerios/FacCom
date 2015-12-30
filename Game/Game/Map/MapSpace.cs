using System;
using System.Collections.Generic;
using System.Text;
using RageEngine;
using RageEngine.Utils;
using RageRTS;
using RageEngine.Graphics;
using RageEngine.Rendering;
using SharpDX;

namespace RageRTS.Map 
{
    public partial class MapSpace
    {

        private TerrainInfo info;
        public Dictionary<Point2D,InteractiveObject> objectsMap = new Dictionary<Point2D,InteractiveObject>(500);

        public byte[] staticSpace,   // Map Obstruction
                      unitSpace,     // Unit avoidance ( cleared every update )( not used atm )
                      buildingSpace, // Unbuildable spaces
                      pathfinding;   // Calculated available directions

        public const int CellSize = 2;
        public int Width,Height;

        public MapSpace(TerrainInfo info) {
            this.info = info;
            Width = (int)Math.Ceiling((float)info.width/CellSize);
            Height = (int)Math.Ceiling((float)info.height/CellSize);

            staticSpace = new byte[Width * Height];
            Array.Copy(info.accessibilityArray, staticSpace, info.accessibilityArray.Length);

            unitSpace = new byte[Width * Height];
            buildingSpace = new byte[Width * Height];
            pathfinding = new byte[Width * Height];


            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    byte bitmask = 0;
                    int id = x + y * Width;
                    if (staticSpace[id] != 0) {
                        for (int i = 0; i < 8; i++) {
                            if ((i >= 4) && ((bitmask & DirectionDiagonalBlock[i - 4]) == 0)) continue; // Disable diagonal small spaces
                            int tmpX = x + Direction[i, 0];
                            int tmpY = y + Direction[i, 1];
                            if (tmpX < 0 || tmpY < 0 || tmpX >= Width || tmpY >= Height) continue;
                            int tmpId = tmpX + tmpY * Width;
                            if (staticSpace[tmpId] != 0) bitmask |= (byte)(1 << i);
                        }
                    }
                    pathfinding[id] = bitmask;
                }
            }
        }

        public void Init() {
        }
        
        public int GetIndex(Point2D pos){
            int posX, posY;
            posX = (int)(pos.X / CellSize);
            posY = (int)(pos.Y / CellSize);
            return posX + posY * Width;
        }


        /*public void UnitSpaceMark(Point2D posReal, int size) {
            Point2D pos = WorldToSpace(posReal);
            int sizeSq = (size * size) - 2;
            for (int x = -size; x <= size; x++) {
                for (int y = -size; y <= size; y++) {
                    int newx = (pos.X + x);
                    int newy = (pos.Y + y);

                    int xDist = pos.X - newx;
                    int yDist = pos.Y - newy;
                    float dist = (xDist * xDist + yDist * yDist);
                    float power = dist / sizeSq;

                    int id = newx + newy * Width;

                    if (power<1 && id >= 0 && id < unitSpace.Length) {
                        int toAdd = unitSpace[id]+(int)((1-power) * 255);
                        if (toAdd>255) toAdd=255;
                        unitSpace[id] = (byte)(toAdd);
                    }
                }
            }
        }
        */

        public void GetSpaceObject(Point2D pos2D, out InteractiveObject obj) {
            int posX, posY;
            posX = (int)(pos2D.X / CellSize);
            posY = (int)(pos2D.Y / CellSize);
            GetSpaceObject(posX,posY,out obj);
        }

        public void GetSpaceObject(int posX,int posY, out InteractiveObject obj) {
            objectsMap.TryGetValue(new Point2D(posX, posY), out obj);
        }

        public FVector2 WorldToSpace(FVector2 p) {
            return p / (fint) 2;
        }

        public Point2D WorldToSpace(Vector3 p) {
            int posX, posY;
            posX = (int)(p.X / CellSize);
            posY = (int)(p.Z / CellSize);
            return new Point2D(posX,posY);
        }
        /*
        public Vector3 SpaceToWorld(int X, int Y) {
            Vector3 vec = new Vector3(X*cellSize + cellSize/2, 0, Y*cellSize + cellSize/2);
            return vec;
        }

        public Point2D WorldToSpace(Vector2 pos3D) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize);
            posY = (int)(pos3D.Y / cellSize);
            return new Point2D(posX, posY);
        }

        public Point2D WorldToSpace(Vector3 pos3D) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize) ;
            posY = (int)(pos3D.Z / cellSize) ;
            return new Point2D(posX, posY);
        }

        public int WorldToSpaceId(Vector3 pos3D) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize);
            posY = (int)(pos3D.Z / cellSize);
            int id = posX + posY * width;
            if (id < 0 || id >= buildingSpace.Length) return 0;
            return id;
        }

        public Vector2 GetNearestPos(Vector3 pos3D) {
            int posX, posY;
            posX = (int)( pos3D.X / cellSize) ;
            posY = (int)( pos3D.Z / cellSize) ;
            return new Vector2(posX, posY);
        }

        public Vector3 GetNearestRealPos(Vector3 pos3D) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize);
            posY = (int)(pos3D.Z / cellSize);
            if (posX < 0) posX = 0;
            if (posY < 0) posY = 0;
            if (posX >= width) posX = width-1;
            if (posY >= height) posY = height-1;
            return new Vector3(cellSize/2+ posX*cellSize,0, cellSize/2+ posY*cellSize);
        }*/

        public bool TestWithDiagonals(fint pX, fint pY) {
            int x = (pX / (fint)2).ToInt();
            int y = (pY / (fint)2).ToInt();

            int id = x + y * Width;
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            if (staticSpace[id] == 0)
                return false;
            
            fint modx = pX % fint.one;
            fint mody = pY % fint.one;

            byte bitmask=pathfinding[id];

            if ((bitmask & MapSpace.DirectionDiagonalBlock[0]) == 0) {
                if (mody < modx) return false;
            } else if ((bitmask & MapSpace.DirectionDiagonalBlock[1]) == 0) {
                if (mody > modx) return false;
            }
            if ((bitmask & MapSpace.DirectionDiagonalBlock[2]) == 0) {
                if (mody > ((fint)1 - modx)) return false;
            } else if ((bitmask & MapSpace.DirectionDiagonalBlock[3]) == 0) {
                if (mody < ((fint)1 - modx)) return false;
            }

            return true;
        }



        public bool Test(FVector2 p) {
            int x = (p.X / (fint)2).ToInt();
            int y = (p.Y / (fint)2).ToInt();

            int id = x + y * Width;
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            if (staticSpace[id] == 0)
                return false;
            return true;
        }

        public bool Test(int x, int y) {
            int id = x + y * Width;
            if (x < 0 || y < 0 || x >= Width || y >= Height) return false;
            if (staticSpace[id] == 0) return false;
            return true;
        }
        /*
        public bool Test(Vector3 pos3D, BlueprintIntObject tech) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize);
            posY = (int)(pos3D.Z / cellSize);

            for (int i = -4; i <= 4; i++)
                for (int j = -4; j <= 4; j++)
                        if (tech.space[i + 4 + (j + 4)*9]){
                            if ((posX + i) < 0 || (posY + j) < 0 || (posX + j) >= width || (posY + j) >= height) return false;
                            int id = (posX + i) + (posY+ j) * width;
                            if (staticSpace[id] == 0 || buildingSpace[id] != 0) return false;
                        }

            return true;
        }

        public bool TestNetwork(Vector3 pos3D, BlueprintIntObject tech) {
            int posX, posY;
            posX = (int)(pos3D.X / cellSize);
            posY = (int)(pos3D.Z / cellSize);

            for (int i = -4; i <= 4; i++)
                for (int j = -4; j <= 4; j++)
                    if (tech.space[i + 4 + (j + 4)*9]) {
                        if ((posX + i) < 0 || (posY + j) < 0 || (posX + j) >= width || (posY + j) >= height) return false;
                        int id = (posX + i) + (posY+ j) * width;
                        if (staticSpace[id] == 0) return false;
                    }

            return true;
        }

        public bool TestStatic(Vector3 x) {
            Vector2 vec = GetNearestPos(x);
            return TestStatic((int)vec.X, (int)vec.Y);
        }
        public bool TestStatic(int x, int y) {
            int id = x + y * width;
            if (x < 0 || y < 0 || x >= width || y >= height) return false;
            if (staticSpace[id] == 0) return false;
            return true;
        }

        public bool Test(Vector2 pos2D) {
            int posX, posY;
            posX = (int)(pos2D.X / cellSize);
            posY = (int)(pos2D.Y / cellSize);
            return Test(posX, posY);
        }
        public bool Test(Vector3 x) {
            Vector2 vec = GetNearestPos(x);
            return Test((int)vec.X, (int)vec.Y);
        }
        public bool Test(int x, int y) {
            int id = x + y * width;
            if (x < 0 || y < 0 || x >= width || y >= height) return false;
            if (staticSpace[id] == 0) return false;
            if (buildingSpace[id] != 0) return false;
            return true;
        }

        public bool TestNetwork(int x, int y) {
            int id = x + y * width;
            if (x < 0 || y < 0 || x >= width || y >= height) return false;
            if (staticSpace[id] == 0) return false;
            return true;
        }

        public void Set(InteractiveObject obj){
            _Set(obj, true);
        }

        public void Unset(InteractiveObject obj) {
            _Set(obj, false);
        }*/

        /*public void _Set(InteractiveObject obj, bool value) {
            int posX, posY;
            posX = (int)(obj.Position.X / CellSize);
            posY = (int)(obj.Position.Y / CellSize);

            for (int i=-4; i<=4; i++)
                for (int j = -4; j <= 4; j++) {
                    if (obj.blueprint.space[i + 4 + (j + 4)*9]) {
                        UpdatePathfinding(posX + i, posY + j, (byte)(value ? 1 : 0));

                        Point2D pos = new Point2D(posX + i, posY + j);
                        if (value) {
                            if (!objectsMap.ContainsKey(pos)) objectsMap.Add(pos, obj);
                        } else {
                            objectsMap.Remove(new Point2D(posX + i, posY + j));
                        }
                    }
                }
        }*/


        internal void UpdatePathfinding(int x, int y, byte taken) {

            UpdateTile(x, y, taken);

            for (int i = 0; i < 8; i++) {
                int tmpX = x + Direction[i, 0];
                int tmpY = y + Direction[i, 1];
                UpdateTile(tmpX, tmpY, -1);
            }
            return;

        }

        internal void UpdateTile(int x, int y, int taken) {
            byte bitmask = 0;
            int id = x + y * Width;
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            byte tile=pathfinding[id];

            if (tile==0 && taken==-1) return;

            if (taken==-1) {
                taken=(tile==0?1:0);
            } else {
                buildingSpace[id] = (byte)taken;
            }

            if (taken==0) {
                for (int i = 0; i < 8; i++) {
                    int tmpX = x + Direction[i, 0];
                    int tmpY = y + Direction[i, 1];
                    if (tmpX < 0 || tmpY < 0 || tmpX >= Width || tmpY >= Height) continue;
                    int tempId = tmpX + tmpY * Width;

                    if ((i >= 4) && ((bitmask & DirectionDiagonalBlock[i - 4]) == 0)) continue;
                    if (buildingSpace[tempId] == 0 && staticSpace[tempId] != 0) bitmask |= (byte)(1 << i);
                }
            }
            pathfinding[id] = bitmask;
        }

        /*public static void DrawGrid(int size, Color4 color){
            Vector3 corner1,corner2,corner3,corner4;
            //Calculate2DBounds(out corner1, out corner2, out corner3, out corner4);

            for (int x = 0; x < BackgroundGame.Map_Space.width; x+=size) {
                for (int y = 0; y < BackgroundGame.Map_Space.height; y+=size) {
                    Vector3 pos = new Vector3(x * 2, 0,y * 2);
                    //if (!ContainsIn2DBounds(pos, corner1, corner2, corner3, corner4)) continue;
                    ForegroundGame.Map.info.GetHeight(pos, ref pos.Y);
                    if (Global.frustum.Contains(pos) != ContainmentType.Disjoint) {
                        Vector3 vec2,vec3,vec4;
                      
                        vec2 = pos + new Vector3(size*2, 0, 0);ForegroundGame.Map.info.GetHeight(vec2, ref vec2.Y);                        
                        vec3 = pos + new Vector3(size*2, 0, size*2);ForegroundGame.Map.info.GetHeight(vec3, ref vec3.Y);                        
                        vec4 = pos + new Vector3(0, 0, size*2);ForegroundGame.Map.info.GetHeight(vec4, ref vec4.Y);
                        vec2.Y += 0.1f;
                        vec3.Y += 0.1f;
                        vec4.Y += 0.1f;

                        SceneManager.LineManager.AddLine(vec2, vec3, color);
                        SceneManager.LineManager.AddLine(vec3, vec4, color);
                    }
                }
            }
        }*/



        //---------------------------------------------------------------------------------------------------------
        // Do not modify order without modifying ALL the direction arrays
        //---------------------------------------------------------------------------------------------------------

        public static sbyte[,] Direction = new sbyte[8, 2]{ 
                { 0, 1},  // 0 : 1  : Up
                { 0,-1} , // 1 : 2  : Down
                { 1, 0},  // 2 : 4  : Left
                {-1, 0},  // 3 : 8  : Right
                { 1,-1},  // 4 : 16 : Down + Left
                {-1, 1},  // 5 : 32 : Up + Right
                { 1, 1},  // 6 : 64 : Up + Left
                {-1,-1}   // 7 : 128: Down + Right
            };
        public static sbyte[] DirectionOpposite = new sbyte[8] { 1, 0, 3, 2,   5, 4, 7, 6 };

        public static sbyte[] DirectionDiagonalBlock = new sbyte[4] { 
            6, // Down + Left
            9, // Up + Right
            5, // Up + Left
            10 // Down + Right
        };


        public void Update() {
            Array.Clear(unitSpace, 0, unitSpace.Length);
        }
    }

}
