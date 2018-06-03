using System;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class RPG_Camera : MonoBehaviour {

    public static RPG_Camera instance;

    public Transform cameraPivot;
    public CameraMode cameraMode = CameraMode.Standard;
    public bool Initialised;
    public bool AllowZooming;
    private IRPGController rpgController;

    private float topDownHeight
    {
        get { return Rm_RPGHandler.Instance.Customise.TopDownHeight; }
    }

    private float topDownDistance
    {
        get { return -1 * Rm_RPGHandler.Instance.Customise.TopDownDistance; }
    }

    private Vector3 topDownOffset
    {
        get { return new Vector3(Rm_RPGHandler.Instance.Customise.CameraXOffset, Rm_RPGHandler.Instance.Customise.CameraYOffset, Rm_RPGHandler.Instance.Customise.CameraZOffset); }
    }

    public float sensitivityY = 15F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationY = 0F;

    public bool EnableAntiClip = true;

    public bool canControlCamera = true;
    public bool TempDisableOrbiting = false;
    public bool TempDisableCameraRotation = false;
    public float distance = 5f;
    public float distanceMax = 30f;
    public float mouseLookSpeed = 8f;
    public float gamepadLookSpeed = 1f;
    public float mouseScroll = 15f;
    public float mouseSmoothingFactor = 0.08f;
    public float camDistanceSpeed = 0.7f;
    public float camBottomDistance = 1f;
    public float firstPersonThreshold = 0.8f;
    public float characterFadeThreshold = 1.8f;

    public Vector3 desiredPosition;
    private float desiredDistance;
    private float lastDistance;
    public float mouseX = 0f;
    public float mouseXSmooth = 0f;
    public float mouseXVel;
    public float mouseY = 0f;
    public float mouseYSmooth = 0f;
    public float mouseYVel;
    private float mouseYMin = -89.5f;
    private float mouseYMax = 89.5f;
    private float distanceVel;
    private bool camBottom;
    private bool constraint;
    
    private static float halfFieldOfView;
    private static float planeAspect;
    private static float halfPlaneHeight;
    private static float halfPlaneWidth;

    public Vector3 weaponPos;
    public Vector3 weaponRot;


    public static Rmh_Customise Customise
    {
        get { return Rm_RPGHandler.Instance.Customise; }
    }

    public bool GetClickToRotateButton
    {
        get
        {
            switch (Rm_RPGHandler.Instance.Customise.ClickToRotateOption)
            {
                case ClickOption.Left:
                    return Input.GetMouseButton(0);
                case ClickOption.Right:
                    return Input.GetMouseButton(1);
                case ClickOption.Middle:
                    return Input.GetMouseButton(2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool GetOrbitPlayerButton
    {
        get
        {
            switch (Rm_RPGHandler.Instance.Customise.OrbitPlayerOption)
            {
                case ClickOption.Left:
                    return Input.GetMouseButton(0);
                case ClickOption.Right:
                    return Input.GetMouseButton(1);
                case ClickOption.Middle:
                    return Input.GetMouseButton(2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    void Awake() {
        instance = this;
    }

    void Start()
    {
        cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;
        transform.rotation = GetObject.PlayerMono.transform.rotation;
        distance = Mathf.Clamp(distance, 0.05f, distanceMax);
        desiredDistance = distance;

        halfFieldOfView = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        planeAspect = Camera.main.aspect;
        halfPlaneHeight = Camera.main.nearClipPlane * Mathf.Tan(halfFieldOfView);
        halfPlaneWidth = halfPlaneHeight * planeAspect;

        mouseX = 0f;
        mouseY = 15f;        
    }

    public void Init()
    {
        rpgController = GetObject.PlayerController;
        cameraPivot = GetObject.PlayerMonoGameObject.transform.Find("cameraPivot");


        Initialised = true;
    }

    void Update()
    {
        cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;
    }

    void FixedUpdate()
    {
        if (!canControlCamera || !Initialised) return;
        if (GameMaster.GamePaused || GameMaster.CutsceneActive || cameraMode == CameraMode.Manual) return;

        var OnMobile = false;

#if (UNITY_IOS || UNITY_ANDROID)
        OnMobile = true;
#endif

        if(cameraMode == CameraMode.Standard)
        {
            if (!UIHandler.MouseOnUI || OnMobile)
            {
                GetInput();
                GetDistanceInput();
            }
           
            //rpgController.clickToMove = false;
            GetDesiredPosition();    
        }
        else if(cameraMode == CameraMode.TopDown)
        {
            if (!UIHandler.MouseOnUI || OnMobile)
            {
                GetDistanceInput();
            }
            GetDesiredTopDownPosition();
        }
        else if(cameraMode == CameraMode.FirstPerson)
        {
            GetFPSPosition();
            if (!UIHandler.MouseOnUI || OnMobile)
            {
                GetFPSInput();
            }
            ModelAsChildForFps();
        }

        if(cameraMode != CameraMode.Manual)
        {
            PositionUpdate();

            CharacterFade(); 
        }

        if (OnMobile)
        {
            //GetObject.PlayerMonoGameObject.transform.eulerAngles = new Vector3(0, GetObject.RPGCamera.transform.eulerAngles.y, 0);
        }
    }

    public void RotateWithCharacter(float val)
    {
        mouseX += val;
    }

    private void ModelAsChildForFps()
    {
        var playerController = GetObject.PlayerMono.Controller;
        {
            var model = playerController.CharacterModel.transform;
            model.parent = transform;
            model.localPosition = weaponPos;
            model.localEulerAngles = weaponRot;
        }
    }

    private void GetFPSPosition()
    {
        //desiredPosition = Vector3.Lerp(this.transform.position,cameraPivot.position, 50 * Time.deltaTime);
        desiredPosition = cameraPivot.position;
    }

    private void GetFPSInput()
    {
        if (rpgController != null)
        {
            sensitivityY = rpgController.MouseSensitivity;
        }
        //todo: this
        sensitivityY = cameraPivot.parent.GetComponent<RPGController>().MouseSensitivity;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        //rpgAnimation.AnimationModel.transform.localEulerAngles = new Vector3(-rotationY, rpgAnimation.AnimationModel.transform.localEulerAngles.y, 0);
    }

    void GetInput() {

        if (distance > 0.1) { // distance > 0.05 would be too close, so 0.1 is fine
            Debug.DrawLine(transform.position, transform.position - Vector3.up * camBottomDistance, Color.green);
            camBottom = Physics.Linecast(transform.position, transform.position - Vector3.up * camBottomDistance);
        }

        bool constrainMouseY = camBottom && transform.position.y - cameraPivot.transform.position.y <= 0;


        var canOrbit = true;

#if (UNITY_IOS || UNITY_ANDROID)
        canOrbit = false;
#endif

        if (Customise.EnableOrbitPlayer && canOrbit && GetOrbitPlayerButton && !TempDisableOrbiting)
        { //|| Input.GetMouseButton(1)
            mouseX += Input.GetAxis("Mouse X") * mouseLookSpeed;

            if (constrainMouseY) {
                if (Input.GetAxis("Mouse Y") < 0)
                    mouseY -= Input.GetAxis("Mouse Y") * mouseLookSpeed;
            } else
                mouseY -= Input.GetAxis("Mouse Y") * mouseLookSpeed;

            mouseY = Mathf.Clamp(mouseY, minimumY, maximumY);

        }
        else if (RPG.Input.GetKey(RPG.Input.RotateCamUp) || RPG.Input.GetKey(RPG.Input.RotateCamDown) || RPG.Input.GetKey(RPG.Input.RotateCamLeft) || RPG.Input.GetKey(RPG.Input.RotateCamRight))
        {
            if(!GetObject.PlayerController.MovingForward)
                mouseX -= RPG.Input.GetAxis(RPG.Input.CameraHorizontalAxis) * gamepadLookSpeed;

            if (constrainMouseY)
            {
                if (RPG.Input.GetKey(RPG.Input.RotateCamDown))
                    mouseY -= RPG.Input.GetAxis(RPG.Input.CameraVerticalAxis) * 0.5f * gamepadLookSpeed;
            }
            else
                mouseY -= RPG.Input.GetAxis(RPG.Input.CameraVerticalAxis) * 0.5f * gamepadLookSpeed;

            mouseY = Mathf.Clamp(mouseY, minimumY, maximumY);
        }
        else
        {
            mouseY = ClampAngle(mouseY, -89.5f, 89.5f);
        }
        
        


        mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, mouseSmoothingFactor);
        mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, mouseSmoothingFactor);

        if (constrainMouseY)
            mouseYMin = mouseY;
        else
            mouseYMin = -89.5f;
        
        mouseYSmooth = ClampAngle(mouseYSmooth, mouseYMin, mouseYMax);  

    #if (!UNITY_IOS && !UNITY_ANDROID)

        if (Customise.EnableClickToRotate && GetClickToRotateButton && !TempDisableCameraRotation
            && ((GetObject.PlayerController.CurrentAction == null) || GetObject.PlayerController.CurrentAction != null && !GetObject.PlayerController.CurrentAction.FaceQueueTarget))
        {
            var rotation = Vector3.zero;
            var rotateRight = Input.GetAxis("Mouse X") * mouseLookSpeed;
            rotation.y = rotateRight * 0.4f * 200;
            if(!GetObject.PlayerCharacter.Stunned)
            {
                GetObject.PlayerMonoGameObject.transform.Rotate(rotation * Time.deltaTime);    
            }
        }
#endif
    }

    void GetDistanceInput()
    {
        if(AllowZooming)
            desiredDistance = desiredDistance - Input.GetAxis("Mouse ScrollWheel") * mouseScroll;

        if (desiredDistance > distanceMax)
            desiredDistance = distanceMax;

        if (desiredDistance < 0.05)
            desiredDistance = 0.05f;
    }

    void GetDesiredPosition() {
        distance = desiredDistance;
        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);

        float closestDistance;
        constraint = false;

        if (EnableAntiClip)
        {
            closestDistance = CheckCameraClipPlane(cameraPivot.position, desiredPosition);

            if (closestDistance != -1)
            {
                distance = closestDistance;
                desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);

                constraint = true;
            }


            distance -= Camera.main.nearClipPlane;
        }

        if (lastDistance < distance || !constraint)
            distance = Mathf.SmoothDamp(lastDistance, distance, ref distanceVel, camDistanceSpeed);
        
        if (distance < 0.05)
            distance = 0.05f;

        lastDistance = distance;

        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance); // if the camera view was blocked, then this is the new "forced" position
    }

    void GetDesiredTopDownPosition()
    {
        transform.position = cameraPivot.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + topDownHeight, transform.position.z - topDownDistance);
        transform.LookAt(cameraPivot.position);

        transform.position += topDownOffset;
    }

    void PositionUpdate() {
        if (cameraMode == CameraMode.Standard || cameraMode == CameraMode.FirstPerson)
        {
            if(Vector3.Distance(transform.position, desiredPosition) < 50)
            {
                transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 20 * Time.deltaTime);
            }
            else
            {
                transform.position = desiredPosition;
            }
        }
        if (distance > 0.05 && cameraMode != CameraMode.FirstPerson && cameraMode != CameraMode.Manual)
            transform.LookAt(cameraPivot);
    }

    #region "CharacterFade / Clipping"
    void CharacterFade() {
        //if (RPGAnimation == null)
        //    return;
        //
        //EnumerateFades(RPG_Animation.instance.AnimationModel);    
    }

    void EnumerateFades (Transform aTransform)
    {
        for (int i = 0; i < aTransform.childCount; i++)
        {
            var child = aTransform.GetChild(i);
            if(child.childCount > 0)
            {
                EnumerateFades(child);
            }
            else
            {
                var matRenderer = child.GetComponent<Renderer>();
                if(matRenderer != null)
                {
                    if (distance < firstPersonThreshold)
                        matRenderer.enabled = false;

                    else if (distance < characterFadeThreshold)
                    {
                        matRenderer.enabled = true;

                        float characterAlpha = 1 - (characterFadeThreshold - distance) / (characterFadeThreshold - firstPersonThreshold);
                        if (matRenderer.material.color.a != characterAlpha)
                            matRenderer.material.color = new Color(matRenderer.material.color.r, matRenderer.material.color.g, matRenderer.material.color.b, characterAlpha);

                    }
                    else
                    {

                        matRenderer.enabled = true;

                        if (matRenderer.material.color.a != 1)
                            matRenderer.material.color = new Color(matRenderer.material.color.r, matRenderer.material.color.g, matRenderer.material.color.b, 1);
                    }


                    if(cameraMode != CameraMode.Standard)
                    {
                        matRenderer.enabled = true;

                        if (matRenderer.material.color.a != 1)
                            matRenderer.material.color = new Color(matRenderer.material.color.r, matRenderer.material.color.g, matRenderer.material.color.b, 1);
                    }
                }
            }
        }
    }

    Vector3 GetCameraPosition(float xAxis, float yAxis, float distance) {
        Vector3 offset = new Vector3(0, 0, -distance);

        if(GetObject.PlayerController.Target != null && Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget
            && Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock)
        {
            offset.x = 5;
        }

        Quaternion rotation = Quaternion.Euler(xAxis, yAxis, 0);
        return cameraPivot.position + rotation * offset;
    }

    float CheckCameraClipPlane(Vector3 from, Vector3 to) {
        var closestDistance = -1f;
                  
        RaycastHit hitInfo;

        ClipPlaneVertexes clipPlane = GetClipPlaneAt(to);

        Debug.DrawLine(clipPlane.UpperLeft, clipPlane.UpperRight);
        Debug.DrawLine(clipPlane.UpperRight, clipPlane.LowerRight);
        Debug.DrawLine(clipPlane.LowerRight, clipPlane.LowerLeft);
        Debug.DrawLine(clipPlane.LowerLeft, clipPlane.UpperLeft);
     
        Debug.DrawLine(from, to, Color.red);
        Debug.DrawLine(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperRight, Color.cyan);
        Debug.DrawLine(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerRight, Color.cyan);


        if (Physics.Linecast(from, to, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "LootItem")
            closestDistance = hitInfo.distance - Camera.main.nearClipPlane;

        if (Physics.Linecast(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "LootItem")
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance = Vector3.Distance(hitInfo.point + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "LootItem")
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance = Vector3.Distance(hitInfo.point - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "LootItem")
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance = Vector3.Distance(hitInfo.point + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "LootItem")
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance = Vector3.Distance(hitInfo.point - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);


        return closestDistance;
    }

    float ClampAngle(float angle, float min, float max) {
        while (angle < -360 || angle > 360) {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }

    public struct ClipPlaneVertexes {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static ClipPlaneVertexes GetClipPlaneAt(Vector3 pos) {
        var clipPlane = new ClipPlaneVertexes();

        if (Camera.main == null)
            return clipPlane;

        Transform transform = Camera.main.transform;
        float offset = Camera.main.nearClipPlane;

        clipPlane.UpperLeft = pos - transform.right * halfPlaneWidth;
        clipPlane.UpperLeft += transform.up * halfPlaneHeight;
        clipPlane.UpperLeft += transform.forward * offset;

        clipPlane.UpperRight = pos + transform.right * halfPlaneWidth;
        clipPlane.UpperRight += transform.up * halfPlaneHeight;
        clipPlane.UpperRight += transform.forward * offset;

        clipPlane.LowerLeft = pos - transform.right * halfPlaneWidth;
        clipPlane.LowerLeft -= transform.up * halfPlaneHeight;
        clipPlane.LowerLeft += transform.forward * offset;

        clipPlane.LowerRight = pos + transform.right * halfPlaneWidth;
        clipPlane.LowerRight -= transform.up * halfPlaneHeight;
        clipPlane.LowerRight += transform.forward * offset;

        
        return clipPlane;
    }
    #endregion

    public void SwitchToCharacter(Transform charTransform)
    {
        var cameraPiv = charTransform.Find("cameraPivot");
        
        //todo: create cameraPivot with gameObject
        if (cameraPiv != null)
        {
            cameraPivot = cameraPiv;
        }
        else
        {
            var go = new GameObject("cameraPivot");
            go.transform.position = charTransform.position + (charTransform.up / 2);
            go.transform.parent = charTransform;
            cameraPivot = go.transform;
        }
    }

    public void ResetCanera()
    {
        mouseX = GetObject.PlayerMonoGameObject.transform.eulerAngles.y;
        mouseXSmooth = mouseX;
        mouseY = 5;
        mouseYSmooth = 5;
    }
}

public enum CameraMode
{
    Standard,
    TopDown,
    FirstPerson,
    Manual
}

public enum ClickOption
{
    Left,
    Right,
    Middle
}