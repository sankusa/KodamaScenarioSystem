using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IScenarioEngine {
        bool IsPaused {get;}
        /// <summary>
        /// ポーズ
        /// </summary>
        void Pause();
        /// <summary>
        /// 再開
        /// </summary>
        void Resume();

        bool WaitingPreload{get;}

        /// <summary>
        /// 指定した型、名前の変数の値を取得
        /// </summary>
        T GetVariableValue<T>(string variableName);

        /// <summary>
        /// 指定した型、名前の変数の値を設定
        /// </summary>
        void SetVariableValue<T>(string variableName, T value);
        
        /// <summary>
        /// 参照解決
        /// </summary>
        T Resolve<T>();

        /// <summary>
        /// 参照解決(全件)
        /// </summary>
        IEnumerable<T> ResolveAll<T>();
    }
}