using System;
using UnityEngine;

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
    #region Vibration
    // ���� On / Off
    private bool enableVibrate;
    public void EnableVibrate(bool enable)
    {
      enableVibrate = enable;
      OnChangedGameOption(GameSettingType.EnableVibrate);
    }
    #endregion

    private void Awake()
    {
      // ������ 60���� ����
      Application.targetFrameRate = 60;
    }

    // ���� ���� �� ���� ���� �ҷ�����
    private void Start()
    {
      enableVibrate = GetGameSettingValueAsBool(GameSettingType.EnableVibrate);
    }
    // ���� ���� �� ���� ���� �ڵ� ����
    private void OnDestroy()
    {
      PlayerPrefs.Save();
    }

    // ���� �ɼ� ���� �� ����
    private void OnChangedGameOption(GameSettingType gameSettingType)
    {
      switch(gameSettingType)
      {
        case GameSettingType.EnableVibrate:
          PlayerPrefs.SetInt("enableVibrate", Convert.ToInt32(enableVibrate));
          break;
      }

      PlayerPrefs.Save();
    }

    #region Static Functions
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
    #endregion
  }
}