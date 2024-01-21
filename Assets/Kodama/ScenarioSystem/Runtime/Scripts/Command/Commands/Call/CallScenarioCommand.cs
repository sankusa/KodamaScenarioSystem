using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        [SerializeField] private ScenarioAndChildPageSelector _target;
        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentScenario = _target.Scenario;
                    service.PagePlayProcess.SubsequentPage = _target.Page;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _target.Scenario, _target.Page, cancellationToken);
                    break;

                case CallType.Async:
                    ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _target.Scenario, _target.Page, cancellationToken)
                        .ForgetAndLogException();
                    break;
            }
        }

        public override string GetSummary() {
            StringBuilder sb = SharedStringBuilder.Instance;
            sb.Append(_target.GetSummary());
            sb.Append(",  ");
            sb.Append(_callType.ToString());
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public override string Validate() {
            return _target.Validate(nameof(_target));
        }
    }
}