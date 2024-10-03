using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateFallAttack : PlayerStateBase
  {
    private bool isMoveInputEnabled = false;
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.FallAttack;

    private void RefreshPlayerState()
    {
      isMoveInputEnabled = false;
      isAttackInputEnabled = false;
    }

    override protected void OnPlayerStateEnter()
    {
      base.OnPlayerStateEnter();
      RefreshPlayerState();

      // 낙하 공격 시전 시 x축 속도를 제거
      player.velocity = Vector2.zero;
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