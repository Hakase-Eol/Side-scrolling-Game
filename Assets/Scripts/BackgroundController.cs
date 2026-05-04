using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)]
    float scrollSpeed = 10f;

    [SerializeField]
    Transform background1;

    [SerializeField]
    Transform background2;

    [SerializeField]
    Camera targetCamera;

    private SpriteRenderer background1Renderer;
    private SpriteRenderer background2Renderer;
    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (background1 == null || background2 == null)
            return;

        background1Renderer = background1.GetComponent<SpriteRenderer>();
        background2Renderer = background2.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (background1 == null || background2 == null || background1Renderer == null || background2Renderer == null)
            return;

        MoveBackground(background1);
        MoveBackground(background2);

        RepositionIfOutside(background1, background1Renderer, background2Renderer);
        RepositionIfOutside(background2, background2Renderer, background1Renderer);
    }

    void MoveBackground(Transform bg)
    {
        bg.position += Vector3.left * scrollSpeed * Time.deltaTime;
    }

    void RepositionIfOutside(Transform movingBackground, SpriteRenderer movingRenderer, SpriteRenderer otherRenderer)
    {
        if (movingRenderer.bounds.max.x > GetCameraLeftX())
            return;
        float newX = otherRenderer.bounds.max.x + movingRenderer.bounds.extents.x;
        movingBackground.position = new Vector3(newX, movingBackground.position.y, movingBackground.position.z);
    }

    float GetCameraLeftX()
    {
        if (targetCamera == null)
            return float.NegativeInfinity;

        float distance = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        return targetCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }
}
