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

Public Class frmSelectMob
    Private closeB As Boolean = False
    Public MobScript() As MobInfo
    Private openedId As Long = 0
    Public Event MobSelected(ByVal sender As frmSelectMob, ByVal item As Long, ByVal prevItem As Long)
    Public Event MobScriptLoaded()
    Public MobsLoaded As Boolean = False
    Private returnId As Long = 0

    Public Structure MobInfo
        Dim ID As Long
        Dim Name As String
        Dim Level As Integer
    End Structure

    Private Class listItem
        Public iName As String
        Public iId As Long
        Public Sub New(ByVal name As String, ByVal id As Long)
            iName = name
            iId = id
        End Sub
        Public Overrides Function ToString() As String
            Return iName
        End Function
    End Class

    Private Function str2MobInfo(ByVal txt As String) As MobInfo
        Dim arr() As String = txt.Split(vbTab)
        Dim item As New MobInfo
        For i As Integer = 0 To arr.Length - 1
            Dim l As String = arr(i)
            Select Case i
                Case 0 : item.ID = l
                Case 1 : item.Name = l
                Case 20 : item.Level = l
            End Select
        Next
        Return item
    End Function

    Public Sub New(ByVal file As String, Optional ByVal interactive As Boolean = True)
        Me.closeB = False
        Me.openedId = 0
        Me.MobsLoaded = False
        Me.returnId = 0
        Me.InitializeComponent()
        Try
            Dim list As New ArrayList
            Dim reader As New IO.StreamReader(file)
            Do While Not reader.EndOfStream
                list.Add(reader.ReadLine)
            Loop
            reader.Close()
            Me.construct(DirectCast(list.ToArray(GetType(String)), String()), interactive)
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            If interactive Then
                Interaction.MsgBox(exception.Message, MsgBoxStyle.OkOnly, Nothing)
            End If
        End Try
    End Sub

    Public Sub New(ByVal lines As String(), Optional ByVal interactive As Boolean = True)
        Me.closeB = False
        Me.openedId = 0
        Me.MobsLoaded = False
        Me.returnId = 0
        Me.InitializeComponent()
        Me.construct(lines, interactive)
    End Sub

    Private Sub construct(ByVal lines As String(), Optional ByVal interactive As Boolean = True)
        Array.Resize(MobScript, 0)
        Try
            Dim str As String
            For Each str In lines
                If ((str.Length > 1) AndAlso (str.Substring(0, 2) <> "//")) Then
                    ReDim Preserve MobScript(UBound(MobScript) + 1)
                    MobScript(UBound(MobScript)) = str2MobInfo(str)
                End If
            next
        Catch ex As Exception
            If interactive Then MsgBox(ex.Message)
            Exit Sub
        End Try
        MobsLoaded = True
        RaiseEvent MobScriptLoaded()
        setFilter()
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        'Me.Close()
        Dim item As Long = 0
        If Not Me.lstMobs.SelectedItem Is Nothing Then item = Me.lstMobs.SelectedItem.iID
        returnId = item
        Me.Hide()
        RaiseEvent MobSelected(Me, item, openedId)
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        'Me.Close()
        Me.Hide()
    End Sub
    Public Sub kill()
        MobScript = Nothing
        closeB = True
        Me.Close()
    End Sub
    Private Sub frmSelectItem_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not closeB AndAlso Not e.CloseReason = CloseReason.WindowsShutDown AndAlso Not e.CloseReason = CloseReason.TaskManagerClosing AndAlso Not e.CloseReason = CloseReason.FormOwnerClosing Then e.Cancel = True
        Me.Hide()
    End Sub
    Public Function open(Optional ByVal itemId As Long = 0) As Long
        openedId = itemId
        returnId = 0
        If itemId > 0 Then
            Dim found As Boolean = False
            Dim i As Long = 0
            For Each it As listItem In Me.lstMobs.Items
                If it.iId = itemId Then
                    Me.lstMobs.SelectedIndex = i

                    found = True
                    Exit For
                End If
                i += 1
            Next
            If Not found Then
                'Me.cmbItemType.SelectedIndex = 0
                Me.txtMobName.Text = ""
                Me.txtMobID.Text = itemId
                Me.txtMobLvl.Text = ""
                setFilter()
                If Me.lstMobs.Items.Count > 0 Then Me.lstMobs.SelectedIndex = 0
            End If
        End If
        Me.ShowDialog()
        Return returnId
    End Function

    Public Sub setFilter()
        Dim name As String = Me.txtMobName.Text
        Dim id As Long = Val(Me.txtMobID.Text)
        Dim level As Long = Val(Me.txtMobLvl.Text)
        Me.lstMobs.Items.Clear()
        For Each s As MobInfo In MobScript
            If (name = "" OrElse s.Name.IndexOf(name) >= 0) AndAlso (id = 0 OrElse s.ID.ToString.IndexOf(id.ToString) >= 0) AndAlso (level = 0 OrElse s.Level = level) Then
                Dim lI As New listItem("[" & s.ID & "] " & s.Name & ", " & s.Level, s.ID)
                Me.lstMobs.Items.Add(lI)
            End If
        Next
    End Sub

    Private Sub txtMobName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMobName.TextChanged
        setFilter()
    End Sub

    Private Sub txtMobID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMobID.TextChanged
        setFilter()
    End Sub

    Private Sub txtMobLvl_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMobLvl.TextChanged
        setFilter()
    End Sub

End Class
