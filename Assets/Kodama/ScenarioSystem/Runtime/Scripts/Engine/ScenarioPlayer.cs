using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using UnityEngine.Assertions;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPlayer : IDisposable {
        private readonly ServiceLocator _serviceLocator;

        private List<PlayProcess> _processes = new List<PlayProcess>();

        public bool IsPlaying => _processes.Any();
        public bool WaitingPreload => _processes.Where(x => x.WaitingPreload).Any();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ScenarioPlayer(ServiceLocator serviceLocator) {
            _serviceLocator = serviceLocator;
        }

        public void Dispose() {
            _cts.Cancel();
        }

        public void PlayScenario(string scenarioName, CancellationToken cancellationToken = default) {
            PlayScenarioAsync(scenarioName, cancellationToken).Forget();
        }

        public async UniTask PlayScenarioAsync(string scenarioName, CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _cts.Token)
                .Token;

            AvailableScenario available = ScenarioManager.Instance.FindAvailableByName(scenarioName);
            if(available == null) {
                Debug.Log($"Scenario \"{scenarioName}\" is not added.");
                return;
            }

            PlayProcess process = new PlayProcess(_serviceLocator, available);
            _processes.Add(process);
            
            try {
                await process.PlayAsync(linkedToken);
            }
            finally {
                _processes.Remove(process);
            }
        }
    }
}