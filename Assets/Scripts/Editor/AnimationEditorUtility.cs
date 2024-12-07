using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    public static class AnimationEditorUtility
    {
        public static AnimatorController GetValidAnimatorControllerOrNull(out string errorMessage)
        {
            errorMessage = string.Empty;
            GameObject targetGameObject = Selection.activeGameObject;

            if (ReferenceEquals(targetGameObject, null))
            {
                errorMessage = "Please select a GameObject with an Animator to preview.";
                return null;
            }

            if (targetGameObject.TryGetComponent(out Animator animator) == false)
            {
                errorMessage = "The selected GameObject does not have an Animator component.";
                return null;
            }

            if (animator.runtimeAnimatorController is not AnimatorController animatorController)
            {
                errorMessage = "The selected Animator does not have a valid AnimatorController.";
                return null;
            }

            return animatorController;
        }


        public static ChildAnimatorState FindMatchingStateRecursion(AnimatorStateMachine stateMachine, StateEventBehaviour behaviour)
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
                var matchingState = AnimationEditorUtility.FindMatchingStateRecursion(subStateMachine.stateMachine, behaviour);

                if (matchingState.state != null)
                {
                    return matchingState;
                }
            }

            return default;
        }


        public static float CalculateWeightForChild(BlendTree blendTree, ChildMotion child, float targetWeight)
        {
            return 0;
        }

        
        public static void EnforceTPose()
        {
            GameObject selected = Selection.activeGameObject;

            if (!selected || !selected.TryGetComponent(out Animator animator) || !animator.avatar)
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
            var methodInfo = typeof(BlendTree).GetMethod("GetInputBlendValue", BindingFlags.NonPublic | BindingFlags.Instance);
            
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
                    .FirstOrDefault(childClip => childClip != null);
            }

            return null;
        }
    }
}