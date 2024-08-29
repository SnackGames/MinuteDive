using UnityEngine;
using GameMode;

public class GameModeDungeon : GameModeBase
{
  [Header("GameModeDungeon")]
  [ReadOnly] public float RemainTime = 0f;
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
    RemainTime -= damage;
  }

  override protected void Update()
  {
    base.Update();

    if (!hasTimeExpired)
    {
      RemainTime -= Time.deltaTime;
      if (RemainTime <= 0f)
      {
        hasTimeExpired = true;
        RemainTime = 0f;
        OnRemainTimeExpired();
      }
    }
  }
  #endregion

  public float StartTimer()
  {
    if (ModeManager != null)
      RemainTime = ModeManager.InitialRemainTime;

    return RemainTime;
  }

  public float GetRemainTime() => RemainTime;

  private void OnRemainTimeExpired()
  {
    ModeManager?.OnRemainTimeExpired.Invoke();
  }
}