using GameMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeLobby : GameModeBase
{
  #region GameModeBase
  public override GameModeType GetGameModeType() => GameModeType.Lobby;

  // Update is called once per frame
  override protected void Update()
  {
    base.Update();
  }
  #endregion
}