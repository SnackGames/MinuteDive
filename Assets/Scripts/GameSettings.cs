using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum GameSettingType
{
  // Frame Rate
  FrameRate,

  // 진동 사용 여부
  EnableVibrate,
}

[System.Serializable]
public class GameSettingData
{
  public int targetFrameRate = 60;
  public bool enableVibrate = true;
}

public static class SaveLoadGameSettingsSystem
{
  private static string gameSettingsSavePath = Application.persistentDataPath + "/Settings.sav";

  public static void SaveGameSettings(GameSettingData gameSettingData)
  {
    if (!GameSettings.GetGameSettings().gameSettingChanged)
      return;

    BinaryFormatter formatter = new BinaryFormatter();
    FileStream stream = new FileStream(gameSettingsSavePath, FileMode.OpenOrCreate);
    formatter.Serialize(stream, gameSettingData);
    stream.Close();

    GameSettings.GetGameSettings().gameSettingChanged = false;
  }

  public static GameSettingData LoadGameSettings()
  {
    GameSettingData settingData = null;

    if(File.Exists(gameSettingsSavePath))
    {
      BinaryFormatter formatter = new BinaryFormatter();

      FileStream stream = new FileStream(gameSettingsSavePath, FileMode.Open);
      settingData = formatter.Deserialize(stream) as GameSettingData;
      stream.Close();
    }
    else
    {
      settingData = LoadDefaultGameSettings();
    }

    return settingData;
  }

  public static GameSettingData LoadDefaultGameSettings()
  {
    return new GameSettingData();
  }
}

public class GameSettings : MonoBehaviour
{
  [ReadOnly] public GameSettingData gameSettingData;
  [ReadOnly] public bool gameSettingChanged = false;

  static private GameSettings gameSettingSingleton;
  private List<UI_GameSettings_Base> gameSettingUIList;

  #region Actions
  public UnityAction<bool> GetBoolAction(GameSettingType gameSettingType)
  {
    switch (gameSettingType)
    {
      case GameSettingType.EnableVibrate:
        return EnableVibrate;
      default:
        return null;
    }
  }

  public void EnableVibrate(bool enable)
  {
    bool cachedEnableVibrate = gameSettingData.enableVibrate;
    gameSettingData.enableVibrate = enable;
    if (cachedEnableVibrate != enable)
    {
      gameSettingChanged = true;
    }
  }
  #endregion

  #region Public Methods
  static public GameSettings GetGameSettings() { return gameSettingSingleton; }

  public bool GetGameSettingValueAsBool(GameSettingType gameSettingType)
  {
    switch(gameSettingType)
    {
      case GameSettingType.EnableVibrate:
        return gameSettingData.enableVibrate;
      default:
        Debug.LogError("GetGameSettingValueAsBool: Cannot Get Setting Value of " + gameSettingType + " as Bool!");
        return false;
    }
  }
  public int GetGameSettingValueAsInt(GameSettingType gameSettingType)
  {
    switch (gameSettingType)
    {
      case GameSettingType.FrameRate:
        return gameSettingData.targetFrameRate;
      default:
        Debug.LogError("GetGameSettingValueAsInt: Cannot Get Setting Value of " + gameSettingType + " as Int!");
        return 0;
    }
  }

  public void ResetGameSettings()
  {
    gameSettingData = SaveLoadGameSettingsSystem.LoadDefaultGameSettings();
    gameSettingChanged = true;
    foreach (UI_GameSettings_Base gameSettingUI in gameSettingUIList)
    {
      gameSettingUI.OnSetGameSettingValue();
    }
  }

  public void SaveGameSettings()
  {
    SaveLoadGameSettingsSystem.SaveGameSettings(gameSettingData);
  }

  public void RegisterGameSettingUI(UI_GameSettings_Base gameSettingUI)
  {
    gameSettingUIList.Add(gameSettingUI);
    gameSettingUI.OnSetGameSettingValue();
  }
  public void UnregisterGameSettingUI(UI_GameSettings_Base gameSettingUI)
  {
    gameSettingUIList.Remove(gameSettingUI);
  }
  #endregion

  #region Private Methods
  private void Awake()
  {
    gameSettingSingleton = this;
    gameSettingUIList = new List<UI_GameSettings_Base>();

    // 게임 시작 시 저장된 세팅 로드
    gameSettingData = SaveLoadGameSettingsSystem.LoadGameSettings();
  }

  private void OnDestroy()
  {
    // 게임 종료 시 세팅 자동 저장
    SaveGameSettings();
  }
  #endregion
}