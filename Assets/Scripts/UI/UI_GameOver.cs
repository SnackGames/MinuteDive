using GameMode;
using System;
using TMPro;
using Unity.VisualScripting;
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

      foreach (Transform child in lootedItemPanel)
        Destroy(child.gameObject);

      foreach (Data.ItemData itemData in InventoryManager.GetInventory()?.lootedItemsThisRun)
        Instantiate(itemUIPrefab, lootedItemPanel)?.GetComponent<UI_InventoryItem>()?.SetItemData(itemData);
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
