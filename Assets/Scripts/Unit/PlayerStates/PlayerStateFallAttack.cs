using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateFallAttack : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.FallAttack;

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
              if (player.isOnGround)
              {
                player.DequePressedInput();
                player.isReservedDashDirectionRight = pressedInput == ButtonInputType.Right;
                return PlayerStateType.Dash;
              }
            }
            break;

          case ButtonInputType.Attack:
            {
              if (player.isOnGround)
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
  }
}