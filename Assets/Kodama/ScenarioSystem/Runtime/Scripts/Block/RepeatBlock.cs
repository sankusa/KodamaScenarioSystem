using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class RepeatBlock : Block, Block.ILoopBlock {
        public RepeatCommand RepeatCommand {get;}
        public int Counter {get; set;}
        public RepeatBlock(RepeatCommand repeatCommand) {
            RepeatCommand = repeatCommand;
        }
        
    }
}