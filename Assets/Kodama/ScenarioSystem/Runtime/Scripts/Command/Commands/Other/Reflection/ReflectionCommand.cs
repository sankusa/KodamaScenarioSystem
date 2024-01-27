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
        [SerializeField, TextArea] private string _summary;
        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            await _invokeData.Invoke(service, cancellationToken);
        }

        public override string GetSummary() {
            return _summary;
        }
    }
}