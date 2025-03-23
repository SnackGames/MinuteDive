using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GlobalInputDetector : MonoBehaviour
{
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {
      GameObject currentTooltipOwner = TooltipManager.Get.currentTooltipOwner;
      if (currentTooltipOwner == null)
        return;

      PointerEventData pointerData = new PointerEventData(EventSystem.current)
      {
        position = Input.mousePosition
      };

      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(pointerData, results);

      bool clickedOnTooltipOwner = false;
      foreach (var result in results)
      {
        if (result.gameObject == currentTooltipOwner)
        {
          clickedOnTooltipOwner = true;
          break;
        }
      }

      if (!clickedOnTooltipOwner)
      {
        TooltipManager.Get.HideTooltip();
      }
    }
  }
}
