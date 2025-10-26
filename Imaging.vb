Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Namespace Support
    Public Class Imaging
        Public Shared Function RotateImage(ByVal img As Image, ByVal angle As Single) As Bitmap
            Dim n9, n10, n11, n12 As Double
            If img Is Nothing Then Return Nothing

            Dim width As Double = img.Width
            Dim height As Double = img.Height
            Dim n3 As Double = (angle * Math.PI) / 180
            Dim d As Double = n3
            Do While (d < 0)
                d += 2 * Math.PI
            Loop
            If (((d >= 0) AndAlso (d < Math.PI / 2)) OrElse ((d >= Math.PI) AndAlso (d < Math.PI * 1.5))) Then
                n9 = Math.Abs(Math.Cos(d)) * width
                n10 = Math.Abs(Math.Sin(d)) * width
                n11 = Math.Abs(Math.Cos(d)) * height
                n12 = Math.Abs(Math.Sin(d)) * height
            Else
                n9 = Math.Abs(Math.Sin(d)) * height
                n10 = Math.Abs(Math.Cos(d)) * height
                n11 = Math.Abs(Math.Sin(d)) * width
                n12 = Math.Abs(Math.Cos(d)) * width
            End If
            Dim a As Double = n9 + n12
            Dim n6 As Double = n11 + n10
            Dim n7 As Integer = Math.Ceiling(a)
            Dim n8 As Integer = Math.Ceiling(n6)
            Dim bmp As Bitmap = New Bitmap(n7, n8)
            Using gr As Graphics = Graphics.FromImage(bmp)
                Dim pointArray() As Point
                If ((d >= 0) AndAlso (d < Math.PI / 2)) Then
                    pointArray = New Point() {New Point(n12, 0), New Point(n7, n10), New Point(0, n11)}
                ElseIf ((d >= Math.PI / 2) AndAlso (d < Math.PI)) Then
                    pointArray = New Point() {New Point(n7, n10), New Point(n9, n8), New Point(n12, 0)}
                ElseIf ((d >= Math.PI) AndAlso (d < Math.PI * 1.5)) Then
                    pointArray = New Point() {New Point(n9, n8), New Point(0, n11), New Point(n7, n10)}
                Else
                    pointArray = New Point() {New Point(0, n11), New Point(n12, 0), New Point(n9, n8)}
                End If
                gr.DrawImage(img, pointArray)
            End Using
            Return bmp
        End Function

        Public Shared Sub BmpAddFast(ByVal from As Bitmap, ByVal addTo As Bitmap, ByVal area As Rectangle)
            Dim bmpData As BitmapData = addTo.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)
            Dim destination(bmpData.Stride * bmpData.Height) As Byte
            Marshal.Copy(bmpData.Scan0, destination, 0, destination.Length)

            Dim bmpData2 As BitmapData = from.LockBits(New Rectangle(0, 0, from.Width, from.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
            Dim source(bmpData2.Stride * bmpData2.Height) As Byte
            Marshal.Copy(bmpData2.Scan0, source, 0, source.Length)
            from.UnlockBits(bmpData2)

            Dim cnt As Integer = 0
            For i As Integer = from.Height - 1 To 0 Step -1
                For j As Integer = 0 To from.Width - 1 Step 1
                    destination(cnt) = source(cnt)
                    cnt += 1
                    destination(cnt) = source(cnt)
                    cnt += 1
                    destination(cnt) = source(cnt)
                    cnt += 1
                Next
            Next
            Marshal.Copy(destination, 0, bmpData.Scan0, destination.Length)
            addTo.UnlockBits(bmpData)
        End Sub

        Public Shared Function BmpGetRegion(ByVal from As Bitmap, ByVal area As Rectangle) As Bitmap

            Dim bgra As New Color()
            Dim addTo As New Bitmap(area.Width, area.Height, PixelFormat.Format24bppRgb)
            Dim bmpData As BitmapData = addTo.LockBits(New Rectangle(0, 0, area.Width, area.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)
            Dim destination(bmpData.Stride * bmpData.Height) As Byte
            Marshal.Copy(bmpData.Scan0, destination, 0, destination.Length)
            Dim cnt As Integer = 0
            For i As Integer = area.Y To from.Height - 1 OrElse area.Bottom - 1 Step -1
                For j As Integer = area.X To from.Width - 1 OrElse area.Right - 1 Step 1
                    bgra = from.GetPixel(j, i)
                    destination(cnt) = bgra.B
                    cnt += 1
                    destination(cnt) = bgra.G
                    cnt += 1
                    destination(cnt) = bgra.R
                    cnt += 1
                Next
            Next
            Marshal.Copy(destination, 0, bmpData.Scan0, destination.Length)
            addTo.UnlockBits(bmpData)
            Return addTo
        End Function
    End Class
End Namespace