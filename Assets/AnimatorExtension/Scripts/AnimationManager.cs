using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorExtension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimationManager : MonoBehaviour
    {
        private bool _synchronizeState;
        
        private Animator _animator;
        
        private AnimationEventReceiver _eventReceiver;

        private AnimatorStateInfo[] _currentAnimeStateInfoList;
        
        private readonly Dictionary<int, Queue<int>> _skipFrames = new Dictionary<int, Queue<int>>(); 

        private readonly Dictionary<int, StateMachineBehaviourExtension> _stateMachines = new();



        public bool synchronizeState
        {
            get { return _synchronizeState; }
            
            set { _synchronizeState = value; }
        }
        
        

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _eventReceiver = GetComponent<AnimationEventReceiver>();

            _currentAnimeStateInfoList = new AnimatorStateInfo[_animator.layerCount];
        }



        public void SetSkipFrames(int layerIndex, params int[] skipFrames)
        {
            
        }
        


        private void FixedUpdate()
        {
            for (int i = 0; i < _animator.layerCount; i++)
            {
                _currentAnimeStateInfoList[i] = _animator.GetCurrentAnimatorStateInfo(i);

                int hash = _currentAnimeStateInfoList[i].fullPathHash;

                if (_stateMachines.ContainsKey(hash))
                {
                    _stateMachines[hash].OnStateFixedUpdate(_animator, _currentAnimeStateInfoList[i], i);
                }
                else
                {
                    var behaviour = _animator.GetBehaviour<StateMachineBehaviourExtension>();

                    if (behaviour is not null)
                    {
                        _stateMachines.Add(hash, behaviour);
                    }
                }
            }
        }
    }
}