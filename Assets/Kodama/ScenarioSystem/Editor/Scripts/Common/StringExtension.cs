using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {    
    public static class StringExtension  {
        public static int CountLine(this string s) {
            int n = 0;
            foreach(char c in s) {
                if (c == '\n') n++;
            }
            return n + 1;
        }
    }
}