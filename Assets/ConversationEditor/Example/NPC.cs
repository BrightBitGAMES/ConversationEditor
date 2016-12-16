using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BrightBit
{

public class NPC : MonoBehaviour
{
    [SerializeField] ConversationOwner conversationOwner;
    [SerializeField] Conversation.Language language = Conversation.Language.English;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !conversationOwner.IsTalking())
        {
            Player player = (Player) FindObjectOfType(typeof(Player));

            conversationOwner.StartTalking(player);
        }
    }

    void OnGUI()
    {
        if (!conversationOwner.IsTalking()) return;

        Narration currentNarration  = conversationOwner.CurrentNarration;
        List<Option> currentOptions = conversationOwner.CurrentOptions;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
        GUILayout.FlexibleSpace();

        GUILayout.Label(currentNarration.GetText(language), style, GUILayout.ExpandWidth(true));

        Option selection = null;

        foreach (Option option in currentOptions)
        {
            string text = option.GetText(language);

            if (GUILayout.Button(string.IsNullOrEmpty(text) ? "[CONTINUE]" : text, GUILayout.ExpandWidth(true))) selection = option;
        }

        GUILayout.Space(5.0f);

        if (selection != null)
        {
            conversationOwner.Advance(selection);

            if (!conversationOwner.CanAdvance && conversationOwner.IsTalking()) StartCoroutine(StopConversation());
        }

        GUILayout.EndVertical();
    }

    IEnumerator StopConversation()
    {
        yield return new WaitForSeconds(4.0f);

        conversationOwner.StopTalking();
    }
}

} // of namespace BrightBit
