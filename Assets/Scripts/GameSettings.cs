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

  // ���� ��� ����
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

    Debug.Log("Save Game Setting!");
    BinaryFormatter formatter = new BinaryFormatter();
    FileStream stream = new FileStream(gameSettingsSavePath, FileMode.OpenOrCreate);
    formatter.Serialize(stream, gameSettingData);
    stream.Close();
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

  #region Public Methods
  static public GameSettings GetGameSettings() { return gameSettingSingleton; }

  public void EnableVibrate(bool enable)
  {
    bool cachedEnableVibrate = gameSettingData.enableVibrate;
    gameSettingData.enableVibrate = enable;
    if (cachedEnableVibrate != enable)
    {
      gameSettingChanged = true;
    }
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
  #endregion

  #region Private Methods
  private void Awake()
  {
    gameSettingSingleton = this;

    // ���� ���� �� ����� ���� �ε�
    gameSettingData = SaveLoadGameSettingsSystem.LoadGameSettings();
  }

  private void OnDestroy()
  {
    // ���� ���� �� ���� �ڵ� ����
    SaveLoadGameSettingsSystem.SaveGameSettings(gameSettingData);
  }
  #endregion
}