using UnityEngine;

namespace RunAndJump.LevelCreator
{
    public class PaletteItem : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum Category
        {
            Misc,
            Collectables,
            Enemies,
            Blocks
        }

        public Category category = Category.Misc;
        public string itemName = string.Empty;
        public Object inspectedScript;
#endif
    }
}
