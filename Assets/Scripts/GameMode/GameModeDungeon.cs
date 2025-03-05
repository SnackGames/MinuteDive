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

    Player player = Player.Get;
    TimeManager.GetTimeManager().StartTimer(player.playerStat.GetStatValue(StatType.InitialRemainTime));
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

    float finalDamage = damage - player.playerStat.GetStatValue(StatType.Defense);
    TimeManager.GetTimeManager().ReduceTime(finalDamage);
    AssetReferenceManager.GetAssetReferences().remainTime.OnTimeChanged(-finalDamage);

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