using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ExitCommand : CommandBase {
        public override void Execute(ICommandService service) {
            service.PageProcess.JumpToEndIndex();
        }
    }
}