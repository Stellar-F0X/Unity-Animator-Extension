using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorExpansion.Editor
{
    [CustomEditor(typeof(StateEventBehaviour))]
    public class StateEventBehaviourEditor : UnityEditor.Editor
    {
        private float _previewNormalizedTime;
        private bool _isPreviewing;

        private AnimationClip _previewAnimationClip;
        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _previewMixer;


        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            var eventSender = (StateEventBehaviour)target;

            if (this.Validate(eventSender, out string errorMessage))
            {
                GUILayout.Space(10);

                if (_isPreviewing)
                {
                    if (GUILayout.Button("Stop Preview"))
                    {
                        AnimationEditorUtility.EnforceTPose();
                        _isPreviewing = false;
                        AnimationMode.StopAnimationMode();

                        if (_playableGraph.IsValid())
                        {
                            _playableGraph.Destroy();
                        }
                    }
                    else
                    {
                        this.PreviewAnimationClip(eventSender);
                    }
                }
                else if (GUILayout.Button("Preview"))
                {
                    _isPreviewing = true;
                    AnimationMode.StartAnimationMode();
                }

                GUILayout.Label($"Previewing at {_previewNormalizedTime:F2}s", EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error, true);
            }
        }


        private void PreviewAnimationClip(StateEventBehaviour behaviour)
        {
            AnimatorController ac = AnimationEditorUtility.GetValidAnimatorControllerOrNull(out string errorMessage);

            if (ac == null)
            {
                return;
            }

            ChildAnimatorState matchingState = ac.layers
                .Select(layer => AnimationEditorUtility.FindMatchingStateRecursion(layer.stateMachine, behaviour))
                .FirstOrDefault(s => s.state != null);

            if (matchingState.state != null)
            {
                switch (matchingState.state.motion)
                {
                    case BlendTree blendTree:
                        this.SampleBlendTreeAnimation(behaviour, behaviour.triggerTime);
                        break;
                    
                    case AnimationClip clip:
                        //triggerTime은 0 ~ 1 비율이므로 시간과 곱하여 특정 타이밍을 고를 수 있음.
                        _previewNormalizedTime = behaviour.triggerTime * clip.length;
                        AnimationMode.SampleAnimationClip(Selection.activeGameObject, clip, _previewNormalizedTime);
                        break;
                }
            }
        }


        private void SampleBlendTreeAnimation(StateEventBehaviour behaviour, float normalizedTime)
        {
            Animator animator = Selection.activeGameObject.GetComponent<Animator>();

            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }

            _playableGraph = PlayableGraph.Create("BlendTreePreviewGraph");
            _previewMixer = AnimationMixerPlayable.Create(_playableGraph, 1, true);

            var output = AnimationPlayableOutput.Create(_playableGraph, "Animation", animator);
            output.SetSourcePlayable(_previewMixer);

            AnimatorController animatorController = AnimationEditorUtility.GetValidAnimatorControllerOrNull(out string errorMessage);

            if (animatorController != null)
            {
                ChildAnimatorState matchingState = animatorController.layers
                    .Select(layer => AnimationEditorUtility.FindMatchingStateRecursion(layer.stateMachine, behaviour))
                    .FirstOrDefault(state => state.state != null);

                // If the matching state is not a BlendTree, bail out
                if (matchingState.state.motion is BlendTree blendTree)
                {
                    float maxThreshold = blendTree.children.Max(child => child.threshold);

                    AnimationClipPlayable[] clipPlayables = new AnimationClipPlayable[blendTree.children.Length];
                    float[] weights = new float[blendTree.children.Length];
                    float totalWeight = 0f;

                    // Scale target weight according to max threshold
                    float targetWeight = Mathf.Clamp(normalizedTime * maxThreshold, blendTree.minThreshold, maxThreshold);

                    for (int i = 0; i < blendTree.children.Length; i++)
                    {
                        ChildMotion child = blendTree.children[i];
                        float weight = CalculateWeightForChild(blendTree, child, targetWeight);
                        weights[i] = weight;
                        totalWeight += weight;

                        AnimationClip clip = AnimationEditorUtility.GetAnimationClipFromMotionOrNull(child.motion);
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


        private bool Validate(StateEventBehaviour behaviour, out string errorMessage)
        {
            AnimatorController ac = AnimationEditorUtility.GetValidAnimatorControllerOrNull(out errorMessage);

            if (ac != null)
            {
                ChildAnimatorState matchingState = ac.layers
                    .Select(layer => AnimationEditorUtility.FindMatchingStateRecursion(layer.stateMachine, behaviour))
                    .FirstOrDefault(state => state.state != null);

                _previewAnimationClip = AnimationEditorUtility.GetAnimationClipFromMotionOrNull(matchingState.state?.motion);

                if (_previewAnimationClip == null)
                {
                    errorMessage = "No valid AnimationClip found for the current state.";
                    return false;
                }
            }

            return true;
        }

        
        float CalculateWeightForChild(BlendTree blendTree, ChildMotion child, float targetWeight)
        {
            float weight = 0f;

            if (blendTree.blendType == BlendTreeType.Simple1D)
            {
                // Find the neighbors around the target weight
                ChildMotion? lowerNeighbor = null;
                ChildMotion? upperNeighbor = null;

                foreach (var motion in blendTree.children)
                {
                    if (motion.threshold <= targetWeight && (lowerNeighbor == null || motion.threshold > lowerNeighbor.Value.threshold))
                    {
                        lowerNeighbor = motion;
                    }

                    if (motion.threshold >= targetWeight && (upperNeighbor == null || motion.threshold < upperNeighbor.Value.threshold))
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
                Vector2 targetPos = new Vector2 {
                    x = AnimationEditorUtility.GetBlendParameterValue(blendTree, blendTree.blendParameter),
                    y = AnimationEditorUtility.GetBlendParameterValue(blendTree, blendTree.blendParameterY)
                };
                
                float distance = Vector2.Distance(targetPos, child.position);
                weight = Mathf.Clamp01(1.0f / (distance + 0.001f));
            }

            return weight;
        }
    }
}