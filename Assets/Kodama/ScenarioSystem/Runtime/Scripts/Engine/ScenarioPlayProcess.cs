using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.Profiling.Memory.Experimental;
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

        public ScenarioPlayProcess(RootPlayProcess rootProcess, Scenario scenario, Action<ScenarioPlayProcess> _onAllPageProcessFinished) {
            _rootProcess = rootProcess;
            _scenario = scenario;
            _variables = Scenario?.Variables.Select(x => x.Copy()).ToList();
        }

        public PagePlayProcess CreatePageProcess(ScenarioPage page) {
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
            ScenarioPage page = Scenario.Pages.FirstOrDefault(x => x.Name == pageName);
            if(page == null) {
                Debug.LogWarning($"Page \"{pageName}\" not found.");
                return null;
            }
            return CreatePageProcess(page);
        }
    }
}