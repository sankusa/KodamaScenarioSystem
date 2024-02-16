using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ProcessManager {
        private static int _preloadWaitCounter = 0;
        public static bool WaitingPreload => _preloadWaitCounter > 0;

        internal static async UniTask PlayNewProcessAsync(Scenario scenario, ScenarioPage page, IServiceLocator serviceLocator, Action onProcessFinished, CancellationToken cancellationToken) {
            RootPlayProcess rootProcess = new RootPlayProcess(serviceLocator, onProcessFinished);
            ScenarioPlayProcess scenarioProcess = rootProcess.CreateScenarioProcess(scenario);
            PagePlayProcess pageProcess;
            if(page == null) {
                pageProcess = scenarioProcess.CreatePageProcess(scenario.DefaultPage);
            }
            else {
                pageProcess = scenarioProcess.CreatePageProcess(page);
            }          

            await CorePlayLoopAsync(pageProcess, cancellationToken);
        }

        // public static async UniTask PlayNewProcessAsync(string scenarioName, ScenarioPage page, IServiceLocator serviceLocator, Action onProcessFinished, CancellationToken cancellationToken= default) {
        //     Scenario scenario = PlayableScenarioManager.Instance.FindPlayableByName(scenarioName)?.Scenario;
        //     await PlayNewProcessAsync(scenario, page, serviceLocator, onProcessFinished, cancellationToken);
        // }

        internal static async UniTask PlayPageInSameScenarioProcessAsync(PagePlayProcess pageProcess, ScenarioPage page, CancellationToken cancellationToken) {
            PagePlayProcess newPageProcess = pageProcess.ScenarioProcess.CreatePageProcess(page);
            await CorePlayLoopAsync(newPageProcess, cancellationToken);
        }

        internal static async UniTask PlayScenarioInSameRootProcessAsync(PagePlayProcess pageProcess, Scenario scenario, ScenarioPage page, CancellationToken cancellationToken, IReadOnlyList<ICallArg> args = null) {
            PagePlayProcess newPageProcess = pageProcess.ScenarioProcess.RootProcess.CreateScenarioProcess(scenario, args).CreatePageProcess(page);
            await CorePlayLoopAsync(newPageProcess, cancellationToken);
        }

        // 実行関数の中核
        // ページプロセス再生＋後続処理の指定があればプロセスを生成して後続処理を行う。これを後続処理の指定が無くなるまで繰り返す
        private static async UniTask CorePlayLoopAsync(PagePlayProcess pageProcess, CancellationToken cancellationToken) {
            while(true) {
                // プリロード待機
                if(pageProcess.ScenarioProcess.Scenario.CurrentPreloadState == Scenario.PreloadState.Preloading) {
                    _preloadWaitCounter++;
                    try{
                        await UniTask.WaitUntil(() => pageProcess.ScenarioProcess.Scenario.CurrentPreloadState == Scenario.PreloadState.Preloaded, cancellationToken: cancellationToken);
                    }
                    finally {
                        _preloadWaitCounter--;
                    }
                }
                await pageProcess.PlayAsync(cancellationToken);

                // 後続処理用の情報を取り出し、後続処理が必要な場合はプロセスを生成してループを回す
                // 後続処理の必要が無ければbreak

                bool playSubsequent = false;
                ScenarioPlayProcess scenarioProcess = pageProcess.ScenarioProcess;
                RootPlayProcess rootProcess = scenarioProcess.RootProcess;
                // ルートプロセス切り替えの指定があれば、新規にルートプロセスを作成
                if(pageProcess.SwitchRootProcessOnPlaySubsequentScenario) {
                    rootProcess = new RootPlayProcess(pageProcess.ScenarioProcess.RootProcess.ServiceLocator, pageProcess.OnNewRootProcessFinished);
                }
                
                // シナリオが指定されていたら、ルートプロセスから新規にシナリオプロセスを生やす
                if(pageProcess.SubsequentScenario != null) {
                    Scenario subsequentScenario = pageProcess.SubsequentScenario;
                    scenarioProcess = rootProcess.CreateScenarioProcess(subsequentScenario, pageProcess.SubsequentScenarioCallArgs);

                    // ページの指定が無ければデフォルトを設定
                    if(pageProcess.SubsequentPage == null) {
                        pageProcess = scenarioProcess.CreatePageProcess(subsequentScenario.DefaultPage);
                    }
                    else {
                        pageProcess = scenarioProcess.CreatePageProcess(pageProcess.SubsequentPage);
                    }

                    playSubsequent = true;
                }

                // ページが指定されていたら、シナリオプロセスから新規にページプロセスを生やす
                if(pageProcess.SubsequentPage != null) {
                    pageProcess = scenarioProcess.CreatePageProcess(pageProcess.SubsequentPage);

                    playSubsequent = true;
                }

                if(playSubsequent == false) break;
            }
        }
    }
}