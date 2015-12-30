using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using RageEngine.Graphics;
using RageEngine.Utils;
using RageEngine;
using SharpDX;
using RageEngine.Rendering;
using RageEngine.LQDB;
using RageRTS.Map;

namespace RageRTS
{

    public static class BackgroundGame {
        public static int GameSpeed = 1;
        public static int GameTick = 0, GameTick_Last = 0;

        public static float TimeInterpolate = 0;
        public static double TimeInterpolateDelta = 0;
        public static double TimeInterpolateLast = 0;

        public static Random random = new Random();

        public static Script Scripts_IObjects, Scripts_Projectiles;

        public static LocalityQueryProximityDatabase<InteractiveObject> IntObjsLQDB;
        public static List<InteractiveObject> IntObjs = new List<InteractiveObject>(1000);

        public static CircularList<byte[]> OrderListToSend = new CircularList<byte[]>(20);

        public static MapSpace MapSpace;

    }

    public static class ForegroundGame {

        public static Stopwatch timer = new Stopwatch();

        public static TerrainInfo Map;
        public static DecalManager TerrainDecals;
        public static TerrainDebug TerrainDebug;

        public static InteractiveObject2DManager IntObjsManager;

        public static Texture startegicIcons, selectionBrackets, unitPlaceholder;
        public static List<InteractiveObject3D> IntObjs3D = new List<InteractiveObject3D>(1000);
    }


}
