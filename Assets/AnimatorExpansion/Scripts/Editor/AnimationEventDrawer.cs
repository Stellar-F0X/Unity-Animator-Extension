using UnityEditor;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    [CustomPropertyDrawer(typeof(AnimationEvent))]
    public class AnimationEventDrawer : PropertyDrawer
    {
        private float _verticalPosition = 0;

        private bool _foldOut;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                #region Calculate PropertyField Rect

                SerializedProperty eventName = property.FindPropertyRelative("eventName");
                SerializedProperty eventHash = property.FindPropertyRelative("eventHash");
                SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");

                float nameWidth = position.width * 0.7f;
                float hashWidth = position.width * 0.3f;

                _verticalPosition = 5f;
                Rect stateNameRect = new Rect(position.x,                 position.y + _verticalPosition, nameWidth,     EditorGUIUtility.singleLineHeight);
                Rect stateHashRect = new Rect(position.x + nameWidth + 5, position.y + _verticalPosition, hashWidth - 5, EditorGUIUtility.singleLineHeight);
                _verticalPosition += EditorGUIUtility.singleLineHeight;
                
                Rect stateEventRect = new Rect(position.x,                position.y + _verticalPosition, position.width, EditorGUIUtility.singleLineHeight);
                _verticalPosition += EditorGUIUtility.singleLineHeight;

                #endregion

                
                #region Draw PropertyFields
                
                eventName.stringValue = EditorGUI.TextField(stateNameRect, eventName.stringValue);
                
                using (new EditorGUI.DisabledScope(true))
                {
                    if (string.IsNullOrEmpty(eventName.stringValue) || string.IsNullOrWhiteSpace(eventName.stringValue))
                    {
                        eventHash.intValue = 0;
                    }
                    else
                    {
                        eventHash.intValue = Utility.StringToHash(eventName.stringValue);
                    }

                    EditorGUI.PropertyField(stateHashRect, eventHash, GUIContent.none);
                }

                EditorGUI.Slider(stateEventRect, triggerTime, 0f, 1f, GUIContent.none);
                
                #endregion
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _verticalPosition;
        }
    }
}