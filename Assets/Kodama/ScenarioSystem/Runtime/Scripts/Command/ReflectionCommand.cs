using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ReflectionCommand : AsyncCommandBase {
        [SerializeField] private string commandname;
        [SerializeReference] List<object> args = new List<object>();

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            Debug.Log(GetType().BaseType);
            await Task.CompletedTask;
        }
    }
}