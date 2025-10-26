Imports System.ComponentModel
Imports System.Drawing

Public Class CMiniMapPanel
    Inherits Panel

    ' Fields
    Private WithEvents worker As New BackgroundWorker
    Public gameFld As String = ""
    Private locD As Single
    Private locX As Single
    Private locY As Single
    Public map As Bitmap = Nothing
    Private mp As Panel
    Private notAvailableL As Label
    Private Shared npcColor As Color = Color.Violet
    Private Const npcWidth As Integer = 2
    Public openZone As Integer = 0
    Public origArrow As Bitmap = Nothing
    Private pleaseWaitL As Label
    Private waitingForDraw As Boolean
    Public Shadows Event Click(ByVal sender As Object, ByVal e As EventArgs)
    ' Nested Types
    Private Structure workerArgs
        Public zone As Integer
        Public folder As String
        Public npcs As CNpcParser.npcStruct()
        Public Shared Function Create(ByVal gamefolder As String, ByVal zone As Integer, Optional ByRef npcs As CNpcParser.npcStruct() = Nothing) As workerArgs
            Dim args2 As New workerArgs
            args2.zone = zone
            args2.folder = gamefolder
            args2.npcs = npcs
            Return args2
        End Function
    End Structure

    ' Methods
    Public Sub New()
        Me.pleaseWaitL = New Label
        Me.notAvailableL = New Label
        Me.waitingForDraw = False
        Me.mp = New Panel
        Dim size As New Size(200, 200)
        Me.Size = size
        Me.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        Me.Visible = False
        Me.mp.BackgroundImage = Nothing
        Me.mp.BackColor = Color.Transparent
        Me.Controls.Add(Me.mp)
        Me.pleaseWaitL.AutoSize = True
        Me.pleaseWaitL.Font = New Font("Comic Sans MS Bold", 18.0!, FontStyle.Italic, GraphicsUnit.Pixel)
        Me.pleaseWaitL.Text = "Please wait..."
        Me.pleaseWaitL.BackColor = Color.Transparent
        Me.pleaseWaitL.ForeColor = Color.BlueViolet
        Dim point As New Point(&H23, 40)
        Me.pleaseWaitL.Location = point
        Me.pleaseWaitL.Visible = False
        Me.Controls.Add(Me.pleaseWaitL)
        Me.notAvailableL.AutoSize = True
        Me.notAvailableL.Font = New Font("Comic Sans MS Bold", 18.0!, FontStyle.Italic, GraphicsUnit.Pixel)
        Me.notAvailableL.Text = "Map Not available"
        Me.notAvailableL.BackColor = Color.Transparent
        Me.notAvailableL.ForeColor = Color.Red
        point = New Point(20, 40)
        Me.notAvailableL.Location = point
        Me.notAvailableL.Visible = False
        Me.Controls.Add(Me.notAvailableL)

        For Each c As Control In Me.Controls
            AddHandler c.Click, AddressOf subClick
        Next
    End Sub
    Private Sub subClick(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Click
        RaiseEvent Click(Me, e)
    End Sub
    Private Sub drawLoc()
        Me.mp.Visible = True
        Dim pnt As Point = MyPoint.Cord2Loc(New Point(Me.locX, Me.locY), Me.map.Width)
        Dim img As New Bitmap(Me.Width, Me.Height, Me.map.PixelFormat)
        Dim graphs As Graphics = Graphics.FromImage(img)
        Dim destRect As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim srcRect As New Rectangle(pnt.X - Me.Height / 2, pnt.Y - Me.Height / 2, Me.Width, Me.Height)
        graphs.DrawImage(Me.map, destRect, srcRect, GraphicsUnit.Pixel)
        Me.BackgroundImage = img
    End Sub



    Public Sub drawLocation(ByVal x As Single, ByVal y As Single, ByVal dir As Single)
        If openZone < 1 OrElse gameFld = String.Empty Then Return
        If (((x <> Me.locX) OrElse (y <> Me.locY)) OrElse (Me.locD <> dir)) Then
            If dir < 0.0! AndAlso Me.locD <> dir Then
                Me.origArrow = Global.rylCoder.My.Resources.Xa_mark ' for teleports
            ElseIf Me.locD <> dir Then
                Try
                    Me.origArrow = FischR.Wrapper.LoadDDS(Me.gameFld & "\Texture\Interface\mmap01.dds", New Rectangle(&H80, &H15, &H16, &H30))
                Catch ex As Exception
                    Me.origArrow = Global.rylCoder.My.Resources.Xa_mark
                End Try
                If Me.origArrow Is Nothing Then Me.origArrow = Global.rylCoder.My.Resources.Xa_mark
            End If
            If ((x = Me.locX) AndAlso (y = Me.locY)) Then
                Me.locD = dir
                Me.setRot()
            Else
                Me.Visible = True
                Me.BackgroundImage = Nothing
                Me.locX = x
                Me.locY = y
                If (dir <> Me.locD) Then
                    Me.locD = dir
                    Me.setRot()
                End If
                Me.locD = dir
                If (Not Me.worker.IsBusy AndAlso (Not Me.map Is Nothing)) Then
                    Me.drawLoc()
                ElseIf Not Me.worker.IsBusy Then
                    Me.notAvailableL.Visible = True
                Else
                    Me.waitingForDraw = True
                End If
            End If
        Else
            Me.Visible = True
        End If
    End Sub

    Public Sub openMap(ByVal gamefolder As String, ByVal zone As Integer, Optional ByRef npcs As CNpcParser.npcStruct() = Nothing)
        If ((Me.map Is Nothing) OrElse (zone <> Me.openZone)) Then
            Me.gameFld = gamefolder
            Me.openZone = zone
            If Me.worker.IsBusy Then
                Me.worker = New BackgroundWorker
            End If
            If (Me.origArrow Is Nothing) Then
                Try
                    Me.origArrow = FischR.Wrapper.LoadDDS(Me.gameFld & "\Texture\Interface\mmap01.dds", New Rectangle(&H80, &H15, &H16, &H30))
                Catch ex As Exception
                    Me.origArrow = Global.rylCoder.My.Resources.Xa_mark
                End Try
                If Me.origArrow Is Nothing Then Me.origArrow = Global.rylCoder.My.Resources.Xa_mark
            End If
            Me.pleaseWaitL.Visible = True
            Me.notAvailableL.Visible = False
            Me.mp.Visible = False
            Me.worker.RunWorkerAsync(workerArgs.Create(gamefolder, zone, (npcs)))
        End If
    End Sub

    Private Sub setRot()
        Dim num As Double = (locD / Math.PI) * 180.0!
        If (num >= 360) Then num = (num - 360)
        If num < 0 Then num = num + 360

        Me.mp.BackgroundImage = Support.Imaging.RotateImage(Me.origArrow, num)
        Me.mp.Size = Me.mp.BackgroundImage.Size
        Dim point As New Point(CInt(Math.Round(CDbl(((CDbl(Me.Width) / 2) - (CDbl(Me.mp.Width) / 2))))), CInt(Math.Round(CDbl(((CDbl(Me.Height) / 2) - (CDbl(Me.mp.Height) / 2))))))
        Me.mp.Location = point
    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles worker.DoWork
        Dim wk As workerArgs = e.Argument
        Dim bmp As Bitmap = CMiniMap.CreateMap(wk.folder, wk.zone)
        If Not wk.npcs Is Nothing AndAlso Not bmp Is Nothing Then
            For Each npc As CNpcParser.npcStruct In wk.npcs
                Dim npcPosLines As CNpcParser.NPCline() = npc.Lines(CNpcParser.NPCline.knownType.ESetPosition)
                If npc.Map > 0 AndAlso npc.Map = wk.zone Then
                    Dim cord As New Point(npcPosLines(0).Params(2).value, npcPosLines(0).Params(4).value)
                    Dim loc As Point = MyPoint.Cord2Loc(cord, bmp.Width)
                    For i As Integer = loc.X - npcWidth / 2 To loc.X + npcWidth / 2
                        For j As Integer = loc.Y - npcWidth / 2 To loc.Y + npcWidth / 2
                            If i >= 0 AndAlso j >= 0 AndAlso i < bmp.Width AndAlso j < bmp.Height Then
                                bmp.SetPixel(i, j, CMiniMapPanel.npcColor)
                            End If
                        Next
                    Next
                End If
            Next
        End If
        e.Result = bmp
    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
        Me.map = e.Result
        Me.pleaseWaitL.Visible = False
        If Me.waitingForDraw Then
            If Not Me.map Is Nothing Then
                Me.drawLoc()
            Else
                Me.notAvailableL.Visible = True
            End If
        End If
    End Sub

End Class

