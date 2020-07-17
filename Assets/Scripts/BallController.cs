using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public Color BallColor { set { ballColor = value; CheckMaterialColor(value); } get { return ballColor; } }
    private Color ballColor;

    public Rigidbody RBody { get { if (!rBody) rBody = GetComponent<Rigidbody>(); return rBody; } }
    private Rigidbody rBody;

    private MeshRenderer ballRenderer;
    private Material ballMat;

    private void CheckMaterialColor(Color newColor)
    {
        if (!ballRenderer)
        {
            ballRenderer = GetComponent<MeshRenderer>();
            ballMat = new Material(ballRenderer.material);
            ballRenderer.material = ballMat;
        }
        if (ballMat.color != newColor)
        {
            ballMat.color = newColor;
        }
    }

    public void FireBall(BlockController hitTarget)
    {
        RBody.isKinematic = false;
        Destroy(gameObject, 3f);
        transform.parent = null;
        Vector3 dir = (hitTarget.transform.position - transform.position).normalized;
        Vector3 movePosition = hitTarget.transform.position;
        StartCoroutine(Move(GameManager.Instance.GetGameSettings().ShootTime, movePosition));
    }


    public IEnumerator Move(float movementTime, Vector3 newPosition)
    {
        float timer = 0f;
        float percent = 0f;
        Vector3 startPosition = transform.position;
        while (timer < movementTime)
        {
            Collider[] infos = Physics.OverlapSphere(transform.position, 0.1f);
            if (Vector3.Distance(transform.position, newPosition) <= Const.BLOCK_POSITION_SKINWIDTH)
            {
                float closestBlock = Mathf.Infinity;
                BlockController closestController = null;
                foreach (var item in infos)
                {
                    BlockController controller = item.GetComponent<BlockController>();
                    if (!controller || !controller.IsActive) continue;
                    float distance = Vector3.Distance(transform.position, controller.transform.position);
                    if (distance < closestBlock)
                    {
                        closestBlock = distance;
                        closestController = controller;
                    }
                }
                if (closestController && closestController.BlockColor == BallColor) DestroyBlock(closestController);
                else Destroy(gameObject);
                break;
            }

            timer += Time.deltaTime;
            percent = timer / movementTime;
            transform.position = Vector3.Lerp(startPosition, newPosition, percent);
            float addedY = GameManager.Instance.GetGameSettings().BallPath.Evaluate(percent);
            transform.position += Vector3.up * addedY;
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }

    private void DestroyBlock(BlockController controller)
    {
        if (controller) controller.HitBlock();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        BlockController controller = collision.collider.GetComponent<BlockController>();
        if (controller && BallColor != controller.BlockColor) return;
        RBody.AddForce((transform.position - collision.transform.position).normalized * 10, ForceMode.Impulse);
        DestroyBlock(controller);
        Destroy(this);
    }

}
