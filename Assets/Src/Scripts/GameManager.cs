using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPoint;
    [SerializeField] Transform player;

    [SerializeField] CameraController cameraController;

    Transform ActivePlayer;
    Movement playerController;

    Util.Perspective gamePerspective = Util.Perspective.None;

    bool _Transiting = false;
    bool _Respawn = false;

    #region public functions
    
    void Transit()
    {
        _Transiting = true;
        playerController.SetMovement(gamePerspective);
        cameraController.SetActiveCamera(gamePerspective, ActivePlayer);
        _Transiting = false;
    }

    public void SetWindMovement(bool enable, Util.Direction dir = Util.Direction.Up, float windSpeed = 0f)
    {
        _Transiting = true;
        playerController.SetWindMovement(enable, dir, windSpeed);
        _Transiting = false;
    }

    public void SetWaterMovement(bool enable)
    {
        _Transiting = true;
        playerController.SetWaterMovement(enable);
        _Transiting = false;
    }

    public void SwitchPerspective()
    {
        if (gamePerspective == Util.Perspective.TD)
        {
            gamePerspective = Util.Perspective.FB;
        }
        else if (gamePerspective == Util.Perspective.FB)
        {
            gamePerspective = Util.Perspective.TD;
        }
        else if (gamePerspective == Util.Perspective.None)
        {
            Assert.IsTrue(false, "Error: game perspective is not set!!");
        }

        Transit();
    }

    public void Respawn()
    {
        _Respawn = true;
    }

    IEnumerator RespawnCoroutine()
    {

        playerController.FreezeMovement();

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    #endregion


    #region private helper functions


    #endregion

    #region Core Game Loop

    void Start()
    {
        Debug.Log(player);
        Debug.Log(playerSpawnPoint);
        ActivePlayer = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);

        playerController = ActivePlayer.GetComponent<Movement>();

        gamePerspective = Util.Perspective.FB;
        playerController.SetMovement(gamePerspective);
        cameraController.SetActiveCamera(gamePerspective, ActivePlayer);

        _Transiting = false;
        _Respawn = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchPerspective();
        }

        if (!_Transiting)
        {
            playerController.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (!_Transiting)
        {
            playerController.OnFixedUpdate();
        }
    }

    private void LateUpdate()
    {
        if (!_Transiting)
        {
            if (_Respawn)
            {
                StartCoroutine(RespawnCoroutine());
            }
            else
            {
                cameraController.GetActiveCamera().FollowTarget(ActivePlayer.transform);
            }
        }
    }

    #endregion

}
