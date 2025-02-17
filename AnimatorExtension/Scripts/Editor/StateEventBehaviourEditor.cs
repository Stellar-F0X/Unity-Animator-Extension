using System;
using System.Linq;
using AnimatorExtension.Parameters;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using AnimatorController = UnityEditor.Animations.AnimatorController;

namespace AnimatorExtension.AnimatorEditor
{
    [CustomEditor(typeof(StateEventBehaviour))]
    internal class StateEventBehaviourEditor : Editor
    {
        private float _previewNormalizedTime;
        private bool _isPreviewing;
        private int _currentFocusIndex;

        private Animator _animator;
        private AnimatorController _controller;
        private ReorderableList _animationEventList;
        private StateEventBehaviour _stateEventBehaviour;
        private AnimationSamplePlayer _animationSamplePlayer;

        private AnimationEventContainer _animationEventContainer = new AnimationEventContainer();
        private AnimationEventDrawer _animationEventDrawer = new AnimationEventDrawer();

        private readonly Type _customEventParameterType = typeof(CustomAnimationEventParameter);

        
        
        
        private bool TryCreateEventList()
        {
            _stateEventBehaviour = target as StateEventBehaviour;

            SerializedProperty property = serializedObject.FindProperty("animationEventList");
            _animationEventList = new ReorderableList(serializedObject, property, true, true, false, false);

            _animationEventDrawer.onFocusedPointSlider = i => _previewNormalizedTime = _stateEventBehaviour.animationEventList[i].triggerTime;
            _animationEventDrawer.onFocusedRangeSlider = t => _previewNormalizedTime = t;

            _animationEventList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Animation Event List");
            _animationEventList.elementHeightCallback = this.DrawAnimationEventHeight;
            _animationEventList.drawElementCallback = this.DrawAnimationEventGUI;
            
            return _animationEventList is not null;
        }
        


        private bool TryInitializeAnimator()
        {
            _controller = AnimationUtility.GetAnimatorControllerOrNull();

            if (_animator is null || _animator.runtimeAnimatorController != _controller)
            {
                AnimationEventController eventController = null;
                
                if (Selection.activeGameObject is not null)
                {
                    eventController = Selection.activeGameObject.GetComponent<AnimationEventController>();
                }
                else
                {
                    eventController = FindObjectsByType<AnimationEventController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                        .FirstOrDefault(animEventController => animEventController.animator.runtimeAnimatorController == _controller);
                }

                if (eventController is null)
                {
                    _controller = null;
                    return false;
                }
                else
                {
                    _animator = eventController.animator;
                }
                
                AnimationUtility.SetAnimationEventsInContainer(eventController, _animationEventContainer);
            }

            _animationSamplePlayer = new AnimationSamplePlayer(_animator, _controller);
            return true;
        }



        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (this.Validate(_stateEventBehaviour, out string errorMessage))
                {
                    _animationSamplePlayer.TryDestroyPlayableGraph();
                    this.DrawEventStateBehaviourGUI(_stateEventBehaviour);
                    this.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Error, true);
                }
            }
        }



        public void OnDisable()
        {
            if (_controller is null)
            {
                return;
            }
            
            _isPreviewing = false;
            _previewNormalizedTime = 0;

            if (_animator is not null && _controller is not null)
            {
                _animationSamplePlayer?.TryDestroyPlayableGraph();

                AnimationUtility.EnforceTPose(_animator);
                AnimationMode.StopAnimationMode();
            }
        }



        private bool Validate(StateEventBehaviour behaviour, out string errorMessage)
        {
            if (_controller is null)
            {
                if (this.TryInitializeAnimator() == false)
                {
                    errorMessage = "Unable to find AnimatorController.";
                    return false;
                }
            }
            
            if (_animationEventList is null)
            {
                if (this.TryCreateEventList() == false)
                {
                    errorMessage = "animation event list is null.";
                    return false;
                }
            }

            if (AnimationUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                var previewClip = AnimationUtility.GetAnimationClipFromMotionOrNull(matchingState.state?.motion);

                if (previewClip is null)
                {
                    errorMessage = "Unable to find AnimationClip found for the current state.";
                    return false;
                }
            }

            errorMessage = string.Empty;
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
                    eventName = _animationEventContainer.eventNames[0],
                    triggerTime = _previewNormalizedTime,
                    eventHash = _animationEventContainer.eventNameHashes[0],
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
            if (index >= _stateEventBehaviour.animationEventList.Count)
            {
                return;
            }

            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);

            if (isFocused)
            {
                _currentFocusIndex = index;
            }

            position.y += 5;
            int pickedEvent = _animationEventDrawer.DrawStringHashField(position, property, _animationEventContainer.eventNames, _animationEventContainer.eventNameHashes);
            position.y += EditorGUIUtility.singleLineHeight + 5;
            _animationEventDrawer.DrawDropdownSliderField(position, property, index);
            position.y += EditorGUIUtility.singleLineHeight + 5;

            if (pickedEvent >= 0 && pickedEvent < _animationEventContainer.count)
            {
                if (_animationEventContainer.paramTypes[pickedEvent] == EAnimationEventParameter.Customization)
                {
                    Type castingTargetType = _animationEventContainer.customParamTypes[pickedEvent];

                    if (_customEventParameterType.IsAssignableFrom(castingTargetType) == false)
                    {
                        EditorGUILayout.HelpBox($"[{index + 1}] \"{castingTargetType.Name}\" cannot be cast to CustomParameter.", MessageType.Error, true);
                        return;
                    }

                    AnimationEventParameter customParam = _stateEventBehaviour.animationEventList[index].parameter;

                    if (customParam.customValue is null || customParam.customValue.GetType() != castingTargetType)
                    {
                        customParam.customValue = (CustomAnimationEventParameter)Activator.CreateInstance(castingTargetType);
                        _stateEventBehaviour.animationEventList[index].parameter = customParam;
                    }
                }

                _animationEventDrawer.DrawParameterField(position, property, _animationEventContainer.paramTypes[pickedEvent]);
            }
        }



        private float DrawAnimationEventHeight(int index)
        {
            SerializedProperty property = _animationEventList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty eventHashProp = property.FindPropertyRelative("eventHash");
            SerializedProperty parameter = property.FindPropertyRelative("parameter");

            Type castingTargetType = _animationEventContainer.FindTypeByHash(eventHashProp.intValue);

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