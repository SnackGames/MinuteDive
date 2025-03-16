using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemTooltip : MonoBehaviour
{
  public TextMeshProUGUI title;
  public TextMeshProUGUI description;
  public Transform effects;
  public GameObject effectPrefab;

  public void setTooltipData(int itemID)
  {
    ItemData itemData = InventoryManager.GetInventory().GetItemData(itemID);
    if (itemData == null)
      return;

    title.SetText(itemData.itemName);
    description.SetText(itemData.itemDescription);

    foreach (Transform child in effects)
      Destroy(child.gameObject);

    foreach (StatModifier modifier in itemData.statModifiers)
      Instantiate(effectPrefab, effects)?.GetComponent<TextMeshProUGUI>()?.SetText(modifier.GetModifierString());
  }
}
