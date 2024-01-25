using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ReflectionCommand : AsyncCommandBase {
        [SerializeField] private ReflectionMethodInvokeData _invokeData;
        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            _invokeData.Invoke(service);
            await UniTask.CompletedTask;
        }
    }
}