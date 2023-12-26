using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class EndRepeatCommand : CommandBase, IBlockEnd {
        public string BlockType => "Repeat";
        
        public override void Execute(ICommandService service) {
            if(service.PagePlayProcess.PeekBlock() is RepeatBlock repeatBlock) {
                service.PagePlayProcess.JumpToIndex(repeatBlock.StartIndex);
            }
        }

        public override string GetSummary() {
            return "<color=orange><b>EndRepeat</b></color>";
        }
    }
}