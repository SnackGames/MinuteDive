using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateAttack : PlayerStateBase
  {
    private bool isMoveInputEnabled = false;
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Attack;

    private void RefreshPlayerState(Animator animator)
    {
      isMoveInputEnabled = false;
      isAttackInputEnabled = false;
      animator.SetBool("attack", false);

      player.velocity.x = player.attackMoveSpeed * (player.isLookingRight ? 1.0f : -1.0f);
    }

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      RefreshPlayerState(animator);
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
                RefreshPlayerState(animator);
                animator.SetTrigger("nextAttack");
                return null;
              }
            } break;
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