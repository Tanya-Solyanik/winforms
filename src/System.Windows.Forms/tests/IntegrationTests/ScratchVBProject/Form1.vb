' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.IO
Imports System.Reflection

' As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
<DesignerCategory("Default")>
Partial Public Class Form1
    Inherits Form

    Public Sub New()
        InitializeComponent()

        Dim writes As Action() =
        {
            AddressOf ClipboardSetData
        }

        Dim x As Integer = 40
        Dim y As Integer = 10
        Dim button As Button
        For Each write As Action In writes
            button = New Button With {
                .Text = write.GetMethodInfo().Name,
                .Location = New Point(x, y),
                .Size = New Size(150, 50)
            }

            AddHandler button.Click, Sub(sender, e) write()
            Controls.Add(button)

            y += 60
        Next

        Dim reads As Action() =
        {
            AddressOf ClipboardTryGetData
        }

        x += 190
        y = 10
        For Each read As Action In reads
            button = New Button With {
                .Text = read.GetMethodInfo().Name,
                .Location = New Point(x, y),
                .Size = New Size(150, 50)
            }

            AddHandler button.Click, Sub(sender, e) read()
            Controls.Add(button)

            y += 60
        Next

        Dim roundtrips As Action() =
        {
            AddressOf RoundTripCustomFormats,
            AddressOf RoundTripPenData
        }

        x += 190
        y = 10
        For Each roundtrip As Action In roundtrips
            button = New Button With {
                .Text = roundtrip.GetMethodInfo().Name,
                .Location = New Point(x, y),
                .Size = New Size(150, 50)
            }

            AddHandler button.Click, Sub(sender, e) roundtrip()
            Controls.Add(button)

            y += 60
        Next
    End Sub

    ' Reads
    Private Sub ClipboardTryGetData()
        Dim MyData As TestData = Nothing
        If Clipboard.TryGetData(Of TestData)("custom", MyData) Then
            Text = $"{MyData._text1} {MyData._text2}"
        Else
            Text = "Could not retrieve data off the clipboard."
        End If

        'Dim MyData As Bitmap = Nothing
        'If Clipboard.TryGetData(Of Bitmap)(MyData) Then
        '    Text = "got bitmap"
        'Else
        '    Text = "Could not retrieve data off the clipboard."
        'End If
    End Sub

    ' Writes
    Private Sub ClipboardSetData()
        Dim data As New TestData("Hello", "World")
        Clipboard.SetData("TestData", data) ' this helper method avoids specifying the format
        Text = $"format {GetType(TestData).FullName}"

        ' Clipboard.SetData(DataFormats.Bitmap, "D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg")
        ' Text = "Bitmap"
    End Sub

    ' RoundTrips
    Public Sub RoundTripPenData()
        Dim f1 As New Font("Microsoft Sans Serif", 10)
        Clipboard.SetData(DataFormats.PenData, f1)
        Dim o As Object = Clipboard.GetData(DataFormats.PenData)
        Dim f As Font = TryCast(o, Font)
    End Sub

    Public Sub RoundTripCustomFormats()
        Dim myTextFormat As String = "MyTextFormat"
        Dim myBlobFormat As String = "MyBlobFormat"
        Dim guid As Guid = Guid.NewGuid()
        Dim myText As String = guid.ToString()
        Dim myBlob As New MemoryStream(guid.ToByteArray())
        Dim unicodeText As String = "Euro char: \u20AC"
        Dim data As New DataObject()
        data.SetData(myTextFormat, myText)
        data.SetData(myBlobFormat, myBlob)
        data.SetText(unicodeText)
        Clipboard.SetDataObject(data)
        Dim copiedDataObject As IDataObject = Clipboard.GetDataObject()
        Dim copiedText As String = TryCast(copiedDataObject?.GetData(myTextFormat), String)
        Dim copiedBlob As MemoryStream = TryCast(copiedDataObject?.GetData(myBlobFormat), MemoryStream)
        Dim copiedUnicodeText As String = Clipboard.GetText()
    End Sub

End Class
