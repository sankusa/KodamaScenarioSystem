using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVariableValueFieldAttribute : Attribute {
    public Type Type {get;}

    public CustomVariableValueFieldAttribute(Type type) {
        Type = type;
    }
}
