using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 100.0f;
    public float cameraDamp = 1f;
 
    private IUserInput pi;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private float tempEulerX;

    private GameObject myCamera;
    private Vector3 cameraDampVelocity;
    [SerializeField]
    private GameObject lockTarget;
    
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        var ac = playerHandle.GetComponent<ActorController>();
        pi = ac.pi;
        model = ac.model;
        tempEulerX = 20f;
        myCamera = Camera.main.gameObject;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()  // 放 Update / LateUpdate 都会发生抖动
    {
        if (lockTarget == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;
        
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.deltaTime);
        
            tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0f, 0f);

            model.transform.eulerAngles = tempModelEuler;   
        }
        else
        {
            Vector3 tempForward = lockTarget.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward; 
        }

        // camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.02f);  // Vector3.Lerp
        myCamera.transform.position = Vector3.SmoothDamp(myCamera.transform.position, transform.position, ref cameraDampVelocity, cameraDamp);  // Vector3.SmoothDamp
        
        // myCamera.transform.eulerAngles = transform.eulerAngles;
        myCamera.transform.rotation = Quaternion.Slerp(myCamera.transform.rotation, transform.rotation, 0.3f);
        // myCamera.transform.LookAt(cameraHandle.transform);
    }

    public void LockUnlock()
    {
        Vector3 modelOrigin1 = model.transform.position;
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0);
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask("Enemy"));
        if (cols.Length == 0)
        {
            lockTarget = null;
        }
        else
        {
            foreach (var col in cols)
            {
                if (lockTarget == col.gameObject)
                {
                    lockTarget = null;
                    break;
                }
                lockTarget = col.gameObject;
                break;
            }
        }
    }
}