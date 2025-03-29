using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using GestionFM1.Core.Models;
using GestionFM1.Read.QueryDataStore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExcelController : ControllerBase // Nom du contrôleur mis à jour
    {
        private readonly QueryDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<ImportExcelController> _logger;  // Logger

        public ImportExcelController(QueryDbContext context, IWebHostEnvironment hostEnvironment, ILogger<ImportExcelController> logger) // Logger
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger; // Logger
        }

        // Méthodes pour ExcelFm1
        [HttpPost("upload-fm1")]
        public async Task<IActionResult> UploadExcelFm1File(IFormFile file)
        {
             _logger.LogInformation("UploadExcelFm1File invoked.");
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            if (string.IsNullOrEmpty(file.FileName))
            {
                 _logger.LogWarning("File name is required.");
                return BadRequest("File name is required.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var excelFm1Entries = new List<ExcelFm1>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var siteCode = worksheet.Cells[row, 1].Value?.ToString();
                    var typeDevice = worksheet.Cells[row, 2].Value?.ToString();
                    var snPs = worksheet.Cells[row, 3].Value?.ToString();

                    var excelFm1 = new ExcelFm1
                    {
                        Id = Guid.NewGuid(),
                        SiteCode = siteCode ?? "",
                        TypeDevice = typeDevice ?? "",
                        SnPs = snPs ?? ""
                    };

                    excelFm1Entries.Add(excelFm1);
                }
            }

            _context.ExcelFm1s.AddRange(excelFm1Entries);
            await _context.SaveChangesAsync();

             _logger.LogInformation("File uploaded and data saved successfully.");
            return Ok("File uploaded and data saved successfully.");
        }

        [HttpGet("get-all-fm1")]
        public async Task<IActionResult> GetAllExcelFm1()
        {
             _logger.LogInformation("GetAllExcelFm1 invoked.");
            var excelFm1Entries = await _context.ExcelFm1s.ToListAsync();
             _logger.LogInformation($"Retrieved {excelFm1Entries.Count} ExcelFm1 entries.");
            return Ok(excelFm1Entries);
        }

       [HttpPost("upload-composent")]
public async Task<IActionResult> UploadExcelComposentFile(IFormFile file)
{
     _logger.LogInformation("UploadExcelComposentFile invoked.");
    if (file == null || file.Length == 0)
    {
        _logger.LogWarning("No file uploaded.");
        return BadRequest("No file uploaded.");
    }

    if (string.IsNullOrEmpty(file.FileName))
    {
        _logger.LogWarning("File name is required.");
        return BadRequest("File name is required.");
    }

    // Utiliser un dossier personnalisé pour les uploads
    var currentDirectory = Directory.GetCurrentDirectory();

    var uploadsFolder = Path.Combine(currentDirectory, "Uploads");

    if (!Directory.Exists(uploadsFolder))
    {
        Directory.CreateDirectory(uploadsFolder);
    }

    var filePath = Path.Combine(uploadsFolder, file.FileName);

    try
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var excelComposents = new List<ExcelComposent>();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var itemBaseId = worksheet.Cells[row, 1].Value?.ToString();
                var productName = worksheet.Cells[row, 2].Value?.ToString();
                var serialNumber = worksheet.Cells[row, 3].Value?.ToString();
                var totalAvailable = worksheet.Cells[row, 4].Value?.ToString();

                var excelComposent = new ExcelComposent
                {
                    Id = Guid.NewGuid(), // Générer un nouvel ID Guid
                    AnComposent = itemBaseId ?? "",
                    ComposentName = productName ?? "",
                    SnComposent = serialNumber ?? "",
                    TotalAvailable = string.IsNullOrEmpty(totalAvailable)
                        ? 0
                        : double.Parse(totalAvailable.Replace(",", "."), CultureInfo.InvariantCulture)
                };

                excelComposents.Add(excelComposent);
            }
        }

        _context.ExcelComposents.AddRange(excelComposents);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File uploaded and data saved successfully.");
        return Ok("File uploaded and data saved successfully.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Internal server error occurred during file upload.");
        return StatusCode(500, "Internal server error. Please try again later.");
    }
}
        [HttpGet("get-all-composent")]
        public async Task<IActionResult> GetAllExcelComposent()
        {
              _logger.LogInformation("GetAllExcelComposent invoked.");
            var excelComposents = await _context.ExcelComposents.ToListAsync();
            _logger.LogInformation($"Retrieved {excelComposents.Count} ExcelComposent entries.");
            return Ok(excelComposents);
        }
    }
}