using TMPro;
using UnityEngine;

namespace UI
{
  [RequireComponent(typeof(Animator))]
  public class UI_RegionTitle : MonoBehaviour
  {
    public TextMeshProUGUI regionText;

    public void OnRegionChanged(string regionTitle)
    {
      GetComponent<Animator>()?.SetTrigger("Play");
      regionText?.SetText(regionTitle);
    }
  }
}