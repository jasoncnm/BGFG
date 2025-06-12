using UnityEngine;

public class SwitchPerspective : MonoBehaviour
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
            //other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
            other.transform.position = transform.position;
            manager.SwitchPerspective();
        }
    }
}
