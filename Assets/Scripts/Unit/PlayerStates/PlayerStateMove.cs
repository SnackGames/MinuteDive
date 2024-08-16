using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateMove : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Move;

    override protected PlayerStateType? ProcessStateChange(Animator animator)
    {
      while (player.HasPressedInput())
      {
        ButtonInputType pressedInput = player.PeekPressedInput();
        switch(pressedInput)
        {
          // 이동 키는 무시한다
          case ButtonInputType.Left:
          case ButtonInputType.Right:
            {
              player.DequePressedInput();
              continue;
            }

          // 공격
          case ButtonInputType.Attack:
            {
              player.DequePressedInput();
              return player.isOnGround ? PlayerStateType.Attack : PlayerStateType.FallAttack;
            }
        }
        break;
      }

      return null;
    }
  }
}