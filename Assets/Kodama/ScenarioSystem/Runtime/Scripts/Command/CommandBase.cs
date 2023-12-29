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

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="service">利用可能な機能群</param>
        public virtual void Execute(ICommandService service) {}

        /// <summary>
        /// エディタ表示用のサマリ作成
        /// </summary>
        public virtual string GetSummary() {
            return GetType().Name;
        }

        protected string LogCaption => $"<b><i><color={ColorPreference.CommandNameColor}>{GetType().Name}</color></i></b>    ";

        public CommandBase Copy() {
            return JsonUtility.FromJson(JsonUtility.ToJson(this), GetType()) as CommandBase;
        }
    }
}