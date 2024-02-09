using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public struct FlexibleEnumerable<T> : IEnumerable<T> where T : class{
        private readonly T _target1;
        private readonly T _target2;
        private readonly IList<T> _list;
        public FlexibleEnumerable(T target1 = null, T target2 = null) {
            _target1 = target1;
            _target2 = target2;
            _list = null;
        }
        public FlexibleEnumerable(IList<T> list) {
            _target1 = null;
            _target2 = null;
            _list = list;
        }
        public Enumerator GetEnumerator() {
            if(_list == null) {
                return new Enumerator(_target1, _target2);
            }
            else {
                return new Enumerator(_list);
            }
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T> {
            private readonly T _target1;
            private readonly T _target2;
            private readonly IList<T> _list;
            private int _index;
            public Enumerator(T target1, T target2) {
                _target1 = target1;
                _target2 = target2;
                _list = null;
                _index = -1;
            }
            public Enumerator(IList<T> list) {
                _target1 = null;
                _target2 = null;
                _list = list;
                _index = -1;
            }
            public T Current {
                get {
                    if(_list == null) {
                        if(_index == 0) return _target1;
                        else if(_index == 1) return _target2;
                        return null;
                    }
                    else {
                        return _list[_index];
                    }
                }
            }
            object IEnumerator.Current => Current;

            public void Dispose() {}

            public bool MoveNext() {
                _index++;
                if(_list == null) {
                    if(_index == 0) return _target1 != null;
                    else if(_index == 1) return _target2 != null;
                    return false;
                }
                else {
                    return _index < _list.Count;
                }
            }

            void IEnumerator.Reset() {
                _index = -1;
            }
        }
    }
}