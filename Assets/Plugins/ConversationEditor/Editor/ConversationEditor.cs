using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BrightBit
{

public class ConversationEditor : EditorWindow
{
    [SerializeField] TreeViewState  treeViewState;
    static ConversationTreeViewData treeViewData;
    static ConversationTreeView     treeView;

    static string[] languages;

    TextArea textArea = new TextArea();

    Color narrationColor;
    Color optionColor;

    const int POPUP_ADD        = 1 << 0;
    const int POPUP_COPY       = 1 << 1;
    const int POPUP_CUT        = 1 << 2;
    const int POPUP_PASTE      = 1 << 3;
    const int POPUP_PASTE_LINK = 1 << 4;
    const int POPUP_MOVE_UP    = 1 << 5;
    const int POPUP_MOVE_DOWN  = 1 << 6;

    static GUIContent[] ModeToggles = new GUIContent[]
    {
        new GUIContent("Condition", "..."),
        new GUIContent("Action", "..."),
        new GUIContent("Data", "..."),
        new GUIContent("Comments", "...")
    };

    Vector2 nodeTextAreaScrollPosition;
    Vector2 commentsTextAreaScrollPosition;

    Action[] modeActions = null;

    int mode;

    bool focusTextArea             = false;
    bool unfinishedSelectionChange = false;

    [SerializeField]
    Conversation selectedConversation  = null;
    Dictionary<Conversation, ConversationTreeViewNode> clipboardEntries = new Dictionary<Conversation, ConversationTreeViewNode>();

    const string CURRENT_LANGUAGE = "BrightBit.ConversationEditor.CurrentLanguage";

    Conversation.Language CurrentLanguage
    {
        get { return (Conversation.Language) EditorPrefs.GetInt(CURRENT_LANGUAGE); }
        set { EditorPrefs.SetInt(CURRENT_LANGUAGE, (int) value);                   }
    }

    static ConversationEditor()
    {
        languages = Enum.GetNames(typeof(Conversation.Language)).Select(l => l.ToTitleCase()).ToArray();
    }

    [MenuItem ("Window/BrightBit's Conversation Editor")]
    static void ShowWindow()
    {
        ((ConversationEditor)EditorWindow.GetWindow(typeof(ConversationEditor))).Initialise();
    }

    void OnProjectChange()
    {
        Repaint();
    }

    void OnSelectionChange()
	{
		UnityEngine.Object selection = Selection.activeObject;

		if (selection != selectedConversation && selection is Conversation)
		{
			selectedConversation = Selection.activeObject as Conversation;

            if (!AssetDatabase.Contains(selectedConversation)) unfinishedSelectionChange = true;

            FinishSelectionChange();
		}
	}

    void FinishSelectionChange()
    {
        Initialise();

        treeViewData  = new ConversationTreeViewData(treeView, selectedConversation);
        treeView.Data = treeViewData;

        Repaint();
    }

    void OnDoubleClick(TreeViewNode node)
    {
        ConversationTreeViewNode ctvn = node as ConversationTreeViewNode;

        if (ctvn.IsLink) treeView.SetSelection(ctvn.LinkTarget);
        else             treeView.SetExpanded(node, !treeView.IsExpanded(node));
    }

    void OnFocus()
	{
		OnSelectionChange();
	}

    public TreeView ConversationTreeView
    {
        get
        {
            Initialise();

            return treeView;
        }
    }

    void Initialise()
    {
        titleContent = new GUIContent("Conversation");

        if (treeViewState == null) treeViewState = new TreeViewState();
        if (treeView == null)
        {
            treeView      = new ConversationTreeView(this, treeViewState);
            treeViewData  = new ConversationTreeViewData(treeView, selectedConversation);
            treeView.Data = treeViewData;

            treeView.OnContextClick      += OnContextClick;
            treeView.OnItemDoubleClicked += OnDoubleClick;
            //treeView.OnKeyboard          += OnKeyboard;
        }

        minSize = new Vector2(620, 300);

        modeActions = new Action[]
        {
            new Action(DrawCondition),
            new Action(DrawAction),
            new Action(DrawData),
            new Action(DrawComments),
        };
    }

    void OnAddNode(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        if (active == null) throw new System.Exception("OnAdd() : There is no selection to add a node to.");

        ConversationNode newNode = null;

        string example = "<Enter your text here>";

        if (active.Node is Narration) newNode = Option.Create(selectedConversation);
        else                          newNode = Narration.Create(selectedConversation);

        newNode.SetText(CurrentLanguage, example);

        ConversationTreeViewNode node = new ConversationTreeViewNode(treeView, newNode, treeViewData.CreateID(), false);

        active.Node.AddChild(newNode);
        active.AddChild(node);

        treeViewData.ForceUpdate();

        treeView.SetExpanded(node, true);
        treeView.SetSelection(node);

        focusTextArea = true;
    }

    void OnCopyNode(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        clipboardEntries[selectedConversation] = active;
    }

    void OnCutNode(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        ConversationTreeViewNode cutNode = treeViewData.OnCutNode(active);

        if (cutNode != null)
        {
            clipboardEntries[selectedConversation] = cutNode;
            treeView.AdvanceSelection(+1);
        }

        treeViewData.ForceUpdate();
        treeViewData.ForceUpdate();
    }

    void OnPaste(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        treeViewData.CreateCopyAndAdd(clipboardEntries.GetValueOrDefault(selectedConversation), active);

        treeViewData.ForceUpdate();
    }

    void OnPasteAsLink(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        treeViewData.CreateLink(active, clipboardEntries.GetValueOrDefault(selectedConversation));

        treeViewData.ForceUpdate();
    }

    void OnMoveUp(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        active.MoveUp();

        treeViewData.ForceUpdate();
    }

    void OnMoveDown(object selection)
    {
        ConversationTreeViewNode active = (ConversationTreeViewNode) selection;

        active.MoveDown();

        treeViewData.ForceUpdate();
    }

    void OnGUI()
    {
        // GUIHacks.BeginProSkin(position); // for testing purposes

        Initialise();

        if (selectedConversation == null)
		{
			GUILayout.Space(10);
			GUILayout.Label("No conversation selected", EditorStyles.centeredGreyMiniLabel);
			return;
		}
        else if (AssetDatabase.Contains(selectedConversation) && unfinishedSelectionChange)
        {
            unfinishedSelectionChange = false;
            FinishSelectionChange();
        }

        if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed") Repaint();

        EditorGUILayout.BeginVertical(GUI.skin.box);

            DrawHeader();

            EditorGUILayout.BeginVertical(GUI.skin.box);
                ConversationTreeView.OnGUILayout(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(100));
            EditorGUILayout.EndVertical();

            DrawFooter();

            GUILayout.Space(2);

        EditorGUILayout.EndVertical();

        // GUIHacks.EndProSkin();
    }

    void DrawHeader() // draw button with name of currently selected conversation
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUIStyle.none);

            GUILayout.Space(1f);

            GUIStyle style = new GUIStyle("PreButton") { stretchWidth = true, alignment = TextAnchor.MiddleCenter };

            if (GUILayout.Button(selectedConversation.name, style))
            {
                Selection.activeObject = selectedConversation;
                EditorGUIUtility.PingObject(selectedConversation);
            }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    void DrawFooter() // draw text area and "tabs" for the content of ConversationNodes
    {
        EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.MaxHeight(170));

            GUILayout.Space(1);

            ConversationTreeViewNode active = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

            EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box) { padding = new RectOffset(8, 8, 8, 8) }, GUILayout.ExpandHeight(true));

            GUI.SetNextControlName("BrightBit.ConversationEditor.TextArea");

            using (new EditorGUI.DisabledScope(active == null || active.IsRoot || active.IsLink))
            {
                string content = active != null ? active.GetText(CurrentLanguage) : string.Empty;

                content = textArea.OnGUILayout(content, ref nodeTextAreaScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                if (active != null) active.SetText(CurrentLanguage, content);

                EditorUtility.SetDirty(selectedConversation);

                if (focusTextArea && Event.current.type == EventType.Repaint)
                {
                    EditorGUI.FocusTextInControl("BrightBit.ConversationEditor.TextArea");
                    focusTextArea = false;
                    Event.current.Use();
                }

                GUILayout.Space(4);
            }

            using (new EditorGUI.DisabledScope(active == null || active.IsLink))
            {
                CurrentLanguage = (Conversation.Language) EditorGUILayout.Popup("Current Language", (int) CurrentLanguage, languages);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(new GUIStyle(GUI.skin.box) { padding = new RectOffset(8, 8, 8, 8) }, GUILayout.Width(380), GUILayout.ExpandHeight(true));

                if (active != null && active.IsRoot) DrawRootNodeSettings();
                else if (active != null)
                using (new EditorGUI.DisabledScope(active.IsLink))
                {
                    mode = GUILayout.Toolbar(mode, ModeToggles, "LargeButton", GUILayout.ExpandWidth(true));

                    GUILayout.Space(5);

                    modeActions[mode]();
                }

            EditorGUILayout.EndVertical();

            GUILayout.Space(1);

        EditorGUILayout.EndHorizontal();
    }

    void DrawRootNodeSettings()
    {
        ConversationTreeViewNode selection = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

        if (selection == null) return;

        RootNode node = selection.Node as RootNode;

        if (node == null) return;

        treeView.DisplayLanguage = (Conversation.Language) EditorGUILayout.Popup("Display Language", (int) treeView.DisplayLanguage, languages);
        treeView.NarrationColor  = EditorGUILayout.ColorField("Narration Color", treeView.NarrationColor);
        treeView.OptionColor     = EditorGUILayout.ColorField("Option Color", treeView.OptionColor);
        treeView.LinkColor       = EditorGUILayout.ColorField("Link Color", treeView.LinkColor);
        // treeView.HideNodeIDs     = EditorGUILayout.Toggle("Hide Node IDs", treeView.HideNodeIDs); // for debugging only
    }

    void DrawCondition()
    {
        ConversationTreeViewNode selection = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

        if (selection == null) return;

        ConversationNode node = selection.Node as ConversationNode;

        if (node == null) return;

        SerializedObject target = new SerializedObject(node);

        EditorGUILayout.PropertyField(target.FindProperty("showCondition"), GUILayout.ExpandWidth(true));

        target.ApplyModifiedProperties();
    }

    void DrawAction()
    {
        ConversationTreeViewNode selection = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

        if (selection == null) return;

        ConversationNode node = selection.Node as ConversationNode;

        if (node == null) return;

        SerializedObject target = new SerializedObject(node);

        EditorGUILayout.PropertyField(target.FindProperty("showAction"), GUILayout.ExpandWidth(true));

        target.ApplyModifiedProperties();
    }

    void DrawData()
    {
        ConversationTreeViewNode active = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

        if (active == null || active.IsRoot) return;

        if (active.IsNarration)
        {
            active.Tag = EditorGUILayout.TextField(active.IsNarration ? "Narration Tag" : "Option Tag", active.Tag, GUILayout.ExpandWidth(true));
        }

        EditorGUILayout.TextField("User Data", "");

        SerializedObject target = new SerializedObject(active.Node as ConversationNode);

        EditorGUILayout.PropertyField(target.FindProperty("audioClip"), new GUIContent("Common Audio"), GUILayout.ExpandWidth(true));

        SerializedProperty audioClip = target.FindProperty("audioClips").GetArrayElementAtIndex((int) CurrentLanguage);

        EditorGUILayout.PropertyField(audioClip, new GUIContent("Language Audio"), GUILayout.ExpandWidth(true));

        target.ApplyModifiedProperties();
    }

    void DrawComments()
    {
        ConversationTreeViewNode active = ConversationTreeView.GetSelection() as ConversationTreeViewNode;

        string oldComment = active != null ? active.Comments : string.Empty;
        string newComment = textArea.OnGUILayout(oldComment, ref commentsTextAreaScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        if (active != null) active.Comments = newComment;
    }

    void OnContextClick(TreeViewNode node)
    {
        ConversationTreeViewNode active = node as ConversationTreeViewNode;

        GenericMenu menu = new GenericMenu();

        int popupElements = POPUP_ADD | POPUP_COPY | POPUP_CUT | POPUP_PASTE | POPUP_PASTE_LINK | POPUP_MOVE_UP | POPUP_MOVE_DOWN;

        ConversationTreeViewNode clipboard = clipboardEntries.GetValueOrDefault(selectedConversation);

        if (active == null) popupElements = 0;
        else
        {
            if (active.ID == treeViewData.Root.ID)                  popupElements &= ~(POPUP_COPY | POPUP_CUT | POPUP_PASTE_LINK);
            if (clipboard != null && clipboard.Parent == null)      popupElements &= ~(POPUP_PASTE_LINK);
            if (clipboard == null)                                  popupElements &= ~(POPUP_PASTE | POPUP_PASTE_LINK);
            else if ( clipboard.IsNarration &&  active.IsNarration) popupElements &= ~(POPUP_PASTE | POPUP_PASTE_LINK);
            else if (!clipboard.IsNarration && !active.IsNarration) popupElements &= ~(POPUP_PASTE | POPUP_PASTE_LINK);
            if (!active.CanMoveUp())                                popupElements &= ~(POPUP_MOVE_UP);
            if (!active.CanMoveDown())                              popupElements &= ~(POPUP_MOVE_DOWN);
        }

        menu.AddItem(new GUIContent("Add"),           false, (popupElements & POPUP_ADD)        != 0 ? (GenericMenu.MenuFunction2) OnAddNode     : null, active);
        menu.AddItem(new GUIContent("Copy"),          false, (popupElements & POPUP_COPY)       != 0 ? (GenericMenu.MenuFunction2) OnCopyNode    : null, active);
        menu.AddItem(new GUIContent("Cut"),           false, (popupElements & POPUP_CUT)        != 0 ? (GenericMenu.MenuFunction2) OnCutNode     : null, active);
        menu.AddItem(new GUIContent("Paste"),         false, (popupElements & POPUP_PASTE)      != 0 ? (GenericMenu.MenuFunction2) OnPaste       : null, active);
        menu.AddItem(new GUIContent("Paste As Link"), false, (popupElements & POPUP_PASTE_LINK) != 0 ? (GenericMenu.MenuFunction2) OnPasteAsLink : null, active);

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Move Up"),   false, (popupElements & POPUP_MOVE_UP)   != 0 ? (GenericMenu.MenuFunction2) OnMoveUp   : null, active);
        menu.AddItem(new GUIContent("Move Down"), false, (popupElements & POPUP_MOVE_DOWN) != 0 ? (GenericMenu.MenuFunction2) OnMoveDown : null, active);

        menu.ShowAsContext();
    }
}

} // of namespace BrightBit
