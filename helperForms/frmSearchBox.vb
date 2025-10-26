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

Public Class frmSearchBox
    Private parentNpc As RichTextBox = Nothing
    Private closeB As Boolean = False
    Private prev_result As Long = -1
    Public Sub New(ByRef owner As RichTextBox)
        parentNpc = owner
        InitializeComponent()
    End Sub
    Public Sub kill()
        closeB = True
        Me.Close()
    End Sub
    Public Event DoSearch(ByVal sender As frmSearchBox, ByVal text As String, ByVal matchCase As Boolean)
    Public Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Me.txtSearch.Text <> "" Then
            Dim txt As String = Me.txtSearch.Text
            Dim cas As Boolean = Me.chkMatch.Checked
            If Me.rdDecimal.Checked Then
                Try
                    Dim nr As Long = Convert.ToInt64(Me.txtSearch.Text, 10)
                    txt = Hex(nr).ToUpper
                    cas = False
                Catch ex As Exception
                    Me.txtSearch.BackColor = Color.Pink
                    Exit Sub
                End Try
            End If
            RaiseEvent DoSearch(Me, txt, cas)
            If Not parentNpc Is Nothing Then
                With Me.parentNpc
                    Dim st As Integer = 0
                    If .SelectionStart > 0 Then st = .SelectionStart
                    If .SelectionLength > 0 Then st += .SelectionLength
                    Dim loc As Long = .Text.IndexOf(txt, st, IIf(cas, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase))
                    If loc < 0 AndAlso st > 0 Then loc = .Text.IndexOf(txt, IIf(cas, StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase))
                    If loc < 0 Then
                        MsgBox("No results for" & vbNewLine & txt & vbNewLine & "")
                        Me.Show()
                    Else
                        .Focus()
                        .Select(loc, txt.Length)
                        .ScrollToCaret()
                        'Debug.WriteLine(loc & " - " & txt.Length)
                        Me.txtSearch.Focus()
                    End If
                End With
            End If
        Else
            Me.Show()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        'Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Hide()
    End Sub

    Private Sub txtSearch_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Return Then
            OK_Button_Click(Me, New EventArgs)
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub txtSearch_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSearch.TextChanged
        If Me.txtSearch.BackColor <> Color.White Then Me.txtSearch.BackColor = Color.White
    End Sub

    Public Sub F3press()
        OK_Button_Click(Me, New EventArgs)
    End Sub

    Private Sub frmSearchBox_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not closeB AndAlso Not e.CloseReason = CloseReason.WindowsShutDown AndAlso Not e.CloseReason = CloseReason.TaskManagerClosing AndAlso Not e.CloseReason = CloseReason.FormOwnerClosing Then e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmSearchBox_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        Me.TopMost = True
        Me.txtSearch.Focus()
        Me.txtSearch.SelectionStart = 0
        Me.txtSearch.SelectionLength = Me.txtSearch.Text.Length
    End Sub
End Class
