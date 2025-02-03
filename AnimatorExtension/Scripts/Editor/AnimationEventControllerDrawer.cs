using System;
using UnityEditor;
using UnityEngine;

namespace AnimatorExtension.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(AnimationEventController))]
    public class AnimationEventControllerDrawer : Editor
    {
        private int _originalIndentLevel = 0;
        
        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = _originalIndentLevel;
            SerializedProperty prop = serializedObject.FindProperty("debug");
            prop.boolValue = EditorGUILayout.Toggle(new GUIContent("Debug Mode"), prop.boolValue);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(3);

            if (Application.isPlaying)
            {
                AnimationEventController eventController = (AnimationEventController)target;
                EditorGUILayout.LabelField("Event Name", "Active");
                EditorGUI.indentLevel++;

                using (new EditorGUI.DisabledScope(true))
                {
                    foreach (var animEvent in eventController.eventList)
                    {
                        EditorGUILayout.Toggle(animEvent.callbackName, animEvent.enable);
                    }
                }
            }
        }

        public void OnEnable()
        {
            _originalIndentLevel = EditorGUI.indentLevel;
        }
    }
}