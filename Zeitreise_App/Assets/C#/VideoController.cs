using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public GameObject videoPrefab;  // Das Video-Prefab
    public Transform videoContainer; // Hier könntest du einen leeren GameObject-Container für die Videos verwenden
    public Button plusButton;
    public Button minusButton;

    private int currentVideoIndex = 0;
    private GameObject[] videos;
    private VideoPlayer videoPlayer;

    void Start()
    {
        // Initialisiere die Videos
        GameObject videoObject = Instantiate(videoPrefab.gameObject, videoContainer);
        VideoPlayer vp = videoObject.GetComponent<VideoPlayer>();

        videos = new GameObject[20];
        for (int i = 0; i < 20; i++)
        {
            videos[i] = videoObject;
            videoObject.SetActive(false);

            string videoName = "Video" + (i + 1);
            VideoClip videoClip = Resources.Load<VideoClip>("Mario/" + videoName);

            if (videoClip == null)
            {
                Debug.LogError("VideoClip " + (i + 1) + " konnte nicht geladen werden! Name: " + videoName);
            }
            else
            {
                vp.clip = videoClip;
                Debug.Log("VideoClip " + (i + 1) + " erfolgreich geladen! Name: " + videoName);
            }
        }

        // Hole den VideoPlayer des ersten Videos
        videoPlayer = videos[currentVideoIndex].GetComponent<VideoPlayer>();

        // Spiele das erste Video ab
        PlayCurrentVideo();

        // Setze die Button-Handler
        plusButton = GameObject.FindGameObjectWithTag("plusButton").GetComponent<Button>();
        minusButton = GameObject.FindGameObjectWithTag("minusButton").GetComponent<Button>();

        plusButton.onClick.AddListener(PlayNextVideo);
        minusButton.onClick.AddListener(PlayPreviousVideo);
    }

    void PlayNextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % videos.Length;
        PlayCurrentVideo();
    }

    void PlayPreviousVideo()
    {
        currentVideoIndex = (currentVideoIndex - 1 + videos.Length) % videos.Length;
        PlayCurrentVideo();
    }

    void PlayCurrentVideo()
    {
        // Deaktiviere alle Videos
        foreach (var video in videos)
        {
            video.SetActive(false);
        }

        // Aktiviere das aktuelle Video
        videos[currentVideoIndex].SetActive(true);

        // Starte das Video ab dem Anfang
        videoPlayer.Stop();
        videoPlayer.Play();
    }
}
