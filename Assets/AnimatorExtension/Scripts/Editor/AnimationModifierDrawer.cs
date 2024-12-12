using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace AnimatorExpansion.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(AnimationModifier))]
    public class AnimationModifierDrawer : Editor
    {
        private string[] _paramNameList;

        private bool _rangeModified;

        private ReorderableList _speedModifierList;
        private AnimatorController _controller;



        private void OnEnable()
        {
            AnimationUtility.GetAnimatorAndController(out _, out _controller);

            SerializedProperty speedProp = serializedObject.FindProperty("speedInfos");

            if (_controller && _controller.parameters.Length > 0)
            {
                List<string> newParamNameList = new List<string>();

                newParamNameList.Add("None");
                newParamNameList.AddRange(_controller
                    .parameters
                    .Where(p => p.type == AnimatorControllerParameterType.Float)
                    .Select(p => p.name)
                    .ToArray());

                _paramNameList = newParamNameList.ToArray();
            }

            _speedModifierList = new ReorderableList(serializedObject, speedProp, true, true, true, true);
            _speedModifierList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Speed Modifier List");
            _speedModifierList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 5;
            _speedModifierList.drawElementCallback = this.DrawSpeedModifierList;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Space(10);

            using (new EditorGUILayout.VerticalScope(GUI.skin.window))
            {
                SerializedProperty property = serializedObject.FindProperty("speedMultiplierParameterName");
                int selectedIndex = Array.IndexOf(_paramNameList, property.stringValue);
                selectedIndex = EditorGUILayout.Popup(selectedIndex.Equals(-1) ? 0 : selectedIndex, _paramNameList);

                if (_paramNameList.Length > selectedIndex && selectedIndex >= 0)
                {
                    property.stringValue = _paramNameList[selectedIndex];
                }
                else
                {
                    property.stringValue = _paramNameList[0];
                }

                GUILayout.Space(5);

                _speedModifierList.DoLayoutList();

                this.OrderBySpeedControlRangePart();

                this.serializedObject.ApplyModifiedProperties();
            }
        }


        private void DrawSpeedModifierList(Rect position, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = _speedModifierList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty controlRange = property.FindPropertyRelative("controlRange");
            SerializedProperty speedCurve = property.FindPropertyRelative("speedCurve");

            if (_controller)
            {
                position.y += 3f;

                SerializedProperty minProp = controlRange.FindPropertyRelative("min");
                SerializedProperty maxProp = controlRange.FindPropertyRelative("max");

                Rect minMaxRect = CustomEditorUtility.CalculateVariableRect(position, 0.1f);
                float minValue = Mathf.Clamp01(EditorGUI.FloatField(minMaxRect, minProp.floatValue));
                minProp.floatValue = minValue > maxProp.floatValue ? maxProp.floatValue : minValue;

                Rect maxMaxRect = CustomEditorUtility.CalculateVariableRect(position, 0.1f, position.width * 0.1f + 5);
                float maxValue = Mathf.Clamp01(EditorGUI.FloatField(maxMaxRect, maxProp.floatValue));
                maxProp.floatValue = maxValue > minProp.floatValue ? maxValue : minProp.floatValue;

                float offset = position.width * 0.2f;
                Rect curveRect = CustomEditorUtility.CalculateConstantRect(position, position.width - offset, offset, 10);
                speedCurve.animationCurveValue = EditorGUI.CurveField(curveRect, speedCurve.animationCurveValue);
            }
        }


        private void OrderBySpeedControlRangePart()
        {
            for (int i = 0; i < _speedModifierList.serializedProperty.arraySize - 1; i++)
            {
                SerializedProperty currProperty = _speedModifierList.serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty currControlRange = currProperty.FindPropertyRelative("controlRange");
                SerializedProperty currMinProp = currControlRange.FindPropertyRelative("min");
                SerializedProperty currMaxProp = currControlRange.FindPropertyRelative("max");

                SerializedProperty nextProperty = _speedModifierList.serializedProperty.GetArrayElementAtIndex(i + 1);
                SerializedProperty nextControlRange = nextProperty.FindPropertyRelative("controlRange");
                SerializedProperty nextMinProp = nextControlRange.FindPropertyRelative("min");
                SerializedProperty nextMaxProp = nextControlRange.FindPropertyRelative("max");
                
                if (currMaxProp.floatValue < currMinProp.floatValue)
                {
                    currMaxProp.floatValue = currMinProp.floatValue;
                }
                
                if (nextMinProp.floatValue < currMaxProp.floatValue)
                {
                    nextMinProp.floatValue = currMaxProp.floatValue;
                }
                
                if (nextMaxProp.floatValue < nextMinProp.floatValue)
                {
                    nextMaxProp.floatValue = nextMinProp.floatValue;
                }
            }
        }
    }
}