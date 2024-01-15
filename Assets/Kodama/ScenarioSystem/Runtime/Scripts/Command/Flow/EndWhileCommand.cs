using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndWhileCommand : CommandBase, IBlockEnd {
        public string BlockType => "While";
        
        public override void Execute(ICommandService service) {
            if(service.PagePlayProcess.PeekBlock() is WhileBlock whileBlock) {
                service.PagePlayProcess.JumpToIndex(whileBlock.StartIndex);
            }
        }

        public override string GetSummary() {
            return "";
        }
    }
}