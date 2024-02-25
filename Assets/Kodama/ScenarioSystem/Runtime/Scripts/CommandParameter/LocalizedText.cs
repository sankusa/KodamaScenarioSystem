#if KODAMA_SCENARIO_LOCALIZATION_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class LocalizedText {
        public static LocalizedText CreateLocalizedText() {
            LocalizedText localizedText = new LocalizedText();
            foreach(Locale locale in LocalizationSettings.AvailableLocales.Locales) {
                localizedText._records.Add(new LocalizedTextRecord(locale.Identifier.Code));
            }
            return localizedText;
        }

        [SerializeField] private List<LocalizedTextRecord> _records;

#if UNITY_EDITOR
        public void AddRecord(CommandBase parentCommand, string localeCode) {
            if(_records.Any(x => x.LocaleCode == localeCode)) return;

            Undo.RecordObject(parentCommand, "Add LocalizedText record");
            _records.Add(new LocalizedTextRecord(localeCode));
            _records.Sort((a, b) => a.LocaleCode.CompareTo(b.LocaleCode));
        }

        public void RemoveRecord(CommandBase parentCommand, string localeCode) {
            Undo.RecordObject(parentCommand, "Remove LocalizedText record");
            _records.Remove(_records.Find(x => x.LocaleCode == localeCode));
        }
#endif

        public string GetProjectLocaleText() {
            return _records.Find(x => x.LocaleCode == LocalizationSettings.ProjectLocale.Identifier.Code)?.Text;
        }
        
        public string ResolveText() {
            Locale locale = LocalizationSettings.SelectedLocale;
            LocalizedTextRecord record = _records.Find(x => x.LocaleCode == locale.Identifier.Code);
            if(record == null) {
                List<Locale> fallbacks = new List<Locale>(locale.GetFallbacks());
                while(fallbacks.Count > 0) {
                    locale = fallbacks[0];
                    record = _records.Find(x => x.LocaleCode == locale.Identifier.Code);
                    if(record != null) break;

                    IEnumerable<Locale> subsequentFallbacks = locale.GetFallbacks();
                    if(subsequentFallbacks.Count() > 0) {
                        fallbacks.AddRange(subsequentFallbacks);
                    }
                    fallbacks.RemoveAt(0);
                }
            }

            if(record == null) return "";

            return record.Text;
        }
    }

    [Serializable]
    public class LocalizedTextRecord {
        [SerializeField] private string _localeCode;
        public string LocaleCode => _localeCode;

        public LocalizedTextRecord(string localeCode) {
            _localeCode = localeCode;
        }

        [SerializeField, TextArea(2, 10)] private string _text;
        public string Text => _text;
    }
}
#endif