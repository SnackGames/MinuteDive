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

      // 캐릭터가 바라보는 방향
      bool isRight = false;
      if (player.isLookingRight) isRight = player.velocity.x >= 0.0f;
      else isRight = player.velocity.x > 0.0f;
      player.SetLookingDirection(isRight);
    }

    virtual protected PlayerStateType? ProcessStateChange(Animator animator) => null;

    protected void TriggerStateChange(Animator animator, PlayerStateType state)
    {
      switch(state)
      {
        case PlayerStateType.Move: animator.SetTrigger("move"); break;
        case PlayerStateType.Attack: animator.SetTrigger("attack"); break;
        case PlayerStateType.FallAttack: animator.SetTrigger("fallAttack"); break;
        case PlayerStateType.Dash: animator.SetTrigger("dash"); break;
        default: Debug.LogError($"등록되지 않은 PlayerState: {state}"); break;
      }
    }

    public virtual void AnimTrigger_EnableMoveInput(bool enable) { }
    public virtual void AnimTrigger_EnableAttackInput(bool enable) { }
  }
}