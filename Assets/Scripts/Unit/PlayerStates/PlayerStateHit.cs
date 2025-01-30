using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    public float stateDuration = 1f;

    private float stateEnterTime = 0f;
    private float endAnimationDuration = 0f;

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

    override protected void OnPlayerSubStateMachineEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerSubStateMachineEnter(animator, stateInfo, layerIndex);
      stateEnterTime = Time.time;
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      if (player.userStateChangeData.isMoveReserved())
      {
        return PlayerStateType.Move;
      }

      float elapsedTime = Time.time - stateEnterTime;
      if(elapsedTime > stateDuration - endAnimationDuration)
      {
        return PlayerStateType.Move;
      }

      return null;
    }
  }
}
