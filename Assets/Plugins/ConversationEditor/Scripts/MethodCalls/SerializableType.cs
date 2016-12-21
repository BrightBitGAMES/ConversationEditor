using UnityEngine;
using System;

namespace BrightBit
{

[Serializable]
public sealed class SerializableType : ISerializationCallbackReceiver
{
    [SerializeField] string assemblyQualifiedName = string.Empty;

    Type type;

    public Type Type
    {
        get { return type; }
    }

    public SerializableType() {}

    public SerializableType(Type type)
    {
        if (type == null) throw new ArgumentException("Null can't be serialized as a type!", "type");

        this.type = type;
    }

    public string Name
    {
        get { return !string.IsNullOrEmpty(assemblyQualifiedName) ? assemblyQualifiedName : string.Empty; }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName))
        {
            this.assemblyQualifiedName = type != null ? type.FullName + ", " + type.Assembly.GetName().Name : string.Empty;
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName))
        {
            type = null;
        }
        else
        {
            type = System.Type.GetType(assemblyQualifiedName);

            // if (type == null) Debug.LogWarning("'" + assemblyQualifiedName + "' was referenced but the class type wasn't found!");
        }
    }

    public override string ToString()
    {
        return type != null ? type.FullName : "NULL";
    }

    public static implicit operator SerializableType(Type type) { return new SerializableType(type); }
    public static implicit operator Type(SerializableType st)   { return st.type;                    }
}

} // of namespace BrightBit 
