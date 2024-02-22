using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif

namespace Kodama.ScenarioSystem {
    public class AddressablesLoadScenarioAndPreloadCommand : AsyncCommandBase, IPreloadable {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
        [SerializeField] private AssetReferenceT<Scenario> _scenarioReference;
#endif

#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            if(_scenarioReference.IsValid()) return;

            await _scenarioReference.LoadAssetAsync();
            Scenario scenario = _scenarioReference.Asset as Scenario;
            scenario.PreloadResourcesAsyncWithReleaseOnError(this);
            if(scenario.CurrentPreloadState == Scenario.PreloadState.Preloading) {
                await UniTask.WaitUntil(
                    () => scenario.CurrentPreloadState == Scenario.PreloadState.Preloaded,
                    cancellationToken: cancellationToken
                );
            }
        }
#endif

        public async UniTask PreloadAsync() {
            await UniTask.CompletedTask;
        }

        public void Release() {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
            if(_scenarioReference.Asset != null) {
                Scenario scenario = _scenarioReference.Asset as Scenario;
                scenario.ReleaseResources(this);
                _scenarioReference.ReleaseAsset();
            }
#endif
        }

        public override string GetSummary() {
#if UNITY_EDITOR
    #if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
            SharedStringBuilder.Append(_scenarioReference.editorAsset?.name);
    #endif
#endif
            return SharedStringBuilder.Output();
        }

        public override string ValidateAsyncCommand() {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
    #if UNITY_EDITOR
        if(_scenarioReference.editorAsset == null) {
            SharedStringBuilder.Append("Scenario reference is null");
        }
    #endif
#else
    SharedStringBuilder.Append("Define symbol \"KODAMA_SCENARIO_ADDRESSABLE_SUPPORT\" required");
#endif
            return SharedStringBuilder.Output();
        }
    }
}