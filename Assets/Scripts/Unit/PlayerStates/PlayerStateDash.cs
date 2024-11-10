using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateDash : PlayerStateBase
  {
    private bool isAttackInputEnabled = false;

    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Dash;

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
      isAttackInputEnabled = false;

      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      player.SetLookingDirection(player.isReservedDashDirectionRight);
      player.velocity.x = player.dashSpeed * (player.isReservedDashDirectionRight ? 1.0f : -1.0f);
      player.dashEffect.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      player.dashEffect.Stop();
      base.OnStateExit(animator, stateInfo, layerIndex);
    }

    override protected PlayerStateType? ProcessStateChange(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

          case ButtonInputType.Attack:
            {
              // 낙하 공격
              if (player.canFallAttack)
              {
                player.DequePressedInput();
                return PlayerStateType.FallAttack;
              }
              // 공격
              else if (isAttackInputEnabled)
              {
                player.DequePressedInput();
                return PlayerStateType.Attack;
              }
            } break;
        }
        break;
      }

      // TODO_HIT

      return null;
    }

    public override void AnimTrigger_EnableAttackInput(bool enable)
    {
      isAttackInputEnabled = enable;
    }
  }
}