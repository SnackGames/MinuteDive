using UnityEngine;
using Unit;

  [RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
  public Player player;
  public float lerpSpeed = 3.0f;
  public Vector2 cameraOffset = Vector2.zero;
  public bool fixXAxis = false;

  protected Camera playerCamera;

  private void Awake()
  {
    playerCamera = GetComponent<Camera>();
  }

  protected virtual void Update()
  {
    if (player == null) return;

    Vector3 targetPosition = player.transform.position + new Vector3(cameraOffset.x, cameraOffset.y);
    targetPosition.z = transform.position.z;
    if (fixXAxis) targetPosition.x = transform.position.x;

    transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
  }
}
