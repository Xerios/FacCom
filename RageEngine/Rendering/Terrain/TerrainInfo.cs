#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using RageEngine.Utils;
using RageEngine.Debug;
using System.Diagnostics;
using SharpDX;
using RageEngine.Graphics;
#endregion

namespace RageEngine.Rendering
{

    public class TerrainInfo {
        public float waterLevel = 40;
        public Vector4 waterColor = new Vector4(0.7f, 40.0f/255, 147.0f/255, 176.0f/255);

        public float[,] altitudeData;
        public Vector3[,] normalData;

        public byte[] accessibilityArray;

        public int width, height;
        public float maxAltitude;

        public Texture TextureMap, ColorMap, Detail, LightMap, TextureArray, Water, WaterBump, Shore, Clouds;

        public TerrainInfo() {

        }

        public void Load() {
            normalData = new Vector3[width, height];
            LightMap = ContentPipeline.Resources.GetEmptyTexture();
        }

        public float this[int x, int z] {
            get { return this.altitudeData[x, z]; }
        }

        public bool IsOnHeightmap(Vector3 position) {
            return (position.X>0 && position.X<width && position.Z>0 && position.Z<height);
        }

        public void GetHeightInacurate(Vector3 position, ref float heightOut) {
            int left, top;
            left = (int)position.X;
            top = (int)position.Z;

            if (left < 0) left = 0; else if (left >= width - 1) left = width - 2;
            if (top < 0) top = 0; else if (top >= height - 1) top = height - 2;

            heightOut = altitudeData[left, top];
        }

        public float GetHeight(Vector2 position) {
            int left, top;
            left = (int)position.X;
            top = (int)position.Y;

            if (left < 0) left = 0; else if (left >= width - 1) left = width - 2;
            if (top < 0) top = 0; else if (top >= height - 1) top = height - 2;

            float xNormalized = (position.X % 1);
            float zNormalized = (position.Y % 1);

            float topHeight = GameUtils.Lerp(altitudeData[left, top], altitudeData[left + 1, top], xNormalized);
            float bottomHeight = GameUtils.Lerp(altitudeData[left, top + 1], altitudeData[left + 1, top + 1], xNormalized);

            return GameUtils.Lerp(topHeight, bottomHeight, zNormalized);
        }

        public void GetHeight(Vector3 position, ref float heightOut, float addHeight = 0) {

            int left, top;
            left = (int)position.X;
            top = (int)position.Z;

            if (left < 0) left = 0; else if (left >= width - 1) left = width - 2;
            if (top < 0) top = 0; else if (top >= height - 1) top = height - 2;

            float xNormalized = (position.X % 1);
            float zNormalized = (position.Z % 1);

            float topHeight = GameUtils.Lerp(altitudeData[left, top], altitudeData[left + 1, top], xNormalized);
            float bottomHeight = GameUtils.Lerp(altitudeData[left, top + 1], altitudeData[left + 1, top + 1], xNormalized);

            heightOut = GameUtils.Lerp(topHeight, bottomHeight, zNormalized) + addHeight;
        }

        public void GetHeightAndNormal(Vector3 position, ref float heightOut, bool getNormal, ref Vector3 normal) {
            GetHeightAndNormal(new Vector2(position.X, position.Z), ref heightOut, getNormal, ref normal);
        }

        public void GetHeightAndNormal(Vector2 position, ref float heightOut, bool getNormal, ref Vector3 normal) {

            // we'll use integer division to figure out where in the "heights" array
            // position is. Remember that integer division always rounds
            // down, so that the result of these divisions is the indices of the "upper
            // left" of the 4 corners of that cell.
            int left, top;
            left = (int)position.X;
            top = (int)position.Y;

            if (left < 0) left = 0; else if (left >= width - 1) left = width - 2;
            if (top < 0) top = 0; else if (top >= height - 1) top = height - 2;

            // next, we'll use modulus to find out how far away we are from the upper
            // left corner of the cell. Mod will give us a value from 0 to terrainScale,
            // which we then divide by terrainScale to normalize 0 to 1.
            float xNormalized = (position.X % 1);
            float zNormalized = (position.Y % 1);

            // Now that we've calculated the indices of the corners of our cell, and
            // where we are in that cell, we'll use bilinear interpolation to calculuate
            // our height. This process is best explained with a diagram, so please see
            // the accompanying doc for more information.
            // First, calculate the heights on the bottom and top edge of our cell by
            // interpolating from the left and right sides.
            float topHeight = GameUtils.Lerp(
                altitudeData[left, top],
                altitudeData[left + 1, top],
                xNormalized);

            float bottomHeight = GameUtils.Lerp(
                altitudeData[left, top + 1],
                altitudeData[left + 1, top + 1],
                xNormalized);

            // next, interpolate between those two values to calculate the height at our
            // position.
            heightOut = GameUtils.Lerp(topHeight, bottomHeight, zNormalized);

            if (!getNormal) return;
            // We'll repeat the same process to calculate the normal.
            Vector3 topNormal = Vector3.Lerp(
                normalData[left, top],
                normalData[left + 1, top],
                xNormalized);

            Vector3 bottomNormal = Vector3.Lerp(
                normalData[left, top + 1],
                normalData[left + 1, top + 1],
                xNormalized);

            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();
        }

        public Vector3 Pick(Ray ray) {
            return Pick(ray, true);
        }

        public Vector3 Pick(Ray ray, bool water) {
            Vector3 currentPoint = ray.Position;

            LineContacts con = TerrainPicker.Ray3HeightFieldCollider.FindContacts(ray.Position, ray.Direction, this);
            if (con.HasContact) {
                currentPoint += ray.Direction * con.EntryTime;
                return currentPoint;
            }
            
            return new Vector3(0,-99999,0);
        }
    }
}

