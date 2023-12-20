using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ElseIfCommand : CommandBase, IBlockEnd, IBlockStart {
        public string BlockType => "If";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            IfBlock ifBlock;
            // Ifブロックを取り出して再プッシュ
            if(service.PlayProcess.PeekBlock() is IfBlock) {
                ifBlock = service.PlayProcess.PopBlock() as IfBlock;
                service.PlayProcess.SetUpAndPushBlock(this, ifBlock);
            }
            // Ifブロック中でなければ、新規作成
            else {
                ifBlock = new IfBlock();
                service.PlayProcess.SetUpAndPushBlock(this, ifBlock);
            }

            // 評価終了済ならジャンプ
            if(ifBlock.EvaluationFinished) {
                service.PlayProcess.JumpToIndex(ifBlock.EndIndex);
            }
            // 評価未了ならIfCommandと同じ動作
            else {
                // 評価
                bool result = _condition.Evaluate(service.PlayProcess);
                ifBlock.EvaluationFinished = result;

                // Trueなら続行、FalseならBlockEndまで飛ぶ
                if(result == false) {
                    service.PlayProcess.JumpToIndex(ifBlock.EndIndex);
                }
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>ElseIf</b></color>  " + _condition.GetSummary();
        }
    }
}