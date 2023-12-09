using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private ARTrackedImageManager trackedImageManager;

    // Liste von VideoClips
    public VideoClip[] videoClips;

    private int currentClipIndex = 0;

    void Start()
    {
        // Hole den ARTrackedImageManager
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        // VideoClips zur Laufzeit hinzuf�gen (kannst du nach Bedarf anpassen)
        videoClips = new VideoClip[22];
        for (int i = 0; i < 22; i++)
        {
            videoClips[i] = Resources.Load<VideoClip>("Videos/Video" + (i + 1));
        }

        // Weise den Callback f�r das Image-Tracking-Event zu
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        // Video abspielen
        PlayCurrentVideo();
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Hier k�nntest du �berpr�fen, ob das erkannte Bild das ist, auf das du das Video rendern m�chtest
            // In diesem Beispiel gehen wir davon aus, dass das erste erkannte Bild verwendet wird
            if (trackedImage.referenceImage.name == "DeinZielbildName")
            {
                // Setze das Video auf das Zielbild
                videoPlayer.targetMaterialRenderer = trackedImage.GetComponent<Renderer>();
                videoPlayer.Play();

                // F�ge die Callback-Funktion f�r das Image-Tracking-Event hinzu
                trackedImage.tracked += OnImageTracked;
                trackedImage.trackingLost += OnImageTrackingLost;

                break;
            }
        }
    }

    private void OnImageTracked(ARTrackedImage trackedImage)
    {
        // Hier k�nntest du zus�tzliche Aktionen ausf�hren, wenn das Bild getrackt wird
    }

    private void OnImageTrackingLost(ARTrackedImage trackedImage)



