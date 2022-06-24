using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsLibrary;

namespace CompressionLibrary.Validators
{
    internal class ImageCompressorValidator
    {


        internal async Task<(bool, ValidatorResponse)> AreImagePathsValidAsync(List<string> imagePaths)
        {
            var boolResult = ListUtils.AnyOrFalse(imagePaths);
            var responseResult = new ValidatorResponse();

            if (!boolResult)
            {
                return (boolResult, new ValidatorResponse());
            }
            else
            {
                foreach (var imagePath in imagePaths)
                {

                    boolResult = File.Exists(imagePath) ? await IsImageExtensionValid(imagePath) : false;
                }
            }

            return (boolResult, new ValidatorResponse());


        }


        private Task<bool> IsImageExtensionValid(string imagePath)
        {
            //TODO: Figure out a way to do this without hardcoding the extensions.
            var fileExtension = Path.GetExtension(imagePath);
            return fileExtension != ".png" || fileExtension != ".jpg" ? Task.FromResult(false) : Task.FromResult(true);
        }





    }
}
