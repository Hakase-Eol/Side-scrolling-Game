using UnityEngine;

/*
 * ItemMover 컴포넌트 역할
 *
 * Update()
 * - 매 프레임 왼쪽으로 scrollSpeed만큼 이동한다.
 * - 카메라 왼쪽 끝보다 완전히 벗어나면 오브젝트를 소멸시킨다.
 *
 * GetCameraLeftX()
 * - 현재 카메라 왼쪽 끝의 월드 X 좌표를 반환한다.
 */
public class ItemMover : MonoBehaviour
{
    [HideInInspector]
    public float scrollSpeed = 5f;

    [HideInInspector]
    public Camera targetCamera;

    [HideInInspector]
    public bool isObstacle = false;

    public AudioClip collisionSound;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x < GetCameraLeftX())
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isObstacle)
            GameManager.Instance?.TakeDamage();
        else
            GameManager.Instance?.AddScore();

        if (collisionSound != null)
            AudioSource.PlayClipAtPoint(collisionSound, transform.position);

        Destroy(gameObject);
    }

    float GetCameraLeftX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return float.NegativeInfinity;

        float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }
}
