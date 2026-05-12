using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int score = 0;
    public int Score => score;
    
    public void AddScore(int amount = 1)
    {
        if (isGameOver) return;

        score += amount;
        Debug.Log($"Score: {score}");
        scoreText.text = $"Score: {score}";
    }

    [Header("Prefabs & Spawn Settings")]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] float itemSpawnInterval = 2f;
    [SerializeField] Vector2 itemSpawnYRange = new Vector2(-2f, 2f);
    [SerializeField] int itemsPerObstacle = 10;
    [SerializeField] Transform itemParent;

    [Header("Environment Settings")]
    [SerializeField] public float scrollSpeed = 10f;
    [SerializeField] Camera targetCamera;

    [Header("UI Settings")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] GameObject[] heartObjects; // 하트 UI 오브젝트들을 담을 배열
    [SerializeField] GameObject gameOverUI;     // 게임 오버 시 띄울 UI 패널

    [Header("Player Stats")]
    [SerializeField] int maxHp = 5; // 하트 개수에 맞춰 5로 변경

    [Header("Speed Scale")]
    [SerializeField]
    float speedIncreaseRate = 0.5f;

    [SerializeField]
    float maxScrollSpeed = 30f;
    
    int hp;
    bool isGameOver = false;
    float spawnTimer;
    int itemSpawnCount;
    float distance;

    void Awake()
    {
        Instance = this;
        hp = maxHp;

        // 시작할 때 게임 오버 UI는 꺼둡니다.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        scrollSpeed = Mathf.Min(scrollSpeed + speedIncreaseRate * Time.deltaTime, maxScrollSpeed);
        Debug.Log($"Scroll Speed: {scrollSpeed}");
        
        spawnTimer += Time.deltaTime;

        distance += scrollSpeed * Time.deltaTime;
        if (distanceText != null)
            distanceText.text = $"Distance: {(int)distance}m";
            
        if (spawnTimer < itemSpawnInterval)
            return;

        spawnTimer = 0f;

        itemSpawnCount++;
        if (itemSpawnCount >= itemsPerObstacle)
        {
            itemSpawnCount = 0;
            SpawnObstacle();
        }
        else
        {
            SpawnItem();
        }
    }

    public void TakeDamage()
    {
        if (isGameOver) return;

        hp--;
        Debug.Log($"HP: {hp}");

        // 데미지를 입을 때마다 해당하는 인덱스의 하트 UI를 끕니다.
        // 예: hp가 4가 되면 heartObjects[4] (5번째 하트)가 꺼집니다.
        if (hp >= 0 && hp < heartObjects.Length)
        {
            heartObjects[hp].SetActive(false);
        }

        if (hp <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");

        // 게임 오버 UI 활성화
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    void SpawnItem()
    {
        float spawnX = GetCameraRightX();
        float spawnY = Random.Range(itemSpawnYRange.x, itemSpawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = item.GetComponent<ItemMover>();
        if (mover == null)
            mover = item.AddComponent<ItemMover>();

        mover.scrollSpeed = scrollSpeed;
        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
    }

    void SpawnObstacle()
    {
        float spawnX = GetCameraRightX();
        float spawnY = Random.Range(itemSpawnYRange.x, itemSpawnYRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, itemParent);

        ItemMover mover = obstacle.GetComponent<ItemMover>();
        if (mover == null)
            mover = obstacle.AddComponent<ItemMover>();

        mover.scrollSpeed = scrollSpeed;
        mover.targetCamera = targetCamera != null ? targetCamera : Camera.main;
        mover.isObstacle = true;
    }

    float GetCameraRightX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return 10f;

        float distance = Mathf.Abs(cam.transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, distance)).x;
    }
}