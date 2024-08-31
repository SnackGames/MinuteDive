using System;
using UnityEngine;

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
    #region Vibration
    // 진동 On / Off
    private bool enableVibrate;
    public void EnableVibrate(bool enable)
    {
      enableVibrate = enable;
      OnChangedGameOption(GameSettingType.EnableVibrate);
    }
    #endregion

    private void Awake()
    {
      // 프레임 60으로 고정
      Application.targetFrameRate = 60;
    }

    // 게임 시작 시 저장 정보 불러오기
    private void Start()
    {
      enableVibrate = GetGameSettingValueAsBool(GameSettingType.EnableVibrate);
    }
    // 게임 종료 시 현재 세팅 자동 저장
    private void OnDestroy()
    {
      PlayerPrefs.Save();
    }

    // 게임 옵션 변경 시 저장
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