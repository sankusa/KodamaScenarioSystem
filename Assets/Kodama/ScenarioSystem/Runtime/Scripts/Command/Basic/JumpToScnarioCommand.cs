using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class JumpToScnarioCommand : AsyncCommandBase {
        [SerializeField] private string _scenarioName;
        [SerializeField] private bool _return;
        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            Debug.Log("xx");
            if(_return) {
                await ScenarioPlayLoop.PlayAsync(_scenarioName, "", service.ServiceLocator, null, service.PagePlayProcess as PagePlayProcess, cancellationToken);
            }
            else {
                service.PagePlayProcess.SubsequentScenarioName = _scenarioName;
                service.PagePlayProcess.JumpToEndIndex();
            }
        }
    }
}