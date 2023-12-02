using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class JumpToScnarioCommand : CommandBase {
        [SerializeField] private string _scenarioName;
        [SerializeField] private bool _returnOnExit;
        public override void Execute(ICommandService service) {
            service.Player.JumpToScenario(_scenarioName, _returnOnExit);
        }
    }
}