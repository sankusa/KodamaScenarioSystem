using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhileCommand : CommandBase, IBlockStart {
        public string BlockType => "While";

        [SerializeField] private Condition _condition;

        public override void Execute(ICommandService service) {
            WhileBlock whileBlock;
            if(service.PageProcess.PeekBlock() is WhileBlock wb && wb.WhileCommand == this) {
                whileBlock = service.PageProcess.PeekBlock() as WhileBlock;
            }
            else {
                whileBlock = new WhileBlock(this);
                service.PageProcess.SetUpAndPushBlock(this, whileBlock);
            }
            
            // 評価
            bool result = _condition.Evaluate(service.PageProcess);

            // Trueなら続行、FalseならBlockEndまで飛ぶ
            if(result == false) {
                service.PageProcess.JumpToIndex(whileBlock.EndIndex + 1);
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