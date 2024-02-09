using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;

namespace Kodama.ScenarioSystem {
    public class ScenarioEngine : MonoBehaviour {
        // Awake時に自動でキャッシュに追加するシナリオ
        [SerializeField] private List<Scenario> _scenarios;
        [SerializeField] private bool _playFirstScenarioOnAwake;

        // 参照解決機能
        [SerializeField] private ComponentBinding _componentBinding;

        void Awake() {
            // PlayableScenarioを準備
            foreach(Scenario scenario in _scenarios) {
                scenario.PreloadResourcesAsyncWithReleaseOnError(this);
            }
            // 自動再生
            if(_playFirstScenarioOnAwake && _scenarios.Count > 0) {
                Play(_scenarios[0]);
            }
        }

        void OnDestroy() {
            // PlayableScenarioを解放
            foreach(Scenario scenario in _scenarios) {
                scenario.ReleaseResources(this);
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

            var tmpPreloadKey = new TemporaryPreloadKey();

            scenario.PreloadResourcesAsyncWithReleaseOnError(tmpPreloadKey);

            ServiceLocator serviceLocator = new ServiceLocator(_componentBinding);
            await ProcessManager.PlayNewProcessAsync(scenario, null, serviceLocator,
                () => scenario.ReleaseResources(tmpPreloadKey),
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

            Scenario scenario = _scenarios.Find(x => x.name == scenarioName);
            Assert.IsNotNull(scenario);

            ServiceLocator serviceLocator = new ServiceLocator(_componentBinding);
            await ProcessManager.PlayNewProcessAsync(scenario, null, serviceLocator, onProcessFinished, linkedToken);
        }
            
        /// <summary>
        /// 準備済みシナリオ同期実行
        /// </summary>
        public void Play(string scenarioName, Action onAllProcessCompletedOrCancelled = null, CancellationToken cancellationToken = default) =>
            PlayAsync(scenarioName, onAllProcessCompletedOrCancelled, cancellationToken).ForgetAndLogException();
    }
}