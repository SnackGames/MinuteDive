using UnityEngine;

public class GameSettings : MonoBehaviour
{
  // ���� On / Off
  public static bool enableVibrate = true;
  public void EnableVibrate(bool enable) => enableVibrate = enable;
}