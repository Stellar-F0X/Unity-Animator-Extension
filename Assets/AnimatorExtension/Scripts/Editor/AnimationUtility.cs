using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using AnimatorExpansion.Parameters;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Callbacks;

namespace AnimatorExpansion.Editor
{
    public static class AnimationUtility
    {
        private struct SEventInfo
        {
            public SEventInfo(string eventName, EParameterType paramType)
            {
                this.eventName = eventName;
                this.paramType = paramType;
            }

            public string eventName;
            public EParameterType paramType;
        }

        public const BindingFlags ANIMATOR_BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;

        private const BindingFlags _EVENT_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly static Type _animatorWindowType = Type.GetType("UnityEditor.Graphs.AnimatorControllerTool, UnityEditor.Graphs");
        
        private readonly static Type _SearchAttributeType = typeof(AnimationEventAttribute);

        private readonly static Dictionary<Type, Func<MonoBehaviour, IEnumerable<SEventInfo>>> _MethodCache = new ();

        private readonly static Lazy<List<SEventInfo>> _CachedEventName = new Lazy<List<SEventInfo>>(() => new List<SEventInfo>());

        private readonly static Lazy<List<SEventInfo>> _TempCachedEventNames = new Lazy<List<SEventInfo>>(() => new List<SEventInfo>());


        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            _MethodCache.Clear();
            _CachedEventName.Value.Clear();
            _TempCachedEventNames.Value.Clear();
        }


        public static void GetAnimationEventNames(AnimationEventReceiver receiver, out string[] nameList, out EParameterType[] paramTypeList)
        {
            if (receiver == null)
            {
                nameList = null;
                paramTypeList = null;
                return;
            }
            
            var components = receiver.GetComponentsInChildren<MonoBehaviour>(true);

            OnReloadScripts();

            if (components == null || components.Length == 0)
            {
                nameList = null;
                paramTypeList = null;
                return;
            }

            _CachedEventName.Value.Add(new SEventInfo("None", EParameterType.Void));

            foreach (var component in components)
            {
                Type type = component.GetType();

                if (_MethodCache.TryGetValue(type, out var getMethodNames) == false)
                {
                    getMethodNames = CreateMethodDelegate(type);
                    _MethodCache[type] = getMethodNames;
                }

                _CachedEventName.Value.AddRange(getMethodNames(component));
            }

            nameList = _CachedEventName.Value.Select(info => info.eventName).ToArray();
            paramTypeList = _CachedEventName.Value.Select(info => info.paramType).ToArray();
        }


        private static Func<MonoBehaviour, IEnumerable<SEventInfo>> CreateMethodDelegate(Type type)
        {
            foreach (var method in type.GetMethods(_EVENT_BINDING_FLAGS))
            {
                if (method.GetCustomAttribute(_SearchAttributeType) is AnimationEventAttribute attribute)
                {
                    _TempCachedEventNames.Value.Add(new SEventInfo(attribute.eventName, attribute.parameterType));
                }
            }

            return _ => _TempCachedEventNames.Value;
        }


        private static ChildAnimatorState FindMatchingStateRecursion(AnimatorStateMachine stateMachine, StateMachineBehaviour behaviour)
        {
            foreach (var state in stateMachine.states)
            {
                if (state.state.behaviours.Contains(behaviour))
                {
                    return state;
                }
            }

            foreach (var subStateMachine in stateMachine.stateMachines)
            {
                var matchingState = FindMatchingStateRecursion(subStateMachine.stateMachine, behaviour);

                if (matchingState.state != null)
                {
                    return matchingState;
                }
            }

            return default;
        }


        public static float CalculateWeightForChild(BlendTree blendTree, ChildMotion child, float targetWeight)
        {
            float weight = 0f;

            if (blendTree.blendType == BlendTreeType.Simple1D)
            {
                // Find the neighbors around the target weight
                ChildMotion? lowerNeighbor = null;
                ChildMotion? upperNeighbor = null;

                foreach (var motion in blendTree.children)
                {
                    if (motion.threshold <= targetWeight && (lowerNeighbor is null || motion.threshold > lowerNeighbor.Value.threshold))
                    {
                        lowerNeighbor = motion;
                    }

                    if (motion.threshold >= targetWeight && (upperNeighbor is null || motion.threshold < upperNeighbor.Value.threshold))
                    {
                        upperNeighbor = motion;
                    }
                }

                if (lowerNeighbor.HasValue && upperNeighbor.HasValue)
                {
                    if (Mathf.Approximately(child.threshold, lowerNeighbor.Value.threshold))
                    {
                        weight = 1.0f - Mathf.InverseLerp(lowerNeighbor.Value.threshold, upperNeighbor.Value.threshold, targetWeight);
                    }
                    else if (Mathf.Approximately(child.threshold, upperNeighbor.Value.threshold))
                    {
                        weight = Mathf.InverseLerp(lowerNeighbor.Value.threshold, upperNeighbor.Value.threshold, targetWeight);
                    }
                }
                else
                {
                    // Handle edge cases where there is no valid interpolation range
                    weight = Mathf.Approximately(targetWeight, child.threshold) ? 1f : 0f;
                }
            }
            else if (blendTree.blendType == BlendTreeType.FreeformCartesian2D || blendTree.blendType == BlendTreeType.FreeformDirectional2D)
            {
                Vector2 targetPos = new Vector2
                {
                    x = GetBlendParameterValue(blendTree, blendTree.blendParameter),
                    y = GetBlendParameterValue(blendTree, blendTree.blendParameterY)
                };

                float distance = Vector2.Distance(targetPos, child.position);
                weight = Mathf.Clamp01(1.0f / (distance + 0.001f));
            }

            return weight;
        }


        public static void EnforceTPose(Animator animator)
        {
            if (animator is null || animator.avatar is null)
            {
                return;
            }

            SkeletonBone[] skeletonBones = animator.avatar.humanDescription.skeleton;

            foreach (HumanBodyBones hbb in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (hbb == HumanBodyBones.LastBone)
                {
                    continue;
                }

                Transform boneTransform = animator.GetBoneTransform(hbb);
                if (!boneTransform)
                {
                    continue;
                }

                SkeletonBone skeletonBone = skeletonBones.FirstOrDefault(sb => sb.name == boneTransform.name);

                if (skeletonBone.name == null)
                {
                    continue;
                }

                if (hbb == HumanBodyBones.Hips)
                {
                    boneTransform.localPosition = skeletonBone.position;
                }

                boneTransform.localRotation = skeletonBone.rotation;
            }
        }


        public static float GetBlendParameterValue(BlendTree blendTree, string parameterName)
        {
            var methodInfo = typeof(BlendTree).GetMethod("GetInputBlendValue", ANIMATOR_BINDING_FLAGS);

            if (methodInfo == null)
            {
                Debug.LogError("Failed to find GetInputBlendValue method via reflection.");
                return 0f;
            }

            return (float)methodInfo.Invoke(blendTree, new object[] { parameterName });
        }


        public static AnimationClip GetAnimationClipFromMotionOrNull(Motion motion)
        {
            if (motion is AnimationClip clip)
            {
                return clip;
            }

            if (motion is BlendTree blendTree)
            {
                return blendTree.children
                    .Select(child => GetAnimationClipFromMotionOrNull(child.motion))
                    .FirstOrDefault(childClip => childClip is not null);
            }

            return null;
        }


        public static bool TryGetChildAnimatorState(AnimatorController animatorController, StateMachineBehaviour behaviour, out ChildAnimatorState matchingState)
        {
            matchingState = animatorController.layers
                .Select(layer => FindMatchingStateRecursion(layer.stateMachine, behaviour))
                .FirstOrDefault(state => state.state is not null);

            if (matchingState.state is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        
        public static void GetAnimatorAndController(out Animator animator, out AnimatorController controller)
        {
            EditorWindow window = EditorWindow.GetWindow(_animatorWindowType);

            FieldInfo animatorField = _animatorWindowType.GetField("m_PreviewAnimator", ANIMATOR_BINDING_FLAGS);
            FieldInfo controllerField = _animatorWindowType.GetField("m_AnimatorController", ANIMATOR_BINDING_FLAGS);

            animator = animatorField.GetValue(window) as Animator;
            controller = controllerField.GetValue(window) as AnimatorController;
        } 
    }
}