using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMode;
using UnityEngine.UI;
using System;

public class UI_GameSetting_Toggle : MonoBehaviour
{
  [Header("UI_GameSetting_Toggle")]
  public GameSettingType gameSettingType;
  [ReadOnly] public Toggle toggle;

  // Start is called before the first frame update
  void Start()
  {
    toggle = gameObject.GetComponent<Toggle>();
    if(toggle != null )
    {
      toggle.isOn = GameSettings.GetGameSettings().GetGameSettingValueAsBool(gameSettingType);
      toggle.onValueChanged.AddListener(GameSettings.GetGameSettings().GetBoolAction(gameSettingType));
    }
  }
}
