using UnityEngine;

public class GeyserParticles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] ParticleSystem geyserWaterParticles;
    [SerializeField] ParticleSystem geyserQuakeParticles;
    [SerializeField, Range(0f, 3f)] float particleYAdjustment;

    GeyserV2 geyserRef;
    [SerializeField] bool isDebugging = false;

    private void Start()
    {
        geyserRef = GetComponent<GeyserV2>();
        UpdateParticlesHeight();
    }

    private void Update()
    {
        UpdateParticlesHeight();
        WaterParticleSwitch();
        GeyserQuakeParticles();        
    }

    private void WaterParticleSwitch()
    {
        if (!geyserRef.IsGeyserBlocked())
        {
            if (geyserRef.GetGeyserStatus() && geyserRef.GetGeyserSORef().needsThePlayer)
            {
                if (!geyserWaterParticles.isPlaying)
                {
                    geyserWaterParticles.Play();
                }
            }
            else if (geyserRef.GetAutoGeyserStatus())
            {
                if (!geyserWaterParticles.isPlaying)
                {
                    geyserWaterParticles.Play();
                }
            }
            else if (geyserRef.GetGeyserSORef().isItPermanent)
            {
                if (!geyserWaterParticles.isPlaying)
                {
                    geyserWaterParticles.Play();
                }
            }
            else
            {
                geyserWaterParticles.Stop();
            }
        }
        else
        {
            geyserWaterParticles.Stop();
        }

    }
    private void GeyserQuakeParticles()
    {
        if (!geyserRef.IsParticlesDelayOver())
        {
            geyserQuakeParticles.Play();
        }
        else
        {
            geyserQuakeParticles.Stop();
        }
    }

    private void UpdateParticlesHeight()
    {
        ParticleSystem.MainModule mainParticleSyS = geyserWaterParticles.main;

        mainParticleSyS.startSpeed = (geyserRef.GetGeyserSORef().geyserHeightPlayer * .5f) + particleYAdjustment;

    }

}
