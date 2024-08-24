using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMode
{
  [Serializable]
  public enum GameModeType
  {
    None = 0,
    Lobby,
    Dungeon
  }

  public abstract class GameModeBase
  {
    public abstract GameModeType GetGameModeType();
    public abstract void StartGameMode();
    public abstract void FinishGameMode();

    public virtual void Update()
    {

    }
  }
}
