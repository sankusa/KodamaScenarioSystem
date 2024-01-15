using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    internal class CommandDropdown : AdvancedDropdown {
        private Action<Type> _onCommandSelected;
        public CommandDropdown(AdvancedDropdownState state, Action<Type> onCommandSelected) : base(state) {
            _onCommandSelected = onCommandSelected;
        }

        protected override AdvancedDropdownItem BuildRoot() {
            var root = new AdvancedDropdownItem("Root");
            foreach(CommandGroupSetting groupSetting in CommandGroupSetting.All) {
                AdvancedDropdownItem groupItem = new AdvancedDropdownItem(groupSetting.DisplayName);
                root.AddChild(groupItem);

                foreach(CommandSetting setting in groupSetting.CommandSettings) {
                    groupItem.AddChild(new CommandDropdownItem(setting));
                }
            }


            // for (var i = 0; i < 10; i++) {
            //     var item = new AdvancedDropdownItem($"Item {i + 1}");
            //     for (var j = 0; j < 10; j++)
            //         item.AddChild(new AdvancedDropdownItem($"Item {i + 1} - {j + 1}"));
            //     root.AddChild(item);
            // }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            if(item is CommandDropdownItem commandDropdownItem) {
                _onCommandSelected?.Invoke(commandDropdownItem.Setting.CommandScript.GetClass());
            }
        }

        public void ShowDropDown(Rect rect) {
            Show(rect);
            EditorWindow window = EditorWindow.focusedWindow;
            if (window != null && window.GetType().Namespace == typeof(AdvancedDropdown).Namespace) {
                Rect windowPos = window.position;
                window.maxSize = new Vector2(400, 800);
                windowPos.height = 300;
                window.position = windowPos;
            }
        }
    }
    internal class CommandDropdownItem : AdvancedDropdownItem {
        private readonly CommandSetting _setting;
        public CommandSetting Setting => _setting;

        public CommandDropdownItem(CommandSetting setting) : base(setting.DisplayName) {
            _setting = setting;
            icon = setting.Icon;
        }
    }
}