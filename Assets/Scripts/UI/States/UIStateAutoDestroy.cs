using UnityEngine;

namespace UIState
{
  public class UIStateAutoDestroy : StateMachineBehaviour
  {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      // #TODO ��� parent ������ �����Ҽ��ֵ��� ����
      Destroy(animator.transform.parent.gameObject, stateInfo.length);
    }
  }
}
