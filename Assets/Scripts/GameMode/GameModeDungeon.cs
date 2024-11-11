using UnityEngine;
using GameMode;
using UI;
using Unit;

public class GameModeDungeon : GameModeBase
{
  #region GameModeBase
  override public GameModeType GetGameModeType() => GameModeType.Dungeon;
  public override void StartGameMode()
  {
    base.StartGameMode();
    TimeManager.GetTimeManager().StartTimer(TimeManager.GetTimeManager().initialRemainTime);
  }
  public override void FinishGameMode()
  {
    base.FinishGameMode();
  }

  override public void OnPlayerHit(float damage)
  {
    TimeManager.GetTimeManager().ReduceTime(damage);
    AssetReferenceManager.GetAssetReferences().remainTime.OnTimeChanged(-damage);
    Player player = Player.Get;

    if (TimeManager.GetRemainTime() > 0)
    {
      player.userStateChangeData.reserveHit(true);
    }
    else
    {
      player.userStateChangeData.reserveDying(true);
    }
  }

  override protected void Update()
  {
    base.Update();
  }
  #endregion
}