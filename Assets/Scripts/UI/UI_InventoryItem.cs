using UI;
using Unit;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_InventoryItem : MonoBehaviour
{
  private Button button;

  public void SetItemData(Data.ItemData itemData)
  {
    GetComponentInChildren<UI_Item>()?.SetItemData(itemData);
  }

  private void Awake()
  {
    button = GetComponent<Button>();
    if(button != null)
    {
      button.onClick.AddListener(OnButtonClick);
    }
  }

  private void OnButtonClick()
  {
    int itemID = -1;
    if (GetComponentInChildren<UI_Item>() != null)
    {
      itemID = GetComponentInChildren<UI_Item>().itemData.itemID;
    }

    InventoryManager.GetInventory().EquipItems(new HashSet<int> { itemID });
  }

  private void OnDestroy()
  {
    button.onClick.RemoveListener(OnButtonClick);
  }
}
