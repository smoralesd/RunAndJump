using UnityEngine;
using UnityEditor;

namespace RunAndJump.LevelCreator
{
    [CustomEditor(typeof(Level))]
    public class LevelInspector: Editor
    {
        private Level _myTarget;

        private void OnEnable()
        {
            _myTarget = (Level)target;
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("The GUI of this inspector was modified.");
            EditorGUILayout.LabelField("The current level time is: " + _myTarget.TotalTime);
        }
    }
}
