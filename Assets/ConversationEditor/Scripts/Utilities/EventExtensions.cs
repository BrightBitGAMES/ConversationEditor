using UnityEngine;

namespace BrightBit
{

public static class EventExtensions
{
    public static bool IsLeftMouseButton(this Event e)       { return e.button == 0; }
    public static bool IsRrightMouseButton(this Event e)     { return e.button == 1; }
    public static bool IsMiddleMouseButton(this Event e)     { return e.button == 2; }

    public static bool IsLeftMouseButtonDown(this Event e)   { return e.type == EventType.MouseDown && IsLeftMouseButton(e);   }
    public static bool IsRightMouseButtonDown(this Event e)  { return e.type == EventType.MouseDown && IsRrightMouseButton(e); }
    public static bool IsMiddleMouseButtonDown(this Event e) { return e.type == EventType.MouseDown && IsMiddleMouseButton(e); }

    public static bool IsDoubleClick(this Event e)           { return e.clickCount == 2; }
}

} // of namespace BrightBit
