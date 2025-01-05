#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;

public static class MaterialRestorer
{
    /// <summary>
    /// Restores all MeshRenderer and SkinnedMeshRenderer materials on the given GameObject using
    /// the supplied ProcessMaterials method. It moves or creates .mat files in the specified folder
    /// if needed.
    /// </summary>
    /// <param name="rootObject">Root object whose children should have materials restored.</param>
    /// <param name="materialsFolderPath">Path to the Materials folder where .mat assets reside or should be created.</param>
    public static void RestoreMaterialsOnLoadedObject(GameObject rootObject, string materialsFolderPath)
    {
        // Restore MeshRenderer materials
        MeshRenderer[] meshRenderers = rootObject.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var meshRenderer in meshRenderers)
        {
            Material[] updatedMaterials = ProcessMaterialsSafe(meshRenderer.sharedMaterials, materialsFolderPath);
            meshRenderer.sharedMaterials = updatedMaterials;
        }

        // Restore SkinnedMeshRenderer materials
        SkinnedMeshRenderer[] skinnedRenderers = rootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var skinnedRenderer in skinnedRenderers)
        {
            Material[] updatedMaterials = ProcessMaterialsSafe(skinnedRenderer.sharedMaterials, materialsFolderPath);
            skinnedRenderer.sharedMaterials = updatedMaterials;
        }
    }

    /// <summary>
    /// A safe wrapper around your ProcessMaterials method that won’t break builds.
    /// This only executes in the Editor; otherwise it just returns the unmodified materials.
    /// </summary>
    private static Material[] ProcessMaterialsSafe(Material[] materials, string materialsFolderPath)
    {
#if UNITY_EDITOR
        return ProcessMaterials(materials, materialsFolderPath);
#else
        // In a build (runtime), you might just return the materials as-is
        // or handle them with another approach, since AssetDatabase won't work.
        return materials;
#endif
    }

    // Your existing ProcessMaterials method:
#if UNITY_EDITOR
    private static Material[] ProcessMaterials(Material[] materials, string materialsFolderPath)
    {
        Material[] updatedMaterials = new Material[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            Material originalMaterial = materials[i];
            if (originalMaterial == null)
            {
                // Keep null if there's no material for that slot
                updatedMaterials[i] = null;
                continue;
            }

            // Check if this material is already a saved asset
            string originalPath = AssetDatabase.GetAssetPath(originalMaterial);
            Material finalMaterial = null;

            if (!string.IsNullOrEmpty(originalPath))
            {
                // The material is already a .mat file in the project.
                if (originalPath.StartsWith(materialsFolderPath))
                {
                    // It's already in the right folder. Use it directly.
                    finalMaterial = originalMaterial;
                }
                else
                {
                    // Move it to our Materials folder
                    string fileName = Path.GetFileName(originalPath);
                    string newPath = Path.Combine(materialsFolderPath, fileName).Replace('\\', '/');

                    // If there's already a file at that path, Unity will rename the moved file
                    AssetDatabase.MoveAsset(originalPath, newPath);
                    finalMaterial = AssetDatabase.LoadAssetAtPath<Material>(newPath);

                    Debug.Log($"Moved material '{originalMaterial.name}' from '{originalPath}' to '{newPath}'.");
                }
            }
            else
            {
                // The material is not an asset (likely an instance). Let's see if there's a matching .mat in the folder.
                string potentialPath = Path.Combine(materialsFolderPath, originalMaterial.name + ".mat").Replace('\\', '/');
                Material existingMat = AssetDatabase.LoadAssetAtPath<Material>(potentialPath);

                if (existingMat != null)
                {
                    // Reuse existing material
                    finalMaterial = existingMat;
                    Debug.Log($"Reused existing material '{existingMat.name}' at '{potentialPath}'.");
                }
                else
                {
                    // Create a new .mat asset in the folder by cloning the original
                    Material newMat = Object.Instantiate(originalMaterial);
                    AssetDatabase.CreateAsset(newMat, potentialPath);
                    finalMaterial = newMat;
                    Debug.Log($"Created new material asset for '{originalMaterial.name}' at '{potentialPath}'.");
                }
            }

            updatedMaterials[i] = finalMaterial;
        }

        return updatedMaterials;
    }
#endif
}
