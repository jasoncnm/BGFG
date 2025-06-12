using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    FCamera fCamera;
    TopDownCamera topDownCamera;

    GameCamera ActiveCamera = null;

    private void Awake()
    {
        fCamera = transform.GetComponentInChildren<FCamera>();
        topDownCamera = transform.GetComponentInChildren<TopDownCamera>();

    }


    public void SetActiveCamera(Util.Perspective perspective, Transform target)
    {
        switch(perspective)
        {
            case Util.Perspective.TD:
                ActiveCamera = topDownCamera.GetComponent<GameCamera>();
                fCamera.gameObject.SetActive(false);
                break;
            case Util.Perspective.FB:
                ActiveCamera = fCamera.GetComponent<GameCamera>();
                topDownCamera.gameObject.SetActive(false);
                break;
            case Util.Perspective.None:
                Assert.IsTrue(false, "ERROR: No Perspective is Given to Camera Script!!!");
                break;
        }
 
        ActiveCamera.gameObject.SetActive(true);
        ActiveCamera.InitPosition(target);
    }


    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        GameCamera cam = GetActiveCamera();

        Vector3 originalPos = cam.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }



    public GameCamera GetActiveCamera()
    {
        return ActiveCamera;
    }
}
