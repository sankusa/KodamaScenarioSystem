using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IScenarioPlayerForCommand  {
        bool IsPaused {get;}
        void Pause();
        void Resume();

        /// <summary>
        /// ラベルへジャンプ
        /// </summary>
        void JumpToLabel(string label);

        /// <summary>
        /// ページへジャンプ
        /// </summary>
        void JumpToPage(string pageName, bool returnOnExit = false);

        /// <summary>
        /// 他のシナリオへジャンプ
        /// </summary>
        void JumpToScenario(string scenarioName, bool returnOnExit = false);

        /// <summary>
        /// 指定した型、名前の変数の値を取得
        /// </summary>
        T GetVariableValue<T>(string variableName);

        /// <summary>
        /// 指定した型、名前の変数の値を設定
        /// </summary>
        void SetVariableValue<T>(string variableName, T value);
    }
}