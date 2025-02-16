using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
  static private TimeManager timeManagerSingleton;
  static public TimeManager GetTimeManager() { return timeManagerSingleton; }

  static private float remainTime = 0.0f;
  static private float currentTimeScale = 0.0f;

  [Header("TimeManager")]
  [ReadOnly] public float remainTimeReadOnly = 0.0f;
  [ReadOnly] public float currentTimeScaleReadOnly = 0.0f;
  public float initialRemainTime = 30.0f;
  public float initialTimeScale = 1.0f;
  public UnityEvent OnRemainTimeExpired;
  public UnityEvent OnTimeScaleChanged;

  #region Public Methods
  static public float GetRemainTime() => remainTime;
  static public float GetTimeScale() => currentTimeScale;

  public void SetTimeScale(float newTimeScale)
  {
    Time.timeScale = newTimeScale;
    currentTimeScale = newTimeScale;

    OnTimeScaleChanged.Invoke();
  }

  public void StartTimer(float newRemainTime)
  {
    remainTime = newRemainTime;
  }

  public void ReduceTime(float reduceTime)
  {
    if (remainTime <= 0f)
      return;

    remainTime -= reduceTime;
    if (remainTime <= 0f)
    {
      remainTime = 0f;
      OnRemainTimeExpired.Invoke();
    }
  }

  public void PrintCurrentTimeScale(float newTimeScale)
  {
    Debug.Log(newTimeScale);
  }
  #endregion

  #region Private Methods
  private void Awake()
  {
    timeManagerSingleton = this;
    SetTimeScale(initialTimeScale);
  }

  void Start()
  {

  }

  void Update()
  {
    if (remainTimeReadOnly != remainTime) remainTimeReadOnly = remainTime;
    if (currentTimeScaleReadOnly != currentTimeScale) currentTimeScaleReadOnly = currentTimeScale;
    ReduceTime(Time.deltaTime);
  }
  #endregion
}
