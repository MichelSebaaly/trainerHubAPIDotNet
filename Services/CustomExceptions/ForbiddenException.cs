using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CustomExceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Access denied") : base(message) { }
    }
}
