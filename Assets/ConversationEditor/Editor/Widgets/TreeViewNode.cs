using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightBit
{

public abstract class TreeViewNode
{
    static GUIStyle FoldoutStyle   = new GUIStyle("IN Foldout");
    static GUIStyle RowStyle       = new GUIStyle("PR Label");
    static GUIStyle SelectionStyle = new GUIStyle("PR Label");

    static GUIContent tempGUIContent = new GUIContent();

    protected TreeView owner;

    public virtual int ID              { get; protected set; }
    public virtual int Depth           { get; protected set; }
    public abstract string DisplayName { get; }

    public TreeViewNode Parent { get; protected set; }

    public virtual bool HasChildren
    {
        get
        {
            return Children != null && Children.Count > 0;
        }
    }

    public virtual int NumChildren
    {
        get
        {
            return Children == null ? 0 : Children.Count;
        }
    }

    public virtual List<TreeViewNode> Children { get; private set; }

    public virtual Texture2D Icon   { get { return null;        } }
    public virtual Color LabelColor { get { return Color.black; } }
    public virtual object UserData  { get; set; }

    static TreeViewNode()
    {
        Texture2D background          = RowStyle.hover.background;
        RowStyle.onNormal.background  = background;
        RowStyle.onActive.background  = background;
        RowStyle.onFocused.background = background;
        RowStyle.alignment            = TextAnchor.MiddleLeft;
    }

    public TreeViewNode(TreeView owner, int id)
    {
        this.owner  = owner;
        this.ID     = id;
        this.Parent = null;
        this.Depth  = 0;
    }

    public void AddChild(TreeViewNode child)
    {
        if (Children == null) Children = new List<TreeViewNode>();

        if (child != null)
        {
            child.Parent = this;
            child.Depth  = Depth + 1;

            Children.Add(child);
        }
    }

    public void RemoveChild(TreeViewNode child)
    {
        if (child != null)
        {
            child.Parent = null;
            child.Depth  = -1;

            if (Children != null) Children.Remove(child);
        }
    }

    protected virtual float CalculateIndentToFoldout()
    {
        return owner.BaseIndent + (float) this.Depth * owner.IndentWidth;
    }

    protected virtual float CalculateIndentToIcon()
    {
        return this.CalculateIndentToFoldout() + owner.FoldoutWidth;
    }

    public float GetWidth()
    {
        float result = this.CalculateIndentToIcon();

        float minWidth;
        float maxWidth;

        tempGUIContent.text = this.DisplayName;

        RowStyle.CalcMinMaxWidth(tempGUIContent, out minWidth, out maxWidth);

        result += maxWidth;
        result += owner.BaseIndent; // base indent for right side, too

        return result;
    }

    public void Draw(Rect nodeRect, bool selected, bool hasFocus)
    {
        if (selected && Event.current.type == EventType.Repaint) SelectionStyle.Draw(nodeRect, false, false, true, hasFocus);

        string label = this.DisplayName.Split(new [] { '\r', '\n' }).FirstOrDefault();

        DrawFoldout(nodeRect);
        DrawIcon(nodeRect);
        DrawLabel(nodeRect, label, selected, hasFocus);
    }

    protected virtual void DrawFoldout(Rect nodeRect)
    {
        if (!owner.Data.IsExpandable(this)) return;

        Rect rect = nodeRect;
        
        rect.x     = this.CalculateIndentToFoldout();
        rect.width = owner.FoldoutWidth;

        EditorGUI.BeginChangeCheck();

        bool expand = GUI.Toggle(rect, owner.Data.IsExpanded(this), GUIContent.none, FoldoutStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (Event.current.alt) owner.Data.SetExpandedWithChildren(this, expand);
            else                   owner.Data.SetExpanded(this, expand);
        }
    }

    protected virtual void DrawIcon(Rect nodeRect)
    {
        if (Icon == null) return;

        Rect rect = nodeRect;

        rect.x     += this.CalculateIndentToIcon() + owner.IconPadding.Left;
        rect.width  = owner.IconWidth;
        rect.height = owner.IconWidth;

        GUI.DrawTexture(rect, this.Icon);
    }

    protected virtual void DrawLabel(Rect nodeRect, string label, bool selected, bool focused)
    {
        if (Event.current.type != EventType.Repaint) return;

        Rect rect = nodeRect;

        float contentIndent = this.CalculateIndentToIcon();

        rect.x     += contentIndent;
        rect.y     -= 1;
        rect.width -= contentIndent;

        RowStyle.padding.left = (int) (owner.IconWidth + owner.IconPadding.Total);
        RowStyle.normal.textColor = this.LabelColor;
        RowStyle.focused.textColor = RowStyle.normal.textColor;
        RowStyle.Draw(rect, label, false, false, selected, focused);
    }
}

} // of namespace BrightBit
