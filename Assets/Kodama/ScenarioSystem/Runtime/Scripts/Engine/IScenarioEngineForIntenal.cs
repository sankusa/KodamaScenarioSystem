using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal interface IScenarioEngineForInternal {
        internal void JumpToLabel(string targetLabel);
    }
}