using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public class CommandSummaryDrawerAttribute : Attribute {
        private readonly Type _type;
        public Type Type => _type;
        
        public CommandSummaryDrawerAttribute(Type type) {
            _type = type;
        }
    }
}