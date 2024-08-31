using TMPro;
using UnityEngine;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Main Info")]
  public class UI_MainInfo : MonoBehaviour
  {
    [Header("UI Links")]
    public TextMeshProUGUI moneyText;

    public void SetMoney(int money)
    {
      if (moneyText) moneyText.text = $"{money}";
    }
  }
}