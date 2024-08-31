using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Setting
{
  [Serializable]
  public enum GameSettingType
  {
    // 진동 사용 여부
    EnableVibrate,
  }

  public class GameSettings : MonoBehaviour
  {
    private HashSet<GameSettingType> changedGameSettings = new HashSet<GameSettingType>();

    #region Vibration
    // 진동 On / Off
    private bool enableVibrate;
    public void EnableVibrate(bool enable)
    {
      Debug.Log("Enabled Vibrate!");
      bool cachedEnableVibrate = enableVibrate;
      enableVibrate = enable;
      if (cachedEnableVibrate != enable)
      {
        changedGameSettings.Add(GameSettingType.EnableVibrate);
      }
    }
    #endregion

    #region Public Methods
    public void SaveChangedGameSettings()
    {
      foreach (GameSettingType changedGameSettingType in changedGameSettings)
      {
        switch(changedGameSettingType)
        {
          case GameSettingType.EnableVibrate:
            SetGameSettingValueAsBool(changedGameSettingType, enableVibrate);
            break;
          default:
            continue;
        }
      }
      PlayerPrefs.Save();
      changedGameSettings.Clear();
    }

    public UnityAction<bool> GetBoolAction(GameSettingType gameSettingType)
    {
      switch(gameSettingType)
      {
        case GameSettingType.EnableVibrate:
          return EnableVibrate;
        default:
          return null;
      }
    }
    #endregion

    #region Private Methods
    private void Awake()
    {
      // 프레임 60으로 고정
      Application.targetFrameRate = 60;

      // 게임 시작 시 저장 정보 불러오기
      changedGameSettings = new HashSet<GameSettingType>();
      enableVibrate = GetGameSettingValueAsBool(GameSettingType.EnableVibrate);
    }

    // 게임 종료 시 변경된 세팅 있으면 자동 저장
    private void OnDestroy()
    {
      SaveChangedGameSettings();
    }
    #endregion

    #region Static Methods
    public static string GetGameSettingKey(GameSettingType gameSettingType)
    {
      switch(gameSettingType)
      {
        case GameSettingType.EnableVibrate:
          return "enableVibrate";
      }
      return string.Empty;
    }

    public static bool GetGameSettingValueAsBool(GameSettingType gameSettingType)
    {
      if(PlayerPrefs.HasKey(GetGameSettingKey(gameSettingType)) == true)
      {
        return Convert.ToBoolean(PlayerPrefs.GetInt(GetGameSettingKey(gameSettingType)));
      }

      return GetDefaultGameSettingValueAsBool(gameSettingType);
    }

    public static bool GetDefaultGameSettingValueAsBool(GameSettingType gameSettingType)
    {
      switch(gameSettingType)
      {
        case GameSettingType.EnableVibrate: return true;
      }

      Debug.LogError("GetDefaultGameSettingValueAsBool: Cannot Find Default Game Setting Value as Bool!");
      return false;
    }

    public static bool SetGameSettingValueAsBool(GameSettingType gameSettingType, bool _value)
    {
      string gameSettingKey = GetGameSettingKey(gameSettingType);
      if (gameSettingKey == string.Empty)
      {
        Debug.LogError("SetGameSettingValueAsBool: Cannot Find Valid game Setting Key!");
        return false;
      }

      PlayerPrefs.SetInt(gameSettingKey, Convert.ToInt32(_value));
      return true;
    }
    #endregion
  }
}