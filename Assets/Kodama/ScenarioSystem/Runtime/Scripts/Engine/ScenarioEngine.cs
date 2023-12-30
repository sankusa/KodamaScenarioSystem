using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;
using Codice.Client.Common;
using UnityEngine.AddressableAssets;
using Codice.Client.BaseCommands.BranchExplorer;

namespace Kodama.ScenarioSystem {
    public class ScenarioEngine : MonoBehaviour {
        // Awake時に自動でキャッシュに追加するシナリオ
        [SerializeField] private List<Scenario> _scenarios;
        [SerializeField] private bool _playFirstScenarioOnAwake;

        // 参照解決機能
        [SerializeField] private ComponentBinding _componentBinding;
        private ServiceLocator _serviceLocator;

        void Awake() {
            _serviceLocator = new ServiceLocator(_componentBinding);
            // PlayableScenarioを準備
            foreach(Scenario scenario in _scenarios) {
                PlayableScenarioManager.Instance.GetOrCreatePlayableScenario(scenario, this);
            }
            // 自動再生
            if(_playFirstScenarioOnAwake && _scenarios.Count > 0) {
                Play(_scenarios[0].name);
            }
        }

        void OnDestroy() {
            // PlayableScenarioを解放
            foreach(Scenario scenario in _scenarios) {
                PlayableScenarioManager.Instance.ReleasePlayableScenario(this);
            }
        }

        /// <summary>
        /// シナリオ非同期実行
        /// </summary>
        public async UniTask PlayAsync(Scenario scenario, CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            ).Token;

            linkedToken.ThrowIfCancellationRequested();

            var tmpReferenceSource = new LightReferenceSource();

            PlayableScenario playable = PlayableScenarioManager.Instance.GetOrCreatePlayableScenario(scenario, tmpReferenceSource);

            await ProcessManager.PlayNewProcessAsync(playable.Scenario, null, _serviceLocator,
                () => PlayableScenarioManager.Instance.ReleasePlayableScenario(tmpReferenceSource),
                linkedToken
            );
        }

        /// <summary>
        /// シナリオ同期実行
        /// </summary>
        public void Play(Scenario scenario, CancellationToken cancellationToken = default) =>
            PlayAsync(scenario, cancellationToken).ForgetAndLogException();

        /// <summary>
        /// 準備済みシナリオ非同期実行
        /// </summary>
        public async UniTask PlayAsync(string scenarioName, Action onProcessFinished = null, CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                this.GetCancellationTokenOnDestroy()
            ).Token;

            linkedToken.ThrowIfCancellationRequested();

            await ProcessManager.PlayNewProcessAsync(scenarioName, null, _serviceLocator, onProcessFinished, linkedToken);
        }
            
        /// <summary>
        /// 準備済みシナリオ同期実行
        /// </summary>
        public void Play(string scenarioName, Action onAllProcessCompletedOrCancelled = null, CancellationToken cancellationToken = default) =>
            PlayAsync(scenarioName, onAllProcessCompletedOrCancelled, cancellationToken).ForgetAndLogException();
    }
}