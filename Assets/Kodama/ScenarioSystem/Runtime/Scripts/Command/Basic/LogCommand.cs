using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LogCommand : CommandBase {
        [SerializeField] private string _message;
        public string Message => _message;

        public override void Execute(ICommandService service) {
            Debug.Log($"{LogCaption}<b>Scenario</b>[ {Page.Scenario.name} ]    <b>Page</b>[ {Page.name} ]    <b>Index</b>[ {Index.ToString()} ]\n{_message}", Page.Scenario);
        }

        public override string GetSummary() {
            return $"<b><color=#22BB22>ログ出力</color></b> : <color=#AAAAFF>{_message}</color>";
        }
    }
}