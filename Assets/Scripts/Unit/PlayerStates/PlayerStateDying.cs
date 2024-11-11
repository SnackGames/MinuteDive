using System.Collections;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateDying : PlayerStateBase
  {
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

    protected override PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      return null;
    }
  }
}
