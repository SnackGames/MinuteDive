using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Setting
{
  [Serializable]
  public enum GameSettingType
  {
    // ���� ��� ����
    EnableVibrate,
  }

  public class GameSettings : MonoBehaviour
  {
    private HashSet<GameSettingType> changedGameSettings = new HashSet<GameSettingType>();

    #region Vibration
    // ���� On / Off
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
      // ������ 60���� ����
      Application.targetFrameRate = 60;

      // ���� ���� �� ���� ���� �ҷ�����
      changedGameSettings = new HashSet<GameSettingType>();
      enableVibrate = GetGameSettingValueAsBool(GameSettingType.EnableVibrate);
    }

    // ���� ���� �� ����� ���� ������ �ڵ� ����
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