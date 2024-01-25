using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class ReflectionMethodInvokeData {
        [SerializeField] private TypeId _typeId;
        public TypeId TypeId => _typeId;
        [SerializeField] private MethodType _methodType;
        public MethodType MethodType {
            get => _methodType;
            set => _methodType = value;
        }
        [SerializeField] private TargetInstanceResolveWay _instanceResolveWay;
        public TargetInstanceResolveWay InstanceResolveWay {
            get => _instanceResolveWay;
            set => _instanceResolveWay = value;
        }
        [SerializeField] private MethodData _methodData;
        public MethodData MethodData {
            get => _methodData;
            set => _methodData = value;
        }

        public object Invoke(ICommandService service) {
            Type targetType = _typeId.ResolveType();
            Type[] argTypes = _methodData.ArgData.Select(x => x.TypeId.ResolveType()).ToArray();
            object[] args = new object[argTypes.Length];
            for(int i = 0; i < args.Length; i++) {
                MethodArgData argData  = _methodData.ArgData[i];
                if(argData.ResolveWay == MethodArgResolveWay.Null) {
                    args[i] = null;
                }
                else if(argData.ResolveWay == MethodArgResolveWay.Value) {
                    args[i] = ValueStringConverterBundle.Instance.FindConverter(argTypes[i]).StringToValue(argData.ValueString);
                }
                else if(argData.ResolveWay == MethodArgResolveWay.Variable) {
                    args[i] = service.PagePlayProcess.FindVariable(argTypes[i], argData.VaribaleId).GetValueAsObject();
                }
                else if(argData.ResolveWay == MethodArgResolveWay.FromServiceLocater) {
                    args[i] = service.ServiceLocator.Resolve(argTypes[i]);
                }
                else if(argData.ResolveWay == MethodArgResolveWay.DefaultValue) {
                    args[i] = System.Type.Missing;
                }
            }

            if(_methodType == MethodType.Instance) {
                object targetIstance = null;
                if(_instanceResolveWay == TargetInstanceResolveWay.FromServiceLocater) {
                    targetIstance = service.ServiceLocator.Resolve(targetType);
                }
                MethodInfo methodInfo = targetType.GetMethod(
                    _methodData.MethodName,
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    argTypes,
                    null
                );
                return methodInfo.Invoke(targetIstance, args);
            }
            else if(_methodType == MethodType.Static) {
                MethodInfo methodInfo = targetType.GetMethod(
                    _methodData.MethodName,
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    argTypes,
                    null
                );
                return methodInfo.Invoke(null, args);
            }
            return null;
        }
    }

    [Serializable]
    public class TypeId {
        [SerializeField] private string _assemblyName;
        public string AssemblyName {
            get => _assemblyName;
            set => _assemblyName = value;
        }
        [SerializeField] private string _typeFullName;
        public string TypeFullName {
            get => _typeFullName;
            set => _typeFullName = value;
        }

        public string TypeName => TypeFullName.Split('.').Last();

        public TypeId(Type type) {
            _assemblyName = type.Assembly.GetName().Name;
            _typeFullName = type.FullName;
        }

        public Type ResolveType() {
            return AppDomain.CurrentDomain.
                GetAssemblies()
                .First(x => x.GetName().Name == _assemblyName)
                .GetType(_typeFullName);
        }

        public void Clear() {
            _assemblyName = "";
            _typeFullName = "";
        }
    }

    public enum MethodType {
        Instance = 0,
        Static = 5,
    }

    public enum TargetInstanceResolveWay {
        FromServiceLocater = 0,
    }

    [Serializable]
    public class MethodData {
        [SerializeField] private string _methodName;
        public string MethodName => _methodName;

        [SerializeField] private List<MethodArgData> _argData;
        public IReadOnlyList<MethodArgData> ArgData => _argData;

        [SerializeField] private TypeId _returnTypeId;
        public TypeId ReturnTypeId => _returnTypeId;

        public MethodData(MethodInfo methodInfo, ValueStringConverterBundle converters) {
            _methodName = methodInfo.Name;
            _argData = new List<MethodArgData>();
            foreach(ParameterInfo parameterInfo in methodInfo.GetParameters()) {
                _argData.Add(new MethodArgData(parameterInfo, converters));
            }
            _returnTypeId = new TypeId(methodInfo.ReturnType);
        }

        public void Clear() {
            _methodName = "";
            _argData.Clear();
            _returnTypeId.Clear();
        }
    }

    [Serializable]
    public enum MethodArgResolveWay {
        Null = 0,
        Value = 1,
        Variable = 2,
        FromServiceLocater = 3,
        DefaultValue = 4,
    }

    [Serializable]
    public class MethodArgData {
        [SerializeField] private TypeId _typeId;
        public TypeId TypeId => _typeId;

        [SerializeField] private string _argName;
        public string ArgName => _argName;

        [SerializeField] private MethodArgResolveWay _resolveWay;
        public MethodArgResolveWay ResolveWay {
            get => _resolveWay;
            set => _resolveWay = value;
        }

        [SerializeField] private string _valueString;
        public string ValueString {
            get => _valueString;
            set => _valueString = value;
        }

        [SerializeField] private string _variableId;
        public string VaribaleId {
            get => _variableId;
            set => _variableId = value;
        }

        public MethodArgData(ParameterInfo parameterInfo, ValueStringConverterBundle converters) {
            _typeId = new TypeId(parameterInfo.ParameterType);
            _argName = parameterInfo.Name;
            ValueStringConverterBase converter = converters.FindConverter(parameterInfo.ParameterType);
            if(converter == null) {
                _valueString = "";
            }
            else {
                _valueString = converter.InitialString;
            }
            _variableId = "";
        }
    }
}