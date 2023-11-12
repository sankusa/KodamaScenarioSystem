using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if KODAMA_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// 非同期タスクコマンド基底クラス
    /// </summary>
    [Serializable]
    public abstract class UniTaskCommandBase : CommandBase {
        /// <summary>
        /// 待機するか
        /// </summary>
        [SerializeField] private bool _wait = true;
        public bool Wait => _wait;
        /// <summary>
        /// 非同期コマンドの場合は処理内容は空。代わりにExcecuteAsyncを呼び出す。
        /// </summary>
        public sealed override void Execute(IScenarioEngine engine) {}

        /// <summary>
        /// 非同期コマンド実行
        /// </summary>
        /// <param name="engine">シナリオエンジン</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns></returns>
        public virtual async UniTask ExecuteAsync(IScenarioEngine engine, CancellationToken cancellationToken) {
            await UniTask.CompletedTask;
        }
    }
}
#endif