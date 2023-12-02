using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class JumpToPageCommand : CommandBase {
        [SerializeField] private string _pageName;
        [SerializeField] private bool _returnOnExit;

        public override void Execute(ICommandService service) {
            service.Player.JumpToPage(_pageName, _returnOnExit);
        }
    }
}