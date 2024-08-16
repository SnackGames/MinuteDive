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
      player.velocity.x = player.dashSpeed * (player.isLookingRight ? 1.0f : -1.0f);
    }
  }
}