using Discord;
using log4net;
using LoggerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLibrary;

namespace CompressionLibrary.Validators
{
    internal struct ImageCompressorValidator
    {
        private static readonly ILog _logger = LogAsync.GetLogger();
        public ValidatorResponse ValidationReponse { get; private set; } = new ValidatorResponse();

        public ImageCompressorValidator()
        {

        }

        internal async Task LogResponse(ValidatorResponse response)
        {
            string msg;
            switch (response.Response)
            {
                case ResponseType.InvalidImageExtension:
                    msg = "Image extension is invalid.";
                    await _logger.ErrorAsync(msg);
                    break;
                case ResponseType.FileDoesNotExist:
                    msg = "One or more files does not exist.";
                    await _logger.ErrorAsync(msg);
                    break;
                case ResponseType.ListEmptyOrNull:
                    msg = "Provided list is empty and/or null";
                    await _logger.ErrorAsync(msg);
                    break;
                default:
                    break;
            }
        }

        internal async Task<bool> AreImagePathsValidAsync(List<string> imagePaths)
        {
            var boolResult = ListUtils.AnyOrFalse(imagePaths);
            if (!boolResult)
            {
                ValidationReponse.SetResponse(ResponseType.ListEmptyOrNull);
                return (boolResult);
            }
            else
            {
                foreach (var imagePath in imagePaths)
                {
                    if (File.Exists(imagePath))
                    {
                        (boolResult, ValidationReponse) = await IsImageExtensionValid(imagePath);
                    }
                    else
                    {
                        ValidationReponse.SetResponse(ResponseType.FileDoesNotExist);
                        return (boolResult = false);
                    }
                }
            }

            ValidationReponse.SetResponse(ResponseType.Valid);
            return (boolResult);
        }

        private static Task<(bool, ValidatorResponse)> IsImageExtensionValid(string imagePath)
        {

            //TODO: Figure out a way to do this without hardcoding the extensions.
            var fileExtension = Path.GetExtension(imagePath);
            if (fileExtension != ".png" || fileExtension  != ".jpg")
            {
                return Task.FromResult((false, new ValidatorResponse(ResponseType.InvalidImageExtension)));
            }
            else
            {
                return Task.FromResult((true, new ValidatorResponse(ResponseType.Valid)));
            }
        }





    }
}
