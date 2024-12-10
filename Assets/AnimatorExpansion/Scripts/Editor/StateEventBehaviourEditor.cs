using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AnimatorExpansion.Parameters;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.UIElements;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Object = System.Object;

namespace AnimatorExpansion.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(StateEventBehaviour))]
    internal class StateEventBehaviourEditor : Editor
    {
        private string _newEventName;
        private float _previewNormalizedTime;
        private bool _isPreviewing;
        private int _currentFocusIndexInList;

        private string[] _eventNameList;
        private EParameterType[] _parameterTypeList;

        private Animator _animator;
        private AnimatorController _controller;
        private ReorderableList _animationEventList;
        private StateEventBehaviour _behaviour;
        private AnimationEventDrawer _animationEventDrawer;
        private AnimationSamplePlayer _animationSamplePlayer;



        private void OnEnable()
        {
            SerializedProperty property = serializedObject?.FindProperty("animationEventList");

            AnimationUtility.GetCurrentAnimatorAndController(out _controller, out _animator);

            if (property != null && _controller != null && _animator != null)
            {
                _animationEventList = new ReorderableList(serializedObject, property, true, true, false, false);

                _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");
                _animationEventList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 4 + 5f;
                _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;

                _animationSamplePlayer = new AnimationSamplePlayer(_animator, _controller);
                _animationEventDrawer = new AnimationEventDrawer();

                _behaviour = (StateEventBehaviour)target;

                _animationEventDrawer.onFocusedPointSlider = i => _previewNormalizedTime = _behaviour.animationEventList[i].triggerTime;
                _animationEventDrawer.onFocusedRangeSlider = (min, max) => _previewNormalizedTime = _animationEventDrawer.OnFocusRangeSliderField(min, max);

                if (_animator.TryGetComponent(out AnimationEventReceiver receiver))
                {
                    AnimationUtility.GetAnimationEventNames(receiver, out _eventNameList);
                }
            }
        }


        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (_controller == null || _animator == null)
                {
                    EditorGUILayout.HelpBox("Please click the Animator GameObject.", MessageType.Error, true);
                    return;
                }

                if (this.Validate(_behaviour))
                {
                    _animationSamplePlayer.TryDestroyPlayableGraph();
                    this.DrawEventStateBehaviourGUI(_behaviour);
                    this.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.HelpBox("No valid AnimationClip found for the current state.", MessageType.Error, true);
                }
            }
        }


        public void OnDisable()
        {
            _isPreviewing = false;

            if (_animator != null)
            {
                _animationSamplePlayer?.TryDestroyPlayableGraph();

                AnimationUtility.EnforceTPose(_animator);
                AnimationMode.StopAnimationMode();
            }
        }


        private bool Validate(StateEventBehaviour behaviour)
        {
            if (_animationEventList == null)
            {
                return false;
            }

            if (AnimationUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                var previewClip = AnimationUtility.GetAnimationClipFromMotionOrNull(matchingState.state?.motion);

                if (previewClip is null)
                {
                    return false;
                }
            }

            return true;
        }


        private void DrawEventStateBehaviourGUI(StateEventBehaviour behaviour)
        {
            GUILayout.Space(10);
            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope(GUI.skin.window))
            {
                if (_isPreviewing)
                {
                    if (GUILayout.Button("Stop Preview"))
                    {
                        this.OnDisable();
                    }
                    else
                    {
                        _animationSamplePlayer.PreviewAnimationClip(behaviour, _previewNormalizedTime);
                    }
                }
                else if (GUILayout.Button("Preview"))
                {
                    _isPreviewing = true;
                    AnimationMode.StartAnimationMode();
                }

                if (GUILayout.Button("Add Event"))
                {
                    this.AddNewAnimationEvent(behaviour.animationEventList);
                }

                if (GUILayout.Button("Remove Event"))
                {
                    this.RemoveAnimationEvent(behaviour.animationEventList, _currentFocusIndexInList);
                }

                if (_isPreviewing)
                {
                    GUILayout.Label($"Previewing at {_previewNormalizedTime:F2}s", EditorStyles.helpBox);
                }

                GUILayout.Space(10);

                _animationEventList.DoLayoutList();
            }
        }


        private void DrawAnimationEventGUI(Rect position, int index, bool isActive, bool isFocused)
        {
            if (_behaviour.animationEventList.Count <= index)
            {
                return;
            }

            if (isFocused)
            {
                _currentFocusIndexInList = index;
            }

            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);

            position.y += 5;
            _animationEventDrawer.DrawStringHashField(position, property, _eventNameList);
            position.y += EditorGUIUtility.singleLineHeight + 8;
            _animationEventDrawer.DrawDropdownSliderField(position, property, index);
            position.y += EditorGUIUtility.singleLineHeight + 8;
            _animationEventDrawer.DrawParameterField(position, property);
        }


        private void AddNewAnimationEvent(List<AnimationEvent> eventList)
        {
            AnimationEvent animationEvent = new AnimationEvent()
            {
                eventName = _newEventName,
                eventHash = Utility.StringToHash(_newEventName),
                triggerTime = _previewNormalizedTime,
                rangeTriggerTime = new MinMax(_previewNormalizedTime, _previewNormalizedTime)
            };

            eventList.Add(animationEvent);
        }


        private void RemoveAnimationEvent(List<AnimationEvent> eventList, int removeIndex)
        {
            if (removeIndex < 0 || eventList.Count <= removeIndex)
            {
                return;
            }

            eventList.RemoveAt(removeIndex);
        }
    }
}