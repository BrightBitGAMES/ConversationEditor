using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Linq;
using System.Reflection;

namespace BrightBit
{

#if UNITY_EDITOR

public static class SerializedPropertyExtensions
{
    static readonly BindingFlags FieldBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public static bool IsMissing(this SerializedProperty property)
    {
        return property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0;
    }

    public static T GetValue<T>(this SerializedProperty property)
    {
        string path = property.propertyPath;
        object obj  = property.serializedObject.targetObject;

        foreach (string part in path.Split('.'))
        {
            obj = GetFieldOrPropertyValue<object>(part, obj);
        }

        return (T) obj;
    }

    public static void SetValue<T>(this SerializedProperty property, T value)
    {
        object obj = property.serializedObject.targetObject;

        string[] fieldHierarchy = property.propertyPath.Split('.');

        for (int i = 0; i < fieldHierarchy.Length - 1; ++i)
        {
            obj = GetFieldOrPropertyValue<object>(fieldHierarchy[i], obj);
        }

        string fieldName = fieldHierarchy.Last();

        FieldInfo field = obj.GetType().GetField(fieldName, FieldBindingFlags);

        if (field != null)
        {
            field.SetValue(obj, value);
            return;
        }

        PropertyInfo target = obj.GetType().GetProperty(fieldName, FieldBindingFlags);

        if (target != null)
        {
            target.SetValue(obj, value, null);
            return;
        }

        throw new InvalidOperationException("The property '" + fieldName + "' couldn't be found!");
    }

    static T GetFieldOrPropertyValue<T>(string fieldName, object obj)
    {
        FieldInfo field = obj.GetType().GetField(fieldName, FieldBindingFlags);

        if (field != null) return (T)field.GetValue(obj);

        PropertyInfo property = obj.GetType().GetProperty(fieldName, FieldBindingFlags);

        if (property != null) return (T)property.GetValue(obj, null);

        return default(T);
    }
}

#endif

} // of namespace BrightBit
