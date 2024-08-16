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

      // 공격이 끝났을 시에만 입력 처리
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
            // 임시 대시 처리
            case ButtonInputType.Left:
            case ButtonInputType.Right:
              {
                processedInput = true;
                pressedInputs.Dequeue();
                nextPlayerState = PlayerState.Dash;

                anim.SetTrigger("triggerDash");
              }
              break;

            // 공격 (다음 공격 진행)
            case ButtonInputType.Attack:
              {
                processedInput = true;

                // 임시로 땅 위에 있을때만 발동
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