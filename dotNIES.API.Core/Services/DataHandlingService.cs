using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.API.Core.Services;

/// <summary>
/// This base class is used to handle the data connection. It contains generic methods for retreiving and manipulate data.
/// </summary>
/// <param name="connectieString"></param>
public sealed class DataHandlingService(string connectieString)
{
    private readonly string _connectieString = connectieString;


}

