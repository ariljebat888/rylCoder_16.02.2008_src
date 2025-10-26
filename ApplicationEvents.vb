Namespace My
    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Public fileToOpenOnStartup As String = ""
        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            AddMath.sharedDataOverWrite(My.Application.Info.DirectoryPath & "\rylCoder_hack.cfg")
            Dim s As Collections.ObjectModel.ReadOnlyCollection(Of String) = e.CommandLine
            Dim karg As String = ""
            For Each arg As String In s
                If arg = "-compile" Then 'for disabling karg's
                ElseIf arg = "-decompile" Then
                ElseIf karg = "-compile" Then
                    e.Cancel = True
                    Dim work As New frmWorking
                    work.Show()
                    Application.DoEvents()
                    Dim delete As Boolean = False
                    Dim fIn As String = arg
                    Dim fOut As String = arg.Substring(0, arg.Length - 3) & "mcf"
                    Dim sOut As IO.Stream = IO.File.OpenWrite(fOut)
                    If (sOut.Length > 0 AndAlso MsgBox("Do you want to overwrite the file '" & vbNewLine & fOut & "' ?", MsgBoxStyle.YesNo, "mcfCoder") = MsgBoxResult.Yes) OrElse sOut.Length < 1 Then
                        Dim script As New CScriptParser()
                        Application.DoEvents()
                        If script.File2struct(fIn) Then
                            Dim compiler As New CMcfCompiler()
                            Try
                                compiler.Compile(script.Functions)
                                Dim compData As Byte() = CMcfCoder.EnCryptArea(compiler.Data, CMcfCoder.Col.EFirstCol)
                                sOut.SetLength(0)
                                sOut.Write(compData, 0, compiler.Data.Length)
                                sOut.Flush()
                            Catch ex As Exception
                                delete = True
                                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "mcfCoder error")
                            End Try
                        Else
                            delete = True
                        End If
                    End If
                    sOut.Close()
                    If delete Then
                        My.Computer.FileSystem.DeleteFile(fOut)
                    End If
                    work.Close()
                ElseIf karg = "-decompile" Then
                    e.Cancel = True
                    Dim work As New frmWorking
                    work.Show()
                    Application.DoEvents()
                    Dim fIn As String = arg
                    Dim fOut As String = arg.Substring(0, arg.Length - 3) & "mcs"
                    Dim sIn As IO.Stream = IO.File.OpenRead(fIn)
                    If (My.Computer.FileSystem.FileExists(fOut) AndAlso MsgBox("Do you want to overwrite the file '" & vbNewLine & fOut & "' ?", MsgBoxStyle.YesNo, "mcfCoder") = MsgBoxResult.Yes) OrElse Not My.Computer.FileSystem.FileExists(fOut) Then
                        Dim dat(sIn.Length - 1) As Byte
                        sIn.Read(dat, 0, sIn.Length)
                        sIn.Close()
                        Application.DoEvents()
                        Dim decompiler As New CMcfDecompiler()
                        Try
                            dat = CMcfCoder.DeCryptArea(dat, CMcfCoder.Col.EFirstCol)
                            decompiler.Decompile(dat)
                            Dim scripter As New CScriptParser
                            If scripter.Struct2File(decompiler.Functions, fOut) Then

                            End If
                        Catch ex As Exception
                            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "mcfCoder error")
                        End Try
                    End If
                    work.Close()
                Else
                    If My.Computer.FileSystem.FileExists(arg) Then
                        fileToOpenOnStartup = arg
                    End If
                End If
                karg = arg
            Next
        End Sub
    End Class

End Namespace

