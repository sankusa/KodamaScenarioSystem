using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServiceLocator {
    T Resolve<T>();
    IEnumerable<T> ResolveAll<T>();
}
