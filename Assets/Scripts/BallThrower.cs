using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour
{

    public Transform ShootPosition;
    public Transform NextShotPosition;

    private BallController firstBall;
    private BallController nextBall;

    public GameObject BallPrefab
    {
        set { ballPrefab = value; }
        get { if (!ballPrefab) { ballPrefab = Resources.Load<GameObject>("Prefabs/Ball"); } return ballPrefab; }
    }
    private GameObject ballPrefab;

    public int CurrentBalls { set { currentBalls = value; UpdateCurrentBallsUI(); } get { return currentBalls; } }
    private int currentBalls;

    public static BallThrower Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentBalls = GameManager.Instance.GetGameSettings().BallsPerLevel;
        GameManager.Instance.OnStartGame += InitScreenBalls;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStartGame -= InitScreenBalls;
    }

    public void ThrowBall(Vector3 touchPos)
    {
        if (CurrentBalls <= 0)
        {
            UIManager.Instance.OpenLoseMenu();
            return;
        };
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 2);
            BlockController controller = hitInfo.collider.GetComponent<BlockController>();
            if (!controller) return;
            ShootBall(controller);
        }

    }

    private void UpdateCurrentBallsUI()
    {
        UIManager.Instance.UpdateBallsLeftUI(CurrentBalls);
    }

    private void ShootBall(BlockController controller)
    {
        CurrentBalls--;
        nextBall.transform.position = firstBall.transform.position;
        firstBall.FireBall(controller);
        nextBall.transform.localScale = firstBall.transform.localScale;
        firstBall = nextBall;
        GameSettings settings = GameManager.Instance.GetGameSettings();
        Color ballColor = settings.BlockColors[Random.Range(0, settings.BlockColors.Length)];
        nextBall = Instantiate(BallPrefab, NextShotPosition.position, Quaternion.identity).GetComponent<BallController>();
        nextBall.transform.parent = transform;
        nextBall.BallColor = ballColor;
        nextBall.transform.localScale = firstBall.transform.localScale / 3;
    }

    private void InitScreenBalls()
    {
        GameSettings settings = GameManager.Instance.GetGameSettings();
        Color ballColor = settings.BlockColors[Random.Range(0, settings.BlockColors.Length)];
        firstBall = Instantiate(BallPrefab, ShootPosition.position, Quaternion.identity).GetComponent<BallController>();
        firstBall.BallColor = ballColor;
        firstBall.transform.parent = transform;
        ballColor = settings.BlockColors[Random.Range(0, settings.BlockColors.Length)];
        nextBall = Instantiate(BallPrefab, NextShotPosition.position, Quaternion.identity).GetComponent<BallController>();
        nextBall.BallColor = ballColor;
        nextBall.transform.parent = transform;
        nextBall.transform.localScale = firstBall.transform.localScale / 2;
    }
}
