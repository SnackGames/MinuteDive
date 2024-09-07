using UnityEngine;
using GameMode;
using UI;

public class GameModeDungeon : GameModeBase
{
  [Header("GameModeDungeon")]
  [ReadOnly] public float remainTime = 0f;
  private bool hasTimeExpired = false;

  #region GameModeBase
  override public GameModeType GetGameModeType() => GameModeType.Dungeon;
  public override void StartGameMode()
  {
    base.StartGameMode();
    StartTimer();
    hasTimeExpired = false;
  }
  public override void FinishGameMode()
  {
    base.FinishGameMode();
  }

  override public void OnPlayerHit(float damage)
  {
    remainTime -= damage;
    AssetReferenceManager.GetAssetReferences().remainTime.OnTimeChanged(damage);
  }

  override protected void Update()
  {
    base.Update();

    if (!hasTimeExpired)
    {
      remainTime -= Time.deltaTime;
      if (remainTime <= 0f)
      {
        hasTimeExpired = true;
        remainTime = 0f;
        OnRemainTimeExpired();
      }
    }
  }
  #endregion

  public float StartTimer()
  {
    if (ModeManager != null)
      remainTime = ModeManager.InitialRemainTime;

    return remainTime;
  }

  public float GetRemainTime() => remainTime;

  private void OnRemainTimeExpired()
  {
    ModeManager?.OnRemainTimeExpired.Invoke();
  }
}