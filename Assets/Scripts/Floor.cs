using UnityEngine;

public enum FloorExitType
{
  LeftMost         = 0,
  Center           = 1,
  RightMost        = 2,
  LeftAndRightMost = 3
}

public enum FloorContentType
{
  Empty       = 0,
  Monster_1   = 100,
  Monster_2   = 101,
  Boss        = 999
}

[RequireComponent(typeof(BoxCollider2D))]
public class Floor : Region
{
  public Vector2 GetFloorSize()
  {
    Vector3 boundSize = GetComponent<BoxCollider2D>()?.size ?? Vector3.zero;
    return new Vector2(boundSize.x, boundSize.y);
  }

  public void Start()
  {
    if (OnRegionEnter == null) OnRegionEnter = new RegionEvent();
    OnRegionEnter.AddListener(OnFloorEnter);
  }

  public void OnFloorEnter(string regionName)
  {
    FloorManager.GetFloorManager().IncrementCurrentFloor();
  }
}