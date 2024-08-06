using UnityEngine;
using Unit;

  [RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
  public Player player;
  public float lerpSpeed = 3.0f;

  protected Camera playerCamera;

  private void Awake()
  {
    playerCamera = GetComponent<Camera>();
  }

  protected virtual void Update()
  {
    if (player == null) return;

    Vector3 targetPosition = player.transform.position;
    targetPosition.z = transform.position.z;

    transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
  }
}
