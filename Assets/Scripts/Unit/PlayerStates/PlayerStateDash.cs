using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateDash : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Dash;

    override protected void OnPlayerStateEnter() 
    {
      base.OnPlayerStateEnter();
      player.SetLookingDirection(player.isReservedDashDirectionRight);
      player.velocity.x = player.dashSpeed * (player.isReservedDashDirectionRight ? 1.0f : -1.0f);
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator)
    {
      while (player.HasPressedInput())
      {
        ButtonInputType pressedInput = player.PeekPressedInput();
        switch (pressedInput)
        {
          // 이동 키는 무시한다
          case ButtonInputType.Left:
          case ButtonInputType.Right:
            {
              player.DequePressedInput();
              continue;
            }

          // 낙하 공격
          case ButtonInputType.Attack:
            {
              if (!player.isOnGround)
              {
                player.DequePressedInput();
                return PlayerStateType.FallAttack;
              }
            } break;
        }
        break;
      }

      return null;
    }
  }
}