using Newtonsoft.Json;
using SeaweedFSManagement.Models;

namespace SeaweedFSManagement.Services
{
    public class SeaweedFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public SeaweedFileService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FileItem> UploadFileAsync(Stream fileStream, string fileName)
        {
            var seaweedServerUrl = _configuration["SeaweedFS:ServerUrl"];

            using (var client = _httpClientFactory.CreateClient())
            {
                //第1步：请求fid和卷服务器URL
                var assignUrl = $"{seaweedServerUrl}/dir/assign";
                var assignResponse = await client.GetAsync(assignUrl);

                if (assignResponse.IsSuccessStatusCode)
                {
                    var assignData = await assignResponse.Content.ReadAsStringAsync();
                    var assignResult = JsonConvert.DeserializeObject<AssignResult>(assignData);

                    if (assignResult != null && !string.IsNullOrEmpty(assignResult.Fid) && !string.IsNullOrEmpty(assignResult.Url))
                    {
                        //步骤2：上传文件内容到卷服务器
                        var uploadUrl = $"http://{assignResult.Url}/{assignResult.Fid}";

                        using (var formData = new MultipartFormDataContent())
                        using (var fileContent = new StreamContent(fileStream))
                        {
                            formData.Add(fileContent, "file", fileName);

                            var uploadResponse = await client.PostAsync(uploadUrl, formData);

                            if (uploadResponse.IsSuccessStatusCode)
                            {
                                var fileInfo = await uploadResponse.Content.ReadAsStringAsync();
                                return new FileItem
                                {
                                    FileName = fileName,
                                    FileId = assignResult.Fid
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }
        public async Task<byte[]> DownloadFileAsync(string fileId)
        {
            var seaweedServerUrl = _configuration["SeaweedFS:ServerUrl"];
            var downloadUrl = $"{seaweedServerUrl}/{fileId}";

            using (var client = _httpClientFactory.CreateClient())
            {

                var response = await client.GetAsync(downloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }

            return null;
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            var seaweedServerUrl = _configuration["SeaweedFS:ServerUrl"];
            var deleteUrl = $"{seaweedServerUrl}/{fileId}";

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.DeleteAsync(deleteUrl);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
