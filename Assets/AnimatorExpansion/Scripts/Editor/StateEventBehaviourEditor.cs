using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AnimatorExpansion.Parameters;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using BlendTree = UnityEditor.Animations.BlendTree;

namespace AnimatorExpansion.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(StateEventBehaviour))]
    public class StateEventBehaviourEditor : Editor
    {
        private string _newEventName;
        private float _previewNormalizedTime;
        private bool _isPreviewing;
        private int _currentFocusIndexInList;

        private Animator _animator;
        private AnimatorController _controller;
        private ReorderableList _animationEventList;
        private AnimationSamplePlayer _animationSamplePlayer;
        private AnimationEventDrawer _animationEventDrawer;



        private void OnEnable()
        {
            _animationEventList = new ReorderableList(serializedObject, serializedObject.FindProperty("animationEventList"), true, true, false, false);

            _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");

            _animationEventList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 3;

            _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;

            AnimationEditorUtility.GetCurrentAnimatorAndController(out _controller, out _animator);

            _animationSamplePlayer = new AnimationSamplePlayer(_animator, _controller);

            _animationEventDrawer = new AnimationEventDrawer();
        }


        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                var eventSender = (StateEventBehaviour)target;

                if (_controller == null || _animator == null)
                {
                    EditorGUILayout.HelpBox("Please click the Animator GameObject.", MessageType.Error, true);
                    return;
                }

                if (this.Validate(eventSender, out var clip))
                {
                    _animationSamplePlayer.TryDestroyPlayableGraph();

                    this.DrawEventStateBehaviourGUI(eventSender, clip);

                    this.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.HelpBox("No valid AnimationClip found for the current state.", MessageType.Error, true);
                }
            }
        }


        private bool Validate(StateEventBehaviour behaviour, out AnimationClip previewClip)
        {
            if (AnimationEditorUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                previewClip = AnimationEditorUtility.GetAnimationClipFromMotionOrNull(matchingState.state?.motion);

                if (previewClip is null)
                {
                    return false;
                }
            }

            previewClip = null;
            return true;
        }


        private void DrawEventStateBehaviourGUI(StateEventBehaviour behaviour, AnimationClip previewClip)
        {
            GUILayout.Space(10);
            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope(GUI.skin.window))
            {
                if (_isPreviewing)
                {
                    if (GUILayout.Button("Stop Preview"))
                    {
                        _isPreviewing = false;
                        AnimationEditorUtility.EnforceTPose(_animator);
                        AnimationMode.StopAnimationMode();
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

                GUILayout.Label($"Previewing at {_previewNormalizedTime:F2}s", EditorStyles.helpBox);

                GUILayout.Space(10);

                _animationEventList.DoLayoutList();
            }
        }

        private float _verticalPosition = 0f;

        private void DrawAnimationEventGUI(Rect position, int index, bool isActive, bool isFocused)
        {
            StateEventBehaviour behaviour = (StateEventBehaviour)target;

            if (behaviour.animationEventList.Count <= index)
            {
                return;
            }

            if (isFocused)
            {
                _currentFocusIndexInList = index;
            }

            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);

            position.y += 5;
            _animationEventDrawer.DrawStringHashField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + 8;
            _animationEventDrawer.DrawDropdownSliderField(position, property);
        }


        private void AddNewAnimationEvent(List<AnimationEvent> eventList)
        {
            bool canUseHash = string.IsNullOrEmpty(_newEventName) || string.IsNullOrWhiteSpace(_newEventName);

            AnimationEvent animationEvent = new AnimationEvent()
            {
                eventName = _newEventName,
                eventHash = canUseHash ? 0 : Utility.StringToHash(_newEventName),
                triggerTime = _previewNormalizedTime,
                repeatTriggerRange = new MinMax(_previewNormalizedTime, _previewNormalizedTime)
            };

            eventList.Add(animationEvent);
        }


        private void RemoveAnimationEvent(List<AnimationEvent> eventList, int removeIndex)
        {
            if (removeIndex >= 0 && eventList.Count > removeIndex)
            {
                eventList.RemoveAt(removeIndex);
            }
        }
    }
}