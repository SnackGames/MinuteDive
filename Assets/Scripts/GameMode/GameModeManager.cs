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
  public GameModeBase GameMode = null;
  public GameModeEvent OnSetGameMode;

  [Header("Dungeon")]
  public float InitialRemainTime = 60f;
  public UnityEvent OnRemainTimeExpired;

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
}
