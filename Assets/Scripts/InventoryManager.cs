using UnityEngine;
using UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using System.Collections.Generic;
using Data;
using System;
using Unity.VisualScripting;

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

  public GameObject droppedItemPrefab;
  [ReadOnly] public InventoryData inventoryData;
  [ReadOnly] public List<ItemData> itemDataList;
  [ReadOnly] public List<GameObject> droppedItemList;

  private int droppedItemUID = 0;

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

    droppedItemUID = 0;
  }

  private void RefreshItemCache()
  {
#if UNITY_EDITOR
    ValidateItemData();
#endif
  }

  private void ValidateItemData()
  {
#if UNITY_EDITOR
    string folderPath = "Assets/Data/Items";
    string[] assetGuids = AssetDatabase.FindAssets("t:ItemData", new[] { folderPath });
    itemDataList = new List<ItemData>();

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
        UnityEditor.EditorApplication.isPlaying = false;
        return;
      }
      itemDictionary.Add(itemData.itemID, itemData.itemName);
    }

    Debug.Log("ValidateItemData: ItemData Validation Success!");
#endif
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

  public ItemData GetItemData(int itemID)
  {
    foreach (ItemData itemData in itemDataList)
    {
      if (itemData.itemID == itemID)
      {
        return itemData;
      }
    }
    Debug.LogError("GetItemData: Failed to Get ItemData for ItemID: " + itemID);
    return null;
  }

  public GameObject CreateDropItem(int itemID, Vector2 spawnPosition, Vector2 dropTargetPosition)
  {
    ItemData dropItemData = GetItemData(itemID);
    if (dropItemData == null)
      return null;

    GameObject droppedItemObject = Instantiate(droppedItemPrefab);
    UI_Item itemUIScript = droppedItemObject.GetComponentInChildren<UI_Item>();
    if(itemUIScript == null)
    {
      Debug.LogError("CreateDropItem: Cannot Find UI_Item Script!");
      return null;
    }

    DroppedItem droppedItemScript = droppedItemObject.GetComponent<DroppedItem>();
    if(droppedItemScript == null)
    {
      Debug.LogError("CreateDropItem: Cannot Find DroppedItem Script!");
      return null;
    }

    ++droppedItemUID;
    droppedItemObject.name = $"DroppedItem: {dropItemData.itemName}_{droppedItemUID}";
    itemUIScript.SetItemData(dropItemData);
    droppedItemScript.droppedItemUID = droppedItemUID;
    droppedItemScript.spawnedPosition = spawnPosition;
    droppedItemScript.dropTargetPosition = dropTargetPosition;
    droppedItemList.Add(droppedItemObject);
    return droppedItemObject;
  }

  public void ClearDroppedItems()
  {
    foreach(GameObject droppedItem in droppedItemList)
    {
      Destroy(droppedItem);
    }
    droppedItemList.Clear();
  }

  public void ClearDroppedItem(int targetItemUID)
  {
    int targetIndex = droppedItemList.FindIndex(x => x.GetComponent<DroppedItem>()?.droppedItemUID == targetItemUID);
    if (targetIndex != -1)
    {
      Destroy(droppedItemList[targetIndex]);
      droppedItemList.RemoveAt(targetIndex);
    }
  }
}