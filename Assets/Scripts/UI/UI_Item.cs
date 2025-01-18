using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Item")]
  public class UI_Item : MonoBehaviour
  {
    public ItemData itemData;

    public void SetItemData(ItemData newItemData)
    {
      itemData = newItemData;
    }
  }
}
