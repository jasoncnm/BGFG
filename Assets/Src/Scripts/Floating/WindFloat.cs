using UnityEngine;

public class WindFloat : MonoBehaviour
{
    GameManager manager;

    [SerializeField] Util.Direction dir;

    [SerializeField, Range(0, 100)] float windSpeed;

    private void Awake()
    {
        manager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWindMovement(true, dir, windSpeed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWindMovement(true, dir, windSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWindMovement(false);
        }
    }
}
