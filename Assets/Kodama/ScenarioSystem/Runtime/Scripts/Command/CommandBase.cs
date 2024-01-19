using System;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// コマンド基底クラス
    /// </summary>
    [Serializable]
    public abstract class CommandBase {
        public static CommandBase CreateInstance(Type commandType, ScenarioPage page) {
            CommandBase command = Activator.CreateInstance(commandType, page) as CommandBase;
            command._page = page;
            return command;
        }

        [SerializeField, HideInInspector] private ScenarioPage _page;
        public ScenarioPage Page => _page;

        public int Index => Page.IndexOf(this);

        // CreateInstance以からのインスタンス化は禁止
        protected CommandBase() {}

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="service">利用可能な機能群</param>
        public virtual void Execute(ICommandService service) {}

        /// <summary>
        /// エディタ表示用のサマリ作成
        /// </summary>
        public virtual string GetSummary() {
            return null;
        }

        /// <summary>
        /// バリデーション
        /// </summary>
        /// <returns>エラーメッセージ</returns>
        public virtual string Validate() {
            return null;
        }

        public string LogCaption => $"<b><i><color={Colors.CommandNameColor}>{GetType().Name}</color></i></b>    ";
        public string LogHeader => $"{LogCaption}<b>Scenario</b>[ {Page.Scenario.name} ]    <b>Page</b>[ {Page.name} ]    <b>Index</b>[ {Index.ToString()} ]";

        public CommandBase Copy() {
            return JsonUtility.FromJson(JsonUtility.ToJson(this), GetType()) as CommandBase;
        }

        public CommandBase Copy(ScenarioPage page) {
            CommandBase copied = Copy();
            copied._page = page;
            return copied;
        }
    }
}