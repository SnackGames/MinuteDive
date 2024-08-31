using UnityEngine;
using UI;

public class Inventory : MonoBehaviour
{
  [ReadOnly] public int money = 0;
  public UI_MainInfo mainInfo;

  static private Inventory inventorySingleton;
  static public Inventory GetInventory() { return inventorySingleton; }

  private void Awake()
  {
    inventorySingleton = this;
  }

  private void Start()
  {
    mainInfo.SetMoney(money);
  }

  public void AddMoney(int amount)
  {
    money += amount;
    mainInfo.SetMoney(money);
  }
}