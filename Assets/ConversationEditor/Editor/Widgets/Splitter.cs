using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class Splitter
{
    static MethodInfo EditorGUI_BeginVerticalSplit_MethodInfo;
    static MethodInfo EditorGUI_EndVerticalSplit_MethodInfo;

    public class State
    {
        public object Instance { get; private set; }

        public State(float[] relativeSizes, int[] minSizes, int[] maxSizes)
        {
            //splitterStateType = typeof(SplitterState).Assembly.GetType("UnityEditor.SplitterState");
            Type splitterStateType = Type.GetType("UnityEditor.UnityEditor.SplitterState");

            object[] args = { relativeSizes, minSizes, maxSizes };

            Instance = Activator.CreateInstance(splitterStateType, args);
        }
    }

    public Splitter()
    {
        //Type splitterGUILayoutType = typeof(SplitterGUILayout).Assembly.GetType("UnityEditor.SplitterGUILayout");
        Type splitterGUILayoutType = Type.GetType("UnityEditor.UnityEditor.SplitterGUILayout");

        if (splitterGUILayoutType != null)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            EditorGUI_BeginVerticalSplit_MethodInfo = splitterGUILayoutType.GetMethod("BeginVerticalSplit", bindingFlags);
            EditorGUI_EndVerticalSplit_MethodInfo = splitterGUILayoutType.GetMethod("EndVerticalSplit", bindingFlags);
        }
    }

    public void BeginVerticalSplit(State state, params GUILayoutOption[] options)
    {
        if (EditorGUI_BeginVerticalSplit_MethodInfo != null)
        {
            object[] args = new object[] { state.Instance, options };

            EditorGUI_BeginVerticalSplit_MethodInfo.Invoke(null, args);
        }
    }

    public void EndVerticalSplit()
    {
        if (EditorGUI_EndVerticalSplit_MethodInfo != null)
        {
            EditorGUI_EndVerticalSplit_MethodInfo.Invoke(null, null);
        }
    }
}
