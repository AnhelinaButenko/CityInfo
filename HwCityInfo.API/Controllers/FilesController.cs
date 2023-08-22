﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace HwCityInfo.API.Controllers;

[Route("api/files")]
[Authorize]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(
        FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
            ?? throw new System.ArgumentException(
                nameof(fileExtensionContentTypeProvider));
    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId) 
    {
        //look up the actual file, depending on the fileId
        var pathToFile = "getting-started-with-rest-slides.pdf";

        // check whether the file exists if it doesn`t we return 
        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound(); 
        }

        //try to find correct file
        if (!_fileExtensionContentTypeProvider.TryGetContentType(
            pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        //read file
        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
}
