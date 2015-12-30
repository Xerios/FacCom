using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RageEngine;
using System.IO.Compression;
using RageEngine.Graphics;
using SharpDX;
using System.Diagnostics.Contracts;
using RageEngine.ContentPipeline;

namespace RageRTS.Map {

    public class RawMap {

        private static byte version = 1;
        private enum Parts
        {
            Info         = 1,
            Environement = 2,
            Terrain      = 3,
            Pathfinding  = 4,
            Objects      = 5,
            Nodes        = 6,
        }

        public string MainPath;
        //[Graphics.Map]
        public string Type { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public byte Max_Players { get; set; }

        //[Game Objects]
        public List<MapNode> Nodes { get; set; }

        //[Lights]
        public float Ambient { get; set; }
        public Vector3 Ambient_Color { get; set; }

        public float Sun_Rotation { get; set; }
        public float Sun_Height { get; set; }
        public Vector3 Sun_Color { get; set; }
 
        //[Terrain]
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        //
        public float[] Data { get; set; }

        //public int Map_Height { get; set; }
        public float Water_Level { get; set; }
        public Vector3 Water_Color { get; set; }
        public float Water_Density { get; set; }
        public string[] Textures { get; set; }
        //[Decals]
        //[Pathfinding]
        public byte[] Accessibilty_Array { get; set; }

        public List<MapModel> Models { get; set; }
        public List<MapDecal> Decals { get; set; }

        public static RawMap New() {
            RawMap file = new RawMap();
            file.Name = "New Map";

            return file;
        }

        public static RawMap Load(string path) {

            string combinedpath = Path.Combine(Resources.Main_Path, "Maps\\"+path+"\\_Map.ragox");

            RawMap file = new RawMap();
            file.MainPath = path;
            using (BinaryReader stream = new BinaryReader(File.Open(combinedpath, FileMode.Open), Encoding.UTF8)) {
                if (stream.ReadByte()!=1) throw new Exception("This file format is not version 1");
                //----------
                Contract.Equals(stream.ReadByte(), (byte)Parts.Info);
                file.Type = ""; stream.ReadString();
                file.Name   = stream.ReadString();
                file.Author = stream.ReadString();
                file.Description = stream.ReadString();
                file.Max_Players = stream.ReadByte();

                //----------
                Contract.Equals(stream.ReadByte(), (byte)Parts.Environement);
                file.Ambient = stream.ReadSingle();
                file.Ambient_Color = new Vector3(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());

                file.Sun_Rotation = stream.ReadSingle();
                file.Sun_Height = stream.ReadSingle();
                file.Sun_Color = new Vector3(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());

                file.Water_Level = stream.ReadSingle();
                file.Water_Density = stream.ReadSingle();
                file.Water_Color = new Vector3(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());

                //----------
                Contract.Equals(stream.ReadByte(), (byte)Parts.Terrain);
                 
                file.Width = stream.ReadUInt16();
                file.Height = stream.ReadUInt16();

                file.Textures = new string[stream.ReadInt32()];
                for (int i = 0; i < file.Textures.Length; i++) file.Textures[i] = stream.ReadString();

                file.Data = new float[stream.ReadInt32()];
                for (int i = 0; i < file.Data.Length; i++) file.Data[i] = stream.ReadSingle();

                //----------
                Contract.Equals(stream.ReadByte(), (byte)Parts.Pathfinding);

                int acclength = stream.ReadInt32(); 
                file.Accessibilty_Array = stream.ReadBytes(acclength);

                //----------
                Contract.Equals(stream.ReadByte(), (byte)Parts.Objects);

                file.Models = new List<MapModel>(stream.ReadInt32());
                for (int i = 0; i < file.Models.Capacity; i++) {
                    MapModel model = new MapModel();
                    model.file = stream.ReadString();
                    model.Position = new Vector3(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());
                    model.Orientation = new Quaternion(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());
                    model.Scale = new Vector3(stream.ReadSingle(),stream.ReadSingle(),stream.ReadSingle());
                    model.instanced = stream.ReadBoolean();
                    file.Models.Add(model);
                }
            }


            return file;
        }

        public static void Save(RawMap file, string path) {
            path = Path.Combine(Resources.Main_Path, "Maps\\"+path+"\\_Map.ragox");

            using (BinaryWriter stream = new BinaryWriter(File.Open(path, FileMode.Create),Encoding.UTF8)){
                stream.Write(version);
                //----------
                stream.Write((byte) Parts.Info);
                stream.Write(file.Type);
                stream.Write(file.Name);
                stream.Write(file.Author);
                stream.Write(file.Description);
                stream.Write(file.Max_Players);
                //----------
                stream.Write((byte) Parts.Environement); 
                stream.Write(file.Ambient);
                stream.Write(file.Ambient_Color.X);
                stream.Write(file.Ambient_Color.Y);
                stream.Write(file.Ambient_Color.Z);

                stream.Write(file.Sun_Rotation);
                stream.Write(file.Sun_Height);
                stream.Write(file.Sun_Color.X);
                stream.Write(file.Sun_Color.Y);
                stream.Write(file.Sun_Color.Z);

                stream.Write(file.Water_Level);
                stream.Write(file.Water_Density);
                stream.Write(file.Water_Color.X);
                stream.Write(file.Water_Color.Y);
                stream.Write(file.Water_Color.Z);

                //----------
                stream.Write((byte) Parts.Terrain); 
                stream.Write(file.Width);
                stream.Write(file.Height);

                stream.Write(file.Textures.Length);
                for (int i = 0; i < file.Textures.Length; i++) stream.Write(file.Textures[i]);

                stream.Write(file.Data.Length);
                for (int i = 0; i < file.Data.Length; i++) stream.Write(file.Data[i]);

                //----------
                stream.Write((byte) Parts.Pathfinding);

                stream.Write(file.Accessibilty_Array.Length);
                stream.Write(file.Accessibilty_Array);

                //----------
                stream.Write((byte) Parts.Objects);

                stream.Write(file.Models.Count);
                for (int i = 0; i < file.Models.Count; i++) {
                    MapModel model = file.Models[i];
                    stream.Write(model.file);

                    stream.Write(model.Position.X);
                    stream.Write(model.Position.Y);
                    stream.Write(model.Position.Z);

                    stream.Write(model.Orientation.X);
                    stream.Write(model.Orientation.Y);
                    stream.Write(model.Orientation.Z);
                    stream.Write(model.Orientation.W);

                    stream.Write(model.Scale.X);
                    stream.Write(model.Scale.Y);
                    stream.Write(model.Scale.Z);

                    stream.Write(model.instanced);
                }
                /*stream.Write(0); // Always 0 decals
                stream.Write(file.Decals.Length);
                for (int i = 0; i < file.Decals.Length; i++){
                    MapDecal model = file.Models[i];
                    stream.Write(model.file);

                    stream.Write(model.Position.X);
                    stream.Write(model.Position.Y);
                    stream.Write(model.Position.Z);

                    stream.Write(model.Orientation.X);
                    stream.Write(model.Orientation.Y);
                    stream.Write(model.Orientation.Z);
                    stream.Write(model.Orientation.W);

                    stream.Write(model.Scale.X);
                    stream.Write(model.Scale.Y);
                    stream.Write(model.Scale.Z);

                    stream.Write(model.foliage);
                }*/

                /*stream.Write(file.Nodes.Count);
                for (int i = 0; i < file.Nodes.Count; i++) {
                    MapNode node = file.Nodes[i];
                    stream.Write(node.type);
                    stream.Write(node.value);
                    stream.Write(node.Position.X);
                    stream.Write(node.Position.Y);
                    stream.Write(node.Scale.X);
                    stream.Write(node.Scale.Y);
                }*/

            }

        }

    }

    public class MapModel {
        public string file;
        public Quaternion Orientation;
        public Vector3 Position;
        public Vector3 Scale;
        public bool instanced = false;

        public MapModel() { }
        public MapModel(string filepath,bool fol) {
            file = filepath;
            instanced = fol;
        }
    }


    public class MapDecal {
        public string file;
        public float Rotation,Scale;
        public Vector3 Position;

        public MapDecal(string filepath) {
            file = filepath;
        }
    }

    public class MapNode
    {
        public string type;
        public string value;
        public Point2D Position;
        public Point2D Scale;

        public MapNode() {
        }
    }

}
