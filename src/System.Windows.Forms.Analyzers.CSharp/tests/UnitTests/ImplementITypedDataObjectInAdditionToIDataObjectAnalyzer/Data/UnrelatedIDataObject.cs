using System;
using System.Runtime.Versioning;

[assembly: TargetFramework(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]

namespace System.Windows.Forms.Analyzers.CSharp.Tests.ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer.Data
{
    internal interface IDataObject
    {
        void DoStuff();
    }

    internal class UnrelatedIDataObject : IDataObject
    {
        public void DoStuff() => throw new NotImplementedException();
    }
}
