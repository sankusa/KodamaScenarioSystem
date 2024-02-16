using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndRepeatCommand : CommandBase, IBlockEnd {
        public string BlockType => "Repeat";
        
        public override void Execute(ICommandService service) {
            if(service.PageProcess.PeekBlock() is RepeatBlock repeatBlock) {
                service.PageProcess.JumpToIndex(repeatBlock.StartIndex);
            }
        }
    }
}