using Data;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
  public Vector2 spawnStart;
  public DungeonData dungeonData;

  private Floor[] floorList;

  public void GenerateFloors()
  {
    // 층 배열 초기화
    if (floorList != null)
      foreach (Floor floor in floorList)
        if (floor != null)
          Destroy(floor.gameObject);

    // 매 층마다 소환 가능한 층들을 선발해서 무작위로 생성
    // sliding window로 추후에 더 최적화 가능.
  }
}