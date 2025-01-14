using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using JicsawPuzzle;

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture characterTexture; 
    public GameObject copyCharacter;
    public GameObject captureImagePos;
    public string characterName;
    public Camera camera;
    public Rect captureArea;
    public GameObject photoFrame;
    public Image photo;
    public AudioClip captureAudio;
    public AudioClip showAudio;
    public GameObject CameraImage;
    public ResultUIController ResultUI;

    [Space(5), Header("Capture")]
    public RectTransform CaptureRect;
    public RectTransform RootCanvasRect;

    AudioSource audioSource;
    Texture2D screenImage;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        captureArea.width = Screen.width;
        captureArea.height = Screen.height;
    }

    public void CharacterTakeScreenShot()
    {
        //captureCanvas의 자식 오브젝트를 지움
        if (captureImagePos.transform.childCount > 0) 
            Destroy(captureImagePos.transform.GetChild(0).gameObject);

        //사용할 이미지 복제
        Instantiate(copyCharacter, captureImagePos.transform);

        RenderTexture.active = characterTexture;
        if (characterTexture != null)
        {
            //이미지 만들기
            Texture2D Image = new Texture2D(characterTexture.width, characterTexture.height);
            Image.ReadPixels(new Rect(0, 0, characterTexture.width, characterTexture.height), 0, 0);
            Image.Apply();

            byte[] pngBytes = Image.EncodeToPNG();
            SavePNG(pngBytes);
            RenderTexture.active = null;
            characterTexture.Release();
        }
    }
    public void TakeScreenShot()
    {
        Rect captureArea = CaptureRect.rect;
        float xClop = (RootCanvasRect.rect.width - captureArea.height) / 2;
        float yClop = (RootCanvasRect.rect.height - captureArea.width) / 2;

        int width = (int)(RootCanvasRect.rect.width - xClop * 2);
        int height = (int)(RootCanvasRect.rect.height - yClop * 2);

        int capWidth = (int)RootCanvasRect.rect.width;
        int capHeight = (int)RootCanvasRect.rect.height;
        // 지정한 영역에 맞는 RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(capWidth, capHeight, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        // 기존 RenderTexture의 활성 상태 저장
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Texture2D에 RenderTexture 내용을 복사
        screenImage = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenImage.ReadPixels(new Rect(xClop, yClop, width, height), 0, 0, true);
        screenImage.Apply();
        
        
        // RenderTexture 비활성화 및 카메라 타겟 초기화
        camera.targetTexture = null;
       // RenderTexture.active = previous;
        Destroy(renderTexture);

        // PNG로 인코딩 및 저장
        byte[] imageBytes = screenImage.EncodeToPNG();
        screenImage.LoadImage(imageBytes);
        // SavePNG(imageBytes);
        SaveToGallery();
    }
    void SavePNG(byte[] pngArray)
    {
        Debug.Log("Picture taken");
        string path = Path.Combine(Application.persistentDataPath, characterName + ".png");
        File.WriteAllBytes(path, pngArray);
        Debug.Log(path);
    }
    public void SaveToGallery()
    {
        // do something with texture
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
        if (permission == NativeGallery.Permission.Denied)
        {
            if (NativeGallery.CanOpenSettings())
            {
                NativeGallery.OpenSettings();
            }
        }
        Sprite sprite = Sprite.Create(screenImage, new Rect(0, 0, screenImage.width, screenImage.height), Vector2.zero);
        photo.sprite = sprite;
        audioSource.clip = captureAudio;
        audioSource.Play();
        Invoke("TakePhoto", 1f);
    }
    void TakePhoto()
    {
        CameraImage.SetActive(false);
        photoFrame.SetActive(true);
    }
    public void RetakePhoto()
    {
        CameraImage.SetActive(true);
        photoFrame.SetActive(false);
        audioSource.clip = showAudio;
        audioSource.Play();
    }
    public void EndGame()
    {
        NativeGallery.SaveImageToGallery(screenImage, "AnsanIndustrialHistoryMuseum", "아니사니바기와 함께 사진찍기");
        photoFrame.SetActive(false);
        StartCoroutine(ResultUI.Play());
    }


}
