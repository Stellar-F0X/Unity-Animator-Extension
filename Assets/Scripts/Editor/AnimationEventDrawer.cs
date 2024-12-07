using UnityEditor;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    [CustomPropertyDrawer(typeof(AnimationEvent))]
    public class AnimationEventDrawer : PropertyDrawer
    {
        private float _verticalPosition = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                #region Calculate PropertyField Rect

                SerializedProperty eventName = property.FindPropertyRelative("eventName");
                SerializedProperty eventHash = property.FindPropertyRelative("eventHash");
                SerializedProperty unityEvent = property.FindPropertyRelative("animationEvent");

                _verticalPosition = EditorGUIUtility.singleLineHeight;
                Rect stateNameRect = new Rect(position.x, _verticalPosition, position.width, EditorGUIUtility.singleLineHeight);
                _verticalPosition += EditorGUIUtility.singleLineHeight + 4;
                Rect stateHashRect = new Rect(position.x, position.y + _verticalPosition, position.width, EditorGUIUtility.singleLineHeight);
                _verticalPosition += EditorGUIUtility.singleLineHeight + 4;
                Rect stateEventRect = new Rect(position.x, position.y + _verticalPosition, position.width, EditorGUI.GetPropertyHeight(unityEvent));
                _verticalPosition += EditorGUI.GetPropertyHeight(unityEvent);

                #endregion

                #region Draw PropertyFields

                EditorGUI.PropertyField(stateNameRect, eventName);

                GUI.enabled = false;
                
                if (string.IsNullOrEmpty(eventName.stringValue) || string.IsNullOrWhiteSpace(eventName.stringValue))
                {
                    eventHash.intValue = 0;
                }
                else
                {
                    eventHash.intValue = Utility.StringToHash(eventName.stringValue);
                }

                EditorGUI.PropertyField(stateHashRect, eventHash);
                GUI.enabled = true;

                EditorGUI.PropertyField(stateEventRect, unityEvent);

                #endregion
            }
        }

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _verticalPosition;
        }
    }
}