using UnityEngine;

public class BlockDestroyAnimatorState : StateMachineBehaviour
{
    public bool DestroyEnded { get; private set; }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DestroyEnded = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DestroyEnded = true;
    }
}
