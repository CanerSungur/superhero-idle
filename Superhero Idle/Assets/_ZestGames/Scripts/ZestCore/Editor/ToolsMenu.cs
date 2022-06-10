using UnityEditor;

namespace ZestCore.Editor
{
    public class ToolsMenu
    {
        [MenuItem("Zest Games/File Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDirectories("_Project", "Animations", "Graphics", "Graphics/Materials", "Graphics/Sprites", "Scenes", "Scripts", "Imports", "Prefabs");
            AssetDatabase.Refresh();
        }

        [MenuItem("Zest Games/Setup/Load New Manifest")]
        private async static void LoadNewManifest() => await Packages.ReplacePackageFromGist("9c979869faba62f726b843c760896662");

        [MenuItem("Zest Games/Setup/Packages/New Input System")]
        private static void AddNewInputSystem() => Packages.InstallUnityPackage("inputsystem");

        [MenuItem("Zest Games/Setup/Packages/Post Processing")]
        private static void AddPostProcessing() => Packages.InstallUnityPackage("postprocessing");

        [MenuItem("Zest Games/Setup/Packages/Cinemachine")]
        private static void AddCinemachine() => Packages.InstallUnityPackage("cinemachine");

        [MenuItem("Zest Games/Setup/Packages/Unity Recorder")]
        private static void AddUnityRecorder() => Packages.InstallUnityPackage("recorder");

        [MenuItem("Zest Games/Setup/Packages/Text Mesh Pro")]
        private static void AddTextMeshPro() => Packages.InstallUnityPackage("textmeshpro");

        [MenuItem("Zest Games/Setup/Packages/Pro Builder")]
        private static void AddProBuilder() => Packages.InstallUnityPackage("probuilder");
    }
}
