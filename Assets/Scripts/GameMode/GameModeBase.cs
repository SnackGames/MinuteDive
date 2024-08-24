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

  public abstract class GameModeBase : MonoBehaviour
  {
    public abstract GameModeType GetGameModeType();
    public abstract void StartGameMode();
    public abstract void FinishGameMode();

    virtual protected void Update()
    {

    }

    void Start()
    {
      StartGameMode();
    }
    void OnDestroy()
    {
      FinishGameMode();
    }

  }
}
