using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;
using Codice.Client.Common;
using UnityEngine.AddressableAssets;

namespace Kodama.ScenarioSystem {
    public class ScenarioEngine : MonoBehaviour {
        // Awake時に自動でキャッシュに追加するシナリオ
        [SerializeField] private List<Scenario> _scenarios;
        [SerializeField] private bool _playFirstScenarioOnAwake;

        // 参照解決機能
        [SerializeField] private ComponentBinding _componentBinding;
        private ServiceLocator _serviceLocator;

        // シナリオ再生機能
        private ScenarioPlayer _scenarioPlayer;

        public bool WaitingPreload => _scenarioPlayer.WaitingPreload;
        public bool IsPlaying => _scenarioPlayer.IsPlaying;

        void Awake() {
            _serviceLocator = new ServiceLocator(_componentBinding);
            _scenarioPlayer = new ScenarioPlayer(_serviceLocator);
            // 初期アタッチ済みシナリオをキャッシュに追加
            foreach(Scenario scenario in _scenarios) {
                AddScenario(scenario, false);
            }
            // 自動再生
            if(_playFirstScenarioOnAwake && _scenarios.Count > 0) {
                _scenarioPlayer.PlayScenario(_scenarios[0].name);
            }
        }

        void OnDestroy() {
            _scenarioPlayer.Dispose();
            ScenarioManager.Instance.RemoveAll();
        }

        /// <summary>
        /// シナリオの追加
        /// </summary>
        /// <param name="scenario">シナリオ</param>
        /// <param name="removeOnExitScenario">シナリオ終了時に自動でRemoveするか</param>
        /// <param name="onRemove">Removeコールバック</param>
        public void AddScenario(Scenario scenario, bool removeOnExitScenario = false, Action onRemove = null) =>
            ScenarioManager.Instance.Add(scenario, removeOnExitScenario, onRemove);

        /// <summary>
        /// シナリオを外す
        /// </summary>
        /// <param name="scenario"></param>
        public void RemoveScenario(Scenario scenario) => ScenarioManager.Instance.Remove(scenario);

        /// <summary>
        /// シナリオ非同期実行
        /// </summary>
        public async UniTask PlayScenarioAsync(string scenarioName, CancellationToken cancellationToken = default) =>
            await _scenarioPlayer.PlayScenarioAsync(scenarioName, cancellationToken);

        /// <summary>
        /// シナリオ同期実行
        /// </summary>
        public void PlayScenario(string scenarioName, CancellationToken cancellationToken = default) =>
            _scenarioPlayer.PlayScenario(scenarioName, cancellationToken);
    }
}