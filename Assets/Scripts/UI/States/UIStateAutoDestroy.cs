using UnityEngine;

namespace UIState
{
  public class UIStateAutoDestroy : StateMachineBehaviour
  {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      // #TODO 어느 parent 까지를 선택할수있도록 개선
      Destroy(animator.transform.parent.gameObject, stateInfo.length);
    }
  }
}
