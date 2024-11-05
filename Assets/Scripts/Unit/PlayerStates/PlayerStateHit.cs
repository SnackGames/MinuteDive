using Unit;
using UnityEngine;

namespace PlayerState
{
  public class PlayerStateHit : PlayerStateBase
  {
    public override PlayerStateType GetPlayerStateType() => PlayerStateType.Hit;

    private void RefreshPlayerState(Animator animator)
    {
      player.userStateChangeData.resetReserveHit();
      animator.SetBool("hit", false);
    }

    override protected void OnPlayerStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      base.OnPlayerStateEnter(animator, stateInfo, layerIndex);
      RefreshPlayerState(animator);

      Debug.Log("Enter Hit State!");
    }

    protected override PlayerStateType? ProcessStateChange(Animator animator)
    {
      //return PlayerStateType.Move;
      return null;
    }
  }
}
