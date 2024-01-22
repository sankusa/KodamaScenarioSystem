using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Kodama.ScenarioSystem {
    public class AsyncCommandBase : CommandBase {
        /// <summary>
        /// 待機するか
        /// </summary>
        [SerializeField] private AsyncCommandSetting _asyncCommandSetting = new AsyncCommandSetting();
        public AsyncCommandSetting AsyncCommandSetting => _asyncCommandSetting;

        /// <summary>
        /// 非同期コマンドの場合は処理内容は空。代わりにExcecuteAsyncを呼び出す。
        /// </summary>
        public sealed override void Execute(ICommandService service) {}

        /// <summary>
        /// 非同期コマンド実行
        /// </summary>
        /// <param name="service">利用可能な機能群</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns></returns>
        public virtual async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            await UniTask.CompletedTask;
        }

        public sealed override string Validate() {
            SharedStringBuilder.Append(_asyncCommandSetting.Validate(this, nameof(AsyncCommandSetting)));
            SharedStringBuilder.AppendAsNewLine(ValidateAsyncCommand());
            return SharedStringBuilder.Output();
        }

        public virtual string ValidateAsyncCommand() {
            return null;
        }
    }
}