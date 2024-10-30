using UnityEngine;
using GameMode;
using UI;

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
    // TODO_HIT 피격시 플레이어 쪽에 해야 할 작업 추가
  }

  override protected void Update()
  {
    base.Update();
  }
  #endregion
}