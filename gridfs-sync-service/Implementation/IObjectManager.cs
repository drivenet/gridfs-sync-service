﻿using System.IO;
using System.Threading.Tasks;

namespace GridFSSyncService.Implementation
{
    internal interface IObjectManager
    {
        Task Upload(string objectName, Stream readOnlyInput);

        Task Delete(string objectName);

        Task Flush();
    }
}
