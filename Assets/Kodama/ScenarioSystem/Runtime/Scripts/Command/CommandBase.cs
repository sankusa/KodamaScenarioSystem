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
        /// <param name="engine">シナリオエンジン</param>
        public virtual void Execute(IScenarioEngine engine) {}

        /// <summary>
        /// エディタ表示用のサマリ作成
        /// </summary>
        public virtual string GetSummary() {
            return "";
        }
    }
}