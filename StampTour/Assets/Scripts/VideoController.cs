using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; // TextMeshPro를 사용하기 위해 필요
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button upButton;
    public Button downButton;
    public Button completeButton; // 완료 버튼 추가
    public TextMeshProUGUI channelChangeText; // 채널 변경 안내 텍스트 추가
    public VideoClip[] videoClips;
    private int currentChannel = 0;
    private bool isChannelChangeComplete = false; // 채널 변경 완료 여부

    void Start()
    {
        // 각 버튼에 클릭 이벤트를 연결합니다.
        upButton.onClick.AddListener(NextChannel);
        downButton.onClick.AddListener(PreviousChannel);

        // 완료 버튼 클릭 이벤트를 연결합니다.
        completeButton.onClick.AddListener(OnCompleteButtonClick);

    

        // 초기 비디오 재생
        if (videoClips.Length > 0)
        {
            videoPlayer.clip = videoClips[currentChannel];
            videoPlayer.Play();
        }

        // 채널 변경 안내 텍스트를 3초 동안 표시합니다.
        StartCoroutine(ShowChannelChangeText());
    }

    IEnumerator ShowChannelChangeText()
    {
        channelChangeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        channelChangeText.gameObject.SetActive(false);
    }

    void NextChannel()
    {
        currentChannel = (currentChannel + 1) % videoClips.Length;
        ChangeChannel(currentChannel);
    }

    void PreviousChannel()
    {
        currentChannel = (currentChannel - 1 + videoClips.Length) % videoClips.Length;
        ChangeChannel(currentChannel);
    }

    void ChangeChannel(int channelIndex)
    {
        if (channelIndex < 0 || channelIndex >= videoClips.Length)
        {
            Debug.LogError("Invalid channel index");
            return;
        }

        videoPlayer.clip = videoClips[channelIndex];
        videoPlayer.Play();
    }

    void OnCompleteButtonClick()
    {
        if(GameManager.Instance.GetIsSceneFinished("TV"))
        {
            GameManager.RollbackMainScene();
        }
    }
}
