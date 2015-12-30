using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RageEngine.ContentPipeline;
using RageEngine.Input;
using RageEngine.Utils;
using SharpDX;

namespace RageEngine.Graphics.TwoD.GUI {

    public enum GUIAlign {
        None     = 0,
        Left     = 1,
        Center   = 2,
        Right    = 4,
        Top      = 8,
        Middle   = 16,
        Bottom   = 32,
    };

    public enum GUIPosition {
        Relative,
        Absolute,
        Fixed
    }

    public delegate void GUIAction(Element obj);

    public class Element {

        public object Tag;

        public string Name;
        public Element Parent;

        public Element InsertTo {
            set {
                if (value!=null)  value.Add(this);
            }
        }

        public Point2D Size = new Point2D(-100,-100);
        public Rectangle Margin;

        public GUIAlign Align   = GUIAlign.None;

        public Color4 StyleColor = Color.White;
        public bool Enabled    = false, Unclickable = false;

        public GUIAction onClick;//, onHover, onMouseOver, onMouseOut;
        protected GUIAction onMouseDown,onMouseUp,onFocus;

        protected Vector2 actualPosition = new Vector2(0);
        protected Vector2 actualSize;
        private bool mouseInside;
        public bool mouseOver, mouseDown;

        //public Dictionary<string, object> Values;

        //---------------------
        public List<Element> Elements;

        public Point2D PositionMarker;
        public Rectangle Padding;
        public bool Wrap = false;
        public GUIAlign InnerAlign = GUIAlign.Top;
        //---------------------



        public Element() {
            Initialize();
        }

        /*public Object this[string str] {
            get {
                if (Values == null) return null;
                object value = null;
                Values.TryGetValue(str, out value);
                return value;
            }
            set {
                if (Values == null) Values = new Dictionary<string, object>();
                if (!Values.ContainsKey(str)) {
                    Values.Add(str, value);
                } else {
                    Values.Remove(str);
                    Values.Add(str, value);
                }
            }
        }*/


        public void InnerUpdate() {

            mouseOver = false;

            Update();

            if (Parent != null) {
                int containerX, containerY, containerW, containerH;

                containerX = (int)Parent.actualPosition.X + Parent.Padding.Left;
                containerY = (int)Parent.actualPosition.Y + Parent.Padding.Top;
                containerW = (int)Parent.actualSize.X - Parent.Padding.Left - Parent.Padding.Right;
                containerH = (int)Parent.actualSize.Y - Parent.Padding.Top - Parent.Padding.Right;


                if (Size.X > 0) actualSize.X = Size.X; else actualSize.X = (containerW - Margin.Left - Margin.Right) * (float)Size.X*-0.01f;
                if (Size.Y > 0) actualSize.Y = Size.Y; else actualSize.Y = (containerH - Margin.Top - Margin.Bottom) * (float)Size.Y*-0.01f;

                if (Parent.InnerAlign == GUIAlign.None) {

                    // Horizontal Align
                    if (Align.HasFlag(GUIAlign.Left)) {
                        actualPosition.X = containerX + Margin.Left;
                    } else if (Align.HasFlag(GUIAlign.Right)) {
                        actualPosition.X = containerX + containerW - actualSize.X - Margin.Right;
                    } else if (Align.HasFlag(GUIAlign.Center))
                        actualPosition.X = containerX + containerW/2 - actualSize.X/2;

                    // Vertical Align
                    if (Align.HasFlag(GUIAlign.Top))
                        actualPosition.Y = containerY + Margin.Top;
                    else if (Align.HasFlag(GUIAlign.Bottom))
                        actualPosition.Y = containerY + containerH - actualSize.Y - Margin.Bottom;
                    else if (Align.HasFlag(GUIAlign.Middle))
                        actualPosition.Y = containerY + containerH/2 - actualSize.Y/2;

                } else {

                    if (Parent.InnerAlign.HasFlag(GUIAlign.Top)) {
                        /*if ((Parent.PositionMarker.X + Margin.Left + (int)finalSize.Y +  Margin.Right) > (Parent.finalSize.X - Parent.Padding.Right)) {
                            Parent.PositionMarker.X = 0;
                            Parent.PositionMarker.Y += Margin.Top +  (int)finalSize.Y + Margin.Bottom;
                        }*/
                        actualPosition.X = Parent.PositionMarker.X + Margin.Left;
                        actualPosition.Y = Parent.PositionMarker.Y + Margin.Top;
                        Parent.PositionMarker.Y +=  Margin.Top + (int)actualSize.Y +  Margin.Bottom;
                    }

                    if (Parent.InnerAlign.HasFlag(GUIAlign.Left)) {
                        actualPosition.X = Parent.PositionMarker.X + Margin.Left;
                        actualPosition.Y = Parent.PositionMarker.Y + Margin.Top;
                        Parent.PositionMarker.X +=  Margin.Left + (int)actualSize.X +  Margin.Right;
                    }
                }

                mouseInside=GameUtils.PointInRectangle(InputManager.MouseState.X, InputManager.MouseState.Y, (int)actualPosition.X, (int)actualPosition.Y, (int)actualSize.X, (int)actualSize.Y);
                if (mouseInside) GUIManager.hoveredOn = this;

                //---------------------------------------------------------

                /*if (InnerAlign.HasFlag(GUIAlign.Left)){
                    PositionMarker.X = Padding.Left;
                }else if (InnerAlign.HasFlag(GUIAlign.Right)){
                    PositionMarker.X = (int)actualSize.X - Padding.Right;
                }else*/ 
                if (InnerAlign.HasFlag(GUIAlign.Top)) {
                    PositionMarker.X = (int)actualPosition.X + Padding.Left;
                    PositionMarker.Y = (int)actualPosition.Y + Padding.Top;
                } 
                if (InnerAlign.HasFlag(GUIAlign.Left)) {
                    PositionMarker.X = (int)actualPosition.X + Padding.Left;
                    PositionMarker.Y = (int)actualPosition.Y + Padding.Top;
                }/* else if (InnerAlign.HasFlag(GUIAlign.Bottom)) {
                    PositionMarker.Y = (int)actualSize.Y - Padding.Bottom;
                }*/

            } else {
                actualSize.X = Size.X;
                actualSize.Y = Size.Y;
            }

            if (Elements!=null) for (int i = 0; i < Elements.Count; i++)  Elements[i].InnerUpdate();
        }

        private Color4 randColor = new Color4((float)GameUtils.random.NextDouble(), (float)GameUtils.random.NextDouble(), (float)GameUtils.random.NextDouble(),0.25f);

        public void InnerDraw() {

            if (Parent != null) {

                //if (mouseOver) randColor = new Color4((float)GameUtils.random.NextDouble(), (float)GameUtils.random.NextDouble(), (float)GameUtils.random.NextDouble(), 0.25f);
                //QuadRenderer.Draw(Resources.GetEmptyTexture(), new Rectangle((int)actualPosition.X, (int)actualPosition.Y, (int)actualSize.X, (int)actualSize.Y), randColor);

                Draw();
            }

            if (Elements!=null) for (int i = Elements.Count - 1; i >= 0; i--) Elements[i].InnerDraw();
        }

        protected virtual void Initialize() { }
        protected virtual void Update() { }
        protected virtual void Draw() { }

        public void Add(Element element) {
            if (Elements == null) Elements = new List<Element>();
            element.Parent = this;
            Elements.Add(element);
            //--------------------
            GUIManager.list.Add(element);
            if (element.Name != null) GUIManager.nameList.Add(element.Name, element);
        }

        public void Remove(Element element) {
            if (Elements != null) Elements.Remove(element);
        }

        public void Clear() {
            if (Elements != null) Elements.Clear();
        }

        public void Focus() {
            if (GUIManager.focusedOn == this) return;
            GUIManager.focusedOn = this;
            if (onFocus != null) onFocus.Invoke(this);
        }

        public virtual bool MouseMove() {
            bool answer = false;
            if (Unclickable) {
                answer = false;
            } else if (mouseOver) {
                mouseDown = true;
                answer = true;
            }

            if (Elements!=null) {
                for (int i = 0; i < Elements.Count; i++) {
                    if (Elements[i].MouseMove()) {
                        answer=true;
                    }
                }
            }
            return answer;
        }
        public virtual bool MouseDown() {
            bool answer = false;
            if (Unclickable) {
                answer = false;
            } else if (mouseOver) {
                mouseDown = true;
                answer = true;
                if (Enabled) {
                    if (onMouseDown != null) onMouseDown.Invoke(this);
                    Focus();
                }
            }

            if (Elements!=null) {
                for (int i = 0; i < Elements.Count; i++) {
                    if (Elements[i].MouseDown()) {
                        answer=true;
                    }
                }
            }
            return answer;
        }

        public virtual bool MouseUp() {
            bool answer = false;
            if (Unclickable) {
                answer = false;
            } else if (mouseDown) {
                mouseDown = false;
                if (mouseInside) {
                    if (onClick != null && mouseOver) onClick.Invoke(this);
                    answer = true;
                }
                if (onMouseUp != null) onMouseUp.Invoke(this);
                //if (mouseOver) answer = true;
            }

            if (Elements!=null) for (int i = 0; i < Elements.Count; i++) if (Elements[i].MouseUp()) answer = true; // Makes sure every element receives the message that the mouse is out
            return answer;
        }

    }
}
