using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Item")]
  public class ItemData : ScriptableObject
  {
    public int itemID = -1;
    public string itemName = "Unknown";
    public Sprite itemSprite = null;
  }
}