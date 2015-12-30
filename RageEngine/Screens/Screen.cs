using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.Graphics.ScreenManager {
    public class Screen {
        public string Name;

        private List<ScreenLayer> layers = new List<ScreenLayer>();
        private IOrderedEnumerable<ScreenLayer> orderedList;

        private bool init = false;


        public Screen(string name) {
            this.Name = name;
        }

        public void AddLayer(ScreenLayer screen) {
            layers.Add(screen);
            screen.Initialize();
            RefreshPriorities();
        }

        public void RefreshPriorities() {
            orderedList = from layer in layers orderby layer.ScreenOrder select layer;
        }

        public virtual void Initialize() {

        }

        public virtual void Shutdown() {
            foreach (ScreenLayer layer in layers) layer.Shutdown();
        }

        public void Update() {
            foreach (ScreenLayer layer in orderedList) layer.Update();
            if (!init) init = true;
            if (delayResize) Resize();
        }

        public void Render() {
            if (!init) return;
            foreach (ScreenLayer layer in orderedList) layer.Render();
        }

        bool delayResize = false;
        public void Resize() {
            if (!init) {
                delayResize=true;
                return;
            }
            delayResize=false;
            foreach (ScreenLayer layer in orderedList) layer.Resize();
        }

    }
}
