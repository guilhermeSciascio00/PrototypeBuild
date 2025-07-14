using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private InputManager _inputManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PauseGameManager();
    }

    //Do whatever is needed to be done in the game while it's paused.
    private void PauseGameManager()
    {
        if (_inputManager.IsGamePaused)
        {
            Debug.LogWarning("Game is paused!! Stay where you are!");
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Game is unpaused!! Move!! move!!");
            Time.timeScale = 1f;
        }
    }
}
