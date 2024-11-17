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
        StopPlaying();
        return;
      }
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