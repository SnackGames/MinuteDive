using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMode;
using Unity.VisualScripting;
using UnityEngine.Events;
using System;

[Serializable]
public class GameModeEvent : UnityEvent<GameModeType> { }

public class GameModeManager : MonoBehaviour
{
  [Header("GameModeMangaer")]
  [ReadOnly] public GameModeType CurrentMode = GameModeType.None;
  public GameModeEvent OnSetGameMode;
  private GameModeBase GameMode = null;

  void Start()
  {
    SetGameMode(GameModeType.Lobby);
  }

  void Update()
  {
    if( GameMode != null )
      CurrentMode = GameMode.GetGameModeType();
  }

  public void OnRegionEnter(string RegionName)
  {
    if (RegionName == "Lobby")
    {
      SetGameMode(GameModeType.Lobby);
    }
  }
  public void OnRegionExit(string RegionName)
  {
    if (RegionName == "Lobby")
    {
      SetGameMode(GameModeType.Dungeon);
    }
  }

  public void SetGameMode(GameModeType Type)
  {
    GameModeType PrevType = GameModeType.None;
    if (GameMode != null)
      PrevType = GameMode.GetGameModeType();

    if (PrevType == Type)
      return;

    if (GameMode != null)
      GameMode.FinishGameMode();
    switch (Type)
    {
      case GameModeType.Lobby:
        GameMode = new GameModeLobby();
        GameMode.StartGameMode();
        break;
      case GameModeType.Dungeon:
        GameMode = new GameModeDungeon();
        GameMode.StartGameMode();
        break;
      default:
        GameMode = null;
        break;
    }

    OnSetGameMode.Invoke(Type);
  }
}
