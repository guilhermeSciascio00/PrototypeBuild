using UnityEngine;

public class BrokenState : StateCore
{

    private float spawnTime = 1f;
    private float spawnCountdown;

    private bool _shouldISpawn = false;

    public override void EnterState()
    {
        BrkableStateMachineRef.VisualPlatRef.SetActive(false);
        BrkableStateMachineRef.fallingParticleSyS.gameObject.SetActive(false);
        spawnCountdown = spawnTime;
    }

    public override void UpdateState()
    {
        ResetPlatform();
    }

    public override void PhysicsUpdateState()
    {
       
    }

    public override void ExitState()
    {

    }

    private void ResetPlatform()
    {
        if(IsSpawnAreaAvailable() && IsRespawnTimeOver())
        {
            BrkableStateMachineRef.SwitchState(BrkableStateMachineRef.ActiveState);
        }
    }

    private bool IsRespawnTimeOver()
    {
        spawnCountdown -= Time.deltaTime;
        if(spawnCountdown < 0f)
        {
            spawnCountdown = 0f;
            return true;
        }
        return false;
    }

    public bool IsAreaSpawnable() => _shouldISpawn;

    private bool IsSpawnAreaAvailable()
    {
        Collider2D spawnArea = Physics2D.OverlapCircle(BrkableStateMachineRef.RootObjectTransform.position, BrkableStateMachineRef.GetCircleRadius(), LayerMask.GetMask("Player", "Objects", "Geyser"));

        _shouldISpawn = spawnArea;
        return !spawnArea;
    }

}
