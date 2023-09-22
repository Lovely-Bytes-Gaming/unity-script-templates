// DONT_REPLACE_KEYWORDS  <-- Stops the script processor from changing this script. Copy/Paste at your leisure :)

#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor.Compilation;
using UnityEditorInternal;

namespace ScriptTemplates
{
    internal sealed class ScriptProcessor : AssetModificationProcessor
    {
        private static readonly char[] _delimiters = new char[] { '/', '\\', '.' };

        private const string
            _keywordNamespace = "#NAMESPACE#",
            _keywordAssetMenu = "#ASSETMENU#",
            _keywordDontReplace = "// DONT_REPLACE_KEYWORDS",
            _defaultAssetMenu = "Custom";

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);
            
            if(!path.EndsWith(".cs"))
                return;
            
            int assetFolderIndex = Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal);
            path = Application.dataPath[..assetFolderIndex] + path;
            
            if (!System.IO.File.Exists(path))
                return;

            string fileContent = System.IO.File.ReadAllText(path);
            if(fileContent.Contains(_keywordDontReplace))
                return;
            
            string namespaceString = ExtractNamespace(fileContent);
            string assetMenuString = namespaceString.Replace('.', '/');
            
            fileContent = fileContent.Replace(_keywordAssetMenu, assetMenuString);

            System.IO.File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }
        
        private static string ExtractNamespace(in string fileContent)
        {
            int start = fileContent.IndexOf("namespace", StringComparison.Ordinal);

            if (start < 0)
                return _defaultAssetMenu;

            // Skip the "namespace" keyword, which has 9 letters
            start += 9;

            int charactersToRead = fileContent.IndexOf('\n', start) - start - 1;
            charactersToRead = Mathf.Max(0, charactersToRead);
            
            string result = fileContent.Substring(start, charactersToRead);
            return result;
        }
    }
}
#endif