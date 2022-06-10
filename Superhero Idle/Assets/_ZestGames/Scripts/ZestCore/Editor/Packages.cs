using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace ZestCore.Editor
{
    public class Packages
    {
        public static async Task ReplacePackageFromGist(string id, string user = "CanerSungur")
        {
            var url = GetGistUrl(id, user);
            var contents = await GetContents(url);
            ReplacePackageFile(contents);
        }

        private static string GetGistUrl(string id, string user = "CanerSungur") => $"https://gist.github.com/{user}/{id}/raw";

        private static async Task<string> GetContents(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        private static void ReplacePackageFile(string contents)
        {
            var existing = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            File.WriteAllText(existing, contents);
            UnityEditor.PackageManager.Client.Resolve();
        }

        /// <summary>
        /// Dont need version number. It will automaticly download the latest version.
        /// </summary>
        /// <param name="packageName"></param>
        public static void InstallUnityPackage(string packageName)
        {
            UnityEditor.PackageManager.Client.Add($"com.unity.{packageName}");
        }
    }
}
