#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using System.Windows.Forms;
using RageEngine.Utils;
#endregion

namespace RageEngine.Input {

    public static class InputManager {

        private static Dictionary<Keys, string>   mapping_keyToAction  = new Dictionary<Keys, string>();
        private static Dictionary<string, Keys[]> mapping_actionToKeys  = new Dictionary<string, Keys[]>();
        private static Dictionary<string, Action> actions = new Dictionary<string, Action>();
        private static List<Keys> pressedKeys = new List<Keys>(10);

        public static event Action<MouseInputEventArgs> MouseDown;
        public static event Action<MouseInputEventArgs> MouseUp;
        public static event Action<MouseInputEventArgs> Wheel;
        public static event Action<MouseInputEventArgs> MouseMove;

        public static MouseState MouseState;
        private static MouseState MouseStateRealTime;
        private static MouseState MouseStateLast;
        private static int mouseWheelDelta;

        public static TextInput textInput;

        public static bool Ctrl = false;
        public static bool CancelNextInput = false;

        private static double DoubleClickTimer = 0;


        public static void Initialize(){

            //Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None,Main.Form.Handle);
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None, Main.Form.Handle);

            textInput = new TextInput(Main.Form.Handle);

            //Device.MouseInput +=Device_MouseInput;
            Device.KeyboardInput +=Device_KeyboardInput;

            Main.Form.MouseUp += new MouseEventHandler(Window_MouseUp);
            Main.Form.MouseDown += new MouseEventHandler(Window_MouseDown);
            Main.Form.MouseMove += new MouseEventHandler(Window_MouseMove);
            Main.Form.MouseWheel += new MouseEventHandler(Window_MouseWheel);
        }

        public static void Dispose() {
            Device.KeyboardInput -= Device_KeyboardInput;
            //Device.MouseInput -= Device_MouseInput;

            Main.Form.MouseUp -= Window_MouseUp;
            Main.Form.MouseDown -= Window_MouseDown;
            Main.Form.MouseMove -= Window_MouseMove;
            Main.Form.MouseWheel -= Window_MouseWheel;
        }

        public static void AssignKey(string action, string[] keys){

            var keysEnumd = new Keys[keys.Length];

            for (int i = 0; i < keys.Length; i++) {
                keysEnumd[i] = (Keys)Enum.Parse(typeof(Keys), keys[i]);
            }

            mapping_actionToKeys.Add(action, keysEnumd);

            for (int i = 0; i < keysEnumd.Length; i++){
                mapping_keyToAction.Add(keysEnumd[i], action);
            }
        }

        public static void AssignAction(string action, Action function) {
            actions.Add(action, function);
        }

        public static bool IsDown(string action) {
            Keys[] keys;
            mapping_actionToKeys.TryGetValue(action, out keys);

            for (int i = 0; i < keys.Length; i++) if (pressedKeys.Contains(keys[i])) return true;

            return false;
        }

        public static bool IsControlDown() {
            return pressedKeys.Contains(Keys.LButton | Keys.ShiftKey);
        }

        public static bool IsAltDown() {
            return pressedKeys.Contains(Keys.RButton | Keys.ShiftKey);
        }
        
        public static bool IsShiftDown() {
            return pressedKeys.Contains(Keys.ShiftKey);
        }

        public static bool IsKeyDown(string key) {
            return pressedKeys.Contains((Keys)Enum.Parse(typeof(Keys), key));
        }

        static void Device_KeyboardInput(object sender, KeyboardInputEventArgs e) {
            string actionLabel;
            mapping_keyToAction.TryGetValue(e.Key, out actionLabel);

            if (actionLabel!=null) {
                Action action;
                actions.TryGetValue(actionLabel, out action);

                if (action!=null && !pressedKeys.Contains(e.Key)) {
                    if (!CancelNextInput)  //GUI Input can cancel normal actions
                        action.Invoke();
                     else
                        CancelNextInput=false;
                }
            }

            if (e.State == KeyState.KeyDown || e.State == KeyState.SystemKeyDown) {
                if (!pressedKeys.Contains(e.Key)) pressedKeys.Add(e.Key);
            } else if (e.State == KeyState.KeyUp || e.State == KeyState.SystemKeyUp) {
                if (pressedKeys.Contains(e.Key)) pressedKeys.Remove(e.Key);
            }
        }

        static void Window_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) { MouseStateRealTime.Set(e.Button, false); }
        static void Window_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) { MouseStateRealTime.Set(e.Button, true); }
        static void Window_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) { MouseStateRealTime.Set(e.X, e.Y); }
        static void Window_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) { MouseStateRealTime.Set(e.Delta); }


        /// <summary>
        /// Update, called from BaseGame.Update().
        /// Will catch all new states for keyboard, mouse and the gamepad.
        /// </summary>
        public static void Update() {
            // Handle mouse input variables            
      
            DoubleClickTimer += Global.Timer.Time.Elapsed;// TODO: Double Clikc fix, time based and non tick
            MouseStateLast = MouseState;
            MouseState = MouseStateRealTime;


            mouseWheelDelta = MouseState.Wheel - MouseStateLast.Wheel;

            // Initialzie mouse event args
            MouseInputEventArgs mouseArgs = new MouseInputEventArgs();
            mouseArgs.Handled = false;
            mouseArgs.X = MouseState.X;
            mouseArgs.Y = MouseState.Y;
            mouseArgs.WheelDelta = mouseWheelDelta;
            mouseArgs.IsLeftButtonDown = (MouseState[MouseButtons.Left]);
            mouseArgs.IsRightButtonDown = (MouseState[MouseButtons.Right]);
            mouseArgs.IsMiddleButtonDown = (MouseState[MouseButtons.Middle]);

            // Mouse wheel event
            if (mouseWheelDelta != 0 && Wheel != null)
                Wheel(mouseArgs);

            // Mouse Left Button Events
            if (MouseState[MouseButtons.Left] && !MouseStateLast[MouseButtons.Left]) {
                mouseArgs.Button = MouseButtons.Left;

                if (MouseDown != null) MouseDown(mouseArgs);

            } else if (!MouseState[MouseButtons.Left] && MouseStateLast[MouseButtons.Left]) {
                mouseArgs.Button = MouseButtons.Left;

                if (DoubleClickTimer < 0.25f && GameUtils.Distance2D(MouseStateLast.X,MouseStateLast.Y,MouseState.X,MouseState.Y)<50) 
                    mouseArgs.IsDoubleClick = true; // TODO: this DOUBLE CLICK might be time off'd
                if (MouseUp != null) MouseUp(mouseArgs);
                DoubleClickTimer=0;
            }



            // Mouse Right Button Events
            if (MouseState[MouseButtons.Right] && !MouseStateLast[MouseButtons.Right]) {
                mouseArgs.Button = MouseButtons.Right;

                if (MouseDown != null)
                    MouseDown(mouseArgs);
            } else if (!MouseState[MouseButtons.Right] && MouseStateLast[MouseButtons.Right]) {
                mouseArgs.Button = MouseButtons.Right;

                if (MouseUp != null)
                    MouseUp(mouseArgs);
            }

            // Mouse Middle Button Events
            if (MouseState[MouseButtons.Middle] && !MouseStateLast[MouseButtons.Middle]) {
                mouseArgs.Button = MouseButtons.Middle;

                if (MouseDown != null)
                    MouseDown(mouseArgs);
            } else if (!MouseState[MouseButtons.Middle] && MouseStateLast[MouseButtons.Middle]) {
                mouseArgs.Button = MouseButtons.Middle;

                if (MouseUp != null)
                    MouseUp(mouseArgs);
            }

            // Mouse move event
            if (MouseMove != null && (MouseState.X != MouseStateLast.X ||MouseState.Y != MouseStateLast.Y)) MouseMove(mouseArgs);
        }
    }


    public struct MouseState {
        static Dictionary<MouseButtons, int> ButtonBits = new Dictionary<MouseButtons, int>() { { MouseButtons.Right, 1 }, { MouseButtons.Middle, 2 }, { MouseButtons.Left, 4 }, { MouseButtons.XButton1, 8 }, { MouseButtons.XButton2, 16 }, { MouseButtons.None, 32 } };

        int buttonState0;

        int positionX;
        int positionY;
        int scrollWheel;

        public int X { get { return positionX; } }
        public int Y { get { return positionY; } }
        public int Wheel { get { return scrollWheel; } }

        public bool this[MouseButtons button] {
            get {
                int bit = ButtonBits[button];
                return ((buttonState0 & bit) != 0);
            }
        }

        public void Set(System.Windows.Forms.MouseButtons button, bool buttonState) {
            int bit = ButtonBits[(MouseButtons)button];
            int bitSet = (buttonState) ? bit : 0;
            int bitClear = bit ^ 63;

            buttonState0 = (buttonState0 & bitClear) | bitSet;
        }

        public void Set(int x, int y) {
            positionX = x;
            positionY = y;
        }

        public void Set(int wheel) {
            scrollWheel += wheel;
        }
    }

    public class MouseInputEventArgs : EventArgs {
        public bool Handled { get; set; }
        public MouseButtons Button { get; internal set; }

        public int X { get; internal set; }
        public int Y { get; internal set; }

        public float WheelDelta { get; internal set; }

        public bool IsDoubleClick { get; internal set; }
        public bool IsLeftButtonDown { get; internal set; }
        public bool IsRightButtonDown { get; internal set; }
        public bool IsMiddleButtonDown { get; internal set; }

        public MouseInputEventArgs() { }
        public MouseInputEventArgs(MouseButtons button, int x, int y, float wheelDelta) {
            Button = button;
            X = x;
            Y = y;
            WheelDelta = wheelDelta;
        }
    }

}
