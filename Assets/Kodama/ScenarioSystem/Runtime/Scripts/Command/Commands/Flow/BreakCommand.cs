using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class BreakCommand : CommandBase {
        
        public override void Execute(ICommandService service) {
            if(service.PageProcess.PeekBlock() is Block.ILoopBlock) {
                Block block = service.PageProcess.PopBlock();
                service.PageProcess.JumpToIndex(block.EndIndex + 1);
            }
        }
    }
}