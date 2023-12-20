using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ContinueCommand : CommandBase {
        
        public override void Execute(ICommandService service) {
            if(service.PlayProcess.PeekBlock() is Block.ILoopBlock) {
                Block block = service.PlayProcess.PopBlock();
                service.PlayProcess.JumpToIndex(block.StartIndex);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>Continue</b></color>";
        }
    }
}