using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPlayProcess {
        private readonly RootPlayProcess _rootProcess;
        public RootPlayProcess RootProcess => _rootProcess;

        private readonly Scenario _scenario;
        public Scenario Scenario => _scenario;

        private readonly List<VariableBase> _variables;
        public IReadOnlyList<VariableBase> Variables => _variables;

        private readonly List<PagePlayProcess> _unfinishedPageProcesses = new List<PagePlayProcess>();

        private Action<ScenarioPlayProcess> _onAllPageProcessFinished;

        public ScenarioPlayProcess(RootPlayProcess rootProcess, Scenario scenario, Action<ScenarioPlayProcess> onAllPageProcessFinished, IReadOnlyList<ICallArg> args = null) {
            _rootProcess = rootProcess;
            _scenario = scenario;
            _variables = Scenario?.Variables.Select(x => x.Copy()).ToList();
            _onAllPageProcessFinished = onAllPageProcessFinished;

            if(args != null) {
                for(int i = 0; i < args.Count; i++) {
                    ICallArg arg = args[i];
                    VariableBase variable = _variables.Find(x => x.IsMatch(arg.TargetType, arg.VariableId));
                    if(variable == null) {
                        Debug.LogWarning($"Args[{i}]({arg.TargetType.Name})'s target variable not found");
                    }
                    else {
                        variable.SetValue(arg);
                    }
                }
            }
        }

        public PagePlayProcess CreatePageProcess(ScenarioPage page) {
            if(page == null) page = _scenario.DefaultPage;
            PagePlayProcess pageProcess = new PagePlayProcess(this, page, OnPageProcessFinished);
            _unfinishedPageProcesses.Add(pageProcess);
            return pageProcess;

            void OnPageProcessFinished(PagePlayProcess pageProcess) {
                _unfinishedPageProcesses.Remove(pageProcess);
                if(_unfinishedPageProcesses.Count == 0) {
                    _onAllPageProcessFinished?.Invoke(this);
                }
            }
        }

        public PagePlayProcess CreatePageProcess(string pageName) {
            ScenarioPage page = Scenario.Pages.FirstOrDefault(x => x.name == pageName);
            if(page == null) {
                Debug.LogWarning($"Page \"{pageName}\" not found.");
                return null;
            }
            return CreatePageProcess(page);
        }
    }
}