using Data;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
  static private FloorManager floorManagerSingleton;
  static public FloorManager GetFloorManager() { return floorManagerSingleton; }

  static private int currentFloor = 0;

  [Header("FloorManager")]
  [ReadOnly] public int currentFloorReadOnly = 0;
  public Vector2 spawnStart;
  public DungeonData dungeonData;
  public int dungeonSeed = 0;

  private List<Floor> floorList = new List<Floor>();

  #region Public Methods
  static public int GetCurrentFloor() => currentFloor;
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
    dungeonData.VerifyDungeonFloorGen();

    // 층 배열 초기화
    foreach (Floor floor in floorList)
      Destroy(floor.gameObject);
    floorList.Clear();

    System.Random random = new System.Random(dungeonSeed);

    // 매 층마다 소환 가능한 층들을 선발해서 무작위로 생성
    // sliding window 기법으로 추후에 더 최적화 가능.
    int floorNumber = 1;
    Vector3 floorPosition = new Vector3(spawnStart.x, spawnStart.y, 0.0f);
    List<FloorGenData> viableFloorList;
    FloorExitType? prevFloorExitType = FloorExitType.Center;
    while (true)
    {
      viableFloorList = dungeonData.GetViableFloorGenList(floorNumber++, prevFloorExitType);
      if (viableFloorList.Count <= 0)
        break;

      FloorGenData pickedFloorGenData = viableFloorList[random.Next(0, viableFloorList.Count)];
      float? pickedFloorHeight = pickedFloorGenData.floorPrefab.GetComponent<Floor>()?.GetFloorSize().y;

      GameObject spawnedFloor = Instantiate(pickedFloorGenData.floorPrefab, floorPosition - new Vector3(0.0f, pickedFloorHeight * 0.5f ?? 0.0f, 0.0f), Quaternion.identity);
      Floor floorComponent = spawnedFloor.GetComponent<Floor>();
      if (floorComponent != null)
        floorList.Add(floorComponent);

      floorPosition.y -= pickedFloorHeight ?? 0.0f;
      prevFloorExitType = pickedFloorGenData.floorExitType;
    }

    Debug.Log($"Generated dungeon floors with the seed {dungeonSeed}.");
  }
  #endregion

  #region Private Methods
  private void Awake()
  {
    floorManagerSingleton = this;
  }
  #endregion
}