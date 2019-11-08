using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace lonefire.Services
{
    public class FileIoHelper : IFileIoHelper
    {
        private readonly INotifier _toaster;
        private readonly IConfiguration _config;
        public FileIoHelper(
            INotifier toaster,
            IConfiguration config
            )
        {
            _toaster = toaster;
            _config = config;
        }

        private string ImageUploadPath => "wwwroot" + _config.GetValue<string>("img_upload_path");
        private string FileUploadPath => "wwwroot" + _config.GetValue<string>("file_upload_path");

        public async Task<string> SaveImgAsync(IFormFile img, string savePath, int imgNameLengthLimit)
        {
            return await CopyToAsync(img, ImageUploadPath + savePath, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", imgNameLengthLimit);
        }

        public async Task<string> SaveImgAsync(IFormFile img, string savePath, int imgNameLengthLimit, string imgName)
        {
            return await CopyToAsync(img, ImageUploadPath + savePath, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", imgNameLengthLimit, imgName);
        }

        public bool DeleteImg(string imgToDeletePath, string imgToDelete)
        {
            return Delete(ImageUploadPath + imgToDeletePath, imgToDelete, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", 256);
        }
        public bool DeleteImg(string imgToDeletePath, string imgToDelete, int imgNameLengthLimit)
        {
            return Delete(ImageUploadPath + imgToDeletePath, imgToDelete, @"^[\s\S]+\.(jpg|gif|png|bmp|jpeg|svg)$", imgNameLengthLimit);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", 256);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath, int fileNameLengthLimit)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", fileNameLengthLimit);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string savePath, int fileNameLengthLimit, string saveFileName)
        {
            return await CopyToAsync(file, FileUploadPath + savePath, @"[\s\S]+", fileNameLengthLimit, saveFileName);
        }

        public bool DeleteFile(string fileToDeletePath, string fileToDelete)
        {
            return Delete(FileUploadPath + fileToDeletePath, fileToDelete, @"[\s\S]+", 256);
        }

        public bool DeleteFile(string fileToDeletePath, string fileToDelete, int fileNameLengthLimit)
        {
            return Delete(FileUploadPath + fileToDeletePath, fileToDelete, @"[\s\S]+", fileNameLengthLimit);
        }

        public bool MoveImgDir(string src, string dst)
        {
            return MoveDir(ImageUploadPath + src, ImageUploadPath + dst);
        }

        public bool MoveFileDir(string src, string dst)
        {
            return MoveDir(FileUploadPath + src, FileUploadPath + dst);
        }

        public bool MoveDir(string src, string dst)
        {
            if (src == null || dst == null || src.Length > 512 || dst.Length > 512)
            {
                _toaster.ToastInfo("文件夹移动或重命名失败: 目标不存在 或 文件夹名格式错误");
                return false;
            }
            else
            {
                try
                {
                    Directory.Move(src, dst);
                    _toaster.ToastInfo("文件夹移动或重命名成功");
                    return true;
                }
                catch (Exception)
                {
                    _toaster.ToastInfo("文件夹移动或重命名失败: IO错误");
                    return false;
                }
            }
        }

        //Save file with random name
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> CopyToAsync(IFormFile file, string savePath, string validateRegex, int fileNameLengthLimit)
        {
            if (file == null || file.FileName.Length > fileNameLengthLimit)
            {
                _toaster.ToastInfo("文件上传失败: 文件不存在 或 文件名格式错误");
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);

            //validate fileName
            var regex = validateRegex;
            var match = Regex.Match(fileName, regex, RegexOptions.IgnoreCase);
            if (file.Length > 0 && fileName.Length <= fileNameLengthLimit && match.Success)
            {
                //get extension name
                string extName = Path.GetExtension(fileName);
                Random random = new Random();
                fileName = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss") + random.Next(0, 100) + extName;

                //Dir create (No need to check existence)
                try
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), savePath));
                }
                catch (Exception)
                {
                    return null;
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    return fileName;
                }
            }
            return null;
        }

        //Save file with specified filename
        //returns: Suceed-> (string) fileName.ext 
        //         Failed-> null
        public async Task<string> CopyToAsync(IFormFile file, string savePath, string validateRegex, int fileNameLengthLimit, string saveFileName)
        {
            if (file == null || file.FileName.Length > fileNameLengthLimit)
            {
                _toaster.ToastInfo("文件上传失败: 文件不存在 或 文件名格式错误");
                return null;
            }

            var fileName = Path.GetFileName(file.FileName);

            //validate fileName
            var regex = validateRegex;
            var match = Regex.Match(fileName, regex, RegexOptions.IgnoreCase);
            if (file.Length > 0 && fileName.Length <= fileNameLengthLimit && match.Success)
            {
                //Full File Name
                string extName = Path.GetExtension(fileName);
                if (Path.GetExtension(saveFileName).Length == 0)
                {
                    fileName = saveFileName + extName;
                }
                else
                {
                    fileName = Path.GetFileNameWithoutExtension(saveFileName) + extName;
                }

                //Dir create (No need to check existence)
                try
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), savePath));
                }
                catch (Exception)
                {
                    _toaster.ToastInfo("目录 " + Path.Combine(Directory.GetCurrentDirectory(), savePath) + " 创建失败");
                    return null;
                }
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), savePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        await file.CopyToAsync(stream);
                        _toaster.ToastInfo("文件 " + fileName + " 上传成功");
                    }
                    catch (Exception)
                    {
                        _toaster.ToastInfo("文件 " + fileName + " 上传失败: IO错误");
                        return null;
                    }
                    return fileName;
                }
            }
            _toaster.ToastInfo("文件 " + fileName + " 上传失败: 文件名格式错误");
            return null;
        }

        //Delete file with filename.ext & path
        public bool Delete(string fileToDeletePath, string fileToDelete, string validateRegex, int fileNameLengthLimit)
        {
            var match = Regex.Match(fileToDelete, validateRegex, RegexOptions.IgnoreCase);
            if (fileToDelete == null || fileToDelete.Length > fileNameLengthLimit || !match.Success)
            {
                _toaster.ToastInfo("文件 " + fileToDelete + "删除失败: 文件名格式错误");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileToDeletePath, fileToDelete);

            try
            {
                // Check if file exists with its full path    
                if (System.IO.File.Exists(filePath))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(filePath);
                    _toaster.ToastInfo("文件删除成功");
                    return true;
                }
                else
                {
                    _toaster.ToastInfo("文件 " + fileToDelete + "删除失败: 文件不存在");
                    return false;

                }
            }
            catch (Exception)
            {
                _toaster.ToastInfo("文件 " + fileToDelete + " 删除失败: IO错误");
                return false;
            }
        }

        public bool DeleteImgDir(string path)
        {
            return DeleteDir(ImageUploadPath + path);
        }

        public bool DeleteFileDir(string path)
        {
            return DeleteDir(FileUploadPath + path);
        }

        public bool DeleteDir(string path)
        {
            try
            {
                Directory.Delete(path, true);
                _toaster.ToastInfo("文件夹 " + path + " 删除成功");
                return true;
            }
            catch (Exception)
            {
                _toaster.ToastInfo("文件夹 " + path + " 删除失败: IO错误");
                return false;
            }
        }
    }
}
