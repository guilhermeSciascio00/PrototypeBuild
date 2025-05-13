using UnityEngine;

public class GeyserParticles : MonoBehaviour
{

    //TRY PAUSE AND CLEAR

    [SerializeField] ParticleSystem geyserWaterParticles;
    [SerializeField] ParticleSystem geyserQuakeParticles;
    [SerializeField, Range(0f, 3f)] float particleYAdjustment;

    Geyser geyserRef;


    private void Start()
    {
        geyserRef = GetComponent<Geyser>();
    }

    private void Update()
    {
        UpdateParticlesHeight();
        WaterParticleSwitch();
        GeyserQuakeParticles();

    }

    private void WaterParticleSwitch()
    {
        if (geyserRef.HasGeyserStarted())
        {
            GeyserParticleSyS(geyserWaterParticles, playIT: true);
        }

        if (!geyserRef.HasGeyserStarted() || geyserRef.IsObjectIn)
        {
            GeyserParticleSyS(geyserWaterParticles, playIT: false);
        }

    }
    private void GeyserQuakeParticles()
    {
        if (!geyserRef.HasGeyserStarted())
        {
            GeyserParticleSyS(geyserQuakeParticles, playIT: true);
        }

        if (geyserRef.HasGeyserStarted() || geyserRef.IsObjectIn)
        {
            GeyserParticleSyS(geyserQuakeParticles, playIT: false);
            
        }
    }

    private void GeyserParticleSyS(ParticleSystem particleSyS, bool playIT)
    {
        switch (playIT)
        {
            case true:
                if (!particleSyS.isPlaying)
                {
                    particleSyS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particleSyS.Play();
                }

                break;
            case false:
                if (particleSyS.isPlaying)
                {
                    particleSyS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                break;
        }

    }


private void UpdateParticlesHeight()
    {
        ParticleSystem.MainModule mainParticleSyS = geyserWaterParticles.main;

        mainParticleSyS.startSpeed = (geyserRef.GeyserSORef.geyserHeightPlayer * .5f) + particleYAdjustment;

    }

}
