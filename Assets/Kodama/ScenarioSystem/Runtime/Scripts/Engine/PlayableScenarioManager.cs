using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {


    // 一時的なReferenceSource用
    public class LightReferenceSource {}
    
    internal class PlayableScenarioManager {
        // Playableシナリオ参照管理クラス
        private class PlayableScenarioHolder {
            public PlayableScenario Playable {get;}
            private List<object> _referenceSources {get;} = new List<object>();

            public PlayableScenarioHolder(PlayableScenario playable, object referenceSource) {
                Playable = playable;
                _referenceSources.Add(referenceSource);
            }

            public void AddReferenceSources(object referenceSource) {
                _referenceSources.Add(referenceSource);
            }

            public void Release(object referenceSource) {
                _referenceSources.Remove(referenceSource);
                if(_referenceSources.Count == 0) {
                    Playable.Dispose();
                }
            }
        }
    
        private static PlayableScenarioManager _instance;
        public static PlayableScenarioManager Instance {
            get {
                if(_instance == null) {
                    _instance = new PlayableScenarioManager();
                }
                return _instance;
            }
        }

        private List<PlayableScenarioHolder> _holders {get;} = new List<PlayableScenarioHolder>();
        public IEnumerable<PlayableScenario> Playables => _holders.Select(x => x.Playable);

        public PlayableScenario FindPlayable(Scenario scenario) => Playables.FirstOrDefault(x => x.Scenario == scenario);
        public PlayableScenario FindPlayableByName(string scenarioName) => Playables.FirstOrDefault(x => x.Scenario.name == scenarioName);

        public PlayableScenario GetOrCreatePlayableScenario(Scenario scenario, object referenceSource) {
            PlayableScenarioHolder holder = _holders.Find(x => x.Playable.Scenario == scenario);
            if(holder == null) {
                PlayableScenario playable = new PlayableScenario(scenario);
                holder = new PlayableScenarioHolder(playable, referenceSource);
                _holders.Add(holder);
                playable.AddListenerToOnDispose(() =>_holders.Remove(holder));
            }
            else {
                holder.AddReferenceSources(referenceSource);
            }
            return holder.Playable;
        }

        public void ReleasePlayableScenario(object referenceSource) {
            for(int i = _holders.Count - 1; i >= 0; i--) {
                _holders[i].Release(referenceSource);
            }
        }
    }
}