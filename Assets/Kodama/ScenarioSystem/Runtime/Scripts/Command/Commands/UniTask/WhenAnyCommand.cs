using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhenAnyCommand : AsyncCommandBase {
        [SerializeField] private List<VariableName<UniTask>> _unitaskVariableNames;

        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            IEnumerable<UniTask> uniTasks = _unitaskVariableNames.Select(x => service.PagePlayProcess.GetVariableValue<UniTask>(x.Name));
            await UniTask.WhenAny(uniTasks);
        }

        public override string GetSummary() {
            StringBuilder sb = SharedStringBuilder.Instance;
            _unitaskVariableNames
                .ForEach(x => {
                    sb.Append(x.GetSummary());
                    sb.Append("  ");
                });
            string summary = sb.ToString();
            sb.Clear();

            return summary;
        }

        public override string Validate() {
            StringBuilder sb = SharedStringBuilder.Instance;
            _unitaskVariableNames
                .ForEach(x => {
                    string ret = x.Validate(this, true);
                    sb.Append(ret);
                    if(string.IsNullOrEmpty(ret) == false) {
                        sb.Append("  ");
                    }
                });
            string errorMessage = sb.ToString();
            sb.Clear();

            return errorMessage;
        }
    }
}