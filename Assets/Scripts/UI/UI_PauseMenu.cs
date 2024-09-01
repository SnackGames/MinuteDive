using UnityEngine;

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

      // 일시정지 해제될 때 변경된 세팅 일괄 저장
      if (!pause)
      {
        GameSettings.GetGameSettings().SaveGameSettings();
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