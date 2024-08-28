using UnityEngine;
using TMPro;

namespace UI
{
  public class UI_RemainTime : MonoBehaviour
  {
    [Header("RemainTime")]
    [ReadOnly] public GameModeDungeon GameModeDungeon;
    public GameModeManager ModeManager;

    protected TextMeshProUGUI text;

    private void Awake()
    {
      text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
      if (text == null)
        return;

      if (ModeManager != null)
        GameModeDungeon = ModeManager.GameMode as GameModeDungeon;

      if (GameModeDungeon != null)
      {
        float time = GameModeDungeon.GetRemainTime();
        bool isTimeUrgent = time <= 10.0f;
        text.enabled = true;
        text.color = isTimeUrgent ? Color.red : Color.white;
        text.SetText(isTimeUrgent ? $"{time:F2}" : $"{time:F0}");
      }
      else
      {
        text.enabled = false;
      }
    }
  }
}