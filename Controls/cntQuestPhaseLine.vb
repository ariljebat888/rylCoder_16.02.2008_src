Public Class cntQuestPhaseLine
    Public qLine As CQuestParser.QLine = Nothing
    Public level As Integer = 0
    Private lvl3BG As Color = Color.FromArgb(&HFFFFE9E9)
    Private lvl3BGa As Color = Color.FromArgb(&HFFFFBBBB)
    Private lvl4BG As Color = Color.FromArgb(&HFFE9FFE9)
    Private lvl4BGa As Color = Color.FromArgb(&HFFBBFFBB)
    Private mouseIsDown As Boolean = False
    Public Event openQLinesForEdit(ByRef sender As cntQuestPhaseLine, ByRef line As CQuestParser.QLine)
    Public Event lineWantsToMove(ByRef sender As cntQuestPhaseLine, ByRef line As CQuestParser.QLine, ByVal offset As Integer)
    Private butDownLocation As Integer = 0
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub New(ByRef line As CQuestParser.QLine)
        InitializeComponent()
        setQuestLine(line)
    End Sub
    Public Sub setQuestLine(ByRef line As CQuestParser.QLine)
        qLine = line

        level = IIf(Array.IndexOf(CQuestParser.QuestPhase.lvl3functions, qLine.Type) >= 0, 3, 4)
        If qLine.Type = CQuestParser.QLine.KnownType.EElse Then level = 3
        Me.BackColor = IIf(level = 3, lvl3BG, lvl4BG)
        Me.flowMain.Controls.Add(New frmNpcEdit.namedLabel(IIf(level <> 3, StrDup(3, " "), "") & CQuestParser.QLine.Type2String(qLine.Type) & "(", True))
        Dim i As Integer = 0
        For Each p As CMcfBase.SParamElem In qLine.params
            Me.flowMain.Controls.Add(New frmNpcEdit.namedLabel(p.value.ToString & IIf(i < qLine.params.Length - 1, ",", "")))
            i += 1
        Next
        Me.flowMain.Controls.Add(New frmNpcEdit.namedLabel(")", True))
        setHooks()
    End Sub
    Private Sub cntQuestPhaseLine_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        setHooks()
    End Sub

    Private Sub cntQuestPhaseLine_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent openQLinesForEdit(Me, qLine)
    End Sub

    Private Sub cntQuestPhaseLine_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseEnter
        Me.BackColor = IIf(level = 3, lvl3BGa, lvl4BGa)
    End Sub
    Private Sub cntQuestPhaseLine_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.MouseLeave
        Me.BackColor = IIf(level = 3, lvl3BG, lvl4BG)
    End Sub

    Private Sub setHooks(Optional ByVal cntCollec As Windows.Forms.Control.ControlCollection = Nothing)
        If cntCollec Is Nothing Then cntCollec = Me.Controls
        For Each c As Control In cntCollec
            AddHandler c.MouseEnter, AddressOf cntQuestPhaseLine_MouseEnter
            AddHandler c.MouseLeave, AddressOf cntQuestPhaseLine_MouseLeave
            AddHandler c.MouseClick, AddressOf cntQuestPhaseLine_MouseClick
            AddHandler c.MouseDown, AddressOf cntQuestPhaseLine_MouseDown
            AddHandler c.MouseUp, AddressOf cntQuestPhaseLine_MouseUp
            AddHandler c.MouseMove, AddressOf cntQuestPhaseLine_MouseMove
            If Not c.Controls Is Nothing AndAlso c.Controls.Count > 0 Then
                setHooks(c.Controls)
            End If
        Next
    End Sub

    Private Sub cntQuestPhaseLine_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        mouseIsDown = True
        butDownLocation = e.Location.Y
    End Sub
    Private Sub cntQuestPhaseLine_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mouseIsDown Then
            Me.Cursor = Cursors.Default
            Dim offDiff As Integer = e.Location.Y - butDownLocation
            If offDiff > 5 OrElse offDiff < -5 Then RaiseEvent lineWantsToMove(Me, qLine, offDiff + Me.Location.Y)
        End If
        mouseIsDown = False
    End Sub

    Private Sub cntQuestPhaseLine_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If mouseIsDown Then
            Me.Cursor = Cursors.SizeAll
        End If
    End Sub
End Class
