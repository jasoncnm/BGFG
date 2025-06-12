using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    GameManager manager;

    private void Awake()
    {
        manager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWaterMovement(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWaterMovement(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.SetWaterMovement(false);
        }
    }
}
