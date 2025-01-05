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
    // Your existing ProcessMaterials method:
#if UNITY_EDITOR
    private static Material[] ProcessMaterials(Material[] materials, string materialsFolderPath)
    {
        // If you don't need to store or move materials as .mat assets,
        // simply return the original materials without any AssetDatabase calls.
        return materials;
    }
#endif

}
