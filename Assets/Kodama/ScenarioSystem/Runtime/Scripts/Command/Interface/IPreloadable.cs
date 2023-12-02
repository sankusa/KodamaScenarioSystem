using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IPreloadable {
        UniTask PreloadAsync(ScenarioEngine engine);
        void Release(ScenarioEngine engine);
    }
}