using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class IfCommand : CommandBase, IBlockStart {
        public string BlockType => "If";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            IfBlock ifBlock = new IfBlock();
            service.PlayProcess.SetUpAndPushBlock(this, ifBlock);

            // 評価
            bool result = _condition.Evaluate(service.PlayProcess);
            ifBlock.EvaluationFinished = result;

            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PlayProcess.JumpToIndex(ifBlock.EndIndex);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>If</b></color>  " + _condition.GetSummary();
        }
    }
}