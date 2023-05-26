using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.EF;
using webapi.Models;

namespace webapi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [DisableRequestSizeLimit]
    public class FileUploadController : ControllerBase
    {
        ValidationViewModel _validationViewModel;
        private CustomerContext _customerContext;

        private readonly IConfiguration _configuration;

        public FileUploadController(CustomerContext customerContext, IConfiguration configuration)
        {
            _validationViewModel = new ValidationViewModel();
            _customerContext = customerContext;
            _configuration = configuration;
        }

        [HttpPost("ImportFile")]
        public async Task<IActionResult> ImportFile([FromForm] IFormFile file)
        {
            ResponseContext responseData = new ResponseContext();
            try
            {
                //Perform basic validations
                //check if file is present
                if (file != null)
                {
                    string extension = Path.GetExtension(file.FileName);

                    //check if it's a CSV file
                    if (extension == ".csv")
                    {
                        //read the file
                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            System.IO.File.WriteAllBytes(@"c:\home\bu\" + file.FileName, memoryStream.ToArray());
                            responseData.IsSuccess = true;
                        }
                    }
                    else
                    {
                        //Handle other file types
                        responseData.ErrorMessage = "Only CSV Files are allowed! Please choose a .csv file and try again!";
                        responseData.IsSuccess = false;
                    }
                }
                else
                {
                    responseData.ErrorMessage = "No file rececived at the server!";
                    responseData.IsSuccess = false;
                }
                //do something with the file here'
            }
            catch (Exception ex)
            {
                responseData.ErrorMessage = ex.Message;
                responseData.IsSuccess = false;

            }
            return Ok(responseData);
        }

        [HttpPost("/ProcessFile")]
        public async Task<IActionResult> ProcessFile([FromQuery] string fileName = "")
        {
            ResponseContext responseContext = new ResponseContext();
            CSVHelper cSVHelper = new CSVHelper();

            if (!string.IsNullOrEmpty(fileName))
            {
                _validationViewModel = cSVHelper.Validate($@"c:\home\bu\{fileName}");

                try
                {
                    if (_validationViewModel.Details != null)
                    {
                        string connstr = _configuration.GetConnectionString("Azure-DB");


                        AzureHelper azureHelper = new AzureHelper(_configuration);
                        var isSuccess = azureHelper.SQLBulkInsertion(_validationViewModel.Details);

                        this._customerContext.Database.SetCommandTimeout(1000);
                        //_customerContext.Details.AddRange(_validationViewModel.Details);
                        var start = DateTime.Now;
                        _customerContext.BulkInsert(_validationViewModel.Details);
                        var end = DateTime.Now;
                        var difference = (end - start).TotalSeconds;
                        //_customerContext.SaveChanges();
                        responseContext.IsSuccess = true;
                        responseContext.ErrorMessage = string.Empty;
                    }
                    if (_validationViewModel.ValidationModel != null)
                    {
                        _validationViewModel.ValidationModel.CSVFile = fileName;
                        _customerContext.Validation.AddRange(_validationViewModel.ValidationModel);
                        _customerContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    responseContext.IsSuccess = false;
                    responseContext.ErrorMessage = ex.Message;
                }
            }
            return Ok(responseContext);
        }

        [HttpGet("/GetData")]
        public async Task<IActionResult> GetValidationData(string fileName)
        {
            ResponseContext responseContext = new ResponseContext();

            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    responseContext.Data = _customerContext.Validation.Where(r => r.CSVFile == fileName).OrderByDescending(r => r.DateOfUpload).FirstOrDefault();
                    responseContext.IsSuccess = true;
                }
                catch
                {
                    responseContext.IsSuccess = false;
                    responseContext.ErrorMessage = "Unable to fetch validation data.";
                }
            }
            else
            {
                responseContext.IsSuccess = false;
                responseContext.ErrorMessage = "Not a valid file!";
            }
            return Ok(responseContext);
        }
    }
}