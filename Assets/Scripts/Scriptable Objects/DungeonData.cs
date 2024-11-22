using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct FloorGenData
{
  public Vector2Int validFloorRange;
  public List<FloorExitType> validFloorEntranceTypes;
  public FloorExitType floorExitType;
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

      HashSet<FloorExitType?> prevFloorExitTypes = new HashSet<FloorExitType?>();
      prevFloorExitTypes.Add(null);
      
      for (int i = 1; i < maxFloorNumber; i++)
      {
        HashSet<FloorExitType?> currFloorExitTypes = new HashSet<FloorExitType?>();

        foreach (FloorExitType? exitType in prevFloorExitTypes)
        {
          List<FloorGenData> viableFloorList = GetViableFloorGenList(i, exitType);

          // 빠진 층이 있는지 검수
          if (viableFloorList.Count <= 0)
          {
            StopPlaying($"Missing floor gen data on floor {i}. FloorExitType: {exitType}");
            return;
          }

          foreach (FloorGenData viableFloor in viableFloorList)
          {
            currFloorExitTypes.Add(viableFloor.floorExitType);
          }
        }

        prevFloorExitTypes = currFloorExitTypes;
      }
    }

    public List<FloorGenData> GetViableFloorGenList(int floorNumber, FloorExitType? prevFloorExitType = null)
    {
      List<FloorGenData> list = new List<FloorGenData>();

      foreach (FloorGenData floorGenData in floorGenData)
      {
        if (floorNumber < floorGenData.validFloorRange.x || floorNumber > floorGenData.validFloorRange.y)
          continue;

        if (prevFloorExitType != null && floorGenData.validFloorEntranceTypes.Count > 0 && !floorGenData.validFloorEntranceTypes.Contains(prevFloorExitType.Value))
          continue;

        list.Add(floorGenData);
      }

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