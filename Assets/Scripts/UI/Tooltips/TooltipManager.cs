using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class TooltipManager : MonoBehaviour
{
  static private TooltipManager tooltipManagerSingleton;
  static public TooltipManager Get { get => tooltipManagerSingleton; }

  public GameObject tooltipPanel;

  public void Awake()
  {
    tooltipManagerSingleton = this;

    HideTooltip();
  }

  public void ShowTooltip(int itemID, RectTransform rectTransform)
  {
    ItemData itemData = InventoryManager.GetInventory().GetItemData(itemID);
    if (itemData == null || itemData.itemID == 0)
      return;

    // 툴팁 피벗 및 위치 오프셋 설정
    bool isLeftSide = rectTransform.position.x < (Screen.width / 2f);
    tooltipPanel.GetComponent<RectTransform>().pivot = isLeftSide ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
    float offsetX = isLeftSide ? rectTransform.rect.size.x : -rectTransform.rect.size.x;
    Vector2 finalPos = rectTransform.position + new Vector3(offsetX, 0f, 0f);

    tooltipPanel.SetActive(true);
    tooltipPanel.GetComponent<UI_ItemTooltip>()?.setTooltipData(itemID);
    tooltipPanel.transform.position = finalPos;
  }
  public void HideTooltip()
  {
    tooltipPanel.SetActive(false);
  }
}
