using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    [HideInInspector]
    public Camera targetCamera;

    private BoxCollider2D platformCollider;
    private List<GameObject> bonusItems = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        if (platformCollider != null)
            platformCollider.isTrigger = true;
    }

    float GetCameraLeftX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return float.NegativeInfinity;

        float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.left * GameManager.Instance.scrollSpeed * Time.deltaTime;
        transform.position += movement;

        for (int i = bonusItems.Count - 1; i >= 0; i--)
        {
            if (bonusItems[i] != null)
                bonusItems[i].transform.position += movement;
            else
                bonusItems.RemoveAt(i);        
        }

        if (transform.position.x < GetCameraLeftX())
            Destroy(gameObject);
    }

    public void AddBonusItem(GameObject item)
    {
        bonusItems.Add(item);
    }
    
    void OnDestroy()
    {
        foreach (GameObject item in bonusItems)
        {
            if (item != null)
                Destroy(item);
        }
        bonusItems.Clear();

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlatformDestroyed();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Destroy(gameObject);
    }
}
