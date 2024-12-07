using System;
using AnimatorExpansion.Parameters;
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
                SerializedProperty eventName = property.FindPropertyRelative("eventName");
                SerializedProperty eventHash = property.FindPropertyRelative("eventHash");
                SerializedProperty sendType = property.FindPropertyRelative("sendType");

                _verticalPosition = 0f;

                position.y += 5f;
                Rect stateNameRect = GetRect(position, 0.7f, 0);
                eventName.stringValue = EditorGUI.TextField(stateNameRect, eventName.stringValue);

                using (new EditorGUI.DisabledScope(true))
                {
                    eventHash.intValue = string.IsNullOrWhiteSpace(eventName.stringValue) ? 0 : Utility.StringToHash(eventName.stringValue);
                    Rect stateHashRect = GetRect(position, 0.3f, 0, offsetX: position.width * 0.7f + 5, subtractWidth: 5);
                    EditorGUI.PropertyField(stateHashRect, eventHash, GUIContent.none);
                }

                _verticalPosition += EditorGUIUtility.singleLineHeight + 10;
                Rect stateSendTypeRect = GetRect(position, 0.15f, _verticalPosition);
                EditorGUI.PropertyField(stateSendTypeRect, sendType, GUIContent.none);

                if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Range)
                {
                    this.DrawRangeTrigger(position, property);
                }
                else if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Specific)
                {
                    this.DrawSpecificTrigger(position, property);
                }
            }
        }

        
        private void DrawRangeTrigger(Rect position, SerializedProperty property)
        {
            SerializedProperty triggerTime = property.FindPropertyRelative("repeatTriggerRange");
            SerializedProperty min = triggerTime.FindPropertyRelative("min");
            SerializedProperty max = triggerTime.FindPropertyRelative("max");

            Rect minSliderFieldRect = GetRect(position, 0, _verticalPosition, offsetX: position.width * 0.15f + 10, width: 50);
            EditorGUI.FloatField(minSliderFieldRect, GUIContent.none, min.floatValue);

            Rect maxSliderFieldRect = GetRect(position, 0, _verticalPosition, offsetX: position.width - 60, width: 50);
            EditorGUI.FloatField(maxSliderFieldRect, GUIContent.none, max.floatValue);

            
            float minFloatValue = min.floatValue;
            float maxFloatValue = max.floatValue;
            float sliderWidth = position.width - (position.x + position.width * 0.15f + 90);
            Rect sliderFieldRect = GetRect(position, 0, _verticalPosition, offsetX: position.width * 0.15f + 65, width: sliderWidth);
            EditorGUI.MinMaxSlider(sliderFieldRect, ref minFloatValue, ref maxFloatValue, 0f, 1f);
            min.floatValue = (float)Math.Round(minFloatValue, 3);
            max.floatValue = (float)Math.Round(maxFloatValue, 3);
        }

        
        private void DrawSpecificTrigger(Rect position, SerializedProperty property)
        {
            SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");
            float triggerTimeWidth = position.width * 0.85f;
            Rect stateSliderRect = GetRect(position, 0, _verticalPosition, offsetX: position.width * 0.15f + 10, width: triggerTimeWidth - 20);
            EditorGUI.Slider(stateSliderRect, triggerTime, 0f, 1f, GUIContent.none);
        }

        
        private Rect GetRect(Rect basePosition, float widthPercentage, float verticalOffset, float offsetX = 0, float subtractWidth = 0, float width = 0)
        {
            float rectWidth = width == 0 ? basePosition.width * widthPercentage - subtractWidth : width;
            return new Rect(basePosition.x + offsetX, basePosition.y + verticalOffset, rectWidth, EditorGUIUtility.singleLineHeight);
        }

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _verticalPosition;
        }
    }
}