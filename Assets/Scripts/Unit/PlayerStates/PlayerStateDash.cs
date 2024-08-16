using Unit;

namespace PlayerState
{
  public class PlayerStateDash : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Dash;

    override protected void OnPlayerStateEnter() 
    {
      base.OnPlayerStateEnter();
      player.SetLookingDirection(player.isReservedDashDirectionRight);
      player.velocity.x = player.dashSpeed * (player.isReservedDashDirectionRight ? 1.0f : -1.0f);
    }
  }
}