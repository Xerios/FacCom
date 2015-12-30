using RageEngine;
using RageEngine.ContentPipeline;
using RageEngine.Graphics;
using RageEngine.LQDB;
using RageRTS.Behaviors;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageRTS {
    public enum IntObjType { NONE, SUPPORT, OFFENSIVE, DEFENSIVE, BASE, WALL }
    public enum IntObjMovementType { NONE, LAND, AIR, WATER, CUSTOM }

    public class InteractiveObject {
        private static uint UNIQUE_ID = 0;

        // Initialized values
        internal readonly uint id;
        internal readonly WeakReference parent;
        internal readonly BlueprintIntObject blueprint;


        //--------------------------------------------
        //       Deterministic floats
        //--------------------------------------------
        public FVector2 Position, Velocity;
        public fint Rotation;

        public bool Moving = false;

        public int Health,HealthMax;

        //--------------------------------------------
        //       Game object specific values
        //--------------------------------------------
        public int                Side     = -1;
        public IntObjType         Type     = IntObjType.NONE;
        public IntObjMovementType Movement = IntObjMovementType.NONE;

        public InteractiveObject2D IntObj2D;
        public InteractiveObject3D IntObj3D;

        public ITokenForProximityDatabase<InteractiveObject> proximityToken;

        protected Order CurrentOrder     = null;
        protected bool CurrentOrderLock = false;
        public Queue<Order> Orders  = new Queue<Order>();
        public List<Behavior> Behaviors = new List<Behavior>();


        //--------------------------------------------
        public bool Initialized = false;

        public InteractiveObject(InteractiveObject parent, BlueprintIntObject blueprint) {
            this.id = ++InteractiveObject.UNIQUE_ID;
            this.parent = new WeakReference(parent);
            this.blueprint = blueprint;
        }

        //=========================================================================================================================

        public void Init(int side, FVector2 pos, float rotation) {
            this.Side = side;
            this.Position = pos;

            IntObj3D = new InteractiveObject3D(this);
            ForegroundGame.IntObjs3D.Add(IntObj3D);
            IntObj2D = ForegroundGame.IntObjsManager.Add(this);

            proximityToken = BackgroundGame.IntObjsLQDB.AllocateToken(this);
            proximityToken.UpdateForNewPosition(new Point3D(pos.X.ToInt(), 0, pos.Y.ToInt()));


            onInitialize();

            Initialized = true;
        }

        public void Dispose() {

            Initialized = false;

            proximityToken.Dispose();

            if (IntObj2D.selected) InGameLayer.instance.RefreshSelection();
            ForegroundGame.IntObjsManager.Remove(IntObj2D);

            onDestroy();

        }


		public virtual void Update() {
            if (!Initialized) return;

            FVector2 previousPosition = Position;

            onUpdate();

            for (int i = 0; i < Behaviors.Count; i++) Behaviors[i].Update();

            if (previousPosition!=Position) {
                proximityToken.UpdateForNewPosition(new Point3D(Position.X.ToInt(), 0, Position.Y.ToInt()));
            }
		}

        public virtual void Prepare() {
            if (!Initialized) return;

            Vector3 pos = new Vector3(Position.X.ToFloat(), 0, (float)Position.Y.ToFloat());
            Vector3 normal = Vector3.UnitY;
            ForegroundGame.Map.GetHeightAndNormal(pos, ref pos.Y, true, ref normal);
            IntObj3D.Position = pos;
        }

        //=========================================================================================================================

        public void Order(Order order) {
            //if (order.playerID != side) return;

            if (order.overwrite) {
                Orders.Clear();
                if (CurrentOrderLock && CurrentOrder!=null)
                    Orders.Enqueue(CurrentOrder);
                else 
                    CurrentOrder=null;
            }

            Orders.Enqueue(order);

            if (CurrentOrder==null) NextOrder();
        }

        protected void NextOrder() {
            if (Orders.Count == 0) {
                CurrentOrder=null;
                return;
            }
            CurrentOrder = Orders.Dequeue();
            onOrder();
        }
        //=========================================================================================================================

        protected virtual void onInitialize() { }
        protected virtual void onOrder() { }
        protected virtual void onUpdate() { }
        protected virtual void onDestroy() { }
    }
}
