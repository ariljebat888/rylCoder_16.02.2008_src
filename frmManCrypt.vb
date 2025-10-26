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

Public Class frmManCrypt

    Private Sub btnInBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInBrowse.Click
        If My.Computer.FileSystem.FileExists(Me.txtFileIn.Text) Then
            fileOpen.FileName = Me.txtFileIn.Text
        End If
        If fileOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtFileIn.Text = fileOpen.FileName
        End If
    End Sub

    Private Sub btnOutBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOutBrowse.Click
        'If My.Computer.FileSystem.FileExists(Me.txtFileOut.Text) Then
        fileSave.FileName = Me.txtFileOut.Text
        'End If
        If fileSave.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtFileOut.Text = fileSave.FileName
        End If

    End Sub

    Private Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGo.Click
        Me.btnGo.Enabled = False
        Me.lblStatus.Text = "Converting... Please wait a moment"
        If Not My.Computer.FileSystem.FileExists(Me.txtFileIn.Text) OrElse Me.txtFileOut.Text = "" Then Exit Sub
        Dim s As IO.FileStream
        Dim w As IO.FileStream
        Try
            s = My.Computer.FileSystem.GetFileInfo(Me.txtFileIn.Text).OpenRead()
            w = My.Computer.FileSystem.GetFileInfo(Me.txtFileOut.Text).OpenWrite()
        Catch ex As Exception
            MsgBox(ex.Message)
            Me.btnGo.Enabled = True
            Me.lblStatus.Text = "Error."
            Exit Sub
        End Try
        w.SetLength(0)
        Dim f1data(s.Length - 1) As Byte
        Dim out As Byte() = {}
        s.Read(f1data, 0, s.Length)
        s.Close()
        If Me.chkGsf.Checked Then
            If Me.chkEncrypt.Checked Then
                Dim gFile As New CGsfCoder.GsfFile
                gFile.type = CGsfCoder.getGsfType(f1data)
                Dim foundPos As Long = CGsfCoder.getFileSplitPos(f1data)
                Dim image(foundPos - 1) As Byte
                Dim gsfdata(f1data.Length - foundPos - 6) As Byte
                Array.ConstrainedCopy(f1data, 0, image, 0, image.Length)
                Array.ConstrainedCopy(f1data, foundPos + 5, gsfdata, 0, gsfdata.Length)
                gFile.picture = image
                gFile.gsfData = gsfdata
                out = CGsfCoder.Crypt(gFile)
            Else
                Dim gFile As CGsfCoder.GsfFile = CGsfCoder.DeCrypt(f1data)
                ReDim out(gFile.picture.Length + gFile.gsfData.Length + 4)
                Array.ConstrainedCopy(gFile.picture, 0, out, 0, gFile.picture.Length)
                Array.ConstrainedCopy(gFile.gsfData, 0, out, gFile.picture.Length + 5, gFile.gsfData.Length)
            End If
        Else
            If Me.chkEncrypt.Checked Then
                out = CMcfCoder.EnCryptArea(f1data)
            Else
                out = CMcfCoder.DeCryptArea(f1data)
            End If
        End If
        w.Write(out, 0, out.Length)
        w.Close()
        Me.btnGo.Enabled = True
        Me.lblStatus.Text = "Convert Complete."
    End Sub
End Class
