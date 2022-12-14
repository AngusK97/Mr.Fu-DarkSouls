using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 100.0f;
    public float cameraDamp = 1f;
    public Image lockDot;
    public bool lockState;
 
    private IUserInput pi;
    
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject model;
    private float tempEulerX;

    private GameObject myCamera;
    private Vector3 cameraDampVelocity;
    
    public GameObject targetObj;
    public float targetHalfHeight;
    
    void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        var ac = playerHandle.GetComponent<ActorController>();
        pi = ac.pi;
        model = ac.model;
        tempEulerX = 20f;
        myCamera = Camera.main.gameObject;
        
        targetObj = null;
        targetHalfHeight = 0f;
        
        Cursor.lockState = CursorLockMode.Locked;
        lockDot.enabled = false;
        lockState = false;
    }

    private void Update()
    {
        if (targetObj != null)
        {
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(
                targetObj.transform.position + new Vector3(0, targetHalfHeight, 0));

            if (Vector3.Distance(model.transform.position, targetObj.transform.position) > 10.0f)
                UnlockTarget();
        }
    }

    private void FixedUpdate()  // 放 Update / LateUpdate 都会发生抖动
    {
        if (targetObj == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;
            
            // camera horizontal move
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.deltaTime);
        
            // camera vertical move
            tempEulerX -= pi.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0f, 0f);
            
            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            Vector3 tempForward = targetObj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            transform.LookAt(targetObj.transform);
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
            UnlockTarget();
        }
        else
        {
            foreach (var col in cols)
            {
                if (targetObj == col.gameObject)
                {
                    UnlockTarget();
                    break;
                }

                LockTarget(col.gameObject, col.bounds.extents.y);
                break;
            }
        }
    }

    private void LockTarget(GameObject obj, float halfHeight)
    {
        targetObj = obj;
        targetHalfHeight = halfHeight;
        lockDot.enabled = true;
        lockState = true;
    }

    private void UnlockTarget()
    {
        targetObj = null;
        targetHalfHeight = 0f;
        lockDot.enabled = false;
        lockState = false;
    }
}
