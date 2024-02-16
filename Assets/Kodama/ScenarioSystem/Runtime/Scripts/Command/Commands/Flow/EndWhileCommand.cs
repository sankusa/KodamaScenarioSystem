using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndWhileCommand : CommandBase, IBlockEnd {
        public string BlockType => "While";
        
        public override void Execute(ICommandService service) {
            if(service.PageProcess.PeekBlock() is WhileBlock whileBlock) {
                service.PageProcess.JumpToIndex(whileBlock.StartIndex);
            }
        }
    }
}