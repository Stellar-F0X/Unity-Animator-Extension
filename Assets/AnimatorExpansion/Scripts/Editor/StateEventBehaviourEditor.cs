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
        private AnimationClip _previewAnimationClip;

        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _previewMixer;
        private ReorderableList _animationEventList;


        
        private void OnEnable()
        {
            _animationEventList = new ReorderableList(serializedObject,  serializedObject.FindProperty("animationEventList"), true, true, false, false);
            
            _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");
            
            _animationEventList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 3;
            
            _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;
        }


        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                var eventSender = (StateEventBehaviour)target;

                if (_controller == null || _animator == null)
                {
                    AnimationEditorUtility.GetCurrentAnimatorAndController(out _controller, out _animator);

                    if (_animator == null)
                    {
                        EditorGUILayout.HelpBox("Please click the Animator GameObject.", MessageType.Error, true);
                        return;
                    }
                }

                if (this.Validate(eventSender))
                {
                    if (_playableGraph.IsValid())
                    {
                        _playableGraph.Destroy();
                    }

                    this.DrawEventStateBehaviourGUI(eventSender);

                    this.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.HelpBox("No valid AnimationClip found for the current state.", MessageType.Error, true);
                }
            }
        }


        private void PreviewAnimationClip(StateEventBehaviour behaviour)
        {
            if (AnimationEditorUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                switch (matchingState.state.motion)
                {
                    case BlendTree blendTree:
                        this.SampleBlendTreeAnimation(behaviour, _previewNormalizedTime);
                        break;

                    case AnimationClip clip:
                        //triggerTime은 0 ~ 1 비율이므로 시간과 곱하여 특정 타이밍을 고를 수 있음.
                        AnimationMode.SampleAnimationClip(_animator.gameObject, clip, _previewNormalizedTime * clip.length);
                        break;
                }
            }
        }


        private void SampleBlendTreeAnimation(StateEventBehaviour behaviour, float normalizedTime)
        {
            _playableGraph = PlayableGraph.Create("BlendTreePreviewGraph");
            _previewMixer = AnimationMixerPlayable.Create(_playableGraph, 1, true);

            var output = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
            output.SetSourcePlayable(_previewMixer);

            if (AnimationEditorUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                if (matchingState.state.motion is BlendTree blendTree)
                {
                    AnimationClipPlayable[] clipPlayables = new AnimationClipPlayable[blendTree.children.Length];

                    float maxThreshold = blendTree.children.Max(child => child.threshold);
                    float[] weights = new float[blendTree.children.Length];
                    float totalWeight = 0f;
                    
                    float targetWeight = Mathf.Clamp(normalizedTime * maxThreshold, blendTree.minThreshold, maxThreshold);

                    for (int i = 0; i < blendTree.children.Length; i++)
                    {
                        float weight = AnimationEditorUtility.CalculateWeightForChild(blendTree, blendTree.children[i], targetWeight);

                        AnimationClip clip = AnimationEditorUtility.GetAnimationClipFromMotionOrNull(blendTree.children[i].motion);

                        totalWeight += weight;
                        weights[i] = weight;

                        clipPlayables[i] = AnimationClipPlayable.Create(_playableGraph, clip);
                    }

                    // Normalize weights so they sum to 1
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] /= totalWeight;
                    }

                    _previewMixer.SetInputCount(clipPlayables.Length);

                    for (int i = 0; i < clipPlayables.Length; i++)
                    {
                        _previewMixer.ConnectInput(i, clipPlayables[i], 0);
                        _previewMixer.SetInputWeight(i, weights[i]);
                    }

                    AnimationMode.SamplePlayableGraph(_playableGraph, 0, normalizedTime);
                }
            }
        }


        private bool Validate(StateEventBehaviour behaviour)
        {
            if (AnimationEditorUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                _previewAnimationClip = AnimationEditorUtility.GetAnimationClipFromMotionOrNull(matchingState.state?.motion);

                if (_previewAnimationClip is null)
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
                        AnimationEditorUtility.EnforceTPose(_animator);
                        _isPreviewing = false;
                        AnimationMode.StopAnimationMode();
                    }
                    else
                    {
                        this.PreviewAnimationClip(behaviour);
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


        private void DrawAnimationEventGUI(Rect rect, int index, bool isActive, bool isFocused)
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
            
            SerializedProperty element = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);
            
            EditorGUI.PropertyField(rect, element, true);
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