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