using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMode
{
  [Serializable]
  public enum GameModeType
  {
    Lobby,
    Dungeon
  }

  public abstract class GameModeBase : MonoBehaviour
  {
    public abstract GameModeType GetGameModeType();

    // Update is called once per frame
    protected virtual void Update()
    {

    }
  }
}
