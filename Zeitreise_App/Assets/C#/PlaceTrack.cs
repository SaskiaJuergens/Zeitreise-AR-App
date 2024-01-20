using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrack : MonoBehaviour
{
    public GameObject TelefonContainer; // TelefonContainer deklarieren
    public GameObject FotoContainer;    // FotoContainer deklarieren
    public GameObject VideoContainer;

    // Reference to AR tracked image manager component
    private ARTrackedImageManager _trackedImagesManager;

    // List of prefabs to instantiate - name of corresponding 2D images in reference library
    // GameObject could be Video or 3D object too
    // One of the GameObjects should respond to one of the tracked images
    //public GameObject[] ArPrefabs;

    // Keep dictionary array of created prefabs
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        // Cache a reference to the Tracked Image Manager Component
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        // Attach event handler when tracked image changes
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        // Remove event handler
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // Event Handler
    // When the tracker finds something
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Loop durch alle neuen erkannten Bilder
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log("Images added");
            // Hol den Namen des Referenzbildes
            var imageName = trackedImage.referenceImage.name;

            // Finde das entsprechende Prefab für dieses erkannte Bild
            GameObject prefabToInstantiate = null;

            if (string.Equals(imageName, "Telefon", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Telefon tracked");
                prefabToInstantiate = TelefonContainer;
                Debug.Log(prefabToInstantiate.transform.position);
            }
            else if (string.Equals(imageName, "Mario", StringComparison.OrdinalIgnoreCase))
            {
                prefabToInstantiate = VideoContainer;
            }
            else if (string.Equals(imageName, "Portrait", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Foto tracked");
                prefabToInstantiate = FotoContainer;
            }
            else
            {
                Debug.Log("No image recognized");
            }

            Debug.Log(prefabToInstantiate);

            if (prefabToInstantiate != null && !_instantiatedPrefabs.ContainsKey(imageName))
            {
                // Instanziere das Prefab und setze es als Kind des ARTrackedImage
                var newPrefab = Instantiate(prefabToInstantiate, trackedImage.transform);
                // Füge das erstellte Prefab zu unserem Array hinzu
                _instantiatedPrefabs[imageName] = newPrefab;
                Debug.Log("Debug wird instanziert");
            }
        }
        /*
        // Loop through all new tracked images that have been detected
        foreach (var trackedImage in eventArgs.added)
        {
            // Get the name of the reference image
            var imageName = trackedImage.referenceImage.name;
            // Now loop over the array of prefabs
            foreach (var curPrefab in ArPrefabs)
            {
                // Check whether this prefab matches the tracked image name
                // or hasn't it already been created
                if (string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0
                    && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    // Instantiate the prefab, parenting it to the ARTrackedImage
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    // Add the created prefab to our array
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }*/

        // For all prefabs that have been created so far, set them active or not depending
        // on whether their corresponding image is currently being tracked
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                _instantiatedPrefabs[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            }

                _instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        // Wenn das AR-Subsystem aufhört, nach einem erkannten Bild zu suchen
        // wenn das Objekt nicht mehr erkannt wird, für eine gewisse Zeit aus der Szene verschwindet
        foreach (var trackedImage in eventArgs.removed)
        {
            // Zerstöre das Prefab
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            // Entferne die Instanz aus unserem Array
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }
    }
}
