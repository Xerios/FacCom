using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageRTS.Behaviors {
    public interface Behavior {
        void Dispose();
        void Update();
        void RenderDebug();
    }
}
