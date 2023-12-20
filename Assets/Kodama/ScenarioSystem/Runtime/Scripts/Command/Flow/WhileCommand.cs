using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhileCommand : CommandBase, IBlockStart {
        public string BlockType => "While";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            WhileBlock whileBlock;
            if(service.PlayProcess.PeekBlock() is WhileBlock wb && wb.WhileCommand == this) {
                whileBlock = service.PlayProcess.PeekBlock() as WhileBlock;
            }
            else {
                whileBlock = new WhileBlock(this);
                service.PlayProcess.SetUpAndPushBlock(this, whileBlock);
            }
            
            // 評価
            bool result = _condition.Evaluate(service.PlayProcess);

            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PlayProcess.JumpToIndex(whileBlock.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>While</b></color>  " + _condition.GetSummary();
        }
    }
}