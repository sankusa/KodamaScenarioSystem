using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class BreakCommand : CommandBase {
        
        public override void Execute(ICommandService service) {
            if(service.PlayProcess.PeekBlock() is Block.ILoopBlock) {
                Block block = service.PlayProcess.PopBlock();
                service.PlayProcess.JumpToIndex(block.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>Break</b></color>";
        }
    }
}