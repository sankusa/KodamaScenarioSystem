using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class ComponentBinding : MonoBehaviour {
        // コンポーネントの参照登録
        [SerializeField] private List<Component> _components;
        public IReadOnlyList<Component> Components => _components;
    }
}