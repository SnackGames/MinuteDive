using TMPro;
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
  Tutorial    = 1,
  Monster_1   = 100,
  Monster_2   = 101,
  Boss        = 999
}

[RequireComponent(typeof(BoxCollider2D))]
public class Floor : Region
{
  public TextMeshPro floorText;
  public int floorNumber = 0;

  private Color originalTextColor;

  public Vector2 GetFloorSize()
  {
    Vector3 boundSize = GetComponent<BoxCollider2D>()?.size ?? Vector3.zero;
    return new Vector2(boundSize.x, boundSize.y);
  }

  public void Start()
  {
    if (OnRegionEnter == null) OnRegionEnter = new RegionEvent();
    OnRegionEnter.AddListener(OnFloorEnter);
    if (OnRegionExit == null) OnRegionExit = new RegionEvent();
    OnRegionExit.AddListener(OnFloorExit);
  }

  public void InitFloor(int floor)
  {
    floorNumber = floor;

    if (floorText)
      originalTextColor = floorText.color;

    UpdateText();
  }

  public void OnFloorEnter(string regionName)
  {
    FloorManager.GetFloorManager().IncrementCurrentFloor();
    UpdateText();
  }

  public void OnFloorExit(string regionName)
  {
    UpdateText();
  }

  private void UpdateText()
  {
    if (!floorText)
      return;

    floorText.text = floorNumber.ToString();
    if (floorNumber == FloorManager.GetMaxFloor())
    {
      floorText.color = new Color(0.88f, 0.61f, 0.1f, 0.18f);
      floorText.enabled = true;
    }
    else if (floorNumber == FloorManager.GetFloorManager().currentFloorReadOnly)
    {
      floorText.color = originalTextColor;
      floorText.enabled = true;
    }
    else
    {
      floorText.color = originalTextColor;
      floorText.enabled = false;
    }
  }
}