using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateDash : PlayerStateBase
  {
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Dash;

    override protected void OnPlayerStateEnter() 
    {
      isAttackInputEnabled = false;

      base.OnPlayerStateEnter();
      player.SetLookingDirection(player.isReservedDashDirectionRight);
      player.velocity.x = player.dashSpeed * (player.isReservedDashDirectionRight ? 1.0f : -1.0f);
      player.dashEffect.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.dashEffect.Stop();
      base.OnStateExit(animator, stateInfo, layerIndex);
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator)
    {
      while (player.HasPressedInput())
      {
        ButtonInputType pressedInput = player.PeekPressedInput();
        switch (pressedInput)
        {
          // �̵� Ű�� �����Ѵ�
          case ButtonInputType.Left:
          case ButtonInputType.Right:
            {
              player.DequePressedInput();
              continue;
            }

          case ButtonInputType.Attack:
            {
              // ���� ����
              if (!player.isOnGround)
              {
                player.DequePressedInput();
                return PlayerStateType.FallAttack;
              }
              // ����
              else if (isAttackInputEnabled)
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

    public override void AnimTrigger_EnableAttackInput(bool enable)
    {
      isAttackInputEnabled = enable;
    }
  }
}