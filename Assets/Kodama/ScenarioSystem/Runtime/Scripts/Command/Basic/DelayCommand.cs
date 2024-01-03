using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class DelayCommand : AsyncCommandBase {
        [SerializeField] private float _seconds;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            await UniTask.Delay((int)(_seconds * 1000), cancellationToken: cancellationToken);
        }

        public override string GetSummary() {
            return $"Delay {_seconds} s";
        }
    }
}