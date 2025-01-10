using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class Manager : MonoBehaviour
{
    enum PermissionStatus
    {
        UNKNOWN,
        YES,
        NO
    }
    PermissionStatus m_hasGPS = PermissionStatus.UNKNOWN;
    PermissionStatus m_hasCamera = PermissionStatus.UNKNOWN;

    void Start()
    {
        StartCoroutine(CheckPermissions());    
    }

    public void OnCameraClicked()
    {
        StartCoroutine(TakeScreenShot());
    }

    public void OnFilesClicked()
    {
        PickImage(256);
    }

#if UNITY_IOS
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
#endif

    public static bool CheckCameraPerission()
    {
#if UNITY_IOS
        return Application.HasUserAuthorization(UserAuthorization.WebCam);
#elif UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
        return false;
#endif
    }

    IEnumerator CheckPermissions()
    {
        // check camera permission
        if (CheckCameraPerission())
        {
            m_hasCamera = PermissionStatus.YES;
        }
        else 
        {
            DialogBox.ShowDialog("This app requires camera access\nYour photos will not be uploaded or shared in any way",
                OnCameraOk);
            while (m_hasCamera == PermissionStatus.UNKNOWN)
                yield return null;
        }

        // check location permission
#if UNITY_ANDROID// && !UNITY_EDITOR
        Debug.Log("Android GPS permissions");
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            DialogBox.ShowDialog("Would you like to put your GPS coordinates on your images?\nThis app will never upload or share your location",
                OnGpsYes, OnGpsNo);
            while (m_hasGPS == PermissionStatus.UNKNOWN)
                yield return null;
        }
#endif

#if false   // I don't think we need this
        {   // wait up to 5 seconds for this to work
            float m_timeOut = 5.0f;
            while (m_timeOut > 0.0f && Input.location.isEnabledByUser == false)
            {
                m_timeOut -= Time.deltaTime;
                yield return null;
            }
        }
#endif

        Debug.Log("Location Enabled = " + Input.location.isEnabledByUser.ToString());
        if (Input.location.isEnabledByUser)
            m_hasGPS = PermissionStatus.YES;
        else
            m_hasGPS = PermissionStatus.NO;

        if (m_hasGPS == PermissionStatus.YES)
        {
            Input.location.Start();
            Debug.Log("Waiting for LocationServiceStatus");
            float m_timeOut = 15.0f;
            while (m_timeOut > 0.0f && Input.location.status == LocationServiceStatus.Initializing)
            {
                m_timeOut -= Time.deltaTime;
                yield return null;
            }
            if (Input.location.status != LocationServiceStatus.Running)
            {
                DialogBox.ShowDialog("There was a problem getting your GPS location", OnGpsNo);
                while (m_hasGPS != PermissionStatus.NO)
                    yield return null;
            }
            else
            {
                Debug.Log("LocationServiceStatus Running");
            }
        }
    }

    void OnCameraOk()
    {
        Debug.Log("OnCameraOk");

#if UNITY_IOS
        StartCoroutine(RequestCameraPermission());
#elif UNITY_ANDROID
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += CameraPermissionDenied;
        callbacks.PermissionGranted += CameraPermissionGranted;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
#else
        m_hasCamera = PermissionStatus.NO;
#endif
    }

#if UNITY_IOS
    IEnumerator RequestCameraPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (CheckCameraPerission())
        {
            m_hasCamera = PermissionStatus.YES;
        }
        else
        {
            m_hasCamera = PermissionStatus.NO;
        }
    }
#endif

#if UNITY_ANDROID
    void CameraPermissionDenied(string permission)
    {
        Debug.Log("CameraPermissionDenied");
        m_hasCamera = PermissionStatus.NO;
    }

    void CameraPermissionGranted(string permission)
    {
        Debug.Log("CameraPermissionGranted");
        m_hasCamera = PermissionStatus.YES;
    }

    void OnGpsYes()
    {
        Debug.Log("OnGpsYes");
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += GPSPermissionDenied;
        callbacks.PermissionGranted += GPSPermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += GPSPermissionDeniedAndDontAskAgain;
        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
    }

    void GPSPermissionDenied(string permission)
    {
        Debug.Log("GPSPermissionDenied");
        m_hasGPS = PermissionStatus.NO;
    }

    void GPSPermissionGranted(string permission)
    {
        Debug.Log("GPSPermissionGranted");
        m_hasGPS = PermissionStatus.YES;
    }

    void GPSPermissionDeniedAndDontAskAgain(string permission)
    {   // mrwTODO don't ask again!
        Debug.Log("GPSPermissionDeniedAndDontAskAgain");
        m_hasGPS = PermissionStatus.NO;
    }
#endif

    void OnGpsNo()
    {
        Debug.Log("OnGpsNo");
        m_hasGPS = PermissionStatus.NO;
    }

    IEnumerator TakeScreenShot()
    {
        // wait until the frame is ready
        yield return new WaitForEndOfFrame();
        
        // grab a screenshot
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        // Encode the texture in JPG format
        byte[] bytes = ImageConversion.EncodeToJPG(tex);

        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "GalleryTest", "Image.jpg",
            (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);
        
        // cleanup
        Destroy(tex);
    }

    private void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("PickImage: Image path: " + path);
        });

        Debug.Log("Permission result: " + permission);
    }
}
