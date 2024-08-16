using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateAttack : PlayerStateBase
  {
    private bool isMoveInputEnabled = false;
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Attack;

    private void RefreshPlayerState()
    {
      isMoveInputEnabled = false;
      isAttackInputEnabled = false;
    }

    override protected void OnPlayerStateEnter()
    {
      base.OnPlayerStateEnter();
      RefreshPlayerState();
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator)
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
                return PlayerStateType.Dash;
              }
            }
            break;

          case ButtonInputType.Attack:
            {
              if (isAttackInputEnabled && player.isOnGround)
              {
                player.DequePressedInput();
                RefreshPlayerState();
                animator.SetTrigger("nextAttack");
                return null;
              }
            } break;
        }
        break;
      }

      return null;
    }

    public override void AnimTrigger_EnableMoveInput()
    {
      isMoveInputEnabled = true;
    }

    public override void AnimTrigger_EnableAttackInput()
    {
      isAttackInputEnabled = true;
    }
  }
}