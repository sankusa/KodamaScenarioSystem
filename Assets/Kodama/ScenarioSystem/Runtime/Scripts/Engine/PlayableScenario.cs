using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kodama.ScenarioSystem {
    internal class PlayableScenario : IDisposable {
        private enum PreloadState {
            Yet,
            Preloading,
            Completed,
        }

        public Scenario Scenario {get; set;}

        private PreloadState _preloadState = PreloadState.Yet;
        private int _waitingPreloadCounter;
        public bool WaitingPreload => _waitingPreloadCounter > 0;
        
        private CancellationTokenSource _cts {get;} = new CancellationTokenSource();

        private event Action _onDispose;

        private bool _isDisposed = false;
        public bool IsDisposed => _isDisposed;

        public PlayableScenario(Scenario scenario) {
            Assert.IsNotNull(scenario);
            Scenario = scenario;
            PreloadResourcesAsync().Forget(e => {
                Debug.LogError(e);
                Dispose();
            });
        }

        public void Dispose() {
            if(_isDisposed) return;

            _cts.Cancel();
            ReleaseResources();
            _onDispose?.Invoke();

            _onDispose = null;
            _isDisposed = true;
        }

        public void AddListenerToOnDispose(Action onDispose) {
            if(_isDisposed) {
                onDispose?.Invoke();
                return;
            }

            _onDispose += onDispose;
        }

        private async UniTask PreloadResourcesAsync() {
            _cts.Token.ThrowIfCancellationRequested();

            if(_preloadState != PreloadState.Yet) return;

            _preloadState = PreloadState.Preloading;
            
            foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                if(command is IPreloadable preloadable) {
                    _cts.Token.ThrowIfCancellationRequested();
                    await preloadable.PreloadAsync();
                }
            }
            
            _preloadState = PreloadState.Completed;
        }

        private void ReleaseResources() {
            foreach(CommandBase command in Scenario.Pages.SelectMany(x => x.Commands)) {
                if(command is IPreloadable preloadable) {
                    preloadable.Release();
                }
            }
        }
    }
}