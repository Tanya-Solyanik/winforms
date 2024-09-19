// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.BinaryFormat;
using System.Windows.Forms.Nrbf;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        internal static class BinaryFormatUtilities
        {
            internal static void WriteObjectToStream(MemoryStream stream, object data, bool restrictSerialization)
            {
                long position = stream.Position;
                bool success = false;

                try
                {
                    success = WinFormsBinaryFormatWriter.TryWriteCommonObject(stream, data);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Being extra cautious here, but the Try method above should never throw in normal circumstances.
                    Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
                }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
                if (!success)
                {
                    // This check is to help in trimming scenarios with a trim warning on a call to
                    // BinaryFormatter.Serialize(), which has a RequiresUnreferencedCode annotation.
                    // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Serialize(),
                    // will not be called,
                    // If the flag is true, the trimmer will generate a warning for calling a method that has a
                    // RequiresUnreferencedCode annotation.
                    if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                    {
                        throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                    }

                    stream.Position = position;
                    new BinaryFormatter()
                    {
                        Binder = restrictSerialization ? new BitmapBinder() : null
                    }.Serialize(stream, data);
                }
#pragma warning restore SYSLIB0011
            }

            internal static object? ReadObjectFromStream<T>(
                MemoryStream stream,
                Func<TypeName, Type> resolver,
                bool restrictDeserialization,
                bool legacyMode)
            {
                long startPosition = stream.Position;
                // TanyaSo: verify that resolver is set.
                SerializationBinder binder = restrictDeserialization
                    ? new BitmapBinder()
                    : new DataObject.Composition.Binder(typeof(T), resolver, legacyMode);
                // TanyaSo - pass in user-provided resolver into the TRS that hydrates records
                // BinaryFormattedObject.Options options = new()
                // {
                //    Binder = binder
                // };

                SerializationRecord? record;
                try
                {
                    record = stream.Decode();

                    if (record.TryGetCommonObject<T>(out object? value))
                    {
                        return value.GetType().IsAssignableTo(typeof(T)) ? value : null;
                    }

                    if (!legacyMode)
                    {
                        if (!record.TypeNameMatches(typeof(T)))
                        {
                            Type type = resolver(record.TypeName)
                                ?? throw new NotSupportedException($"'resolver' function provided in '{nameof(Clipboard.TryGetData)}'" +
                                    $" method should never return a null. It should throw a '{nameof(NotSupportedException)}' when encountering unsupported types.");

                            if (!type.IsAssignableTo(typeof(T)))
                            {
                                return null;
                            }
                        }

                        //// TanyaSo - add type resolution and dehydration logic from the BFO code.
                        // if (record.TryGetObject<T>(out value))
                        // {
                        //    Debug.Assert(value is T, $"NRBF throws in case of corrupted or unsupported data," +
                        //        $" it should not deserialize an unexpected type.");
                        //    return value;
                        // }
                    }

                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // TanyaSO offset array case breaks stream.Decode?
                    // Couldn't parse for some reason, let the BinaryFormatter try to handle it.
                }

                if (legacyMode || LocalAppContextSwitches.ClipboardEnableUnsafeBinaryFormatterDeserialization)
                {
                    stream.Position = startPosition;
                    return ReadObjectWithBinaryFormatter<T>(stream, binder);
                }

                return null;
            }

            private static object? ReadObjectWithBinaryFormatter<T>(MemoryStream stream, SerializationBinder binder)
            {
                // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Deserialize(),
                // which has a RequiresUnreferencedCode annotation.
                // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Deserialize() will not be called,
                // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                {
                    throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                }

#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                // cs/dangerous-binary-deserialization
                return new BinaryFormatter()
                {
                    Binder = binder,
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                }.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior.
#pragma warning restore SYSLIB0050
#pragma warning restore SYSLIB0011
            }
        }
    }
}
