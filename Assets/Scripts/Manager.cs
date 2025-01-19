using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class Manager : MonoBehaviour
{
    public float m_timeBetweenPhotos = 0.5f;    // wait a little between pictures

    enum PermissionStatus
    {
        UNKNOWN,
        YES,
        NO
    }
    PermissionStatus m_hasGPS = PermissionStatus.UNKNOWN;
    PermissionStatus m_hasCamera = PermissionStatus.UNKNOWN;

    bool m_cameraDelay = true;  // the user can't take photos till we're ready
    bool m_isTakingPhoto = false;
    AudioSource m_cameraSound;
    List<GameObject> m_hideForPhoto = new List<GameObject>();

    void Start()
    {
        m_cameraSound = GetComponent<AudioSource>();
        StartCoroutine(CheckPermissions());    
    }

    public void OnCameraClicked()
    {
        if (m_cameraDelay)
            return;
        m_cameraDelay = true;
        m_cameraSound.Play();
        StartCoroutine(TakeScreenShot());
    }

    public bool IsTakingPhoto()
    {
        return m_isTakingPhoto;
    }

    public void AddHideForPhoto(GameObject obj)
    {
        if (m_hideForPhoto.Contains(obj))
            return;
        m_hideForPhoto.Add(obj);
    }

    public void RemoveHideForPhoto(GameObject obj)
    {
        if (m_hideForPhoto.Contains(obj))
            m_hideForPhoto.Remove(obj);
    }

    public void OnFilesClicked()
    {
        PickImage(256);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
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

        m_cameraDelay = false;  // we're ready to take pictures
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

        m_isTakingPhoto = true;
        foreach (GameObject obj in m_hideForPhoto)
            obj.SetActive(false);
        yield return new WaitForEndOfFrame();

        // grab a screenshot
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        // Encode the texture in JPG format
        byte[] bytes = ImageConversion.EncodeToJPG(tex);

        // Save the screenshot to Gallery/Photos
        DateTime now = DateTime.Now;
        string imageName = "Image" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "Angilator", imageName,
            (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);
        
        // cleanup
        Destroy(tex);

        m_isTakingPhoto = false;
        foreach (GameObject obj in m_hideForPhoto)
            obj.SetActive(true);

        // wait before next photo
        yield return new WaitForSeconds(m_timeBetweenPhotos);
        m_cameraDelay = false;
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
