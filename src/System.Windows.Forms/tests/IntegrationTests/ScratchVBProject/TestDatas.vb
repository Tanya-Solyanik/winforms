' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
Imports System.Runtime.Serialization

<Serializable>
Friend NotInheritable Class TestData
    Public Sub New(text1 As String, text2 As String)
        _text1 = text1
        _text2 = text2
    End Sub

    Public _text1 As String
    Public _text2 As String
End Class

<Serializable>
Friend NotInheritable Class TestData1
    Public Sub New(point As PointF)
        _point = point
    End Sub

    Public _point As PointF
End Class

<Serializable>
Friend NotInheritable Class TestData2
    Public Sub New(streamer As ImageListStreamer)
        _streamer = streamer
    End Sub

    Public _streamer As ImageListStreamer
End Class

<Serializable>
Friend NotInheritable Class TestData3
    Public Sub New(bitmap As Bitmap)
        _bitmap = bitmap
    End Sub

    Public _bitmap As Bitmap

    Friend NotInheritable Class InnerData
        Public Sub New(text As String)
            _text = text
        End Sub

        Public _text As String
    End Class
End Class

<Serializable>
Public NotInheritable Class TestData4
    Implements ISerializable

    Private ReadOnly _n1 As Integer

    Public Sub New()
        _n1 = -1
    End Sub

    Private Sub New(info As SerializationInfo, context As StreamingContext)
        _n1 = If(info.GetValue(NameOf(_n1), GetType(Integer)) IsNot Nothing, CInt(info.GetValue(NameOf(_n1), GetType(Integer))), -1)
    End Sub

    ' The following method serializes the instance.
#Disable Warning SYSLIB0050 ' Type or member is obsolete
    Private Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
#Enable Warning SYSLIB0050 ' Type or member is obsolete
        info.AddValue(NameOf(_n1), _n1)
    End Sub
End Class

