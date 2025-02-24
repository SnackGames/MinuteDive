using Data;
using UnityEngine;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Item")]
  public class UI_Item : MonoBehaviour
  {
    public ItemData itemData;
    public int money;
    public bool isMoney { get; private set; }

    public void SetItemData(ItemData newItemData)
    {
      isMoney = false;
      itemData = newItemData;

      // #TODO_ITEM newItemData에 따라 아이템 이미지 적용
    }

    public void SetMoneyData(int newMoney)
    {
      isMoney = true;
      money = newMoney;
    }
  }
}
