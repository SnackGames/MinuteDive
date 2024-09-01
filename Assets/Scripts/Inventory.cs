using UnityEngine;
using UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class InventoryData
{
  public int money;
  public int[] items;
}

public static class SaveLoadInventorySystem
{
  private static string inventorySavePath = Application.persistentDataPath + "/inventory.sav";

  public static void SaveInventory(InventoryData inventoryData)
  {
    BinaryFormatter formatter = new BinaryFormatter();

    FileStream stream = new FileStream(inventorySavePath, FileMode.OpenOrCreate);
    formatter.Serialize(stream, inventoryData);
    stream.Close();
  }

  public static InventoryData LoadInventory()
  {
    InventoryData invntoryData = null;

    if (File.Exists(inventorySavePath))
    {
      BinaryFormatter formatter = new BinaryFormatter();

      FileStream stream = new FileStream(inventorySavePath, FileMode.Open);
      invntoryData = formatter.Deserialize(stream) as InventoryData;
      stream.Close();
    }
    else
    {
      invntoryData= new InventoryData();
    }

    return invntoryData;
  }
}

public class Inventory : MonoBehaviour
{
  static private Inventory inventorySingleton;
  static public Inventory GetInventory() { return inventorySingleton; }

  [ReadOnly] public InventoryData inventoryData;
  public UI_MainInfo mainInfo;

  private void Awake()
  {
    inventorySingleton = this;
  }

  private void Start()
  {
    inventoryData = SaveLoadInventorySystem.LoadInventory();

    // 임시로 아이템 표시
    inventoryData.items = new int[16];
    inventoryData.items[0] = 1;
    inventoryData.items[2] = 1;

    mainInfo.SetMoney(inventoryData.money);
    mainInfo.SetItems(inventoryData.items);
  }

  public void SaveInventory()
  {
    SaveLoadInventorySystem.SaveInventory(inventoryData);
  }

  public void AddMoney(int amount)
  {
    inventoryData.money += amount;
    mainInfo.SetMoney(inventoryData.money);
  }
}