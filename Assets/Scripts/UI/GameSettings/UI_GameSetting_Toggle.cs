using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMode;
using UnityEngine.UI;
using System;

public class UI_GameSetting_Toggle : UI_GameSettings_Base
{
  [Header("UI_GameSetting_Toggle")]
  [ReadOnly] public Toggle toggle;

  override protected void Start()
  {
    toggle = gameObject.GetComponent<Toggle>();
    if (toggle != null)
    {
      toggle.onValueChanged.AddListener(GameSettingManager.GetGameSettings().GetBoolAction(gameSettingType));
    }

    base.Start();
  }

  override protected void OnDestroy()
  {
    base.OnDestroy();
  }

  override public void OnSetGameSettingValue()
  {
    base.OnSetGameSettingValue();

    if (toggle != null)
    {
      toggle.isOn = GameSettingManager.GetGameSettings().GetGameSettingValueAsBool(gameSettingType);
    }
  }
}
