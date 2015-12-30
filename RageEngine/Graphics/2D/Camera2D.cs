using SharpDX;
using SharpDX.Direct3D11;
namespace RageEngine.Graphics.TwoD
{
    public static class Camera2D
    {
        private static float _zoom = 1f; // Camera Zoom
        private static Matrix _transform,_reverseTransform; // Matrix Transform
        private static Vector2 _pos= new Vector2(0); // Camera Position
        private static float _rotation = 0; // Camera Rotation
        private static Rectangle screenRectangle = new Rectangle(0,0,0,0);
  
        public static float ZoomMin = 0.5f,
                            ZoomMax = 10;
        // Sets and gets X
        public static float X
        {
            get { return _pos.X; }
            set { 
                _pos.X = value;
                Update();
            }
        }
        // Sets and gets Y
        public static float Y
        {
            get { return _pos.Y; }
            set { 
                _pos.Y = value;
                Update();
            }
        }
        // Sets and gets zoom
        public static float Zoom
        {
            get { return _zoom; }
            set { 
                _zoom = value;
                if (_zoom < ZoomMin) _zoom = ZoomMin;
                if (_zoom > ZoomMax) _zoom = ZoomMax; 
                Update();
            } // Negative zoom will flip image
        }

        // Rotation
        public static float Rotation {
            get { return _rotation; }
            set {
                _rotation = value;
                Update();
            }
        }

        private static Buffer cameraBuffer;
        public static void Initialize() {
            cameraBuffer = new Buffer(Display.device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        private static void Update() {

            _transform = Matrix.Translation(-_pos.X, -_pos.Y, 0) *
                         Matrix.RotationZ(-_rotation) *
                         Matrix.Translation(Display.Width*0.5f, Display.Height*0.5f, 0) *
                         Display.screenMatrix*
                         Matrix.Scaling(_zoom, _zoom, 0) ;


            _reverseTransform = Matrix.Translation(-Display.Width*0.5f, -Display.Height*0.5f, 0) *
                                Matrix.RotationZ(_rotation) *
                                Matrix.Scaling(1/_zoom, 1/_zoom, 0) *
                                Matrix.Translation(_pos.X, _pos.Y, 0);
                         

            screenRectangle = new Rectangle(
                (int)(-(float)Display.Width/2/Camera2D.Zoom),
                (int)(-(float)Display.Height/2/Camera2D.Zoom),
                (int)((float)Display.Width/Camera2D.Zoom),
                (int)((float)Display.Height/Camera2D.Zoom));

        }

        public static void UpdateBuffer() {
            Display.context.UpdateSubresource(ref _transform, cameraBuffer);
        }

        public static Matrix getMatrix(){
            return _transform;
        }

        public static Buffer getBuffer() {
            return cameraBuffer;
        }

        public static Vector2 getCoords(int x, int y){
            Vector2 transf = Vector2.TransformCoordinate(new Vector2(x, y), _reverseTransform);
            return transf;
        }

        public static Vector2 toScreen(float x, float y) {
            Matrix trans = Matrix.Translation(-_pos.X, -_pos.Y, 0) *
                         Matrix.Scaling(_zoom, _zoom, 0) *
                         Matrix.RotationZ(-_rotation) *
                         Matrix.Translation(Display.Width*0.5f, Display.Height*0.5f, 0)
                         ;

            Vector2 transf = Vector2.TransformCoordinate(new Vector2(x, y), trans);
            return transf;

        }
    }
}

