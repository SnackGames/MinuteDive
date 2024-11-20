using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateFallAttack : PlayerStateBase
  {
    private bool isMoveInputEnabled = false;
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.FallAttack;

    private void RefreshPlayerState(Animator animator)
    {
      isMoveInputEnabled = false;
      isAttackInputEnabled = false;
      animator.SetBool("fallAttack", false);
    }

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      RefreshPlayerState(animator);
    }

    override protected void OnPlayerSubStateMachineEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerSubStateMachineEnter(animator, stateInfo, layerIndex);
      player.velocity = Vector2.zero;
      player.receivedImpulseDuringFallAttack = false;
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      while (player.HasPressedInput())
      {
        ButtonInputType pressedInput = player.PeekPressedInput();
        switch (pressedInput)
        {
          case ButtonInputType.Left:
          case ButtonInputType.Right:
            {
              if (isMoveInputEnabled && player.isOnGround)
              {
                player.DequePressedInput();
                player.isReservedDashDirectionRight = pressedInput == ButtonInputType.Right;
                return PlayerStateType.Dash;
              }
            }
            break;

          case ButtonInputType.Attack:
            {
              if (isAttackInputEnabled && player.isOnGround)
              {
                player.DequePressedInput();
                return PlayerStateType.Attack;
              }
            }
            break;
        }
        break;
      }

      return null;
    }

    public override void AnimTrigger_EnableMoveInput(bool enable)
    {
      isMoveInputEnabled = enable;
    }

    public override void AnimTrigger_EnableAttackInput(bool enable)
    {
      isAttackInputEnabled = enable;
    }
  }
}