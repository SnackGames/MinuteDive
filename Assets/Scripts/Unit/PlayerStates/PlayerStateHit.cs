using Unit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Hit;

    private void RefreshPlayerState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.userStateChangeData.resetReserveHit();
      animator.SetBool("hit", false);
    }

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      RefreshPlayerState(animator, stateInfo, layerIndex);

      // x축 속도 제거
      player.velocity = Vector2.zero;
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      return null;
    }
  }
}
