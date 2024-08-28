using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMode;
using Unity.VisualScripting;

public class GameModeDungeon : GameModeBase
{
  [Header("GameModeDungeon")]
  [ReadOnly] public float RemainTime = 0f;

  #region GameModeBase
  override public GameModeType GetGameModeType() => GameModeType.Dungeon;
  public override void StartGameMode()
  {
    base.StartGameMode();
    StartTimer();
  }
  public override void FinishGameMode()
  {
    base.FinishGameMode();
  }

  override protected void Update()
  {
    base.Update();

    if (RemainTime <= 0f)
      return;

    RemainTime -= Time.deltaTime;
    if (RemainTime <= 0f)
    {
      RemainTime = 0f;
      OnRemainTimeExpired();
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
    if (RemainTime > 0f)
      return;

    if (ModeManager != null)
      ModeManager.OnRemainTimeExpired.Invoke();
  }
}