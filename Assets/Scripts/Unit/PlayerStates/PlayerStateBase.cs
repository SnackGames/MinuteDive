using Unit;
using System;
using UnityEngine;

namespace PlayerState
{
  [RequireComponent(typeof(Player))]
  public class PlayerStateBase : StateMachineBehaviour
  {
    protected Player player;
    private bool isStateChangeTriggered = false;

    public virtual PlayerStateType GetPlayerStateType() => PlayerStateType.Move;

    sealed override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      isStateChangeTriggered = false;

      player = animator.GetComponent<Player>();
      player.prevPlayerState = player.playerState;
      player.playerState = GetPlayerStateType();
      player.playerStateBehaviour = this;
      player.isAttacking = false;
      player.isFallAttacking = false;

      OnPlayerStateEnter(animator, stateInfo, layerIndex);
    }

    virtual protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      if (player.prevPlayerState != player.playerState)
      {
        OnPlayerSubStateMachineEnter(animator, stateInfo, layerIndex);
      }
    }

    virtual protected void OnPlayerSubStateMachineEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      if(!isStateChangeTriggered)
      {
        PlayerStateType? nextState = ProcessStateChange(animator, stateInfo, layerIndex);
        if (nextState != null)
        {
          isStateChangeTriggered = true;
          TriggerStateChange(animator, nextState.Value);
        }
      }

      animator.SetBool("isRunning", Math.Abs(player.velocity.x) > 0.0f);
      animator.SetBool("isFalling", !player.isOnGround);

      // 캐릭터가 바라보는 방향. 이동 입력이 있을 때에만 변경함
      if (player.moveInput != 0)
      {
        bool isRight = false;
        if (player.isLookingRight) isRight = player.moveInput >= 0.0f;
        else isRight = player.moveInput > 0.0f;
        player.SetLookingDirection(isRight);
      }
    }

    virtual protected PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => null;

    protected void TriggerStateChange(Animator animator, PlayerStateType state)
    {
      switch(state)
      {
        case PlayerStateType.Move:
          animator.SetTrigger("move");
          break;
        case PlayerStateType.Attack:
          animator.SetTrigger("leaveMove");
          animator.SetBool("attack", true);
          break;
        case PlayerStateType.FallAttack:
          animator.SetTrigger("leaveMove");
          animator.SetBool("fallAttack", true);
          break;
        case PlayerStateType.Dash:
          animator.SetTrigger("dash");
          break;
        case PlayerStateType.Hit:
          animator.SetTrigger("leaveMove");
          animator.SetBool("hit", true);
          break;
        case PlayerStateType.Dying:
          animator.SetTrigger("leaveMove");
          animator.SetBool("dying", true);
          break;
        case PlayerStateType.Win:
          animator.SetTrigger("leaveMove");
          animator.SetBool("win", true);
          break;
        default: Debug.LogError($"등록되지 않은 PlayerState: {state}"); break;
      }
    }

    sealed override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      OnPlayerStateExit(animator, stateInfo, layerIndex);
    }

    virtual protected void OnPlayerStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    public virtual void AnimTrigger_EnableMoveInput(bool enable) { }
    public virtual void AnimTrigger_EnableAttackInput(bool enable) { }
  }
}