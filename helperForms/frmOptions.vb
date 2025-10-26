Imports System
Imports Microsoft.Win32
Imports System.Windows.Forms
Imports System.Reflection

Public Class frmOptions

    Private Sub frmOptions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.txtRylFolder.Text = frmNpcEdit.RylGameDir

        Me.chkFileTypeMcf.Checked = isAccotiated(".mcf")
        Me.chkFileTypeGsf.Checked = isAccotiated(".gsf")
        Me.chkFileTypeSkey.Checked = isAccotiated(".skey")
        Me.chkFileTypeGcmds.Checked = isAccotiated(".gcmds")
    End Sub

    Private Function fileTypeOpener(ByVal extension As String) As String
        Dim classes As RegistryKey = Registry.ClassesRoot
        Dim extK As RegistryKey = classes.OpenSubKey(extension, False)
        If Not extK Is Nothing Then
            Dim extV As String = extK.GetValue("", "").ToString()
            If extV <> "" Then
                Dim ext2K As RegistryKey = classes.OpenSubKey(extV & "\\shell\\open\\command", False)
                If Not ext2K Is Nothing Then
                    Return ext2K.GetValue("", "").ToString()
                End If
            End If
        End If
        Return ""
    End Function
    Private Sub setFileTypeOpener(ByVal extension As String, ByVal path As String)
        Dim classes As RegistryKey = Registry.ClassesRoot
        Dim shellKeyName As String = extension.Substring(1) & "_auto_file"
        Dim extK As RegistryKey = classes.CreateSubKey(extension)
        extK.SetValue("", shellKeyName)
        Dim a1 As RegistryKey = classes.CreateSubKey(shellKeyName)
        Dim a2 As RegistryKey = a1.CreateSubKey("shell")
        Dim a3 As RegistryKey = a2.CreateSubKey("open")
        Dim a4 As RegistryKey = a3.CreateSubKey("command")
        a4.SetValue("", """" & path & """ ""%1""")
        Dim a5 As RegistryKey = a1.CreateSubKey("DefaultIcon")
        a5.SetValue("", """" & path & """, 0")
    End Sub
    Private Sub deleteFileTypeOpener(ByVal extension As String)
        Dim classes As RegistryKey = Registry.ClassesRoot
        Dim extK As RegistryKey = classes.OpenSubKey(extension, False)
        If Not extK Is Nothing Then
            Dim extV As String = extK.GetValue("", "").ToString()
            If extV <> "" Then
                Try
                    classes.DeleteSubKeyTree(extV)
                Catch ex As ArgumentException
                End Try
            End If
            classes.DeleteSubKeyTree(extension)
        End If
    End Sub

    Private Function isAccotiated(ByVal extension As String) As Boolean
        Dim app As String = Assembly.GetEntryAssembly().CodeBase.ToLower().Substring("file:///".Length).Replace("/", "\")
        Dim exec As String = fileTypeOpener(extension).Replace("/", "\")
        Return exec.ToLower().Contains(app)
    End Function

    Private Sub btnSetRylFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetRylFolder.Click
        If Not IO.Directory.Exists(Me.txtRylFolder.Text) Then
            MsgBox("Directory doesnt exist", MsgBoxStyle.Exclamation, "rylCoder")
            Exit Sub
        End If
        Try
            AddMath.setRylFolder(Me.txtRylFolder.Text)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Can't set folder")
            Exit Sub
        End Try
        frmNpcEdit.RylGameDir = AddMath.getRylFolder()
    End Sub

    Private Sub manageExtensionOpener(ByVal extension As String, ByVal isChecked As Boolean)
        Dim app As String = Assembly.GetEntryAssembly().CodeBase.ToLower().Substring("file:///".Length).Replace("/", "\")
        Try
            If isChecked AndAlso Not isAccotiated(extension) Then
                Try
                    setFileTypeOpener(extension, app)
                Catch ex2 As Exception
                    Try
                        deleteFileTypeOpener(extension)
                        setFileTypeOpener(extension, app)
                    Catch ex3 As Exception
                        MsgBox(ex3.Message, MsgBoxStyle.Exclamation, "Cant set the extension opener for .mcf")
                    End Try
                End Try
            ElseIf Not isChecked AndAlso isAccotiated(extension) Then
                deleteFileTypeOpener(extension)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub btnSetFileTypes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetFileTypes.Click
        manageExtensionOpener(".mcf", Me.chkFileTypeMcf.Checked)
        manageExtensionOpener(".gsf", Me.chkFileTypeGsf.Checked)
        manageExtensionOpener(".skey", Me.chkFileTypeSkey.Checked)
        manageExtensionOpener(".gcmds", Me.chkFileTypeGcmds.Checked)

        Me.chkFileTypeMcf.Checked = isAccotiated(".mcf")
        Me.chkFileTypeGsf.Checked = isAccotiated(".gsf")
        Me.chkFileTypeSkey.Checked = isAccotiated(".skey")
        Me.chkFileTypeGcmds.Checked = isAccotiated(".gcmds")
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        dlgBrowse.SelectedPath = txtRylFolder.Text
        If dlgBrowse.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtRylFolder.Text = dlgBrowse.SelectedPath
        End If
    End Sub
End Class