using Unit;
using System;
using UnityEngine;
using UnityEngine.U2D;

namespace PlayerState
{
  [RequireComponent(typeof(Player))]
  public class PlayerStateBase : StateMachineBehaviour
  {
    protected Player player;

    sealed override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player = animator.GetComponent<Player>();

      OnStateEnter();
    }

    virtual protected void OnStateEnter() {}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Unit.PlayerState? nextState = ProcessStateChange();
      if(nextState != null)
      {
        TriggerStateChange(animator, nextState.Value);
      }

      animator.SetBool("isRunning", (Math.Abs(player.velocity.x) > 0.0f) && (Math.Abs(player.moveInput) > 0.0f));
      animator.SetBool("isFalling", !player.isOnGround);

      // ĳ���Ͱ� �ٶ󺸴� ����
      if (player.isLookingRight) player.isLookingRight = player.velocity.x >= 0.0f;
      else player.isLookingRight = player.velocity.x > 0.0f;
      player.SetLookingDirection(player.isLookingRight);
    }

    virtual protected Unit.PlayerState? ProcessStateChange() => null;

    protected void TriggerStateChange(Animator animator, Unit.PlayerState state)
    {
      player.playerState = state;

      switch(state)
      {
        case Unit.PlayerState.Move: animator.SetTrigger("changeToMove"); break;
        default: Debug.LogError($"��ϵ��� ���� PlayerState: {state}"); break;
      }
    }
  }
}