using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ElseCommand : CommandBase, IBlockEnd, IBlockStart {
        public string BlockType => "If";

        public override void Execute(ICommandService service) {
            if(service.PagePlayProcess.PeekBlock() is IfBlock) {
                IfBlock ifBlock = service.PagePlayProcess.PopBlock() as IfBlock;
                service.PagePlayProcess.SetUpAndPushBlock(this, ifBlock);
                if(ifBlock.EvaluationFinished) {
                    service.PagePlayProcess.JumpToIndex(ifBlock.EndIndex);
                }
                else {
                    ifBlock.EvaluationFinished = true;
                }
            }
            // Else以前に対応するIfやElseIfが存在していない場合、評価結果=Trueとして扱う。
            else {
                IfBlock ifBlock = new IfBlock(){EvaluationFinished = true};
                service.PagePlayProcess.SetUpAndPushBlock(this, ifBlock);
            }
        }

        public override string GetSummary() {
            return $"<color={Colors.BlockSummaryCaption}>Else</color>";
        }
    }
}