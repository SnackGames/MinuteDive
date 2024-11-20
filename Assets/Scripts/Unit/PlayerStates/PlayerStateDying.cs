using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateDying : PlayerStateBase
  {
    private float stateEnterTime = 0f;
    private float stateDuration = 3f;
    private float startAnimationDuration = 0f;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Dying;

    private void RefreshPlayerState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.userStateChangeData.resetReserveDying();
      animator.SetBool("dying", false);
    }

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      RefreshPlayerState(animator, stateInfo, layerIndex);
    }

    override protected void OnPlayerSubStateMachineEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerSubStateMachineEnter(animator, stateInfo, layerIndex);
      stateEnterTime = Time.time;
      startAnimationDuration = stateInfo.length;
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      if (player.userStateChangeData.isMoveReserved())
      {
        return PlayerStateType.Move;
      }

      float elapsedTime = Time.time - stateEnterTime;
      if (elapsedTime > stateDuration)
      {
        return PlayerStateType.Move;
      }

      return null;
    }
  }
}
