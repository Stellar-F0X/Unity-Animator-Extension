using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace AnimatorExpansion
{
    [Serializable]
    public class AnimationEvent
    {
        public AnimationEvent(string eventName)
        {
            this.eventName = eventName;
            eventHash = Utility.StringToHash(eventName);
        }
        
        [HideInInspector]
        public int eventHash; 
        
        public string eventName;
        public UnityEvent animationEvent;
        

        public void Initialize(Animator animator)
        {
            
        }
    }
    
    
    
    #if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(AnimationEvent))]
    public class AnimationEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                SerializedProperty eventName = property.FindPropertyRelative("eventName");
                SerializedProperty eventHash = property.FindPropertyRelative("eventHash");
                SerializedProperty unityEvent = property.FindPropertyRelative("animationEvent");

                Rect stateNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                Rect stateHashRect = new Rect(position.x, position.y +  + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                Rect stateEventRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUI.GetPropertyHeight(unityEvent));

                EditorGUI.PropertyField(stateNameRect, eventName);
                GUI.enabled = false;
                eventHash.intValue = Utility.StringToHash(eventName.stringValue);
                EditorGUI.PropertyField(stateHashRect, eventHash);
                GUI.enabled = true;
                EditorGUI.PropertyField(stateEventRect, unityEvent);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty stateEventProperty = property.FindPropertyRelative("animationEvent");
            return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(stateEventProperty) + 4;
        }
    }
    
    #endif
}