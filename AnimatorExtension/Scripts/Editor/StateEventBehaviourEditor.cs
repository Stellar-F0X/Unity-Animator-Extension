using System;
using System.Collections.Generic;
using System.Linq;
using AnimatorExtension.Parameters;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace AnimatorExtension.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(StateEventBehaviour))]
    internal class StateEventBehaviourEditor : Editor
    {
        private float _previewNormalizedTime;
        private bool _isPreviewing;
        private int _currentFocusIndex;

        private Animator _animator;
        private AnimatorController _controller;
        private ReorderableList _animationEventList;
        private StateEventBehaviour _behaviour;
        private AnimationSamplePlayer _animationSamplePlayer;

        private EventInfoContainer _eventContainer = new EventInfoContainer();
        private AnimationEventDrawer _animationEventDrawer = new AnimationEventDrawer();

        private readonly Type _customEventParameterType = typeof(CustomAnimationEventParameter);



        private void GetAnimatorAndControllerOfReceiver(out AnimationEventController controller)
        {
            if (_animator is null)
            {
                if (AnimationUtility.GetAnimationController(out _controller) == false)
                {
                    controller = null;
                    Debug.LogError("Unable to find the animation controller. Please check if the animation is set up correctly.");
                    return;
                }

                controller = FindObjectsByType<AnimationEventController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .FirstOrDefault(eventReceiver => eventReceiver.animator.runtimeAnimatorController == _controller);

                if (controller is null)
                {
                    Debug.LogError("Unable to find AnimationEventReceiver. Please check if there are receivers linked to the controller.");
                    return;
                }

                _animator = controller.animator;
            }
            else
            {
                controller = _animator.GetComponent<AnimationEventController>();
            }
        }



        private void OnEnable()
        {
            SerializedProperty property = serializedObject?.FindProperty("animationEventList");

            if (_animator is null)
            {
                this.GetAnimatorAndControllerOfReceiver(out AnimationEventController receiver);
                ReflectionUtility.SetEventsForContainer(receiver, _eventContainer);
            }
            
            _animationEventList = new ReorderableList(serializedObject, property, true, true, false, false);

            _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");
            _animationEventList.elementHeightCallback = this.DrawAnimationEventHeight;
            _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;

            _animationSamplePlayer = new AnimationSamplePlayer(_animator, _controller);
            _behaviour = target as StateEventBehaviour;

            _animationEventDrawer.onFocusedPointSlider = i => _previewNormalizedTime = _behaviour.animationEventList[i].triggerTime;
            _animationEventDrawer.onFocusedRangeSlider = t => _previewNormalizedTime = t;
        }


        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (this.Validate(_behaviour))
                {
                    _animationSamplePlayer.TryDestroyPlayableGraph();
                    this.DrawEventStateBehaviourGUI(_behaviour);
                    this.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.HelpBox("Invalid AnimationClip found for the current state.", MessageType.Error, true);
                }
            }
        }


        public void OnDisable()
        {
            _isPreviewing = false;
            _previewNormalizedTime = 0;

            if (_animator is not null)
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
                AnimationEvent animationEvent = new AnimationEvent
                {
                    eventName = _eventContainer.eventNames[0],
                    triggerTime = _previewNormalizedTime,
                    eventHash = _eventContainer.eventNameHashes[0],
                    rangeTriggerTime = new MinMax(_previewNormalizedTime, _previewNormalizedTime)
                };

                behaviour.animationEventList.Add(animationEvent);
            }

            if (GUILayout.Button("Remove Event"))
            {
                int count = behaviour.animationEventList.Count;

                if (_currentFocusIndex >= 0 && _currentFocusIndex < count)
                {
                    behaviour.animationEventList.RemoveAt(_currentFocusIndex);
                }
            }

            if (_isPreviewing)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.y += EditorGUIUtility.singleLineHeight + 5;

                string previewMsg = $"{_previewNormalizedTime * 100f:F2}%";
                EditorGUI.ProgressBar(lastRect, _previewNormalizedTime, previewMsg);
            }

            GUILayout.Space(_isPreviewing ? 30 : 10);
            _animationEventList.DoLayoutList();
        }


        private void DrawAnimationEventGUI(Rect position, int index, bool isActive, bool isFocused)
        {
            if (index >= _behaviour.animationEventList.Count)
            {
                return;
            }

            if (isFocused)
            {
                _currentFocusIndex = index;
            }

            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);

            position.y += 5;
            int pickedEvent = _animationEventDrawer.DrawStringHashField(position, property, _eventContainer.eventNames, _eventContainer.eventNameHashes);
            position.y += EditorGUIUtility.singleLineHeight + 5;
            _animationEventDrawer.DrawDropdownSliderField(position, property, index);
            position.y += EditorGUIUtility.singleLineHeight + 5;


            if (pickedEvent >= 0 && pickedEvent < _eventContainer.count)
            {
                if (_eventContainer.paramTypes[pickedEvent] == EAnimationEventParameter.Customization)
                {
                    Type castingTargetType = _eventContainer.customParamTypes[pickedEvent];

                    if (_customEventParameterType.IsAssignableFrom(castingTargetType) == false)
                    {
                        EditorGUILayout.HelpBox($"[{index + 1}] \"{castingTargetType.Name}\" cannot be cast to CustomParameter.", MessageType.Error, true);
                        return;
                    }

                    AnimationEventParameter customParam = _behaviour.animationEventList[index].parameter;

                    if (customParam.customValue is null || customParam.customValue.GetType() != castingTargetType)
                    {
                        customParam.customValue = (CustomAnimationEventParameter)Activator.CreateInstance(castingTargetType);
                        _behaviour.animationEventList[index].parameter = customParam;
                    }
                }

                _animationEventDrawer.DrawParameterField(position, property, _eventContainer.paramTypes[pickedEvent]);
            }
        }


        private float DrawAnimationEventHeight(int index)
        {
            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty eventHashProp = property.FindPropertyRelative("eventHash");
            SerializedProperty parameter = property.FindPropertyRelative("parameter");

            Type castingTargetType = _eventContainer.FindTypeByHash(eventHashProp.intValue);

            if (castingTargetType is not null && _customEventParameterType.IsAssignableFrom(castingTargetType) == false)
            {
                return EditorGUIUtility.singleLineHeight * 3f;
            }

            if (parameter is not null)
            {
                SerializedProperty paramType = parameter.FindPropertyRelative("parameterType");

                if ((EAnimationEventParameter)paramType.enumValueIndex == EAnimationEventParameter.Customization)
                {
                    SerializedProperty customValueProp = parameter.FindPropertyRelative("customValue");

                    if (customValueProp.isExpanded)
                    {
                        return EditorGUIUtility.singleLineHeight * 3f + EditorGUI.GetPropertyHeight(customValueProp);
                    }
                }
            }

            return EditorGUIUtility.singleLineHeight * 4f;
        }
    }
}