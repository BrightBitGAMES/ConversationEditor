using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BrightBit
{

public static class MethodInfoExtensions
{
    static readonly Dictionary<Type, string> TypeToStringMap = new Dictionary<Type, string>
    {
        { typeof(void),   "void"   },
        { typeof(int),    "int"    },
        { typeof(float),  "float"  },
        { typeof(bool),   "bool"   },
        { typeof(string), "string" },
    };

    public static string ToStringWithSignature(this MethodInfo methodInfo)
    {
        string returnType = TypeToString(methodInfo.ReturnType);
        string methodName = methodInfo.Name;
        string parameters = String.Join(", ", methodInfo.GetParameters().Select(p => TypeToString(p.ParameterType)).ToArray());

        return returnType + " " + methodName + " (" + parameters + ")";
    }

    static string TypeToString(Type type)
    {
        string result;

        if (TypeToStringMap.TryGetValue(type, out result)) return result;
        else                                               return type.Name;
    }
}

} // of namespace BrightBit
