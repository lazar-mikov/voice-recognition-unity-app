using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    private bool camAvailable;
    private WebCamTexture cam;
    private Texture defaultBackground;

    public RawImage backround;
    public AspectRatioFitter fit;
    void Start()
    {
        defaultBackground = backround.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            Debug.Log("Blah");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if(devices[i].isFrontFacing)   {
                cam = new WebCamTexture(devices[i].name, Screen.width,Screen.height);
            }  
                }
        if(cam == null)
        {
            Debug.Log("blah 2");
            return;
        }
        cam.Play();
        backround.texture = cam;
        camAvailable = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)cam.width / (float)cam.height;
        fit.aspectRatio = ratio;

        float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
        backround.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -cam.videoRotationAngle;
        backround.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}
