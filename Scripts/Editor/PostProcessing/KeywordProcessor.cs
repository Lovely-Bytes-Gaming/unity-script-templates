// DONT_REPLACE_KEYWORDS

#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ScriptTemplates.PostProcessing 
{
    internal sealed class KeywordProcessor : AssetModificationProcessor
    {
        private static readonly char[] delimiters = new char[] { '/', '\\', '.' };
        private static readonly List<string> excludeFromNamespace = new() 
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
            if(Directory.Exists(path))
                ProcessDirectory(path);

            else if (path.EndsWith(".cs"))
                ProcessScriptAsset(path);
        }

        private static void ProcessDirectory(string path) 
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var fileInfos = dirInfo.GetFiles();
            var subdirs = dirInfo.GetDirectories();

            foreach(var info in fileInfos)
                ProcessPath(info.FullName);

            foreach(var info in subdirs)
                ProcessPath(info.FullName);
        }

        private static void ProcessScriptAsset(string path) 
        {
            path = path.Remove(0, path.IndexOf("Assets"));

            var namespaceStr = "namespace " + NamespaceFromPath(path);
            var scriptStr = File.ReadAllText(path);

            if(!scriptStr.Contains("namespace "))
                AddNamespaceKeyword(ref scriptStr);

            int idx = 0;
            while(true) 
            {
                idx = scriptStr.IndexOf("namespace ", idx);
                
                if(idx < 0)
                    break;

                int endIdx = scriptStr.IndexOf('\n', idx);

                scriptStr = scriptStr.Remove(idx, endIdx - idx);
                scriptStr = scriptStr.Insert(idx, namespaceStr);

                idx += "namespace ".Length;
            }
            System.IO.File.WriteAllText(path, scriptStr);
        }

        private static void AddNamespaceKeyword(ref string fileContent) 
        {
            int idx = fileContent.LastIndexOf("using");

            if(idx < 0)
                idx = 0;

            else
                idx = fileContent.IndexOf('\n', idx) + 1;

            fileContent = fileContent.Insert(idx, "\nnamespace ReplaceMe\n{\n");
            idx = fileContent.IndexOf('{') + 2;

            do
            {
                fileContent = fileContent.Insert(idx, "\t");
                idx = fileContent.IndexOf('\n', idx + 1) + 1;
            } while (idx > 0);

            fileContent += "\n}";
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