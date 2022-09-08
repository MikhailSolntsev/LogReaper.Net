using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReaper.Net.Exceptions;

public class FileRenameException : Exception
{
    public FileRenameException(string? message) : base(message)
    {
    }
}
