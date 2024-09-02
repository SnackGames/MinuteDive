using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameSettings_Base : MonoBehaviour
{
  [Header("UI_GameSetting")]
  public GameSettingType gameSettingType;

  protected virtual void Start()
  {
    GameSettings.GetGameSettings().RegisterGameSettingUI(this);
  }

  protected virtual void OnDestroy()
  {
    GameSettings.GetGameSettings().UnregisterGameSettingUI(this);
  }

  public virtual void OnSetGameSettingValue() { }
}
