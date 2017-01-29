using UnityEngine;
using UnityEditor;

namespace RunAndJump.LevelCreator
{
    public class PaletteWindow : EditorWindow
    {
        public static PaletteWindow instance;

        public static void ShowPalette()
        {
            instance = GetWindow<PaletteWindow>();
            instance.titleContent = new GUIContent("Palette");
        }
    }
}