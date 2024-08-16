using Unit;
using System;
using UnityEngine;

namespace PlayerState
{
  [RequireComponent(typeof(Player))]
  public class PlayerStateBase : StateMachineBehaviour
  {
    protected Player player;

    public virtual PlayerStateType GetPlayerStateType() => PlayerStateType.Move;

    sealed override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player = animator.GetComponent<Player>();
      player.playerState = GetPlayerStateType();
      player.playerStateBehaviour = this;

      OnPlayerStateEnter();
    }

    virtual protected void OnPlayerStateEnter() { }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      PlayerStateType? nextState = ProcessStateChange(animator);
      if(nextState != null)
      {
        TriggerStateChange(animator, nextState.Value);
      }

      animator.SetBool("isRunning", Math.Abs(player.velocity.x) > 0.0f);
      animator.SetBool("isFalling", !player.isOnGround);

      // ĳ���Ͱ� �ٶ󺸴� ����
      if (player.isLookingRight) player.isLookingRight = player.velocity.x >= 0.0f;
      else player.isLookingRight = player.velocity.x > 0.0f;
      player.SetLookingDirection(player.isLookingRight);
    }

    virtual protected PlayerStateType? ProcessStateChange(Animator animator) => null;

    protected void TriggerStateChange(Animator animator, PlayerStateType state)
    {
      switch(state)
      {
        case PlayerStateType.Move: animator.SetTrigger("move"); break;
        case PlayerStateType.Attack: animator.SetTrigger("attack"); break;
        case PlayerStateType.Dash: animator.SetTrigger("dash"); break;
        default: Debug.LogError($"��ϵ��� ���� PlayerState: {state}"); break;
      }
    }

    public virtual void AnimTrigger_EnableMoveInput() { }
    public virtual void AnimTrigger_EnableAttackInput() { }
  }
}