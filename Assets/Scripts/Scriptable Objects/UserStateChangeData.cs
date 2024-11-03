using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/User")]
  public class UserStateChangeData : ScriptableObject
  {
    private bool hitReserved = false;

    public void reserveHit( bool reserved ) { hitReserved = reserved; }
    public void resetReserveHit() { hitReserved = false; }
    public bool isHitReserved() { return hitReserved; }
  }
}