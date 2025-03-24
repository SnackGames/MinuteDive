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
}