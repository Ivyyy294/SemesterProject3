using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rigs.Editor
{

    public class AnimationImporter : AssetPostprocessor
    {
        private void OnPreprocessAnimation()
        {
            if (assetImporter is not ModelImporter importer || !importer.assetPath.StartsWith(AssetManager.RigsPath))
            {
                return;
            }

            ProcessAnimationImporter(importer);
        }

        public static bool ProcessAnimationImporter(ModelImporter importer)
        {
            bool needsReimport = false;

            if (importer.clipAnimations == null || importer.clipAnimations.Length == 0)
            {
                importer.clipAnimations = importer.defaultClipAnimations;
                needsReimport = true;
            }

            var clips = importer.clipAnimations;

            foreach (ModelImporterClipAnimation clip in clips)
            {
                if (!clip.takeName.EndsWith("_LOOP")) continue;
                
                Debug.Log(clip.takeName);
                clip.loop = true;
                clip.loopTime = true;
                clip.loopPose = true;
                needsReimport = true;
            }

            if (needsReimport)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                Debug.Log(importer.assetPath);
            }

            return needsReimport;
        }
    }
}