using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhenAllCommand : AsyncCommandBase {
        [SerializeField] private List<VariableKey<UniTask>> _unitaskVariableKeys;

        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            IEnumerable<UniTask> uniTasks = _unitaskVariableKeys.Select(x => service.PageProcess.FindVariable<UniTask>(x.Id).Value);
            await UniTask.WhenAll(uniTasks);
        }

        public override string GetSummary() {
            _unitaskVariableKeys
                .ForEach(x => {
                    SharedStringBuilder.Append(x.GetSummary(this));
                    SharedStringBuilder.Append("  ");
                });
            return SharedStringBuilder.Output();
        }

        public override string ValidateAsyncCommand() {
            _unitaskVariableKeys
                .ForEach(x => {
                    SharedStringBuilder.AppendAsNewLine(x.Validate(this, label: "UniTaskVariableKeys"));
                });
            return SharedStringBuilder.Output();
        }
    }
}