using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.RawFiles {

    public class RawVertex {

        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;
        public Vector3 tangent;
        public int blendIndex1, blendIndex2, blendIndex3, blendIndex4;
        public float blendWeight1, blendWeight2, blendWeight3, blendWeight4;

        public RawVertex() { }

        public RawVertex(Vector3 setPos, Vector2 setTexCoord, Vector3 setNormal) {
            position=setPos;
            texCoord=setTexCoord;
            normal=setNormal;
            tangent=Vector3.Zero;
            blendIndex1=0;
            blendIndex2=0;
            blendIndex3=0;
            blendIndex4=0;
            blendWeight1=0;
            blendWeight2=0;
            blendWeight3=0;
            blendWeight4=0;
        }

        public RawVertex(Vector3 setPos, Vector2 setTexCoord, Vector3 setNormal, Vector3 setTangent) {
            position=setPos;
            texCoord=setTexCoord;
            normal=setNormal;
            tangent=setTangent;
            blendIndex1=0;
            blendIndex2=0;
            blendIndex3=0;
            blendIndex4=0;
            blendWeight1=0;
            blendWeight2=0;
            blendWeight3=0;
            blendWeight4=0;
        }

        public RawVertex Clone() {
            RawVertex ret=new RawVertex(position, texCoord, normal, tangent);
            ret.blendIndex1=blendIndex1;
            ret.blendIndex2=blendIndex2;
            ret.blendIndex3=blendIndex3;
            ret.blendIndex4=blendIndex4;
            ret.blendWeight1=blendWeight1;
            ret.blendWeight2=blendWeight2;
            ret.blendWeight3=blendWeight3;
            ret.blendWeight4=blendWeight4;
            return ret;
        }


        /// <summary>
        /// Returns true if two vertices are nearly equal. For example the
        /// tangent or normal data does not have to match 100%.
        /// Used to optimize vertex buffers and to generate indices.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns>Bool</returns>
        public static bool NearlyEquals(RawVertex a, RawVertex b) {
            //SkinningWithColladaModelsInXna.Helpers.Log.Write("Compare a=" + a.pos + ", " + a.uv + ", "+a.normal+
            //  " with b=" + b.pos + ", " + b.uv+ ", "+b.normal);
            //return false;
            // Position has to match, else it is just different vertex
            return a.position==b.position&&
                // Ignore blend indices and blend weights, they are the same
                // anyway, because they are calculated from the bone distances.
                Math.Abs(a.texCoord.X-b.texCoord.X)<0.001f&&
                Math.Abs(a.texCoord.Y-b.texCoord.Y)<0.001f&&
                // Normals and tangents do not have to be very close, we can't see
                // any difference between small variations here, but by optimizing
                // similar vertices we can improve the overall rendering performance.
                (a.normal-b.normal).Length()<0.1f&&
                (a.tangent-b.tangent).Length()<0.1f;
        }
    }
}
