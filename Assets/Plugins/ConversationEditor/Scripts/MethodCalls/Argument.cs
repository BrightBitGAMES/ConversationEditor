using UnityEngine;
using System;

namespace BrightBit
{

[Serializable]
public class Argument
{
    public enum ArgumentMode { VOID, INT, BOOL, FLOAT, STRING }

    [SerializeField] ArgumentMode mode = ArgumentMode.VOID;

    [SerializeField] int intValue;
    [SerializeField] bool boolValue;
    [SerializeField] float floatValue;
    [SerializeField] string stringValue;

    public ArgumentMode Mode
    {
        get { return mode;  }
        set { mode = value; }
    }

    public int Int
    {
        get { return intValue;  }
        set { intValue = value; }
    }

    public bool Bool
    {
        get { return boolValue;  }
        set { boolValue = value; }
    }

    public float Float
    {
        get { return floatValue;  }
        set { floatValue = value; }
    }

    public string String
    {
        get { return stringValue;  }
        set { stringValue = value; }
    }

    public object GetValueByType(Type type)
    {
        if (type == typeof (int))         return intValue;
        else if (type == typeof (bool))   return boolValue;
        else if (type == typeof (float))  return floatValue;
        else if (type == typeof (string)) return stringValue;

        throw new System.InvalidOperationException("Unsupported parameter type : " + type);
    }

    public void Clear()
    {
        intValue    = 0;
        boolValue   = false;
        floatValue  = 0.0f;
        stringValue = string.Empty;
    }

    public object[] ToObjects() // internal use only
    {
        if (mode == ArgumentMode.VOID) return new object[0];

        object[] result = new object[1];

        switch (mode)
        {
            case ArgumentMode.INT    : result[0] = intValue;    break;
            case ArgumentMode.BOOL   : result[0] = boolValue;   break;
            case ArgumentMode.FLOAT  : result[0] = floatValue;  break;
            case ArgumentMode.STRING : result[0] = stringValue; break;
        }

        return result;
    }

    public Type[] ToTypes() // internal use only
    {
        if (mode == ArgumentMode.VOID) return new Type[0];

        Type[] result = new Type[1];

        switch (mode)
        {
            case ArgumentMode.INT    : result[0] = typeof(int);    break;
            case ArgumentMode.BOOL   : result[0] = typeof(bool);   break;
            case ArgumentMode.FLOAT  : result[0] = typeof(float);  break;
            case ArgumentMode.STRING : result[0] = typeof(string); break;
        }

        return result;
    }
}

} // of namespace BrightBit
