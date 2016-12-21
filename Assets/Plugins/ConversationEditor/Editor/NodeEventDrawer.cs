using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace BrightBit
{

[CustomPropertyDrawer(typeof(NodeEvent), true)]
public class NodeEventDrawer : PropertyDrawer
{
    // the signatures don't contain return types
    static readonly Type[][] SupportedMethodSignatures = new Type[][]
    {
        new Type[] { typeof(ConversationNode) },
        new Type[] { typeof(ConversationNode), typeof(int) },
        new Type[] { typeof(ConversationNode), typeof(float) },
        new Type[] { typeof(ConversationNode), typeof(bool) },
        new Type[] { typeof(ConversationNode), typeof(string) },
        new Type[] { typeof(ConversationNode), typeof(string), typeof(int) },

        new Type[] { typeof(int) },
        new Type[] { typeof(float) },
        new Type[] { typeof(bool) },
        new Type[] { typeof(string) },
        new Type[] { typeof(string), typeof(int) },

        new Type[] { } // void
    };

    List<MethodInfo> methods = new List<MethodInfo>();

    SerializedProperty propTarget;
    SerializedProperty propMethodInfo;
    SerializedProperty propArgument;

    const float padding = 2.0f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		Rect scriptRect = new Rect(position.x, position.y,                position.width, EditorGUI.GetPropertyHeight(propTarget));
		Rect methodRect = new Rect(position.x, scriptRect.yMax + padding, position.width, EditorGUIUtility.singleLineHeight);
		Rect argRect    = new Rect(position.x, methodRect.yMax + padding, position.width, EditorGUIUtility.singleLineHeight);

        Rect helpRect = methodRect; helpRect.height = 40;

        SerializableMethodInfo serializableMethodInfo = propMethodInfo.GetValue<SerializableMethodInfo>();

        EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(scriptRect, propTarget, GUIContent.none);
        if (EditorGUI.EndChangeCheck())
        {
            propMethodInfo.serializedObject.ApplyModifiedProperties();

            propMethodInfo.SetValue<SerializableMethodInfo>(null);
            methods.Clear();
        }

        MonoScript script = propTarget.GetValue<MonoScript>();

        if (propTarget.IsMissing())
        {
            SerializableMethodInfo smi = propMethodInfo.GetValue<SerializableMethodInfo>();

            if (smi != null && !string.IsNullOrEmpty(smi.DeclaringTypeName))
                EditorGUI.HelpBox(helpRect, "The MonoScript for the class '" + smi.DeclaringTypeName + "' is missing!", MessageType.Warning);

            return;
        }

        bool requiresBooleanReturnType = property.GetValue<NodeEvent>() is NodeCondition;

        methods = GetAllMethods(script, requiresBooleanReturnType);

		if (methods.Count > 0)
        {
            CustomPopup(methodRect, serializableMethodInfo, methods);

            if (serializableMethodInfo.IsValid) DrawArguments(argRect, serializableMethodInfo, propArgument.GetValue<Argument>());
        }
        else if (script != null && !EditorApplication.isCompiling)
        {
            EditorGUI.HelpBox(helpRect, "No static methods found that meet the required criteria!", MessageType.Warning);
        }
	}

    void CustomPopup(Rect position, SerializableMethodInfo serializableMethodInfo, List<MethodInfo> methods)
    {
        MethodInfo currentMethod = serializableMethodInfo;

        GUIContent label = new GUIContent(currentMethod != null ? currentMethod.Name : "No Function");

        if (GUI.Button(position, label, EditorStyles.popup))
        {
            GenericMenu popupMenu = CreatePopup(serializableMethodInfo, methods);

            popupMenu.DropDown(position);
        }
    }

    GenericMenu CreatePopup(SerializableMethodInfo serializableMethodInfo, List<MethodInfo> methods)
    {
        GenericMenu result = new GenericMenu();

        result.AddItem(new GUIContent("No Function"), !serializableMethodInfo.IsValid, ChangeMethod, null);

        if (methods.Count == 0) return result;

        result.AddSeparator(string.Empty);

        foreach (MethodInfo method in methods)
        {
            bool selected = method == (MethodInfo) serializableMethodInfo;

            result.AddItem(new GUIContent(method.ToStringWithSignature()), selected, ChangeMethod, method);
        }

        return result;
    }

    void ChangeMethod(object source)
    {
        MethodInfo method = (MethodInfo) source;

        if (method != null) propMethodInfo.SetValue<SerializableMethodInfo>(new SerializableMethodInfo(method));
        else                propMethodInfo.SetValue<SerializableMethodInfo>(null);

        propMethodInfo.serializedObject.ApplyModifiedProperties();
    }

    void DrawArguments(Rect argRect, MethodInfo methodInfo, Argument argument)
    {
        ParameterInfo[] parameters = methodInfo.GetParameters();

        if (parameters.Length > 0) argRect = DrawField(argRect, parameters[0], argument);
        if (parameters.Length > 1) argRect = DrawField(argRect, parameters[1], argument);
        if (parameters.Length > 2) argRect = DrawField(argRect, parameters[2], argument);
    }

    Rect DrawField(Rect argRect, ParameterInfo paramInfo, Argument argument)
    {
        Type type = paramInfo.ParameterType;

        if (type == typeof(ConversationNode)) return argRect;

        string name = paramInfo.Name.ToSeparatedPascalCase();

        if (type == typeof(string))     argument.String = EditorGUI.TextField(argRect,  name, argument.String);
        else if (type == typeof(int))   argument.Int    = EditorGUI.IntField(argRect,   name, argument.Int);
        else if (type == typeof(float)) argument.Float  = EditorGUI.FloatField(argRect, name, argument.Float);
        else if (type == typeof(bool))  argument.Bool   = EditorGUI.Toggle(argRect,     name, argument.Bool);

        return new Rect(argRect.x, argRect.yMax + padding, argRect.width, argRect.height);
    }

    List<MethodInfo> GetAllMethods(MonoScript script, bool booleanReturnType)
    {
        List<MethodInfo> result = new List<MethodInfo>();

        if (script == null) return result;

        Type type = script.GetClass();

        if (type == null)
        {
            if (!EditorApplication.isCompiling) Debug.LogWarning("Check the class name in the MonoScript '" + script.name + "'!");

            return result;
        }

        BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;

        IEnumerable<MethodInfo> methods = type.GetMethods(bindingFlags);

        Type requiredReturnType = booleanReturnType ? typeof(bool) : typeof(void);

        Func<MethodInfo, bool> methodIsValid = (mi) => mi.ReturnType == requiredReturnType && !mi.IsGenericMethod && HasValidParameters(mi);

        result = new List<MethodInfo>(methods.Where(methodIsValid));

        return result;
    }

    static bool HasValidParameters(MethodInfo m)
    {
        Type[] methodSignature = m.GetParameters().Select(p => p.ParameterType).ToArray();

        foreach (Type[] supportedSignature in SupportedMethodSignatures)
        {
            if (supportedSignature.SequenceEqual(methodSignature)) return true;
        }

        return false;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();

        propTarget     = property.FindPropertyRelative("script");
        propMethodInfo = property.FindPropertyRelative("methodInfo");
        propArgument   = property.FindPropertyRelative("argument");

        MonoScript monoScript                         = propTarget.GetValue<MonoScript>();
        SerializableMethodInfo serializableMethodInfo = propMethodInfo.GetValue<SerializableMethodInfo>();

        bool classKnown = !string.IsNullOrEmpty(serializableMethodInfo.DeclaringTypeName);

        if (monoScript == null && !propTarget.IsMissing()) return (EditorGUIUtility.singleLineHeight + padding) * 1;
        if (propTarget.IsMissing() && classKnown)          return (EditorGUIUtility.singleLineHeight + padding) * 1 + 40;
        if (!serializableMethodInfo.IsValid)               return (EditorGUIUtility.singleLineHeight + padding) * 2;

        int numParams = serializableMethodInfo.NumParameters;

        if (HasConversationInfoParam(serializableMethodInfo)) numParams -= 1;

        return (EditorGUIUtility.singleLineHeight + padding) * (2 + numParams);
    }

    bool HasConversationInfoParam(MethodInfo methodInfo)
    {
        ParameterInfo[] @params = methodInfo.GetParameters();

        foreach (ParameterInfo param in @params)
        {
            if (param.ParameterType == typeof(ConversationNode)) return true;
        }

        return false;
    }
}

} // of namespace BrightBit
