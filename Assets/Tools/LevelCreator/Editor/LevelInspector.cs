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
            DrawLevelDataGUI();
        }

        private void DrawLevelDataGUI()
        {
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);

            _myTarget.TotalTime = EditorGUILayout.IntField(" Total Time", Mathf.Max(0, _myTarget.TotalTime));
            _myTarget.Gravity = EditorGUILayout.FloatField(" Gravity", Mathf.Max(0, _myTarget.Gravity));
            _myTarget.Bgm = (AudioClip)EditorGUILayout.ObjectField(" Bgm", _myTarget.Bgm, typeof(AudioClip), false);
            _myTarget.Background = (Sprite)EditorGUILayout.ObjectField(" Background", _myTarget.Background, typeof(Sprite), false);
        }
    }
}
