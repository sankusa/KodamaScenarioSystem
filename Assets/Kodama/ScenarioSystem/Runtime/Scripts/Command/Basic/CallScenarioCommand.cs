using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CallScenarioCommand : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }

        [SerializeField] private CallType _callType;
        [SerializeField] private Scenario _targetScenario;
        public Scenario TargetScenario => _targetScenario;
        [SerializeField] private ScenarioPage _targetPage;
        public ScenarioPage TargetPage => _targetPage;
        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentScenario = _targetScenario;
                    service.PagePlayProcess.SubsequentPage = _targetPage;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _targetScenario, _targetPage, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _targetScenario, _targetPage, cancellationToken)
                        .ForgetAndLogException();
                    break;
            }
        }

        public override string GetSummary() {
            return $"<color={Colors.CallSummaryCaption}>Call Scenario [ <color={Colors.Args}>{(_targetScenario != null ? _targetScenario.name : "")}</color> ] Page [ <color={Colors.Args}>{(_targetPage != null ? _targetPage.name : "")}</color> ] (<color={Colors.Args}>{_callType.ToString()}</color>)</color>";
        }
    }
}