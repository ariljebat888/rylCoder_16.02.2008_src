Imports System
Imports System.IO
Imports System.Drawing.Imaging

Public Class CMiniMap
    ' Fields
    Private Const FileMask As String = "{0}\Texture\Widetexture\Zone{1}\{2}_{3}.dds"
    Public Const TilesPainted As Integer = 11

    ' Methods
	Public Shared Function CreateMap(ByVal GameFolder As String, ByVal Zone As Integer) As Bitmap
        Try
            If Zone = 16 Then Zone = 8
            Return FischR.Wrapper.LoadMapDDS(String.Format(FileMask, GameFolder, Zone, "{0}", "{1}"))
        Catch ex As Exception
            MessageBox.Show(ex.ToString, ex.Message)
            Return Nothing
        End Try
	End Function
End Class
