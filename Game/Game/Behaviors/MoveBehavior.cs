using RageEngine;
using RageEngine.Graphics;
using RageEngine.Utils;
using RageRTS.Map;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageRTS.Behaviors {
    public class MoveBehavior : Behavior {
        private InteractiveObject parent;

        public bool active = false, reached = false, unreachable = false;
        FVector2 start,dest;
        FlowFieldPath flowfield;

        public MoveBehavior(InteractiveObject parent) {
            this.parent = parent;
            this.parent.Behaviors.Add(this);
        }

        public void Move(Order_Move order) {
            FVector2 cur2D = BackgroundGame.MapSpace.WorldToSpace(parent.Position);
            int id = cur2D.X.ToInt() + cur2D.Y.ToInt() * BackgroundGame.MapSpace.Width;
            byte direction = order.flowfield==null ? (byte)0 : ((id >= 0 && id < order.flowfield.Length) ? order.flowfield.Get(id) : (byte)0);

            if (direction!=0 && direction!=1) {
                active = true;
                reached = false;
                start = parent.Position;
                dest = order.destination;
                flowfield = order.flowfield;
                parent.Moving = true;
                unreachable=false;
            } else {
                active = false;
                reached = false;
                unreachable=true;
                start = parent.Position;
                dest = parent.Position;
                parent.Moving = false;
            }
        }

        public void Dispose() {}

        public List<InteractiveObject> unitsNearBy = new List<InteractiveObject>(100);


        int Speed = 1;
        int Radius = 1;
        int NeighborCheckRadius = 2;
        bool radiusNear,radiusTouch;

        public void Update() {

            parent.Velocity.X = fint.zero;
            parent.Velocity.Y = fint.zero;

            radiusNear=false;
            radiusTouch=false;
            //------------------------------------------------------------------------------------
            unitsNearBy.Clear();
            parent.proximityToken.FindNeighbors(new Point3D(parent.Position.X.ToInt(), 0, parent.Position.Y.ToInt()), NeighborCheckRadius, unitsNearBy);

            DisperseUpdate();


            var dist = (parent.Position - dest).sqrMagnitude;
            int reachDist = 1;
            if (parent.Orders.Count!=0 && parent.Orders.Peek().type==OrderType.MOVE) reachDist = 10;

            if (active) {
                fint xxx = fint.zero, yyy = fint.zero;
                //------------------------------------------------------------------------------------

                FVector2 cur2D = BackgroundGame.MapSpace.WorldToSpace(parent.Position);
                int id = cur2D.X.ToInt() + cur2D.Y.ToInt() * BackgroundGame.MapSpace.Width;
                byte direction = flowfield==null ? (byte)255 : ((id >= 0 && id < flowfield.Length) ? flowfield.Get(id) : (byte)0);

                //if (direction == 255 || dist<(fint)((reachDist+1) * (reachDist+1))) {
                    parent.Rotation = FVector2.Angle(dest, parent.Position);
                /*} else {
                    if (direction != 0 && direction != 1) {
                        if (direction >= 100) direction -= 100;

                        int x = -MapSpace.Direction[direction - 2, 0];
                        int y = -MapSpace.Direction[direction - 2, 1];
                        parent.Rotation =  FMath.Atan2((fint)y, (fint)x);
                    }
                }*/



                xxx = FMath.Cos(parent.Rotation) * (fint)Speed;
                yyy = FMath.Sin(parent.Rotation) * (fint)Speed;

                //------------------------------------------------------------------------------------

                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X + parent.Velocity.X + xxx, parent.Position.Y + parent.Velocity.Y))
                    xxx = fint.zero;
                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X+parent.Velocity.X, parent.Position.Y+parent.Velocity.Y + yyy))
                    yyy = fint.zero;
                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X+parent.Velocity.X+xxx, parent.Position.Y+parent.Velocity.Y+yyy)) {
                    xxx = fint.zero;
                    yyy = fint.zero;
                }

                parent.Velocity.X += xxx;
                parent.Velocity.Y += yyy;
            }


            parent.Velocity.X = GameUtils.Clamp(parent.Velocity.X, -Speed, Speed);
            parent.Velocity.Y = GameUtils.Clamp(parent.Velocity.Y, -Speed, Speed);

            parent.Position += parent.Velocity;
            //if (!active) 

            if (active) {
                dist = (parent.Position - dest).sqrMagnitude;
                if (dist<= (fint)(reachDist * reachDist)) {
                    //parent.Position = dest;
                    active=false;
                    reached=true;
                    parent.Moving = false;
                }
            }

        }


        public void DisperseUpdate() {

            FVector2 offset = FVector2.Zero;
            int pushCount = 0;

            radiusNear = unitsNearBy.Count>0;

            foreach (InteractiveObject unit in unitsNearBy) {
                if (unit.id == parent.id || unit.Moving!=parent.Moving) continue;

                fint off_avoidanceX = (parent.Position.X - unit.Position.X);
                fint off_avoidanceY = (parent.Position.Y - unit.Position.Y);
                fint distance = (off_avoidanceX * off_avoidanceX) + (off_avoidanceY * off_avoidanceY);


                if (distance!=fint.zero && distance >= (fint)(Radius * Radius)) continue;

                //fint dist = FMath.Sqrt(distance);

                FVector2 vec = new FVector2(off_avoidanceX, off_avoidanceY);
                vec.Normalize();
                /*if (distance==0) {
                    off_avoidanceX = 1;
                    off_avoidanceY = 1;
                }*/

                //distance=Radius*Radius;

                //double dist = Math.Sqrt(distance)/Point2D.DIVIDER;
                //double dist= ((Radius - Math.Sqrt(distance))/Radius);

                offset.X += vec.X;
                offset.Y += vec.Y;

                pushCount++;
            }

            if (pushCount != 0) {
                radiusTouch=true;

                fint xxx = offset.X / (fint)pushCount;
                fint yyy = offset.Y / (fint)pushCount;

                //xxx = GameUtils.Clamp(xxx, -Radius, Radius);
                //yyy = GameUtils.Clamp(yyy, -Radius, Radius);

                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X + parent.Velocity.X + xxx, parent.Position.Y + parent.Velocity.Y))
                    xxx = fint.zero;
                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X + parent.Velocity.X, parent.Position.Y + parent.Velocity.Y + yyy))
                    yyy = fint.zero;
                if (!BackgroundGame.MapSpace.TestWithDiagonals(parent.Position.X + parent.Velocity.X + xxx, parent.Position.Y + parent.Velocity.Y + yyy)) {
                    xxx = fint.zero;
                    yyy = fint.zero;
                }
                parent.Velocity.X += xxx;
                parent.Velocity.Y += yyy;
            }
        }

        public void RenderDebug() {
            if (active) {
                Vector3 CurrentPosition = new Vector3(dest.X.ToFloat(), 0, dest.Y.ToFloat());
                Vector3 normal = Vector3.UnitY;
                ForegroundGame.Map.GetHeightAndNormal(CurrentPosition, ref CurrentPosition.Y, true, ref normal);

                Color color = Color.White;
                if (active) color = Color.Yellow;
                if (reached) color = Color.Green;
                SceneManager.LineManager.AddLine(parent.IntObj3D.Position + new Vector3(0, 0.5f, 0), CurrentPosition + new Vector3(0, 0.5f, 0), new Color4(0, 1, 0, 0.2f));
            }
            //if (radiusNear) SceneManager.LineManager.AddGroundCircle(parent.IntObj3D.Position, NeighborCheckRadius/Point2D.DIVIDER, new Color4(0, 1, 0, 0.5f));
            if (radiusTouch) SceneManager.LineManager.AddGroundCircle(parent.IntObj3D.Position, Radius, Color.Red);



            /*if (flowfield != null) {
                Vector3 _dir = new Vector3(0);
                for (int x = 0; x < BackgroundGame.MapSpace.Width; x++) {
                    for (int y = 0; y < BackgroundGame.MapSpace.Height; y++) {
                        byte val = flowfield.Get(x + y * BackgroundGame.MapSpace.Width);

                        Color4 colorz = new Color4(1, 0, 1, 0);

                        if (val == 0 || val == 1) continue;
                        if (val >= 100 && val != 255) {
                            val -= 100;
                            colorz = new Color4(1, 1, 1, 1);
                        }
                        Vector3 tmppos = new Vector3(1 + x * 2, 0, 1 + y * 2);
                        ForegroundGame.Map.GetHeight(tmppos, ref tmppos.Y);
                        if (SceneManager.Camera.Frustum.Contains(tmppos) == ContainmentType.Disjoint) continue;
                        tmppos += new Vector3(0, 0.1f, 0);


                        if (val != 255) {
                            _dir.X = MapSpace.Direction[val - 2, 0];
                            _dir.Z = MapSpace.Direction[val - 2, 1];
                            SceneManager.LineManager.AddLine(tmppos+ _dir * 0.5f, new Color4(1, 0, 0, 1), tmppos -_dir, new Color4(0, 1, 0, 1));
                            //SceneManager.LineManager.AddArrowFlat(tmppos + _dir * 0.5f, -_dir, colorz);
                        }

                    }
                }
            }*/

            /*float step = 1f/10;
            for (float x = 0; x < 1; x+=step) {
                for (float y = 0; y < 1; y+=step) {
                    SceneManager.LineManager.AddLine(CurrentPosition + new Vector3(x, 0, y), CurrentPosition + new Vector3(x, 3, y), Color.Green);
                }
            }*/
        }
    }
}
