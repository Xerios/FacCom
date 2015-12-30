#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RageEngine;
using RageEngine.Utils;
using System.Drawing;
using RageEngine.Graphics;
using SharpDX;
using RageEngine.Input;
#endregion

namespace RageRTS {
    public class TopDownCamera : Camera {
        public float MinZoom = 20;
        public float ZoomLevel = 30;
        public bool StrategicView = false;

        public bool ChangingCameraAngle = false;
        internal const int CAMERA_ANGLE_CHANGE_SENSITIVITY = 10;

        private float Sensitivity;

        private Point deltaPoint,startPoint;


        private float Radius;
        private float Yaw,interpolatedYaw;
        private float Pitch,interpolatedPitch;
        private Vector3 interpolatedTarget;

        private Matrix currentMatrix = Matrix.Identity;

        private const float Angle_TopDown = GameUtils.PiOver2 - 0.00001f;
        private const float Angle_Side = GameUtils.PiOver2 * 0.6f;

        private bool enableDragging = false;


        public Vector2 Position {
            get { return new Vector2(target.X,target.Z); } 
            set {
                target=interpolatedTarget=new Vector3(value.X, 0, value.Y);
                ForegroundGame.Map.GetHeight(target, ref target.Y);
                interpolatedTarget.Y=target.Y;
            } 
        }
        

        public TopDownCamera() {
            minDepth = 1;
            maxDepth = 1000;
            Sensitivity = 0.04f;

            Yaw = 0;
            Pitch=Angle_TopDown;
            currentMatrix = Matrix.Identity;
            GroundOffset=MinZoom;
        }

        public void UpdatePosition() {
            StrategicView=Radius > 175;

            if (StrategicView) {
                Pitch = Angle_TopDown;
                interpolatedYaw= GameUtils.AngleNormalize(interpolatedYaw);
                Yaw = 0;
            } else { 
                Pitch = Angle_Side; 
            }

            Radius=10+(ZoomLevel*ZoomLevel*ZoomLevel)/150*ForegroundGame.Map.width/5000;

            float camLimits = Radius * 0.4f;

            if (camLimits>ForegroundGame.Map.width-camLimits) 
                target.X=ForegroundGame.Map.width*0.5f;
            else if (target.X>ForegroundGame.Map.width-camLimits) 
                target.X=ForegroundGame.Map.width-camLimits;
            else if (target.X<camLimits) 
                target.X=camLimits;

            if (camLimits>ForegroundGame.Map.height-camLimits) 
                target.Z=ForegroundGame.Map.height*0.5f;
            else if (target.Z>ForegroundGame.Map.height-camLimits) 
                target.Z=ForegroundGame.Map.height-camLimits;
            else if (target.Z<camLimits) 
                target.Z=camLimits;

            ForegroundGame.Map.GetHeight(target, ref target.Y);
            if (target.Y<ForegroundGame.Map.waterLevel) target.Y=ForegroundGame.Map.waterLevel;

        }

        public override void Update() {
            UpdatePosition();

            GroundOffset = GameUtils.Lerp(GroundOffset, Radius, 0.1f);
            interpolatedPitch = GameUtils.Lerp(interpolatedPitch, Pitch, 0.1f);
            interpolatedYaw = GameUtils.AngleLerp(interpolatedYaw, Yaw, 0.1f);
            eye.Y = (float)Math.Sin(interpolatedPitch) * GroundOffset;
            eye.Z = (float)Math.Cos(interpolatedYaw) * (float)Math.Cos(interpolatedPitch) * GroundOffset;
            eye.X = (float)Math.Sin(interpolatedYaw) * (float)Math.Cos(interpolatedPitch) * GroundOffset; 
            Vector3.Lerp(ref interpolatedTarget, ref target,0.1f, out interpolatedTarget);
            eyePosition = eye + interpolatedTarget;
            View = Matrix.LookAtRH(eyePosition, interpolatedTarget, Vector3.UnitY);
            eyeDirection = eyePosition - interpolatedTarget;

            maxDepth = GroundOffset*4;
            //Projection = Matrix.OrthoRH((int)Display.width * GroundOffset/1000,(int)Display.height * GroundOffset/1000, -50, maxDepth);
            Projection = Matrix.PerspectiveFovRH(GameUtils.PiOver4, CameraHelper.AspectRatio(), 1, maxDepth);
            //currentMatrix = Matrix.Lerp(currentMatrix, targetMatrix, 0.1f);
            //currentMatrix = LerpHelper.Slerp(currentMatrix, targetMatrix, 0.1f);
            //finalMatrix = currentMatrix;

            ViewProjection = View * Projection;

            Frustum = new BoundingFrustum(ViewProjection);
        }

        public void Input_Wheel(object sender, MouseInputEventArgs e) {
            ZoomLevel -= e.WheelDelta * Sensitivity;

            if (ZoomLevel < MinZoom) {
                ZoomLevel = MinZoom;
                UpdatePosition();
                return;
            } else if (ZoomLevel > 100) {
                ZoomLevel = 100;
            }

            if (e.WheelDelta > 0) {
                Ray ray = CameraHelper.RayFromScreen(e.X, e.Y);
                Vector3 destination=ForegroundGame.Map.Pick(ray);
                if (destination != -Vector3.UnitZ) {
                    target = Vector3.Lerp(target, destination, 0.25f);
                }
            }
        }


        public void Input_MouseDown(object sender, MouseInputEventArgs e) {
            if (e.IsRightButtonDown || e.IsMiddleButtonDown) {
                enableDragging=true;
                startPoint.X = e.X;
                startPoint.Y = e.Y;
                deltaPoint.X = startPoint.X;
                deltaPoint.Y = startPoint.Y;
            }
        }

        public void Input_MouseUp(object sender, MouseInputEventArgs e) {
            enableDragging=false;
            ChangingCameraAngle=false;
        }

        public void Input_MouseMove(object sender, MouseInputEventArgs e) {
            if (!enableDragging) return;

            var dist = GameUtils.Distance2D(e.X, e.Y, startPoint.X, startPoint.Y); 
            if (dist>=CAMERA_ANGLE_CHANGE_SENSITIVITY) ChangingCameraAngle = true;

            if (e.IsRightButtonDown) {
                float dx = e.X - deltaPoint.X;
                float dy = e.Y - deltaPoint.Y;

                target.Z -= ((float)Math.Cos(Yaw) * dy - (float)Math.Sin(Yaw) * dx) * Radius/200;
                target.X -= ((float)Math.Sin(Yaw) * dy + (float)Math.Cos(Yaw) * dx) * Radius/200;


            } else if (e.IsMiddleButtonDown) {
                float dx = e.X - deltaPoint.X;

                Yaw -= dx * 0.00085f * GameUtils.Pi;
            }
            deltaPoint.X = e.X;
            deltaPoint.Y = e.Y;
        }

    }
}
