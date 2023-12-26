using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IScenarioPlayProcess  {
        /// <summary>
        /// ページへジャンプ
        /// </summary>
        UniTask PlayPageAsync(string pageName, CancellationToken cancellationToken = default);
    }
}