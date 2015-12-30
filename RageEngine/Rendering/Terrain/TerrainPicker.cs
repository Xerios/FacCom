using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.Rendering {
    class TerrainPicker {

        /// <summary>Peforms collision detection between a ray and a height field</summary>
        public static class Ray3HeightFieldCollider {

            // TODO: This code is extremely efficient, but too darn complicated.
            //       Took me several days of my life to perfect this thing.
            //       Look for a simpler, maybe even less efficient but more maintainable solution

            /// <summary>Locates the impact time (if any) of a ray in a heightmap</summary>
            /// <param name="rayStart">Position the ray starts out from</param>
            /// <param name="rayDirection">Direction the ray is extending into</param>
            /// <param name="heightField">Height field to collide against</param>
            /// <returns>The exact time when the ray hits the heightmap</returns>
            /// <remarks>
            ///   This method uses a swept approach. It will determine the exact time index
            ///   where the ray first touches the heightmap mathematically, without resorting
            ///   to stepping.
            /// </remarks>
            public static LineContacts FindContacts(Vector3 rayStart, Vector3 rayDirection, TerrainInfo heightField) {
                float rayStartTimeIndex = 0.0f; // Time index to which rayStart has been moved

                // See whether the ray starts inside of the terrain. If so, we can directly return
                // a collision at time index 0.0 and don't need to cope with this special case in
                // the remainder of this method. 
                float startHeight = heightField.GetHeight(new Vector2(rayStart.X, rayStart.Z));
                if (rayStart.Y < startHeight)
                    return new LineContacts(0.0f, float.PositiveInfinity); // Who cares for exit?

                // Time when the ray will leave the valid height range
                float[] heightCrossingTime = null;

                // If the ray is starting above the range of valid heights for the heightmap,
                // we can skip detection until it enters the valid height range. If it doesn't,
                // the ray won't hit the heightmap altogether.
                int ValidHeightRange = 1000; // MAXHEIGHT ... heightField?
                if (rayStart.Y > ValidHeightRange) {

                    // If the ray does not enter the valid height range, it will never be able
                    // to touch the heightmap
                    if (rayDirection.Y >= 0.0f) {
                        return new LineContacts(float.NaN, float.NaN);
                    }

                    // Adjust the position to where the ray enters the valid height range
                    rayStartTimeIndex = (ValidHeightRange - rayStart.Y) / rayDirection.Y;
                    rayStart += rayDirection * rayStartTimeIndex;

                } else {

                    // If the ray started below the valid height range and goes upwards, it will
                    // eventually leave the valid height range
                    if (rayDirection.Y > 0.0f)
                        heightCrossingTime = new float[] {(ValidHeightRange - rayStart.Y) / rayDirection.Y};
                }

                // Special case: If the ray is perfectly vertical, it will neither enter nor
                // exit the area of the heightmap
                if ((rayDirection.X == 0.0f) && (rayDirection.Z == 0.0f)) {

                    // If the ray goes downwards, we can calculate when it hits the surface
                    if (rayDirection.Y < 0.0f) {
                        float impactTime = (startHeight - rayStart.Y) / rayDirection.Y + rayStartTimeIndex;
                        return new LineContacts(impactTime, float.PositiveInfinity);
                    }

                    // Otherwise, it won't hit the surface at all
                    return new LineContacts(float.NaN, float.NaN);

                }

                // Here we can be sure that:
                //
                //   - The ray does not go straight down, thus, a normal of its direction
                //     on the X/Z plane can be built without the risk for dividing by zero
                //   - The ray is above the ground and has not yet touched the heightmap
                //   - The ray's starting location is not within the terrain
                //   - The ray is within the valid height range and will eventually either
                //     * hit the ground (within the heightmap region oder outside of it)
                //     * or raise above the valid height range

                return internalFindHeightmapContacts(
                  rayStart, rayDirection, rayStartTimeIndex, heightCrossingTime, heightField
                );
            }

            /// <summary>Determines the point of contact between a ray and the heightmap</summary>
            /// <param name="rayStart">Location the ray starts at</param>
            /// <param name="rayDirection">Direction into which the ray is going</param>
            /// <param name="rayStartTimeIndex">Time index of the provided start location</param>
            /// <param name="heightCrossingTime">Time the ray crosses the valid height range</param>
            /// <param name="heightField">Height field to collide against</param>
            /// <returns>The point of contact between the ray and the heightmap</returns>
            private static LineContacts internalFindHeightmapContacts(
              Vector3 rayStart, Vector3 rayDirection, float rayStartTimeIndex,
              float[] heightCrossingTime,
              TerrainInfo heightField
            ) {
                // These coordinates are updated during the detection run to reflect the
                // location currently being checked. They range from one element before the
                // heightmap starts to one element after the heightmap ends. Positions
                // outside of the heightmap get a special treatment because this heightmap
                // class sees heightmaps as being clamped at the ends 
                int quadX = Math.Min(Math.Max((int)rayStart.X, -1), heightField.width);
                int quadZ = Math.Min(Math.Max((int)rayStart.Z, -1), heightField.height);

                // Follow the ray until it hit something or advances to a place where the
                // final outcome can be clearly determined
                while (true) {
                    bool leftOutside = (quadX < 0);
                    bool rightOutside = (quadX >= heightField.width - 1);
                    bool topOutside = (quadZ < 0);
                    bool bottomOutside = (quadZ >= heightField.height - 1);

                    if (leftOutside) {
                        if (topOutside) { // Left + top outside
                            #region Contact finder for the upper left heightmap corner
                            float contactTimeIndex;
                            if (rayDirection.Y < 0.0f)
                                contactTimeIndex = (heightField[0, 0] - rayStart.Y) / rayDirection.Y;
                            else
                                contactTimeIndex = float.MaxValue;

                            float leftCrossingTimeIndex;
                            if (rayDirection.X > 0.0f)
                                leftCrossingTimeIndex = -rayStart.X / rayDirection.X;
                            else
                                leftCrossingTimeIndex = float.MaxValue;

                            float topCrossingTimeIndex;
                            if (rayDirection.Z > 0.0f)
                                topCrossingTimeIndex = -rayStart.Z / rayDirection.Z;
                            else
                                topCrossingTimeIndex = float.MaxValue;

                            if (leftCrossingTimeIndex < topCrossingTimeIndex) {
                                if (contactTimeIndex < leftCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (leftCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadX += 1;
                                }
                            } else {
                                if (contactTimeIndex < topCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (topCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadZ += 1;
                                }
                            }
                            #endregion // Contact finder for the upper left heightmap corner
                        } else if (bottomOutside) { // Left + bottom outside
                            #region Contact finder for the lower left heightmap corner
                            float contactTimeIndex;
                            if (rayDirection.Y < 0.0f)
                                contactTimeIndex = (heightField[0, heightField.height - 1] - rayStart.Y) / rayDirection.Y;
                            else
                                contactTimeIndex = float.MaxValue;

                            float leftCrossingTimeIndex;
                            if (rayDirection.X > 0.0f)
                                leftCrossingTimeIndex = -rayStart.X / rayDirection.X;
                            else
                                leftCrossingTimeIndex = float.MaxValue;

                            float bottomCrossingTimeIndex;
                            if (rayDirection.Z < 0.0f)
                                bottomCrossingTimeIndex =
                (((float)(heightField.height - 1)) - rayStart.Z) / rayDirection.Z;
                            else
                                bottomCrossingTimeIndex = float.MaxValue;

                            if (leftCrossingTimeIndex < bottomCrossingTimeIndex) {
                                if (contactTimeIndex < leftCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (leftCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadX += 1;
                                }
                            } else {
                                if (contactTimeIndex < bottomCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (bottomCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadZ -= 1;
                                }
                            }
                            #endregion // Contact finder for the lower left heightmap corner
                        } else { // Left outside
                            #region Contact finder for splices left of the heightmap (X < 0)
                            float heightmapEntryTime;
                            if (rayDirection.X > 0.0f)
                                heightmapEntryTime = -rayStart.X / rayDirection.X;
                            else
                                heightmapEntryTime = float.MaxValue;

                            while (true) {
                                LineContacts contacts = findLeftSpliceContacts(
                                  rayStart, rayDirection, quadZ, heightField
                                );
                                if (!float.IsNaN(contacts.EntryTime)) {
                                    return contacts;
                                }

                                float crossingTimeIndex;
                                if (rayDirection.Z < 0.0f) {
                                    crossingTimeIndex = ((float)quadZ - rayStart.Z) / rayDirection.Z;
                                } else if (rayDirection.Z > 0.0f) {
                                    crossingTimeIndex = ((float)(quadZ + 1) - rayStart.Z) / rayDirection.Z;
                                } else {
                                    crossingTimeIndex = float.MaxValue;
                                    if (heightmapEntryTime == float.MaxValue) {
                                        return new LineContacts(float.NaN, float.NaN);
                                    }
                                }

                                // Will the heightmap be entered before we reach the next splice?
                                if (heightmapEntryTime < crossingTimeIndex) {
                                    quadX += 1;
                                    break;
                                } else {
                                    quadZ += Math.Sign(rayDirection.Z);
                                    if ((quadZ < 0) || (quadZ >= (heightField.height - 1)))
                                        break;
                                }
                            }
                            #endregion // Contact finder for splices left of the heightmap (X < 0)
                        }
                    } else if (rightOutside) {
                        if (topOutside) { // Right + top outside
                            #region Contact finder for the upper right heightmap corner
                            float contactTimeIndex;
                            if (rayDirection.Y < 0.0f)
                                contactTimeIndex = (heightField[heightField.width - 1, 0] - rayStart.Y) / rayDirection.Y;
                            else
                                contactTimeIndex = float.MaxValue;

                            float rightCrossingTimeIndex;
                            if (rayDirection.X < 0.0f)
                                rightCrossingTimeIndex = (((float)heightField.width - 1) - rayStart.X) / rayDirection.X;
                            else
                                rightCrossingTimeIndex = float.MaxValue;

                            float topCrossingTimeIndex;
                            if (rayDirection.Z > 0.0f)
                                topCrossingTimeIndex = -rayStart.Z / rayDirection.Z;
                            else
                                topCrossingTimeIndex = float.MaxValue;

                            if (rightCrossingTimeIndex < topCrossingTimeIndex) {
                                if (contactTimeIndex < rightCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (rightCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadX -= 1;
                                }
                            } else {
                                if (contactTimeIndex < topCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (topCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadZ += 1;
                                }
                            }
                            #endregion // Contact finder for the upper right heightmap corner
                        } else if (bottomOutside) { // Right + bottom outside
                            #region Contact finder for the lower right heightmap corner
                            float contactTimeIndex;
                            if (rayDirection.Y < 0.0f)
                                contactTimeIndex = (heightField[heightField.width - 1, heightField.height - 1] - rayStart.Y) / rayDirection.Y;
                            else
                                contactTimeIndex = float.MaxValue;

                            float rightCrossingTimeIndex;
                            if (rayDirection.X < 0.0f)
                                rightCrossingTimeIndex = (((float)heightField.width - 1) - rayStart.X) / rayDirection.X;
                            else
                                rightCrossingTimeIndex = float.MaxValue;

                            float bottomCrossingTimeIndex;
                            if (rayDirection.Z < 0.0f)
                                bottomCrossingTimeIndex = (((float)heightField.height - 1) - rayStart.Z) / rayDirection.Z;
                            else
                                bottomCrossingTimeIndex = float.MaxValue;

                            if (rightCrossingTimeIndex < bottomCrossingTimeIndex) {
                                if (contactTimeIndex < rightCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (rightCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadX -= 1;
                                }
                            } else {
                                if (contactTimeIndex < bottomCrossingTimeIndex) {
                                    return new LineContacts(contactTimeIndex, float.PositiveInfinity);
                                } else if (bottomCrossingTimeIndex == float.MaxValue) {
                                    return new LineContacts(float.NaN, float.NaN);
                                } else {
                                    quadZ -= 1;
                                }
                            }
                            #endregion // Contact finder for the lower right heightmap corner
                        } else { // Right outside
                            #region Contact finder for splices right of the heightmap (X > width)
                            float heightmapEntryTime;
                            if (rayDirection.X < 0.0f)
                                heightmapEntryTime = ((float)(heightField.width - 1) - rayStart.X) / rayDirection.X;
                            else
                                heightmapEntryTime = float.MaxValue;

                            while (true) {
                                LineContacts contacts = findRightSpliceContacts(
                                  rayStart, rayDirection, quadZ, heightField
                                );
                                if (!float.IsNaN(contacts.EntryTime)) {
                                    return contacts;
                                }

                                float crossingTimeIndex;
                                if (rayDirection.Z < 0.0f) {
                                    crossingTimeIndex = ((float)quadZ - rayStart.Z) / rayDirection.Z;
                                } else if (rayDirection.Z > 0.0f) {
                                    crossingTimeIndex = ((float)(quadZ + 1) - rayStart.Z) / rayDirection.Z;
                                } else {
                                    crossingTimeIndex = float.MaxValue;
                                    if (heightmapEntryTime == float.MaxValue) {
                                        return new LineContacts(float.NaN, float.NaN);
                                    }
                                }

                                // Will the heightmap be entered before we reach the next splice?
                                if (heightmapEntryTime < crossingTimeIndex) {
                                    quadX -= 1;
                                    break;
                                } else {
                                    quadZ += Math.Sign(rayDirection.Z);
                                    if ((quadZ < 0) || (quadZ >= (heightField.height - 1)))
                                        break;
                                }
                            }
                            #endregion // Contact finder for splices right of the heightmap (X > width)
                        }
                    } else {
                        if (topOutside) { // Top outside
                            #region Contact finder for splices behind the heightmap (Y < 0)
                            float heightmapEntryTime;
                            if (rayDirection.Z > 0.0f)
                                heightmapEntryTime = -rayStart.Z / rayDirection.Z;
                            else
                                heightmapEntryTime = float.MaxValue;

                            while (true) {
                                LineContacts contacts = findTopSpliceContacts(
                                  rayStart, rayDirection, quadX, heightField
                                );
                                if (!float.IsNaN(contacts.EntryTime)) {
                                    return contacts;
                                }

                                float crossingTimeIndex;
                                if (rayDirection.X < 0.0f) {
                                    crossingTimeIndex = ((float)quadX - rayStart.X) / rayDirection.X;
                                } else if (rayDirection.X > 0.0f) {
                                    crossingTimeIndex = ((float)(quadX + 1) - rayStart.X) / rayDirection.X;
                                } else {
                                    crossingTimeIndex = float.MaxValue;
                                    if (heightmapEntryTime == float.MaxValue) {
                                        return new LineContacts(float.NaN, float.NaN);
                                    }
                                }

                                // Will the heightmap be entered before we reach the next splice?
                                if (heightmapEntryTime < crossingTimeIndex) {
                                    quadZ += 1;
                                    break;
                                } else {
                                    quadX += Math.Sign(rayDirection.X);
                                    if ((quadX < 0) || (quadX >= (heightField.width - 1)))
                                        break;
                                }
                            }
                            #endregion // Contact finder for splices behind the heightmap (Y < 0)
                        } else if (bottomOutside) { // Bottom outside
                            #region Contact finder for splices in front of the heightmap (Y > length)
                            float heightmapEntryTime;
                            if (rayDirection.Z < 0.0f)
                                heightmapEntryTime = ((float)heightField.height - rayStart.Z) / rayDirection.Z;
                            else
                                heightmapEntryTime = float.MaxValue;

                            while (true) {
                                LineContacts contacts = findBottomSpliceContacts(
                                  rayStart, rayDirection, quadX, heightField
                                );
                                if (!float.IsNaN(contacts.EntryTime)) {
                                    return contacts;
                                }

                                float crossingTimeIndex;
                                if (rayDirection.X < 0.0f) {
                                    crossingTimeIndex = ((float)quadX - rayStart.X) / rayDirection.X;
                                } else if (rayDirection.X > 0.0f) {
                                    crossingTimeIndex = ((float)(quadX + 1) - rayStart.X) / rayDirection.X;
                                } else {
                                    crossingTimeIndex = float.MaxValue;
                                    if (heightmapEntryTime == float.MaxValue) {
                                        return new LineContacts(float.NaN, float.NaN);
                                    }
                                }

                                // Will the heightmap be entered before we reach the next splice?
                                if (heightmapEntryTime < crossingTimeIndex) {
                                    quadZ -= 1;
                                    break;
                                } else {
                                    quadX += Math.Sign(rayDirection.X);
                                    if ((quadX < 0) || (quadX >= (heightField.width - 1)))
                                        break;
                                }
                            }
                            #endregion // Contact finder for splices in front of the heightmap (Y > length)
                        } else { // Inside!
                            #region Contact finder for inside region of heightmap
                            while (true) {
                                LineContacts contacts = findQuadContacts(
                                  rayStart, rayDirection, quadX, quadZ, heightField
                                );
                                if (!float.IsNaN(contacts.EntryTime)) {
                                    return contacts;
                                }

                                float crossingTimeIndexX, crossingTimeIndexZ;

                                if (rayDirection.X < 0.0f) {
                                    crossingTimeIndexX = ((float)quadX - rayStart.X) / rayDirection.X;
                                } else if (rayDirection.X > 0.0f) {
                                    crossingTimeIndexX = ((float)(quadX + 1) - rayStart.X) / rayDirection.X;
                                } else {
                                    crossingTimeIndexX = float.MaxValue;
                                }

                                if (rayDirection.Z < 0.0f) {
                                    crossingTimeIndexZ = ((float)quadZ - rayStart.Z) / rayDirection.Z;
                                } else if (rayDirection.Z > 0.0f) {
                                    crossingTimeIndexZ = ((float)(quadZ + 1) - rayStart.Z) / rayDirection.Z;
                                } else {
                                    crossingTimeIndexZ = float.MaxValue;
                                }

                                if (crossingTimeIndexX < crossingTimeIndexZ) {
                                    if (heightCrossingTime != null)
                                        if (heightCrossingTime[0] < crossingTimeIndexX)
                                            return new LineContacts(float.NaN, float.NaN);

                                    quadX += Math.Sign(rayDirection.X);

                                    if ((quadX < 0) || (quadX >= (heightField.width - 1)))
                                        break;

                                } else {
                                    if (heightCrossingTime != null)
                                        if (heightCrossingTime[0] < crossingTimeIndexZ)
                                            return new LineContacts(float.NaN, float.NaN);

                                    quadZ += Math.Sign(rayDirection.Z);

                                    if ((quadZ < 0) || (quadZ >= (heightField.height - 1)))
                                        break;
                                }
                            }
                            #endregion // Contact finder for inside region of heightmap
                        }
                    }
                }
            }

        }

        // What a splice is:
        //
        // As far as collision detection is concerned, the borders of the height map are
        // assumed to extend towards infinity. This produces two kinds of border cells:
        // The corners will essentially become limited planes with two sides extending
        // two infinity. The other border cells will have only one side that extends to
        // infinity.
        //
        // The latter kind of cells have been termed "splices" for this project.
        //

        /// <summary>Determines the time index at which a ray will hit a top splice</summary>
        /// <param name="rayStart">Starting position of the ray</param>
        /// <param name="rayDirection">Direction the ray extends into</param>
        /// <param name="x">Grid location of the splice to test against, X coordinate</param>
        /// <param name="heightField">Height field to collide against</param>
        /// <returns>Time index when the ray hits the splice, or null if it doesn't</returns>
        private static LineContacts findTopSpliceContacts(
          Vector3 rayStart, Vector3 rayDirection, int x, TerrainInfo heightField
        ) {
            Vector3 normal = Vector3.Normalize(
              new Vector3(heightField[x, 0] - heightField[x + 1, 0], 1.0f, 0.0f)
            );
            Vector3 offset = new Vector3((float)x, heightField[x, 0], 0.0f);

            LineContacts contacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, offset, normal
            );
            if (!float.IsNaN(contacts.EntryTime)) {
                bool isOutsideOfSplice =
          ((rayStart.Z + rayDirection.Z * contacts.EntryTime) > 0.0f) ||
          (x != (int)(rayStart.X + rayDirection.X * contacts.EntryTime));

                if (isOutsideOfSplice) {
                    contacts.EntryTime = float.NaN;
                    contacts.ExitTime = float.NaN;
                }
            }

            return contacts;
        }

        /// <summary>Determines the time index at which a ray will hit a bottom splice</summary>
        /// <param name="rayStart">Starting position of the ray</param>
        /// <param name="rayDirection">Direction the ray extends into</param>
        /// <param name="x">Grid location of the splice to test against, X coordinate</param>
        /// <param name="heightField">Height field to collide against</param>
        /// <returns>Time index when the ray hits the splice, or null if it doesn't</returns>
        private static LineContacts findBottomSpliceContacts(
          Vector3 rayStart, Vector3 rayDirection, int x, TerrainInfo heightField
        ) {
            Vector3 normal = Vector3.Normalize(
              new Vector3(
                heightField[x, heightField.height - 1] - heightField[x + 1, heightField.height - 1],
                1.0f,
                0.0f
              )
            );
            Vector3 offset = new Vector3((float)x, heightField[x, heightField.height - 1], 0.0f);

            LineContacts contacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, offset, normal
            );
            if (!float.IsNaN(contacts.EntryTime)) {
                float mapLength = (float)(heightField.height - 1);
                bool isOutsideOfSplice =
          ((rayStart.Z + rayDirection.Z * contacts.EntryTime) < mapLength) ||
          (x != (int)(rayStart.X + rayDirection.X * contacts.EntryTime));

                if (isOutsideOfSplice) {
                    contacts.EntryTime = float.NaN;
                    contacts.ExitTime = float.NaN;
                }
            }

            return contacts;
        }

        /// <summary>Determines the time index at which a ray will hit a left splice</summary>
        /// <param name="rayStart">Starting position of the ray</param>
        /// <param name="rayDirection">Direction the ray extends into</param>
        /// <param name="y">Grid location of the splice to test against, Y coordinate</param>
        /// <param name="heightField">Height field to collide against</param>
        /// <returns>Time index when the ray hits the splice, or null if it doesn't</returns>
        private static LineContacts findLeftSpliceContacts(
          Vector3 rayStart, Vector3 rayDirection, int y, TerrainInfo heightField
        ) {
            Vector3 normal = Vector3.Normalize(
              new Vector3(0.0f, 1.0f, heightField[0, y] - heightField[0, y + 1])
            );
            Vector3 offset = new Vector3(0.0f, heightField[0, y], (float)y);

            LineContacts contacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, offset, normal
            );
            if (!float.IsNaN(contacts.EntryTime)) {
                bool isOutsideOfSplice =
          ((rayStart.X + rayDirection.X * contacts.EntryTime) > 0.0f) ||
          (y != (int)(rayStart.Z + rayDirection.Z * contacts.EntryTime));

                if (isOutsideOfSplice) {
                    contacts.EntryTime = float.NaN;
                    contacts.ExitTime = float.NaN;
                }
            }

            return contacts;
        }

        /// <summary>Determines the time index at which a ray will hit a right splice</summary>
        /// <param name="rayStart">Starting position of the ray</param>
        /// <param name="rayDirection">Direction the ray extends into</param>
        /// <param name="y">Grid location of the splice to test against, Y coordinate</param>
        /// <param name="heightField">Height field to collide against</param>
        /// <returns>Time index when the ray hits the splice, or null if it doesn't</returns>
        private static LineContacts findRightSpliceContacts(
          Vector3 rayStart, Vector3 rayDirection, int y, TerrainInfo heightField
        ) {
            Vector3 normal = Vector3.Normalize(
              new Vector3(
                0.0f,
                1.0f,
                heightField[heightField.width - 1, y] - heightField[heightField.width - 1, y + 1]
              )
            );
            Vector3 offset = new Vector3(0.0f, heightField[heightField.width - 1, y], (float)y);

            LineContacts contacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, offset, normal
            );
            if (!float.IsNaN(contacts.EntryTime)) {
                float mapWidth = (float)(heightField.width - 1);
                bool isOutsideOfSplice =
          ((rayStart.X + rayDirection.X * contacts.EntryTime) < mapWidth) ||
          (y != (int)(rayStart.Z + rayDirection.Z * contacts.EntryTime));

                if (isOutsideOfSplice) {
                    contacts.EntryTime = float.NaN;
                    contacts.ExitTime = float.NaN;
                }
            }

            return contacts;
        }



        /// <summary>Determines the time index at which a ray will hit a quad</summary>
        /// <param name="rayStart">Starting position of the ray</param>
        /// <param name="rayDirection">Direction the ray extends into</param>
        /// <param name="x">Grid location of the quad to test against, X coordinate</param>
        /// <param name="y">Grid location of the quad to test against, Y coordinate</param>
        /// <param name="heightField">Height field to collide against</param>
        /// <returns>Time index when the ray hits the quad, or null if it doesn't</returns>
        private static LineContacts findQuadContacts(
          Vector3 rayStart, Vector3 rayDirection, int x, int y, TerrainInfo heightField
        ) {
            Vector3 a, b, c;

            // Build the triangle for the upper left portion of the quad
            a = new Vector3((float)x, heightField[x, y], (float)y);
            b = new Vector3((float)(x + 1), heightField[x + 1, y], (float)y);
            c = new Vector3((float)x, heightField[x, y + 1], (float)(y + 1));

            // Calculate the normal and determine when the plane will be hit by the ray
            Vector3 upperLeftNormal = Vector3.Normalize(Vector3.Cross(a - b, c - b));
            LineContacts upperLeftContacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, a, upperLeftNormal
            );

            // Does the ray intersect with the quad's plane? It might still intersect the
            // plane outside of the quad's region
            if (!float.IsNaN(upperLeftContacts.EntryTime)) {
                float intersectionX =
          rayStart.X + rayDirection.X * upperLeftContacts.EntryTime - (float)x;
                float intersectionY =
          rayStart.Z + rayDirection.Z * upperLeftContacts.EntryTime - (float)y;

                // If the hit is outside of the triangle, discard it
                if (
                  (intersectionX < 0.0f) || (intersectionY < 0.0f) ||
          ((intersectionX + intersectionY) > 1.0f)
                ) {
                    upperLeftContacts.EntryTime = float.NaN;
                    upperLeftContacts.ExitTime = float.NaN;
                }
            }

            // Build the triangle for the upper left portion of the quad
            a = b;
            b = new Vector3((float)(x + 1), heightField[x + 1, y + 1], (float)(y + 1));

            // Calculate the normal and determine when the plane will be hit by the ray
            Vector3 lowerRightNormal = Vector3.Normalize(Vector3.Cross(a - b, c - b));
            LineContacts lowerRightContacts = Ray3Plane3Collider.FindContacts(
              rayStart, rayDirection, a, lowerRightNormal
            );

            // Does the ray intersect with the quad's plane? It might still intersect the
            // plane outside of the quad's region
            if (!float.IsNaN(lowerRightContacts.EntryTime)) {
                float intersectionX =
          rayStart.X + rayDirection.X * lowerRightContacts.EntryTime - (float)x;
                float intersectionY =
          rayStart.Z + rayDirection.Z * lowerRightContacts.EntryTime - (float)y;

                // If the hit is outside of the triangle, we can directly return the
                // contact with the other triangle, be it an actual contact is just null
                if ( (intersectionX > 1.0f) || (intersectionY > 1.0f) || ((intersectionX + intersectionY) < 1.0f)
                ) {
                    return upperLeftContacts;
                }

                // Are both triangles hit by the ray?
                if (!float.IsNaN(upperLeftContacts.EntryTime)) {

                    // Return whichever triangle has been hit first
                    return (upperLeftContacts.EntryTime < lowerRightContacts.EntryTime) ?
            upperLeftContacts : lowerRightContacts;

                } else {

                    // We only have one possible contact, so return it, we don't need to see
                    // whether its an actual contact or just null
                    return lowerRightContacts;

                }

            } else {

                // We only have one possible contact, so return it, we don't need to see
                // whether its an actual contact or just null
                return upperLeftContacts;

            }
        }


    }

    /// <summary>Contains all Ray3 to Plane3 interference detection code</summary>
    public static class Ray3Plane3Collider {

        /// <summary>Determines where a ray will hit a plane, if at all</summary>
        /// <param name="rayStart">Starting point of the ray</param>
        /// <param name="rayDirection">Direction into which the ray extends</param>
        /// <param name="planeOffset">Offset of the plane</param>
        /// <param name="planeNormal">Normal vector of the plane</param>
        /// <returns>The intersection points between the ray and the plane, if any</returns>
        public static LineContacts FindContacts(
          Vector3 rayStart, Vector3 rayDirection,
          Vector3 planeOffset, Vector3 planeNormal
        ) {
            LineContacts contacts = Line3Plane3Collider.FindContacts(
              rayStart, rayDirection, planeOffset, planeNormal
            );

            // If the contact occured before the starting offset of the ray,
            // no collision took place since we're a ray
            if (!float.IsNaN(contacts.EntryTime)) {
                if (contacts.EntryTime < 0.0f) {
                    return LineContacts.None;
                }
            }

            return contacts;
        }

    }

    /// <summary>Contains all Line3 to Plane3 interference detection code</summary>
    public static class Line3Plane3Collider {

        /// <summary>Determines where a line will hit a plane, if at all</summary>
        /// <param name="lineOffset">
        ///   Offset of the line from the coordinate system's center
        /// </param>
        /// <param name="lineDirection">Direction and length of the line</param>
        /// <param name="planeOffset">
        ///   Offset of the plane from the coordinate system's center
        /// </param>
        /// <param name="planeNormal">Normal vector of the plane</param>
        /// <returns>
        ///   The intersection point between the line and the plane, if they touch
        /// </returns>
        public static LineContacts FindContacts(
          Vector3 lineOffset, Vector3 lineDirection,
          Vector3 planeOffset, Vector3 planeNormal
        ) {
            float dot = Vector3.Dot(planeNormal, lineDirection);

            // If the dot product is zero, the line is parallel to the plane and no contact occurs
            if (dot == 0.0) {
                return LineContacts.None;
            } else {
                return new LineContacts(
                  -Vector3.Dot(planeNormal, lineOffset - planeOffset) / dot
                );
            }
        }

    }

    /// <summary>
    ///   Stores the times of first and last contact determined in an intersection test
    /// </summary>
    public struct LineContacts {

        /// <summary>Initializes a new contact point</summary>
        /// <param name="touchTime">Time the contact was made at</param>
        public LineContacts(float touchTime) :
            this(touchTime, touchTime) { }

        /// <summary>Initializes a new contact point</summary>
        /// <param name="entryTime">Time the first contact was made at</param>
        /// <param name="exitTime">Time the last contact has occured</param>
        public LineContacts(float entryTime, float exitTime) {
            this.EntryTime = entryTime;
            this.ExitTime = exitTime;
        }

        /// <summary>Whether a contact is stored in the line contacts instance</summary>
        public bool HasContact {
            get { return !float.IsNaN(EntryTime); }
        }

        /// <summary>
        ///   Determines whether this instance is identical to another instance
        /// </summary>
        /// <param name="otherObject">Other instance of compare against</param>
        /// <returns>True if both instances are identical</returns>
        public override bool Equals(object otherObject) {
            if (!(otherObject is LineContacts)) {
                return false;
            }

            LineContacts other = (LineContacts)otherObject;
            return
              (this.EntryTime == other.EntryTime) &&
        (this.ExitTime == other.ExitTime);
        }

        /// <summary>Returns a hash code for the instance</summary>
        /// <returns>The instance's hash code</returns>
        public override int GetHashCode() {
            return this.EntryTime.GetHashCode() ^ this.ExitTime.GetHashCode();
        }

        /// <summary>A line contacts instance for reporting no contacts</summary>
        public static readonly LineContacts None = new LineContacts(float.NaN, float.NaN);

        /// <summary>Time the first contact was made</summary>
        public float EntryTime;
        /// <summary>Time the last contact occurred</summary>
        public float ExitTime;

    }


}
