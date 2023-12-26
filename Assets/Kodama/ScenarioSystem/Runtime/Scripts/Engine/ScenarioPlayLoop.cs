using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class ScenarioPlayLoop {
        public static async UniTask PlayAsync(Scenario scenario, string pageName, IServiceLocator serviceLocator, Action onProcessFinished, PagePlayProcess pageProcess = null, CancellationToken cancellationToken= default) {
            RootPlayProcess rootProcess;
            ScenarioPlayProcess scenarioProcess;
            


            // ページプロセスから呼び出されていたら、親プロセスからページプロセスを作る。
            if(pageProcess == null) {
                rootProcess = new RootPlayProcess(serviceLocator, () => onProcessFinished?.Invoke());
                scenarioProcess = rootProcess.CreateScenarioProcess(scenario);
                if(string.IsNullOrEmpty(pageName)) {
                    pageProcess = scenarioProcess.CreatePageProcess(scenario.DefaultPage);
                }
                else {
                    pageProcess = scenarioProcess.CreatePageProcess(scenario.Pages.FirstOrDefault(x => x.Name == pageName));
                }
            }
            else {
                rootProcess = pageProcess.ScenarioProcess.RootProcess;
                if(scenario != null) {
                    scenarioProcess = rootProcess.CreateScenarioProcess(scenario);
                }
                else {
                    scenarioProcess = pageProcess.ScenarioProcess;
                }
                pageProcess = scenarioProcess.CreatePageProcess(pageName);
            }
            

            while(true) {
                await pageProcess.PlayAsync(cancellationToken);
                
                // 後続処理準備
                bool doPlaySubsequent = false;
                if(pageProcess.SwitchRootProcessOnPlaySubsequentScenario) {
                    rootProcess = new RootPlayProcess(serviceLocator, pageProcess.OnNewRootProcessFinished);

                    doPlaySubsequent = true;
                }
                if(string.IsNullOrEmpty(pageProcess.SubsequentScenarioName) == false) {
                    Scenario subsequentScenario = PlayableScenarioManager.Instance.FindPlayableByName(pageProcess.SubsequentScenarioName).Scenario;
                    scenarioProcess = rootProcess.CreateScenarioProcess(subsequentScenario);

                    if(string.IsNullOrEmpty(pageProcess.SubsequentPageName)) {
                        pageProcess = scenarioProcess.CreatePageProcess(subsequentScenario.DefaultPage);
                    }

                    doPlaySubsequent = true;
                }
                if(string.IsNullOrEmpty(pageProcess.SubsequentPageName) == false) {
                    pageProcess = scenarioProcess.CreatePageProcess(pageProcess.SubsequentPageName);

                    doPlaySubsequent = true;
                }

                if(doPlaySubsequent == false) break;
            }
        }

        public static async UniTask PlayAsync(string scenarioName, string pageName, IServiceLocator serviceLocator, Action onProcessFinished, PagePlayProcess pageProcess = null, CancellationToken cancellationToken= default) {
            Scenario scenario = PlayableScenarioManager.Instance.FindPlayableByName(scenarioName)?.Scenario;

            await PlayAsync(scenario, pageName, serviceLocator, onProcessFinished, pageProcess, cancellationToken);
        }
    }
}