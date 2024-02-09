using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class RootPlayProcess {
        private IServiceLocator _serviceLocator;
        public IServiceLocator ServiceLocator => _serviceLocator;

        private List<ScenarioPlayProcess> _unfinishedScenarioProcesses = new List<ScenarioPlayProcess>();

        private Action _onAllScenarioProcessFinished;

        public RootPlayProcess(IServiceLocator serviceLocator, Action onAllScenarioProcessFinished) {
            _serviceLocator = serviceLocator;
            _onAllScenarioProcessFinished = onAllScenarioProcessFinished;
        }

        public ScenarioPlayProcess CreateScenarioProcess(Scenario scenario) {
            ScenarioPlayProcess scenarioProcess = new ScenarioPlayProcess(this, scenario, OnScenarioProcessFinished);
            _unfinishedScenarioProcesses.Add(scenarioProcess);
            return scenarioProcess;

            void OnScenarioProcessFinished(ScenarioPlayProcess scenarioProcess) {
                _unfinishedScenarioProcesses.Remove(scenarioProcess);
                if(_unfinishedScenarioProcesses.Count == 0) {
                    _onAllScenarioProcessFinished?.Invoke();
                }
            }
        }
    }
}