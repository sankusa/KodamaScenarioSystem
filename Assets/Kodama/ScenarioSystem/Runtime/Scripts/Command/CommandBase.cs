using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// 全コマンドの基底クラス
    /// </summary>
    [Serializable]
    public abstract class CommandBase {
        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="service">利用可能な機能群</param>
        public virtual void Execute(ICommandService service) {}

        /// <summary>
        /// エディタ表示用のサマリ作成
        /// </summary>
        public virtual string GetSummary() {
            return $"<i><color=cyan>{GetType().Name}</color></i>";
        }
    }
}