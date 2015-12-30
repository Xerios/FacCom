using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using RageEngine;
using RageEngine.Debug;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace RageEngine.Graphics {
    public static class ScreenshotManager {
        private static string screenshotPath;

        private static bool takeScreen = false;

        public static void Initialize() {
            screenshotPath = System.Windows.Forms.Application.StartupPath + "\\..\\__ScreenShots";
            if (!System.IO.Directory.Exists(screenshotPath))
                System.IO.Directory.CreateDirectory(screenshotPath);
        }

        public static void TakeScreenshot() {
            takeScreen = true;
        }

        public static void Render() {
            if (!takeScreen) return;
            takeScreen=false;
            string timestampName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            int i = 1;
            while (File.Exists(String.Format("{0}\\{1}.jpg", screenshotPath, timestampName))) {
                timestampName += " (" + i + ")";
                i++;
            }

            string filename = String.Format("{0}\\{1}.jpg", screenshotPath, timestampName);

            Display.context.ResolveSubresource(Display.backBuffer, 0, Display.backBufferCopy, 0, Format.R8G8B8A8_UNorm);

            Texture2DDescription desc = new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = Format.R8G8B8A8_UNorm,
                Width = Display.Width,
                Height = Display.Height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1,0),
                Usage = ResourceUsage.Staging,
            };

            Texture2D tex = new Texture2D(Display.device,desc);
            Display.context.CopyResource(Display.backBufferCopy, tex);

            Texture2D.ToFile(Display.context, tex, ImageFileFormat.Jpg, filename);

            GameConsole.Add("Screenshot saved to : '"+Path.GetFullPath(filename)+"'");
            tex.Dispose();
        }
    }
}
