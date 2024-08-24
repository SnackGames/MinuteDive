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
      if (text == null)
        return;

      if (ModeManager != null)
        GameModeDungeon = ModeManager.GameMode as GameModeDungeon;

      if (GameModeDungeon != null)
      {
        Color TempColor = text.color;
        TempColor.a = 1f;
        text.color = TempColor;
        text.SetText($"Remain Time: {GameModeDungeon.GetRemainTime():F2}");
      }
      else
      {
        Color TempColor = text.color;
        TempColor.a = 0f;
        text.color = TempColor;
      }
    }
  }
}