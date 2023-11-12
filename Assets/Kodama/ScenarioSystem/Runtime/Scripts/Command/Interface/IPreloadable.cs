using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif
namespace Kodama.ScenarioSystem {
    public interface IPreloadable {
#if KODAMA_UNITASK_SUPPORT
        UniTask PreloadAsync(ScenarioEngine engine);
#else
        Task PreloadAsync(ScenarioEngine engine);
#endif
        void Release(ScenarioEngine engine);
    }
}