using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

namespace BrightBit
{

public static class EditorUtilities
{
    static MethodInfo EditorGUILayout_FoldoutTitlebar_MethodInfo = null;
    static FieldInfo LastControlIdField = typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);

    public static bool FoldoutTitlebar(bool foldout, string content)
    {
        if (EditorGUILayout_FoldoutTitlebar_MethodInfo == null)
        {
            Type editorGUILayoutType = typeof(EditorGUILayout).Assembly.GetType("UnityEditor.EditorGUILayout");

            if (editorGUILayoutType != null)
            {
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

                EditorGUILayout_FoldoutTitlebar_MethodInfo = editorGUILayoutType.GetMethod("FoldoutTitlebar", bindingFlags);
            }
        }

        object[] args = new object[] { foldout, new GUIContent(content) };

        return (bool) EditorGUILayout_FoldoutTitlebar_MethodInfo.Invoke(null, args);
    }

    public static int LastControlID()
    {
        if (LastControlIdField == null)
        {
            Debug.LogError("Compatibility with Unity broke: can't find lastControlId field in EditorGUI");

            return 0;
        }

        return (int) LastControlIdField.GetValue(null);
    }
}

} // of namespace BrightBit
