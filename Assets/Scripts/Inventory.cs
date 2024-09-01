using UnityEngine;
using UI;
using System;

public class Inventory : MonoBehaviour
{
  static private Inventory inventorySingleton;
  static public Inventory GetInventory() { return inventorySingleton; }

  [ReadOnly] public int money = 0;
  public UI_MainInfo mainInfo;

  int[] items = new int[16];

  private void Awake()
  {
    inventorySingleton = this;
  }

  private void Start()
  {
    // 임시로 아이템 표시
    items[0] = 1;
    items[2] = 1;

    mainInfo.SetMoney(money);
    mainInfo.SetItems(items);
  }

  public void AddMoney(int amount)
  {
    money += amount;
    mainInfo.SetMoney(money);
  }
}