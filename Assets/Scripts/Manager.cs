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
    public GameObject m_cameraScreen;
    public GameObject m_calibrateScreen;
    public GameObject m_calibrateMsg00;
    public GameObject m_calibrateMsg01;
    public GameObject m_calibrateMsg02;
    public CameraFeed m_cameraFeed;

    enum CalibrationStage
    {
        NONE,
        FIRST,
        UPSIDE_DOWN,
        SECOND,
        DONE
    }
    CalibrationStage m_calibrationStage = CalibrationStage.NONE;

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

    static Manager s_theManager = null;

    void Start()
    {
        s_theManager = this;
        m_cameraSound = GetComponent<AudioSource>();
        Input.gyro.enabled = true;
        StartCoroutine(CheckPermissions());    
    }

    public static Manager Get()
    {
        return s_theManager;
    }

    public void OnCameraClicked()
    {
        if (m_cameraDelay)
            return;
        m_cameraDelay = true;
        m_cameraSound.Play();
        StartCoroutine(TakeScreenShot());
    }

    public void OnCalibrationClicked()
    {
        switch (m_calibrationStage)
        {
            case CalibrationStage.NONE:
                StartCoroutine(DoCalibration());
                break;
            case CalibrationStage.FIRST:
                m_cameraSound.Play();
                m_calibrationStage = CalibrationStage.UPSIDE_DOWN;
                break;
            case CalibrationStage.SECOND:
                m_cameraSound.Play();
                m_calibrationStage = CalibrationStage.DONE;
                break;
        }
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

    public void OnQuitClicked()
    {
        Debug.Log("OnQuitClicked");
        Application.Quit();
    }

    public float GetTilt()
    {
        Vector3 g = Input.gyro.gravity;
        float len = g.magnitude;
        if (len > 0.01f)
        {
            float ang = Mathf.Rad2Deg * Mathf.Asin(g.x / len);
            return ang;
        }
        return 0.0f;
    }

    float GetElevation_Raw()
    {
        Vector3 g = Input.gyro.gravity;
        float len = g.magnitude;
        if (len > 0.01f)
        {
            float ang = Mathf.Rad2Deg * Mathf.Asin(g.z / len);
            return ang;
        }
        return 0.0f;
    }

    public float GetElevation()
    {
        float ang = GetElevation_Raw();
        bool isCalibrated = PlayerPrefs.GetInt("IsCalibrated", 0) != 0;
        if (isCalibrated)
        {
            float calAng = PlayerPrefs.GetFloat("ElvCalibrate", 0.0f);
            if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                ang += calAng;
            else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                ang -= calAng;
            if (ang > 180.0f)
                ang -= 360.0f;
            if (ang < -180.0f)
                ang += 360.0f;
        }
        return ang;
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
                Debug.Log("There was a problem getting your GPS location");
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

        bool isCalibrated = PlayerPrefs.GetInt("IsCalibrated", 0) != 0;
        if (isCalibrated)
        {
            m_cameraScreen.SetActive(true);
            m_calibrateScreen.SetActive(false);
        }
        else
        {
            DialogBox.ShowDialog("Let's start by calibrating your tilt sensor",
                null);
            while (DialogBox.IsOpen())
                yield return null;
            OnCalibrationClicked();
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

        m_isTakingPhoto = true;
        foreach (GameObject obj in m_hideForPhoto)
            obj.SetActive(false);
        yield return new WaitForEndOfFrame();
        
        DateTime now = DateTime.Now;
        var camTex = m_cameraFeed.GetCamTex();
        {   // grab a screenshot
            int superSize = 1;
            if (camTex != null)
            {
                float superSizeW = (float)camTex.width / Screen.width;
                float superSizeH = (float)camTex.height / Screen.height;
                superSize = (int)(Mathf.Min(superSizeW, superSizeH) + 0.5f);
                superSize = Mathf.Max(superSize, 1);
            }
            var tex = ScreenCapture.CaptureScreenshotAsTexture(superSize);

            // Encode the texture in JPG format
            byte[] bytes = ImageConversion.EncodeToJPG(tex);

            // Save the screenshot to Gallery/Photos
            string imageName = "Image" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "Angilator", imageName,
                (success, path) => Debug.Log("Media save result: " + success + " " + path));

            // cleanup
            Destroy(tex);
        }
        {   // save the raw camera feed
            if (camTex)
            {
                Texture2D tex = new Texture2D(camTex.width, camTex.height);
                tex.SetPixels(camTex.GetPixels());
                byte[] bytes = ImageConversion.EncodeToJPG(tex);

                // Save the screenshot to Gallery/Photos
                string imageName = "Raw" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
                NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "Angilator", imageName,
                    (success, path) => Debug.Log("Media save result: " + success + " " + path));
                // cleanup
                Destroy(tex);
            }
        }

        m_isTakingPhoto = false;
        foreach (GameObject obj in m_hideForPhoto)
            obj.SetActive(true);

        // wait before next photo
        yield return new WaitForSeconds(m_timeBetweenPhotos);
        m_cameraDelay = false;
    }

    IEnumerator DoCalibration()
    {
        m_calibrationStage = CalibrationStage.FIRST;
        m_cameraScreen.SetActive(false);
        m_calibrateScreen.SetActive(true);
        m_calibrateMsg02.SetActive(false);

        // make sure the phone is upright
        m_calibrateMsg00.SetActive(true);
        m_calibrateMsg01.SetActive(false);
        while (Input.deviceOrientation != DeviceOrientation.LandscapeLeft)
            yield return null;
        m_calibrateMsg00.SetActive(false);
        m_calibrateMsg01.SetActive(true);

        // wait for the user to click the button
        while (m_calibrationStage == CalibrationStage.FIRST)
            yield return null;

        float elv1 = GetElevation_Raw();

        // tell user to turn phone upside down
        m_calibrateMsg01.SetActive(false);
        m_calibrateMsg02.SetActive(true);
        while (Input.deviceOrientation != DeviceOrientation.LandscapeRight)
            yield return null;

        // wait for the user to click the button
        m_calibrationStage = CalibrationStage.SECOND;
        while (m_calibrationStage == CalibrationStage.SECOND)
            yield return null;

        // do the calibration
        float elv2 = GetElevation_Raw();
        float calibrate = 0.5f * (elv1 - elv2);
        PlayerPrefs.SetFloat("ElvCalibrate", calibrate);

        // finished calibration
        DateTime now = DateTime.Now;
        PlayerPrefs.SetInt("IsCalibrated", 1);
        PlayerPrefs.SetString("CalibrationDate", now.ToShortDateString());
        m_cameraScreen.SetActive(true);
        m_calibrateScreen.SetActive(false);
        m_calibrationStage = CalibrationStage.NONE;
    }
}
