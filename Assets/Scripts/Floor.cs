using UnityEngine;

public class Floor : Region
{
  public Vector2 GetFloorSize()
  {
    Vector3 boundSize = regionCollider?.bounds.size ?? Vector3.zero;
    return new Vector2(boundSize.x, boundSize.y);
  }
}