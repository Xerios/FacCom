using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using RageEngine.RawFiles;

namespace RageEngine.Graphics {

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2D
    {
        private Vector4 Position;
        private int Color;

        public Vertex2D(Vector2 position, Vector2 texCoord, Color4 color) {
            this.Position = new Vector4(position.X,position.Y,texCoord.X,texCoord.Y);
            this.Color = color.ToRgba();
        }

        public Vertex2D(Vector4 position, Vector2 texCoord, Color4 color) {
            this.Position = new Vector4(position.X, position.Y, texCoord.X, texCoord.Y);
            this.Color = color.ToRgba();
        }

        public static InputElement[] Elements =
        {
            new InputElement("POSITION" , 0, Format.R32G32B32A32_Float , InputElement.AppendAligned, 0),
            new InputElement("COLOR"    , 0, Format.R8G8B8A8_UNorm     , InputElement.AppendAligned, 0), 
        };

        public static int SizeInBytes = 4 * (4 + 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexColor {
        public Vector3 Position;
        public int Color;

        public VertexColor(Vector3 position, Color4 color) {
            this.Position=position;
            this.Color=color.ToRgba();
        }

        static public InputElement[] Elements=
        {
            new InputElement("POSITION" , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("COLOR"    , 0, Format.R8G8B8A8_UNorm  , InputElement.AppendAligned, 0), 
        };
        public static int SizeInBytes=(3+1)*4;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VertexTexture {
        public Vector3 Position;
        public Vector2 TexCoord;

        public VertexTexture(Vector3 position, Vector2 uv) {
            this.Position=position;
            this.TexCoord=uv;
        }

        static public InputElement[] Elements=
        {
            new InputElement("POSITION", 0, Format.R32G32B32_Float  , InputElement.AppendAligned, 0), 
            new InputElement("TEXCOORD", 0, Format.R32G32_Float     , InputElement.AppendAligned, 0),
        };
        public static int SizeInBytes=(3+2)*4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexTextureColor {
        private Vector3 Position;
        private Vector2 TexCoord;
        private int Color;

        public VertexTextureColor(Vector3 position, Vector2 texCoord, Color4 color) {
            this.Position=position;
            this.Color=color.ToRgba();
            this.TexCoord=texCoord;
        }

        public static InputElement[] Elements=
        {
            new InputElement("POSITION" , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("TEXCOORD" , 0, Format.R32G32_Float    , InputElement.AppendAligned, 0),
            new InputElement("COLOR"    , 0, Format.R8G8B8A8_UNorm  , InputElement.AppendAligned, 0), 
        };

        public static int SizeInBytes=sizeof(float)*(3+2+1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2DColor {
        private Vector4 Position;
        private int Color;

        public Vertex2DColor(Vector3 position, Vector2 texCoord, Color4 color) {
            this.Position=new Vector4(position.X, position.Y, texCoord.X, texCoord.Y);
            this.Color=color.ToRgba();
        }

        public static InputElement[] Elements=
        {
            new InputElement("POSITION" , 0, Format.R32G32B32A32_Float , InputElement.AppendAligned, 0), 
            new InputElement("COLOR"    , 0, Format.R8G8B8A8_UNorm  , InputElement.AppendAligned, 0), 
        };

        public static int SizeInBytes=sizeof(float)*(4+1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexNormalTexture {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;

        public VertexNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoord) {
            this.Position=position;
            this.Normal=normal;
            this.TexCoord=texCoord;
        }

        public static InputElement[] Elements=
        {
            new InputElement("POSITION" , 0, Format.R32G32B32_Float  , InputElement.AppendAligned, 0), 
            new InputElement("NORMAL"   , 0, Format.R32G32B32_Float  , InputElement.AppendAligned, 0), 
            new InputElement("TEXCOORD" , 0, Format.R32G32_Float     , InputElement.AppendAligned, 0),
        };
        public static int SizeInBytes=sizeof(float)*(3+3+2);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexNormalTextureTangent {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TexCoord;

        public static InputElement[] Elements=
        {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0), 
            new InputElement("NORMAL"  , 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0), 
            new InputElement("TANGENT" , 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float   , InputElement.AppendAligned, 0),
        };

        public static int SizeInBytes=(3+3+3+2)*sizeof(float);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VertexNormalTextureTangentSkinned {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TexCoord;

        public float BlendIndex1, BlendIndex2, BlendIndex3, BlendIndex4;
        public float BlendWeight1, BlendWeight2, BlendWeight3, BlendWeight4;

        public VertexNormalTextureTangentSkinned(RawVertex v) {
            Position=v.position;
            Normal=v.normal;
            TexCoord=v.texCoord;
            Tangent=v.tangent;
            BlendIndex1=v.blendIndex1;
            BlendIndex2=v.blendIndex2;
            BlendIndex3=v.blendIndex3;
            BlendIndex4=v.blendIndex4;
            BlendWeight1=v.blendWeight1;
            BlendWeight2=v.blendWeight2;
            BlendWeight3=v.blendWeight3;
            BlendWeight4=v.blendWeight4;
        }

        public VertexNormalTextureTangentSkinned(Vector3 setPos, Vector3 setNormal, Vector2 setTexCoord, Vector3 setTangent, float blendIndex1, float blendIndex2, float blendIndex3, float blendIndex4, float blendWeight1, float blendWeight2, float blendWeight3, float blendWeight4) {
            Position=setPos;
            Normal=setNormal;
            TexCoord=setTexCoord;
            Tangent=setTangent;
            BlendIndex1=blendIndex1;
            BlendIndex2=blendIndex2;
            BlendIndex3=blendIndex3;
            BlendIndex4=blendIndex4;
            BlendWeight1=blendWeight1;
            BlendWeight2=blendWeight2;
            BlendWeight3=blendWeight3;
            BlendWeight4=blendWeight4;
        }

        public static InputElement[] Elements=
        {
            new InputElement("POSITION"     , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("NORMAL"       , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("TANGENT"      , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0),            
            new InputElement("TEXCOORD"     , 0, Format.R32G32_Float    , InputElement.AppendAligned, 0),
            new InputElement("BLENDINDICE"  , 0, Format.R32G32B32A32_Float    , InputElement.AppendAligned, 0),
            new InputElement("BLENDWEIGHT"  , 0, Format.R32G32B32A32_Float    , InputElement.AppendAligned, 0),
        };

        public static InputElement[] ElementsInstanced=
        {
            new InputElement("POSITION"     , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("NORMAL"       , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0), 
            new InputElement("TANGENT"      , 0, Format.R32G32B32_Float , InputElement.AppendAligned, 0),            
            new InputElement("TEXCOORD"     , 0, Format.R32G32_Float    , InputElement.AppendAligned, 0),
            new InputElement("BLENDINDICE"  , 0, Format.R32G32B32A32_Float   , InputElement.AppendAligned, 0),
            new InputElement("BLENDWEIGHT"  , 0, Format.R32G32B32A32_Float   , InputElement.AppendAligned, 0),
            new InputElement("TRANSFORM"   , 0, Format.R32G32B32A32_Float    , 0, 1, InputClassification.PerInstanceData,1),
            new InputElement("TRANSFORM"   , 1, Format.R32G32B32A32_Float    ,16, 1, InputClassification.PerInstanceData,1),
            new InputElement("TRANSFORM"   , 2, Format.R32G32B32A32_Float    ,32, 1, InputClassification.PerInstanceData,1),
            new InputElement("TRANSFORM"   , 3, Format.R32G32B32A32_Float    ,48, 1, InputClassification.PerInstanceData,1),
        };

        public static int SizeInBytes=(3+3+3+2+4+4)*sizeof(float);

    }
}
