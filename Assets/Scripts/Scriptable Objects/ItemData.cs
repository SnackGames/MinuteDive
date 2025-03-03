using UnityEngine;
using System;

[Serializable]
public struct DropData
{
  public int itemID;
  public int drawingLotCount;
}

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Item")]
  public class ItemData : ScriptableObject
  {
    public int itemID = -1;
    public string itemName = "Unknown";
    public string itemDescription = "Unknown";
    public Sprite itemSprite = null;
    // #TODO_ITEM 아이템의 능력치 표현용 데이터 추가
  }
}