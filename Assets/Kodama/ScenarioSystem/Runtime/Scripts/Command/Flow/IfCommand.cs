using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class IfCommand : CommandBase, IBlockStart {
        public string BlockType => "If";

        [SerializeField] private bool result;

        public override void Execute(ICommandService service) {
            // 評価
            IfBlock ifBlock = new IfBlock(){EvaluationFinished = result};
            service.PlayProcess.SetUpAndPushBlock(this, ifBlock);
            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PlayProcess.JumpToIndex(ifBlock.EndIndex);
            }
        }

        public override string GetSummary() {
            return "<color=orange>If</color>";
        }
    }
}