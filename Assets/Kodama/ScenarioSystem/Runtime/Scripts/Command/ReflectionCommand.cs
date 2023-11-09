using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System.Threading;
#endif

namespace Kodama.ScenarioSystem {
    [Serializable]
#if KODAMA_UNITASK_SUPPORT
    public class ReflectionCommand : AsyncCommandBase {
#else
    public class ReflectionCommand : CommandBase {
#endif

    [SerializeField] private string commandname;
    [SerializeReference] List<object> args = new List<object>();

#if KODAMA_UNITASK_SUPPORT
        public override async UniTask ExecuteAsync(IScenarioEngine engine, CancellationToken cancellationToken)
        {
            base.Execute(engine);
        }
#else
        public override void Execute(IScenarioEngine engine)
        {
            base.Execute(engine);
        }
#endif
    }
}