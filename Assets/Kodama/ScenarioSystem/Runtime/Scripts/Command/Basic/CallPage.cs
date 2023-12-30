using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class CallPage : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }

        [SerializeField] private CallType _callType;
        [SerializeField] private string _pageName;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentPageName = _pageName;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayPageInSameScenarioProcessAsync(service.PagePlayProcess as PagePlayProcess, _pageName, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayPageInSameScenarioProcessAsync(service.PagePlayProcess as PagePlayProcess, _pageName, cancellationToken)
                        .ForgetAndLogException();
                    break;
            }
        }
    }
}