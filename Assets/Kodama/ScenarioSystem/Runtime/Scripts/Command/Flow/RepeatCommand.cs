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
            if(service.PagePlayProcess.PeekBlock() is RepeatBlock rp && rp.RepeatCommand == this) {
                repeatBlock = service.PagePlayProcess.PeekBlock() as RepeatBlock;
                repeatBlock.Counter++;
            }
            else {
                repeatBlock = new RepeatBlock(this);
                service.PagePlayProcess.SetUpAndPushBlock(this, repeatBlock);
            }

            if(repeatBlock.Counter >= _times) {
                service.PagePlayProcess.JumpToIndex(repeatBlock.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return $"<color={Colors.BlockSummaryCaption}>Repeat</color>  {_times.ToString()}";
        }
    }
}