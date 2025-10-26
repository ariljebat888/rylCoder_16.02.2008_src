'################################################
'##                                            ##
'##         RYL mcf & gsf file editor          ##
'##                                            ##
'##   (C) 2006 & 2007 AlphA                    ##
'##                                            ##
'##   This source is for private development.  ##
'##   You can have this source only with the   ##
'##   owners permission.                       ##
'##                                            ##
'################################################

Imports System.Windows.Forms

Public Class frmMap
    Protected pointers As New ArrayList 'type=picturebox
    Private mapsWeHave As Integer() = {12, 8, 16, 1, 2, 3, 4, 5}
    Private mapMaxCords As Integer() = {0, 4096, 4096, 4096, 3150, 3750, 0, 0, 4096, 0, 0, 0, 4096, 0, 0, 0, 4096}
    Public Const map1VertCorrection As Integer = -630
    Private resM As Resources.ResourceManager = Global.rylCoder.My.Resources.ResourceManager()
    Public iOpenZone As Integer = 12
    Protected activePoint As MyPoint = Nothing
    Private iAllowMoves As Boolean = True
    Private iAllowAdds As Boolean = False
    Public DefaultPointerSmall As Boolean = False
    Public Event MapChange(ByRef sender As frmMap, ByVal newMapNr As Integer)
    Public Event PointOnClick(ByRef sender As frmMap, ByRef point As MyPoint)
    Private iZoom As Single = 1
    Private orgMap As Bitmap = Nothing
    Public Const WheelZoom As Single = 0.2
    Private mapMove As Boolean = False
    Private mapMoveStart As Point
    Private mapMoveMapLoc As Point
    Private selectStart As Boolean = False
    Private selectLoc As Point
    Public AllowInvalidate As Boolean = True
    Public AllowMultiSelect As Boolean = False
    Protected mapsChanged As Integer = 0
    'Private mapZoomRec As Boolean = False
    'Private zoomRectangle As New Panel

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub
    Public Property openZone() As Integer
        Get
            Return iOpenZone
        End Get
        Set(ByVal value As Integer)
            iOpenZone = value
            MyPoint.mapWidth = mapMaxCords(iOpenZone)
            If iOpenZone = 1 Then
                MyPoint.vertCorrection = map1VertCorrection
            Else
                MyPoint.vertCorrection = 0
            End If
            orgMap = resM.GetObject("zone_" & iOpenZone)
            Me.picMap.Image = orgMap
            iZoom = 1.0
            Dim point As New Point(0, 0)
            Me.picMap.Location = point
            If Me.mapsChanged > 0 Then
                If (Not MapChangeEvent Is Nothing) Then
                    RaiseEvent MapChange(Me, iOpenZone)
                End If
            End If
            mapsChanged += 1
        End Set
    End Property
    Public Property zoomFactor() As Single
        Get
            Return iZoom
        End Get
        Set(ByVal value As Single)
            If Math.Round(value, 3) >= 1 AndAlso value < 20 Then
                If ((value > 3.0) AndAlso (zoomFactor <= 3.0)) Then
                    Dim bitmap As Bitmap = Nothing
                    bitmap = CMiniMap.CreateMap(frmNpcEdit.RylGameDir, openZone)
                    If (Not bitmap Is Nothing) Then
                        Me.orgMap = bitmap
                    End If
                ElseIf ((value <= 3.0) AndAlso (zoomFactor > 3.0)) Then
                    Me.orgMap = resM.GetObject("zone_" & openZone)
                End If

                iZoom = value
                Me.sldZoom.Value = CInt(Math.Round(CDbl(Me.iZoom)))

                If value > 3.0! Then value = value - 2.0!
                If iZoom <= 1.0! Then Me.picMap.Location = New Point(0, 0)
                If Not Me.picMap.Image Is Nothing AndAlso Not Me.picMap.Image Is orgMap Then Me.picMap.Image.Dispose()
                If value <> 1.0! Then
                    Me.picMap.Image = AddMath.resizeImage(orgMap, 0, 0, orgMap.Width * value, orgMap.Width * value, value)
                Else
                    Me.picMap.Image = orgMap
                End If
                Me.picMap.Size = Me.picMap.Image.Size
                For Each p As MyPoint In pointers
                    p.refreshLoc()
                Next
            End If
        End Set
    End Property
    Public Function addPointer(ByVal x As Single, ByVal y As Single, Optional ByRef Tag As Object = Nothing, Optional ByVal txt1 As String = "", Optional ByVal txt2 As String = "", Optional ByVal smallPointer As Boolean = False) As MyPoint
        Dim mp As New MyPoint
        Me.Controls.Add(mp)
        mp.Parent = Me.picMap
        mp.Tag = Tag
        If smallPointer Then
            'mp.Image = Global.mcfCoder.My.Resources.Resources.showOnMapSmall
            mp.Image = Nothing
            mp.Size = New Size(3, 3)
            mp.BackColor = Color.Red
            mp.smallPointer = True
        Else
            mp.Image = Global.rylCoder.My.Resources.Resources.X_mark
            mp.BackColor = Color.Transparent
            mp.Size = mp.Image.Size
        End If
        Dim p As New MyPoint.SinglePoint
        p.X = x
        p.Y = y
        mp.Cords = p
        If txt1 <> "" Then
            mp.ToolTip = txt1
            If txt2 <> "" Then mp.ToolTipCaption = txt2
        End If
        mp.AllowedToMove = iAllowMoves
        mp.AllowedToDelete = iAllowAdds
        pointers.Add(mp)
        AddHandler mp.LocationChange, AddressOf PointLocationChange
        AddHandler mp.MouseDown, AddressOf mp_MouseClick
        AddHandler mp.DeleteMe, AddressOf ClearPoint

        Return mp
    End Function
    Public Sub ClearPointers()
        For Each p As MyPoint In pointers
            ClearPoint(p, True)
        Next
        pointers.Clear()
    End Sub
    Public Sub ClearPoint(ByRef point As MyPoint, Optional ByVal clearall As Boolean = False)
        If Not clearall Then pointers.Remove(point)
        Me.Controls.Remove(point)
        If point Is activePoint Then activePoint = Nothing
        point.Dispose()
        point = Nothing
    End Sub
    Public Property AllowMoves() As Boolean
        Get
            Return iAllowMoves
        End Get
        Set(ByVal value As Boolean)
            iAllowMoves = value
            For Each p As MyPoint In pointers
                p.AllowedToMove = value
            Next
            Me.txtLocationX.Enabled = value
            Me.txtLocationY.Enabled = value
        End Set
    End Property
    Public Property AllowAddition() As Boolean
        Get
            Return iAllowAdds
        End Get
        Set(ByVal value As Boolean)
            iAllowAdds = value
            For Each p As MyPoint In pointers
                p.AllowedToDelete = value
            Next
        End Set
    End Property

    Private Sub frmMap_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim thSize As Integer = 80
        For Each i As Integer In mapsWeHave
            Dim pic As Bitmap = resM.GetObject("zone_" & i)
            Dim img As New PictureBox
            img.Size = New Size(thSize, thSize)
            img.BackColor = Color.LightGray
            img.Cursor = Cursors.Hand
            img.Tag = i
            AddHandler img.Click, AddressOf map_OnClick
            flowMapThumb.Controls.Add(img)
            img.Image = pic.GetThumbnailImage(thSize, thSize, AddressOf drawThumb, System.IntPtr.Zero)
        Next
        openZone = iOpenZone
        'SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        For Each p As MyPoint In pointers
            p.BringToFront()
            'p.BackColor = Color.Transparent
            'p.BackColor = Color.Red
            p.Show()
        Next
        If pointers.Count = 1 Then
            mp_MouseClick(pointers(0), New MouseEventArgs(Windows.Forms.MouseButtons.Left, 1, 0, 0, 0))
        End If
        If AllowMultiSelect AndAlso iAllowAdds Then
            Me.btnDeleteMulti.Visible = True
        End If
    End Sub
    Private Sub setPointStatus(ByRef point As MyPoint, ByVal active As Boolean)
        If active Then
            If point.isActive = active Then Exit Sub
            If Not selectStart Then activePoint = point
            If Not point.smallPointer Then
                If Not point.Image Is My.Resources.Resources.Xa_mark Then point.Image = My.Resources.Resources.Xa_mark
            Else
                If point.BackColor <> Color.Blue Then point.BackColor = Color.Blue
            End If
            point.isActive = True
            If Not selectStart Then activePoint.RaiseLocChange()
        Else
            If Not point.smallPointer Then
                If Not point.Image Is My.Resources.Resources.X_mark Then point.Image = My.Resources.Resources.X_mark
            Else
                If point.BackColor <> Color.Red Then point.BackColor = Color.Red
            End If
            point.isActive = False
            If point Is activePoint Then activePoint = Nothing
        End If
    End Sub
    Private Sub mp_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        If Not activePoint Is Nothing Then
            setPointStatus(activePoint, False)
        End If
        setPointStatus(sender, True)
        RaiseEvent PointOnClick(Me, sender)
    End Sub
    Private Sub goingToAdd(ByVal sender As Object, ByVal e As EventArgs)
        Dim mp As New MyPoint
        Me.Controls.Add(mp)
        mp.Parent = Me.picMap
        mp.Tag = Nothing
        If DefaultPointerSmall Then
            mp.Image = Global.rylCoder.My.Resources.Resources.showOnMapSmall
            mp.smallPointer = True
        Else
            mp.Image = Global.rylCoder.My.Resources.Resources.X_mark
        End If
        Dim p As Point = Me.picMap.PointToClient(Control.MousePosition)
        p.Y -= mp.Image.Size.Height / 2
        p.X -= mp.Image.Size.Width / 2
        mp.Location = p
        mp.Cords = mp.Cords 'refresh the onFlyCords
        mp.Size = mp.Image.Size
        mp.AllowedToMove = iAllowMoves
        mp.AllowedToDelete = iAllowAdds
        pointers.Add(mp)
        mp.BringToFront()
        mp.BackColor = Color.Transparent
        AddHandler mp.LocationChange, AddressOf PointLocationChange
        AddHandler mp.MouseDown, AddressOf mp_MouseClick
        AddHandler mp.DeleteMe, AddressOf ClearPoint
        mp.Show()
    End Sub
    Private Sub picMap_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMap.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right AndAlso iAllowAdds Then
            Dim cnt As New ContextMenuStrip
            cnt.Items.Add("&Add", Nothing, AddressOf goingToAdd)
            cnt.Show(Control.MousePosition)
        End If
    End Sub
    Private Function drawThumb() As Boolean
    End Function
    Private Sub map_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        If Not sender.tag Is Nothing AndAlso sender.tag > 0 Then
            orgMap = resM.GetObject("zone_" & sender.tag)
            zoomFactor = 1
            RaiseEvent MapChange(Me, sender.tag)
        End If
    End Sub
    Public ReadOnly Property Points() As Single()()
        Get
            Dim ps As New ArrayList 'type=single()
            For Each p As MyPoint In pointers
                Dim pp As MyPoint.SinglePoint = p.Cords
                ps.Add(New Single() {pp.X, pp.Y})
            Next
            Return ps.ToArray(GetType(Single()))
        End Get
    End Property
    Private Sub PointLocationChange(ByRef sender As MyPoint, ByVal x As Single, ByVal y As Single)
        Me.txtLocationX.Text = x
        Me.txtLocationY.Text = y
        Me.txtLocationX.Refresh()
        Me.txtLocationY.Refresh()
        If AllowInvalidate Then
            sender.Refresh()
            sender.Parent.Refresh()
        End If
    End Sub
    Private Sub updateFromBoxes()
        Dim x As Single = 0
        Dim y As Single = 0
        Try
            x = Single.Parse(Me.txtLocationX.Text)
            y = Single.Parse(Me.txtLocationY.Text)
        Catch ex As Exception
            MsgBox(ex.Message)
            Exit Sub
        End Try
        If Not activePoint Is Nothing Then
            Dim sp As New MyPoint.SinglePoint
            sp.X = x
            sp.Y = y
            activePoint.Cords = sp
        End If
    End Sub
    Private Sub txtLocationX_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtLocationX.KeyDown
        If e.KeyCode = Keys.Return Then
            updateFromBoxes()
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub txtLocationY_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtLocationY.KeyDown
        If e.KeyCode = Keys.Return Then
            updateFromBoxes()
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub txtLocationX_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLocationX.Leave
        updateFromBoxes()
    End Sub
    Private Sub txtLocationY_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLocationY.Leave
        updateFromBoxes()
    End Sub

    Private Sub sldZoom_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles sldZoom.ValueChanged
        If (Me.zoomFactor <> Me.sldZoom.Value) Then
            Me.zoomFactor = Me.sldZoom.Value
        End If
    End Sub

    Private Sub frmMap_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        Dim p As Point = Me.pnlMap.PointToClient(Control.MousePosition)
        If (((p.X > 0) AndAlso (p.Y > 0)) AndAlso ((p.X < Me.pnlMap.Width) AndAlso (p.Y < Me.pnlMap.Height))) Then
            Dim point2 As Point = Me.picMap.PointToClient(Control.MousePosition)
            Dim num As Double = (CDbl(point2.X) / CDbl(Me.picMap.Width))
            Dim num2 As Double = (CDbl(point2.Y) / CDbl(Me.picMap.Height))
            If (e.Delta < 0) Then
                Me.zoomFactor = (Me.zoomFactor - WheelZoom)
            Else
                Me.zoomFactor = (Me.zoomFactor + WheelZoom)
            End If
            Dim point3 As New Point(CInt(Math.Round(CDbl((Me.picMap.Width * num)))), CInt(Math.Round(CDbl((Me.picMap.Height * num2)))))
            Me.picMap.Location = picMap.Location + point2 - point3
        End If
    End Sub
    Private Sub picMap_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMap.MouseDown
        If Not activePoint Is Nothing Then
            For Each p As MyPoint In pointers
                setPointStatus(p, False)
            Next
        End If
        If e.Button = Windows.Forms.MouseButtons.Middle Then
            Me.Cursor = Cursors.Hand
            mapMove = True
            mapMoveStart = Control.MousePosition
            mapMoveMapLoc = Me.picMap.Location
            'ElseIf e.Button = Windows.Forms.MouseButtons.Left Then
            '    Me.Cursor = Cursors.Cross
            '    mapZoomRec = True
            '    mapMoveStart = Control.MousePosition
            '    mapMoveMapLoc = e.Location
            '    Me.picMap.Controls.Add(zoomRectangle)
            '    zoomRectangle.Parent = Me.picMap
            '    zoomRectangle.BorderStyle = BorderStyle.FixedSingle
            '    zoomRectangle.BackColor = Color.Transparent
            '    zoomRectangle.ForeColor = Color.Blue
            '    zoomRectangle.Location = e.Location
            '    zoomRectangle.Visible = True
        ElseIf e.Button = Windows.Forms.MouseButtons.Left AndAlso AllowMultiSelect Then
            selectStart = True
            selectLoc = e.Location
        End If
    End Sub
    Private Sub picMap_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMap.MouseMove
        If mapMove Then
            Dim xOff As Integer = Control.MousePosition.X - mapMoveStart.X
            Dim yOff As Integer = Control.MousePosition.Y - mapMoveStart.Y
            Dim nP As New Point(mapMoveMapLoc.X + xOff, mapMoveMapLoc.Y + yOff)
            If nP.X < Me.pnlMap.Width - Me.picMap.Width Then nP.X = Me.pnlMap.Width - Me.picMap.Width
            If nP.Y < Me.pnlMap.Height - Me.picMap.Height Then nP.Y = Me.pnlMap.Height - Me.picMap.Height
            If nP.X > 0 Then nP.X = 0
            If nP.Y > 0 Then nP.Y = 0
            Me.picMap.Location = nP
            'ElseIf mapZoomRec Then

            '    Dim xOff As Integer = Control.MousePosition.X - mapMoveStart.X
            '    Dim yOff As Integer = Control.MousePosition.Y - mapMoveStart.Y
            '    If xOff < 0 Then
            '        xOff = xOff * (-1)
            '        zoomRectangle.Left -= xOff
            '    End If
            '    If yOff < 0 Then
            '        yOff = yOff * (-1)
            '        zoomRectangle.Top -= yOff
            '    End If
            '    zoomRectangle.Size = New Size(xOff, yOff)
        ElseIf selectStart Then
            Dim m As Point = e.Location
            Dim s As Point = selectLoc
            If s.X > m.X Then
                s.X = e.Location.X
                m.X = selectLoc.X
            End If
            If s.Y > m.Y Then
                s.Y = e.Location.Y
                m.Y = selectLoc.Y
            End If
            For Each p As MyPoint In pointers
                Dim l As Point = p.Location
                If l.X >= s.X AndAlso l.X <= m.X AndAlso l.Y >= s.Y AndAlso l.Y <= m.Y Then
                    setPointStatus(p, True)
                Else
                    setPointStatus(p, False)
                End If
            Next
        End If
    End Sub
    Public ReadOnly Property VisibleArea() As Rectangle
        Get
            Dim testP As New MyPoint
            testP.Visible = False
            testP.Parent = Me.picMap
            testP.Location = New Point(Me.picMap.Left * (-1), Me.picMap.Top * (-1))
            testP.Size = New Size(0, 0)
            Dim p As MyPoint.SinglePoint = testP.Cords

            testP.Location = New Point(Me.picMap.Left * (-1) + Me.pnlMap.Width, Me.picMap.Top * (-1) + Me.pnlMap.Height)
            Dim s As MyPoint.SinglePoint = testP.Cords
            Return New Rectangle(p.X, p.Y - (p.Y - s.Y), s.X - p.X, p.Y - s.Y)
        End Get
    End Property
    Private Sub picMap_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles picMap.MouseUp
        If mapMove Then
            Me.Cursor = Cursors.Default
            mapMove = False
            'ElseIf mapZoomRec Then
            '    Me.Cursor = Cursors.Default
            '    mapZoomRec = False
            '    zoomRectangle.Visible = False
            '    Me.picMap.Controls.Remove(zoomRectangle)
        ElseIf selectStart Then
            selectStart = False
        End If
    End Sub

    Private Sub btnDeleteMulti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteMulti.Click
        Dim nP As New ArrayList
        For Each p As MyPoint In pointers
            If p.isActive Then
                ClearPoint(p, True)
            Else
                nP.Add(p)
            End If
        Next
        pointers = nP
    End Sub
End Class

Public Class MyPoint
    Inherits Windows.Forms.PictureBox
    Public Event LocationChange(ByRef sender As MyPoint, ByVal lX As Single, ByVal lY As Single)
    Public Event DeleteMe(ByRef sender As MyPoint, ByVal ignore As Boolean)
    Dim Draging As Boolean
    Dim Xpos As Single
    Dim Ypos As Single
    Dim suhe As Single = 0
    Public Shared mapBorder As Integer = 316
    Public Shared mapWidth As Integer = 4096
    Public Shared vertCorrection As Integer = 0
    Public Shared horiCorrection As Integer = 0
    Private iToolTip As ToolTip = Nothing
    Private iAllowedToMove As Boolean = True
    Private iAllowedToDelete As Boolean = False
    Public smallPointer As Boolean = False
    Public onFlyLoc As New SinglePoint
    Public isActive As Boolean = False

    Public Sub New()
        Me.Cursor = Cursors.Hand
    End Sub
    Public Property Cords() As SinglePoint
        Get
            suhe = Parent.Size.Width / (mapWidth - 2 * mapBorder)
            Dim nL As New SinglePoint
            With Parent
                nL.X = (Me.Location.X + Me.Size.Width / 2) / suhe + mapBorder - horiCorrection
                nL.Y = (.Size.Height - (Me.Location.Y + Me.Size.Height / 2)) / suhe + mapBorder + vertCorrection
            End With
            nL.X = Math.Round(nL.X) ' 1pixel=8units ingame.. so its really pointless to have floats
            nL.Y = Math.Round(nL.Y)
            Return nL
        End Get
        Set(ByVal value As SinglePoint)
            suhe = Parent.Size.Width / (mapWidth - 2 * mapBorder)
            onFlyLoc = value
            Dim nL As New Point
            'Me.Size = Me.Image.Size
            With Parent
                nL.X = (value.X + horiCorrection - mapBorder) * suhe - (Me.Size.Width / 2)
                nL.Y = (mapWidth - value.Y + vertCorrection - mapBorder) * suhe - (Me.Size.Height / 2)
            End With
            Me.Location = nL

        End Set
    End Property
    Public Sub refreshLoc()
        Dim value As SinglePoint = onFlyLoc
        suhe = Parent.Size.Width / (mapWidth - 2 * mapBorder)
        Dim nL As New Point
        With Parent
            nL.X = (value.X - mapBorder) * suhe - (Me.Size.Width / 2)
            nL.Y = (mapWidth - value.Y - mapBorder) * suhe - (Me.Size.Height / 2)
        End With
        Me.Location = nL
    End Sub
    Private Sub goingToDelete(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent DeleteMe(Me, False)
    End Sub

    Private Sub MyPoint_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If iAllowedToDelete Then
                Dim cnt As New ContextMenuStrip
                cnt.Items.Add("&Delete", Nothing, AddressOf goingToDelete)
                cnt.Show(Control.MousePosition)
            End If
        End If
    End Sub
    Private Sub pic_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If Draging = False Then
            Draging = True
            Xpos = e.X
            Ypos = e.Y
        End If
    End Sub
    Public WriteOnly Property ToolTipCaption() As String
        Set(ByVal value As String)
            If Not iToolTip Is Nothing Then
                iToolTip.ToolTipTitle = value
            End If
        End Set
    End Property
    Public WriteOnly Property ToolTip() As String
        Set(ByVal value As String)
            Dim tt As New ToolTip()
            tt.ToolTipIcon = ToolTipIcon.Info
            tt.AutoPopDelay = 3000
            tt.UseAnimation = True
            tt.UseFading = True
            tt.IsBalloon = True
            tt.ShowAlways = True
            tt.SetToolTip(Me, value)
            tt.Tag = value
            iToolTip = tt
        End Set
    End Property

    Private Sub MyPoint_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
        If Not iToolTip Is Nothing Then
            iToolTip.SetToolTip(Me, iToolTip.Tag)
        End If
    End Sub
    Private Sub pic_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If Draging = True AndAlso iAllowedToMove Then
            Me.Left += (e.X - Xpos)
            Me.Top += (e.Y - Ypos)
            If Me.Left < -Me.Width / 2 Then Me.Left = -Me.Width / 2
            If Me.Top < -Me.Height / 2 Then Me.Top = -Me.Height / 2
            If Me.Left > Me.Parent.Width - Me.Width / 2 Then Me.Left = Me.Parent.Width - Me.Width / 2
            If Me.Top > Me.Parent.Height - Me.Height / 2 Then Me.Top = Me.Parent.Height - Me.Height / 2
            RaiseLocChange()
        End If
    End Sub
    Private eventInProgress As Boolean = False
    Public Sub RaiseLocChange()
        Dim pS As SinglePoint = Me.Cords
        onFlyLoc = pS
        If Not eventInProgress Then
            eventInProgress = True
            RaiseEvent LocationChange(Me, pS.X, pS.Y)
            eventInProgress = False
        End If
    End Sub
    Private Sub pic_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        Draging = False
    End Sub
    Public Structure SinglePoint
        Dim X As Single
        Dim Y As Single
    End Structure
    Public Property AllowedToMove() As Boolean
        Get
            Return iAllowedToMove
        End Get
        Set(ByVal value As Boolean)
            iAllowedToMove = value
        End Set
    End Property
    Public Property AllowedToDelete() As Boolean
        Get
            Return iAllowedToDelete
        End Get
        Set(ByVal value As Boolean)
            iAllowedToDelete = value
        End Set
    End Property
    Public Shared Function Cord2Loc(ByVal cord As Point, ByVal bottomWidth As Integer) As Point
        Dim suhe As Single = bottomWidth / (mapWidth - 2 * mapBorder)
        Dim nL As New Point
        nL.X = (cord.X + horiCorrection - mapBorder) * suhe
        nL.Y = (mapWidth - cord.Y + vertCorrection - mapBorder) * suhe
        Return nL
    End Function
    Public Shared Function Loc2Cord(ByVal cord As Point, ByVal bottomWidth As Integer) As Point
        Dim suhe As Single = bottomWidth / (mapWidth - 2 * mapBorder)
        Dim nL As New Point
        nL.X = cord.X / suhe + mapBorder - horiCorrection
        nL.Y = (bottomWidth - cord.Y) / suhe + mapBorder + vertCorrection
        If nL.X < 0 Then nL.X = 0
        If nL.Y < 0 Then nL.Y = 0
        If nL.X >= bottomWidth Then nL.X = bottomWidth - 1
        If nL.Y >= bottomWidth Then nL.Y = bottomWidth - 1
        Return nL
    End Function
End Class
