using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NRespawn : MonoBehaviour
{
    Transform graphics;

    GameManager manager;

    Vector3 originalPosition;

    private void Start()
    {
        graphics = transform.GetComponentInChildren<MeshRenderer>().transform;
        manager = FindAnyObjectByType<GameManager>();
        originalPosition = transform.position;
    }

    public void OnHitObject()
    {

        graphics.gameObject.SetActive(false);
        manager.Respawn();

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Respawn"))
        {
            OnHitObject();
        }
    }
}
