using UnityEngine;

[CreateAssetMenu(fileName = "GeyserStatsSO", menuName = "Scriptable Objects/GeyserStatsSO")]
public class GeyserStatsSO : ScriptableObject
{

    public float geyserSpeedUP;
    public float geyserSpeedDown;
    public float geyserDelay;
    public float geyserHeightPlayer;
    public float geyserHeightBox;

    public float geyserUPForce; 
    public float geyserDownForce; 

    public bool needsThePlayer;
    public bool isItPermanent;
}
