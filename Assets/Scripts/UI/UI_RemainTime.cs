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
    [ReadOnly] public GameModeType GameModeType;

    protected TextMeshProUGUI text;

    private void Awake()
    {
      //GameModeDungeon = GameObject.FindObjectOfType<GameModeDungeon>();
      GameModeDungeon = null;
      if(GameModeDungeon != null)
        GameModeType = GameModeDungeon.GetGameModeType();

      text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Update()
    {
      if(GameModeDungeon != null && text != null)
      {
        text.SetText($"Remain Time: {GameModeDungeon.GetRemainTime():F2}");
      }
    }
  }
}