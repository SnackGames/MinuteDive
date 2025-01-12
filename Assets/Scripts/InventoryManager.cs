using UnityEngine;
using UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System.Collections.Generic;
using Data;

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

public class InventoryManager : MonoBehaviour
{
  static private InventoryManager inventorySingleton;
  static public InventoryManager GetInventory() { return inventorySingleton; }

  [ReadOnly] public InventoryData inventoryData;

  private void Awake()
  {
    inventorySingleton = this;
  }

  private void Start()
  {
    RefreshItemCache();

    inventoryData = SaveLoadInventorySystem.LoadInventory();

    // 임시로 아이템 표시
    inventoryData.items = new int[16];
    inventoryData.items[0] = 1;
    inventoryData.items[2] = 1;

    AssetReferenceManager.GetAssetReferences().SetMoney(inventoryData.money);
    AssetReferenceManager.GetAssetReferences().SetItems(inventoryData.items);
  }

  private void RefreshItemCache()
  {
    ValidateItemData();
  }

  private void ValidateItemData()
  {
    string folderPath = "Assets/Data/Items";
    string[] assetGuids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });
    List<ItemData> itemDataList = new List<ItemData>();

    foreach (string guid in assetGuids)
    {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(path);
      if (itemData != null)
      {
        itemDataList.Add(itemData);
      }
    }

    Dictionary<int, string> itemDictionary = new Dictionary<int, string>();
    foreach (ItemData itemData in itemDataList)
    {
      // 중복 ID 발생
      if(itemDictionary.ContainsKey(itemData.itemID))
      {
        Debug.LogError($"ValidateItemData: ItemData Validation Failed! [{itemDictionary[itemData.itemID]}] and [{itemData.itemName}] has Duplicated ItemID {itemData.itemID}!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        return;
      }
      itemDictionary.Add(itemData.itemID, itemData.itemName);
    }

    Debug.Log("ValidateItemData: ItemData Validation Success!");
  }

  public void SaveInventory()
  {
    SaveLoadInventorySystem.SaveInventory(inventoryData);
  }

  public void AddMoney(int amount)
  {
    inventoryData.money += amount;
    AssetReferenceManager.GetAssetReferences().SetMoney(inventoryData.money);
  }
}