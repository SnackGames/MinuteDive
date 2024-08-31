using UnityEngine;
using Setting;

namespace UI
{
  [DisallowMultipleComponent]
  [AddComponentMenu("UI/Pause Menu")]
  public class UI_PauseMenu : MonoBehaviour
  {
    public GameObject pauseUI;

    public void PauseGame(bool pause)
    {
      Time.timeScale = pause ? 0.0f : 1.0f;
      pauseUI.SetActive(pause);

      // �Ͻ����� ������ �� ����� ���� �ϰ� ����
      if (pause == false)
      {
        GameSettings gameSettings = FindObjectOfType<GameSettings>();
        if (gameSettings != null)
        {
          gameSettings.SaveChangedGameSettings();
        }
      }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
      if (!hasFocus) PauseGame(true);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
      if (pauseStatus) PauseGame(true);
    }
  }
}