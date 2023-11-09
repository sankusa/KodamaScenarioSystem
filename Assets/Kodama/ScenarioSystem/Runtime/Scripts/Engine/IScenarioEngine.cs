using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IScenarioEngine {
        /// <summary>
        /// 参照解決
        /// </summary>
        public T Resolve<T>();

        /// <summary>
        /// 参照解決(全件)
        /// </summary>
        public IEnumerable<T> ResolveAll<T>();
    }
}