using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IPlayProcess  {
        bool IsPaused {get;}
        /// <summary>
        /// 一時停止
        /// </summary>
        void Pause();
        /// <summary>
        /// 再開
        /// </summary>
        void Resume();

        /// <summary>
        /// インデックスへジャンプ
        /// </summary>
        void JumpToIndex(int index);

        /// <summary>
        /// ラベルへジャンプ
        /// </summary>
        void JumpToLabel(string label);

        void JumpToBlockEnd(IBlockStart startBlock);

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
        /// 指定した型、名前の変数の値を取得
        /// </summary>
        object GetVariableValue(Type variableType, string variableName);

        /// <summary>
        /// 指定した型、名前の変数の値を設定
        /// </summary>
        void SetVariableValue<T>(string variableName, T value);

        /// <summary>
        /// 指定した型、名前の変数の値を設定
        /// </summary>
        void SetVariableValue(Type variableType, string variableName, object value);

        /// <summary>
        /// ブロックを表すインスタンスをプッシュ
        /// </summary>
        /// <param name="block"></param>
        public void SetUpAndPushBlock(IBlockStart blockStart, Block block);

        /// <summary>
        /// ブロックを表すインスタンスをポップ
        /// </summary>
        /// <param name="block"></param>
        public Block PopBlock();

        /// <summary>
        /// ブロックを表すインスタンスをピーク
        /// </summary>
        /// <param name="block"></param>
        public Block PeekBlock();
    }
}