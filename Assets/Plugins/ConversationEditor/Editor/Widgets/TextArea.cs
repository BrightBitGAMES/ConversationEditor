using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class TextArea
{
    static MethodInfo EditorGUI_ScrollableTextAreaInternal_MethodInfo;

    public TextArea()
    {
        Type editorGUIType = typeof(EditorGUI).Assembly.GetType("UnityEditor.EditorGUI");

        if (editorGUIType != null)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            EditorGUI_ScrollableTextAreaInternal_MethodInfo = typeof(EditorGUI).GetMethod("ScrollableTextAreaInternal", bindingFlags);
        }
    }

    public string OnGUILayout(string text, ref Vector2 scrollPosition, params GUILayoutOption[] opts)
    {
        return OnGUILayout(text, ref scrollPosition, EditorStyles.textArea, opts);
    }

    public string OnGUILayout(string text, ref Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] opts)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, style, opts);

        return OnGUI(position, text, ref scrollPosition, style);
    }

    public string OnGUI(Rect rect, string text, ref Vector2 scrollPosition, GUIStyle style)
    {
        if (EditorGUI_ScrollableTextAreaInternal_MethodInfo != null)
        {
            object[] args = new object[] { rect, text, scrollPosition, style };

            string result = (string) EditorGUI_ScrollableTextAreaInternal_MethodInfo.Invoke(null, args);

            scrollPosition = (Vector2) args[2];

            return result;
        }

        return text;
    }
}