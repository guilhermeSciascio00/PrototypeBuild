using UnityEngine;

public class GeyserParticles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ParticleSystem geyserParticles;
    [SerializeField, Range(0f, 3f)] float particleYAdjustment;

    GeyserV2 geyserRef;

    private void Start()
    {
        geyserRef = GetComponent<GeyserV2>();
        UpdateParticlesHeight();
    }

    private void Update()
    {
        UpdateParticlesHeight();
        ParticleSwitch();
    }

    private void ParticleSwitch()
    {
        if (geyserRef.GetGeyserStatus() && geyserRef.GetGeyserSORef().needsThePlayer)
        {
            geyserParticles.Play();
        }
        else if(geyserRef.GetAutoGeyserStatus() && !geyserRef.GetGeyserSORef().needsThePlayer)
        {
            geyserParticles.Play();
        }
        else if (geyserRef.GetGeyserSORef().isItPermanent)
        {
            geyserParticles.Play();
        }
        else
        {
            geyserParticles.Stop();
        }
        
    }

    private void UpdateParticlesHeight()
    {
        //fix here
        geyserParticles.startSpeed = (geyserRef.GetGeyserSORef().geyserHeightPlayer * .5f) + particleYAdjustment;
    }

}
