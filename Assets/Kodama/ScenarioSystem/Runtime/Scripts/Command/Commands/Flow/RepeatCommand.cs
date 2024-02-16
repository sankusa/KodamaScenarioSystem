using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class RepeatCommand : CommandBase, IBlockStart {
        public string BlockType => "Repeat";

        [SerializeField, Min(0)] private int _times;

        public override void Execute(ICommandService service) {
            RepeatBlock repeatBlock;
            if(service.PageProcess.PeekBlock() is RepeatBlock rp && rp.RepeatCommand == this) {
                repeatBlock = service.PageProcess.PeekBlock() as RepeatBlock;
                repeatBlock.Counter++;
            }
            else {
                repeatBlock = new RepeatBlock(this);
                service.PageProcess.SetUpAndPushBlock(this, repeatBlock);
            }

            if(repeatBlock.Counter >= _times) {
                service.PageProcess.JumpToIndex(repeatBlock.EndIndex + 1);
            }
        }

        public override string GetSummary() {
            return $"{_times.ToString()} times";
        }
    }
}