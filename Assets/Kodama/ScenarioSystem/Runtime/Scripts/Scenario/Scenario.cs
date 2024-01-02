using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine.Assertions;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kodama.ScenarioSystem {
    [CreateAssetMenu(fileName = "New " + nameof(Scenario), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(Scenario))]
    public class Scenario : ScriptableObject {
        [SerializeField] private List<ScenarioPage> _pages;
        public IList<ScenarioPage> Pages => _pages;
        public ScenarioPage DefaultPage => _pages[0];

        [SerializeReference] private List<VariableBase> _variables;
        public IList<VariableBase> Variables => _variables;

#if UNITY_EDITOR
        private const string _defaultPageName = "New Page";
        private const string _undoOperationName_CreatePage = "Create Page";
        private const string _undoOperationName_DestroyPage = "Destroy Page";
        private const string _undoOperationName_ChangeDefaultPage = "Change Default Page";

        [SerializeField] private Vector2 _graphPosition;
        public Vector2 GraphPosition {
            get => _graphPosition;
            set => _graphPosition = value;
        }
        
        public ScenarioPage CreatePage() {
            ScenarioPage newPage = CreateInstance<ScenarioPage>();
            newPage.name = ObjectNames.GetUniqueName(_pages.Select(x => x.name).ToArray(), _defaultPageName);
            newPage.Scenario = this;
            Undo.RegisterCreatedObjectUndo(newPage, _undoOperationName_CreatePage);
            Undo.RecordObject(this, _undoOperationName_CreatePage);
            AssetDatabase.AddObjectToAsset(newPage, this);
            Pages.Add(newPage);
            return newPage;
        }

        public void DestroyPage(ScenarioPage page) {
            Undo.RecordObject(this, _undoOperationName_DestroyPage);
            Pages.Remove(page);
            Undo.DestroyObjectImmediate(page);
        }

        public void DestroyPageAt(int index) {
            DestroyPage(Pages[index]);
        }

        public void ChangeDefaultPage(ScenarioPage page) {
            if(_pages.Contains(page) == false) {
                Debug.LogError("Scenario don't have the page.");
                return;
            }

            Undo.RecordObject(this, _undoOperationName_ChangeDefaultPage);
            _pages.Remove(page);
            _pages.Insert(0, page);
        }
#endif

        public ScenarioPage FindPageByName(string pageName) {
            return _pages.Find(x => x.name == pageName);
        }
    }
}