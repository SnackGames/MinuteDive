using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateMove : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Move;
    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      player.userStateChangeData.resetReserveMove();
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      if (player.userStateChangeData.isHitReserved())
      {
        return PlayerStateType.Hit;
      }
      else if (player.userStateChangeData.isDyingReserved())
      {
        return PlayerStateType.Dying;
      }
      else if (player.userStateChangeData.isWinReserved())
      {
        return PlayerStateType.Win;
      }

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
              if (player.isOnGround)
              {
                player.DequePressedInput();
                return PlayerStateType.Attack;
              }
              else if (player.canFallAttack)
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