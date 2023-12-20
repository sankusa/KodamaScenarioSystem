using System.Collections;
using System.Collections.Generic;
using log4net.Util;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class RepeatCommand : CommandBase, IBlockStart {
        public string BlockType => "Repeat";

        [SerializeField, Min(0)] private int _times;

        public override void Execute(ICommandService service) {
            RepeatBlock repeatBlock;
            if(service.PlayProcess.PeekBlock() is RepeatBlock rp && rp.RepeatCommand == this) {
                repeatBlock = service.PlayProcess.PeekBlock() as RepeatBlock;
                repeatBlock.Counter++;
            }
            else {
                repeatBlock = new RepeatBlock(this);
                service.PlayProcess.SetUpAndPushBlock(this, repeatBlock);
            }

            if(repeatBlock.Counter >= _times) {
                service.PlayProcess.JumpToIndex(repeatBlock.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return $"<color=orange><b>Repeat</b></color>  {_times.ToString()}";
        }
    }
}