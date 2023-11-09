using System.Collections;
using System.Collections.Generic;
using Codice.CM.Triggers;
using UnityEditor;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class ScenarioEditGUIStatus {
        /// <summary>
        /// 編集中のページ
        /// </summary>
        private int _currentPageIndex;
        private static readonly string _currentPageIndexSessionStateKey = $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_currentPageIndex)}";
        internal int CurrentPageIndex {
            get => _currentPageIndex;
            set {
                _currentPageIndex = value;
                SessionState.SetInt(_currentPageIndexSessionStateKey, _currentPageIndex);
                // ページ変更時はコマンド選択状態はリセット
                CurrentCommandIndex = 0;
            }
        }

        /// <summary>
        /// 編集中のコマンド
        /// </summary>
        private int _currentCommandIndex;
        private static readonly string _currentCommandIndexSessionStateKey = $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_currentCommandIndex)}";
        internal int CurrentCommandIndex {
            get => _currentCommandIndex;
            set {
                _currentCommandIndex = value;
                SessionState.SetInt(_currentCommandIndexSessionStateKey, _currentCommandIndex);
            }
        }

        /// <summary>
        /// 選択中のコマンド分類
        /// </summary>
        private string _currentCommandGroupId;
        private static readonly string _currentCommandGroupIdSessionStateKey = $"{nameof(Kodama)}_{nameof(ScenarioSystem)}_{nameof(_currentCommandGroupId)}";
        internal string CurrentCommandGroupId {
            get => _currentCommandGroupId;
            set {
                _currentCommandGroupId = value;
                SessionState.SetString(_currentCommandGroupIdSessionStateKey, _currentCommandGroupId);
            }
        }

        public ScenarioEditGUIStatus() {
            _currentPageIndex = SessionState.GetInt(_currentPageIndexSessionStateKey, 0);
            _currentCommandIndex = SessionState.GetInt(_currentCommandIndexSessionStateKey, 0);
            _currentCommandGroupId = SessionState.GetString(_currentCommandGroupIdSessionStateKey, "");
        }
    }
}