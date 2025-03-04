using GameMode;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameOverEvent : UnityEvent {}

public class UI_GameOver : MonoBehaviour
{
  public GameObject gameOverUI;
  public TextMeshProUGUI lootedMoneyText;
  public Transform lootedItemPanel;

  public GameObject itemUIPrefab; 

  public GameOverEvent OnOpenGameOver;
  public GameOverEvent OnCloseGameOver;

  public void SetFloorText(int currentFloor, int maxFloor)
  {
    TextMeshProUGUI[] childrenText = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
    foreach (TextMeshProUGUI childText in childrenText)
    {
      if (childText.name == "FloorRecord")
      {
        childText.SetText($"도달 층: {currentFloor}층\n최고 도달: {maxFloor}층");
      }
    }
  }

  public void ShowGameOver(bool show)
  {
    if (show)
      OnOpenGameOver.Invoke();
    else
      OnCloseGameOver.Invoke();

    gameOverUI.SetActive(show);
    SetFloorText(FloorManager.GetCurrentFloor(), FloorManager.GetMaxFloor());

    if (show)
    {
      lootedMoneyText?.SetText(InventoryManager.GetInventory()?.lootedMoneyThisRun.ToString());

      // #TODO UI 코드 생성 테스트
      GameObject lootedItem = Instantiate(itemUIPrefab, lootedItemPanel);
      lootedItem = Instantiate(itemUIPrefab, lootedItemPanel);
      lootedItem = Instantiate(itemUIPrefab, lootedItemPanel);
    }
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
