using UnityEngine;

public class SavePrefabExample : MonoBehaviour
{
    [SerializeField] private string prefabToLoadName = "Level";
    [SerializeField] private GameObject prefab;

    private GameObject originalPrefab;

    private void Awake()
    {
        originalPrefab = prefab;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Save
            ES3.Save("Level", prefab);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // Destroy the existing prefab instance
            if (prefab != null)
                prefab.gameObject.SetActive(false);

            // Load the prefab from ES3
            GameObject esobj = ES3.Load("Level", originalPrefab);
            esobj.transform.position = new Vector3 (0, 0, 5);
            // (Optionally, if the loaded instance returns a reference or you can otherwise retrieve it)
            // let's assume 'prefab' is now the loaded instance
            // ...

            // Restore the materials on the loaded prefab (Editor-only)
#if UNITY_EDITOR
            string materialsFolder = "Assets/Materials";  // Adjust the path to your materials folder
            MaterialRestorer.RestoreMaterialsOnLoadedObject(originalPrefab, materialsFolder);
#endif
        }
    }
}
