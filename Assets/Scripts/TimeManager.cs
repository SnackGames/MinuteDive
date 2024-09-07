using System.Collections;
using System.Collections.Generic;
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

  static public float GetRemainTime() => remainTime;
  static public float GetTimeScale() => currentTimeScale;
  static public void SetTimeScale(float newTimeScale) { Time.timeScale = newTimeScale; currentTimeScale = newTimeScale; }
  // TODO: OnSetTimeSclae 이벤트 추가

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
    remainTimeReadOnly = remainTime;
    currentTimeScaleReadOnly = currentTimeScale;
    ReduceTime(Time.deltaTime);
  }

  public void StartTimer(float newRemainTime)
  {
    remainTime = newRemainTime;
  }

  public void ReduceTime(float reduceTime)
  {
    if (remainTime > 0f)
    {
      remainTime -= reduceTime;
      if (remainTime < 0f)
      {
        remainTime = 0f;
        OnRemainTimeExpired.Invoke();
      }
    }
  }
}
