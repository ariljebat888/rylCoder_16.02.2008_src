Public Class frmArrEditor
    Public MobSelector As frmSelectMob = Nothing
    Public Structure SMob
        Dim id As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim resArea As Integer
        Dim scout As Integer
        Dim movingPat As Integer
        Dim pid As UInt32
    End Structure
    Private Structure SMobWithMId
        Dim id As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim resArea As Integer
        Dim pid As UInt32
        Dim scout As Integer
        Dim movingPat As Integer
        Dim mainId As Integer
    End Structure
    Private iMobs As SMob() = {}
    Private unPaintedMobs As New ArrayList 'SMob
    '#1 - CID (HEX) 

    'format: 
    '0x8  000    0000 
    'Zone order  mobID 

    '#2 - KID (DEC) 
    'MOB ID decimal 

    '#3 PID

    '#4 - X coordinate (DEC, INT) 

    '#5 - Y coordinate (DEC, INT) 

    '#6 - Z coordinate (DEC, INT) 

    '#7 - Scout, unknown 

    '#8 - MovingPattern, unknown 

    '#9 - ResspawnArea, in what distance they can respawn from original point
    Protected Sub flush()
        Dim mymobs As New ArrayList
        If MyBase.pointers.Count > 0 Then
            For Each p As MyPoint In MyBase.pointers
                Dim mob As New SMob
                If Not p.Tag Is Nothing Then
                    mob.id = p.Tag(0)
                    mob.resArea = p.Tag(1)
                    mob.pid = p.Tag(2)
                    mob.movingPat = p.Tag(3)
                    mob.scout = p.Tag(4)
                Else
                    mob.id = 1
                    mob.resArea = 32
                    mob.pid = 0
                End If
                Dim loc As MyPoint.SinglePoint = p.Cords
                mob.X = loc.X
                mob.Y = loc.Y
                mymobs.Add(mob)
            Next
            mymobs.AddRange(unPaintedMobs)
        Else
            mymobs = unPaintedMobs
        End If
        mymobs.Sort(New CMobSorter)
        iMobs = mymobs.ToArray(GetType(SMob))
    End Sub
    Public Sub setLines(ByRef lines As String())
        MyBase.ClearPointers()
        Dim nr As Long = 0
        MyBase.AllowAddition = True
        MyBase.DefaultPointerSmall = True
        MyBase.AllowInvalidate = False
        MyBase.AllowMultiSelect = True
        ReDim iMobs(lines.Length - 1)
        For Each sLine As String In lines
            'If nr > 200 Then Exit Sub
            Dim mob As New SMob
            Dim splices As String() = sLine.Split(vbTab)

            mob.id = Integer.Parse(splices(1))
            mob.pid = Convert.ToUInt32(splices(2).Substring(2), 16)
            If mob.pid > 2147483648 Then
                mob.pid = mob.pid - 2147483648
            Else
                mob.pid = 0
            End If
            mob.X = Integer.Parse(splices(3))
            mob.Y = Integer.Parse(splices(5))
            mob.movingPat = Integer.Parse(splices(6))
            mob.scout = Integer.Parse(splices(7))
            mob.resArea = Integer.Parse(splices(8))
            iMobs(nr) = mob
            unPaintedMobs.Add(mob)
            nr += 1
        Next
    End Sub
    Public Function getLines() As String()
        flush()
        Dim mMobs(iMobs.Length - 1) As SMobWithMId
        Dim monNr As Long = 0
        Dim prevMob As Integer = 0
        Dim i As Long = 0
        For Each mob As SMob In iMobs
            If prevMob <> mob.id Then
                prevMob = mob.id
                monNr = 0
            End If
            Dim nMob As New SMobWithMId
            nMob.id = mob.id
            nMob.mainId = &H80000000 + &H10000 * monNr + prevMob
            nMob.pid = mob.pid
            nMob.resArea = mob.resArea
            nMob.X = mob.X
            nMob.Y = mob.Y
            nMob.scout = mob.scout
            nMob.resArea = mob.resArea
            mMobs(i) = nMob
            monNr += 1
            i += 1
        Next
        Array.Sort(mMobs, New CMobPidSorter)
        Dim out(mMobs.Length) As String
        i = 1
        out(0) = "CID	KID	PID	X	Y	Z	Scout	MovingPattern	RespawnArea"
        For Each mob As SMobWithMId In mMobs
            Dim pid As Integer = mob.pid + &H80000000
            Dim s As String = "0x" & Hex(mob.mainId) & vbTab & mob.id & vbTab & "0x" & IIf(mob.pid > 0, Hex(pid), "00000000") & vbTab & mob.X & vbTab & "0" & vbTab & mob.Y & vbTab & mob.scout & vbTab & mob.movingPat & vbTab & mob.resArea
            out(i) = s
            i += 1
        Next
        Return out
    End Function

    Private Sub btnDrawVisible_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDrawVisible.Click
        Me.Cursor = Cursors.WaitCursor
        MyBase.SuspendLayout()
        flush()
        MyBase.ClearPointers()
        unPaintedMobs.Clear()
        Dim visib As Rectangle = MyBase.VisibleArea
        For Each mob As SMob In iMobs
            If mob.X >= visib.Left AndAlso mob.X <= visib.Right AndAlso mob.Y >= visib.Top AndAlso mob.Y <= visib.Bottom Then
                Dim pnt As MyPoint = MyBase.addPointer(mob.X, mob.Y, New Object() {mob.id, mob.resArea, mob.pid, mob.movingPat, mob.scout}, , , True) ', "Mob " & mobId & " with the respawn area of " & resArea, "Mob " & mobId, True)
                If mob.pid > 0 Then pnt.BackColor = Color.Green
            Else
                unPaintedMobs.Add(mob)
            End If
        Next
        MyBase.ResumeLayout(True)
        Me.Cursor = Cursors.Default
    End Sub
    Protected Class CMobSorter
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            If x.id > y.id Then
                Return 1
            ElseIf x.id < y.id Then
                Return -1
            Else
                Return 0
            End If
        End Function
    End Class
    Protected Class CMobPidSorter
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            If x.pid > y.pid Then
                Return 1
            ElseIf x.pid < y.pid Then
                Return -1
            Else
                If x.id > y.id Then
                    Return 1
                ElseIf x.id < y.id Then
                    Return -1
                Else
                    If x.mainId > y.mainId Then
                        Return 1
                    ElseIf x.mainId < y.mainId Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End If
        End Function
    End Class
    Private Sub updateMobInfo()
        Dim pt As MyPoint = MyBase.activePoint
        If Not pt Is Nothing Then
            Try
                If pt.Tag Is Nothing Then pt.Tag = New Object(4) {} 'too lazy to update how it should be

                pt.Tag = New Object() {Integer.Parse(Me.txtMobId.Text), Integer.Parse(Me.txtResArea.Text), Integer.Parse(Me.txtPartyId.Text), pt.Tag(3), pt.Tag(4)}
            Catch ex As Exception
                MsgBox("Check your info. All numbers must be decimal.")
            End Try
            Me.lnkMobName.Text = MobName(pt.Tag(0))
        End If
    End Sub

    Private Sub txtMobId_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMobId.Leave
        updateMobInfo()
    End Sub
    Private Sub txtPartyId_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPartyId.Leave
        updateMobInfo()
    End Sub
    Private Sub txtResArea_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtResArea.Leave
        updateMobInfo()
    End Sub

    Private Sub frmArrEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.cmbMultiStyle.SelectedIndex = 2
    End Sub

    Private Sub frmArrEditor_PointOnClick(ByRef sender As frmMap, ByRef point As MyPoint) Handles Me.PointOnClick
        If Not point.Tag Is Nothing Then
            Me.txtMobId.Text = point.Tag(0)
            Me.txtResArea.Text = point.Tag(1)
            Me.txtPartyId.Text = point.Tag(2)
            Me.lnkMobName.Text = MobName(point.Tag(0))

            Dim apD As Object() = point.Tag
            For Each p As MyPoint In MyBase.pointers
                Dim dat As Object() = p.Tag
                If p Is point Then 'it will already made blue
                ElseIf dat(2) > 0 AndAlso dat(2) = apD(2) Then 'same party
                    p.BackColor = Color.Violet
                ElseIf dat(0) = apD(0) Then 'same id
                    p.BackColor = Color.SkyBlue
                Else
                    p.BackColor = Color.Red
                End If
            Next
        Else
            point.Tag = New Object() {1, 32, 0, 0, 0}
            frmArrEditor_PointOnClick(sender, point)
        End If
    End Sub

    Private Sub btnCreateNewParty_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateNewParty.Click
        Dim partys As New ArrayList
        For Each mob As SMob In iMobs
            If partys.IndexOf(mob.pid) < 0 Then
                partys.Add(mob.pid)
            End If
        Next
        partys.Sort()
        If partys.Count > 0 Then
            Me.txtPartyId.Text = (CType(partys(partys.Count - 1), Integer) + 1)
        Else
            Me.txtPartyId.Text = 1
        End If
    End Sub
    Private Sub btnShowThisParty_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnShowThisParty.Click
        Dim pId As Integer = -1
        Try
            pId = Integer.Parse(Me.txtPartyId.Text)
        Catch ex As Exception
            MsgBox("Not a decimal number in party box")
        End Try
        If pId >= 0 Then
            Me.Cursor = Cursors.WaitCursor
            MyBase.SuspendLayout()
            flush()
            MyBase.ClearPointers()
            unPaintedMobs.Clear()
            For Each mob As SMob In iMobs
                If mob.pid = pId Then
                    MyBase.addPointer(mob.X, mob.Y, New Object() {mob.id, mob.resArea, mob.pid, mob.movingPat, mob.scout}, , , True) ', "Mob " & mobId & " with the respawn area of " & resArea, "Mob " & mobId, True)
                Else
                    unPaintedMobs.Add(mob)
                End If
            Next
            MyBase.ResumeLayout(True)
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Sub btnShowThisTypeMob_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnShowThisTypeMob.Click
        Dim pId As Integer = -1
        Try
            pId = Integer.Parse(Me.txtMobId.Text)
        Catch ex As Exception
            MsgBox("Not a decimal number in ID box")
        End Try
        If pId >= 0 Then
            Me.Cursor = Cursors.WaitCursor
            MyBase.SuspendLayout()
            flush()
            MyBase.ClearPointers()
            unPaintedMobs.Clear()
            For Each mob As SMob In iMobs
                If mob.id = pId Then
                    MyBase.addPointer(mob.X, mob.Y, New Object() {mob.id, mob.resArea, mob.pid, mob.movingPat, mob.scout}, , , True) ', "Mob " & mobId & " with the respawn area of " & resArea, "Mob " & mobId, True)
                Else
                    unPaintedMobs.Add(mob)
                End If
            Next
            MyBase.ResumeLayout(True)
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Public Function PaintToPic(ByRef mobs As SMob(), ByRef bottom As Bitmap) As Bitmap
        Dim mobSize As Integer = 2
        Dim rylVer As Integer = 2
        Dim mypic As New Bitmap(bottom.Width, bottom.Height, bottom.PixelFormat)
        Dim graph As Graphics = Graphics.FromImage(mypic)
        graph.DrawImage(bottom, 0, 0)
        For Each mob As SMob In mobs
            Dim lvl As Integer = MobLevel(mob.id)
            Dim col As Color = Color.Green
            If rylVer = 2 Then
                'Select Case lvl
                '    Case Is < 10
                '        col = Color.DarkGreen
                '    Case Is < 20
                '        col = Color.Green
                '    Case Is < 30
                '        col = Color.GreenYellow
                '    Case Is < 40
                '        col = Color.Yellow
                '    Case Is < 50
                '        col = Color.Orange
                '    Case Is < 60
                '        col = Color.Pink
                '    Case Is < 70
                '        col = Color.Red
                '    Case Is < 80
                '        col = Color.DarkRed
                '    Case Is < 90
                '        col = Color.Purple
                '    Case Else
                '        col = Color.BlueViolet
                'End Select
                Select Case lvl
                    Case Is < 102
                        col = Color.FromArgb(255, 0, 240, 0)
                    Case Is < 104
                        col = Color.FromArgb(255, 0, 200, 0)
                    Case Is < 106
                        col = Color.FromArgb(255, 0, 160, 0)
                    Case Is < 108
                        col = Color.FromArgb(255, 0, 120, 0)
                    Case Is < 110
                        col = Color.FromArgb(255, 0, 80, 0)
                    Case Is < 112
                        col = Color.FromArgb(255, 0, 0, 240)
                    Case Is < 114
                        col = Color.FromArgb(255, 0, 0, 200)
                    Case Is < 116
                        col = Color.FromArgb(255, 0, 0, 160)
                    Case Is < 118
                        col = Color.FromArgb(255, 0, 0, 120)
                    Case Is < 120
                        col = Color.FromArgb(255, 0, 0, 80)
                    Case Is < 118
                        col = Color.FromArgb(255, 240, 0, 0)
                    Case Is < 120
                        col = Color.FromArgb(255, 200, 0, 0)
                    Case Is < 122
                        col = Color.FromArgb(255, 160, 0, 0)
                    Case Is < 124
                        col = Color.FromArgb(255, 120, 0, 0)
                    Case Is < 126
                        col = Color.FromArgb(255, 80, 0, 0)
                    Case Is < 128
                        col = Color.FromArgb(255, 240, 0, 240)
                    Case Is < 130
                        col = Color.FromArgb(255, 220, 0, 220)
                    Case Is < 132
                        col = Color.FromArgb(255, 200, 0, 200)
                    Case Is < 134
                        col = Color.FromArgb(255, 180, 0, 180)
                    Case Is < 136
                        col = Color.FromArgb(255, 160, 0, 160)
                    Case Is < 138
                        col = Color.FromArgb(255, 140, 0, 140)
                    Case Is < 140
                        col = Color.FromArgb(255, 120, 0, 120)
                    Case Is < 142
                        col = Color.FromArgb(255, 100, 0, 100)
                    Case Is < 144
                        col = Color.FromArgb(255, 80, 0, 80)
                    Case Else
                        col = Color.Black
                End Select
            Else
                Select Case lvl
                    Case Is < 30
                        col = Color.DarkGreen
                    Case Is < 60
                        col = Color.Green
                    Case Is < 90
                        col = Color.GreenYellow
                    Case Is < 120
                        col = Color.Yellow
                    Case Is < 150
                        col = Color.Orange
                    Case Is < 180
                        col = Color.Pink
                    Case Is < 210
                        col = Color.Red
                    Case Is < 240
                        col = Color.DarkRed
                    Case Is < 270
                        col = Color.Purple
                    Case Else
                        col = Color.BlueViolet
                End Select
            End If
            Dim pos As Point = MyPoint.Cord2Loc(New Point(mob.X, mob.Y), bottom.Width)
            graph.FillRectangle(New SolidBrush(col), Integer.Parse(Math.Round(pos.X - mobSize / 2)), Integer.Parse(Math.Round(pos.Y - mobSize / 2)), mobSize, mobSize)
        Next
        graph.Save()
        Return mypic
    End Function
    Private Sub btnDrawJpgMap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDrawJpgMap.Click
        If Me.dlgSaveJpg.ShowDialog = Windows.Forms.DialogResult.OK Then
            'png,bmp,gif,jpg,*.*
            Dim type As Imaging.ImageFormat = Imaging.ImageFormat.Png
            Select Case Me.dlgSaveJpg.FilterIndex
                Case 1 : type = Imaging.ImageFormat.Png
                Case 2 : type = Imaging.ImageFormat.Bmp
                Case 3 : type = Imaging.ImageFormat.Gif
                Case 4 : type = Imaging.ImageFormat.Jpeg
            End Select
            Me.Cursor = Cursors.WaitCursor
            flush()
            PaintToPic(iMobs, MyBase.picMap.Image).Save(Me.dlgSaveJpg.FileName, type)
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Function MobLevel(ByVal id As Long) As Integer
        If Not MobSelector Is Nothing Then
            Return frmNpcEdit.GetMobLevel(id, MobSelector)
        Else
            Return 0
        End If
    End Function
    Private Function MobName(ByVal id As Long) As String
        Return frmNpcEdit.GetMobName(id, MobSelector)
    End Function

    Private Sub lnkMobName_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkMobName.LinkClicked
        Dim prevId As Integer = 0
        Try
            prevId = Integer.Parse(Me.txtMobId.Text)
        Catch ex As Exception
        End Try
        If prevId > 0 Then
            Dim id As Long = MobSelector.open(prevId)
            Me.txtMobId.Text = id
            updateMobInfo()
        End If
    End Sub

    Private Sub btnMultiplayPoint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMultiplayPoint.Click
        If Not MyBase.activePoint Is Nothing Then
            Me.Cursor = Cursors.WaitCursor
            Dim tag As Object() = MyBase.activePoint.Tag
            Dim loc As MyPoint.SinglePoint = activePoint.Cords
            Dim rad As Integer = Me.numRadius.Value
            Dim des As Integer = Me.numDensity.Value
            Dim max As Integer = Me.numMaxNumMobs.Value
            Dim added As Integer = 0
            Select Case Me.cmbMultiStyle.SelectedItem
                Case "Circle"
                    For row As Integer = loc.Y - rad To loc.Y + rad Step des
                        For col As Integer = loc.X - rad To loc.X + rad Step des
                            Dim far As Double = Math.Sqrt(Math.Abs(loc.Y - row) ^ 2 + Math.Abs(loc.X - col) ^ 2)
                            If (max <= 0 OrElse (max > 0 AndAlso added <= max)) AndAlso far <= rad AndAlso (row <> loc.Y OrElse col <> loc.X) Then
                                MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                                added += 1
                            End If
                        Next
                    Next
                Case "Circle X"
                    Dim shift As Boolean = False
                    For row As Integer = loc.Y - rad To loc.Y + rad Step des
                        For col As Integer = IIf(shift, loc.X - rad + des / 2, loc.X - rad) To IIf(shift, loc.X + rad - des / 2, loc.X + rad) Step des
                            Dim far As Double = Math.Sqrt(Math.Abs(loc.Y - row) ^ 2 + Math.Abs(loc.X - col) ^ 2)
                            If (max <= 0 OrElse (max > 0 AndAlso added <= max)) AndAlso far <= rad AndAlso (row <> loc.Y OrElse col <> loc.X) Then
                                MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                                added += 1
                            End If
                        Next
                        shift = Not shift
                    Next
                Case "Circle Rand"
                    If max < 0 Then
                        MsgBox("Max mob number must be greater than 0")
                    Else
                        For nr As Integer = 1 To max
                            Dim r As String = (nr + Now.Millisecond ^ 3).ToString
                            If r.Length > 6 Then r = r.Substring(r.Length - 6)
                            Dim rand As New Random(Integer.Parse(r))
                            Dim col As Integer = Math.Round(rand.NextDouble() * rad * 2 + (loc.X - rad))
                            Dim row As Integer = Math.Round(rand.NextDouble() * rad * 2 + (loc.Y - rad))

                            Dim far As Double = Math.Sqrt(Math.Abs(loc.Y - row) ^ 2 + Math.Abs(loc.X - col) ^ 2)
                            If far <= rad Then
                                MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                            Else
                                nr -= 1
                            End If
                        Next
                    End If
                Case "Rectangle"
                    For row As Integer = loc.Y - rad To loc.Y + rad Step des
                        For col As Integer = loc.X - rad To loc.X + rad Step des
                            If (max <= 0 OrElse (max > 0 AndAlso added <= max)) AndAlso (row <> loc.Y OrElse col <> loc.X) Then
                                MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                                added += 1
                            End If
                        Next
                    Next
                Case "Rectangle X"
                    Dim shift As Boolean = False
                    For row As Integer = loc.Y - rad To loc.Y + rad Step des
                        For col As Integer = IIf(shift, loc.X - rad + des / 2, loc.X - rad) To IIf(shift, loc.X + rad - des / 2, loc.X + rad) Step des
                            If (max <= 0 OrElse (max > 0 AndAlso added <= max)) AndAlso (row <> loc.Y OrElse col <> loc.X) Then
                                MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                                added += 1
                            End If
                        Next
                        shift = Not shift
                    Next
                Case "Rectangle Rand"
                    If max < 0 Then
                        MsgBox("Max mob number must be greater than 0")
                    Else
                        For nr As Integer = 1 To max
                            Dim r As String = (nr + Now.Millisecond ^ 3).ToString
                            If r.Length > 6 Then r = r.Substring(r.Length - 6)
                            Dim rand As New Random(Integer.Parse(r))
                            Dim col As Integer = Math.Round(rand.NextDouble() * rad * 2 + (loc.X - rad))
                            Dim row As Integer = Math.Round(rand.NextDouble() * rad * 2 + (loc.Y - rad))
                            MyBase.addPointer(col, row, New Object() {tag(0), tag(1), tag(2), tag(3), tag(4)}, , , True)
                        Next
                    End If
            End Select
            frmArrEditor_PointOnClick(Me, MyBase.activePoint)
            Me.Cursor = Cursors.Default
        Else
            MsgBox("Select a mob first")
        End If
    End Sub
End Class
