using UnityEngine;
using TMPro;

namespace UI
{
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class UI_RemainTime : MonoBehaviour
  {
    [Header("RemainTime")]
    [ReadOnly] public GameModeDungeon GameModeDungeon;
    public GameModeManager ModeManager;
    public GameObject timeChangePrefab;

    protected Animator animator;
    protected TextMeshProUGUI text;

    private void Awake()
    {
      animator = GetComponent<Animator>();
      text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
      if (ModeManager != null)
        GameModeDungeon = ModeManager.GameMode as GameModeDungeon;

      if (GameModeDungeon != null)
      {
        float time = TimeManager.GetRemainTime();
        bool isTimeUrgent = time <= 10.0f;
        text.enabled = true;
        // #TODO 색을 animation으로 이전하면서 작동하지 않게 됨
        // text.color = isTimeUrgent ? Color.red : Color.white;
        text.SetText(isTimeUrgent ? $"{time:F2}" : $"{time:F0}");
      }
      else
      {
        text.enabled = false;
      }
    }

    public virtual void OnTimeChanged(float time)
    {
      if (time > 0.0f)
      {
        animator.SetTrigger("Plus");
      }
      else
      {
        animator.SetTrigger("Minus");
      }

      GameObject timeChangeUI = Instantiate(timeChangePrefab, transform.position, Quaternion.identity);
      timeChangeUI.transform.SetParent(transform,true);
      timeChangeUI.GetComponent<UI_TimeChange>()?.SetTimeChange(time);
    }
  }
}