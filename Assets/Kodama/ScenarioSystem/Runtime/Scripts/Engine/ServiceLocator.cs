using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Kodama.ScenarioSystem {
    internal class ServiceLocator : IServiceLocator {
        private readonly ComponentBinding _componentBinding;

        public ServiceLocator(ComponentBinding componentBinding) {
            _componentBinding = componentBinding;
        }

        public T Resolve<T>() {
            T t = default;
            if(_componentBinding != null) {
                t = _componentBinding.Components.OfType<T>().FirstOrDefault();
                if(t != null) return t;
            }
            t = GlobalComponentBinding.AllComponents.OfType<T>().FirstOrDefault();
            if(t != null) return t;
            return t;
        }

        public object Resolve(Type type) {
            object obj;
            if(_componentBinding != null) {
                obj = _componentBinding.Components.FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
                if(obj != null) return obj;
            }
            obj = GlobalComponentBinding.AllComponents.FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
            if(obj != null) return obj;
            return obj;
        }

        public IEnumerable<T> ResolveAll<T>() {
            IEnumerable<T> tAll = Enumerable.Empty<T>();
            if(_componentBinding != null) {
                tAll.Concat(_componentBinding.Components.OfType<T>());
            }
            tAll.Concat(GlobalComponentBinding.AllComponents.OfType<T>());
            return tAll;
        }
    }
}