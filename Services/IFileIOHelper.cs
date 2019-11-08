using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace lonefire.Services
{
    public interface IFileIoHelper
    {
        //Save to Image upload directory

        /// <summary>
        /// Save Image with random name and custom path and name length limit(defaults to 256)
        /// </summary>
        Task<string> SaveImgAsync(IFormFile img, string savePath, int imgNameLengthLimit = 256); 

        /// <summary>
        /// Save Image with custom name and custom path and name length limit
        /// </summary>
        Task<string> SaveImgAsync(IFormFile img, string savePath, int imgNameLengthLimit, string imgName);

        /// <summary>
        /// Delete Image only 
        /// </summary>
        bool DeleteImg(string imgToDeletePath, string imgToDelete, int imgNameLengthLimit = 256);

        //Save to File upload directory without regex validation
        Task<string> SaveFileAsync(IFormFile file, string savePath); //256 default limit
        Task<string> SaveFileAsync(IFormFile file, string savePath, int fileNameLengthLimit); //random name
        Task<string> SaveFileAsync(IFormFile file, string savePath, int fileNameLengthLimit, string saveFileName);

        //Delete file without regex check
        bool DeleteFile(string fileToDeletePath, string fileToDelete); //256 default limit
        bool DeleteFile(string fileToDeletePath, string fileToDelete, int fileNameLengthLimit);

        bool MoveImgDir(string src, string dst);
        bool MoveFileDir(string src, string dst);

        bool DeleteImgDir(string path);
        bool DeleteFileDir(string path);
        bool DeleteDir(string path);

        //RAW File Operations

        /// <summary>
        /// Move or Rename a directory with 512 or less name length
        /// </summary>
        bool MoveDir(string src, string dst);

        /// <summary>
        /// Save file with random name
        /// returns: Succeed-> (string) fileName.ext 
        ///          Failed-> null
        /// </summary>
        Task<string> CopyToAsync(IFormFile file, string savePath, string validateRegex, int fileNameLengthLimit);

        //Save file with specified filename
        //returns: Succeed-> (string) fileName.ext 
        //         Failed-> null
        Task<string> CopyToAsync(IFormFile file, string savePath, string validateRegex, int fileNameLengthLimit, string saveFileName);

        //Delete file with filename.ext & path
        //returns: Succeed-> true
        //         Failed-> false
        bool Delete(string fileToDeletePath, string fileToDelete, string validateRegex, int fileNameLengthLimit);

    }
}
