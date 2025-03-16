using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class TooltipManager : MonoBehaviour
{
  static private TooltipManager tooltipManagerSingleton;
  static public TooltipManager Get { get => tooltipManagerSingleton; }

  public GameObject tooltipPanel;

  public void Awake()
  {
    tooltipManagerSingleton = this;

    HideTooltip();
  }

  public void ShowTooltip(int itemID, Vector3 position)
  {
    tooltipPanel.SetActive(true);
    tooltipPanel.GetComponent<UI_ItemTooltip>()?.setTooltipData(itemID);
    tooltipPanel.transform.position = position + new Vector3(75, 0, 0);
  }
  public void HideTooltip()
  {
    tooltipPanel.SetActive(false);
  }
}
