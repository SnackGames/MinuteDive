using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct FloorGenData
{
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

[Serializable]
public struct FloorContentData
{
  public Vector2Int targetFloorRange;
  public List<FloorContentCountData> requiredFloorContentCount;

  public readonly FloorContentData GetClone()
  {
    FloorContentData floorContentData = new FloorContentData();
    floorContentData.targetFloorRange = targetFloorRange;
    floorContentData.requiredFloorContentCount = new List<FloorContentCountData>();
    foreach (FloorContentCountData item in requiredFloorContentCount)
      floorContentData.requiredFloorContentCount.Add(item);

    return floorContentData;
  }

  public readonly int GetContentCount(FloorContentType contentType)
  {
    if (requiredFloorContentCount == null)
      return 0;

    foreach (FloorContentCountData requiredContentCountData in requiredFloorContentCount)
      if (requiredContentCountData.contentType == contentType)
        return requiredContentCountData.count;

    return 0;
  }

  public readonly int GetTotalContentCount()
  {
    if (requiredFloorContentCount == null)
      return 0;

    int totalContentCount = 0;
    foreach (FloorContentCountData requiredContentCountData in requiredFloorContentCount)
      totalContentCount += requiredContentCountData.count;

    return totalContentCount;
  }
}

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/Dungeon")]
  public class DungeonData : ScriptableObject
  {
    public string dungeonName;
    public List<FloorContentData> floorContentData;
    public List<FloorGenData> floorGenData;

    public bool VerifyDungeonFloorGen()
    {
      if (floorGenData.Count <= 0 || floorContentData.Count <= 0)
        return StopPlaying("There is no floor to generate.");

      int maxContentFloorNumber = 0;
      foreach (FloorContentData floorContentData in floorContentData)
      {
        if (floorContentData.targetFloorRange.x != maxContentFloorNumber + 1 || floorContentData.targetFloorRange.y < floorContentData.targetFloorRange.x)
          return StopPlaying($"\'Target floor range\' of \'Floor content data\' should be ascending consecutive, and fill all the integer numbers inbetween. Attempted floor range x: {floorContentData.targetFloorRange.x}");

        maxContentFloorNumber = floorContentData.targetFloorRange.y;

        if (floorContentData.targetFloorRange.y - floorContentData.targetFloorRange.x + 1 != floorContentData.GetTotalContentCount())
          return StopPlaying($"\'Target floor range\' does not match total of \'Required Floor Content Count\'. {floorContentData.targetFloorRange.y - floorContentData.targetFloorRange.x + 1} != {floorContentData.GetTotalContentCount()}");
      }

      int maxFloorNumber = 1;
      foreach (FloorGenData floorGenData in floorGenData)
        maxFloorNumber = Mathf.Max(maxFloorNumber, floorGenData.validFloorRange.y);
      if (maxContentFloorNumber != maxFloorNumber)
        return StopPlaying("Floor Content Data and Floor Gen Data's floor number doesn't match.");

      HashSet<FloorExitType?> prevFloorExitTypes = new HashSet<FloorExitType?>();
      FloorContentData viableFloorContentData = new FloorContentData();
      int viableFloorContentIndex = 0;
      for (int i = 1; i < maxFloorNumber; i++)
      {
        if (i > viableFloorContentData.targetFloorRange.y)
          viableFloorContentData = floorContentData[viableFloorContentIndex++];

        HashSet<FloorExitType?> currFloorExitTypes = new HashSet<FloorExitType?>();

        foreach (FloorExitType? exitType in prevFloorExitTypes)
        {
          List<FloorGenData> viableFloorList = GetViableFloorGenList(i, viableFloorContentData, exitType);

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

    public List<FloorGenData> GetViableFloorGenList(int floorNumber, FloorContentData floorContent, FloorExitType? prevFloorExitType = null)
    {
      List<FloorGenData> list = new List<FloorGenData>();

      foreach (FloorGenData floorGenData in floorGenData)
      {
        if (floorNumber < floorGenData.validFloorRange.x || floorNumber > floorGenData.validFloorRange.y)
          continue;

        if (prevFloorExitType != null && floorGenData.validFloorEntranceTypes.Count > 0 && !floorGenData.validFloorEntranceTypes.Contains(prevFloorExitType.Value))
          continue;

        if (floorContent.GetContentCount(floorGenData.floorContentType) <= 0)
          continue;

        list.Add(floorGenData);
      }

      return list;
    }

    private bool StopPlaying(string reason = null)
    {
      if(reason != null)
        Debug.LogError(reason);

#if UNITY_EDITOR
      EditorApplication.isPlaying = false;
#endif

      return false;
    }
  }
}