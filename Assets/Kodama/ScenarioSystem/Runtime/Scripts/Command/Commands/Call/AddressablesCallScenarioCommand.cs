using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif

namespace Kodama.ScenarioSystem {
    public class AddressablesCallScenarioCommand : AsyncCommandBase {
        private enum CallType {
            Jump = 0,
            Await = 1,
            Async = 2,
        }

#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
        [SerializeField] private AssetReferenceT<Scenario> _target;
        public AssetReferenceT<Scenario> Target => _target;
#endif
        [SerializeField] private CallType _callType;
        [SerializeReference] private List<CallArg> _scenarioArgs;
        public List<CallArg> ScenarioArgs => _scenarioArgs;

        public async override UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
            AsyncOperationHandle<Scenario> handle = Addressables.LoadAssetAsync<Scenario>(_target);
            await handle.ToUniTask();
            Scenario scenario = handle.Result;
            TemporaryPreloadKey preloadKey = new TemporaryPreloadKey();
            scenario.PreloadResourcesAsyncWithReleaseOnError(preloadKey);

            switch (_callType) {
                case CallType.Jump:
                    service.PageProcess.SubsequentScenario = scenario;
                    service.PageProcess.SubsequentScenarioCallArgs = _scenarioArgs;
                    service.PageProcess.SwitchRootProcessOnPlaySubsequentScenario = true;
                    service.PageProcess.OnNewRootProcessFinished = () => {
                            scenario.ReleaseResources(preloadKey);
                            Addressables.Release(handle);
                        };
                    service.PageProcess.JumpToEndIndex();
                    break;

                case CallType.Await:
                    await ProcessManager.PlayScenarioAsNewRootProcessAsync(
                        service.PageProcess as PagePlayProcess,
                        scenario,
                        null,
                        cancellationToken,
                        _scenarioArgs,
                        () => {
                            scenario.ReleaseResources(preloadKey);
                            Addressables.Release(handle);
                        }
                    );
                    break;

                case CallType.Async:
                    ProcessManager.PlayScenarioAsNewRootProcessAsync(
                        service.PageProcess as PagePlayProcess,
                        scenario,
                        null,
                        cancellationToken,
                        _scenarioArgs,
                        () => {
                            scenario.ReleaseResources(preloadKey);
                            Addressables.Release(handle);
                        }
                    )
                    .ForgetAndLogException();
                    break;
            }

            // アセットチャーン回避のため、プリロードの終了を待ってからReturn
            if(scenario.CurrentPreloadState == Scenario.PreloadState.Preloading) {
                await UniTask.WaitUntil(
                    () => scenario.CurrentPreloadState == Scenario.PreloadState.Preloaded,
                    cancellationToken: cancellationToken
                );
            }
#else
            await UniTask.CompletedTask;
#endif
        }

        public override string GetSummary() {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
            SharedStringBuilder.Append(_target.editorAsset?.name);
            SharedStringBuilder.Append(",  ");
            SharedStringBuilder.Append(_callType.ToString());
#endif
            return SharedStringBuilder.Output();
        }

        public override string ValidateAsyncCommand() {
#if KODAMA_SCENARIO_ADDRESSABLE_SUPPORT
    #if UNITY_EDITOR
            if(_target.editorAsset == null) {
                SharedStringBuilder.Append("Target is null");
            }
            else {
                if(_scenarioArgs.Count > 0 && _target.editorAsset == null) {
                    SharedStringBuilder.AppendAsNewLine("ScenarioArgs : MissingReference");
                }
                else {
                    for(int i = 0; i < _scenarioArgs.Count; i++) {
                        if(_target.editorAsset.Variables.FirstOrDefault(x => x.IsMatch(_scenarioArgs[i].TargetType, _scenarioArgs[i].VariableId)) == null) {
                            SharedStringBuilder.AppendAsNewLine("ScenarioArgs[");
                            SharedStringBuilder.Append(i.ToString());
                            SharedStringBuilder.Append("] Missing Reference");
                        }
                    }
                }
            }
    #endif
#else
    SharedStringBuilder.Append("Define symbol \"KODAMA_SCENARIO_ADDRESSABLE_SUPPORT\" required");
#endif
            return SharedStringBuilder.Output();
        }
    }
}