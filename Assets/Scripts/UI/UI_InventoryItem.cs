using UI;
using UnityEngine;

public class UI_InventoryItem : MonoBehaviour
{
  public void SetItemData(Data.ItemData itemData)
  {
    GetComponentInChildren<UI_Item>()?.SetItemData(itemData);
  }
}
