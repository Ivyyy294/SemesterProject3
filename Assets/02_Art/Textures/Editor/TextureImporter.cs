using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Textures.Editor
{

    public class TextureImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            UnityEditor.TextureImporter importer = assetImporter as UnityEditor.TextureImporter;
            ProcessTextureImporter(importer);
        }

        public static bool ProcessTextureImporter(UnityEditor.TextureImporter importer)
        {
            var assetName = System.IO.Path.GetFileNameWithoutExtension(importer.assetPath);

            if (!assetName.StartsWith("TEX_")) return false;
            
            bool needsReimport = false;

            var message = assetName;
            var assetNameLower = assetName.ToLower();
            if (assetNameLower.EndsWith("_normal"))
            {
                importer.textureType = TextureImporterType.NormalMap;
                message += " is a Normal Map";
                needsReimport = true;
            }

            if (assetNameLower.EndsWith("_mra"))
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = false;
                message += " is an MRA Map";
                needsReimport = true;
            }

            if (assetNameLower.EndsWith("_mask"))
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = false;
                message += "is a Mask Map";
                needsReimport = true;
            }

            if (needsReimport)
            {
                message += ". Reimport!";
                Debug.Log(message);
                importer.SaveAndReimport();
            }

            return needsReimport;
        }
    }
}

