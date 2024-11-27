using GameMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
  public GameObject gameOverUI;

  public void ShowGameOver(bool show)
  {
    gameOverUI.SetActive(show);
  }

  public void OnSetGameMode(GameModeType Type)
  {
    switch (Type)
    {
      case GameModeType.Lobby:
        if (gameOverUI.activeSelf) ShowGameOver(false);
        break;
    }
  }
}
