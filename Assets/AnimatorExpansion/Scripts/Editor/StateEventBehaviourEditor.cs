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
        private AnimationEventDrawer _animationEventDrawer;
        private AnimationSamplePlayer _animationSamplePlayer;

        private StateEventBehaviour _behaviour;



        private void OnEnable()
        {
            _animationEventList = new ReorderableList(serializedObject, serializedObject.FindProperty("animationEventList"), true, true, false, false);

            _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");
            _animationEventList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 3;
            _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;

            AnimationEditorUtility.GetCurrentAnimatorAndController(out _controller, out _animator);

            _animationSamplePlayer = new AnimationSamplePlayer(_animator, _controller);
            _animationEventDrawer = new AnimationEventDrawer();

            _behaviour = (StateEventBehaviour)target;

            Debug.Assert(_behaviour != null, "StateEventBehaviour load failed.");
            
            _animationEventDrawer.onFocusedPointSlider = i => _previewNormalizedTime = _behaviour.animationEventList[i].triggerTime;
            
            _animationEventDrawer.onFocusedRangeSlider = (min, max) =>
            {
                GUI.FocusControl(string.Empty);
                
                if (min.isChanged && max.isChanged)
                {
                    _previewNormalizedTime = (min.changedValue + max.changedValue) * 0.5f;
                }
                else if (min.isChanged)
                {
                    _previewNormalizedTime = min.changedValue;
                }
                else if (max.isChanged)
                {
                    _previewNormalizedTime = max.changedValue;
                }
            };
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

                if (this.Validate(_behaviour, out var clip))
                {
                    _animationSamplePlayer.TryDestroyPlayableGraph();

                    this.DrawEventStateBehaviourGUI(_behaviour, clip);

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
            
            _animationSamplePlayer.TryDestroyPlayableGraph();
            
            AnimationEditorUtility.EnforceTPose(_animator);
            AnimationMode.StopAnimationMode();
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
            _animationEventDrawer.DrawStringHashField(position, property);
            position.y += EditorGUIUtility.singleLineHeight + 8;
            _animationEventDrawer.DrawDropdownSliderField(position, property, index);
        }


        private void AddNewAnimationEvent(List<AnimationEvent> eventList)
        {
            bool canUseHash = string.IsNullOrEmpty(_newEventName) || string.IsNullOrWhiteSpace(_newEventName);

            AnimationEvent animationEvent = new AnimationEvent()
            {
                eventName = _newEventName,
                eventHash = canUseHash ? 0 : Utility.StringToHash(_newEventName),
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