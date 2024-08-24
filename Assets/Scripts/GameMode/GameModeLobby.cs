using GameMode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Unit;

public class GameModeLobby : GameModeBase
{
  #region GameModeBase
  public override GameModeType GetGameModeType() => GameModeType.Lobby;
  public override void StartGameMode()
  {
    base.StartGameMode();
  }
  public override void FinishGameMode()
  {
    base.FinishGameMode();
  }

  // Update is called once per frame
  override protected void Update()
  {
    base.Update();
  }
  #endregion
}