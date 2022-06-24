using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary.Validators
{
    internal class ValidatorResponse 
    {
        private List<ResponseType> responses = new();

        internal ValidatorResponse()
        {

        }

        internal void SetResponse(ResponseType responseType)
        {
            responses.Add(responseType);
        }

        internal bool IsResponseValid()
        {
            var result = true;
            foreach (var response in responses)
            {
                if (response != ResponseType.Valid)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }


    }
}
