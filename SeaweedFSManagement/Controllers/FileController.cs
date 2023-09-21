using Microsoft.AspNetCore.Mvc;
using SeaweedFSManagement.Models;
using SeaweedFSManagement.Services;

namespace SeaweedFSManagement.Controllers
{
    public class FileController : Controller
    {
        private readonly SeaweedFileService _fileService;
        private readonly RocketDbContext _dbContext;

        public FileController(SeaweedFileService fileService, RocketDbContext dbContext)
        {
            _fileService = fileService;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var files = _dbContext.Files.ToList();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadedFile = await _fileService.UploadFileAsync(stream, file.FileName);
                    if (uploadedFile != null)
                    {
                        _dbContext.Files.Add(uploadedFile);
                        _dbContext.SaveChanges();
                        //成功信息
                        return Json(new { success = true, message = "文件上传成功" });
                    }
                    else
                    {
                        //失败信息
                        return Json(new { success = false, message = "上传图片失败" });
                    }
                }
            }
            //无效文件信息
            return Json(new { success = false, message = "无效的文件" });
        }

        public async Task<IActionResult> Download(int id)
        {
            var fileItem = _dbContext.Files.FirstOrDefault(f => f.Id == id);
            if (fileItem != null)
            {
                var fileBytes = await _fileService.DownloadFileAsync(fileItem.FileId);
                return File(fileBytes, "application/octet-stream", fileItem.FileName);
            }
            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var fileItem = _dbContext.Files.FirstOrDefault(f => f.Id == id);
            try
            {
                if (fileItem != null)
                {
                    await _fileService.DeleteFileAsync(fileItem.FileId);
                    _dbContext.Files.Remove(fileItem);
                    _dbContext.SaveChanges();
                    // 返回成功信息
                    return Json(new { success = true, message = "文件删除成功" });
                }
            }
            catch (Exception ex)
            {
                // 返回失败信息
                return Json(new { success = false, message = "文件删除失败：" + ex.Message });
            }

            // 如果文件不存在，也返回失败信息
            return Json(new { success = false, message = "文件不存在" });
        }
    }
}