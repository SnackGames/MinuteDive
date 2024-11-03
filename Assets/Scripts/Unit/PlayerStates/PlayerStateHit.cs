using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Hit;

    private void RefreshPlayerState()
    {
      player.userStateChangeData.resetReserveHit();
    }

    override protected void OnPlayerStateEnter()
    {
      base.OnPlayerStateEnter();
      RefreshPlayerState();

      Debug.Log("Enter Hit State!");
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator)
    {
      Debug.Log("Process State Change at Hit State!");
      //return PlayerStateType.Move;
      return null;
    }
  }
}
