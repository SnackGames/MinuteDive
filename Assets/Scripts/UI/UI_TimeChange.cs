using TMPro;
using UnityEngine;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Time Change")]
  public class UI_TimeChange : MonoBehaviour
  {
    public TextMeshProUGUI timeText;

    public void SetTimeChange(float time)
    {
      if (timeText != null) timeText.text = $"{time:F2}";
    }
  }
}