#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RageEngine;
using RageEngine.Utils;
using RageEngine.Graphics;
using SharpDX;
#endregion

namespace MapEditor {
    public class EditorCamera : Camera {
        public float minZoom = 10;
        public float ZoomLevel = 30;


        private float Sensitivity;


        private Vector3 eye = Vector3.Zero;

        public float Radius;
        public float Yaw,interpolatedYaw;
        public float Pitch,interpolatedPitch;
        public Vector3 interpolatedTarget;

        private Matrix currentMatrix = Matrix.Identity;

        private const float Angle_TopDown = GameUtils.PiOver2 - 0.00001f;
        private const float Angle_Side = GameUtils.PiOver2 * 0.6f;

        public EditorCamera() {
            Projection = Matrix.PerspectiveFovRH(GameUtils.PiOver4, CameraHelper.AspectRatio(), 1,1000);

            Sensitivity = 0.04f;

            Yaw = 0;
            Pitch = Angle_Side;
            currentMatrix = Matrix.Identity;
            GroundOffset=minZoom;
        }

        public void UpdatePosition() {
            Radius = 10 + (ZoomLevel * ZoomLevel * ZoomLevel) / 150;
            //Yaw= GameUtils.AngleNormalize(Yaw);
            /*if (Radius > 150) {
                //Pitch = Angle_TopDown;
                
                //Yaw = 0;
                //Projection = Matrix.CreatePerspectiveFieldOfView(GameUtils.PiOver4/2, Main.device.Viewport.AspectRatio, 1, 50000);
            } else { 
                //Pitch = Angle_Side; 
                //Projection = Matrix.CreatePerspectiveFieldOfView(GameUtils.PiOver4, Main.device.Viewport.AspectRatio, 1, 50000);
            }*/
            //Projection = Matrix.CreatePerspectiveFieldOfView(GameUtils.PiOver4/2, Display.device.Viewport.AspectRatio, 1, 50000);

           /* float camLimits = InGame.Map_Size / 2 * (Radius / InGame.Map_Size * 5120)/6676;
            if (target.X < camLimits) target.X = camLimits;
            if (target.Z < camLimits) target.Z = camLimits;
            if (target.X > InGame.Map_Dimensions.X - camLimits) target.X = InGame.Map_Dimensions.X - camLimits;
            if (target.Z > InGame.Map_Dimensions.Z - camLimits) target.Z = InGame.Map_Dimensions.Z - camLimits;
            InGame.Scene.Map_Info.GetHeight(target, ref target.Y);
            if (target.Y < InGame.Scene.Map_Info.waterHeight) target.Y = InGame.Scene.Map_Info.waterHeight;*/
            //eye += target;
        }

        public override void Update() {
            GroundOffset = GameUtils.Lerp(GroundOffset, Radius, 0.5f);
            interpolatedPitch = GameUtils.Lerp(interpolatedPitch, Pitch, 0.5f);
            interpolatedYaw = GameUtils.AngleLerp(interpolatedYaw, Yaw, 0.5f);
            eye.Y = (float)Math.Sin(interpolatedPitch) * GroundOffset;
            eye.Z = (float)Math.Cos(interpolatedYaw) * (float)Math.Cos(interpolatedPitch) * GroundOffset;
            eye.X = (float)Math.Sin(interpolatedYaw) * (float)Math.Cos(interpolatedPitch) * GroundOffset;
            interpolatedTarget = target;
            //Vector3.Lerp(ref interpolatedTarget, ref target,0.1f, out interpolatedTarget);
            eyePosition = eye+ interpolatedTarget;
            View = Matrix.LookAtRH(eyePosition, interpolatedTarget, Vector3.UnitY); 
            UpdatePosition();
            Projection = Matrix.PerspectiveFovRH(GameUtils.PiOver4, CameraHelper.AspectRatio(), 1,GroundOffset * (GameUtils.Max(1, GameUtils.PiOver2-interpolatedPitch)*10));

            //currentMatrix = Matrix.Lerp(currentMatrix, targetMatrix, 0.1f);
            //currentMatrix = LerpHelper.Slerp(currentMatrix, targetMatrix, 0.1f);
            //finalMatrix = currentMatrix;

            ViewProjection = View * Projection;

            Frustum = new BoundingFrustum(ViewProjection);
        }



    }
}
