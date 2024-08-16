using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateMove : PlayerStateBase
  {
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.moveInput = 0.0f;
      // �̵�
      if (player.IsHoldingInput(ButtonInputType.Left)) player.moveInput = -1.0f;
      else if (player.IsHoldingInput(ButtonInputType.Right)) player.moveInput = 1.0f;

      base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    override protected Unit.PlayerState? ProcessStateChange()
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
                return Unit.PlayerState.Attack_1;
              }
            } break;
        }
        break;
      }

      return null;
    }
  }
}