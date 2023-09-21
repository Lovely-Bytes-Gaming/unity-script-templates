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
            _keywordDontReplace = "// DONT_REPLACE_KEYWORDS";

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
            
            string assetMenuString = string.IsNullOrEmpty(namespaceString) 
                ? "Custom"
                : namespaceString.Replace('.', '/');
            
            fileContent = fileContent.Replace(_keywordAssetMenu, assetMenuString);

            System.IO.File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }
        
        private static string ExtractNamespace(in string fileContent)
        {
            int start = fileContent.IndexOf("namespace", StringComparison.Ordinal)
                        + "namespace".Length;   // could be hardcoded, but is clearer this way

            int length = fileContent.IndexOf('\n', start) - start - 1;
            length = Mathf.Max(0, length);
            
            string result = fileContent.Substring(start, length);
            return result;
        }
    }
}
#endif