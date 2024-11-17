using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMode;
using Unity.VisualScripting;
using UnityEngine.Events;
using System;
using UnityEngine.UIElements;
using Unit;

[Serializable]
public class GameModeEvent : UnityEvent<GameModeType> { }

public class GameModeManager : MonoBehaviour
{
  [Header("GameModeMangaer")]
  [ReadOnly] public GameModeType CurrentMode = GameModeType.None;
  public GameModeBase GameMode = null;
  public GameModeEvent OnSetGameMode;

  [Header("Lobby")]
  public Vector3 UserInitialPosition;

  void Start()
  {
  }

  void Update()
  {
    if (GameMode != null)
    {
      CurrentMode = GameMode.GetGameModeType();
    }
  }

  public void OnRegionEnter(string RegionName)
  {
    if (RegionName == "Lobby")
    {
      SetGameMode(GameModeType.Lobby);
    }
    else if(RegionName == "Clear")
    {
      Player player = Player.Get;
      if(player != null)
      {
        player.userStateChangeData.reserveWin(true);
      }
    }
  }
  public void OnRegionExit(string RegionName)
  {
    if (RegionName == "Lobby")
    {
      SetGameMode(GameModeType.Dungeon);
    }
    else if (RegionName == "Clear")
    {
      Player player = Player.Get;
      if (player != null)
      {
        player.userStateChangeData.reserveMove(true);
      }
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
    {
      Destroy(GameMode);
    }

    switch (Type)
    {
      case GameModeType.Lobby:
        GameMode = gameObject.AddComponent<GameModeLobby>();
        break;
      case GameModeType.Dungeon:
        GameMode = gameObject.AddComponent<GameModeDungeon>();
        break;
      default:
        GameMode = null;
        break;
    }

    OnSetGameMode.Invoke(Type);
  }

  public void OnPlayerHit(float damage)
  {
    GameMode?.OnPlayerHit(damage);
  }
}
