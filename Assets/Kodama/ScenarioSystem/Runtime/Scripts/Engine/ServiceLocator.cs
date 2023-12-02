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
            return t;
        }

        public IEnumerable<T> ResolveAll<T>() {
            IEnumerable<T> tAll = Enumerable.Empty<T>();
            if(_componentBinding != null) {
                tAll.Concat(_componentBinding.Components.OfType<T>());
            }
            return tAll;
        }
    }
}