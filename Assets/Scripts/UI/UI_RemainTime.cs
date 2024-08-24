using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using GameMode;
using TMPro;

namespace UI
{
  public class UI_RemainTime : MonoBehaviour
  {
    [Header("RemainTime")]
    [ReadOnly] public GameModeDungeon GameModeDungeon;
    private GameModeManager ModeManager;

    protected TextMeshProUGUI text;

    private void Awake()
    {
      ModeManager = GameObject.FindObjectOfType<GameModeManager>();
      text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
      if (ModeManager != null)
        GameModeDungeon = ModeManager.GameMode as GameModeDungeon;

      if (GameModeDungeon != null && text != null)
      {
        text.SetText($"Remain Time: {GameModeDungeon.GetRemainTime():F2}");
      }
    }
  }
}