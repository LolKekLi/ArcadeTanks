using UnityEditor;

namespace Project
{
    public class ModelPostprocessor : AssetPostprocessor
    {
        public void OnPreprocessModel() 
        {
            ModelImporter importer = (ModelImporter)assetImporter;
            importer.importBlendShapes = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.optimizeMeshPolygons = true;
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            importer.importTangents = ModelImporterTangents.None;
            importer.animationCompression = ModelImporterAnimationCompression.KeyframeReduction;

            if (importer.isReadable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }
    }
}