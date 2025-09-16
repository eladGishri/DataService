using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
    public class UpdateOperationFailedException : Exception
    {
        public UpdateOperationFailedException(string message) : base(message)
        {

        }
    }
}
