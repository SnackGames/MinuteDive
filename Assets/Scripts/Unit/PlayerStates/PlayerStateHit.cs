using Unit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    private float stateEnterTime = 0f;
    private float stateDuration = 1f;
    private float startAnimationDuration = 0f;
    private float endAnimationDuration = 0f;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Hit;

    private void RefreshPlayerState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.userStateChangeData.resetReserveHit();
      animator.SetBool("hit", false);

      // Hit State 진입 시점은 Hit State를 나타내는 SubStateMachine 최초 진입 시에만 기록
      if(player.prevPlayerState != player.playerState)
      {
        stateEnterTime = Time.time;
        startAnimationDuration = endAnimationDuration = stateInfo.length;
      }
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
      float elapsedTime = Time.time - stateEnterTime;
      if(elapsedTime > stateDuration - endAnimationDuration)
      {
        return PlayerStateType.Move;
      }

      return null;
    }
  }
}
