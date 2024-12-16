using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Newtonsoft.Json;


namespace KapibaraCore.RevitServer
{
    public class ServerResponseItem
    {
        public string Path { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string LastModifiedBy { get; set; }
        public int Size { get; set; }
        public int ModelSize { get; set; }
        public int FolderCount { get; set; }
        public int ModelCount { get; set; }
        public bool IsFolder { get; set; }
        public bool Exists { get; set; }
    }

    public class TestRevitServer
    {
        public static async Task<TreeItem> GetFoldersAndFilesAsync(string serverNameOrIP, string revitVersion)
        {
            try
            {
                ServerResponseItem rootItem = await GetFolderItemAsync("/|", serverNameOrIP, revitVersion);
                
                TreeItem root = new TreeItem { Name = "Server", Tag = "Folder" };
                TreeItem rootChild = new TreeItem { Name = rootItem.Path, Tag = rootItem.IsFolder ? "Folder" : "Model" };
                root.Children.Add(rootChild);

                if (rootItem.IsFolder && rootItem.FolderCount > 0)
                {
                    await AddContentsAsync(rootChild, rootItem.Path, serverNameOrIP, revitVersion);
                }

                return root;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка в GetFoldersAndFilesAsync", ex.ToString());
                throw;
            }
        }

        private static async Task AddContentsAsync(TreeItem parentItem, string path, string serverNameOrIP, string revitVersion)
        {
            try
            {
                List<ServerResponseItem> items = await GetFolderContentsAsync(path, serverNameOrIP, revitVersion);

                foreach (var item in items)
                {
                    string name = item.Path;

                    TreeItem child = new TreeItem
                    {
                        Name = name,
                        Tag = item.IsFolder ? "Folder" : "Model"
                    };
                    parentItem.Children.Add(child);

                    if (item.IsFolder && item.FolderCount > 0)
                    {
                        await AddContentsAsync(child, item.Path, serverNameOrIP, revitVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка в AddContentsAsync", ex.ToString());
            }
        }

        private static async Task<ServerResponseItem> GetFolderItemAsync(string path, string serverNameOrIP, string revitVersion)
        {
            string content = await GetResponseContentAsync($"{path}/contents", serverNameOrIP, revitVersion);
            
            
            var item = JsonConvert.DeserializeObject<ServerResponseItem>(content);
            if (item == null)
            {
                throw new Exception("Не удалось десериализовать ответ сервера.");
            }

            return item;
        }

        private static async Task<List<ServerResponseItem>> GetFolderContentsAsync(string path, string serverNameOrIP, string revitVersion)
        {
            string content = await GetResponseContentAsync($"{path}/contents", serverNameOrIP, revitVersion);
            
            TaskDialog.Show("Содержимое ответа", content);

            var items = JsonConvert.DeserializeObject<List<ServerResponseItem>>(content);
            if (items == null)
            {
                throw new Exception("Не удалось десериализовать ответ сервера.");
            }

            return items;
        }

        private static async Task<string> GetResponseContentAsync(string info, string serverNameOrIP, string revitVersion)
        {
            string url = $"http://{serverNameOrIP}/RevitServerAdminRESTService{revitVersion}/AdminRESTService.svc{info}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("User-Name", Environment.UserName);
            request.Headers.Add("User-Machine-Name", Environment.MachineName);
            request.Headers.Add("Operation-GUID", Guid.NewGuid().ToString());

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw new Exception("Поток ответа пуст.");
                    }

                    using (var reader = new StreamReader(responseStream))
                    {
                        string content = await reader.ReadToEndAsync();
                        return content;
                    }
                }
            }
            catch (WebException webEx)
            {
                string message = $"Ошибка при выполнении запроса: {webEx.Message}";
                if (webEx.Response != null)
                {
                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string responseText = await reader.ReadToEndAsync();
                        message += $"\nОтвет сервера: {responseText}";
                    }
                }
                throw new Exception(message, webEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось получить ответ от сервера: {ex.Message}", ex);
            }
        }
    }
}
