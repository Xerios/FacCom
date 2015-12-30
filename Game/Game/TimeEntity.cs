using RageEngine;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageRTS {
    public class TimeEntity {

        public Vector3[] PositionList = new Vector3[2];
        public Quaternion[] RotationList = new Quaternion[2];

        public Vector3 Position 
        {
            get {
                return Vector3.Lerp(PositionList[0], PositionList[1], BackgroundGame.TimeInterpolate); 
            }
            set {
                PositionList[0] = PositionList[1];
                PositionList[1] = value;
            }
        }
        public Quaternion Rotation {
            get {
                return Quaternion.Lerp(RotationList[0], RotationList[1], BackgroundGame.TimeInterpolate); 
            }
            set {
                RotationList[0] = RotationList[1];
                RotationList[1] = value;
            }
        }
    }
}
