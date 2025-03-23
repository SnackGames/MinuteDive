using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct FloorGenData
{
  public string floorGenComment;
  public Vector2Int validFloorRange;
  public List<FloorExitType> validFloorEntranceTypes;
  public FloorExitType floorExitType;
  public FloorContentType floorContentType;
  public GameObject floorPrefab;
}

[Serializable]
public struct FloorContentCountData
{
  public FloorContentType contentType;
  public int count;
}

namespace Data
{
  [CreateAssetMenu(menuName = "Data/Dungeon")]
  public class DungeonData : ScriptableObject
  {
    public string dungeonName;
    public List<DungeonBiomeData> dungeonBiomeData;

    public bool VerifyDungeonFloorGen()
    {
      if (dungeonBiomeData.Count <= 0)
        return StopPlaying("There is no dungeon biome to generate.");

      foreach (DungeonBiomeData biomeData in dungeonBiomeData)
        if (biomeData?.VerifyDungeonBiomeGen() == false)
          return false;

      return true;
    }

    private bool StopPlaying(string reason = null)
    {
      if (reason != null)
        Debug.LogError(reason);

#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#endif

      return false;
    }
  }

  [CreateAssetMenu(menuName = "Data/DungeonBiome")]
  public class DungeonBiomeData : ScriptableObject
  {
    public string biomeName;
    public List<FloorContentCountData> requiredFloorContentCount;
    public List<FloorGenData> floorGenData;

    public DungeonBiomeData GetClone()
    {
      DungeonBiomeData clone = CreateInstance<DungeonBiomeData>();
      clone.biomeName = biomeName;
      clone.requiredFloorContentCount = new List<FloorContentCountData>(requiredFloorContentCount);
      clone.floorGenData = new List<FloorGenData>(floorGenData);
      return clone;
    }

    public int GetContentCount(FloorContentType contentType)
    {
      if (requiredFloorContentCount == null)
        return 0;

      foreach (FloorContentCountData requiredContentCountData in requiredFloorContentCount)
        if (requiredContentCountData.contentType == contentType)
          return requiredContentCountData.count;

      return 0;
    }

    public int GetFloorContentTotalCount()
    {
      if (requiredFloorContentCount == null)
        return 0;

      int totalContentCount = 0;
      foreach (FloorContentCountData requiredContentCountData in requiredFloorContentCount)
        totalContentCount += requiredContentCountData.count;

      return totalContentCount;
    }

    public bool VerifyDungeonBiomeGen()
    {
      if (floorGenData.Count <= 0 || requiredFloorContentCount.Count <= 0)
        return StopPlaying("There is no floor to generate.");

      int maxFloorCount = GetFloorContentTotalCount();

      int maxFloorNumber = 0;
      foreach (FloorGenData floorGenData in floorGenData)
        maxFloorNumber = Mathf.Max(maxFloorNumber, floorGenData.validFloorRange.y);
      if (maxFloorCount != maxFloorNumber)
        return StopPlaying("Floor Content Data and Floor Gen Data's floor number doesn't match.");

      HashSet<FloorExitType?> prevFloorExitTypes = new HashSet<FloorExitType?>();
      for (int i = 1; i < maxFloorNumber; i++)
      {
        HashSet<FloorExitType?> currFloorExitTypes = new HashSet<FloorExitType?>();

        foreach (FloorExitType? exitType in prevFloorExitTypes)
        {
          List<FloorGenData> viableFloorList = GetViableFloorGenList(i, exitType);

          // 빠진 층이 있는지 검수
          if (viableFloorList.Count <= 0)
            return StopPlaying($"Missing floor gen data on floor {i}. FloorExitType: {exitType}");

          foreach (FloorGenData viableFloor in viableFloorList)
            currFloorExitTypes.Add(viableFloor.floorExitType);
        }

        prevFloorExitTypes = currFloorExitTypes;
      }

      return true;
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

        if (GetContentCount(floorGenData.floorContentType) <= 0)
          continue;

        list.Add(floorGenData);
      }

      return list;
    }

    private bool StopPlaying(string reason = null)
    {
      if (reason != null)
        Debug.LogError(reason);

#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#endif

      return false;
    }
  }
}