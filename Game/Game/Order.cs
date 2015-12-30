using System;
using System.Collections.Generic;
using System.Text;
using RageEngine.Utils;
using RageRTS.Map;
using System.Drawing;
using RageEngine;

namespace RageRTS {

    public enum OrderType : byte {
        NONE        = 0,
        PAUSE       = 1,

        GROUP       = 10,
        STOP        = 11,
        MOVE        = 12,
        ASSIST      = 13,
        TARGET      = 14,


        PLACE         = 20,
        BUILD         = 21,
        BUILD_CANCEL  = 22,
        BUILD_PAUSE   = 23,
        BUILD_RESUME  = 24,
        POWER_OFF     = 25,
        POWER_ON      = 26,

    }

    public class Order {
        public static uint UNIQUE_ID = 0;
        public uint ID;

        public int playerID;
        public OrderType type = OrderType.NONE;
        public bool official = true;
        public bool overwrite = true;

        public Order(OrderType type) {
            ID = UNIQUE_ID++;
            this.type = type;
        }

        public virtual byte[] Encode() { throw new Exception("This shouldn't execute"); }

        public virtual void Decode(byte[] data, int start, int length) { throw new Exception("This shouldn't execute"); }

        public virtual void Execute() { throw new Exception("This shouldn't execute"); }

        public static void DecodeAndExecute(int playerId,byte[] data,int i,int length) {
            byte typeByte = data[i++];
            bool overwrite = true;
            if (typeByte >= 100) { 
                typeByte -= 100; 
                overwrite = false; 
            }
            OrderType type = (OrderType)typeByte;
            Order order = null;

            switch (type) {
                /*overwritecase OrderType.PLACE: order1 = new Order_Train(); break;
                case OrderType.BUILD: order2 = new Order_Build(); break;
                case OrderType.ASSIST: order5 = new Order_Assist(); break;
                case OrderType.TARGET: order4 = new Order_Target(); break;*/
                case OrderType.MOVE: order = new Order_Move(); break;
            }
            if (order!=null) {
                order.Decode(data, i, length - 1);
                order.playerID = playerId;
                order.overwrite = overwrite;
                order.Execute();
            }

        }
    }

    public class Order_IObj: Order {
        public uint[] caller;
        public Order_IObj(OrderType type) : base(type) { }

        internal void DecodeCallers(byte[] data, ref int i) {
            int numberOfCallers = BitConverter.ToUInt16(data, i); i += 2;
            caller = new uint[numberOfCallers];

            for (int j = 0; j < numberOfCallers; j++) {
                caller[j] = BitConverter.ToUInt32(data, i); i += 4;
            }
        }

        internal void EncodeCallers(ref byte[] data, ref int i) {
            byte[] bytez;

            bytez = BitConverter.GetBytes((UInt16)caller.Length);
            data[i++] = bytez[0];
            data[i++] = bytez[1];

            foreach (uint id in caller) {
                bytez = BitConverter.GetBytes(id);
                data[i++] = bytez[0];
                data[i++] = bytez[1];
                data[i++] = bytez[2];
                data[i++] = bytez[3];
            }
        }

        public override void Execute() {
            foreach (uint id in caller) {
                InteractiveObject iobj = BackgroundGame.IntObjs.Find((o) => o.id == id);
                if (iobj!=null) 
                    iobj.Order(this); 
                else 
                    throw new Exception("Could not find InteractiveObject with ID = " + id + "\nThe IObj probably doesn't exist no more.");
            }
        }
    }

    public class Order_Move: Order_IObj {
        public FVector2 destination;
        public FlowFieldPath flowfield;

        public Order_Move() : base(OrderType.MOVE) { }

        public void Init(FVector2 destination)
        {
            this.destination = destination;
        }

        public override void Decode(byte[] data, int i, int length) {
            DecodeCallers(data, ref i);
            destination = new FVector2();
            destination.X = fint.CreateRaw(BitConverter.ToInt32(data, i)); i += 4;
            destination.Y = fint.CreateRaw(BitConverter.ToInt32(data, i)); i += 4;
        }

        public override byte[] Encode() {
            int i=0;
            byte[] data = new byte[ /*type*/ (1) + /*ID.length*/ (2) + /*IDs*/ (caller.Length * 4) + /*x*/ (4) + /*Y*/ (4)];

            data[i++] = (byte)((int)type + (overwrite ? 0 : 100));

            EncodeCallers(ref data, ref i);

            byte[] bytez;
            bytez = BitConverter.GetBytes(destination.X.raw);
            data[i++] = bytez[0];
            data[i++] = bytez[1];
            data[i++] = bytez[2];
            data[i++] = bytez[3];

            bytez = BitConverter.GetBytes(destination.Y.raw);
            data[i++] = bytez[0];
            data[i++] = bytez[1];
            data[i++] = bytez[2];
            data[i++] = bytez[3];
            return data;
        }

        public override void Execute() {

            if (BackgroundGame.MapSpace.Test(destination)) {

                List<Point2D> currentPositions = new List<Point2D>(caller.Length);

                foreach (uint id in caller) {
                    InteractiveObject iobj = BackgroundGame.IntObjs.Find((o) => o.id == id);
                    if (iobj!=null)
                        currentPositions.Add(iobj.Position.ToPoint2D());
                    else
                        throw new Exception("Could not find InteractiveObject with ID = " + id + "\nThe IObj probably doesn't exist no more.");
                }

                flowfield = BackgroundGame.MapSpace.FindPath(currentPositions, destination.ToPoint2D());

                foreach (uint id in caller) {
                    InteractiveObject iobj = BackgroundGame.IntObjs.Find((o) => o.id == id);
                    if (iobj!=null)
                        iobj.Order(this); 
                    else
                        throw new Exception("Could not find InteractiveObject with ID = " + id + "\nThe IObj probably doesn't exist no more.");
                }

            }
        }
    }
}
