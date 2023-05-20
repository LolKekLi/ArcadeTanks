using UnityEditor;

namespace Project
{
    public class TexturePreprocessor : AssetPostprocessor
    {
        private const string AndroidPlatformName = "Android";
        
        private void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            if (importer.assetPath.Contains("Art/Sprites"))
            {
                if (importer.assetPath.Contains("IgnorePostprocess"))
                {
                    return;
                }

                if (importer.assetPath.Contains("Sprites"))
                {
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;

                        importer.mipmapEnabled = false;
                    }
                }

                SetupPlatformSettings(importer, AndroidPlatformName);
            }
        }
        
        private void SetupPlatformSettings(TextureImporter importer, string platformName)
        {
            var settings = importer.GetPlatformTextureSettings(platformName);
            var defaultSettings = importer.GetDefaultPlatformTextureSettings();
            
            if (settings == null)
            {
                settings = new TextureImporterPlatformSettings()
                {
                    maxTextureSize = defaultSettings.maxTextureSize,
                    resizeAlgorithm = defaultSettings.resizeAlgorithm,
                };
            }
            
            settings.name = platformName;
            settings.overridden = true;
            settings.compressionQuality = (int)TextureCompressionQuality.Normal;
            settings.format = TextureImporterFormat.ASTC_4x4;

            importer.SetPlatformTextureSettings(settings);
        }
    }
}