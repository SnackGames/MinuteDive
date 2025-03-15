using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Item")]
  public class UI_Item : MonoBehaviour
  {
    public ItemData itemData;
    public int money;
    public bool isMoney { get; private set; }
    public Sprite defaultSprite = null;

    public void SetItemData(ItemData newItemData)
    {
      isMoney = false;
      itemData = newItemData;

      // newItemData에 따라 아이템 이미지 적용
      Image image = gameObject.GetComponent<Image>();
      if (newItemData.itemSprite != null)
      {
        image.sprite = newItemData.itemSprite;
      }
      else
      {
        image.sprite = defaultSprite;
      }
    }

    public void SetMoneyData(int newMoney)
    {
      isMoney = true;
      money = newMoney;
    }
  }
}
