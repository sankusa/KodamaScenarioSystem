using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private ScenarioAndChildPageSelector _target;
        public ScenarioAndChildPageSelector Target => _target;
        [SerializeField] private CallType _callType;
        [SerializeReference] private List<CallArg> _scenarioArgs;
        public List<CallArg> ScenarioArgs => _scenarioArgs;
        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            switch (_callType) {
                case CallType.Jump:
                    service.PagePlayProcess.SubsequentScenario = _target.Scenario;
                    service.PagePlayProcess.SubsequentPage = _target.Page;
                    service.PagePlayProcess.SubsequentScenarioCallArgs = _scenarioArgs;
                    service.PagePlayProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _target.Scenario, _target.Page, cancellationToken, _scenarioArgs);
                    break;

                case CallType.Async:
                    ProcessManager.PlayScenarioInSameRootProcessAsync(service.PagePlayProcess as PagePlayProcess, _target.Scenario, _target.Page, cancellationToken, _scenarioArgs)
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
            SharedStringBuilder.Append(_target.Validate("Target"));
            if(_scenarioArgs.Count > 0 && _target.Scenario == null) {
                SharedStringBuilder.AppendAsNewLine("ScenarioArgs : MissingReference");
            }
            else {
                for(int i = 0; i < _scenarioArgs.Count; i++) {
                    if(_target.Scenario.Variables.FirstOrDefault(x => x.TargetType == _scenarioArgs[i].TargetType && x.Id == _scenarioArgs[i].VariableId) == null) {
                        SharedStringBuilder.AppendAsNewLine("ScenarioArgs[");
                        SharedStringBuilder.Append(i.ToString());
                        SharedStringBuilder.Append("] Missing Reference");
                    }
                }
            }
            return SharedStringBuilder.Output();
        }

        public override FlexibleEnumerable<Scenario> GetReferencingScenarios() {
            return new FlexibleEnumerable<Scenario>(_target.Scenario);
        }
    }
}