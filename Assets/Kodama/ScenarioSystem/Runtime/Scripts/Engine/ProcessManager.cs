using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class ProcessManager {
        public static async UniTask PlayNewProcessAsync(Scenario scenario, ScenarioPage page, IServiceLocator serviceLocator, Action onProcessFinished, CancellationToken cancellationToken) {
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

        public static async UniTask PlayNewProcessAsync(string scenarioName, ScenarioPage page, IServiceLocator serviceLocator, Action onProcessFinished, CancellationToken cancellationToken= default) {
            Scenario scenario = PlayableScenarioManager.Instance.FindPlayableByName(scenarioName)?.Scenario;
            await PlayNewProcessAsync(scenario, page, serviceLocator, onProcessFinished, cancellationToken);
        }

        public static async UniTask PlayPageInSameScenarioProcessAsync(PagePlayProcess pageProcess, ScenarioPage page, CancellationToken cancellationToken) {
            PagePlayProcess newPageProcess = pageProcess.ScenarioProcess.CreatePageProcess(page);
            await CorePlayLoopAsync(newPageProcess, cancellationToken);
        }

        public static async UniTask PlayScenarioInSameRootProcessAsync(PagePlayProcess pageProcess, Scenario scenario, string pageName, CancellationToken cancellationToken) {
            PagePlayProcess newPageProcess = pageProcess.ScenarioProcess.RootProcess.CreateScenarioProcess(scenario).CreatePageProcess(pageName);
            await CorePlayLoopAsync(newPageProcess, cancellationToken);
        }

        public static async UniTask PlayScenarioInSameRootProcessAsync(PagePlayProcess pageProcess, string scenarioName, string pageName, CancellationToken cancellationToken) {
            Scenario scenario = PlayableScenarioManager.Instance.FindPlayableByName(scenarioName)?.Scenario;
            await PlayScenarioInSameRootProcessAsync(pageProcess, scenario, pageName, cancellationToken);
        }

        // 実行関数の中核
        // ページプロセス再生＋後続処理の指定があればプロセスを生成して後続処理を行う。これを後続処理の指定が無くなるまで繰り返す
        private static async UniTask CorePlayLoopAsync(PagePlayProcess pageProcess, CancellationToken cancellationToken) {
            while(true) {
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
                if(string.IsNullOrEmpty(pageProcess.SubsequentScenarioName) == false) {
                    Scenario subsequentScenario = PlayableScenarioManager.Instance.FindPlayableByName(pageProcess.SubsequentScenarioName).Scenario;
                    scenarioProcess = rootProcess.CreateScenarioProcess(subsequentScenario);

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