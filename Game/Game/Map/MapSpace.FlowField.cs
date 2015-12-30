using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine;

namespace RageRTS.Map
{
    public partial class MapSpace
    {
        private FlowFieldPath foundPath;

        public FlowFieldPath FindPath(List<Point2D> points, Point2D destination) {

            foundPath = new FlowFieldPath(Width * Height);

            int DestX = (int)(destination.X / CellSize);
            int DestY = (int)(destination.Y / CellSize);

            LinkedList<Point2D> openList = new LinkedList<Point2D>();

            Point2D current = new Point2D(DestX, DestY);
            openList.AddLast(current);



            int x, y;
            foreach (Point2D p in points) {
                var pw = p;
                x = (int)(pw.X / CellSize);
                y = (int)(pw.Y / CellSize);
                foundPath.Set(x  + y * Width, 1);
            }


            int tmpX, tmpY, pathId;

            int foundAmount=0;
            bool stopAdding = false;

            // Experimental poitn of view style ( like in supcom 2 ) flowfield optimization
            //------------------------------------------------------------------------------------
            // Disabled because it doesn't respect diagonal obstruction rule
            /*if (false) { 
                int limit = 0, maxLimit = 20;
                foreach (Point2D p in points) {
                    x = Math.Abs((int)(p.X / CellSize) - DestX);
                    y = Math.Abs((int)(p.Y / CellSize) - DestY);
                    if (x > limit && y > limit) limit = Math.Min(Math.Max(x, y), maxLimit);
                }

                for (int xtmp = -limit; xtmp <= limit; xtmp++) {
                    for (int ytmp = -limit; ytmp <= limit; ytmp++) {
                        pathId = (DestX + xtmp) + (DestY + ytmp) * Width;
                        if (pathId >= 0 && pathId < Height * Width && (xtmp == -limit || ytmp == -limit || xtmp == limit || ytmp == limit))
                            Line(DestX, DestY, DestX + xtmp, DestY + ytmp, foundPath, openList, ref foundAmount);
                    }
                }

                if (foundAmount == points.Count) { openList.Clear(); }
            }*/
            //------------------------------------------------------------------------------------

            while (openList.Count > 0) {

                current = openList.First.Value;
                openList.RemoveFirst();

                //Find neighbours
                byte bitmask = pathfinding[current.X + current.Y * Width];
                for (int i = 0; i < 8; i++) {
                    if ((bitmask & (1 << i)) == (1 << i)) {

                        tmpX = current.X + Direction[i, 0];
                        tmpY = current.Y + Direction[i, 1];
                        pathId = tmpX + tmpY * Width;

                        byte found = foundPath.Get(pathId);
                        if (found == 0 || found == 1) {

                            if (!stopAdding) openList.AddLast(new Point2D(tmpX, tmpY));
                            foundPath.Set(pathId,(byte)(2 + i));

                            if (found == 1) {
                                foundAmount++;
                                if (foundAmount == points.Count) stopAdding = true;
                            }
                        }
                        /*} else {
                            tmpX = current.X + Direction[i, 0];
                            tmpY = current.Y + Direction[i, 1];
                            pathId = tmpX + tmpY * width;
                            byte found = foundPath[pathId];
                            if (found == 1) {
                                foundAmount++;
                                if (foundAmount == points.Count) stopAdding = true;
                            }else if (found<100){
                                if (pathId>=0 && pathId<foundPath.Length) foundPath[pathId] = (byte)(102+i);
                            }
                        */
                    }
                }
            }

            foundPath.Set(DestX  + DestY * Width, 255);

            return foundPath;
        }

        private bool Mark(int x, int y, FlowFieldPath foundPath, LinkedList<Point2D> openList, ref int foundAmount) {
            int pathId = x + y * Width;
            if (pathId < 0 || pathId >= Height * Width || pathfinding[pathId] == 0) {
                return true;
            }
            if (foundPath.Get(pathId) == 1) foundAmount++;
            foundPath.Set(pathId,255);
            openList.AddLast(new Point2D(x, y));
            return false;
        }

        public void Line(int x1, int y1, int x2, int y2, FlowFieldPath foundPath, LinkedList<Point2D> openList, ref int foundAmount) {
            int deltax = Math.Abs(x2 - x1);    	// The difference between the x's
            int deltay = Math.Abs(y2 - y1);    	// The difference between the y's
            int x = x1;                   	// Start x off at the first pixel
            int y = y1;                   	// Start y off at the first pixel

            int xinc1, xinc2, yinc1, yinc2;

            if (x2 >= x1) {
                xinc1 = xinc2 = 1;
            } else {
                xinc1 = xinc2 = -1;
            }

            if (y2 >= y1) {
                yinc1 = yinc2 = 1;
            } else {
                yinc1 = yinc2 = -1;
            }

            int den, num, numadd, numpixels;

            if (deltax >= deltay)     	// There is at least one x-value for every y-value
            {
                xinc1 = 0;              	// Don't change the x when numerator >= denominator
                yinc2 = 0;              	// Don't change the y for every iteration
                den = deltax;
                num = deltax / 2;
                numadd = deltay;
                numpixels = deltax;     	// There are more x-values than y-values
            } else                      	// There is at least one y-value for every x-value
            {
                xinc2 = 0;              	// Don't change the x for every iteration
                yinc1 = 0;              	// Don't change the y when numerator >= denominator
                den = deltay;
                num = deltay / 2;
                numadd = deltax;
                numpixels = deltay;     	// There are more y-values than x-values
            }

            for (int curpixel = 0; curpixel <= numpixels; curpixel++) {
                if (Mark(x, y, foundPath, openList, ref foundAmount)) break;
                num += numadd;          	// Increase the numerator by the top of the fraction
                if (num >= den) {         	// Check if numerator >= denominator
                    num -= den;           	// Calculate the new numerator value
                    x += xinc1;           	// Change the x as appropriate
                    y += yinc1;           	// Change the y as appropriate
                }
                x += xinc2;             	// Change the x as appropriate
                y += yinc2;             	// Change the y as appropriate
            }
        }
    }

    public class FlowFieldPath
    {
        public byte[] path = null;
        public int Length = -1;

        public FlowFieldPath(int size) {
            path = new byte[size];
            Length = size;
        }

        public void Set(int index, byte value){
            path[index]=value;
        }

        public byte Get(int index) {
            return path[index];
        }
    }
}
