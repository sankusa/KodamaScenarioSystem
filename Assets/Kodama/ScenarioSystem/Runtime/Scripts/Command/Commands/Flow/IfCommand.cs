using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class IfCommand : CommandBase, IBlockStart {
        public string BlockType => "If";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            IfBlock ifBlock = new IfBlock();
            service.PagePlayProcess.SetUpAndPushBlock(this, ifBlock);

            // 評価
            bool result = _condition.Evaluate(service.PagePlayProcess);
            ifBlock.EvaluationFinished = result;

            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PagePlayProcess.JumpToIndex(ifBlock.EndIndex);
            }
        }

        public override string GetSummary() {
            return _condition.GetSummary();
        }

        public override string Validate() {
            return _condition.Validate(this);
        }
    }
}