using UnityEngine;

/*
 * BackgroundController 함수 역할
 *
 * Start()
 * - 사용할 카메라를 설정한다.
 * - 인스펙터에 연결된 background1, background2에서 SpriteRenderer를 가져온다.
 *
 * Update()
 * - 두 배경을 매 프레임 왼쪽으로 이동시킨다.
 * - 화면 왼쪽 밖으로 완전히 사라진 배경이 있으면 반대쪽 배경 오른쪽 끝으로 이동시킨다.
 *
 * MoveBackground(Transform bg)
 * - 전달받은 배경 오브젝트를 scrollSpeed에 맞춰 X축 마이너스 방향으로 이동시킨다.
 *
 * RepositionIfOutside(Transform movingBackground, SpriteRenderer movingRenderer, SpriteRenderer otherRenderer)
 * - movingBackground가 카메라 왼쪽 밖으로 완전히 나갔는지 확인한다.
 * - 나갔다면 otherRenderer의 오른쪽 끝에 movingBackground를 이어 붙인다.
 *
 * GetCameraLeftX()
 * - 현재 카메라가 보고 있는 화면의 왼쪽 끝 X 좌표를 월드 좌표로 계산한다.
 */
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
        bg.position += Vector3.left * GameManager.Instance.scrollSpeed * Time.deltaTime;
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

