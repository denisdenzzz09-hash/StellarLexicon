using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    [Header("Kecepatan Scroll")]
    public float scrollSpeed = 0.2f;

    private float spriteWidth;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Kalau sudah lewat kiri layar, loncat ke kanan sejauh 2x lebar sprite
        if (transform.position.x <= -spriteWidth)
            transform.position = new Vector3(spriteWidth, transform.position.y, transform.position.z);
    }
}