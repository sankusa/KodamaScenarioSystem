using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class CallPageCommand : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }

        [SerializeField] private CallType _callType;
        [SerializeField] private ScenarioPage _targetPage;
        public ScenarioPage TargetPage => _targetPage;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentPage = _targetPage;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayPageInSameScenarioProcessAsync(service.PagePlayProcess as PagePlayProcess, _targetPage, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayPageInSameScenarioProcessAsync(service.PagePlayProcess as PagePlayProcess, _targetPage, cancellationToken)
                        .ForgetAndLogException();
                    break;
            }
        }

        public override string GetSummary() {
            return $"{(_targetPage != null ? _targetPage.name : "")},  {_callType.ToString()}";
        }
    }
}