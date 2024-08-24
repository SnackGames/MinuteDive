using TMPro;
using UnityEngine;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/HP")]
  public class UI_HP : MonoBehaviour
  {
    public TextMeshProUGUI hpText;

    public void SetHP(int hp)
    {
      if (hpText != null) hpText.text = $"{hp}";
    }
  }
}