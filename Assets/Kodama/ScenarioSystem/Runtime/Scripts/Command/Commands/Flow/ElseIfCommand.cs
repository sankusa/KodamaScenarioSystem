using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ElseIfCommand : CommandBase, IBlockEnd, IBlockStart {
        public string BlockType => "If";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            IfBlock ifBlock;
            // Ifブロックを取り出して再プッシュ
            if(service.PageProcess.PeekBlock() is IfBlock) {
                ifBlock = service.PageProcess.PopBlock() as IfBlock;
                service.PageProcess.SetUpAndPushBlock(this, ifBlock);
            }
            // Ifブロック中でなければ、新規作成
            else {
                ifBlock = new IfBlock();
                service.PageProcess.SetUpAndPushBlock(this, ifBlock);
            }

            // 評価終了済ならジャンプ
            if(ifBlock.EvaluationFinished) {
                service.PageProcess.JumpToIndex(ifBlock.EndIndex);
            }
            // 評価未了ならIfCommandと同じ動作
            else {
                // 評価
                bool result = _condition.Evaluate(service.PageProcess);
                ifBlock.EvaluationFinished = result;

                // Trueなら続行、FalseならBlockEndまで飛ぶ
                if(result == false) {
                    service.PageProcess.JumpToIndex(ifBlock.EndIndex);
                }
            }
        }

        public override string GetSummary() {
            return _condition.GetSummary(this);
        }

        public override string Validate() {
            return _condition.Validate(this);
        }
    }
}