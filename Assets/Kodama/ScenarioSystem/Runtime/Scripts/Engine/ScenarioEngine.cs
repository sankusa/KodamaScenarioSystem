using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using System.Threading;
#endif

namespace Kodama.ScenarioSystem {
    public class ScenarioEngine : MonoBehaviour, IScenarioEngine, IScenarioEngineForInternal {
        [SerializeField] private List<Scenario> _scenarios;
        [SerializeField] private bool _playOnAwake;
        /// <summary>
        /// コンポーネントの参照登録
        /// </summary>
        [SerializeField] private List<Component> _componentBindings;
        
        private Scenario _currentScenario;
        private int _currentPageIndex;
        private int _currentComamandIndex;

        private bool _isPlaying;
        public bool IsPlaying => _isPlaying;

        private bool _isPaused;

        void Awake() {
            if(_currentScenario == null && _scenarios.Count > 0) {
                _currentScenario = _scenarios[0];
            }
            if(_playOnAwake) {
                Play();
            }
        }

        /// <summary>
        /// 同期実行
        /// </summary>
#if KODAMA_UNITASK_SUPPORT
        public void Play(CancellationToken cancellationToken = default) {
            PlayAsync().Forget();
        }
#else
        public void Play() {
            StartCoroutine(Run());
        }
#endif

        /// <summary>
        /// ラベルから同期実行
        /// </summary>
        /// <param name="label"></param>
        public void Play(string label) {
            JumpToLabel(label);
            Play();
        }

        /// <summary>
        /// UniTaskを利用しない場合の実行関数
        /// </summary>
        private IEnumerator Run() {
            _isPlaying = true;
            _isPaused = false;

            while(_isPlaying) {
                if(_currentScenario == null) break;
                if(_currentPageIndex >= _currentScenario.Pages.Count) break;
                if(_currentComamandIndex >= _currentScenario.Pages[_currentPageIndex].Commands.Count) break;
                
                CommandBase command = _currentScenario.Pages[_currentPageIndex].Commands[_currentComamandIndex];

                command.Execute(this);

                yield return new WaitUntil(() => _isPaused == false);

                _currentComamandIndex++;
            }

            _isPlaying = false;
        }

#if KODAMA_UNITASK_SUPPORT
        /// <summary>
        /// ラベルから非同期実行
        /// </summary>
        /// <param name="label"></param>
        public async UniTask PlayAsync(string label, CancellationToken cancellationToken = default) {
            JumpToLabel(label);
            await PlayAsync(cancellationToken);
        }

        /// <summary>
        /// 非同期実行
        /// </summary>
        public async UniTask PlayAsync(CancellationToken cancellationToken = default) {
            CancellationToken linkedToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy())
                .Token;

            _isPlaying = true;
            _isPaused = false;

            while(_isPlaying) {
                if(_currentScenario == null) break;
                if(_currentPageIndex >= _currentScenario.Pages.Count) break;
                if(_currentComamandIndex >= _currentScenario.Pages[_currentPageIndex].Commands.Count) break;
                
                CommandBase command = _currentScenario.Pages[_currentPageIndex].Commands[_currentComamandIndex];

                if (command is AsyncCommandBase asyncCommand) {
                    if (asyncCommand.Wait) {
                        await asyncCommand.ExecuteAsync(this, linkedToken);
                    }
                    else {
                        asyncCommand.ExecuteAsync(this, linkedToken).Forget();
                    }
                }
                else {
                    command.Execute(this);
                }

                // ポーズ中なら中断
                await UniTask.WaitUntil(() => _isPaused == false, cancellationToken: linkedToken);

                _currentComamandIndex++;
            }

            _isPlaying = false;
        }
#endif

        /// <summary>
        /// ラベルへジャンプ
        /// </summary>
        private void JumpToLabel(string targetLabel) {
            // まず現在のページを調べる
            for(int i = 0; i < _currentScenario.Pages[_currentPageIndex].Commands.Count; i++) {
                LabelCommand labelCommand = _currentScenario.Pages[_currentPageIndex].Commands[i] as LabelCommand;
                if(labelCommand != null && labelCommand.Label == targetLabel) {
                    _currentComamandIndex = i;
                    return;
                }
            }
            // 現在のシナリオを調べる
            for(int i = 0; i < _currentScenario.Pages.Count; i++) {
                for(int j = 0; j < _currentScenario.Pages[i].Commands.Count; j++) {
                    LabelCommand labelCommand = _currentScenario.Pages[i].Commands[j] as LabelCommand;
                    if(labelCommand != null && labelCommand.Label == targetLabel) {
                        _currentPageIndex = i;
                        _currentComamandIndex = j;
                        return;
                    }
                }
            }
            // 保持しているシナリオを全件調べる
            for(int i = 0; i < _scenarios.Count; i++) {
                for(int j = 0; j < _scenarios[i].Pages.Count; j++) {
                    for(int k = 0; k < _scenarios[i].Pages[j].Commands.Count; k++) {
                        LabelCommand labelCommand = _scenarios[i].Pages[j].Commands[k] as LabelCommand;
                        if(labelCommand != null && labelCommand.Label == targetLabel) {
                            _currentScenario = _scenarios[i];
                            _currentPageIndex = j;
                            _currentComamandIndex = k;
                            return;
                        }
                    }
                }
            }
            Debug.LogWarning($"{nameof(JumpToLabel)} : target label not found.");
        }

        /// <summary>
        /// ラベルへジャンプ(内部公開用)
        /// </summary>
        void IScenarioEngineForInternal.JumpToLabel(string targetLabel) {
            JumpToLabel(targetLabel);
        }

        /// <summary>
        /// 参照解決
        /// </summary>
        public T Resolve<T>() {
            return _componentBindings.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// 参照解決(全件)
        /// </summary>
        public IEnumerable<T> ResolveAll<T>() {
            return _componentBindings.OfType<T>();
        }
    }
}