using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerInput pi;
    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 100.0f;
    public float cameraDamp = 1f;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private float tempEulerX;

    private GameObject camera;
    private Vector3 cameraDampVelocity;
    
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        model = playerHandle.GetComponent<ActorController>().model;
        tempEulerX = 20f;
        camera = Camera.main.gameObject;
    }

    private void FixedUpdate()  // 放 Update / LateUpdate 都会发生抖动
    {
        Vector3 tempModelEuler = model.transform.eulerAngles;
        
        playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.deltaTime);
        
        tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
        cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0f, 0f);

        model.transform.eulerAngles = tempModelEuler;
        
        // camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.02f);  // Vector3.Lerp
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, cameraDamp);  // Vector3.SmoothDamp
        camera.transform.eulerAngles = transform.eulerAngles;
    }
}
