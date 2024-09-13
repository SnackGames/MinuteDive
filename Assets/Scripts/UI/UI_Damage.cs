using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Damage")]
  public class UI_Damage : MonoBehaviour
  {
    public TextMeshProUGUI damageText;
    public Image iconImage; 

    public void SetDamage(int damage)
    {
      if (damageText != null) damageText.text = $"{damage}";
    }
  }
}