using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kodama.ScenarioSystem;
using Cysharp.Threading.Tasks;
using UnityEditor;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private ReflectionCommand x;
    [SerializeField] private Variable<int> xx;
    [SerializeField] private Variable<UniTask> y;
    [SerializeReference] private List<VariableBase> list;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Type.GetType("8".GetType().FullName).FullName);
        list.Add(new Variable<int>());
        // list.Add(new Variable<string>());
        // list.Add(new Variable<bool>());
        Debug.Log(JsonUtility.ToJson(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
