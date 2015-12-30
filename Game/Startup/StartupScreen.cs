using RageEngine.ContentPipeline;
using RageEngine.Graphics;
using RageEngine.Graphics.ScreenManager;
using RageEngine.Graphics.TwoD;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageRTS {
    class StartupScreen: Screen {

        public StartupScreen() : base("Startup") { }

        public override void Initialize() {
            AddLayer(new StartupLayer());
        }
    }

    class StartupLayer: ScreenLayer {

        public static Texture logo;

        public override void Initialize() {

            logo = Resources.GetTexture("Textures/logo.png", false);

        }

        public override void Render() {

            QuadRenderer.Begin();
            float alpha = (10 - timer) / 10;

            QuadRenderer.DrawFullScreenGradient(new Color4(38f/255, 38f/255, 39f/255, alpha), new Color4(70f/255, 69f/255, 70f/255, alpha));

            Color4 color = new Color4(1, 1, 1, alpha);

            QuadRenderer.Draw(StartupLayer.logo, new Vector2(Display.Width / 2 - StartupLayer.logo.Width / 2, Display.Height / 2 - StartupLayer.logo.Height / 2 - 20*alpha), color);

            QuadRenderer.End();
        }

        float timer=10;

        public override void Update() {
            timer--;

            if (timer>-1) return;

            //ScreenManager.AddScreen(new LobbyScreen());
            //ScreenManager.GotoScreen("Lobby");

            ScreenManager.AddScreen(new InGameScreen());
            ScreenManager.GotoScreen("Game");
        }
    }
}
