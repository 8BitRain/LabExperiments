using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnBossLevelAsset : Action
{
    public Vector3 assetSpawnPosition;
    public SharedFloat spawnNextLevelAssetDistance;
    public override void OnStart()
    {
        GameObject generatedAsset = transform.GetComponent<LevelAssetGenerator>().GenerateBossLevelAsset();
        PositionSpawnedAsset(generatedAsset);
    }

    public void PositionSpawnedAsset(GameObject generatedAsset)
    {
        Vector3 assetSize = generatedAsset.GetComponent<BoxCollider>().size + generatedAsset.GetComponent<BoxCollider>().center;
        //We want generated assets to be spawned behind assets that exist. Specifically in a straight line (This will need to be adjusted for different directions)
        generatedAsset.transform.position = assetSpawnPosition + new Vector3(assetSize.x, 0,0);
        generatedAsset.transform.SetParent(this.transform);

        //Level is now twice as long, so lets increase the distance until we spawn another piece
        //TODO this could be made more reobust by adding the bounding box size. That way, distance increases for the size we need
        spawnNextLevelAssetDistance.Value = assetSize.x;
    }
    public override TaskStatus OnUpdate()
    {

        return TaskStatus.Success;
    }
}