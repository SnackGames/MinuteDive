using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Data;
using System.Linq;
using System.Collections;
using Unit;

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

  public static void ResetInventory()
  {
    SaveInventory(new InventoryData());
    LoadInventory();
  }
}

public class InventoryManager : MonoBehaviour
{
  static private InventoryManager inventorySingleton;
  static public InventoryManager GetInventory() { return inventorySingleton; }

  public GameObject droppedItemPrefab;
  public GameObject droppedMoneyPrefab;
  [ReadOnly] public InventoryData inventoryData;
  [ReadOnly] public List<ItemData> itemDataList;
  [ReadOnly] public List<GameObject> droppedItemList;
  [ReadOnly] public int lootedMoneyThisRun = 0;
  [ReadOnly] public List<ItemData> lootedItemsThisRun;

  private int droppedItemUID = 0;
  private AsyncOperationHandle<IList<ItemData>> loadHandle;

  private void Awake()
  {
    inventorySingleton = this;
  }

  private void Start()
  {
    RefreshItemCache();

    inventoryData = SaveLoadInventorySystem.LoadInventory();

    // 임시로, 게임을 재실행할 때마다 아이템 초기화
    inventoryData.items = new int[16];
    AssetReferenceManager.GetAssetReferences().SetMoney(inventoryData.money);
    AssetReferenceManager.GetAssetReferences().SetItems(inventoryData.items);

    droppedItemUID = 0;
  }

  private void RefreshItemCache()
  {
    StartCoroutine(ValidateItemData());
  }

  private IEnumerator ValidateItemData()
  {
    // "Items" 라벨이 붙은 모든 ItemData 로드
    loadHandle = Addressables.LoadAssetsAsync<ItemData>("Items", null);
    yield return loadHandle;

    if (loadHandle.Status == AsyncOperationStatus.Succeeded)
    {
      itemDataList = loadHandle.Result.ToList();
      Dictionary<int, string> itemDictionary = new Dictionary<int, string>();
      foreach (ItemData itemData in itemDataList)
      {
        // 중복 ID 발생
        if (itemDictionary.ContainsKey(itemData.itemID))
        {
          Debug.LogError($"ValidateItemData: ItemData Validation Failed! [{itemDictionary[itemData.itemID]}] and [{itemData.itemName}] has Duplicated ItemID {itemData.itemID}!");
#if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
#else
          Application.Quit();
#endif
          yield break;
        }
        itemDictionary.Add(itemData.itemID, itemData.itemName);
      }
      Debug.Log($"ValidateItemData: ItemData Validation Success! itemDataList Size: {itemDataList.Count}");
    }
    else
    {
      Debug.LogError($"ValidateItemData: Failed to Load ItemData Assets from Addressables. {loadHandle.Status}");
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
      yield break;
    }
  }

  public void SaveInventory()
  {
    SaveLoadInventorySystem.SaveInventory(inventoryData);
  }

  public void ResetInventory()
  {
    SaveLoadInventorySystem.ResetInventory();
  }

  public void ResetThisRunData()
  {
    lootedMoneyThisRun = 0;
    lootedItemsThisRun = new List<ItemData>();
  }

  public void AddMoney(int amount)
  {
    lootedMoneyThisRun += amount;
    inventoryData.money += amount;
    AssetReferenceManager.GetAssetReferences().SetMoney(inventoryData.money);
  }

  public ItemData GetItemData(int itemID)
  {
    if (itemID == 0) return ScriptableObject.CreateInstance<ItemData>();

    foreach (ItemData itemData in itemDataList)
    {
      if (itemData.itemID == itemID)
      {
        return itemData;
      }
    }
    Debug.LogError("GetItemData: Failed to Get ItemData for ItemID: " + itemID);
    return ScriptableObject.CreateInstance<ItemData>();
  }

  public GameObject CreateDropItem(int itemID, Vector2 spawnPosition, Vector2 dropTargetPosition)
  {
    if (itemID == 0) return null;

    ItemData dropItemData = GetItemData(itemID);
    if (dropItemData == null)
    {
      Debug.LogError("CreateDropItem: Failed to get ItemData!");
      return null;
    }

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
    droppedItemObject.transform.position = spawnPosition;
    return droppedItemObject;
  }

  public GameObject CreateDropMoney(int money, Vector2 spawnPosition, Vector2 dropTargetPosition)
  {
    GameObject droppedMoneyObject = Instantiate(droppedMoneyPrefab);
    UI_Item itemUIScript = droppedMoneyObject.GetComponentInChildren<UI_Item>();
    if (itemUIScript == null)
    {
      Debug.LogError("CreateDropMoney: Cannot Find UI_Item Script!");
      return null;
    }

    DroppedItem droppedItemScript = droppedMoneyObject.GetComponent<DroppedItem>();
    if (droppedItemScript == null)
    {
      Debug.LogError("CreateDropMoney: Cannot Find DroppedItem Script!");
      return null;
    }

    itemUIScript.SetMoneyData(money);
    droppedItemScript.spawnedPosition = spawnPosition;
    droppedItemScript.dropTargetPosition = dropTargetPosition;
    droppedItemList.Add(droppedMoneyObject);
    droppedMoneyObject.transform.position = spawnPosition;
    return droppedMoneyObject;
  }

  static public int DrawDropItem(List<DropData> dropDataList)
  {
    int totalLotCount = 0;
    foreach (DropData dropData in dropDataList)
      totalLotCount += dropData.drawingLotCount;

    int drawnLot = Random.Range(0, totalLotCount);
    foreach (DropData dropData in dropDataList)
    {
      drawnLot -= dropData.drawingLotCount;
      if (drawnLot < 0) return dropData.itemID;
    }

    return 0;
  }

  public void PickupDropItem(DroppedItem pickupItem)
  {
    UI_Item pickupItemUIScript = pickupItem.gameObject.GetComponentInChildren<UI_Item>();
    if (pickupItemUIScript == null)
    {
      Debug.LogError($"PickupDropItem: failed to get Item UI Script from pickupItem!");
      ClearDroppedItem(pickupItem.droppedItemUID);
      return;
    }

    if (pickupItemUIScript.isMoney)
    {
      AddMoney(pickupItemUIScript.money);
    }
    else
    {
      for (int i = 0; i < inventoryData.items.Length; ++i)
      {
        if (inventoryData.items[i] == 0)
        {
          Debug.Log($"Pick up Item with ItemID {pickupItemUIScript.itemData.itemID}, ItemUID {pickupItem.droppedItemUID}!");
          inventoryData.items[i] = pickupItemUIScript.itemData.itemID;
          AssetReferenceManager.GetAssetReferences().SetItems(inventoryData.items);
          lootedItemsThisRun.Add(pickupItemUIScript.itemData);
          break;
        }
      }
    }

    if (pickupItem.lootSound != string.Empty)
    {
      Player.Get?.AnimTrigger_PlaySound(pickupItem.lootSound);
    }

    ClearDroppedItem(pickupItem.droppedItemUID);
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