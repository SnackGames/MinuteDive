using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateAttack : PlayerStateBase
  {
    [ReadOnly] public bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Attack;

    override protected void OnPlayerStateEnter()
    {
      base.OnPlayerStateEnter();

      isAttackInputEnabled = false;
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

            } break;

          case ButtonInputType.Attack:
            {
              if (isAttackInputEnabled && player.isOnGround)
              {
                player.DequePressedInput();
                isAttackInputEnabled = false;
                animator.SetTrigger("nextAttack");
                return null;
              }
            } break;
        }
        break;
      }

      return null;

#if false
PlayerState nextPlayerState = state;

      // ������ ������ �ÿ��� �Է� ó��
      if (endAttackTriggered)
      {
        endAttackTriggered = false;
        nextPlayerState = PlayerState.Move;

        while (pressedInputs.Count > 0)
        {
          ButtonInputType pressedInput = pressedInputs.Peek().Item1;
          bool processedInput = false;

          switch (pressedInput)
          {
            // �ӽ� ��� ó��
            case ButtonInputType.Left:
            case ButtonInputType.Right:
              {
                processedInput = true;
                pressedInputs.Dequeue();
                nextPlayerState = PlayerState.Dash;

                anim.SetTrigger("triggerDash");
              }
              break;

            // ���� (���� ���� ����)
            case ButtonInputType.Attack:
              {
                processedInput = true;

                // �ӽ÷� �� ���� �������� �ߵ�
                if (isOnGround)
                {
                  pressedInputs.Dequeue();
                  nextPlayerState = GetPlayerStateByAttackIndex(GetNextAttackIndex(GetAttackIndexByPlayerState(state)));
                }
              }
              break;
          }

          if (processedInput) break;
        }
      }

      playerState = nextPlayerState;
#endif
    }

    public override void AnimTrigger_EnableAttackInput()
    {
      isAttackInputEnabled = true;
    }
  }
}