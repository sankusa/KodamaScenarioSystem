using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhileCommand : CommandBase, IBlockStart {
        public string BlockType => "While";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            WhileBlock whileBlock;
            if(service.PagePlayProcess.PeekBlock() is WhileBlock wb && wb.WhileCommand == this) {
                whileBlock = service.PagePlayProcess.PeekBlock() as WhileBlock;
            }
            else {
                whileBlock = new WhileBlock(this);
                service.PagePlayProcess.SetUpAndPushBlock(this, whileBlock);
            }
            
            // 評価
            bool result = _condition.Evaluate(service.PagePlayProcess);

            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PagePlayProcess.JumpToIndex(whileBlock.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return $"<color={Colors.BlockSummaryCaption}>While</color>  " + _condition.GetSummary();
        }
    }
}