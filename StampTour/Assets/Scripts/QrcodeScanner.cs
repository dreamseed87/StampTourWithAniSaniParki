using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;
using System;

public class QRcodeScanner : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageBackground;

    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;

    [SerializeField]
    private TextMeshProUGUI _textOut;

    [SerializeField]
    private RectTransform _scanZone;

    private bool _isCamAvaible;
    private WebCamTexture _cameraTexture;

    private bool _isCamPlaying;
    private int selectedCameraIndex;

    static Result result;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetUpCamera());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }
 
    public void SwitchCamera()
    {
        if (!_isCamAvaible)
        {
            Debug.Log("Camera not available");
            return;
        }

        _isCamPlaying = _cameraTexture.isPlaying;
        Debug.Log("Camera is playing: " + _isCamPlaying);

        if (!_isCamPlaying)
        {
            CameraOn();
        }
        else
        {
            CameraOff();
        }
    }
    private void UpdateCameraRender()
    {
        if(_isCamAvaible == false || _cameraTexture == null)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_cameraTexture.videoRotationAngle;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    
    private IEnumerator SetUpCamera()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
        }

        WebCamDevice[] devices = null;

        do
        {
            devices = WebCamTexture.devices;
            yield return null;
        }
        while (devices.Length != WebCamTexture.devices.Length);




        if(devices.Length == 0)
        {
            _isCamAvaible = false;
            yield break;
        }

        selectedCameraIndex = -1;

        for (int i =0; i<devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            if(devices[i].isFrontFacing == false)
            {
                selectedCameraIndex = i;
                break;
            }
        }

        if (selectedCameraIndex >= 0)
        {
            Debug.Log("Camera Catch");
            _cameraTexture = new WebCamTexture(devices[selectedCameraIndex].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            _isCamAvaible = _cameraTexture ? true : false;
        }
        else
        {
            Debug.Log("Camera Miss");
            _isCamAvaible = false;
            yield break;
        }
    }
    
    private void CameraOn()
    {
        _cameraTexture.Play();
        _rawImageBackground.texture = _cameraTexture;
    }
    private void CameraOff()
    {
        _cameraTexture.Stop();
    }
    

    public void OnclickScan()
    {
        Scan();
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            result = barcodeReader.Decode(_cameraTexture.GetPixels32(),_cameraTexture.width,_cameraTexture.height);
            if(result != null)
            {
                _textOut.text= ChangeToKorean(result.Text);
                GameManager.LoadScene(result.Text);
            }
            else
            {
                _textOut.text = "FAILED TO READ QR CODE";
            }
        }
        catch (Exception e)
        {
            _textOut.text = "FAILED IN TRY";
            Debug.LogWarning(e);
        }
    }

    public void Reset()
    {
        _rawImageBackground.texture = null;
        _textOut.text = string.Empty;
    }

    private string ChangeToKorean(string text)
    {
        string result = string.Empty;
        if(text == "Coloring")
        {
            result = "��ĥ����";
        }
        else if(text == "Photography")
        {
            result = "�������";
        }
        else if(text == "BlackTV")
        {
            result = "���Ƽ��";
        }
        else if (text == "ColorTV")
        {
            result = "�÷�Ƽ��";
        }
        else if (text == "JicsawPuzzle")
        {
            result = "��������";
        }
        else if(text == "3D_Reconstruction")
        {
            result = "�ڵ���";
        }
        return result;
    }
}

