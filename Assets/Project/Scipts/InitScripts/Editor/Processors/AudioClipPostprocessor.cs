using UnityEditor;

namespace Project
{
    public class AudioClipPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessAudio()
        {
            AudioImporter importer = (AudioImporter)assetImporter;

            if (importer.assetPath.Contains("IgnorePostprocess"))
            {
                return;
            }

            if (importer.assetPath.Contains("Streaming"))
            {
                var settings = importer.defaultSampleSettings;
                settings.compressionFormat = UnityEngine.AudioCompressionFormat.Vorbis;
                settings.loadType = UnityEngine.AudioClipLoadType.Streaming;
                settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
                settings.quality = 0;

                AudioImporter audioImporter = (AudioImporter)assetImporter;
                audioImporter.forceToMono = true;

                importer.SetOverrideSampleSettings("Android", settings);
                importer.SetOverrideSampleSettings("iPhone", settings);
            }

            if (importer.assetPath.Contains("Decompressed"))
            {
                var settings = importer.defaultSampleSettings;
                settings.compressionFormat = UnityEngine.AudioCompressionFormat.ADPCM;
                settings.loadType = UnityEngine.AudioClipLoadType.DecompressOnLoad;
                settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;

                AudioImporter audioImporter = (AudioImporter)assetImporter;
                audioImporter.forceToMono = true;
                audioImporter.loadInBackground = true;

                importer.SetOverrideSampleSettings("Android", settings);
                importer.SetOverrideSampleSettings("iPhone", settings);
            }

            if (importer.assetPath.Contains("Compressed"))
            {
                var settings = importer.defaultSampleSettings;
                settings.compressionFormat = UnityEngine.AudioCompressionFormat.ADPCM;
                settings.loadType = UnityEngine.AudioClipLoadType.CompressedInMemory;
                settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;

                AudioImporter audioImporter = (AudioImporter)assetImporter;
                audioImporter.forceToMono = true;

                importer.SetOverrideSampleSettings("Android", settings);
                importer.SetOverrideSampleSettings("iPhone", settings);
            }

        }
    }
}