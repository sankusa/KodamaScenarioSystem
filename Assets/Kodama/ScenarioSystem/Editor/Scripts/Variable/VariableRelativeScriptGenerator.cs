using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Linq;
using System;
using System.IO;

namespace Kodama.ScenarioSystem.Editor {
    public class VariableRelativeScriptGenerator {
        private const string _replaceString_Body = "#Body#";
        private const string _replaceString_Type = "#type#";
        private const string _replaceString_Type_Pascal = "#Type#";

// VariableKey
        private const string _variableKeyFileTemplate = @"using System;

// Auto Generated Script
namespace Kodama.ScenarioSystem {
    " + _replaceString_Body + @"
}
";
        private const string _variableKeyTemplate = @"
    [Serializable]
    public class " + _replaceString_Type_Pascal + @"VariableKey : VariableKey<"+ _replaceString_Type + @">{}
";

        private const string _variableKeyFileName = "/SerializableVariableKey.cs";

// VariableKey
        private const string _valueOrVaiableKeyFileTemplate = @"using System;

// Auto Generated Script
namespace Kodama.ScenarioSystem {
    " + _replaceString_Body + @"
}
";
        private const string _valueOrVaiableKeyTemplate = @"
    [Serializable]
    public class " + _replaceString_Type_Pascal + @"ValueOrVariableKey : ValueOrVariableKey<"+ _replaceString_Type + @">{}
";

        private const string _valueOrVaiableKeyFileName = "/SerializableValueOrVariableKey.cs";

// VariableKey
        private const string _callArgFileTemplate = @"using System;

// Auto Generated Script
namespace Kodama.ScenarioSystem {
    " + _replaceString_Body + @"
}
";
        private const string _callArgTemplate = @"
    [Serializable]
    public class " + _replaceString_Type_Pascal + @"CallArg : CallArg<"+ _replaceString_Type + @">{}
";

        private const string _callArgFileName = "/SerializableCallArg.cs";

        [DidReloadScripts]
        private static void Generate() {
            GenerateVariableKeyScripts();
            GenerateValueOrVariableKeyScripts();
            GenerateCallArgScripts();
        }

        private static void GenerateVariableKeyScripts() {
            Type[] variableTypes = TypeCache.GetTypesDerivedFrom<VariableBase>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .OrderBy(x => x.BaseType.GenericTypeArguments[0].FullName)
                .ToArray();

            Type[] variableArgTypes = variableTypes.Select(x => x.BaseType.GenericTypeArguments[0]).ToArray();

            Type[] variableKeyArgTypes = TypeCache.GetTypesDerivedFrom<VariableKey>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .Select(x => x.BaseType.GenericTypeArguments[0])
                .OrderBy(x => x.FullName)
                .ToArray();

            if(variableArgTypes.SequenceEqual(variableKeyArgTypes) == false) {
                string variableKeyScriptPath = AssetDatabase.FindAssets("VariableKey t:MonoScript")
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .Where(x => AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass() == typeof(VariableKey))
                    .First();
                string generatePath = variableKeyScriptPath.Substring(0, variableKeyScriptPath.LastIndexOf('/'));

                string scriptBody = "";
                foreach(Type variableType in variableTypes) {
                    Type argType = variableType.BaseType.GenericTypeArguments[0];
                    string typeName = TypeNameUtil.ConvertToPrimitiveTypeName(argType.Name);
                    string typeNameCamel = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);
                    scriptBody += _variableKeyTemplate.Replace(_replaceString_Type, argType.FullName).Replace(_replaceString_Type_Pascal, typeNameCamel);
                }
                File.WriteAllText(generatePath + _variableKeyFileName, _variableKeyFileTemplate.Replace(_replaceString_Body, scriptBody));
                AssetDatabase.Refresh();
            }
        }

        private static void GenerateValueOrVariableKeyScripts() {
            Type[] variableTypes = TypeCache.GetTypesDerivedFrom<VariableBase>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .OrderBy(x => x.BaseType.GenericTypeArguments[0].FullName)
                .ToArray();

            Type[] variableArgTypes = variableTypes.Select(x => x.BaseType.GenericTypeArguments[0]).ToArray();

            Type[] valueOrVariableKeyArgTypes = TypeCache.GetTypesDerivedFrom<ValueOrVariableKey>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .Select(x => x.BaseType.GenericTypeArguments[0])
                .OrderBy(x => x.FullName)
                .ToArray();

            if(variableArgTypes.SequenceEqual(valueOrVariableKeyArgTypes) == false) {
                string variableKeyScriptPath = AssetDatabase.FindAssets("ValueOrVariableKey t:MonoScript")
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .Where(x => AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass() == typeof(ValueOrVariableKey))
                    .First();
                string generatePath = variableKeyScriptPath.Substring(0, variableKeyScriptPath.LastIndexOf('/'));

                string scriptBody = "";
                foreach(Type variableType in variableTypes) {
                    Type argType = variableType.BaseType.GenericTypeArguments[0];
                    string typeName = TypeNameUtil.ConvertToPrimitiveTypeName(argType.Name);
                    string typeNameCamel = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);
                    scriptBody += _valueOrVaiableKeyTemplate.Replace(_replaceString_Type, argType.FullName).Replace(_replaceString_Type_Pascal, typeNameCamel);
                }
                File.WriteAllText(generatePath + _valueOrVaiableKeyFileName, _valueOrVaiableKeyFileTemplate.Replace(_replaceString_Body, scriptBody));
                AssetDatabase.Refresh();
            }
        }

        private static void GenerateCallArgScripts() {
            Type[] variableTypes = TypeCache.GetTypesDerivedFrom<VariableBase>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .OrderBy(x => x.BaseType.GenericTypeArguments[0].FullName)
                .ToArray();

            Type[] variableArgTypes = variableTypes.Select(x => x.BaseType.GenericTypeArguments[0]).ToArray();

            Type[] callArgTypes = TypeCache.GetTypesDerivedFrom<CallArg>()
                .Where(x => x.IsAbstract == false && x.IsGenericType == false)
                .Select(x => x.BaseType.GenericTypeArguments[0])
                .OrderBy(x => x.FullName)
                .ToArray();

            if(variableArgTypes.SequenceEqual(callArgTypes) == false) {
                string callArgScriptPath = AssetDatabase.FindAssets("CallArg t:MonoScript")
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .Where(x => AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass() == typeof(CallArg))
                    .First();
                string generatePath = callArgScriptPath.Substring(0, callArgScriptPath.LastIndexOf('/'));

                string scriptBody = "";
                foreach(Type variableType in variableTypes) {
                    Type argType = variableType.BaseType.GenericTypeArguments[0];
                    string typeName = TypeNameUtil.ConvertToPrimitiveTypeName(argType.Name);
                    string typeNameCamel = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1);
                    scriptBody += _callArgTemplate.Replace(_replaceString_Type, argType.FullName).Replace(_replaceString_Type_Pascal, typeNameCamel);
                }
                File.WriteAllText(generatePath + _callArgFileName, _callArgFileTemplate.Replace(_replaceString_Body, scriptBody));
                AssetDatabase.Refresh();
            }
        }
    }
}