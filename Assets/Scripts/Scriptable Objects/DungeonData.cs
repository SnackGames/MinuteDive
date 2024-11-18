using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public void VerifyDungeonFloorGen()
    {
      if (floorGenData.Count <= 0)
      {
        StopPlaying("There is no floors to generate.");
        return;
      }

      int maxFloorNumber = 1;
      foreach (FloorGenData floorGenData in floorGenData)
        maxFloorNumber = Mathf.Max(maxFloorNumber, floorGenData.validFloorRange.y);

      for (int i = 1; i < maxFloorNumber; i++)
      {
        List<FloorGenData> viableFloorList = GetViableFloorGenList(i);

        if (viableFloorList.Count <= 0)
        {
          StopPlaying($"Missing floor gen data on floor {i}.");
          return;
        }
      }
    }

    public List<FloorGenData> GetViableFloorGenList(int floorNumber)
    {
      List<FloorGenData> list = new List<FloorGenData>();

      foreach (FloorGenData floorGenData in floorGenData)
        if (floorNumber >= floorGenData.validFloorRange.x && floorNumber <= floorGenData.validFloorRange.y)
          list.Add(floorGenData);

      return list;
    }

    private void StopPlaying(string reason = null)
    {
      if(reason != null)
        Debug.LogError(reason);

#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#endif
    }
  }
}