using System;
using UI;
using UnityEngine;
using UnityEngine.Events;

// bool isHit, int prevHp, int hp 
[Serializable]
public class HealthEvent : UnityEvent<bool, int, int> { }

public class Health : MonoBehaviour
{
  [SerializeField]
  [ReadOnly]
  private int hp = 1;

  public UI_HP hpUI;

  public HealthEvent OnHPChanged;

  private void Awake()
  {
    hpUI?.SetHP(hp);
  }

  public virtual void SetHP(int newHp, bool isHit = false)
  {
    int prevHp = hp;
    hp = newHp;
    hpUI?.SetHP(hp);
    OnHPChanged.Invoke(isHit, prevHp, hp);
  }

  public virtual void OnHit(int damage)
  {
    SetHP(Math.Max(0, hp - damage), true);
  }
}