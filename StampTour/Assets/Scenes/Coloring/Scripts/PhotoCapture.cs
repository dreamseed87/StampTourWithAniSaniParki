using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    public RenderTexture characterTexture; 
    public GameObject copyCharacter;
    public GameObject captureImagePos;
    public string characterName;
    public Camera camera;
    public Rect captureArea;
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
       
        int width = (int)captureArea.width;
        int height = (int)captureArea.height;

        // 지정한 영역에 맞는 RenderTexture 생성
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        camera.targetTexture = renderTexture;
        camera.Render();

        // 기존 RenderTexture의 활성 상태 저장
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // Texture2D에 RenderTexture 내용을 복사
        Texture2D screenImage = new Texture2D(width, height, TextureFormat.RGB24, false);
        // captureArea 영역 캡쳐
        screenImage.ReadPixels(new Rect(0, 0, width, height), (int)captureArea.x, (int)captureArea.y);
        screenImage.Apply();

        // RenderTexture 비활성화 및 카메라 타겟 초기화
        camera.targetTexture = null;
        RenderTexture.active = previous;
        Destroy(renderTexture);

        // PNG로 인코딩 및 저장
        byte[] imageBytes = screenImage.EncodeToPNG();
        SavePNG(imageBytes);
    }
    void SavePNG(byte[] pngArray)
    {
        Debug.Log("Picture taken");
        string path = Path.Combine(Application.persistentDataPath, characterName + ".png");
        File.WriteAllBytes(path, pngArray);
        Debug.Log(path);
    }
    


}
