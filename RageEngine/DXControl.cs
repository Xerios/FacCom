using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.Direct3D11;
using SharpDX;
using Device = SharpDX.Direct3D11.Device;
using Resource=SharpDX.Direct3D11.Resource;
using SharpDX.DXGI;
using RageEngine.Graphics;
using RageEngine.Debug;
using SharpDX.Direct3D;
using System.ComponentModel;
using RageEngine.Graphics.TwoD;
using RageEngine.Input;
using RageEngine.ContentPipeline;

namespace RageEngine {

    public abstract class DXControl: Control {

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void OnCreateControl()
        {

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);


            Logger.Log("GetSupportedFeatureLevel : "+Device.GetSupportedFeatureLevel().ToString());

            if (Device.GetSupportedFeatureLevel()<FeatureLevel.Level_10_0) {
                Logger.Log("! ERROR : DirectX 10.0 level features are not supported.");
                throw new NotSupportedException("DirectX 10.0 level features are not supported ( "+Device.GetSupportedFeatureLevel().ToString()+")");
            }

            Global.Timer=new Timer();
            Logger.Log("Render form initialized.");

            Display.SetupDisplay(this);

            WinInitialize();

            base.OnCreateControl();
        }


        /// <summary>
        /// Disposes the control.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (DesignMode) return;

            Global.IsRunning=false;

            WinDispose();

            Display.Dispose();

            Resources.Dispose();
            Logger.Log("Resources disposed.");

            InputManager.Dispose();
            Logger.Log("Input Manager disposed.");

            ShaderManager.Dispose();
            Logger.Log("Shaders disposed.");

            //GC.SuppressFinalize(this);

            base.Dispose(disposing);
        }



        #region Paint


        /// <summary>
        /// Redraws the control in response to a WinForms paint message.
        /// </summary>
        /// 
        protected override void OnPaint(PaintEventArgs e)
        {
            try {
                string beginDrawError = BeginDraw();

                if (string.IsNullOrEmpty(beginDrawError)) {
                    // Draw the control using the GraphicsDevice.
                    WinUpdate();
                    Global.Timer.Update();
                    GraphicsManager.Reset();
                    WinRender();
                    EndDraw();
                } else {
                    // If BeginDraw failed, show an error message using System.Drawing.
                    PaintUsingSystemDrawing(e.Graphics, beginDrawError);
                }
            } catch (Exception err) {
                using (ErrorBox error = new ErrorBox(err)) error.ShowDialog();
            }
        }


        /// <summary>
        /// Attempts to begin drawing the control. Returns an error message string
        /// if this was not possible, which can happen if the graphics device is
        /// lost, or if we are running inside the Form designer.
        /// </summary>
        string BeginDraw()
        {
            if (DesignMode) return "DXControl is here";
            // If we have no graphics device, we must be running in the designer.
            if (Display.device==null) return "ERROR : Device null"+"\n\n"+GetType();
            if (Display.device.IsDisposed) return "ERROR : Device disposed !";

            return null;
        }


        /// <summary>
        /// Ends drawing the control. This is called after derived classes
        /// have finished their Draw method, and is responsible for presenting
        /// the finished image onto the screen, using the appropriate WinForms
        /// control handle to make sure it shows up in the right place.
        /// </summary>
        void EndDraw()
        {
            try
            {

                Display.swapChain.Present(1, PresentFlags.None);

                Application.DoEvents();
                this.Invalidate();
                //GraphicsDevice.Present(sourceRectangle, null, this.Handle);
            }
            catch(Exception)
            {
                throw;
                // Present might throw if the device became lost while we were
                // drawing. The lost device will be handled by the next BeginDraw,
                // so we just swallow the exception.
            }
        }




        /// <summary>
        /// If we do not have a valid graphics device (for instance if the device
        /// is lost, or if we are running inside the Form designer), we must use
        /// regular System.Drawing method to display a status message.
        /// </summary>
        protected virtual void PaintUsingSystemDrawing(System.Drawing.Graphics graphics, string text)
        {
            graphics.Clear(System.Drawing.Color.Orange);

            using (Brush brush = new SolidBrush(System.Drawing.Color.Black))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }


        /// <summary>
        /// Ignores WinForms paint-background messages. The default implementation
        /// would clear the control to the current background color, causing
        /// flickering when our OnPaint implementation then immediately draws some
        /// other color over the top using the XNA Framework GraphicsDevice.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }


        protected override void OnResize(EventArgs e) {
            if (this.ClientSize.Width==0 || this.ClientSize.Height == 0) return;

            Display.ResizeDisplay(this);
            WinResize();
            base.OnResize(e);
        }

        #endregion

        #region Abstract Methods

        protected virtual void WinInitialize() { }
        protected virtual void WinUpdate() { }
        protected virtual void WinRender() {
            Display.context.ClearRenderTargetView(Display.renderTarget, SharpDX.Color.CornflowerBlue);
        }
        protected virtual void WinResize() { }
        protected virtual void WinDispose() { }


        #endregion
    }
}
