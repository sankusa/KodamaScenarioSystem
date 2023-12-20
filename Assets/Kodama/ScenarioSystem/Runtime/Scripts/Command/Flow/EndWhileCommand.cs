using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndWhileCommand : CommandBase, IBlockEnd {
        public string BlockType => "While";
        
        public override void Execute(ICommandService service) {
            if(service.PlayProcess.PeekBlock() is WhileBlock whileBlock) {
                service.PlayProcess.JumpToIndex(whileBlock.StartIndex);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>EndWhile</b></color>";
        }
    }
}