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
    Player player = Player.Get;

    if (!player.CanBeHit())
    {
      return;
    }

    TimeManager.GetTimeManager().ReduceTime(damage);
    AssetReferenceManager.GetAssetReferences().remainTime.OnTimeChanged(-damage);

    if (TimeManager.GetRemainTime() > 0)
    {
      player.userStateChangeData.reserveHit(true);
    }
  }

  override protected void Update()
  {
    base.Update();
  }
  #endregion
}