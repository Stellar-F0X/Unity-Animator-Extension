using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEditor.Animations;

namespace AnimatorExtension.AnimatorEditor
{
    internal class AnimationSamplePlayer
    {
        public AnimationSamplePlayer(Animator animator, AnimatorController controller)
        {
            this._animator = animator;
            this._controller = controller;
        }
        
        private Animator _animator;
        private AnimatorController _controller;
        private AnimationClip _previewAnimationClip;

        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _previewMixer;
        
        
        public void TryDestroyPlayableGraph()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
                _previewMixer.Destroy();
            }
        }
        
        
        public void PreviewAnimationClip(StateEventBehaviour behaviour, float normalizedTime)
        {
            if (AnimationUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
            {
                switch (matchingState.state.motion)
                {
                    case BlendTree blendTree:
                        this.SampleBlendTreeAnimation(behaviour, normalizedTime);
                        break;

                    case AnimationClip clip:
                        //triggerTime은 0 ~ 1 비율이므로 시간과 곱하여 특정 타이밍을 고를 수 있음.
                        AnimationMode.SampleAnimationClip(_animator.gameObject, clip, normalizedTime * clip.length);
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

            if (AnimationUtility.TryGetChildAnimatorState(_controller, behaviour, out var matchingState))
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
                        float weight = AnimationUtility.CalculateWeightForChild(blendTree, blendTree.children[i], targetWeight);

                        AnimationClip clip = AnimationUtility.GetAnimationClipFromMotionOrNull(blendTree.children[i].motion);

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
    }
}