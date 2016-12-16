using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BrightBit
{

public class ConversationTreeView : TreeView
{
    const string EDITOR_PREFIX = "BrightBit.ConversationTreeView.";

    public ConversationTreeView(EditorWindow owner, TreeViewState state) : base(owner, state)
    {
        IconPadding = new Padding() { Left = 4.0f, Right = 4.0f };
    }

    public Color NarrationColor
    {
        get { return GetEditorPrefsColor("NarrationColor", Color.red); }
        set { SetEditorPrefsColor("NarrationColor", value);            }
    }

    public Color OptionColor
    {
        get { return GetEditorPrefsColor("OptionColor", Color.blue); }
        set { SetEditorPrefsColor("OptionColor", value);             }
    }

    public Color LinkColor
    {
        get { return GetEditorPrefsColor("LinkColor", Color.gray); }
        set { SetEditorPrefsColor("LinkColor", value);             }
    }

    public Conversation.Language DisplayLanguage
    {
        get { return (Conversation.Language) EditorPrefs.GetInt(EDITOR_PREFIX + "DisplayLanguage"); }
        set { EditorPrefs.SetInt(EDITOR_PREFIX + "DisplayLanguage", (int) value);                   }
    }

    public bool HideNodeIDs
    {
        get { return EditorPrefs.GetBool(EDITOR_PREFIX + "HideNodeIDs", true); }
        set { EditorPrefs.SetBool(EDITOR_PREFIX + "HideNodeIDs", value);       }
    }

    Color GetEditorPrefsColor(string name, Color defaultColor)
    {
        string completeName = EDITOR_PREFIX + name;

        float r = EditorPrefs.GetFloat(completeName + "_R", defaultColor.r);
        float g = EditorPrefs.GetFloat(completeName + "_G", defaultColor.g);
        float b = EditorPrefs.GetFloat(completeName + "_B", defaultColor.b);
        float a = EditorPrefs.GetFloat(completeName + "_A", defaultColor.a);

        return new Color(r, g, b, a);
    }

    void SetEditorPrefsColor(string name, Color targetColor)
    {
        string completeName = EDITOR_PREFIX + name;

        EditorPrefs.SetFloat(completeName + "_R", targetColor.r);
        EditorPrefs.SetFloat(completeName + "_G", targetColor.g);
        EditorPrefs.SetFloat(completeName + "_B", targetColor.b);
        EditorPrefs.SetFloat(completeName + "_A", targetColor.a);
    }
}

} // of namespace BrightBit
