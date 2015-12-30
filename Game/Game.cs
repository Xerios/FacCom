using RageEngine;
using RageEngine.ContentPipeline;
using RageEngine.Debug;
using RageEngine.Graphics;
using RageEngine.Graphics.ScreenManager;
using RageEngine.Graphics.TwoD;
using RageEngine.Input;
using RageEngine.Rendering;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using Keys = System.Windows.Forms.Keys;

namespace RageRTS
{
    public class Game : Main {

        
        protected override void Initialize() {

            ShaderManager.Add("VS_2D", Resources.GetShader("VS", "VS", "UI2D.fx")).SetInputLayout(Vertex2D.Elements);
            ShaderManager.Add("PS_2D", Resources.GetShader("PS", "PS", "UI2D.fx"));

            FontManager.Add("small", Resources.GetFont("Fonts/small"));
            FontManager.Add("default", Resources.GetFont("Fonts/default"));

            ScreenshotManager.Initialize();
            QuadRenderer.Initialize();
            InputManager.Initialize();

            WebWindow.Initialize();

            DebugPerfMonitor.Initialize();

            ScreenManager.AddScreen(new StartupScreen());
            ScreenManager.GotoScreen("Startup");

            Form.KeyDown += Form_KeyDown;

        }

        private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {

            if (e.KeyCode == Keys.Escape) {
                Game.Exit(); 
                e.Handled = true; 
                return; 
            }

            if (e.KeyCode == Keys.F12) ScreenshotManager.TakeScreenshot();

            if (e.KeyCode == Keys.F1) {
                GameConsole.visible = !GameConsole.visible;
                e.Handled = true;
            }

            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Return) {
                Display.Fullscreen=!Display.Fullscreen;
                Display.swapChain.SetFullscreenState(Display.Fullscreen, null);
                e.Handled = true;
            }

            if (e.KeyCode == Keys.F2) {
                Game.Limit_FPS=!Game.Limit_FPS;
                //if (Game.Limit_FPS) GameConsole.Add("Debug", "FPS Limit enabled"); else GameConsole.Add("Debug", "FPS Limit disabled");
                e.Handled = true;
            }
        }

        protected override void Resize() {
            ScreenManager.Resize();
        }
        protected override void Dispose() {
            WebWindow.DisposeAll();
        }
        protected override void Update() {
            DebugPerfMonitor.StartFrame();
            InputManager.Update();
            WebWindow.Update();

            ScreenManager.Update();
        }


        protected override void Render() {


            Display.context.ClearDepthStencilView(Display.depthStencil, DepthStencilClearFlags.Depth, 1.0f, 0);
            Display.context.ClearRenderTargetView(Display.renderTarget,  Color.Black);

            DebugPerfMonitor.BeginMark(0, "Render", Color.Blue);
            ScreenManager.Draw();
            DebugPerfMonitor.EndMark(0, "Render");

            
            GameConsole.Draw();
            DebugPerfMonitor.Draw(new Vector2(50, Display.Height - 100), Display.Width - 100);

            ScreenshotManager.Render();
        }
    }
}