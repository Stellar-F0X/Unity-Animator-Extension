using UnityEngine;

namespace AnimatorExtension
{
    public abstract class StateMachineBehaviourExtension : StateMachineBehaviour
    {
        public virtual void OnStateFixedUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}