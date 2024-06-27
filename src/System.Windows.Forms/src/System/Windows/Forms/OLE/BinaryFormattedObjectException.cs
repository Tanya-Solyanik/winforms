// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataObject
{
    internal partial class ComposedDataObject
    {
        private partial class NativeDataObjectToWinFormsAdapter
        {
            private class BinaryFormattedObjectException : Exception
            {
                public BinaryFormattedObjectException(NotSupportedException nse) : base(nse.Message, nse)
                {
                }
            }
        }
    }
}
