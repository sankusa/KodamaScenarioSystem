using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class DelayCommand : AsyncCommandBase {
        [SerializeField] private FloatValueOrVariableKey _seconds;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            float seconds = _seconds.HasKey() ? (service.PagePlayProcess.FindVariable(_seconds.VariableKey) as FloatVariable).Value : _seconds.Value;
            await UniTask.Delay((int)(seconds * 1000), cancellationToken: cancellationToken);
        }

        public override string GetSummary() {
            return _seconds.GetSummary(this) + " seconds";
        }
    }
}