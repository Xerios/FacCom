using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ColorChooserCSharp
{
	/// <summary>
	/// Summary description for ColorChooser2.
	/// </summary>
	public class ColorChooser2 : System.Windows.Forms.Form {
		internal System.Windows.Forms.Label lblSaturation;
		internal System.Windows.Forms.Label lblHue;
		internal System.Windows.Forms.HScrollBar hsbSaturation;
		internal System.Windows.Forms.HScrollBar hsbHue;
		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnOK;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Panel pnlColor;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Panel pnlSelectedColor;
		internal System.Windows.Forms.Panel pnlBrightness;
		internal System.Windows.Forms.Label Label2;
		internal NumericUpDown nudRed;
		internal NumericUpDown nudBlue;
		internal NumericUpDown nudGreen;

		public Action onColorChange; 
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColorChooser2()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblSaturation = new System.Windows.Forms.Label();
            this.lblHue = new System.Windows.Forms.Label();
            this.hsbSaturation = new System.Windows.Forms.HScrollBar();
            this.hsbHue = new System.Windows.Forms.HScrollBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.pnlColor = new System.Windows.Forms.Panel();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.pnlSelectedColor = new System.Windows.Forms.Panel();
            this.pnlBrightness = new System.Windows.Forms.Panel();
            this.Label2 = new System.Windows.Forms.Label();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSaturation
            // 
            this.lblSaturation.Location = new System.Drawing.Point(426, 204);
            this.lblSaturation.Name = "lblSaturation";
            this.lblSaturation.Size = new System.Drawing.Size(27, 23);
            this.lblSaturation.TabIndex = 50;
            this.lblSaturation.Text = "0";
            this.lblSaturation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHue
            // 
            this.lblHue.Location = new System.Drawing.Point(426, 180);
            this.lblHue.Name = "lblHue";
            this.lblHue.Size = new System.Drawing.Size(27, 23);
            this.lblHue.TabIndex = 49;
            this.lblHue.Text = "0";
            this.lblHue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // hsbSaturation
            // 
            this.hsbSaturation.LargeChange = 1;
            this.hsbSaturation.Location = new System.Drawing.Point(313, 206);
            this.hsbSaturation.Maximum = 255;
            this.hsbSaturation.Name = "hsbSaturation";
            this.hsbSaturation.Size = new System.Drawing.Size(108, 18);
            this.hsbSaturation.TabIndex = 44;
            this.hsbSaturation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
            // 
            // hsbHue
            // 
            this.hsbHue.LargeChange = 1;
            this.hsbHue.Location = new System.Drawing.Point(313, 182);
            this.hsbHue.Maximum = 255;
            this.hsbHue.Name = "hsbHue";
            this.hsbHue.Size = new System.Drawing.Size(108, 18);
            this.hsbHue.TabIndex = 43;
            this.hsbHue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HandleHSVScroll);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(366, 133);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 26);
            this.btnCancel.TabIndex = 42;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(366, 91);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 36);
            this.btnOK.TabIndex = 41;
            this.btnOK.Text = "OK";
            // 
            // Label3
            // 
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(273, 138);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(33, 18);
            this.Label3.TabIndex = 34;
            this.Label3.Text = "B :";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlColor
            // 
            this.pnlColor.Location = new System.Drawing.Point(8, 8);
            this.pnlColor.Name = "pnlColor";
            this.pnlColor.Size = new System.Drawing.Size(224, 216);
            this.pnlColor.TabIndex = 38;
            this.pnlColor.Visible = false;
            // 
            // Label6
            // 
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(273, 204);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(37, 18);
            this.Label6.TabIndex = 36;
            this.Label6.Text = "Sat. :";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label1
            // 
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(273, 91);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(33, 18);
            this.Label1.TabIndex = 32;
            this.Label1.Text = "R :";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label5
            // 
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(273, 181);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(48, 18);
            this.Label5.TabIndex = 35;
            this.Label5.Text = "Hue :";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSelectedColor
            // 
            this.pnlSelectedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSelectedColor.Location = new System.Drawing.Point(273, 8);
            this.pnlSelectedColor.Name = "pnlSelectedColor";
            this.pnlSelectedColor.Size = new System.Drawing.Size(176, 74);
            this.pnlSelectedColor.TabIndex = 40;
            this.pnlSelectedColor.Visible = false;
            // 
            // pnlBrightness
            // 
            this.pnlBrightness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBrightness.Location = new System.Drawing.Point(240, 8);
            this.pnlBrightness.Name = "pnlBrightness";
            this.pnlBrightness.Size = new System.Drawing.Size(24, 216);
            this.pnlBrightness.TabIndex = 39;
            this.pnlBrightness.Visible = false;
            // 
            // Label2
            // 
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(273, 115);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(33, 18);
            this.Label2.TabIndex = 33;
            this.Label2.Text = "G :";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nudRed
            // 
            this.nudRed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudRed.Location = new System.Drawing.Point(299, 91);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(61, 22);
            this.nudRed.TabIndex = 55;
            // 
            // nudBlue
            // 
            this.nudBlue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBlue.Location = new System.Drawing.Point(299, 136);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(61, 22);
            this.nudBlue.TabIndex = 57;
            // 
            // nudGreen
            // 
            this.nudGreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudGreen.Location = new System.Drawing.Point(299, 114);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(61, 22);
            this.nudGreen.TabIndex = 56;
            // 
            // ColorChooser2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(456, 233);
            this.ControlBox = false;
            this.Controls.Add(this.nudRed);
            this.Controls.Add(this.nudBlue);
            this.Controls.Add(this.nudGreen);
            this.Controls.Add(this.lblSaturation);
            this.Controls.Add(this.lblHue);
            this.Controls.Add(this.hsbSaturation);
            this.Controls.Add(this.hsbHue);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.pnlColor);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.pnlSelectedColor);
            this.Controls.Add(this.pnlBrightness);
            this.Controls.Add(this.Label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ColorChooser2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Color";
            this.Load += new System.EventHandler(this.ColorChooser2_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorChooser2_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HandleMouse);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private enum ChangeStyle
		{
			MouseMove,
			RGB,
			HSV,
			None
		}

		private ChangeStyle changeType = ChangeStyle.None;
		private Point selectedPoint;

		private ColorWheel myColorWheel;
		private ColorHandler.RGB RGB;
		private ColorHandler.HSV HSV;

		private void ColorChooser2_Load(object sender, System.EventArgs e)
		{
			// Turn on double-buffering, so the form looks better. 
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			// These properties are set in design view, as well, but they
			// have to be set to false in order for the Paint
			// event to be able to display their contents.
			// Never hurts to make sure they're invisible.
			pnlSelectedColor.Visible = false;
			pnlBrightness.Visible = false;
			pnlColor.Visible = false;

			// Calculate the coordinates of the three
			// required regions on the form.
			Rectangle SelectedColorRectangle =  new Rectangle(pnlSelectedColor.Location, pnlSelectedColor.Size);
			Rectangle BrightnessRectangle = new Rectangle(pnlBrightness.Location, pnlBrightness.Size);
			Rectangle ColorRectangle = new Rectangle(pnlColor.Location, pnlColor.Size);

			// Create the new ColorWheel class, indicating
			// the locations of the color wheel itself, the
			// brightness area, and the position of the selected color.
			myColorWheel = new ColorWheel(ColorRectangle, BrightnessRectangle, SelectedColorRectangle);
			myColorWheel.ColorChanged += 
				new ColorWheel.ColorChangedEventHandler(this.myColorWheel_ColorChanged);

			// Set the RGB and HSV values 
			// of the NumericUpDown controls.
			SetRGB(RGB);
			SetHSV(HSV);		
		}

		private void HandleMouse(object sender,  MouseEventArgs e)
		{
			// If you have the left mouse button down, 
			// then update the selectedPoint value and 
			// force a repaint of the color wheel.
			if ( e.Button == MouseButtons.Left ) 
			{
				changeType = ChangeStyle.MouseMove;
				selectedPoint = new Point(e.X, e.Y);
				this.Invalidate();
			}
		}

		private void frmMain_MouseUp(object sender,  MouseEventArgs e)
		{
			myColorWheel.SetMouseUp();
			changeType = ChangeStyle.None;
		}

		private void SetHSVLabels(ColorHandler.HSV HSV) 
		{
			RefreshText(lblHue, HSV.Hue);
			RefreshText(lblSaturation, HSV.Saturation);
		}

		private void SetRGB(ColorHandler.RGB RGB) 
		{
			// Update the RGB values on the form.
			RefreshValue(nudRed, RGB.Red);
			RefreshValue(nudBlue, RGB.Blue);
			RefreshValue(nudGreen, RGB.Green);
		}

		private void SetHSV( ColorHandler.HSV HSV) 
		{
			// Update the HSV values on the form.
			RefreshValue(hsbHue, HSV.Hue);
			RefreshValue(hsbSaturation, HSV.Saturation);
			SetHSVLabels(HSV);
		}

		private void RefreshValue(HScrollBar hsb, int value) 
		{
			hsb.Value = value;
		}

		private void RefreshText(Label lbl, int value) 
		{
			lbl.Text = value.ToString();
		}

		private void RefreshValue(NumericUpDown nud, int value) {
			// Update the value of the NumericUpDown control, 
			// if the value is different than the current value.
			// Refresh the control, causing an immediate repaint.
			if (nud.Value != value) {
				nud.Value = value;
				nud.Refresh();
			}
		}

		public Color Color  
		{
			// Get or set the color to be
			// displayed in the color wheel.
			get 
			{
				return myColorWheel.Color;
			}

			set 
			{
				// Indicate the color change type. Either RGB or HSV
				// will cause the color wheel to update the position
				// of the pointer.
				changeType = ChangeStyle.RGB;
				RGB = new ColorHandler.RGB(value.R, value.G, value.B);
				HSV = ColorHandler.RGBtoHSV(RGB);
			}
		}

		private void myColorWheel_ColorChanged(object sender,  ColorChangedEventArgs e)  
		{
			SetRGB(e.RGB);
			SetHSV(e.HSV);
		}

		private void HandleHSVScroll(object sender,  ScrollEventArgs e)  
			// If the H, S, or V values change, use this 
			// code to update the RGB values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
		{
			changeType = ChangeStyle.HSV;
			HSV = new ColorHandler.HSV(hsbHue.Value, hsbSaturation.Value, HSV.value);
			SetRGB(ColorHandler.HSVtoRGB(HSV));
			SetHSVLabels(HSV);
			this.Invalidate();
		}

		private void HandleRGBScroll(object sender, ScrollEventArgs e)
		{
			// If the R, G, or B values change, use this 
			// code to update the HSV values and invalidate
			// the color wheel (so it updates the pointers).
			// Check the isInUpdate flag to avoid recursive events
			// when you update the NumericUpdownControls.
			changeType = ChangeStyle.RGB;
			RGB = new ColorHandler.RGB((int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value);
			SetHSV(ColorHandler.RGBtoHSV(RGB));
			this.Invalidate();
		}

		private void ColorChooser2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Depending on the circumstances, force a repaint
			// of the color wheel passing different information.
			switch (changeType)
			{
				case ChangeStyle.HSV:
					myColorWheel.Draw(e.Graphics, HSV);
					break;
				case ChangeStyle.MouseMove:
				case ChangeStyle.None:
					myColorWheel.Draw(e.Graphics, selectedPoint);
					break;
				case ChangeStyle.RGB:
					myColorWheel.Draw(e.Graphics, RGB);
					break;
			}

            if (onColorChange!=null) onColorChange.Invoke();
		}
	
	}
}

