using UnityEngine;

public class RebindManager : MonoBehaviour
{
    [SerializeField] GameObject _rebindCanvasRef;

    [SerializeField] InputManager _inputManager;

    private void Start()
    {
        _rebindCanvasRef.SetActive(false);
    }

    private void Update()
    {
        MenuOpenAndClose();
    }

    private void MenuOpenAndClose()
    {
        if (_inputManager.IsGamePaused)
        {
            _rebindCanvasRef.SetActive(true);
        }
        else
        {
            _rebindCanvasRef.SetActive(false);
        }
    }
}
