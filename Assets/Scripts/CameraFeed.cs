using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class CameraFeed : MonoBehaviour
{
    WebCamTexture m_camTex;
    RawImage m_image;
    AspectRatioFitter m_fitter;

    readonly Vector3 s_defaultScale = new Vector3(1f, 1f, 1f);
    readonly Vector3 s_fixedScale = new Vector3(-1f, 1f, 1f);
    readonly Rect s_defaultRect = new Rect(0f, 0f, 1f, 1f);
    readonly Rect s_fixedRect = new Rect(0f, 1f, 1f, -1f);

    public WebCamTexture GetCamTex()
    {
        if (m_camTex && m_camTex.width > 100)
            return m_camTex;
        return null;
    }

#if false
#if UNITY_IOS || UNITY_WEBGL
    private bool CheckPermissionAndRaiseCallbackIfGranted(UserAuthorization authenticationType, Action authenticationGrantedAction)
    {
        if (Application.HasUserAuthorization(authenticationType))
        {
            if (authenticationGrantedAction != null)
                authenticationGrantedAction();

            return true;
        }
        return false;
    }

    private IEnumerator AskForPermissionIfRequired(UserAuthorization authenticationType, Action authenticationGrantedAction)
    {
        if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
        {
            yield return Application.RequestUserAuthorization(authenticationType);
            if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
                Debug.LogWarning($"Permission {authenticationType} Denied");
        }
    }
#elif UNITY_ANDROID
    private void PermissionCallbacksPermissionGranted(string permissionName)
    {
        StartCoroutine(DelayedCameraInitialization());
    }

    private IEnumerator DelayedCameraInitialization()
    {
        yield return null;
        InitializeCamera();
    }

    private void PermissionCallbacksPermissionDenied(string permissionName)
    {
        Debug.LogWarning($"Permission {permissionName} Denied");
    }

    private void AskCameraPermission()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }
#endif

    void Start()
    {
#if UNITY_IOS || UNITY_WEBGL
        StartCoroutine(AskForPermissionIfRequired(UserAuthorization.WebCam, () => { InitializeCamera(); }));
        return;
#elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            AskCameraPermission();
            return;
        }
#endif
        InitializeCamera();
    }
#endif

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        int bestWidth = -1;
        int bestHeight = -1;
        int bestDevice = -1;
        int index = 0;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log(device.name);
            if (device.isFrontFacing)
                continue;   // don't choose a front-facing camera
            if (device.availableResolutions != null)
            {
                foreach (var rez in device.availableResolutions)
                {
                    if (rez.width > bestWidth)
                    {   // choose the camera with the widest available resolution
                        bestWidth = rez.width;
                        bestHeight = rez.height;
                        bestDevice = index;
                    }
                }
            }
            index++;
        }

        if (bestDevice >= 0)
            m_camTex = new WebCamTexture(devices[bestDevice].name, bestWidth, bestHeight);
        else
            m_camTex = new WebCamTexture();
        m_image = GetComponent<RawImage>();
        m_image.texture = m_camTex;
        m_camTex.Play();
        m_fitter = GetComponent<AspectRatioFitter>();

        Debug.Log("InitializeCamera done");
    }

    private void Update()
    {
        if (false == Manager.CheckCameraPerission())
            return; // don't have permission

        if (m_camTex == null)
            InitializeCamera();
        if (m_camTex == null || m_camTex.width < 100)
        {
            Debug.Log("Camera Not Ready");
            return; // camera feed isn't ready yet
        }

        // Rotate image to show correct orientation 
        Vector3 rot = new Vector3(0.0f, 0.0f, -m_camTex.videoRotationAngle);
        m_image.rectTransform.localEulerAngles = rot;

        if (m_fitter != null)
        {
            // Set AspectRatioFitter's ratio
            float videoRatio =
                (float)m_camTex.width / (float)m_camTex.height;
            m_fitter.aspectRatio = videoRatio;
        }

        // Unflip if vertically flipped
        m_image.uvRect =
            m_camTex.videoVerticallyMirrored ? s_fixedRect : s_defaultRect;
    }
}