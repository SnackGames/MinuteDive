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
    player.userStateChangeData.reserveHit(true);
  }

  override protected void Update()
  {
    base.Update();
  }
  #endregion
}