using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    private float startX;
    private float spriteWidth;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        startX = transform.position.x;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float offset = cam.position.x * parallaxSpeed;
        transform.position = new Vector3(startX + offset, transform.position.y, transform.position.z);

        float dist = cam.position.x - transform.position.x;
        if (Mathf.Abs(dist) >= spriteWidth)
            transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);
    }
}