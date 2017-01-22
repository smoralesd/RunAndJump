using UnityEngine;
using UnityEditor;

namespace RunAndJump.LevelCreator
{
    [CustomPropertyDrawer(typeof(TimeAttribute))]
    public class TimeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntField(
                    new Rect(position.x, position.y, position.width, position.height / 2),
                    label,
                    Mathf.Max(0, property.intValue));

                EditorGUI.LabelField(
                    new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2),
                    " ",
                    TimeFormat(property.intValue));
            }
            else
            {
                EditorGUI.HelpBox(
                    position,
                    "To use the Time Attribute <" + label.text + "> must be an int!",
                    MessageType.Error);
            }
        }

        private string TimeFormat(int totalInSeconds)
        {
            TimeAttribute time = attribute as TimeAttribute;

            if (time.DisplayHours)
            {
                int hours = totalInSeconds / (60 * 60);
                int minutes = ((totalInSeconds % (60 * 60)) / 60);
                int seconds = totalInSeconds % 60;

                return string.Format("{0}:{1}:{2}", hours, minutes.ToString().PadLeft(2, '0'), seconds.ToString().PadLeft(2, '0'));
            }
            else
            {
                int minutes = totalInSeconds / 60;
                int seconds = totalInSeconds % 60;

                return string.Format("{0}:{1}", minutes, seconds.ToString().PadLeft(2, '0'));
            }
        }
    }
}