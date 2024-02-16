using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CallPageCommand : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }
        
        [SerializeField] private SiblingPageSelector _target;
        public ScenarioPage TargetPage => _target.Page;

        [SerializeField] private CallType _callType;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PageProcess.SubsequentPage = _target.Page;
                    service.PageProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayPageInSameScenarioProcessAsync(service.PageProcess as PagePlayProcess, _target.Page, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayPageInSameScenarioProcessAsync(service.PageProcess as PagePlayProcess, _target.Page, cancellationToken)
                        .ForgetAndLogException();
                    break;
            }
        }

        public override string GetSummary() {
            SharedStringBuilder.Append(_target.GetSummary());
            SharedStringBuilder.Append(",  ");
            SharedStringBuilder.Append(_callType.ToString());
            return SharedStringBuilder.Output();
        }

        public override string ValidateAsyncCommand() {
            return _target.Validate(this, "Target");
        }

        public override FlexibleEnumerable<ScenarioPage> GetReferencingSiblingPages() {
            return new FlexibleEnumerable<ScenarioPage>(_target.Page);
        }
    }
}