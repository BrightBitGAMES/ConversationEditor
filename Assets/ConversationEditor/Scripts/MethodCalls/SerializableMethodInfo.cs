using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BrightBit
{

[System.Serializable]
public sealed class SerializableMethodInfo : ISerializationCallbackReceiver
{
    [SerializeField] SerializableType target;
    [SerializeField] string methodName;
    [SerializeField] BindingFlags bindingFlags;
    [SerializeField] List<SerializableType> parameters = null;
    [SerializeField] SerializableType returnType;

    MethodInfo methodInfo;

    public MethodInfo MethodInfo
    {
        get { return methodInfo;  }
        set { methodInfo = value; }
    }

    public SerializableMethodInfo(MethodInfo methodInfo)
    {
        if (methodInfo != null && methodInfo.IsGenericMethod) throw new ArgumentException("Generic methods can't be serialized!", "methodInfo");

        this.methodInfo = methodInfo;
    }

    public bool IsValid
    {
        get { return methodInfo != null; }
    }

    public string DeclaringTypeName
    {
        get { return target.Name; }
    }

    public int NumParameters
    {
        get { return methodInfo != null ? methodInfo.GetParameters().Length : 0; }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (methodInfo == null) return;

        // Debug.Log("SerializableMethodInfo::OnBeforeSerialize");

        target = new SerializableType(methodInfo.DeclaringType);

        methodName = methodInfo.Name;

        bindingFlags |= methodInfo.IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
        bindingFlags |= methodInfo.IsStatic  ? BindingFlags.Static : BindingFlags.Instance;

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();

        if (parameterInfos.IsNullOrEmpty()) parameters = null;
        else                                parameters = new List<SerializableType>(parameterInfos.Select(info => new SerializableType(info.ParameterType)));
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (target == null || string.IsNullOrEmpty(methodName)) return;

        // Debug.Log("SerializableMethodInfo::OnAfterDeserialize");

        Type type = target.Type;

        if (type == null)
        {
            Debug.LogWarning("The method '" + methodName + "' can't be deserialized since the owning class type '" + target.Name + "' is missing!");
            return;
        }

        if (parameters.IsNullOrEmpty()) methodInfo = type.GetMethod(methodName, bindingFlags);
        else                            methodInfo = type.GetMethod(methodName, bindingFlags, null, parameters.Select(p => p.Type).ToArray(), null);
    }

    public override string ToString()
    {
        return methodInfo != null ? methodInfo.ToString() : "NULL";
    }

    public static implicit operator SerializableMethodInfo(MethodInfo mi)  { return new SerializableMethodInfo(mi);      }
    public static implicit operator MethodInfo(SerializableMethodInfo smi) { return smi != null ? smi.methodInfo : null; }
}

} // of namespace BrightBit
