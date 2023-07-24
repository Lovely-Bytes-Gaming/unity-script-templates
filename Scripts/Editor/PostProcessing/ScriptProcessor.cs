// DONT_REPLACE_KEYWORDS  <-- Stops the script processor from changing this script. Copy/Paste at your leisure :)

#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ScriptTemplates.PostProcessing 
{
    internal sealed class ScriptProcessor : AssetModificationProcessor
    {
        private static readonly char[] delimiters = new char[] { '/', '\\', '.' };
        private static readonly string[] excludeFromNamespace = new[]
        { 
            "Scripts", 
            "Editor", 
            "Runtime", 
            "Experimental" 
        };

        private const string
            KEYWORD_NAMESPACE = "#NAMESPACE#",
            KEYWORD_ASSETMENU = "#ASSETMENU#",
            KEYWORD_DONT_REPLACE = "// DONT_REPLACE_KEYWORDS";

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);
            
            if(!path.EndsWith(".cs"))
                return;

            string namespaceString = NamespaceFromPath(path);
            string assetMenuString = namespaceString.Replace('.', '/');

            int assetFolderIndex = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, assetFolderIndex) + path;
            
            if (!System.IO.File.Exists(path))
                return;

            string fileContent = System.IO.File.ReadAllText(path);

            if(fileContent.Contains(KEYWORD_DONT_REPLACE))
                return;
                
            fileContent = fileContent.Replace(KEYWORD_NAMESPACE, namespaceString);
            fileContent = fileContent.Replace(KEYWORD_ASSETMENU, assetMenuString);

            System.IO.File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }

        public static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath) 
        {
            if(sourcePath.EndsWith(".cs")) 
                OnWillMoveScriptAsset(sourcePath, destinationPath);

            return AssetMoveResult.DidNotMove;
        }

        [MenuItem("Assets/Set Namespace By Directory")]
        private static void SetNamespaceByDirectory_ContextMenu() 
        {
            var obj = Selection.activeObject;
            
            if(!obj)
                return;

            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            ProcessPath(path);
        }

        private static void ProcessPath(string path) 
        {
            if(string.IsNullOrEmpty(path))
                return;

            if(Directory.Exists(path))
                ProcessDirectory(path);

            else
                ProcessScriptAsset(path);
        }

        private static void ProcessDirectory(string path) 
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            var files = dirInfo.GetFiles();
            var subdirs = dirInfo.GetDirectories();

            foreach(var info in files)
                ProcessScriptAsset(info.FullName);

            foreach(var info in subdirs)
                ProcessDirectory(info.FullName);
        }

        private static void ProcessScriptAsset(string path) 
        {
            if(!path.EndsWith(".cs"))
                return;

            path = path.Remove(0, path.IndexOf("Assets"));

            var fileContent = File.ReadAllText(path);
            var scriptWrapper = new ScriptWrapper(fileContent);

            if(!scriptWrapper.Value.Contains("namespace"))
                scriptWrapper.AddNameSpace("ReplaceMe");

            var namespaceFromPath = NamespaceFromPath(path);
            scriptWrapper.ChangeNamespace(namespaceFromPath);

            System.IO.File.WriteAllText(path, scriptWrapper.Value);
        }

        private static void OnWillMoveScriptAsset(string sourcePath, string destinationPath) 
        {
            string fileContent = System.IO.File.ReadAllText(sourcePath);
            fileContent = MatchNamespaceWithDirectory(fileContent, sourcePath, destinationPath);

            System.IO.File.WriteAllText(sourcePath, fileContent);
        }

        private static string MatchNamespaceWithDirectory(string scriptContent, string sourcePath, string destinationPath) 
        {
            string oldNamespace = NamespaceFromPath(sourcePath);
            string newNamespace = NamespaceFromPath(destinationPath);

            // only change the namespace if the file's namespace already replicated the projects folder structure before it was moved
            return scriptContent.Replace(oldNamespace, newNamespace);
        }

        private static string NamespaceFromPath(string path) 
        {
            List<string> namespaces = path.Split(delimiters).ToList();
            namespaces = namespaces.GetRange(1, namespaces.Count - 3);
            namespaces = namespaces.Except(excludeFromNamespace).ToList();

            string namespaceString = "";
            for (int i = 0; i < namespaces.Count; i++)
            {
                namespaceString += namespaces[i];

                if(i + 1 < namespaces.Count)
                    namespaceString += ".";
            }

            if(namespaceString.Length == 0)
                namespaceString = "ReplaceMe";

            return namespaceString;
        }
    }
}
#endif