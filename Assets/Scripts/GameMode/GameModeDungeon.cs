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
  public float InitialRemainTime = 60f;

  #region GameModeBase
  override public GameModeType GetGameModeType() => GameModeType.Dungeon;
  public override void StartGameMode()
  {
    StartTimer();
  }
  public override void FinishGameMode()
  {
  }

  // Update is called once per frame
  override protected void Update()
  {
    base.Update();

    if (RemainTime <= 0f)
      return;

    float ElapsedTime = Time.deltaTime;
    RemainTime -= ElapsedTime;
    if (RemainTime <= 0f)
      OnRemainTimeExpired();
  }
  #endregion

  public float StartTimer()
  {
    RemainTime = InitialRemainTime;
    return RemainTime;
  }

  public float GetRemainTime() => RemainTime;

  private void OnRemainTimeExpired()
  {
    if (RemainTime > 0f)
      return;

    Debug.Log("OnRemainTimeExpired Called");
  }
}