using UnityEngine;

namespace UIState
{
  public class UIStateAutoDestroy : StateMachineBehaviour
  {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Destroy(animator.gameObject, stateInfo.length);
    }
  }
}
