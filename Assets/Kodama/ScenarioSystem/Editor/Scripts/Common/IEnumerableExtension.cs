using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public static class IEnumerableExtension {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T value) {
            int index = 0;
            foreach(T t in enumerable) {
                if(t.Equals(value)) return index;
                index++;
            }
            return -1;
        }
    }
}