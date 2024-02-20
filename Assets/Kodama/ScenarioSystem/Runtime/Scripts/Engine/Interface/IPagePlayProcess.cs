using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface IPagePlayProcess {
        ScenarioPage SubsequentPage {set;}
        Scenario SubsequentScenario {set;}
        IReadOnlyList<ICallArg> SubsequentScenarioCallArgs {set;}
        bool SwitchRootProcessOnPlaySubsequentScenario {set;}
        Action OnNewRootProcessFinished {set;}
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
        void JumpToEndIndex();

        /// <summary>
        /// ラベルへジャンプ
        /// </summary>
        void JumpToLabel(string label);

        void JumpToBlockEnd(IBlockStart startBlock);

        /// <summary>
        /// VariableKeyをキーにして変数を取得
        /// </summary>
        Variable<T> FindVariable<T>(IVariableKey<T> variableKey);

        /// <summary>
        /// VariableKeyをキーにして変数を取得
        /// </summary>
        VariableBase FindVariable(IVariableKey variableKey);

        /// <summary>
        /// 型とIdをキーに変数を取得
        /// </summary>
        Variable<T> FindVariable<T>(string id);

        /// <summary>
        /// 型とIdをキーに変数を取得
        /// </summary>
        VariableBase FindVariable(Type targetType, string id);

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