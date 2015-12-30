#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RageEngine;
using RageEngine.Utils;
using SharpDX;
#endregion

namespace RageEngine.Graphics {
    public class Camera {
        public Vector3 eyePosition,eyeDirection;

        public Vector3 target;
        public Vector3 eye = Vector3.Zero;

        private Matrix currentMatrix = Matrix.Identity;

        public Matrix ViewProjection;
        public Matrix View;
        public Matrix Projection;

        public BoundingFrustum Frustum;

        public float minDepth,maxDepth;

        public float GroundOffset;

        public Camera() {
            Projection = Matrix.PerspectiveFovLH(GameUtils.PiOver4, CameraHelper.AspectRatio(), 1, 1000);
            minDepth = 1;
            maxDepth = 1000;
            currentMatrix = Matrix.Identity;

            ViewProjection = View * Projection;
        }

        public virtual void Update() { }

    }
}
