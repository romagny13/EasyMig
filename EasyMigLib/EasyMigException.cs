using System;

namespace EasyMigLib
{
    public class EasyMigException : Exception
    {
        public EasyMigException(string message) 
            : base(message)
        { }
    }
}
