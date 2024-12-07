using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace AnimatorExpansion.Editor
{
    [CustomEditor(typeof(StateEventBehaviour))]
    public class StateEventBehaviourEditor : UnityEditor.Editor
    {
        private float _previewNormalizedTime;
        private bool _isPreviewing;

        private Animator _animator;
        private AnimatorController _controller;
        private AnimationClip _previewAnimationClip;

        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _previewMixer;


        public override VisualElement CreatePreview(VisualElement inspectorPreviewWindow)
        {
            return base.CreatePreview(inspectorPreviewWindow);
        }


        public override void OnInspectorGUI()
        {
            using (new GUIDisableScope(Application.isPlaying))
            {
                base.DrawDefaultInspector();

                var eventSender = (StateEventBehaviour)target;

                if (_controller is null || _animator is null)
                {
                    AnimationEditorUtility.GetCurrentAnimatorAndController(out _controller, out _animator);

                    if (_animator is null)
                    {
                        EditorGUILayout.HelpBox("Please click the Animator GameObject.", MessageType.Error, true);
                        return;
                    }
                }

                if (this.Validate(eventSender))
                {
                    GUILayout.Space(10);

                    if (_playableGraph.IsValid())
                    {
                        _playableGraph.Destroy();
                    }

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
                        this.SampleBlendTreeAnimation(behaviour, behaviour.triggerTime);
                        break;

                    case AnimationClip clip:
                        //triggerTime은 0 ~ 1 비율이므로 시간과 곱하여 특정 타이밍을 고를 수 있음.
                        _previewNormalizedTime = behaviour.triggerTime * clip.length;
                        AnimationMode.SampleAnimationClip(_animator.gameObject, clip, _previewNormalizedTime);
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

                    // Scale target weight according to max threshold
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
    }
}