using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ContinueCommand : CommandBase {
        
        public override void Execute(ICommandService service) {
            if(service.PagePlayProcess.PeekBlock() is Block.ILoopBlock) {
                Block block = service.PagePlayProcess.PopBlock();
                service.PagePlayProcess.JumpToIndex(block.StartIndex);
            }
        }

        public override string GetSummary() {
            return $"<color={Colors.BlockSummaryCaption}>Continue</color>";
        }
    }
}