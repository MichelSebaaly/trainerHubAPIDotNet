using System;
using System.Collections.Generic; 

namespace Services.CustomExceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Not Found") : base(message) { }
    }
}
