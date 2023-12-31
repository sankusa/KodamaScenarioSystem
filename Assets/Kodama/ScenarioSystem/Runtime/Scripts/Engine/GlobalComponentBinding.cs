using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [DefaultExecutionOrder(-1)]
    public class GlobalComponentBinding : MonoBehaviour {
        private static List<GlobalComponentBinding> _list = new List<GlobalComponentBinding>();
        public static IReadOnlyList<GlobalComponentBinding> List => _list;
        public static IEnumerable<Component> AllComponents => _list.SelectMany(x => x._components);

        // コンポーネントの参照登録
        [SerializeField] private List<Component> _components;
        public IReadOnlyList<Component> Components => _components;

        void Awake() {
            _list.Add(this);
        }

        void OnDestroy() {
            _list.Remove(this);
        }
    }
}