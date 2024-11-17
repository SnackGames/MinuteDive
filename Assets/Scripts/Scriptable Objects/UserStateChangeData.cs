using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
  [CreateAssetMenu(fileName = "Data", menuName = "Data/User")]
  public class UserStateChangeData : ScriptableObject
  {
    private bool moveReserved = false;
    private bool hitReserved = false;
    private bool dyingReserved = false;
    private bool winReserved = false;

    public void reserveMove(bool reserved) { moveReserved = reserved; }
    public void resetReserveMove() { moveReserved = false; }
    public bool isMoveReserved() { return moveReserved; }

    public void reserveHit( bool reserved ) { hitReserved = reserved; }
    public void resetReserveHit() { hitReserved = false; }
    public bool isHitReserved() { return hitReserved; }

    public void reserveDying(bool reserved) { dyingReserved = reserved; }
    public void resetReserveDying() { dyingReserved = false; }
    public bool isDyingReserved() { return dyingReserved; }

    public void reserveWin(bool reserved) { winReserved = reserved; }
    public void resetReserveWin() { winReserved = false; }
    public bool isWinReserved() { return winReserved; }
  }
}