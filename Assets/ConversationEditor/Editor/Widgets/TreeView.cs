using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightBit
{

public class TreeView
{
    public Action<Vector2> ScrollChanged            { get; set; }
    public Action<TreeViewNode> OnSelectionChanged  { get; set; }
    public Action<TreeViewNode> OnItemDoubleClicked { get; set; }
    public Action<TreeViewNode> OnContextClick      { get; set; }
    public Action OnKeyboard                        { get; set; }
    public Action ExpandedStateChanged              { get; set; }

    public struct Margin
    {
        public float Top    { get; set; }
        public float Bottom { get; set; }
        public float Left   { get; set; }
        public float Right  { get; set; }
    }

    public struct Padding
    {
        public float Left  { get; set; }
        public float Right { get; set; }

        public float Total
        {
            get
            {
                return Left + Right;
            }
        }
    }

    protected struct Range
    {
        public int First { get; private set; }
        public int Last  { get; private set; }

        public Range(int first, int last)
        {
            First = first;
            Last  = last;
        }
    }

    public float RowHeight     { get; protected set; }
    public float IconWidth     { get; protected set; }
    public float IndentWidth   { get; protected set; }
    public float FoldoutWidth  { get; protected set; }
    public float BaseIndent    { get; protected set; }

    public Margin RowMargin    { get; protected set; }
    public Padding IconPadding { get; protected set; }

    public TreeViewState State { get; private set; }

    TreeViewData data;
    public TreeViewData Data
    {
        get
        {
            return data;
        }

        set
        {
            if (data != null) data.OnExpandedStateChanged -= OnNodeWasExpandedOrCollapsed;

            data = value;

            if (data != null) data.OnExpandedStateChanged += OnNodeWasExpandedOrCollapsed;
        }
    }

    GUIStyle verticalScrollbarStyle   = null;
    GUIStyle horizontalScrollbarStyle = null;

    static Texture2D ScrollViewBackground = new Texture2D(1, 1, TextureFormat.RGBA32, false);

    int controlID;

    bool stopDrawingRows = false; // will be set to true, if a TreeViewNode collapses or expands

    EditorWindow owner;

    Rect guiRect;

    static TreeView()
    {
        ScrollViewBackground.SetPixel(0, 0, Color.white);
        ScrollViewBackground.Apply();

        ScrollViewBackground = Resources.Load("scrollViewBackground") as Texture2D;
    }

    public TreeView(EditorWindow owner, TreeViewState state)
    {
        controlID = GUIUtility.GetControlID(FocusType.Keyboard);

        this.owner = owner;
        this.State = state;

        // these settings try to mimic Unity's own internal TreeView

        RowHeight    = 16.0f;
        IconWidth    = 16.0f;
        IndentWidth  = 14.0f;
        FoldoutWidth = 12.0f;
        BaseIndent   =  2.0f;
        IconPadding  = new Padding() { Right = 2.0f };
    }

    void OnNodeWasExpandedOrCollapsed()
    {
        stopDrawingRows = true;
    }

    public void SetExpanded(TreeViewNode node, bool expand)
    {
        Data.SetExpanded(node.ID, expand);
    }

    public bool IsExpanded(TreeViewNode node)
    {
        return Data.IsExpanded(node.ID);
    }

    public void SetSelection(TreeViewNode node)
    {
        int previousID = State.SelectedNodeID;

        if (node != null)
        {
            Data.Highlight(node.ID);

            int row = Data.GetIndexOfExpandedRowOrItsChild(node.ID);

            if (row != -1)
            {
                State.SelectedNodeID = node.ID;

                Rect rowRect = CalculateRect(row, 0);

                float scrollBarHeight = horizontalScrollbarStyle.CalcSize(GUIContent.none).y + horizontalScrollbarStyle.margin.vertical;
                float offset          = GetMaxWidth() > guiRect.width ? scrollBarHeight : 0.0f;

                float top    = rowRect.y;
                float bottom = top + rowRect.height - guiRect.height + offset;

                if (State.ScrollPos.y < bottom)   State.ScrollPos = new Vector2(State.ScrollPos.x, bottom);
                else if (State.ScrollPos.y > top) State.ScrollPos = new Vector2(State.ScrollPos.x, top);
            }
            else State.SelectedNodeID = 0;
        }
        else State.SelectedNodeID = 0;

        if (OnSelectionChanged != null && previousID != State.SelectedNodeID) OnSelectionChanged(node);
    }

    public void AdvanceSelection(int delta)
    {
        List<TreeViewNode> rows = Data.ExpandedRowsAndTheirChildren;

        if (rows.Count == 0) return;

        delta = Mathf.Clamp(delta, -rows.Count, rows.Count);

        int index = rows.FindIndex(node => node.ID == State.SelectedNodeID);
        index     = index >= 0 ? index : 0;
        index     = Mathf.Clamp(index + delta, 0, rows.Count - 1);

        SetSelection(rows[index]);
    }

    public TreeViewNode GetSelection()
    {
        List<TreeViewNode> rows = Data.ExpandedRowsAndTheirChildren;

        if (rows.Count == 0) return null;

        int index = rows.FindIndex(node => node.ID == State.SelectedNodeID);

        return rows[index >= 0 ? index : 0];

        // return Data.FindNode(State.SelectedNodeID);
    }

    protected virtual Range VisibleRowsRange
    {
        get
        {
            if (Data.ExpandedRowsAndTheirChildren.Count == 0) return new Range(-1, -1);

            float y      = State.ScrollPos.y;
            float height = guiRect.height;

            int firstRowVisible = (int) Mathf.Floor((y - RowMargin.Top) / RowHeight);
            int lastRowVisible  = firstRowVisible + (int) Mathf.Ceil(height / RowHeight);

            firstRowVisible = Mathf.Max(firstRowVisible, 0);
            lastRowVisible  = Mathf.Min(lastRowVisible, Data.ExpandedRowsAndTheirChildren.Count - 1);

            return new Range(firstRowVisible, lastRowVisible);
        }
    }

    protected virtual Rect CalculateRect(int row, float rowWidth)
    {
        float x = 0.0f;
        float y = (float) row * RowHeight + RowMargin.Top;

        return new Rect(x, y, rowWidth, RowHeight);
    }

    protected float GetMaxWidth()
    {
        List<TreeViewNode> rows = Data.ExpandedRowsAndTheirChildren;

        float maxWidth = 0.0f;

        foreach (TreeViewNode current in rows)
        {
            float nodeWidth = current.GetWidth();

            if (nodeWidth > maxWidth) maxWidth = nodeWidth;
        }

        return maxWidth;
    }

    protected virtual Rect ViewRect
    {
        get
        {
            List<TreeViewNode> rows = Data.ExpandedRowsAndTheirChildren;

            float width  = GetMaxWidth();
            float height = (float) rows.Count * RowHeight + RowMargin.Top + RowMargin.Bottom;

            return new Rect(0f, 0f, width, height);
        }
    }

    public void OnGUILayout(params GUILayoutOption[] opts)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, opts);

        OnGUI(position);
    }

    public void OnGUI(Rect rect)
    {
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            GUIUtility.keyboardControl = controlID;
        }

        GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
        //scrollViewStyle.normal.background = ScrollViewBackground;

        GUIStyle lastStyle = GUI.skin.scrollView;
        GUI.skin.scrollView = scrollViewStyle;

        verticalScrollbarStyle   = new GUIStyle(GUI.skin.verticalScrollbar);
        horizontalScrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);

        verticalScrollbarStyle.margin   = new RectOffset(2, 0, 0, 0);
        horizontalScrollbarStyle.margin = new RectOffset(0, 0, 2, 0);

        guiRect       = rect;
        Rect viewRect = ViewRect;

        State.ScrollPos = GUI.BeginScrollView(guiRect, State.ScrollPos, viewRect, horizontalScrollbarStyle, verticalScrollbarStyle);

        Range rows = VisibleRowsRange;

        if (rows.Last >= 0)
        {
            float rowWidth = Mathf.Max(guiRect.width, viewRect.width);

            DrawVisibleRows(rows, rowWidth);
        }

        GUI.EndScrollView();

        GUI.skin.scrollView = lastStyle;

        HandleKeyboard();
    }

    void DrawVisibleRows(Range visibleRows, float rowWidth)
    {
        stopDrawingRows = false;

        for (int currentRow = visibleRows.First; currentRow <= visibleRows.Last; ++currentRow)
        {
            float offset = CalculateRect(currentRow, rowWidth).y - State.ScrollPos.y;

            if (offset <= guiRect.height)
            {
                DrawRow(Data.GetRow(currentRow), currentRow, rowWidth);

                if (stopDrawingRows) return;
            }
        }
    }

    void DrawRow(TreeViewNode node, int row, float rowWidth)
    {
        Rect rowRect = CalculateRect(row, rowWidth);

        bool selected = State.SelectedNodeID == node.ID;
        bool hasFocus = EditorWindow.focusedWindow == owner && GUIUtility.keyboardControl == controlID;

        EditorGUIUtility.SetIconSize(Vector2.one * IconWidth);

        if (node != null) node.Draw(rowRect, selected, hasFocus);

        EditorGUIUtility.SetIconSize(Vector2.zero);

        HandleNodeEvents(rowRect, node);
    }

    public void HandleNodeEvents(Rect rowRect, TreeViewNode node)
    {
        Event current = Event.current;

        if (!rowRect.Contains(current.mousePosition)) return;

        switch (current.type)
        {
            case EventType.MouseDown:

                if (current.IsLeftMouseButton() || current.IsRrightMouseButton())
                {
                    SetSelection(node);

                    if (current.IsLeftMouseButton() && current.IsDoubleClick())
                    {
                        if (OnItemDoubleClicked != null) OnItemDoubleClicked(node);
                    }

                    current.Use();
                }
            break;

            case EventType.ContextClick:

                SetSelection(node);

                if (OnContextClick != null) OnContextClick(node);

                current.Use();
            break;
        }
    }

    void HandleKeyboard()
    {
        if (controlID != GUIUtility.keyboardControl || !GUI.enabled) return;

        if (OnKeyboard != null) OnKeyboard();

        TreeViewNode selection = null;

        if (Event.current.type == EventType.KeyDown)
        {
            KeyCode keyCode = Event.current.keyCode;

            switch (keyCode)
            {
                case KeyCode.UpArrow   : AdvanceSelection(-1);           Event.current.Use(); break;
                case KeyCode.DownArrow : AdvanceSelection(+1);           Event.current.Use(); break;
                case KeyCode.Home      : AdvanceSelection(int.MinValue); Event.current.Use(); break;
                case KeyCode.End       : AdvanceSelection(int.MaxValue); Event.current.Use(); break;

                case KeyCode.LeftArrow :

                    selection = GetSelection();

                    if (selection != null && Data.IsExpandable(selection))
                    {
                        if (Data.IsExpanded(selection)) Data.SetExpanded(selection, false);
                        else                            AdvanceSelection(-1);
                    }
                    else AdvanceSelection(-1);

                    Event.current.Use();
                break;

                case KeyCode.RightArrow :

                    selection = GetSelection();

                    if (selection != null && Data.IsExpandable(selection))
                    {
                        if (!Data.IsExpanded(selection)) Data.SetExpanded(selection, true);
                        else                             AdvanceSelection(+1);
                    }
                    else AdvanceSelection(+1);

                    Event.current.Use();
                break;
            }
        }
    }
}

} // of namespace BrightBit
