using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FloorGenData
{
  public Vector2Int validFloorRange;
  public GameObject floorPrefab;
}

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Dungeon")]
  public class DungeonData : ScriptableObject
  {
    public string dungeonName;
    public List<FloorGenData> floorGenData;
  }
}