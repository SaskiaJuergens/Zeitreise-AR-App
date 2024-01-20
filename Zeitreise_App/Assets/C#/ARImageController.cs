using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

    public class ARImageController : MonoBehaviour
    {
        ARTrackedImageManager imageManager;
        GameObject prefabInstance;
        //AnimationSwitchController animationSwitchController;


        public GameObject trackedImagePrefab;

        private void Awake()
        {
            imageManager = GetComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        private void OnDisable()
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs changedImages)
        {
            if (changedImages.added != null && changedImages.added.Count > 0)
            {
                Debug.Log($"Images added: {changedImages.added}");

                foreach (var image in changedImages.added)
                {
                    prefabInstance = Instantiate(trackedImagePrefab, image.transform);
                }
            }

            if (changedImages.updated != null && changedImages.updated.Count > 0)
            {
                Debug.Log($"Images updated: {changedImages.updated}");
            }

            if (changedImages.removed != null && changedImages.removed.Count > 0)
            {
                Debug.Log($"Images removed: {changedImages.removed}");
            }
        }
    }
