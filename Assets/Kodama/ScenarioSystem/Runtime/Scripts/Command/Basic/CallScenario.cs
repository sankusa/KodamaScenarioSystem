using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CallScenario : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }

        [SerializeField] private CallType _callType;
        [SerializeField] private string _scenarioName;
        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentScenarioName = _scenarioName;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _scenarioName, null, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _scenarioName, null, cancellationToken)
                        .Forget();
                    break;
            }
        }
    }
}