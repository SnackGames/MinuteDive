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
          // �̵� Ű�� �����Ѵ�
          case ButtonInputType.Left:
          case ButtonInputType.Right:
            {
              player.DequePressedInput();
              continue;
            }

          // ����
          case ButtonInputType.Attack:
            {
              // �ӽ÷� �� ���� �������� �ߵ�
              if (player.isOnGround)
              {
                player.DequePressedInput();
                return PlayerStateType.Attack;
              }
            } break;
        }
        break;
      }

      return null;
    }
  }
}