using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [DisallowMultipleComponent]
  public class AssetReferenceManager : MonoBehaviour
  {
    static private AssetReferenceManager assetReferenceSingleton;
    static public AssetReferenceManager GetAssetReferences() { return assetReferenceSingleton; }

    [Header("Asset References")]
    public AssetReferenceData assetReferences;

    [Header("UI Links")]
    public TextMeshProUGUI moneyText;
    public GameObject itemGrid;
    public UI_RemainTime remainTime;
    public UI_GameOver gameOver;

    private void Awake()
    {
      assetReferenceSingleton = this;
    }

    public void SetMoney(int money)
    {
      if (moneyText) moneyText.text = $"{money}";
    }

    public void SetItems(List<int> items)
    {
      foreach (Transform itemGridChlid in itemGrid.transform)
      {
        Destroy(itemGridChlid.gameObject);
      }

      int count = items.Count;
      for (int i = 0; i < count; ++i)
      {
        GameObject inventoryItemObject = Instantiate(InventoryManager.GetInventory().inventoryItemPrefab, itemGrid.transform);
        UI_InventoryItem inventoryItem = inventoryItemObject.GetComponent<UI_InventoryItem>();
        if (inventoryItem != null)
        {
          inventoryItem.isEquippable = true;
          inventoryItem.SetItemData(InventoryManager.GetInventory().GetItemData(items[i]));
          inventoryItem.SetSelected(InventoryManager.GetInventory().GetSelectedInventoryItemIndex().Contains(i));
        }
      }
    }
  }
}