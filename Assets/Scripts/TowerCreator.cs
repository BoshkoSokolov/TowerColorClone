using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerCreator : MonoBehaviour
{
    public static GameObject BlockPrefab
    {
        set { blockPrefab = value; }
        get
        {
            if (!blockPrefab) blockPrefab = Resources.Load<GameObject>("Prefabs/BlockPrefab");
            return blockPrefab;
        }
    }
    private static GameObject blockPrefab;
    public static GameObject CircleBoxCollision
    {
        set { circleBoxCollision = value; }
        get
        {
            if (!circleBoxCollision) circleBoxCollision = Resources.Load<GameObject>("Prefabs/CircleBlockCollision");
            return circleBoxCollision;
        }
    }
    private static GameObject circleBoxCollision;


    public static Tower BuildTower(GameSettings gameSettings, string towerName = "Tower")
    {
        if (!BlockPrefab)
        {
            Debug.LogError("BlockPrefab is missing from Resources folder");
            return null;
        }
        float angle = 360 / gameSettings.NumberOfBlockPerCircle;
        Tower tower = new Tower();
        int blocksInRow = (int)(360 / angle);
        float distanceBetweenBlocks = 56.7f / angle;
        angle /= 2;
        GameObject towerGO = new GameObject(towerName);
        towerGO.transform.position = gameSettings.LevelGroundCenterPosition;
        for (int i = 0; i < gameSettings.TowerHeight; i++)
        {
            Circle circle = new Circle();
            GameObject circleGO = new GameObject("Circle_" + i);
            Vector3 CircleBlockCollisonPosition = gameSettings.LevelGroundCenterPosition + Vector3.up * (i - 1) * gameSettings.BlockHeight;
            Vector3 circleCenterPosition = gameSettings.LevelGroundCenterPosition + Vector3.up * i * gameSettings.BlockHeight;
            circleGO.transform.position = circleCenterPosition;
            Vector3 blockPosition = circleCenterPosition + Vector3.right * distanceBetweenBlocks / blocksInRow;
            float rotateDegrees = 0;
            for (int j = 0; j < blocksInRow; j++)
            {
                Quaternion rotation = Quaternion.AngleAxis(rotateDegrees, Vector3.up);
                GameObject newBlock = Instantiate(BlockPrefab, blockPosition, rotation, circleGO.transform);
                Vector3 addedDistanceToDirection = rotation * newBlock.transform.right * distanceBetweenBlocks;
                newBlock.transform.position += addedDistanceToDirection;
                newBlock.GetComponentInChildren<BlockController>().ParentCircle = circle;
                newBlock.name += "_" + i + "_" + j;
                rotateDegrees += angle;
                circle.Blocks.Add(newBlock);
            }
            if (i % 2 != 0)
            {
                Quaternion rotation = Quaternion.AngleAxis(angle / 2, Vector3.up);
                circleGO.transform.rotation = rotation;
            }
            circleGO.transform.parent = towerGO.transform;
            circle.CircleGO = circleGO;
            circle.ParentTower = tower;
            circle.CircleIndex = i;
            circle.TotalNumberOfBlocks = gameSettings.NumberOfBlockPerCircle;
            circle.CurrentNumberOfBlocks = gameSettings.NumberOfBlockPerCircle;
            GameObject CBC = Instantiate(CircleBoxCollision, CircleBlockCollisonPosition, Quaternion.identity);
            CBC.GetComponent<BlockCollision>().ParentCircle = circle;
            circle.CircleCollision = CBC.GetComponent<BlockCollision>();
            tower.AddCircle(circle);
        }
        tower.TowerGO = towerGO;
        tower.TotalBlocks = gameSettings.TowerHeight * gameSettings.NumberOfBlockPerCircle;
        return tower;
    }
}

[System.Serializable]
public class Tower
{
    public List<Circle> Circles = new List<Circle>();
    public Circle HighestCircle;
    public GameObject TowerGO;
    public int TotalBlocks;

    public void AddCircle(Circle circle)
    {
        if (Circles.Count != 0) Circles[Circles.Count - 1].IsHighest = false;
        circle.IsHighest = true;
        Circles.Add(circle);
        circle.PopulateBlockControllersList();
        HighestCircle = circle;
    }

    public void FindHighestCircle(Circle circle)
    {
        int highestIndex = 0;
        foreach (var item in Circles)
        {
            item.IsHighest = false;
            if (!item.IsEmpty && item.CircleIndex > highestIndex)
            {
                HighestCircle = item;
                highestIndex = item.CircleIndex;
            }
        }
        HighestCircle.IsHighest = true;
        LevelController.Instance.CheckTowerHeightState(this);
    }

    public int GetCurrentNumberOfBlocks()
    {
        int currentNumber = 0;
        foreach (var item in Circles) currentNumber += item.CurrentNumberOfBlocks;
        return currentNumber;
    }
}

public class Circle
{
    public Tower ParentTower;
    public BlockCollision CircleCollision;
    public int CircleIndex;
    public List<GameObject> Blocks = new List<GameObject>();
    private List<BlockController> BlockControllers = new List<BlockController>();
    public GameObject CircleGO;
    public bool IsEmpty = false;
    public bool IsHighest = false;
    public int TotalNumberOfBlocks;
    public int CurrentNumberOfBlocks;


    public void RemoveBlock(BlockController controller)
    {
        if (!BlockControllers.Contains(controller)) return;
        CurrentNumberOfBlocks--;
        if (CurrentNumberOfBlocks <= 0)
        {
            IsEmpty = true;
            if (IsHighest) ParentTower.FindHighestCircle(this);
        }
        LevelController.Instance.GameProgressCheck(ParentTower);
        BlockControllers.Remove(controller);
    }

    public void PopulateBlockControllersList()
    {
        foreach (var item in Blocks)
        {
            BlockControllers.Add(item.GetComponentInChildren<BlockController>());
        }
    }

    public void ActivateBlocks()
    {
        ClearBlocksList();
        foreach (var item in Blocks) item.GetComponentInChildren<BlockController>().ActivateBlock();
    }

    public void DeactivateBlocks()
    {
        ClearBlocksList();
        foreach (var item in Blocks) item.GetComponentInChildren<BlockController>().DeactivateBlock();
    }

    private void ClearBlocksList()
    {
        Blocks = Blocks.Where(item => item != null).ToList();
    }

}
