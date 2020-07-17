using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Animator AnimatorCamera;
    public Transform CameraHolder;
    public Transform CameraTransform;

    public bool FinishedOpenAnimation { set { finishedOpenAnimation = value; if (finishedOpenAnimation) FinishOpenAnimation(); } get { return finishedOpenAnimation; } }
    public bool finishedOpenAnimation = false;

    private const string ANIMATION_OPEN_TRIGGER = "Open";
    private const string OPEN_ANIMATION_NAME = "CameraOpen";

    private Vector3 firstTouchPosition = Vector3.zero;

    public static CameraController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnStartGame += StartGame;
        transform.position = GameManager.Instance.GetGameSettings().LevelGroundCenterPosition;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartGame -= StartGame;
    }

    public void StartGame()
    {
        AnimatorCamera.SetTrigger(ANIMATION_OPEN_TRIGGER);
    }

    public void CheckCameraHeight(Tower tower)
    {
        if (AnimatorCamera.enabled) return;
        int lowestStanding = tower.HighestCircle.CircleIndex - GameManager.Instance.GetGameSettings().ActiveBlocks;
        if (lowestStanding < 0) lowestStanding = 0;
        Vector3 newPosition = CameraHolder.transform.position;
        newPosition.y = tower.Circles[lowestStanding].CircleGO.transform.position.y + Const.CAMERA_BOTTOM_BLOCK_OFFSET;
        MoveCameraToPosition(newPosition);

    }

    private void MoveCameraToPosition(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCameraOverTime(position, GameManager.Instance.GetGameSettings().Controlls.CameraMovementSpeed));
    }

    private IEnumerator MoveCameraOverTime(Vector3 position, float speed)
    {
        while (true)
        {
            CameraHolder.transform.position = Vector3.MoveTowards(CameraHolder.transform.position, position, speed * Time.deltaTime);
            if (Vector3.Distance(CameraHolder.transform.position, position) < Const.BLOCK_POSITION_SKINWIDTH)
            {
                CameraHolder.transform.position = position;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void FinishOpenAnimation()
    {
        AnimatorCamera.enabled = false;
        LevelController.Instance.DeactivateBlocks();
        UIManager.Instance.OpenProgressBar();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (AnimatorCamera.enabled) return;
        if (Input.GetMouseButtonDown(0)) firstTouchPosition = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            InterpretControlls(Input.mousePosition - firstTouchPosition);
            firstTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            BallThrower.Instance.ThrowBall(Input.mousePosition);
            firstTouchPosition = Vector3.zero;
        }
    }
#endif
    
    public void InterpretControlls(Vector3 deltaMove, bool startCoroutine = false)
    {
        StopAllCoroutines();
        int side = deltaMove.x > 0 ? 1 : -1;
        float magnitude = deltaMove.magnitude;
        Vector3 newAngles = CameraHolder.localEulerAngles;
        newAngles.y += magnitude * side * Time.deltaTime * GameManager.Instance.GetGameSettings().Controlls.CameraRotateSpeed;
        CameraHolder.localEulerAngles = newAngles;
        if (startCoroutine) StartCoroutine(LateCameraMover(deltaMove, side));
    }

    private IEnumerator LateCameraMover(Vector3 deltaMove, int side)
    {
        float mag = deltaMove.magnitude;
        float percent = 100 / mag;
        while(mag > 0)
        {
            Vector3 newAngles = CameraHolder.localEulerAngles;
            newAngles.y += mag * side * Time.deltaTime * GameManager.Instance.GetGameSettings().Controlls.CameraRotateSpeed;
            CameraHolder.localEulerAngles = newAngles;
            mag -= percent * 1;
            mag = Mathf.Clamp(mag, 0, Mathf.Infinity);
            yield return new WaitForEndOfFrame();
        }
    }
}
