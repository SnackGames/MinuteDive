using UI;
using Unit;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_InventoryItem : MonoBehaviour
{
  public GameObject outlineObject;
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

    int clickedInventoryItemIndex = transform.GetSiblingIndex();

    // 이미 장착한 아이템을 선택한 경우 장착 해제
    HashSet<int> selectedInventoryItemIndex = InventoryManager.GetInventory().GetSelectedInventoryItemIndex();
    if(selectedInventoryItemIndex.Contains(clickedInventoryItemIndex))
    {
      selectedInventoryItemIndex.Remove(clickedInventoryItemIndex);
    }
    // 이미 장착한 아이템이 아닌 경우 장착할 목록에 추가. 현재는 한번에 한 아이템만 장착 가능.
    else
    {
      selectedInventoryItemIndex.Clear();
      selectedInventoryItemIndex.Add(clickedInventoryItemIndex);
    }
    InventoryManager.GetInventory().SetSelectedInventoryItemIndex(selectedInventoryItemIndex);

    // 기존 아이템들을 모두 장착해제시킨 후 선택한 아이템들 장착
    HashSet<int> itemsToEquip = new HashSet<int>();
    Transform parentTransform = transform.parent;
    if (parentTransform != null)
    {
      for (int i = 0; i < parentTransform.childCount; i++)
      {
        Transform sibling = parentTransform.GetChild(i);
        if (selectedInventoryItemIndex.Contains(i))
        {
          int siblingItemID = sibling.gameObject.GetComponentInChildren<UI_Item>().itemData.itemID;
          itemsToEquip.Add(siblingItemID);
        }
      }
    }
    InventoryManager.GetInventory().UnequipItems(Player.Get.equippedItems);
    InventoryManager.GetInventory().EquipItems(itemsToEquip);

    // 장착한 아이템 표시
    if (parentTransform != null)
    {
      for (int i = 0; i < parentTransform.childCount; i++)
      {
        Transform sibling = parentTransform.GetChild(i);
        sibling.gameObject.GetComponent<UI_InventoryItem>().SetSelected(selectedInventoryItemIndex.Contains(i));
      }
    }
  }

  private void OnDestroy()
  {
    button.onClick.RemoveListener(OnButtonClick);
  }

  public void SetSelected(bool selected)
  {
    if (outlineObject != null)
    {
      outlineObject.SetActive(selected);
    }
  }
}
