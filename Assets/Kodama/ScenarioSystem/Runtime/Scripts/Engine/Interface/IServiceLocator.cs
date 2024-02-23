using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServiceLocator {
    T Resolve<T>() where T : class;
    object Resolve(Type type);
    IEnumerable<T> ResolveAll<T>();
    void Bind(object obj);
}
