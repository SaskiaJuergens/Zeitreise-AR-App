using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;  // Import the Unity UI namespace


public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private ARTrackedImageManager trackedImageManager;

    // Liste von VideoClips
    public VideoClip[] videoClips;

    private int currentClipIndex = 0;

    // UI Buttons for controlling the video
    public Button plusButton;
    public Button minusButton;

    void Start()
    {
        // Hole den ARTrackedImageManager
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        if (trackedImageManager != null)
        {
            // Weise den Callback für das Image-Tracking-Event zu
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager not found in the scene.");
        }

        // VideoClips zur Laufzeit hinzufügen (kannst du nach Bedarf anpassen)
        videoClips = new VideoClip[22];
        for (int i = 0; i < 22; i++)
        {
            videoClips[i] = Resources.Load<VideoClip>("Videos/Video" + (i + 1));
        }

        // Video abspielen
        PlayCurrentVideo();

        // Register button click events
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Hier könntest du überprüfen, ob das erkannte Bild das ist, auf das du das Video rendern möchtest
            // In diesem Beispiel gehen wir davon aus, dass das erste erkannte Bild verwendet wird
            if (trackedImage.referenceImage.name == "DeinZielbildName")
            {
                // Setze das Video auf das Zielbild
                videoPlayer.targetMaterialRenderer = trackedImage.GetComponent<Renderer>();
                videoPlayer.Play();
                break;
            }
        }
    }

    public void PlayNextVideo()
    {
        int nextClipIndex = (currentClipIndex + 1) % videoClips.Length;
        CheckClipIndexBounds(nextClipIndex);

        // Setze das nächste Video
        videoPlayer.clip = videoClips[nextClipIndex];

        // Spiele das Video ab
        videoPlayer.Play();

        currentClipIndex = nextClipIndex;
    }

    public void PlayPreviousVideo()
    {
        int nextClipIndex = (currentClipIndex - 1 + videoClips.Length) % videoClips.Length;
        CheckClipIndexBounds(nextClipIndex);

        // Setze das nächste Video
        videoPlayer.clip = videoClips[nextClipIndex];

        // Spiele das Video ab
        videoPlayer.Play();

        currentClipIndex = nextClipIndex;
    }

    private void PlayCurrentVideo()
    {
        videoPlayer.clip = videoClips[currentClipIndex];

        // Spiele das Video ab
        videoPlayer.Play();
    }

    private void CheckClipIndexBounds(int clipIndex)
    {
        // Stelle sicher, dass der Index nicht über 21 oder unter 0 geht
        if (clipIndex < 0)
        {
            clipIndex = 0;
        }
        else if (clipIndex > 21)
        {
            clipIndex = 21;
        }
    }

    // Plus-Button-Methode
    public void OnPlusButtonClicked()
    {
        PlayNextVideo();
    }

    // Minus-Button-Methode
    public void OnMinusButtonClicked()
    {
        PlayPreviousVideo();
    }
}
