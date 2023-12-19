using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndIfCommand : CommandBase, IBlockEnd {
        public string BlockType => "If";
        
        public override void Execute(ICommandService service) {
            if(service.PlayProcess.PeekBlock() is IfBlock) {
                service.PlayProcess.PopBlock();
            }
        }

        public override string GetSummary() {
            return "<color=orange>EndIf</color>";
        }
    }
}