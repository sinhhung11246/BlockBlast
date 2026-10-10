using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace BlockBlast.Game
{
    /// <summary>Single pointer (mouse or first touch), working with either input backend.</summary>
    public static class PointerInput
    {
#if ENABLE_INPUT_SYSTEM
        public static bool Pressed => Pointer.current != null && Pointer.current.press.wasPressedThisFrame;
        public static bool Released => Pointer.current != null && Pointer.current.press.wasReleasedThisFrame;
        public static Vector2 ScreenPosition => Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
#else
        public static bool Pressed => Input.GetMouseButtonDown(0);
        public static bool Released => Input.GetMouseButtonUp(0);
        public static Vector2 ScreenPosition => Input.mousePosition;
#endif

        public static Vector2 WorldPosition(Camera cam) => cam.ScreenToWorldPoint(ScreenPosition);
    }
}
