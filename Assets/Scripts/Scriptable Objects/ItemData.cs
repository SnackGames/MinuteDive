using UnityEngine;
using System;
using System.Collections.Generic;

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
    public int itemID = 0;
    public string itemName = "Unknown";
    public string itemDescription = "Unknown";
    public Sprite itemSprite = null;
    public List<StatModifier> statModifiers = new List<StatModifier>();
  }
}