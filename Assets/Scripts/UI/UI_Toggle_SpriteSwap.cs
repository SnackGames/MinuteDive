using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof(Toggle))]
  [AddComponentMenu("UI/Toggle Sprite Swap")]
  public class UI_Toggle_SpriteSwap : MonoBehaviour
  {
    public Sprite toggleSprite;

    private Toggle toggle;

    private void Awake()
    {
      toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
      toggle.toggleTransition = Toggle.ToggleTransition.None;
      toggle.onValueChanged.AddListener(OnToggleValueChanged);
      OnToggleValueChanged(toggle.isOn);
    }

    void OnToggleValueChanged(bool newValue)
    {
      Image targetImage = toggle.targetGraphic as Image;
      if (targetImage != null)
      {
        targetImage.overrideSprite = newValue ? toggleSprite : null;
      }
    }
  }
}
