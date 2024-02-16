using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndIfCommand : CommandBase, IBlockEnd {
        public string BlockType => "If";
        
        public override void Execute(ICommandService service) {
            if(service.PageProcess.PeekBlock() is IfBlock) {
                service.PageProcess.PopBlock();
            }
        }
    }
}