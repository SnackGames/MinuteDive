using Data;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class FloorData
{
  public int maxFloor;
}

public static class SaveLoadFloorSubsystem
{
  private static string floorSavePath = Application.persistentDataPath + "/floor.sav";

  public static void SaveFloor(FloorData floorData)
  {
    BinaryFormatter formatter = new BinaryFormatter();

    FileStream stream = new FileStream(floorSavePath, FileMode.OpenOrCreate);
    formatter.Serialize(stream, floorData);
    stream.Close();
  }

  public static FloorData LoadFloor()
  {
    FloorData floorData = null;

    if (File.Exists(floorSavePath))
    {
      BinaryFormatter formatter = new BinaryFormatter();

      FileStream stream = new FileStream(floorSavePath, FileMode.Open);
      floorData = formatter.Deserialize(stream) as FloorData;
      stream.Close();
    }
    else
    {
      floorData = new FloorData();
    }

    return floorData;
  }
}

public class FloorManager : MonoBehaviour
{
  static private FloorManager floorManagerSingleton;
  static public FloorManager GetFloorManager() { return floorManagerSingleton; }

  static private int currentFloor = 0;
  static private FloorData floorData;

  [Header("FloorManager")]
  [ReadOnly] public int currentFloorReadOnly = 0;
  [ReadOnly] public FloorData floorDataReadOnly;
  public Vector2 spawnStart;
  public DungeonData dungeonData;
  public int dungeonSeed = 0;

  private List<Floor> floorList = new List<Floor>();

  #region Public Methods
  static public int GetCurrentFloor() => currentFloor;
  static public int GetMaxFloor() => floorData.maxFloor;
  public void IncrementCurrentFloor() { currentFloor++; currentFloorReadOnly++; }
  public void ResetCurrentFloor() { currentFloor = 0; currentFloorReadOnly = 0; }

  public void GenerateFloors()
  {
    if (dungeonData == null)
    {
      Debug.LogWarning("Dungeon Data is null.");
      return;
    }

    // #TODO 추후에 게임 킬때만 실행하도록 개선
    if (!dungeonData.VerifyDungeonFloorGen())
      return;

    // 층 배열 초기화
    foreach (Floor floor in floorList)
      Destroy(floor.gameObject);
    floorList.Clear();

    // #TODO 조건 설명 추가할 것
    if (dungeonSeed < 1)
      dungeonSeed = Random.Range(0, 10000);

    System.Random random = new System.Random(dungeonSeed);

    // 매 층마다 소환 가능한 층들을 선발해서 무작위로 생성
    // sliding window 기법으로 추후에 더 최적화 가능.
    int floorNumber = 1;
    Vector3 floorPosition = new Vector3(spawnStart.x, spawnStart.y, 0.0f);
    List<FloorGenData> viableFloorList;
    FloorContentData viableFloorContentData = new FloorContentData();
    int viableFloorContentIndex = 0;
    FloorExitType? prevFloorExitType = FloorExitType.Center;
    while (true)
    {
      if (floorNumber > viableFloorContentData.targetFloorRange.y && viableFloorContentIndex < dungeonData.floorContentData.Count)
        viableFloorContentData = dungeonData.floorContentData[viableFloorContentIndex++].GetClone();

      viableFloorList = dungeonData.GetViableFloorGenList(floorNumber++, viableFloorContentData, prevFloorExitType);
      if (viableFloorList.Count <= 0)
        break;

      FloorGenData pickedFloorGenData = viableFloorList[random.Next(0, viableFloorList.Count)];
      float? pickedFloorHeight = pickedFloorGenData.floorPrefab.GetComponent<Floor>()?.GetFloorSize().y;

      for (int i = 0; i < viableFloorContentData.requiredFloorContentCount.Count; ++i)
        if (viableFloorContentData.requiredFloorContentCount[i].contentType == pickedFloorGenData.floorContentType)
        {
          FloorContentCountData tempFloorContentCountData = viableFloorContentData.requiredFloorContentCount[i];
          tempFloorContentCountData.count--;
          viableFloorContentData.requiredFloorContentCount[i] = tempFloorContentCountData;
          break;
        }

      GameObject spawnedFloor = Instantiate(pickedFloorGenData.floorPrefab, floorPosition - new Vector3(0.0f, pickedFloorHeight * 0.5f ?? 0.0f, 0.0f), Quaternion.identity);
      Floor floorComponent = spawnedFloor.GetComponent<Floor>();
      if (floorComponent != null)
        floorList.Add(floorComponent);

      floorPosition.y -= pickedFloorHeight ?? 0.0f;
      prevFloorExitType = pickedFloorGenData.floorExitType;
    }

    Debug.Log($"Generated dungeon floors with the seed {dungeonSeed}.");
  }

  public void OnRemaintimeExpired()
  {
    if(GetMaxFloor() < GetCurrentFloor())
    {
      SetMaxFloor(GetCurrentFloor());
      SaveFloor();
    }
  }

  public void SaveFloor()
  {
    SaveLoadFloorSubsystem.SaveFloor(floorData);
  }

  public void SetMaxFloor(int newmaxFloor)
  {
    if (floorData.maxFloor >= newmaxFloor)
      return;
    
    floorData.maxFloor = newmaxFloor;
    floorDataReadOnly.maxFloor = newmaxFloor;
  }
  #endregion

  #region Private Methods
  private void Awake()
  {
    floorManagerSingleton = this;
    floorData = SaveLoadFloorSubsystem.LoadFloor();
    floorDataReadOnly = floorData;
  }
  #endregion
}