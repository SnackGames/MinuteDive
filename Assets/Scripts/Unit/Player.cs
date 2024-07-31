using Unity.Collections;
using UnityEngine;

namespace Unit
{
  public enum PlayerState
  {
    Idle,
    Falling
  }

  public class Player : MonoBehaviour
  {
    [ReadOnly]
    public PlayerState playerState;

    protected Rigidbody2D Body;
  }
}