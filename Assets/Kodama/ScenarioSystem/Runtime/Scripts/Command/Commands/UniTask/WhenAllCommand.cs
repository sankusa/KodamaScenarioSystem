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
            IEnumerable<UniTask> uniTasks = _unitaskVariableKeys.Select(x => service.PagePlayProcess.FindVariable<UniTask>(x.Id).Value);
            await UniTask.WhenAll(uniTasks);
        }

        public override string GetSummary() {
            StringBuilder sb = SharedStringBuilder.Instance;
            _unitaskVariableKeys
                .ForEach(x => {
                    sb.Append(x.GetSummary(this));
                    sb.Append("  ");
                });
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public override string Validate() {
            StringBuilder sb = SharedStringBuilder.Instance;
            _unitaskVariableKeys
                .ForEach(x => {
                    string ret = x.Validate(this);
                    if(string.IsNullOrEmpty(ret) == false && sb.Length > 0) sb.Append("\n");
                    sb.Append(ret);
                });
            string baseErrorMessage = base.Validate();
            if(string.IsNullOrEmpty(baseErrorMessage) == false && sb.Length > 0) sb.Append("\n");
            sb.Append(baseErrorMessage);
            string errorMessage = sb.ToString();
            sb.Clear();

            return errorMessage;
        }
    }
}