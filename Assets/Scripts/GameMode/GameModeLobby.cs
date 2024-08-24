using GameMode;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class GameModeLobby : GameModeBase
{
  #region GameModeBase
  public override GameModeType GetGameModeType() => GameModeType.Lobby;
  public override void StartGameMode()
  {
  }
  public override void FinishGameMode()
  {
  }

  // Update is called once per frame
  override public void Update()
  {
    base.Update();
  }
  #endregion
}