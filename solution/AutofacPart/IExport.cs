using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    interface IExport
    {
        void Register(ContainerBuilder builder);
    }
}
