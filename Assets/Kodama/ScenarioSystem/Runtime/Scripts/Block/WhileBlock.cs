using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class WhileBlock : Block, Block.ILoopBlock {
        public WhileCommand WhileCommand {get;}
        public WhileBlock(WhileCommand whileCommand) {
            WhileCommand = whileCommand;
        }
    }
}