using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
#if KODAMA_SCENARIO_ZENJECT_SUPPORT
using Zenject;
#endif

namespace Kodama.ScenarioSystem {
    internal class ServiceLocator : IServiceLocator {
        private readonly List<object> _boundObjects = new List<object>();
        private readonly ComponentBinding _componentBinding;

#if KODAMA_SCENARIO_ZENJECT_SUPPORT
        public DiContainer DiContainer {get; set;}
#endif

        public ServiceLocator(ComponentBinding componentBinding) {
            _componentBinding = componentBinding;
        }

        public T Resolve<T>() where T : class {
            T t = default;
            t = _boundObjects.OfType<T>().LastOrDefault();
            if(t != null) return t;

            if(_componentBinding != null) {
                t = _componentBinding.Components.OfType<T>().LastOrDefault();
                if(t != null) return t;
            }
            t = GlobalComponentBinding.AllComponents.OfType<T>().LastOrDefault();
            if(t != null) return t;

#if KODAMA_SCENARIO_ZENJECT_SUPPORT
            if(DiContainer != null) {
                t = DiContainer.TryResolve<T>();
                if(t != null) return t;
            }
#endif

            return t;
        }

        public object Resolve(Type type) {
            object obj;
            obj = _boundObjects.LastOrDefault(x => type.IsAssignableFrom(x.GetType()));
            if(obj != null) return obj;

            if(_componentBinding != null) {
                obj = _componentBinding.Components.LastOrDefault(x => type.IsAssignableFrom(x.GetType()));
                if(obj != null) return obj;
            }
            obj = GlobalComponentBinding.AllComponents.LastOrDefault(x => type.IsAssignableFrom(x.GetType()));
            if(obj != null) return obj;

#if KODAMA_SCENARIO_ZENJECT_SUPPORT
            if(DiContainer != null) {
                obj = DiContainer.TryResolve(type);
                if(obj != null) return obj;
            }
#endif

            return obj;
        }

        public IEnumerable<T> ResolveAll<T>() {
            IEnumerable<T> tAll = Enumerable.Empty<T>();
            tAll.Concat(_boundObjects.OfType<T>());
            if(_componentBinding != null) {
                tAll.Concat(_componentBinding.Components.OfType<T>());
            }
            tAll.Concat(GlobalComponentBinding.AllComponents.OfType<T>());

#if KODAMA_SCENARIO_ZENJECT_SUPPORT
            if(DiContainer != null) {
                tAll.Concat(DiContainer.ResolveAll<T>());
            }
#endif

            return tAll;
        }

        public void Bind(object obj) {
            _boundObjects.Add(obj);
        }
    }
}