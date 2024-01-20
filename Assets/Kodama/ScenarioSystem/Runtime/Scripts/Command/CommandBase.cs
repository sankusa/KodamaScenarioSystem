using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    /// <summary>
    /// コマンド基底クラス
    /// </summary>
    public class CommandBase : ScriptableObject {
#if UNITY_EDITOR
        public static CommandBase CreateInstance(Type commandType, ScenarioPage page) {
            CommandBase command = ScriptableObject.CreateInstance(commandType) as CommandBase;
            // Nullable & [SerializeField] なフィールドはシリアライズ/デシリアライズを通るまでnullのままになる
            // 安全のためシリアライズ & デシリアライズでnullを潰しておく
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(command), command);
            // CommandBase command = Activator.CreateInstance(commandType, page) as CommandBase;
            command._parentPage = page;
            Undo.RegisterCreatedObjectUndo(command, commandType.Name + " Created");
            return command;
        }
#endif

        [SerializeField, HideInInspector] private ScenarioPage _parentPage;
        public ScenarioPage ParentPage => _parentPage;

        public int Index => ParentPage.IndexOf(this);

        // CreateInstance以からのインスタンス化は禁止
        // protected CommandBase() {}

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

        public string LogCaption => $"<b><i>{GetType().Name}</i></b>    ";
        public string LogHeader => $"{LogCaption}<b>Scenario</b>[ {ParentPage.ParentScenario.name} ]    <b>Page</b>[ {ParentPage.name} ]    <b>Index</b>[ {Index.ToString()} ]";

        public CommandBase Copy() {
            CommandBase copied = Instantiate(this);
            copied.name = name;
            Undo.RegisterCreatedObjectUndo(copied, copied.GetType().Name + " Copied");
            return copied;
            // return JsonUtility.FromJson(JsonUtility.ToJson(this), GetType()) as CommandBase;
        }

        public CommandBase Copy(ScenarioPage page) {
            CommandBase copied = Copy();
            copied._parentPage = page;
            return copied;
        }
    }
}