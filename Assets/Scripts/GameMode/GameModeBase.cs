using System;
using System.Collections;
using System.Collections.Generic;
using Unit;
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

  [RequireComponent(typeof(GameModeManager))]
  public class GameModeBase : MonoBehaviour
  {
    protected GameModeManager ModeManager;

    virtual public GameModeType GetGameModeType() => GameModeType.None;
    virtual public void StartGameMode() { ModeManager = GetComponent<GameModeManager>(); }
    virtual public void FinishGameMode() { }

    virtual protected void Update() { }

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
