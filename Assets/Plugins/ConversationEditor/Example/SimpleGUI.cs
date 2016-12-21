using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrightBit;

public class SimpleGUI : MonoBehaviour
{
    [SerializeField] Conversation.Language language = Conversation.Language.English;

    string narration;
    List<string> options;

    bool showGUI = false;

    void Awake()
    {
        Conversation.CurrentLanguage        = language;
        Conversation.NodeProcessor          = new SimpleNodeProcessor();
        Conversation.OnConversationStarted += this.OnConversationStarted;
        Conversation.OnOptionSelected      += this.OnOptionSelected;
        Conversation.OnConversationEnded   += this.OnConversationEnded;
    }

    void OnGUI()
    {        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        if (!showGUI)
        {
            GUILayout.Label("Press SPACE to start the conversation!", style, GUILayout.Width(Screen.width), GUILayout.ExpandWidth(true));
            return;
        }

        GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
        GUILayout.FlexibleSpace();

        GUILayout.Label(narration, style, GUILayout.ExpandWidth(true));

        for (int i = 0; i < options.Count; ++i)
        {
            if (GUILayout.Button(options[i], GUILayout.ExpandWidth(true)))
            {
                Conversation.Select(i);
            }
        }

        if (options.Count == 0 && GUILayout.Button("[END]", GUILayout.ExpandWidth(true))) Conversation.End();

        GUILayout.Space(5.0f);
        GUILayout.EndVertical();
    }

    void OnConversationStarted() { this.showGUI = true;  }
    void OnConversationEnded()   { this.showGUI = false; }

    void OnOptionSelected()
    {
        ConversationProgress progress = Conversation.CurrentProgress;

        narration = progress.CurrentNarrationText;
        options   = progress.CurrentOptionTexts;
    }
}
