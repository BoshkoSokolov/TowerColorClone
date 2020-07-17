using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [HideInInspector]
    public Color BlockColor;
    [HideInInspector]
    public bool IsActive = false;
    [HideInInspector]
    public Circle ParentCircle = null;

    private MeshRenderer meshRenderer;
    private Rigidbody blockRigidbody;
    private BoxCollider blockCollider;
    private List<BlockController> sameColorTouchingBlocks = new List<BlockController>();
    public GameObject OnHitParticleEffect
    {
        set
        {
            OnHitParticleEffect = onHitParticleEffect;
        }
        get
        {
            if (!onHitParticleEffect)
                onHitParticleEffect = Resources.Load<GameObject>("Prefabs/OnHitParticle");
            return onHitParticleEffect;
        }
    }
    private GameObject onHitParticleEffect;

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        float gravityForce = GameManager.Instance.GetGameSettings().BlockGravityForce;
        blockRigidbody.AddForce(Vector3.down * gravityForce);
        if (IsActive)
            blockRigidbody.AddForce((ParentCircle.CircleGO.transform.position - transform.position).normalized * gravityForce / 15);
    }

    #region INITIALIZATION
    private void InitComponentReferences()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        blockRigidbody = GetComponent<Rigidbody>();
        blockCollider = GetComponent<BoxCollider>();
    }

    private void Init()
    {
        InitComponentReferences();
        SetNewColor();
    }
    #endregion

    #region PRIVATE_METHODS
    private void SetNewColor()
    {
        BlockColor = ColorManager.Instance.GetRandomColor();
        Material newMat = new Material(meshRenderer.material);
        newMat.color = BlockColor;
        meshRenderer.material = newMat;
    }
    #endregion

    public void HitBlock()
    {
        DestroyAllTouchingBlocks();
        CameraShaker.Instance.Shake();
        GameObject paritcleGO = Instantiate(OnHitParticleEffect, transform.position, Quaternion.identity);
        Destroy(paritcleGO, paritcleGO.GetComponent<ParticleSystem>().main.duration + 0.1f);
        Collider[] colls = Physics.OverlapSphere(transform.position, GameManager.Instance.GetGameSettings().BlockExplosionRadius);
        foreach (var item in colls)
        {
            BlockController controller = item.GetComponent<BlockController>();
            if (!controller) continue;
            controller.AddExplosion(transform.position);
        }
        DestroyBlock();
    }

    public void RemoveBlock()
    {
        if (!IsActive) return;
        GameObject paritcleGO = Instantiate(OnHitParticleEffect, transform.position, Quaternion.identity);
        Destroy(paritcleGO, paritcleGO.GetComponent<ParticleSystem>().main.duration + 0.1f);
        IsActive = false;
        DestroyAllTouchingBlocks();
        DestroyBlock();
    }

    public void GroundHit()
    {
        ParentCircle.RemoveBlock(this);
        IsActive = false;
    }

    public void AddExplosion(Vector3 position)
    {
        if (!IsActive) return;
        blockRigidbody.AddExplosionForce(GameManager.Instance.GetGameSettings().BlockExplosionForce, position, GameManager.Instance.GetGameSettings().BlockExplosionForce, GameManager.Instance.GetGameSettings().BlockExplosionRadius, ForceMode.Impulse);
    }

    public void DestroyBlock()
    {
        ParentCircle.RemoveBlock(this);
        gameObject.SetActive(false);
    }

    private void MakeBlockInvisible()
    {
        meshRenderer.enabled = false;
        blockRigidbody.isKinematic = true;
        blockCollider.enabled = false;
    }

    private void DestroyAllTouchingBlocks()
    {
        Collider[] infos = Physics.OverlapBox(transform.position, new Vector3(1f, 1.80f, 1f));
        float waitTIme = Const.BLOCK_DESTROY_DELAY;
        foreach (var item in infos)
        {
            BlockController controller = item.GetComponent<BlockController>();
            if (!controller) continue;
            RaycastHit hitInfo;
            Physics.Raycast(transform.position, (controller.transform.position - transform.position).normalized, out hitInfo);
            if (hitInfo.collider && hitInfo.collider.GetComponent<BlockController>() != controller)
            {
                Debug.Log(hitInfo.collider.name + " != " + controller.name);
                continue;
            }
            else if (!hitInfo.collider) continue;
            if (controller.BlockColor == BlockColor && controller != this && controller.IsActive) controller.Invoke("RemoveBlock", waitTIme);
            waitTIme += Const.BLOCK_DESTROY_DELAY;
        }
    }

    public void DeactivateBlock()
    {
        if (!IsActive) return;
        meshRenderer.material.color = Color.black;
        blockRigidbody.isKinematic = true;
        IsActive = false;
    }

    public void ActivateBlock()
    {
        meshRenderer.material.color = BlockColor;
        blockRigidbody.isKinematic = false;
        blockRigidbody.constraints = RigidbodyConstraints.None;
        IsActive = true;
    }
}
