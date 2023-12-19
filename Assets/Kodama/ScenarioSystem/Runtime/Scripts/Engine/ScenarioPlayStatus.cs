using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPlayStatus {
        public AvailableScenario Available {get;}
        public Scenario Scenario => Available.Scenario;
        public Stack<PagePlayStatus> PagePlayStatusStack {get;} = new Stack<PagePlayStatus>();
        public PagePlayStatus CurrentPagePlayStatus => PagePlayStatusStack.Count > 0 ? PagePlayStatusStack.Peek() : null;
        public bool HasPageProcess => PagePlayStatusStack.Count > 0;
        public readonly List<VariableBase> Variables;
        
        public ScenarioPlayStatus(AvailableScenario availableScenario) {
            Available = availableScenario;
            PagePlayStatusStack.Push(new PagePlayStatus(Scenario.Pages[0]));
            Variables = Scenario?.Variables.Select(x => x.Copy()).ToList();
        }

        public void PushNewPageProcess(ScenarioPage page) {
            PagePlayStatusStack.Push(new PagePlayStatus(page));
        }

        public void SwitchToNewPageProcess(ScenarioPage page) {
            if(PagePlayStatusStack.Count > 0) {
                PagePlayStatusStack.Pop();
            }
            PagePlayStatusStack.Push(new PagePlayStatus(page));
        }

        public PagePlayStatus PopPageProcess() {
            return PagePlayStatusStack.Pop();
        }
    }
}