using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform posA;
    public Transform posB;
    public float speed = 3f;
    public bool requireStepOn = false;

    private Vector3 targetPos;
    private bool playerOnBoard = false;
    private Vector3 previousPosition;
    private Transform playerTransform;

    void Start()
    {
        transform.position = posA.position;
        targetPos = posB.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (requireStepOn && !playerOnBoard)
        {
            previousPosition = transform.position;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 delta = transform.position - previousPosition;
        previousPosition = transform.position;

        if (playerOnBoard && playerTransform != null)
            playerTransform.position += delta;

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            targetPos = (targetPos == posB.position) ? posA.position : posB.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnBoard = true;
            playerTransform = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnBoard = false;
            playerTransform = null;
        }
    }
}