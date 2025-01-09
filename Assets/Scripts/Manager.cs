using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void OnCameraClicked()
    {
        StartCoroutine(TakeScreenShot());
    }

    public void OnFilesClicked()
    {
        PickImage(256);
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
