using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Hit;

    private void RefreshPlayerState()
    {
    }

    override protected void OnPlayerStateEnter()
    {
      base.OnPlayerStateEnter();
      RefreshPlayerState();

      Debug.Log("Enter Hit State!");
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator)
    {
      return PlayerStateType.Move;
    }
  }
}
