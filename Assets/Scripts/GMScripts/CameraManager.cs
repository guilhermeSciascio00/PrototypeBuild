using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Camera Attributes")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _cameraTarget;
    [SerializeField] Vector2 _offsetFromTarget;

    private enum PositionType
    {
        Start,
        Update
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCameraPos(PositionType.Start);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetCameraPos(PositionType.Update);
    }

    private Vector3 GetCameraTargetPos(PositionType posType)
    {
        switch (posType)
        {
            case PositionType.Start:
                return new Vector3(_cameraTarget.transform.position.x + _offsetFromTarget.x, _cameraTarget.transform.position.y + _offsetFromTarget.y, -10f);


            case PositionType.Update:
                return new Vector3(_cameraTarget.transform.position.x + _offsetFromTarget.x, _mainCamera.transform.position.y, -10f);
        }
        //Should not get here;
        return default;
        
    }

    private void SetCameraPos(PositionType cameraCurrentType)
    {
        _mainCamera.transform.position = GetCameraTargetPos(cameraCurrentType);
    }
}
