using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    [CreateAssetMenu(fileName = "New " + nameof(Scenario), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(Scenario))]
    public class Scenario : ScriptableObject {
        [SerializeField] private List<ScenarioPage> _pages;
        public IList<ScenarioPage> Pages => _pages;
        public ScenarioPage DefaultPage => _pages[0];

        [SerializeReference] private List<VariableBase> _variables;
        public IList<VariableBase> Variables => _variables;

        // Preload
        [NonSerialized] private static int _preloadingScenarioCount = 0;
        public static int PreloadingScenarioCount => _preloadingScenarioCount;

        public enum PreloadState {
            Unpreloaded,
            Preloading,
            Preloaded,
        }
        
        [NonSerialized] private PreloadState _preloadState = PreloadState.Unpreloaded;
        public PreloadState CurrentPreloadState => _preloadState;
        private CancellationTokenSource _preloadCts = new CancellationTokenSource();
        [NonSerialized] private List<object> _preloadKeys = new List<object>();

#if UNITY_EDITOR
        private const string _defaultPageName = "New Page";
        private const string _undoOperationName_CreatePage = "Create Page";
        private const string _undoOperationName_DestroyPage = "Destroy Page";
        private const string _undoOperationName_ChangeDefaultPage = "Change Default Page";

        [SerializeField] private Vector2 _graphPosition;
        public Vector2 GraphPosition {
            get => _graphPosition;
            set => _graphPosition = value;
        }
#endif

        // Preload
        public async UniTask PreloadResourcesAsync(object preloadKey) {
            _preloadingScenarioCount++;

            try {
                CancellationToken cancellationToken = _preloadCts.Token;
                cancellationToken.ThrowIfCancellationRequested();

                // 循環参照時の無限ループをブロック
                if(_preloadKeys.Contains(preloadKey)) return;

                _preloadKeys.Add(preloadKey);

                foreach(Scenario scenario in GetReferencingScenario()) {
                    await scenario.PreloadResourcesAsync(preloadKey);
                }

                // コマンドのPreload呼び出し
                if(_preloadState != PreloadState.Unpreloaded) return;

                _preloadState = PreloadState.Preloading;
                
                for(int i = 0; i < _pages.Count; i++) {
                    for(int j = 0; j < _pages[i].Commands.Count; j++) {
                        cancellationToken.ThrowIfCancellationRequested();
                        if(_pages[i].Commands[j] is IPreloadable preloadable)
                        await preloadable.PreloadAsync();
                    }
                }
                
                _preloadState = PreloadState.Preloaded;
            }
            finally {
                _preloadingScenarioCount--;
            }
        }

        public void PreloadResourcesAsyncWithReleaseOnError(object preloadKey) {
            PreloadResourcesAsync(preloadKey).Forget(e => {
                Debug.LogError(e);
                ReleaseResources(preloadKey);
            });
        }

        public void ReleaseResources(object preloadKey) {
            // 早期リターン+循環参照時の無限ループをブロック
            if(_preloadKeys.Contains(preloadKey) == false) return;

            _preloadKeys.Remove(preloadKey);
            foreach(Scenario scenario in GetReferencingScenario()) {
                scenario.ReleaseResources(preloadKey);
            }
            if(_preloadKeys.Count != 0) return;
            ForceReleaseResources();
        }

        public void ForceReleaseResources() {
            if(_preloadState == PreloadState.Unpreloaded) return;

            _preloadCts.Cancel();
            _preloadCts = new CancellationTokenSource();

            for(int i = 0; i < _pages.Count; i++) {
                for(int j = 0; j < _pages[i].Commands.Count; j++) {
                    if(_pages[i].Commands[j] is IPreloadable preloadable)
                    preloadable.Release();
                }
            }

            _preloadKeys.Clear();

            _preloadState = PreloadState.Unpreloaded;
        }

        public IEnumerable<Scenario> GetReferencingScenario() {
            return GetReferencingScenarioInternal().Distinct();
        }

        public IEnumerable<Scenario> GetReferencingScenarioInternal() {
            foreach(ScenarioPage page in _pages) {
                for(int i = 0; i < page.Commands.Count; i++) {
                    foreach(Scenario scenario in page.Commands[i].GetReferencingScenarios()) {
                        yield return scenario;
                    }
                }
            }
        }

        void OnDestroy() {
            ForceReleaseResources();
        }

#if UNITY_EDITOR
        public ScenarioPage CreatePage() {
            ScenarioPage newPage = CreateInstance<ScenarioPage>();
            newPage.name = ObjectNames.GetUniqueName(_pages.Select(x => x.name).ToArray(), _defaultPageName);
            newPage.ParentScenario = this;
            Undo.RegisterCreatedObjectUndo(newPage, _undoOperationName_CreatePage);
            Undo.RecordObject(this, _undoOperationName_CreatePage);
            AssetDatabase.AddObjectToAsset(newPage, this);
            Pages.Add(newPage);
            return newPage;
        }

        private void DestroyPage(ScenarioPage page) {
            Undo.RecordObject(this, _undoOperationName_DestroyPage);
            Pages.Remove(page);
            page.RemoveAndDestroyAllCommands();
            Undo.DestroyObjectImmediate(page);
        }

        public void DestroyPageAt(int index) {
            DestroyPage(Pages[index]);
        }

        public void ChangeDefaultPage(ScenarioPage page) {
            if(_pages.Contains(page) == false) {
                Debug.LogError("Scenario don't have the page.");
                return;
            }

            Undo.RecordObject(this, _undoOperationName_ChangeDefaultPage);
            _pages.Remove(page);
            _pages.Insert(0, page);
        }
#endif
        public string Validate() {
            foreach(ScenarioPage page in _pages) {
                SharedStringBuilder.AppendAsNewLine(page.Validate());
            }
            return SharedStringBuilder.Output();
        }
    }
}