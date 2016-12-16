using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace BrightBit
{

[Serializable]
public class NodeEvent
{
#if UNITY_EDITOR
    [SerializeField] protected MonoScript script;
#endif

    [SerializeField] protected Argument argument;
    [SerializeField] internal SerializableMethodInfo methodInfo;

    protected const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

    protected MethodInfo method { get { return methodInfo; } }

    protected object[] CreateParameterArray(ConversationInfo info)
    {
        ParameterInfo[] parameters = method.GetParameters();

        if (parameters.Length == 0)
        {
            return new object[0];
        }
        else if (parameters.Length == 1)
        {
            Type first = parameters[0].ParameterType;

            if (first == typeof(ConversationInfo)) return new object[] { info };

            return new object[] { argument.GetValueByType(first) };
        }
        else if (parameters.Length == 2)
        {
            Type first  = parameters[0].ParameterType;
            Type second = parameters[1].ParameterType;

            if (first == typeof(ConversationInfo))                     return new object[] { info, argument.GetValueByType(second) };
            else if (first == typeof(string) && second == typeof(int)) return new object[] { argument.String, argument.Int };
            else
                throw new System.InvalidOperationException("Invalid types for two parameter arrangement : " + first + " : " + second);
        }
        else if (parameters.Length == 3)
        {
            Type first  = parameters[0].ParameterType;
            Type second = parameters[1].ParameterType;
            Type third  = parameters[2].ParameterType;

            if (first == typeof(ConversationInfo) && second == typeof(string) && third == typeof(int)) return new object[] { info, argument.String, argument.Int };
            else
                throw new System.InvalidOperationException("Invalid types for three parameter arrangement : " + first + " : " + second + " : " + third);
        }
        else throw new System.InvalidOperationException("Invalid amount of parameters!");
    }
}

} // of namespace BrightBit
