using UnityEngine;

public enum FloorExitType
{
  LeftMost,
  Center,
  RightMost,
  LeftAndRightMost
}

[RequireComponent(typeof(BoxCollider2D))]
public class Floor : Region
{
  public Vector2 GetFloorSize()
  {
    Vector3 boundSize = GetComponent<BoxCollider2D>()?.size ?? Vector3.zero;
    return new Vector2(boundSize.x, boundSize.y);
  }
}