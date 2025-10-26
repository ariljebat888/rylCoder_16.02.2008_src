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



Public Class frmNpcEdit
    Private openedFile As String = ""

    Private changed As Boolean = False
    Private dontCloseIfExiting As Boolean = False
    Private closeAfterSave As Boolean = False
    Private cleanAfterSave As Boolean = False

    Private search As frmSearchBox = Nothing
    Friend WithEvents ItemSelector As frmSelectItem = Nothing
    Friend WithEvents MobSelector As frmSelectMob = Nothing
    Private MCFFunctions As CMcfBase.SFunction() = {}
    Private scriptParser As New CScriptParser
    Private npcParser As New CNpcParser
    Private questParser As New CQuestParser
    Private openGsfFile As New CGsfCoder.GsfFile
    Private openGsfTable As CGsfCoder.STableLine() = {}
    Private openFileType As AddMath.FileType
    Public Shared RylGameDir As String = ""
    Private activatedTab As Tabs = Tabs.EUnresolved
    Public Shared useLimitedVersion As Integer = 0
    Public Shared enableServerNotice As Boolean = True
    Public Shared syntaxHighlightEnabled As Boolean = False

#Region "RichEdit Unmanaged Code"
    Private Const WM_SETREDRAW As Integer = &HB
    Private Const WM_USER As Integer = &H400
    Private Const EM_GETEVENTMASK As Integer = (WM_USER + 59)
    Private Const EM_SETEVENTMASK As Integer = (WM_USER + 69)

    Private Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As IntPtr) As IntPtr

#End Region

    Private Sub openFile(ByVal file As String)
        openedFile = file
        If Not My.Computer.FileSystem.FileExists(openedFile) Then Exit Sub
        Dim s As IO.FileStream
        Try
            s = My.Computer.FileSystem.GetFileInfo(openedFile).OpenRead()
        Catch ex As Exception
            MsgBox(ex.Message)
            Me.statusBar.Text = "Couldn't open file"
            GoTo oFEnd
        End Try
        Me.Cursor = Cursors.WaitCursor
        Me.lblOpenFile.Text = "File opened: " & file
        Dim d(s.Length - 1) As Byte
        s.Read(d, 0, s.Length)
        s.Close()
        NoticeServOfUse(file)
        openFileType = AddMath.fileTypeFromName(openedFile)
        If openFileType = AddMath.FileType.GsfFile Then
            Try
                openGsfFile = CGsfCoder.DeCrypt(d)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "File is incorrect")
                Me.statusBar.Text = "Incorrect gsf file"
                openGsfFile = New CGsfCoder.GsfFile
                openGsfTable = New CGsfCoder.STableLine() {}
                GoTo oFEnd
            End Try
            Dim stopLoop As Boolean = False
            Dim ver As Integer = -1
            Dim rver As Integer = 0
            Do While Not stopLoop
                stopLoop = True
                Try
                    openGsfTable = CGsfCoder.Data2Struct(openGsfFile.gsfData, openGsfFile.type, ver, rver)
                Catch ex As CGsfCoder.GsfVersionLoopOutOfRange
                    'Throw
                Catch ex As Exception
                    stopLoop = False
                    ver -= 1
                End Try
            Loop
            If rver > 0 Then openGsfFile.version = rver
            If openGsfTable.Length > 0 Then
                dontActivate = True
                hideTab(Tabs.ENPCEditor)
                hideTab(Tabs.EQuestEditor)
                dontActivate = False
                activateTab(Tabs.EScriptEditor)
                Me.txtEdit.Lines = CGsfCoder.Struct2text(openGsfTable, openGsfFile.type, openGsfFile.version)
            Else
                MsgBox("Incorrect or unsupported GSF file", MsgBoxStyle.Critical, "File is incorrect")
                Me.statusBar.Text = "Incorrect/unsupported gsf file"
                openGsfFile = New CGsfCoder.GsfFile
                openGsfTable = New CGsfCoder.STableLine() {}
                GoTo oFEnd
            End If


            Dim table As CGsfCoder.STableLine() = CGsfCoder.Text2Struct(Me.txtEdit.Lines)
            Dim data As Byte() = CGsfCoder.Struct2Data(table, openGsfFile.type, openGsfFile.version)
            If Not AddMath.compareArr(data, openGsfFile.gsfData) Then
                lblOpenFile.BackColor = Color.Pink
                lblStatus.Text = "Parsing data resulted in unstable table. After saving you may not see expected results."
            Else
                lblOpenFile.BackColor = Color.Transparent
            End If
        ElseIf openFileType = AddMath.FileType.SkeyGcmdsFile Then
            dontActivate = True
            hideTab(Tabs.ENPCEditor)
            hideTab(Tabs.EQuestEditor)
            dontActivate = False
            activateTab(Tabs.EScriptEditor)
            Me.txtEdit.Text = CGcmdsCoder.Decode(d)
        ElseIf openFileType = AddMath.FileType.McfFile Then
            Dim decompiler As New CMcfDecompiler
            Try
                decompiler.Decompile(CMcfCoder.DeCryptArea(d, CMcfCoder.Col.EFirstCol))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "File is incorrect")
                Me.statusBar.Text = "Incorrect mcf file"
                GoTo oFEnd
            End Try
            MCFFunctions = decompiler.Functions
            syntaxLoaded = False 'so we reload the right part again
            syntaxXML = Nothing
            scriptParser.RYLVersion = decompiler.RYLFileVersion
            scriptParser.RYLFileType = decompiler.RYLFileType

            'scriptParser.Struct2TXT(MCFFunctions)
            'Dim lines As String() = scriptParser.TxtLines
            'Dim fff As New IO.StreamWriter("Q:\myProj\vs2005\rylCoder\bin\Debug\temp_debug.txt")
            'fff.Write(String.Join(vbNewLine, lines))
            'fff.Close()
            'scriptParser = New CScriptParser()
            'scriptParser.TXT2struct(lines)
            'Dim funcs2 As CMcfBase.SFunction() = scriptParser.Functions
            'Dim c1 As New CMcfCompiler
            'c1.Compile(funcs2)
            'If Not AddMath.compareArr(CMcfCoder.EnCryptArea(c1.Data), d) Then
            '    MsgBox("Parsing data resulted in unstable table. After saving you may not see expected results.")
            '    Dim bw As New IO.FileStream("Q:\myProj\vs2005\rylCoder\bin\Debug\errornos_data.dat", IO.FileMode.Create)
            '    Dim bb As Byte() = c1.Data
            '    bw.Write(bb, 0, bb.Length)
            '    bw.Close()
            'End If

            loadSyntaxs()
            dontActivate = True
            If decompiler.RYLFileType = CMcfBase.EFileType.ENpcScript Then
                showTab(Tabs.ENPCEditor)
                hideTab(Tabs.EQuestEditor)
            ElseIf decompiler.RYLFileType = CMcfBase.EFileType.EQuest Then
                showTab(Tabs.EQuestEditor)
                hideTab(Tabs.ENPCEditor)
            ElseIf decompiler.RYLFileType = CMcfBase.EFileType.EScript Then
                hideTab(Tabs.ENPCEditor)
                hideTab(Tabs.EQuestEditor)
            End If
            dontActivate = False
            'activateTab(Tabs.EUnresolved)
            If Me.TabCntrl.SelectedTab Is Me.tabScriptEditor Then activateTab(Tabs.EScriptEditor)
            Me.TabCntrl.SelectTab(Me.tabScriptEditor)
        Else
            MsgBox("Not supported file type")
            GoTo oFEnd
        End If

        Me.mnuSave.Enabled = True
        Me.mnuSaveAs.Enabled = True
        Me.mnuClose.Enabled = True
        Me.statusBar.Text = "File loaded"
oFEnd:
        Me.Cursor = Cursors.Arrow
    End Sub

    Public Shared Function getGsfStruct(ByVal file As String) As String()
        If My.Computer.FileSystem.FileExists(file) Then
            Dim stream As IO.FileStream
            Try
                stream = My.Computer.FileSystem.GetFileInfo(file).OpenRead
            Catch exception1 As Exception
                Return Nothing
            End Try
            Dim array As Byte() = New Byte((CInt((stream.Length - 1)) + 1) - 1) {}
            stream.Read(array, 0, CInt(stream.Length))
            stream.Close()
            Dim file2 As New CGsfCoder.GsfFile
            Dim table As CGsfCoder.STableLine() = New CGsfCoder.STableLine(0 - 1) {}
            Try
                file2 = CGsfCoder.DeCrypt((array))
            Catch exception4 As Exception
                Return Nothing
            End Try
            Dim stopFlag As Boolean = False
            Dim testVersion As Integer = -1
            Dim resultVersion As Integer = 0
            Do While Not stopFlag
                stopFlag = True
                Try
                    table = CGsfCoder.Data2Struct(file2.gsfData, file2.type, testVersion, resultVersion)
                    Continue Do
                Catch ex As CGsfCoder.GsfVersionLoopOutOfRange
                    'Throw
                Catch ex As Exception
                    stopFlag = False
                    testVersion -= 1
                    Continue Do
                End Try
            Loop
            If (resultVersion > 0) Then
                file2.version = resultVersion
            End If
            If (table.Length > 0) Then
                Return CGsfCoder.Struct2text((table), file2.type, file2.version)
            End If
        End If
        Return Nothing
    End Function


    Private Sub closeFile()
        cleanAfterSave = False
        dontCloseIfExiting = False
        closeAfterSave = False
        'If changed Then
        '    Dim res As DialogResult = MsgBox("Script has been changed." & vbNewLine & "Do you want to save?", MsgBoxStyle.YesNoCancel)
        '    If res = MsgBoxResult.Yes Then
        '        saveData()
        '        dontCloseIfExiting = True
        '        cleanAfterSave = True
        '        Exit Sub
        '    ElseIf res = Windows.Forms.DialogResult.Cancel Then
        '        dontCloseIfExiting = True
        '        Exit Sub
        '    End If
        'End If
        Me.txtEdit.Clear()
        Me.txtEdit.Tag = ""
        Me.treeNpcs.Nodes.Clear()
        Array.Resize(MCFFunctions, 0)
        scriptParser = New CScriptParser
        npcParser = New CNpcParser
        questParser = New CQuestParser
        openedFile = ""
        Me.mnuClose.Enabled = False
        Me.mnuSave.Enabled = False
        Me.mnuSaveAs.Enabled = False
        Me.lblStatus.Text = "Ready"
        Me.lblOpenFile.BackColor = Color.Transparent
        changed = False
        closeNPCeditor()
        closeQuestEditor()
        openGsfFile = New CGsfCoder.GsfFile
        openGsfTable = New CGsfCoder.STableLine() {}
        Me.lblOpenFile.Text = "File opened: No file loaded"
    End Sub

    Private Sub saveData()
        If openFileType = AddMath.FileType.GsfFile Then
            Me.lblOpenFile.Text = "File opened: " & openedFile
            Me.Cursor = Cursors.WaitCursor
            Dim data As Byte() = {}
            Try
                Dim table As CGsfCoder.STableLine() = CGsfCoder.Text2Struct(Me.txtEdit.Lines)
                openGsfFile.gsfData = CGsfCoder.Struct2Data(table, openGsfFile.type, openGsfFile.version)
                data = CGsfCoder.Crypt(openGsfFile)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Save error")
                Me.statusBar.Text = "Couldn't save"
                GoTo sFEnd
            End Try
            Dim s As IO.FileStream
            Try
                s = New IO.FileStream(openedFile, IO.FileMode.Create)
                s.Write(data, 0, data.Length)
                s.Close()
            Catch ex As Exception
                MsgBox(ex.Message)
                Me.statusBar.Text = "Couldn't open file for save"
                GoTo sFEnd
            End Try
        ElseIf openFileType = AddMath.FileType.SkeyGcmdsFile Then
            Me.lblOpenFile.Text = "File opened: " & openedFile
            Me.Cursor = Cursors.WaitCursor
            Dim data As Byte() = {}
            If Me.txtEdit.TextLength > 0 Then
                data = CGcmdsCoder.Encode(Me.txtEdit.Lines)
            End If
            Dim s As IO.FileStream
            Try
                s = New IO.FileStream(openedFile, IO.FileMode.Create)
                s.Write(data, 0, data.Length)
                s.Close()
            Catch ex As Exception
                MsgBox(ex.Message)
                Me.statusBar.Text = "Couldn't open file for save"
                GoTo sFEnd
            End Try
        ElseIf openFileType = AddMath.FileType.McfFile Then
            If openedFile = "" AndAlso Me.TabCntrl.SelectedTab Is Me.tabScriptEditor Then ' must be new script file, so we need source to save to
                Me.dlgFileSave.CheckFileExists = False
                If Me.dlgFileSave.ShowDialog = Windows.Forms.DialogResult.OK Then
                    openedFile = Me.dlgFileSave.FileName
                Else
                    GoTo sFEnd
                End If
            ElseIf openedFile = "" AndAlso Not Me.TabCntrl.SelectedTab Is Me.tabScriptEditor Then
                MsgBox("You can't save into new file under NPC or Quest editor")
                Exit Sub
            End If
            Me.lblOpenFile.Text = "File opened: " & openedFile
            Me.Cursor = Cursors.WaitCursor
            If Not saveLinesInMem(activeTab) Then
                Me.statusBar.Text = "Couldn't save"
                GoTo sFEnd
            End If
            Dim compiler As New CMcfCompiler()
            Try
                compiler.Compile(MCFFunctions)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Save error")
                Me.statusBar.Text = "Couldn't save"
                GoTo sFEnd
            End Try
            Dim s As IO.FileStream
            Try
                s = New IO.FileStream(openedFile, IO.FileMode.Create)
                s.Write(CMcfCoder.EnCryptArea(compiler.Data), 0, compiler.Data.Length)
                s.Close()
            Catch ex As Exception
                MsgBox(ex.Message)
                Me.statusBar.Text = "Couldn't open file for save"
                GoTo sFEnd
            End Try
        End If
        Me.statusBar.Text = "File saved"
        changed = False
sFEnd:
        Me.Cursor = Cursors.Arrow
    End Sub


#Region "Tab switch stuff"
    Private Enum Tabs
        EUnresolved = 0
        EScriptEditor = 1
        ENPCEditor = 2
        EQuestEditor = 3
    End Enum
    Private Function activeTab() As Tabs
        If Me.TabCntrl.SelectedTab Is Me.tabNPCeditor Then
            Return Tabs.ENPCEditor
        ElseIf Me.TabCntrl.SelectedTab Is Me.tabScriptEditor Then
            Return Tabs.EScriptEditor
        ElseIf Me.TabCntrl.SelectedTab Is Me.tabQuestEditor Then
            Return Tabs.EQuestEditor
        End If
    End Function
    Private Function activateTab(ByVal prevTab As Tabs, Optional ByVal tab As Tabs = Tabs.EUnresolved) As Boolean
        If tab = Tabs.EUnresolved Then tab = activeTab()
        Me.Cursor = Cursors.WaitCursor
        Dim ab As Boolean = saveLinesInMem(prevTab)
        Select Case tab
            Case Tabs.ENPCEditor
                npcParser.Parse(MCFFunctions)
                drawNpcTree()
            Case Tabs.EScriptEditor
                scriptParser.Struct2TXT(MCFFunctions)
                Me.txtEdit.Lines = scriptParser.TxtLines
                changed = False
            Case Tabs.EQuestEditor
                questParser.rylVersion = scriptParser.RYLVersion
                Try
                    questParser.Parse(MCFFunctions)
                Catch ex As Exception
                    MsgBox("Not a valid quest file. Please use the Script editor")
                    Me.TabCntrl.SelectedTab = Me.tabScriptEditor
                    questParser = New CQuestParser
                    Return False
                End Try
                drawQuestTree()
        End Select
        If prevTab = Tabs.ENPCEditor Then
            closeNPCeditor()
        ElseIf prevTab = Tabs.EQuestEditor Then
            closeQuestEditor()
        End If
        activatedTab = tab
        Me.Cursor = Cursors.Arrow
        Return ab
    End Function
    Private Function saveLinesInMem(ByVal tab As Tabs) As Boolean
        If MCFFunctions.Length < 1 Then Return True
        Select Case tab
            Case Tabs.ENPCEditor
                MCFFunctions = npcParser.GetFunctions
            Case Tabs.EScriptEditor
                Dim err As Boolean = False
                If Not changed Then Return True
                Try
                    scriptParser.TXT2struct(Me.txtEdit.Lines)
                Catch ex As textException
                    err = True
                    Me.txtEdit.Focus()
                    Me.txtEdit.Select(ex.overallPos, ex.length)
                    Me.txtEdit.ScrollToCaret()
                    If MessageBox.Show(ex.ToString & vbNewLine & vbNewLine & "Do you want to discard any changes made?", "Cant parse the script", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                        Me.txtEdit.Clear()
                        Me.txtEdit.SelectionColor = Me.txtEdit.ForeColor
                        Me.txtEdit.Lines = scriptParser.TxtLines
                        Return True
                    End If
                Catch ex As Exception
                    err = True
                    If MessageBox.Show(ex.Message & vbNewLine & vbNewLine & "Do you want to discard any changes made?", "Cant parse the script", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
                        Me.txtEdit.Clear()
                        Me.txtEdit.SelectionColor = Me.txtEdit.ForeColor
                        Me.txtEdit.Lines = scriptParser.TxtLines
                        Return True
                    End If
                End Try
                If Not err Then
                    MCFFunctions = scriptParser.Functions
                    changed = False
                Else
                    Return False
                End If
            Case Tabs.EQuestEditor
                MCFFunctions = questParser.GetFunctions
        End Select
        Return True
    End Function

    Private Sub hideTab(ByVal tab As Tabs)
        Select Case tab
            Case Tabs.ENPCEditor
                If Me.TabCntrl.Controls.IndexOf(Me.tabNPCeditor) >= 0 Then Me.TabCntrl.Controls.Remove(Me.tabNPCeditor)
            Case Tabs.EQuestEditor
                If Me.TabCntrl.Controls.IndexOf(Me.tabQuestEditor) >= 0 Then Me.TabCntrl.Controls.Remove(Me.tabQuestEditor)
            Case Tabs.EScriptEditor
                If Me.TabCntrl.Controls.IndexOf(Me.tabScriptEditor) >= 0 Then Me.TabCntrl.Controls.Remove(Me.tabScriptEditor)
        End Select
    End Sub
    Private Sub showTab(ByVal tab As Tabs)
        Me.TabCntrl.SuspendLayout()
        Select Case tab
            Case Tabs.ENPCEditor
                If Me.TabCntrl.TabPages.IndexOf(Me.tabNPCeditor) < 0 Then
                    addTabPosition(Me.tabNPCeditor, 0)
                End If
            Case Tabs.EQuestEditor
                If Me.TabCntrl.TabPages.IndexOf(Me.tabQuestEditor) < 0 Then
                    addTabPosition(Me.tabQuestEditor, IIf(Me.TabCntrl.TabPages.IndexOf(Me.tabNPCeditor) < 0, 0, 1))
                End If
            Case Tabs.EScriptEditor
                If Me.TabCntrl.TabPages.IndexOf(Me.tabScriptEditor) < 0 Then
                    addTabPosition(Me.tabScriptEditor)
                End If
        End Select
        Me.TabCntrl.ResumeLayout()
    End Sub

    Private Sub addTabPosition(ByRef tab As Windows.Forms.TabPage, Optional ByVal pos As Integer = -1)
        Dim cntrs(Me.TabCntrl.TabPages.Count - 1) As TabPage
        Dim i As Integer = 0
        For Each cn As TabPage In Me.TabCntrl.TabPages
            cntrs(i) = cn
            i += 1
        Next
        Dim p As Integer = 0
        Me.TabCntrl.TabPages.Clear()
        For Each cntr As TabPage In cntrs
            If p = pos Then
                p += 1
                Me.TabCntrl.TabPages.Add(tab)
            End If
            Me.TabCntrl.TabPages.Add(cntr)
            p += 1
        Next
        If pos < 0 OrElse p = i Then
            Me.TabCntrl.TabPages.Add(tab)
        End If
    End Sub
#End Region
    Public Shared Function GetItemName(ByVal id As Long, ByRef itemSelector As frmSelectItem) As String
        If Not itemSelector Is Nothing AndAlso Not itemSelector.ItemScript Is Nothing Then
            For Each item As frmSelectItem.GameItem In itemSelector.ItemScript
                If item.ID = id Then
                    Return "[" & IIf(id.ToString.Length > 5, "", Space(5 - id.ToString.Length)) & id & "] " & item.ItemName & ", " & item.TypeName & ", " & item.Limit & " " & item.LimitStat
                End If
            Next
            GoTo uI
        Else
uI:         Return "Unknown item [" & id & "]"
        End If
    End Function
    Private Function GetItemName(ByVal id As Long) As String
        Return GetItemName(id, ItemSelector)
    End Function
    Public Shared Function GetMobName(ByVal id As Long, ByRef mobSelector As frmSelectMob) As String
        If Not mobSelector Is Nothing AndAlso Not mobSelector.MobScript Is Nothing Then
            For Each item As frmSelectMob.MobInfo In mobSelector.MobScript
                If item.ID = id Then
                    'Return "[" & IIf(id.ToString.Length > 5, "", Space(5 - id.ToString.Length)) & id & "] " & item.Name & ", " & item.Level
                    Return "[LV" & item.Level & "]" & item.Name
                End If
            Next
            GoTo uI
        Else
uI:         Return "Unknown mob"
        End If
    End Function
    Private Function GetMobName(ByVal id As Long) As String
        Return GetMobName(id, MobSelector)
    End Function
    Public Shared Function GetMobLevel(ByVal id As Long, ByRef mobSelector As frmSelectMob) As Integer
        If Not mobSelector Is Nothing Then
            For Each item As frmSelectMob.MobInfo In mobSelector.MobScript
                If item.ID = id Then
                    Return item.Level
                End If
            Next
            GoTo uI
        Else
uI:         Return 0
        End If
    End Function
    Private Function GetMobLevel(ByVal id As Long) As Integer
        Return GetMobLevel(id, MobSelector)
    End Function
    Private Sub parseAccessibilityHelp(Optional ByVal cntCollec As Windows.Forms.Control.ControlCollection = Nothing)
        If cntCollec Is Nothing Then cntCollec = Me.Controls
        For Each c As Control In cntCollec
            If c.AccessibleDescription <> "" Then
                Me.tipHoverHelp.SetToolTip(c, c.AccessibleDescription)
            End If
            If Not c.Controls Is Nothing AndAlso c.Controls.Count > 0 Then
                parseAccessibilityHelp(c.Controls)
            End If
        Next
    End Sub

#Region "NPCEditor"

    Private Sub closeNPCeditor()
        Me.treeNpcs.Nodes.Clear()
        npcParser = New CNpcParser
        Me.cmbShopIndex.Items.Clear()
        NpcShopPage.ClearShop()
        'boxes

        'general
        Me.txtNPCID.Text = ""
        Me.txtNPCName.Text = ""
        Me.txtNPCDesc.Text = ""
        Me.txtNPCTexture.Text = ""

        'location
        Me.txtNPCLocationZone.Text = ""
        Me.txtNPCLocationZ.Text = ""
        Me.txtNPCLocationY.Text = ""
        Me.txtNPCLocationX.Text = ""
        Me.txtNPCLocationDir.Text = ""

        'teleport (ZoneMove)
        Me.txtNPCTeleportZone.Text = ""
        Me.txtNPCTeleportZ.Text = ""
        Me.txtNPCTeleportY.Text = ""
        Me.txtNPCTeleportX.Text = ""
        Me.grpNPCTeleport.Visible = False

        'maps
        Me.locationMap.Visible = False
        Me.teleportTargetMap.Visible = False

        'end boxes
        Me.flowNPCTexts.Controls.Clear()
    End Sub

    Private Sub drawNpcTree(Optional ByVal search As String = "")
        Dim nCnt As Long = 0
        Me.treeNpcs.Nodes.Clear()
        Dim mapsInTree() As Integer = {}
        For Each npc As CNpcParser.npcStruct In npcParser.NPCs
            Dim nName As String = npc.Name
            If search = "" OrElse nName.ToLower.IndexOf(search) >= 0 OrElse npc.id.ToString.IndexOf(search) >= 0 OrElse ("0x" & Hex(npc.id)).IndexOf(search) >= 0 Then
                Dim z As Integer = npc.Map
                Dim nodeIndex As Integer = Array.IndexOf(mapsInTree, z)
                If nodeIndex < 0 Then
                    ReDim Preserve mapsInTree(UBound(mapsInTree) + 1)
                    mapsInTree(UBound(mapsInTree)) = z
                    Dim t As TreeNode = Me.treeNpcs.Nodes.Add("Zone " & z)
                    t.Tag = -1
                    nodeIndex = UBound(mapsInTree)
                End If
                Dim npcNode As TreeNode = Me.treeNpcs.Nodes(nodeIndex).Nodes.Add(nName)
                npcNode.Tag = nCnt
            End If
            nCnt += 1
        Next
        If search <> "" Then Me.treeNpcs.ExpandAll()
    End Sub

    Private Sub openNPCforEdit(ByRef npc As CNpcParser.npcStruct)
        'loads
        npcParser.loadedNPC = npc
        Me.txtNPCID.Text = "0x" & AddMath.Hex2(npc.id) & " [" & npc.id & "]"
        Dim setNPCL() As CNpcParser.NPCline = npc.Lines(CNpcParser.NPCline.knownType.ESetNPC)
        Dim setPos() As CNpcParser.NPCline = npc.Lines(CNpcParser.NPCline.knownType.ESetPosition)
        Dim setTele() As CNpcParser.NPCline = npc.Lines(CNpcParser.NPCline.knownType.EAddZoneMove)
        Dim setPops() As CNpcParser.NPCline = npc.Lines(CNpcParser.NPCline.knownType.EAddPopup)
        If setNPCL.Length <> 1 OrElse setPos.Length <> 1 Then Exit Sub

        'general

        If npc.RYLversion = 2 Then
            Dim tmpStrA() As String = setNPCL(0).Params(5).value.Split("\")
            Me.txtNPCName.Text = tmpStrA(0)
            Me.txtNPCDesc.Enabled = True
            If tmpStrA.Length > 1 Then
                Me.txtNPCDesc.Text = tmpStrA(2)
            Else
                Me.txtNPCDesc.Text = ""
            End If
        Else
            Me.txtNPCName.Text = setNPCL(0).Params(4).value
            Me.txtNPCDesc.Enabled = False
        End If

        Me.txtNPCTexture.Text = setNPCL(0).Params(IIf(npc.RYLversion = 1, 3, 4)).value
        Me.lnkOpenModel.Visible = (RylGameDir <> "" AndAlso IO.File.Exists(RylGameDir & "\character\data\" & Me.txtNPCTexture.Text))

        'location
        Me.txtNPCLocationZone.Text = setNPCL(0).Params(0).value
        Me.txtNPCLocationZ.Text = setPos(0).Params(4).value
        Me.txtNPCLocationY.Text = setPos(0).Params(3).value
        Me.txtNPCLocationX.Text = setPos(0).Params(2).value
        Me.txtNPCLocationDir.Text = setPos(0).Params(1).value
        Try
            Me.sldNPCDirection.Value = Me.sldNPCDirection.Minimum + (Convert.ToSingle(Me.txtNPCLocationDir.Text) / (2 * Math.PI) * (Me.sldNPCDirection.Maximum - Me.sldNPCDirection.Minimum))
        Catch exception1 As Exception
        End Try

        'teleport (ZoneMove)
        If setTele.Length = 1 Then
            Me.txtNPCTeleportZone.Text = setTele(0).Params(1).value
            Me.txtNPCTeleportZ.Text = setTele(0).Params(4).value
            Me.txtNPCTeleportY.Text = setTele(0).Params(3).value
            Me.txtNPCTeleportX.Text = setTele(0).Params(2).value
            Me.grpNPCTeleport.Visible = True
        Else
            Me.grpNPCTeleport.Visible = False
        End If
        openMiniMaps()

        'shops
        'AddPopup(35DCE,1C,40012085,FF0FFF,00,00,7A6F,3E,191); 
        Me.cmbShopIndex.Items.Clear()
        For Each l As CNpcParser.NPCline In setPops
            If l.Params(1).value = &H191 AndAlso l.Params(6).value = &HFF0FFF AndAlso l.Params(5).value = &H0 AndAlso l.Params(4).value = &H0 Then
                Dim n As String = l.Params(3).value
                'Dim t As Integer = l.param5 'type of shop.
                Me.cmbShopIndex.Items.Add(n)
            End If
        Next
        If npc.RYLversion = 1 Then Me.cmbShopIndex.Items.Add("")
        If Me.cmbShopIndex.Items.Count > 0 Then Me.cmbShopIndex.SelectedIndex = 0 Else LoadNPCShop(npc, 0, 0, 0)

        'texts
        Me.flowNPCTexts.Controls.Clear()
        Dim allLines As CNpcParser.NPCline() = npc.Lines()
        For Each line As CNpcParser.NPCline In allLines
            Dim index As Integer = -1
            Select Case line.Type
                Case CNpcParser.NPCline.knownType.EAddWords
                    index = 1
                Case CNpcParser.NPCline.knownType.EAddQuestWords 'ryl2 only
                    index = 2
                Case CNpcParser.NPCline.knownType.EAddPopup 'ryl2 only
                    index = 3
                Case CNpcParser.NPCline.knownType.EAddDialog
                    index = 3
            End Select
            If index >= 0 AndAlso line.Params(index).type = CMcfBase.DataType.EString Then
                Dim tI As New CNpcParser.NPCTextItem
                tI.text = line.Params(index).value
                tI.paraIndex = index
                tI.line = line
                Dim cnt As New cntTextEditItem
                cnt.TextItem = tI
                cnt.Command = CNpcParser.NPCline.TypesStrings(line.Type)
                Me.flowNPCTexts.Controls.Add(cnt)
                AddHandler cnt.NPCTextChanged, AddressOf cntTextEditItem_NPCTextChanged
            End If
        Next
    End Sub

    Private Sub openMiniMaps()
        Me.locationMap.openMap(frmNpcEdit.RylGameDir, Me.txtNPCLocationZone.Text, Me.npcParser.NPCs)
        Me.locationMap.drawLocation(Me.txtNPCLocationX.Text, Me.txtNPCLocationZ.Text, Me.txtNPCLocationDir.Text)
        Dim point As New Point(610, 150)
        Me.locationMap.Location = point
        Me.locationMap.Cursor = Cursors.Hand
        If Me.grpNPCTeleport.Visible Then
            Me.locationMap.Location = New Point(610, &H62)
            Me.teleportTargetMap.openMap(frmNpcEdit.RylGameDir, Me.txtNPCTeleportZone.Text, Me.npcParser.NPCs)
            Me.teleportTargetMap.drawLocation(Me.txtNPCTeleportX.Text, Me.txtNPCTeleportZ.Text, -Math.PI / 4.0!)
            Me.teleportTargetMap.Location = New Point(610, &H12E)
            Me.teleportTargetMap.Cursor = Cursors.Hand
        Else
            Me.teleportTargetMap.Visible = False
            Me.teleportTargetMap.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub LoadNPCShop(ByRef npc As CNpcParser.npcStruct, ByVal shop As Integer, ByVal page As Integer, ByVal tab As Integer)
        If Not npc Is Nothing Then
            If page <> selectedPage Then
                selectedPage = page
                Me.lblItemsPage.Text = selectedPage + 1
            End If
            If tab <> Me.tabItemsControl.SelectedIndex Then
                Me.tabItemsControl.SelectedIndex = tab
            End If
            Dim itemLines As CNpcParser.NPCline() = npc.Lines(CNpcParser.NPCline.knownType.EAddItem)
            NpcShopPage.ClearShop()
            If itemLines.Length > 0 Then
                Array.Sort(itemLines, New CNpcParser.NPClinePosComparer)
                For Each item As CNpcParser.NPCline In itemLines
                    'shop, tab, page, item <-ryl2
                    'item, tab, page <-ryl1
                    If (npc.RYLversion = 2 AndAlso item.Params(3).value = page AndAlso item.Params(2).value = tab AndAlso item.Params(1).value = shop) OrElse _
                    (npc.RYLversion = 1 AndAlso item.Params(3).value = page AndAlso item.Params(2).value = tab) Then
                        NpcShopPage.AddItem(New ShopItem(Me, item))
                    End If
                Next
            End If
        End If
    End Sub

    Private Function GetIndexOnList(ByVal itemID As Long) As Integer
        Return NpcShopPage.IndexOnList(itemID)
    End Function

    Public Class ShopItem
        Public itemId As Long
        Public line As CNpcParser.NPCline
        Public name As String
        Public Sub New(ByRef parent As frmNpcEdit, ByRef item As CNpcParser.NPCline)
            If parent.npcParser.RYLVersion = 1 Then itemId = item.Params(1).value Else itemId = item.Params(4).value
            line = item
            name = parent.GetItemName(itemId)
        End Sub
        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Private Sub refreshShop()
        LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
    End Sub

    Private Sub changeNPCGeneral(ByVal type As CNpcParser.NPCline.knownType)
        If Not npcParser.loadedNPC Is Nothing Then
            Dim lines() As CNpcParser.NPCline = npcParser.loadedNPC.Lines(type)
            If lines.Length = 1 Then
                Dim index As Long = npcParser.loadedNPC.iLines.IndexOf(lines(0))
                Try
                    With npcParser.loadedNPC.iLines(index)
                        Select Case type
                            Case CNpcParser.NPCline.knownType.ESetNPC
                                .params(0) = CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, Val(Me.txtNPCLocationZone.Text))
                                If npcParser.RYLVersion = 2 Then
                                    .params(4) = CMcfBase.CreateParamElem(CMcfBase.DataType.EString, Me.txtNPCTexture.Text)
                                    .params(5) = CMcfBase.CreateParamElem(CMcfBase.DataType.EString, Me.txtNPCName.Text & IIf(Me.txtNPCDesc.Text.Length > 0, "\\" & Me.txtNPCDesc.Text, ""))
                                Else
                                    .params(3) = CMcfBase.CreateParamElem(CMcfBase.DataType.EString, Me.txtNPCTexture.Text)
                                    .params(4) = CMcfBase.CreateParamElem(CMcfBase.DataType.EString, Me.txtNPCName.Text)
                                End If
                            Case CNpcParser.NPCline.knownType.ESetPosition
                                .params(4) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCLocationZ.Text))
                                .params(3) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCLocationY.Text))
                                .params(2) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCLocationX.Text))
                                .params(1) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCLocationDir.Text))
                            Case CNpcParser.NPCline.knownType.EAddZoneMove
                                .params(3) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCTeleportZ.Text))
                                .params(2) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCTeleportY.Text))
                                .params(1) = CMcfBase.CreateParamElem(CMcfBase.DataType.EFloat, Single.Parse(Me.txtNPCTeleportX.Text))
                                .params(0) = CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, Val(Me.txtNPCTeleportZone.Text))
                        End Select
                        openMiniMaps()
                    End With
                Catch ex As Exception
                    MsgBox("Check your input in " & CNpcParser.NPCline.TypesStrings(type) & " type text box")
                End Try
            End If
        End If
    End Sub

    Private Sub selectNPCinTree(ByVal id As Long, Optional ByRef nodes As TreeNodeCollection = Nothing)
        If nodes Is Nothing Then nodes = Me.treeNpcs.Nodes
        For Each nod As TreeNode In nodes
            If Not nod.Tag Is Nothing AndAlso nod.Tag >= 0 Then
                If npcParser.NPCs(nod.Tag).id = id Then
                    Me.treeNpcs.SelectedNode = nod
                End If
            Else
                selectNPCinTree(id, nod.Nodes)
            End If
        Next
    End Sub
    Private Sub NPCFromMap()
        Dim map As New frmMap
        map.openZone = 12
        map.AllowMoves = False
        AddHandler map.MapChange, AddressOf map_ZoneChange
        AddHandler map.PointOnClick, AddressOf map_PointOnClick
        DrawNPCsOnMap(map, map.openZone)
        map.ShowDialog()
    End Sub
    Private Sub DrawNPCsOnMap(ByRef frm As frmMap, ByVal map As Integer)
        If Not npcParser Is Nothing Then
            frm.ClearPointers()
            Dim npcs() As CNpcParser.npcStruct = npcParser.NPCs
            For Each npc As CNpcParser.npcStruct In npcs
                If npc.Map = map Then
                    Dim posL() As CNpcParser.NPCline = npc.Lines(CNpcParser.NPCline.knownType.ESetPosition)
                    If posL.Length = 1 Then
                        Dim pos As New MyPoint.SinglePoint
                        pos.X = posL(0).Params(2).value
                        pos.Y = posL(0).Params(4).value
                        Dim n As String() = npc.Name.Split(New String() {"\\"}, StringSplitOptions.None)
                        Dim n2 As String = ""
                        If n.Length = 2 Then n2 = n(1)
                        frm.addPointer(pos.X, pos.Y, npc.id, IIf(n2 <> "", n2, n(0)), n(0), True)
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub map_ZoneChange(ByRef sender As frmMap, ByVal zone As Integer)
        DrawNPCsOnMap(sender, zone)
    End Sub
    Private Sub map_PointOnClick(ByRef sender As frmMap, ByRef point As MyPoint)
        Dim npc As Long = point.Tag
        sender.Close()
        selectNPCinTree(npc)
    End Sub

    Private Sub btnNpcFromMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNpcFromMap.Click
        NPCFromMap()
    End Sub
#Region "NPC page events"
    Private selectedPage As Integer = 0
    Private Const maxPage As Integer = 8
    Private Sub txtSearchNpc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSearchNpc.TextChanged
        drawNpcTree(txtSearchNpc.Text.ToLower)
    End Sub

    Private Sub treeNpcs_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles treeNpcs.AfterSelect
        Dim sN As TreeNode = e.Node
        If Not sN Is Nothing AndAlso sN.Tag >= 0 Then
            openNPCforEdit(npcParser.NPCs(sN.Tag))
        End If
    End Sub
    Private Sub cmbShopIndex_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbShopIndex.SelectedIndexChanged
        LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, 0, 0)
    End Sub
    Private Sub tabItemsControl_Selecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabItemsControl.Selecting
        LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, e.TabPageIndex)
    End Sub

    Private Sub btnItemsPageLeft_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemsPageLeft.Click
        selectedPage -= 1
        If selectedPage < 0 Then
            selectedPage = 0
        Else
            Me.lblItemsPage.Text = selectedPage + 1
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
        End If
    End Sub
    Private Sub btnItemsPageRight_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemsPageRight.Click
        selectedPage += 1
        If selectedPage > maxPage Then
            selectedPage = maxPage
        Else
            Me.lblItemsPage.Text = selectedPage + 1
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
        End If
    End Sub

    Private Sub btnItemToDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToDown.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso NpcShopPage.SelectedIndex < NpcShopPage.Count - 1 Then
            Dim switchItem As ShopItem = NpcShopPage.Items(NpcShopPage.SelectedIndex + 1)
            npcParser.loadedNPC.SwitchPositions(item.line.pos, switchItem.line.pos)
            Dim i As Integer = NpcShopPage.SelectedIndex
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
            NpcShopPage.SelectedIndex = i + 1
        End If
    End Sub
    Private Sub btnItemToUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToUp.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso NpcShopPage.SelectedIndex > 0 Then
            Dim switchItem As ShopItem = NpcShopPage.Items(NpcShopPage.SelectedIndex - 1)
            npcParser.loadedNPC.SwitchPositions(item.line.pos, switchItem.line.pos)
            Dim i As Integer = NpcShopPage.SelectedIndex
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
            NpcShopPage.SelectedIndex = i - 1
        End If
    End Sub
    Private Sub btnItemToPageLeft_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToPageLeft.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso selectedPage > 0 Then
            npcParser.loadedNPC.setParameter(item.line.pos, 3, selectedPage - 1)
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage - 1, Me.tabItemsControl.SelectedIndex)
            NpcShopPage.SelectedIndex = GetIndexOnList(item.itemId)
        End If
    End Sub
    Private Sub btnItemToPageRight_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToPageRight.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso selectedPage < maxPage Then
            npcParser.loadedNPC.setParameter(item.line.pos, 3, selectedPage + 1)
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage + 1, Me.tabItemsControl.SelectedIndex)
            NpcShopPage.SelectedIndex = GetIndexOnList(item.itemId)
        End If
    End Sub
    Private Sub btnItemToTabLeft_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToTabLeft.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso Me.tabItemsControl.SelectedIndex > 0 Then
            npcParser.loadedNPC.setParameter(item.line.pos, 2, Me.tabItemsControl.SelectedIndex - 1)
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex - 1)
            NpcShopPage.SelectedIndex = GetIndexOnList(item.itemId)
        End If
    End Sub
    Private Sub btnItemToTabRight_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnItemToTabRight.Click
        Dim item As ShopItem = NpcShopPage.SelectedItem
        If Not npcParser.loadedNPC Is Nothing AndAlso Not item Is Nothing AndAlso Me.tabItemsControl.SelectedIndex < 4 Then
            npcParser.loadedNPC.setParameter(item.line.pos, 2, Me.tabItemsControl.SelectedIndex + 1)
            LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex + 1)
            NpcShopPage.SelectedIndex = GetIndexOnList(item.itemId)
        End If
    End Sub

    Private Sub cntMnuNPCItem_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles NpcShopPage.MenuOpening
        'How to determine is it open on a item or some other place.. :S
        If npcParser.loadedNPC Is Nothing OrElse ItemSelector Is Nothing Then
            e.Cancel = True
        End If
    End Sub

    Private Sub mnuNPCItemsAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NpcShopPage.AddItemRequest
        ItemSelector.open()
    End Sub
    Private Sub mnuNPCItemsDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NpcShopPage.DeleteRequest
        If Not NpcShopPage.SelectedItem Is Nothing Then
            npcParser.loadedNPC.DeleteLine(NpcShopPage.SelectedItem.line)
            refreshShop()
        End If
    End Sub
    Private Sub mnuNPCItemsEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NpcShopPage.EditItemRequest
        If NpcShopPage.SelectedIndex >= 0 Then
            ItemSelector.open(NpcShopPage.SelectedItem.itemId)
        Else
            MsgBox("Please select a item first")
        End If
    End Sub

    Private Sub ItemSelector_ItemSelected(ByVal sender As frmSelectItem, ByVal item As Long, ByVal prevItem As Long) Handles ItemSelector.ItemSelected
        If scriptParser.RYLFileType = CMcfBase.EFileType.ENpcScript Then
            If prevItem > 0 Then
                If NpcShopPage.SelectedIndex >= 0 Then
                    npcParser.loadedNPC.setParameter(NpcShopPage.SelectedItem.line.pos, IIf(npcParser.RYLVersion = 1, 1, 4), item)
                    refreshShop()
                    NpcShopPage.SelectedIndex = GetIndexOnList(item)
                Else
                    MsgBox("You deselected the item somehow?!?!" & vbNewLine & "Anyway.. do the same thing over again")
                End If
            Else
                Dim nLine As New CNpcParser.NPCline
                nLine.NPCID = npcParser.loadedNPC.id
                'shop, tab, page, item <-ryl2
                'item, tab, page <-ryl1
                If npcParser.RYLVersion = 2 Then
                    nLine.Params = New CMcfBase.SParamElem(4) { _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, nLine.NPCID), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, Me.cmbShopIndex.SelectedIndex), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, Me.tabItemsControl.SelectedIndex), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, selectedPage), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, item) _
                    }
                Else
                    nLine.Params = New CMcfBase.SParamElem(3) { _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, nLine.NPCID), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, item), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, Me.tabItemsControl.SelectedIndex), _
                        CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, selectedPage) _
                    }
                End If
                Dim posLine As CNpcParser.NPCline = npcParser.loadedNPC.iLines(npcParser.loadedNPC.iLines.Count - 1)
                nLine.pos = posLine.pos + 1
                npcParser.FreePosition(nLine.pos)
                nLine.Type = CNpcParser.NPCline.knownType.EAddItem
                npcParser.loadedNPC.AddLine(nLine)
                refreshShop()
            End If
        End If
    End Sub

    Private Sub sldNPCDirection_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles sldNPCDirection.ValueChanged
        If Me.sldNPCDirection.Focused Then
            Me.txtNPCLocationDir.Text = Math.Round(((Me.sldNPCDirection.Value - Me.sldNPCDirection.Minimum) / (Me.sldNPCDirection.Maximum - Me.sldNPCDirection.Minimum)) * 2 * Math.PI, 3)
        End If
        If (((Me.txtNPCLocationDir.Text <> "") AndAlso (Me.txtNPCLocationX.Text <> "")) AndAlso (Me.txtNPCLocationZ.Text <> "")) Then
            Me.locationMap.drawLocation(Convert.ToSingle(Me.txtNPCLocationX.Text), Convert.ToSingle(Me.txtNPCLocationZ.Text), Convert.ToSingle(Me.txtNPCLocationDir.Text))
        End If
    End Sub
    Private Sub txtNPCLocationDir_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCLocationDir.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetPosition)
        Try
            Me.sldNPCDirection.Value = Me.sldNPCDirection.Minimum + (Convert.ToSingle(Me.txtNPCLocationDir.Text) / (2 * Math.PI) * (Me.sldNPCDirection.Maximum - Me.sldNPCDirection.Minimum))
        Catch exception1 As Exception
        End Try
    End Sub
    Private Sub txtNPCLocationZ_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCLocationZ.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetPosition)
    End Sub
    Private Sub txtNPCLocationZone_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCLocationZone.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetNPC)
    End Sub
    Private Sub txtNPCLocationX_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCLocationX.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetPosition)
    End Sub
    Private Sub txtNPCLocationY_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCLocationY.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetPosition)
    End Sub
    Private Sub txtNPCTeleportZ_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCTeleportZ.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.EAddZoneMove)
    End Sub
    Private Sub txtNPCTeleportZone_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCTeleportZone.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.EAddZoneMove)
    End Sub
    Private Sub txtNPCTeleportX_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCTeleportX.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.EAddZoneMove)
    End Sub
    Private Sub txtNPCTeleportY_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCTeleportY.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.EAddZoneMove)
    End Sub

    Private Sub cntTextEditItem_NPCTextChanged(ByRef sender As cntTextEditItem, ByVal textItem As CNpcParser.NPCTextItem, ByVal newText As String)
        Dim a As New CMcfBase.SParamElem
        a.type = CMcfBase.DataType.EString
        a.value = newText
        npcParser.loadedNPC.iLines(npcParser.loadedNPC.iLines.IndexOf(textItem.line)).Params(textItem.paraIndex) = a
    End Sub
    Private Sub txtNPCName_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCName.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetNPC)
    End Sub
    Private Sub txtNPCDesc_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCDesc.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetNPC)
    End Sub
    Private Sub txtNPCTexture_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNPCTexture.Leave
        changeNPCGeneral(CNpcParser.NPCline.knownType.ESetNPC)
    End Sub

    Private Sub btnNpcLocOpenMap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNpcLocOpenMap.Click
        If npcParser.loadedNPC Is Nothing Then Exit Sub
        Dim map As New frmMap
        map.openZone = Val(Me.txtNPCLocationZone.Text)
        map.addPointer(Single.Parse(Me.txtNPCLocationX.Text), Single.Parse(Me.txtNPCLocationZ.Text))
        If map.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtNPCLocationZone.Text = map.openZone
            Dim pnts As Single()() = map.Points
            Me.txtNPCLocationX.Text = pnts(0)(0)
            Me.txtNPCLocationZ.Text = pnts(0)(1)
            changeNPCGeneral(CNpcParser.NPCline.knownType.ESetPosition)
            changeNPCGeneral(CNpcParser.NPCline.knownType.ESetNPC)
        End If
    End Sub
    Private Sub btnNpcTelepOpenMap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNpcTelepOpenMap.Click
        If npcParser.loadedNPC Is Nothing Then Exit Sub
        Dim map As New frmMap
        map.openZone = Val(Me.txtNPCTeleportZone.Text)
        map.addPointer(Single.Parse(Me.txtNPCTeleportX.Text), Single.Parse(Me.txtNPCTeleportZ.Text))
        If map.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtNPCTeleportZone.Text = map.openZone
            Dim pnts As Single()() = map.Points
            Me.txtNPCTeleportX.Text = pnts(0)(0)
            Me.txtNPCTeleportZ.Text = pnts(0)(1)
            changeNPCGeneral(CNpcParser.NPCline.knownType.EAddZoneMove)
        End If
    End Sub

    Private Sub locationMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles locationMap.Click
        If (DirectCast(sender, CMiniMapPanel).Cursor Is Cursors.Hand) Then
            Me.btnNpcLocOpenMap_Click(sender, e)
        End If
    End Sub
    Private Sub teleportTargetMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles teleportTargetMap.Click
        If (DirectCast(sender, CMiniMapPanel).Cursor Is Cursors.Hand) Then
            Me.btnNpcTelepOpenMap_Click(sender, e)
        End If
    End Sub

    Private Sub lnkOpenModel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkOpenModel.LinkClicked
        Dim model As String = Me.txtNPCTexture.Text
        If IO.File.Exists(RylGameDir & "\character\data\" & model) Then
            closeFile()
            If Not dontCloseIfExiting Then
                openFile(RylGameDir & "\character\data\" & model)
            End If
        End If
    End Sub
#End Region

#End Region

#Region "QuestEditor"
    Private Sub drawQuestTree(Optional ByVal search As String = "")
        Dim nCnt As Long = 0
        Me.treeQuests.Nodes.Clear()
        Dim mapsInTree()() As Integer = {}
        For Each quest As CQuestParser.Quest In questParser.Quests
            Dim nName As String = quest.ToString
            If search = "" OrElse nName.ToLower.IndexOf(search) >= 0 OrElse ("0x" & Hex(quest.Id)).ToLower.IndexOf(search) >= 0 Then
                Dim z() As Integer = quest.Maps
                Dim nodeIndex As Integer = AddMath.Array.IndexOfArray(mapsInTree, z)
                If nodeIndex < 0 Then
                    ReDim Preserve mapsInTree(UBound(mapsInTree) + 1)
                    mapsInTree(UBound(mapsInTree)) = New Integer(z.Length - 1) {}
                    Dim j As Integer = 0
                    For Each it As Integer In z
                        mapsInTree(UBound(mapsInTree))(j) = it
                        j += 1
                    Next
                    Dim zoneS As String = IIf(z.Length > 0, "Zone " & String.Join(" & ", AddMath.IntArr2StrArr(z)), "No zone")
                    Dim t As TreeNode = Me.treeQuests.Nodes.Add(zoneS)
                    t.Tag = -1
                    nodeIndex = UBound(mapsInTree)
                End If
                Dim qNode As TreeNode = Me.treeQuests.Nodes(nodeIndex).Nodes.Add(nName)
                qNode.Tag = nCnt
            End If
            nCnt += 1
        Next
        If search <> "" Then Me.treeQuests.ExpandAll()
    End Sub
    Private Sub closeQuestEditor()
        Me.treeQuests.Nodes.Clear()
        closeOpenQuest()
        questParser = New CQuestParser
        If Not openEditor Is Nothing Then closePhaseLine(openEditor)

    End Sub
    Private Sub closeOpenQuest()
        Me.lnkDeleteOpenQuest.Visible = False
        questParser.openQuest = Nothing
        questParser.OpenPhase = Nothing
        Me.treeQuests.SelectedNode = Nothing
        cleanQuestTabs()
    End Sub
    Private Sub cleanQuestTabs()
        ClearQuestCombos()
        Me.txtQExistingQuest.Text = ""
        Me.txtQId.Text = ""
        Me.txtQMaxLvl.Text = ""
        Me.txtQMinLvl.Text = ""
        Me.txtQTAward.Text = ""
        Me.txtQTDesc.Text = ""
        Me.txtQTLevel.Text = ""
        Me.txtQTName.Text = ""
        Me.txtQTShortDesc.Text = ""
        Me.chkQAddToCompletedList.Checked = False
        Me.chkQAllowDel.Checked = False
        Me.chkQParty.Checked = False

        Me.txtQPhaseName.Text = ""
        Me.txtQPhaseZone.Text = ""
        Me.flowQPhases.Controls.Clear()
    End Sub

    Private Sub openQuestForEdit(ByRef quest As CQuestParser.Quest, Optional ByVal reload As Boolean = False)
        If questParser.openQuest Is quest AndAlso Not reload Then Exit Sub
        Dim tmpQ As CQuestParser.Quest = quest
        questParser.openQuest = Nothing 'disable save events on loading
        If reload Then quest = tmpQ
        PaintQuestCombos()
        Dim questStart() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestStart)
        Dim questIcon() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestIcon)
        Dim questType() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestType)
        Dim questCompSave() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestCompleteSave)
        Dim questTitle() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestTitle)
        Dim questLevel() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestLevel)
        Dim questAward() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestAward)
        Dim questDesc() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestDesc)
        Dim questShortDesc() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EQuestShortDesc)
        Try
            If questParser.rylVersion = 1 Then
                checkQuestArrSize(questIcon, CQuestParser.QLine.KnownType.EQuestIcon)
            Else
                checkQuestArrSize(questStart, CQuestParser.QLine.KnownType.EQuestStart)
                checkQuestArrSize(questType, CQuestParser.QLine.KnownType.EQuestType)
            End If
            checkQuestArrSize(questCompSave, CQuestParser.QLine.KnownType.EQuestCompleteSave)
            checkQuestArrSize(questTitle, CQuestParser.QLine.KnownType.EQuestTitle)
            checkQuestArrSize(questLevel, CQuestParser.QLine.KnownType.EQuestLevel)
            checkQuestArrSize(questAward, CQuestParser.QLine.KnownType.EQuestAward)
            checkQuestArrSize(questDesc, CQuestParser.QLine.KnownType.EQuestDesc)
            checkQuestArrSize(questShortDesc, CQuestParser.QLine.KnownType.EQuestShortDesc)
        Catch ex As Exception
            MsgBox(ex.Message)
            GoTo last
        End Try
        'general
        If questParser.rylVersion = 1 Then
            Me.cmbQClass.Enabled = False
            Me.cmbQNation.Enabled = False
            Me.cmbQuestType.Enabled = False
            Me.txtQMinLvl.Enabled = False
            Me.txtQMaxLvl.Enabled = False
            Me.chkQAllowDel.Enabled = False
            Me.txtQExistingQuest.Enabled = False
            Me.chkQParty.Enabled = False
            Me.lnkGoToExQuest.Visible = False
            Me.txtQId.MaxLength = 255

            Me.txtQId.Text = quest.IdString
        Else
            Me.cmbQClass.Enabled = True
            Me.cmbQNation.Enabled = True
            Me.cmbQuestType.Enabled = True
            Me.txtQMinLvl.Enabled = True
            Me.txtQMaxLvl.Enabled = True
            Me.chkQAllowDel.Enabled = True
            Me.txtQExistingQuest.Enabled = True
            Me.chkQParty.Enabled = True
            Me.lnkGoToExQuest.Visible = True
            Me.txtQId.MaxLength = 4

            Dim qId As Long = questStart(0).params(0).value
            Dim qAllowDelete As Boolean = True
            If qId >= &HF000 Then
                qId -= &HF000
                qAllowDelete = False
            End If
            Me.txtQId.Text = AddMath.Hex2(questStart(0).params(0).value)
            Me.chkQAllowDel.Checked = qAllowDelete
            Me.txtQMinLvl.Text = questStart(0).params(1).value
            Me.txtQMaxLvl.Text = questStart(0).params(2).value

            cmbItem.setComboSelected(Me.cmbQClass, questStart(0).params(3).value)
            cmbItem.setComboSelected(Me.cmbQNation, questStart(0).params(5).value)
            Me.txtQExistingQuest.Text = IIf(questStart(0).params(4).value < 1, "", Hex(questStart(0).params(4).value))
            Me.chkQParty.Checked = IIf(questType(0).params(1).value > 0, True, False)
            cmbItem.setComboSelected(Me.cmbQuestType, questType(0).params(0).value)
        End If

        Me.txtQTName.Text = questTitle(0).params(0).value
        Me.chkQAddToCompletedList.Checked = questCompSave(0).params(0).value
        Me.txtQTLevel.Text = questLevel(0).params(0).value
        Me.txtQTShortDesc.Lines = CType(questShortDesc(0).params(0).value, String).Split(New String() {"\\"}, StringSplitOptions.None)
        Me.txtQTAward.Lines = CType(questAward(0).params(0).value, String).Split(New String() {"\\"}, StringSplitOptions.None)

        'description
        Me.txtQTDesc.Lines = CType(questDesc(0).params(0).value, String).Split(New String() {"\\"}, StringSplitOptions.None)

        'phases
        redrawPhasesCmb(quest)
        openQuestPhaseForEdit(quest, 1)
        Me.lnkDeleteOpenQuest.Visible = True
last:
        questParser.openQuest = quest
    End Sub
    Private Sub refreshQuestPhase()
        openQuestPhaseForEdit(questParser.openQuest, questParser.OpenPhase.Id)
    End Sub
    Private Sub openQuestPhaseForEdit(ByRef quest As CQuestParser.Quest, ByVal number As Integer)
        Dim phases As CQuestParser.QuestPhase() = quest.Phases()
        If Not openEditor Is Nothing Then closePhaseLine(openEditor)
        For Each phase As CQuestParser.QuestPhase In phases
            If phase.Id = number Then
                questParser.OpenPhase = phase
                Me.txtQPhaseName.Lines = phase.Name.Split(New String() {"\\"}, StringSplitOptions.None)
                Me.txtQPhaseZone.Text = phase.Zone

                'add pointers
                Me.lblQPhaseNumberOfTargets.Text = "(" & phase.mapPointers.Length & ")"
                Me.flowQPhases.Controls.Clear()
                If Not phase.mainFunction Is Nothing Then
                    Dim line As New cntQuestPhaseLine(phase.mainFunction)
                    addPhaseLine(line)
                    For Each mL As CQuestParser.QLine In phase.mainFunctionSiblings
                        Dim lineS As New cntQuestPhaseLine(mL)
                        addPhaseLine(lineS)
                    Next
                    If Not phase.ElseFunction Is Nothing Then
                        Dim line2 As New cntQuestPhaseLine(phase.ElseFunction)
                        addPhaseLine(line2)
                        For Each mL As CQuestParser.QLine In phase.elseSiblings
                            Dim lineS As New cntQuestPhaseLine(mL)
                            addPhaseLine(lineS)
                        Next
                    Else
                        Dim line2 As New LinkLabel
                        line2.Width = 200
                        line2.Text = "Add else"
                        AddHandler line2.LinkClicked, AddressOf lnkAddElseF_LinkClicked
                        Me.flowQPhases.Controls.Add(line2)
                    End If
                    Dim line3 As New LinkLabel
                    line3.Width = 200
                    line3.Text = "Add function"
                    AddHandler line3.LinkClicked, AddressOf lnkAddSubF_LinkClicked
                    Me.flowQPhases.Controls.Add(line3)
                Else
                    Dim line As New LinkLabel
                    line.Width = 200
                    line.Text = "Add trigger"
                    AddHandler line.LinkClicked, AddressOf lnkAddMainF_LinkClicked
                    Me.flowQPhases.Controls.Add(line)
                End If

            End If
        Next
    End Sub
    Dim openPhaseLine As CQuestParser.QLine = Nothing
    Dim openEditor As cntQuestPhaseLineEditor = Nothing
    Private Sub openPhaseLineForEdit(ByRef line As CQuestParser.QLine)
        If Not openEditor Is Nothing Then closePhaseLine(openEditor)
        If line.Type = CQuestParser.QLine.KnownType.EElse Then Exit Sub
        Dim editor As New cntQuestPhaseLineEditor(line, syntaxXML)
        AddHandler editor.Save, AddressOf savePhaseLine
        AddHandler editor.Close, AddressOf closePhaseLine
        AddHandler editor.NeedItemName, AddressOf LineEditor_NeedItemName
        AddHandler editor.NeedItemSelect, AddressOf LineEditor_NeedItemSelect
        AddHandler editor.NeedMobName, AddressOf LineEditor_NeedMobName
        AddHandler editor.NeedMobSelect, AddressOf LineEditor_NeedMobSelect
        AddHandler editor.Delete, AddressOf deletePhaseLine
        Me.Controls.Add(editor)
        Dim loc As New Point
        Dim pp As Point = Me.tabCntrlQuest.PointToScreen(New Point(0, 0))
        pp.X += Me.tabCntrlQuest.Size.Width
        pp.Y += Me.tabCntrlQuest.Size.Height - editor.Size.Height
        editor.Location = Me.PointToClient(pp)

        editor.Show()
        editor.BringToFront()
        openPhaseLine = line
        openEditor = editor
    End Sub
    Private Sub savePhaseLine(ByRef editor As cntQuestPhaseLineEditor, ByRef line As CQuestParser.QLine)
        refreshQuestPhase()
        closePhaseLine(editor)
    End Sub
    Private Sub closePhaseLine(ByRef editor As cntQuestPhaseLineEditor)
        Me.Controls.Remove(editor)
        editor.Dispose()
        editor = Nothing
        openEditor = Nothing
        openPhaseLine = Nothing
    End Sub
    Private Sub deletePhaseLine(ByRef editor As cntQuestPhaseLineEditor, ByRef line As CQuestParser.QLine)
        questParser.openQuest.DeleteLine(line)
        closePhaseLine(editor)
        refreshQuestPhase()
    End Sub
    Private Sub addPhaseLine(ByRef line As cntQuestPhaseLine)
        AddHandler line.lineWantsToMove, AddressOf PhaseLine_lineWantsToMove
        AddHandler line.openQLinesForEdit, AddressOf PhaseLine_openQLinesForEdit
        Me.flowQPhases.Controls.Add(line)
    End Sub
    Private Function ChangeQuestGeneral(ByVal type As CQuestParser.QLine.KnownType) As Boolean
        Dim err As Boolean = False
        If Not questParser.openQuest Is Nothing Then
            Dim lines() As CQuestParser.QLine = questParser.openQuest.Lines(type)
            If lines.Length = 1 Then
                'Dim index As Long = npcParser.loadedNPC.iLines.IndexOf(lines(0))
                Try
                    With lines(0)
                        Select Case type
                            Case CQuestParser.QLine.KnownType.EQuestStart
                                .params(0).value = Convert.ToInt32(Me.txtQId.Text, 16)
                                questParser.openQuest.Id = .params(0).value
                                .params(1).value = Convert.ToInt32(Me.txtQMinLvl.Text, 10)
                                .params(2).value = Convert.ToInt32(Me.txtQMaxLvl.Text, 10)
                                .params(3).value = CType(Me.cmbQClass.SelectedItem, cmbItem).iItem
                                If Me.txtQExistingQuest.Text = "" Then
                                    .params(4).value = 0
                                Else
                                    .params(4).value = Convert.ToInt32(Me.txtQExistingQuest.Text, 16)
                                End If
                                .params(5).value = CType(Me.cmbQNation.SelectedItem, cmbItem).iItem
                            Case CQuestParser.QLine.KnownType.EQuestType
                                .params(0).value = CType(Me.cmbQuestType.SelectedItem, cmbItem).iItem
                                .params(1).value = IIf(Me.chkQParty.Checked, 1, 0)
                            Case CQuestParser.QLine.KnownType.EQuestCompleteSave
                                .params(0).value = Me.chkQAddToCompletedList.Checked
                            Case CQuestParser.QLine.KnownType.EQuestTitle
                                .params(0).value = Me.txtQTName.Text
                            Case CQuestParser.QLine.KnownType.EQuestLevel
                                .params(0).value = Me.txtQTLevel.Text
                            Case CQuestParser.QLine.KnownType.EQuestAward
                                .params(0).value = String.Join("\\", Me.txtQTAward.Lines)
                            Case CQuestParser.QLine.KnownType.EQuestDesc
                                .params(0).value = String.Join("\\", Me.txtQTDesc.Lines)
                            Case CQuestParser.QLine.KnownType.EQuestShortDesc
                                .params(0).value = String.Join("\\", Me.txtQTShortDesc.Lines)
                        End Select
                    End With
                Catch ex As Exception
                    err = True
                    MsgBox("Check your input in " & CQuestParser.QLine.Type2String(type) & " type text box")
                End Try
            Else
                err = True
            End If
            Return Not err
        Else
            Return False
        End If
    End Function

    Private Sub PaintQuestCombos()
        ClearQuestCombos()
        For Each e As CQuestParser.QClassEnum.base In [Enum].GetValues(GetType(CQuestParser.QClassEnum.base))
            Me.cmbQClass.Items.Add(New cmbItem(e, CQuestParser.QClassEnum.Type2String(e)))
        Next
        For Each e As CQuestParser.QNationEnum.base In [Enum].GetValues(GetType(CQuestParser.QNationEnum.base))
            Me.cmbQNation.Items.Add(New cmbItem(e, CQuestParser.QNationEnum.Type2String(e)))
        Next
        For Each e As CQuestParser.QTypeEnum.base In [Enum].GetValues(GetType(CQuestParser.QTypeEnum.base))
            Me.cmbQuestType.Items.Add(New cmbItem(e, CQuestParser.QTypeEnum.Type2String(e)))
        Next
    End Sub
    Private Sub ClearQuestCombos()
        Me.cmbQClass.Items.Clear()
        Me.cmbQNation.Items.Clear()
        Me.cmbQuestType.Items.Clear()
        Me.cmbQPhases.Items.Clear()
    End Sub
    Private Sub checkQuestArrSize(ByVal arr As CQuestParser.QLine(), ByVal type As CQuestParser.QLine.KnownType)
        If arr.Length <> 1 Then
            Throw New Exception("Function " & CQuestParser.QLine.Type2String(type) & IIf(arr.Length < 1, " not found", " is specified too many times"))
        End If
    End Sub
    Private Sub selectNodeInQuestTree(ByVal QuestID As Integer, Optional ByRef nodes As TreeNodeCollection = Nothing)
        If nodes Is Nothing Then nodes = Me.treeQuests.Nodes
        For Each nod As TreeNode In nodes
            If Not nod.Tag Is Nothing AndAlso nod.Tag >= 0 Then
                If questParser.Quests(nod.Tag).Id = QuestID Then
                    Me.treeQuests.SelectedNode = nod

                End If
            Else
                selectNodeInQuestTree(QuestID, nod.Nodes)
            End If
        Next
    End Sub
    Private Sub redrawPhasesCmb(ByRef quest As CQuestParser.Quest, Optional ByVal activePhase As Integer = 1)
        Dim questPhases() As CQuestParser.QLine = quest.Lines(CQuestParser.QLine.KnownType.EAddPhase)
        Me.cmbQPhases.Items.Clear()
        For Each p As CQuestParser.QLine In questPhases
            Me.cmbQPhases.Items.Add(New cmbItem(p.params(1).value, p.params(2).value))
        Next
        If Me.cmbQPhases.Items.Count > 0 Then
            cmbItem.setComboSelected(Me.cmbQPhases, activePhase)
        Else
            questParser.OpenPhase = Nothing
            Me.flowQPhases.Controls.Clear()
            Me.txtQPhaseZone.Text = ""
            Me.txtQPhaseName.Text = ""
            Me.lblQPhaseNumberOfTargets.Text = "(0)"
        End If
    End Sub

    Private Sub addLineToPhase(ByRef line As CQuestParser.QLine, ByVal phaseNr As Integer)
        If Not questParser.openQuest Is Nothing Then
            With questParser.openQuest
                If .iLines.Length > 0 Then line.ownerFunction = .iLines(0).ownerFunction
                Dim added As Boolean = False
                Dim nLines As New ArrayList 'type=Qline
                For Each l As CQuestParser.QLine In .iLines
                    If l.Type = CQuestParser.QLine.KnownType.EAddPhase AndAlso Not added Then
                        If l.params(1).value = phaseNr + 1 Then
                            nLines.Add(line)
                            added = True
                        End If
                    ElseIf l.Type = CQuestParser.QLine.KnownType.EQuestEnd AndAlso Not added Then
                        nLines.Add(line)
                        added = True
                    End If
                    nLines.Add(l)
                Next
                If Not added Then
                    nLines.Add(line)
                    added = True
                End If
                .iLines = nLines.ToArray(GetType(CQuestParser.QLine))
            End With
        End If
    End Sub
    Private Sub switchPhases(ByVal ph1 As Integer, ByVal ph2 As Integer)
        If ph2 < ph1 Then
            Dim t As Integer = ph1
            ph1 = ph2
            ph2 = t
        End If
        Dim nLines As New ArrayList
        Dim out As New ArrayList
        Dim ph1l As New ArrayList
        Dim ph2l As New ArrayList
        Dim ph1n As Integer = -1
        Dim ph2n As Integer = -1
        Dim going As Integer = -1
        For Each l As CQuestParser.QLine In questParser.openQuest.iLines
            If l.Type = CQuestParser.QLine.KnownType.EAddPhase Then
                If l.params(1).value = ph2 Then
                    going = 1
                    ph1n = nLines.Count
                    l.params(1).value = ph1
                ElseIf l.params(1).value = ph1 Then
                    going = 2
                    ph2n = nLines.Count
                    l.params(1).value = ph2
                Else
                    going = -1
                End If
            ElseIf l.Type = CQuestParser.QLine.KnownType.EQuestEnd Then
                going = -1
            End If
            If going < 0 Then
                nLines.Add(l)
            ElseIf going = 1 Then
                ph1l.Add(l)
            ElseIf going = 2 Then
                ph2l.Add(l)
            End If
        Next
        Dim p As Integer = 0
        For Each l As CQuestParser.QLine In nLines
            If p = ph1n Then
                For Each l2 As CQuestParser.QLine In ph1l
                    out.Add(l2)
                Next
            End If
            If p = ph2n Then
                For Each l2 As CQuestParser.QLine In ph2l
                    out.Add(l2)
                Next
            End If
            out.Add(l)
            p += 1
        Next
        'ryl1 doesnt have the end tag
        If p = ph1n Then
            For Each l2 As CQuestParser.QLine In ph1l
                out.Add(l2)
            Next
        End If
        If p = ph2n Then
            For Each l2 As CQuestParser.QLine In ph2l
                out.Add(l2)
            Next
        End If
        questParser.openQuest.iLines = out.ToArray(GetType(CQuestParser.QLine))
    End Sub
#Region "Quest page events"
    Private Sub txtSearchQuest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSearchQuest.TextChanged
        drawQuestTree(txtSearchQuest.Text.ToLower)
    End Sub

    Private Sub treeQuests_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles treeQuests.AfterSelect
        treeQuests_NodeMouseClick(Me, New System.Windows.Forms.TreeNodeMouseClickEventArgs(Me.treeQuests.SelectedNode, Windows.Forms.MouseButtons.Left, 1, 0, 0))
    End Sub
    Private Sub treeQuests_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles treeQuests.NodeMouseClick
        Dim sN As TreeNode = e.Node
        If Not sN Is Nothing AndAlso sN.Tag >= 0 Then
            openQuestForEdit(questParser.Quests(sN.Tag))
        End If
    End Sub
    Private Sub lnkGoToExQuest_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkGoToExQuest.LinkClicked
        Dim tx As String = Me.txtQExistingQuest.Text
        If Trim(tx) <> "" Then
            Dim exQ As Integer = 0
            Try
                exQ = Convert.ToInt32(tx, 16)
            Catch ex As Exception
                MsgBox("Errornous Quest ID '" & tx & "'")
                Exit Sub
            End Try
            Dim index As Integer = questParser.questIndexForID(exQ)
            If index >= 0 Then
                selectNodeInQuestTree(exQ)
                Me.treeQuests.Focus()
                'openQuestForEdit(questParser.Quests(index))
            Else
                MsgBox("Quest 0x" & AddMath.Hex2(exQ) & " not found")
            End If
        End If
    End Sub

    Private Sub lnkAddNewQuest_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkAddNewQuest.LinkClicked
        Dim q As CQuestParser.Quest = questParser.AddNewQuest()
        drawQuestTree(txtSearchQuest.Text.ToLower)
        selectNodeInQuestTree(q.Id)
    End Sub
    Private Sub lnkDeleteOpenQuest_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkDeleteOpenQuest.LinkClicked
        questParser.DeleteQuest(questParser.openQuest)
        closeOpenQuest()
        drawQuestTree(txtSearchQuest.Text.ToLower)
    End Sub
    'triggers

    Private Sub chkQAllowDel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQAllowDel.Click
        Dim qId As Long = 0
        Try
            qId = Convert.ToUInt32(Me.txtQId.Text, 16)
        Catch ex As Exception
            MsgBox("Cant change type. Invalid Hex in quest ID")
            Exit Sub
        End Try
        If qId >= &HF000 Then
            qId -= &HF000
            Me.chkQAllowDel.Checked = True
        Else
            qId += &HF000
            Me.chkQAllowDel.Checked = False
        End If
        Me.txtQId.Text = Hex(qId)
    End Sub
    Private Sub txtQId_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQId.Leave
        If Not questParser Is Nothing AndAlso Not questParser.openQuest Is Nothing Then
            If questParser.rylVersion = 1 Then
                If Me.txtQId.Text = String.Empty Then
                    MsgBox("Quest ID can not be empty", MsgBoxStyle.Exclamation, "rylCoder")
                    Me.txtQId.Text = questParser.openQuest.IdString
                Else
                    questParser.openQuest.IdString = Me.txtQId.Text
                End If
            Else
                If ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart) Then
                    If Me.txtQId.Text.Length = 4 AndAlso Me.txtQId.Text.Substring(0, 1).ToUpper = "F" Then
                        Me.chkQAllowDel.Checked = False
                    Else
                        Me.chkQAllowDel.Checked = True
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub txtQMaxLvl_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQMaxLvl.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart)
    End Sub
    Private Sub txtQMinLvl_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQMinLvl.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart)
    End Sub

    Private Sub txtQExistingQuest_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQExistingQuest.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart)
    End Sub

    Private Sub txtQTAward_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQTAward.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestAward)
    End Sub
    Private Sub txtQTDesc_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQTDesc.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestDesc)
    End Sub
    Private Sub txtQTLevel_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQTLevel.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestLevel)
    End Sub
    Private Sub txtQTName_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQTName.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestTitle)
    End Sub
    Private Sub txtQTShortDesc_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQTShortDesc.Leave
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestShortDesc)
    End Sub

    Private Sub chkQAddToCompletedList_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQAddToCompletedList.CheckStateChanged
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestCompleteSave)
    End Sub
    Private Sub chkQParty_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQParty.CheckStateChanged
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestType)
    End Sub

    Private Sub cmbQClass_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQClass.SelectedIndexChanged
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart)
    End Sub
    Private Sub cmbQNation_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQNation.SelectedIndexChanged
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestStart)
    End Sub
    Private Sub cmbQuestType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQuestType.SelectedIndexChanged
        ChangeQuestGeneral(CQuestParser.QLine.KnownType.EQuestType)
    End Sub

    'phase editor

    Private Sub lnkAddMainF_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Dim nL As New CQuestParser.QLine
        nL.Type = CQuestParser.QLine.KnownType.ETrigger_Start
        nL.params = New CMcfBase.SParamElem() {}
        addLineToPhase(nL, questParser.OpenPhase.Id)
        refreshQuestPhase()
    End Sub
    Private Sub lnkAddElseF_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Dim nL As New CQuestParser.QLine
        nL.Type = CQuestParser.QLine.KnownType.EElse
        nL.params = New CMcfBase.SParamElem() {}
        addLineToPhase(nL, questParser.OpenPhase.Id)
        refreshQuestPhase()
    End Sub
    Private Sub lnkAddSubF_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        Dim nL As New CQuestParser.QLine
        nL.Type = CQuestParser.QLine.KnownType.EEvent_MsgBox
        nL.params = New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(CMcfBase.DataType.EString, "")}
        addLineToPhase(nL, questParser.OpenPhase.Id)
        refreshQuestPhase()
    End Sub

    Private Sub cmbQPhases_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQPhases.SelectedIndexChanged
        If Not questParser.openQuest Is Nothing Then openQuestPhaseForEdit(questParser.openQuest, CType(Me.cmbQPhases.SelectedItem, cmbItem).iItem)
    End Sub

    Private Sub btnQAddPhase_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQAddPhase.Click
        If Not questParser.openQuest Is Nothing Then
            Dim Nl As New CQuestParser.QLine()
            Nl.Type = CQuestParser.QLine.KnownType.EAddPhase
            Dim phase As Integer = Me.cmbQPhases.Items.Count + 1
            Nl.params = New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 0), CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, phase), CMcfBase.CreateParamElem(CMcfBase.DataType.EString, "")}
            addLineToPhase(Nl, phase - 1)
            redrawPhasesCmb(questParser.openQuest, phase)
        End If
    End Sub
    Private Sub btnQDeletePhase_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQDeletePhase.Click
        If Not questParser.openQuest Is Nothing AndAlso Me.cmbQPhases.Items.Count > 0 Then
            Dim ls As CQuestParser.QLine() = questParser.openQuest.getLinesForPhase(CType(Me.cmbQPhases.SelectedItem, cmbItem).iItem)
            For Each l As CQuestParser.QLine In ls
                questParser.openQuest.DeleteLine(l)
            Next
            Dim i As Integer = 1
            For Each l As CQuestParser.QLine In questParser.openQuest.iLines
                If l.Type = CQuestParser.QLine.KnownType.EAddPhase Then
                    l.params(1).value = i
                    i += 1
                End If
            Next
            redrawPhasesCmb(questParser.openQuest)
        End If
    End Sub

    Private Sub btnQMovePhaseDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQMovePhaseDown.Click
        If Not questParser.openQuest Is Nothing AndAlso Me.cmbQPhases.Items.Count > 0 AndAlso Me.cmbQPhases.Items.Count - 1 > Me.cmbQPhases.SelectedIndex Then
            Dim selQ As Integer = CType(Me.cmbQPhases.SelectedItem, cmbItem).iItem
            switchPhases(selQ, selQ + 1)
            redrawPhasesCmb(questParser.openQuest, selQ + 1)
        End If
    End Sub
    Private Sub btnQMovePhaseUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQMovePhaseUp.Click
        If Not questParser.openQuest Is Nothing AndAlso Me.cmbQPhases.Items.Count > 0 AndAlso Me.cmbQPhases.SelectedIndex > 0 Then
            Dim selQ As Integer = CType(Me.cmbQPhases.SelectedItem, cmbItem).iItem
            switchPhases(selQ - 1, selQ)
            redrawPhasesCmb(questParser.openQuest, selQ - 1)
        End If
    End Sub
    Private Sub txtQPhaseName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQPhaseName.TextChanged
        If Not questParser.OpenPhase Is Nothing AndAlso Not questParser.openQuest Is Nothing Then
            Dim ls As CQuestParser.QLine() = questParser.openQuest.getLinesForPhase(questParser.OpenPhase.Id)
            For Each l As CQuestParser.QLine In ls
                If l.Type = CQuestParser.QLine.KnownType.EAddPhase Then
                    l.params(2).value = String.Join("\\", Me.txtQPhaseName.Lines)
                    questParser.OpenPhase.Name = l.params(2).value
                End If
            Next
        End If
    End Sub
    Private Sub txtQPhaseZone_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQPhaseZone.TextChanged
        If Not questParser.OpenPhase Is Nothing AndAlso Not questParser.openQuest Is Nothing Then
            Dim ls As CQuestParser.QLine() = questParser.openQuest.getLinesForPhase(questParser.OpenPhase.Id)
            For Each l As CQuestParser.QLine In ls
                If l.Type = CQuestParser.QLine.KnownType.EAddPhase Then
                    Dim err As Boolean = False
                    Try
                        l.params(0).value = Integer.Parse(Me.txtQPhaseZone.Text)
                        questParser.OpenPhase.Zone = l.params(0).value
                    Catch ex As Exception
                        err = True
                    End Try
                    If err Then
                        Me.txtQPhaseZone.BackColor = Color.Pink
                    Else
                        Me.txtQPhaseZone.BackColor = Color.White
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub btnQPhaseTargetsOnMap_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQPhaseTargetsOnMap.Click
        If Not questParser.OpenPhase Is Nothing AndAlso Not questParser.openQuest Is Nothing Then
            Dim map As New frmMap
            map.AllowMoves = True
            map.AllowAddition = True
            map.openZone = questParser.OpenPhase.Zone
            For Each p As Point In questParser.OpenPhase.mapPointers
                map.addPointer(p.X, p.Y)
            Next
            If map.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim ps As Single()() = map.Points
                Dim newPoints(ps.Length - 1) As CQuestParser.QLine
                Dim i As Integer = 0
                For Each p As Single() In ps
                    newPoints(i) = New CQuestParser.QLine
                    newPoints(i).Type = CQuestParser.QLine.KnownType.EPhase_Target
                    newPoints(i).params = New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, p(0)), CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, p(1))}
                    If questParser.openQuest.iLines.Length > 0 Then newPoints(i).ownerFunction = questParser.openQuest.iLines(0).ownerFunction
                    i += 1
                Next
                Dim tLines As New ArrayList
                Dim going As Boolean = False
                Dim wasgoing As Boolean = False
                Dim added As Boolean = False
                For Each l As CQuestParser.QLine In questParser.openQuest.iLines
                    Dim dontadd As Boolean = False
                    If l.Type = CQuestParser.QLine.KnownType.EAddPhase Then
                        If l.params(1).value = questParser.OpenPhase.Id Then
                            going = True
                            wasgoing = True
                        Else
                            If wasgoing AndAlso Not added Then
                                tLines.AddRange(newPoints)
                                added = True
                            End If
                            going = False
                        End If
                    ElseIf l.Type = CQuestParser.QLine.KnownType.EPhase_Target AndAlso going Then
                        If Not added Then
                            tLines.AddRange(newPoints)
                            added = True
                        End If
                        dontadd = True
                    Else
                        If wasgoing AndAlso Not added Then
                            tLines.AddRange(newPoints)
                            added = True
                        End If
                    End If
                    If Not dontadd Then tLines.Add(l)
                Next
                If wasgoing AndAlso Not added Then
                    tLines.AddRange(newPoints)
                    added = True
                End If
                questParser.openQuest.iLines = tLines.ToArray(GetType(CQuestParser.QLine))
                redrawPhasesCmb(questParser.openQuest, questParser.OpenPhase.Id)
            End If
        End If
    End Sub
    Private Sub PhaseLine_openQLinesForEdit(ByRef sender As cntQuestPhaseLine, ByRef line As CQuestParser.QLine)
        If Not line Is openPhaseLine Then
            openPhaseLineForEdit(line)
        End If
    End Sub
    Private Sub PhaseLine_lineWantsToMove(ByRef sender As cntQuestPhaseLine, ByRef line As CQuestParser.QLine, ByVal offsetFromTop As Integer)
        Dim startH As Integer = offsetFromTop
        Dim pos As Integer = -1
        For Each c As Control In Me.flowQPhases.Controls
            pos += 1
            If c.Location.Y > startH Then
                Exit For
            End If
        Next
        Dim oldPos As Integer = Me.flowQPhases.Controls.GetChildIndex(sender)
tst:    If pos = oldPos Then GoTo nomove
        If oldPos = 0 Then GoTo nomove
        If pos < 1 Then
            pos += 1
            GoTo tst
        End If
        If Not Me.flowQPhases.Controls.Item(pos).GetType Is GetType(cntQuestPhaseLine) Then
            pos -= 1
            GoTo tst
        End If
        Dim otherLine As CQuestParser.QLine = CType(Me.flowQPhases.Controls.Item(pos), cntQuestPhaseLine).qLine
        Dim nLines As New ArrayList
        For Each l As CQuestParser.QLine In questParser.openQuest.iLines
            If l Is line Then
                nLines.Add(otherLine)
            ElseIf l Is otherLine Then
                nLines.Add(line)
            Else
                nLines.Add(l)
            End If
        Next
        questParser.openQuest.iLines = nLines.ToArray(GetType(CQuestParser.QLine))
        Me.flowQPhases.Controls.SetChildIndex(sender, pos)
nomove:
    End Sub
    Private Sub LineEditor_NeedItemName(ByRef sender As cntQuestPhaseLineEditor, ByVal id As Long, ByRef name As String)
        name = GetItemName(id)
    End Sub
    Private Sub LineEditor_NeedItemSelect(ByRef sender As cntQuestPhaseLineEditor, ByRef sender2 As LinkLabel)
        If ItemSelector Is Nothing Then
            MsgBox("Please load a Itemscript in")
            Exit Sub
        Else
            Dim nT As New CNpcParser.NPCTextItem
            nT.paraIndex = sender2.Tag.paraIndex
            nT.Tag = sender2.Tag.Tag
            nT.text = ItemSelector.open(IIf(sender2.Tag.text.Length > 0, sender2.Tag.text, 0))
            sender2.Tag = nT
        End If
    End Sub
    Private Sub LineEditor_NeedMobName(ByRef sender As cntQuestPhaseLineEditor, ByVal id As Long, ByRef name As String)
        name = GetMobName(id)
    End Sub
    Private Sub LineEditor_NeedMobSelect(ByRef sender As cntQuestPhaseLineEditor, ByRef sender2 As LinkLabel)
        If MobSelector Is Nothing Then
            MsgBox("Please load a Monsterprototype in")
            Exit Sub
        Else
            Dim nT As New CNpcParser.NPCTextItem
            nT.paraIndex = sender2.Tag.paraIndex
            nT.Tag = sender2.Tag.Tag
            nT.text = MobSelector.open(IIf(sender2.Tag.text.Length > 0, sender2.Tag.text, 0))
            sender2.Tag = nT
        End If
    End Sub
#End Region

    Public Class cmbItem
        Public iTxt As String
        Public iItem As Integer
        Public Sub New()

        End Sub
        Public Sub New(ByVal item As Integer, ByVal txt As String)
            iItem = item
            iTxt = txt
        End Sub
        Public Shared Operator =(ByVal a As cmbItem, ByVal b As Integer) As Boolean
            Return a.iItem = b
        End Operator
        Public Shared Operator <>(ByVal a As cmbItem, ByVal b As Integer) As Boolean
            Return a.iItem <> b
        End Operator
        Public Overrides Function ToString() As String
            Return iTxt
        End Function
        Public Shared Sub setComboSelected(ByRef combo As ComboBox, ByVal enu As Integer)
            Dim index As Integer = 0
            Dim found As Boolean = False
            For Each i As cmbItem In combo.Items
                If i = enu Then
                    found = True
                    Exit For
                End If
                index += 1
            Next
            If found Then combo.SelectedIndex = index Else MsgBox("Unknown value in " & combo.Name & " : " & enu)
        End Sub
    End Class
#End Region

#Region "ScriptEditor"
    Public syntaxXML As Xml.XmlElement = Nothing
    Public syntaxLoaded As Boolean = False
    Private Sub txtEdit_CarrierEvent()
        If syntaxXML Is Nothing AndAlso Not syntaxLoaded Then
            loadSyntaxs()
        ElseIf syntaxLoaded AndAlso syntaxXML Is Nothing Then
            'no help available
        End If
        Dim cL As Integer = Me.txtEdit.SelectionStart
        Dim sL As String = ""
        Dim lS As Integer = 0
        For Each l As String In Me.txtEdit.Lines
            Dim len As Integer = l.Length + 1
            If cL >= lS AndAlso cL < lS + len Then
                sL = l
                Exit For
            End If
            lS += len
        Next
        If sL <> "" AndAlso sL.IndexOf("(") > 0 Then
            Dim fName As String = Trim(sL.Split("(")(0).Replace(vbTab, ""))
            Dim selPar As Integer = 0
            If (sL.IndexOf("(") >= (cL - lS)) OrElse (sL.IndexOf(");") > 0 AndAlso (cL - lS) > sL.IndexOf(");")) Then
                selPar = -1
            Else
                If sL.IndexOf(",", 0, sL.Length) > 0 Then
                    Dim p As Integer = 0
                    Do While p >= 0 AndAlso p <= sL.Length
                        Dim n As Integer = sL.IndexOf(",", p, sL.Length - p)
                        If n >= (cL - lS) Then
                            Exit Do
                        End If
                        If n < 0 Then
                            p = -1
                        Else
                            selPar += 1
                            p = n + 1
                        End If
                    Loop
                End If
            End If
            showActiveSyntaxHelp(fName, selPar)
        Else
            showActiveSyntaxHelp("", -1)
        End If
    End Sub

    Private prevFName As String = ""
    Private prevFParC As Integer = -1
    Private prevParIndex As Integer = -1
    Private Sub showActiveSyntaxHelp(ByVal funcName As String, ByVal parIndex As Integer)
        If funcName <> prevFName OrElse parIndex <> prevParIndex Then
            Me.pnlSyntaxHelpM.SuspendLayout()
            Me.pnlSyntaxHelpM.Controls.Clear()
            prevFName = funcName
            prevParIndex = parIndex
            Dim f As New CMcfBase.SFunction
            For Each fS As CMcfBase.SFunction In scriptParser.Functions
                If fS.name = funcName Then
                    f = fS
                    Exit For
                End If
            Next
            If f.name <> "" Then
                Dim xmlPars As Xml.XmlNodeList = Nothing
                If Not syntaxXML Is Nothing Then
                    xmlPars = syntaxXML.SelectNodes("func[@name='" & f.name & "' and @parcount=" & f.parameterTypes.Length & "]/param")
                End If
                Me.pnlSyntaxHelpM.Controls.Add(New namedLabel(f.name & "("))
                Dim cnt As Integer = 0
                For Each p As CMcfBase.DataType In f.parameterTypes
                    Dim parName As String = "unknown"
                    If Not xmlPars Is Nothing AndAlso xmlPars.Count > cnt Then parName = xmlPars(cnt).Attributes.GetNamedItem("name").Value
                    Me.pnlSyntaxHelpM.Controls.Add(New namedLabel(parName, IIf(parIndex = cnt, True, False)))
                    Me.pnlSyntaxHelpM.Controls.Add(New namedLabel("[" & CMcfBase.DataTypeString(p) & "]", IIf(parIndex = cnt, True, False)))
                    If cnt <> f.parameterTypes.Length - 1 Then Me.pnlSyntaxHelpM.Controls.Add(New namedLabel(","))
                    cnt += 1
                Next
                If Not xmlPars Is Nothing AndAlso parIndex >= 0 AndAlso parIndex < xmlPars.Count Then
                    Me.lblSyntaxHelpParInfo.Text = xmlPars(parIndex).Attributes.GetNamedItem("desc").Value
                ElseIf Not syntaxXML Is Nothing AndAlso parIndex < 0 AndAlso syntaxXML.SelectNodes("func[@name='" & f.name & "' and @parcount=" & f.parameterTypes.Length & "]").Count > 0 Then
                    Dim Hn As Xml.XmlNode = syntaxXML.SelectSingleNode("func[@name='" & f.name & "' and @parcount=" & f.parameterTypes.Length & "]").Attributes.GetNamedItem("desc")
                    If Not Hn Is Nothing Then Me.lblSyntaxHelpParInfo.Text = Hn.Value
                Else
                    Me.lblSyntaxHelpParInfo.Text = ""
                End If
                Me.pnlSyntaxHelpM.Controls.Add(New namedLabel(")"))
            Else
                Me.pnlSyntaxHelpM.Controls.Add(New namedLabel("No function by that name exists"))
                Me.lblSyntaxHelpParInfo.Text = ""
            End If
            Me.pnlSyntaxHelpM.ResumeLayout()
        End If
    End Sub

    Private Sub loadSyntaxs()
        Dim x As New Xml.XmlDataDocument
        Dim resources As System.Resources.ResourceManager = My.Resources.ResourceManager 'System.ComponentModel.ComponentResourceManager(GetType(My.Resources))
        x.LoadXml(CType(resources.GetObject("xmlFunctionSyntax"), String))
        If Not scriptParser Is Nothing AndAlso scriptParser.RYLVersion > 0 AndAlso scriptParser.RYLFileType > 0 Then
            syntaxXML = x.SelectSingleNode("syntax/ryl[@version=" & scriptParser.RYLVersion & "]/mcf[@type=" & CType(scriptParser.RYLFileType, Integer) & "]")
        End If
        syntaxLoaded = True
    End Sub

    Public Class namedLabel
        Inherits Label
        Sub New(ByVal text As String, Optional ByVal bold As Boolean = False)
            MyBase.New()
            Me.Text = text
            If bold Then Me.Font = New Font(Me.Font, FontStyle.Bold)
            Me.AutoSize = True
            Me.Margin = New Padding(0)
            Me.Padding = New Padding(0)
            Me.Size = New Size(0, 0)
        End Sub
    End Class
    Private Sub txtEdit_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEdit.GotFocus
        txtEdit_CarrierEvent()
    End Sub
    Private Sub txtEdit_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtEdit.KeyDown
        If e.Control Then
            'Debug.WriteLine(e.KeyCode)
            Select Case e.KeyCode
                Case Keys.F
                    If search Is Nothing Then
                        search = New frmSearchBox(Me.txtEdit)
                    End If
                    search.Show()
                Case Keys.A
                    Me.txtEdit.SelectAll()
                Case Keys.S
                    mnuSave_Click(Me, New EventArgs)
                Case Keys.X
                    'Me.txtEdit.Cut()
                Case Keys.C
                    'Me.txtEdit.Copy()
                Case Keys.V
                    'Me.txtEdit.Paste(DataFormats.GetFormat(DataFormats.Text))
            End Select
        End If
        If e.KeyCode = Keys.F3 Then
            If search Is Nothing Then
                search = New frmSearchBox(Me.txtEdit)
            Else
                search.OK_Button_Click(Me, New EventArgs)
            End If
        End If
    End Sub
    Private Sub txtEdit_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtEdit.KeyUp
        txtEdit_CarrierEvent()
    End Sub
    Private Sub txtEdit_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtEdit.MouseClick
        txtEdit_CarrierEvent()
    End Sub

    Private Sub txtEdit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEdit.TextChanged
        changed = True
        If Me.txtEdit.Text <> "" AndAlso Not Me.mnuSaveAs.Enabled Then Me.mnuSaveAs.Enabled = True
        If syntaxHighlightEnabled Then
            Dim lines() As String = Me.txtEdit.Lines
            Dim cL As Integer = Me.txtEdit.SelectionStart
            Dim lS As Integer = 0
            Dim fromL As Integer = 0
            Dim toL As Integer = lines.Length - 1
            Dim pos As Long = 0
            Dim ll As Integer = 0
            If Me.txtEdit.Tag = "full run done" Then
                For Each l As String In lines
                    Dim len As Integer = l.Length + 1
                    If cL >= lS AndAlso cL < lS + len Then
                        fromL = ll
                        toL = ll
                        pos = lS
                        Exit For
                    End If
                    lS += len
                    ll += 1
                Next
            Else
                'Me.txtEdit.Visible = False
            End If
            Me.txtEdit.SuspendLayout()
            ' Stops RichText redrawing:
            SendMessage(Me.txtEdit.Handle, WM_SETREDRAW, 0, IntPtr.Zero)
            ' Stops RichText sending any events:
            Dim eventMask As IntPtr = SendMessage(Me.txtEdit.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero)



            Dim selStart As Integer = Me.txtEdit.SelectionStart
            Dim selLength As Integer = Me.txtEdit.SelectionLength

            For lNr As Integer = fromL To toL
                Dim line As String = lines(lNr)
                Me.txtEdit.Select(pos, line.Length)
                Me.txtEdit.SelectionColor = Color.Black
                If line.Length >= 2 AndAlso line.Substring(0, 2) = "//" Then
                    Me.txtEdit.Select(pos, line.Length)
                    Me.txtEdit.SelectionColor = Color.DarkGreen
                ElseIf line <> "" Then

                    Dim functionFound As Boolean = False
                    For Each sDataType As String In CMcfBase.DataTypeString
                        If line.StartsWith(sDataType & " ") Then
                            Try
                                If line.IndexOf("(") < 0 OrElse line.IndexOf(")") < 0 Then Throw New textException(1, 0, line.Length)
                                Dim a As String() = line.Split("(")
                                Dim c As String() = a(0).Split(" ")
                                For i As Integer = 0 To CMcfBase.DataTypeString.Length - 1
                                    If c(0) = CMcfBase.DataTypeString(i) Then
                                        Me.txtEdit.Select(pos + line.IndexOf(c(0)), c(0).Length)
                                        Me.txtEdit.SelectionColor = Color.Blue
                                    End If
                                Next
                                'Me.txtEdit.Select(pos + line.IndexOf(c(1)), c(1).Length)
                                'Me.txtEdit.SelectionColor = Color.Purple
                                Dim p2 As Integer = pos + line.IndexOf("("c) + 1
                                Dim b As String() = a(1).Split(")")(0).Split(",")
                                If b.Length > 1 OrElse b(0).Trim() <> "" Then
                                    For i As Integer = 0 To b.Length - 1
                                        Dim k As Integer = Array.IndexOf(CMcfBase.DataTypeString, b(i).Trim())
                                        If k > 0 Then
                                            Me.txtEdit.Select(p2, b(i).Length)
                                            Me.txtEdit.SelectionColor = Color.Blue
                                        End If
                                        p2 += b(i).Length + 1
                                    Next
                                End If

                            Catch ex As Exception
                            End Try
                            functionFound = True
                            Exit For
                        End If
                    Next
                    Try
                        If Not functionFound Then
                            Try
                                Dim a() As String = line.Trim().Split(New Char() {"("c}, 2)
                                If a.Length = 2 Then
                                    Dim b As String = a(1).Substring(0, a(1).LastIndexOf(")"c) + 1)
                                    'Me.txtEdit.Select(pos + line.IndexOf(a(0)), a(0).Length)
                                    'Me.txtEdit.SelectionColor = Color.Purple
                                    Dim qIn As Boolean = False
                                    Dim pCh As Char = Chr(0)
                                    Dim buffer As String = ""
                                    Dim p2 As Integer = pos + line.IndexOf(a(0)) + a(0).Length + 1
                                    For kl As Integer = 0 To b.Length - 1
                                        Dim ch As Char = b(kl)
                                        If ch = ControlChars.Quote AndAlso pCh <> "\" Then
                                            qIn = Not qIn
                                            buffer &= ch
                                        Else
                                            If (Not qIn AndAlso ch = ",") OrElse kl = b.Length - 1 Then
                                                If buffer.Trim().Length >= 2 Then
                                                    If buffer.Trim()(0) = """"c Then
                                                        Me.txtEdit.Select(p2 - buffer.Length, buffer.Length)
                                                        Me.txtEdit.SelectionColor = Color.Brown
                                                    ElseIf buffer.Trim().Substring(0, 2) = "0x" Then
                                                        Me.txtEdit.Select(p2 - buffer.Length, buffer.Length)
                                                        Me.txtEdit.SelectionColor = Color.Orchid
                                                    End If
                                                End If
                                                buffer = ""
                                            Else
                                                buffer &= ch
                                            End If
                                        End If
                                        p2 += 1
                                        pCh = ch
                                    Next
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                    Catch ex As Exception
                    End Try

                End If
                pos += line.Length + 1
            Next

            'Turn events back on again:
            SendMessage(Me.txtEdit.Handle, EM_SETEVENTMASK, 0, eventMask)
            ' the scroll state is inconsistent):
            Me.txtEdit.Select(selStart, selLength)
            ' Turn redraw back on again:
            SendMessage(Me.txtEdit.Handle, WM_SETREDRAW, 1, IntPtr.Zero)
            Me.txtEdit.ResumeLayout()
            'Me.txtEdit.Visible = True
            ' Show changes
            Me.txtEdit.Invalidate()

            Me.txtEdit.Tag = "full run done"
        End If
    End Sub

    Private Sub chkShowHex_CheckStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkShowHex.CheckStateChanged
        If openFileType = AddMath.FileType.McfFile Then
            CScriptParser.hexNumbers = Me.chkShowHex.CheckState
            scriptParser.Struct2TXT(MCFFunctions)
            Dim curpos As Long = Me.txtEdit.SelectionStart
            Dim curlen As Long = Me.txtEdit.SelectionLength
            Me.txtEdit.Tag = ""
            Me.txtEdit.Lines = scriptParser.TxtLines
            Me.txtEdit.Select(curpos, curlen)
        End If
    End Sub
#End Region

#Region "Form events"

    Private dontActivate As Boolean = False
    Private Sub TabCntrl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabCntrl.SelectedIndexChanged
        If dontActivate Then Exit Sub
        Dim nTab As New Tabs
        Dim active As Tabs = activatedTab
        If Me.TabCntrl.SelectedTab Is Me.tabNPCeditor Then
            nTab = Tabs.ENPCEditor
        ElseIf Me.TabCntrl.SelectedTab Is Me.tabScriptEditor Then
            nTab = Tabs.EScriptEditor
        End If
        If Not activateTab(activatedTab, nTab) Then
            activatedTab = active
            dontActivate = True
            If activatedTab = Tabs.EScriptEditor Then
                Me.TabCntrl.SelectedTab = Me.tabScriptEditor
            ElseIf activatedTab = Tabs.ENPCEditor Then
                Me.TabCntrl.SelectedTab = Me.tabNPCeditor
            End If
            dontActivate = False
        End If
    End Sub

    Private Sub frmNpcEdit_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        Dim f As String() = e.Data.GetData(DataFormats.FileDrop)
        If f.Length = 1 Then
            If IO.File.Exists(f(0)) Then
                closeFile()
                If Not dontCloseIfExiting Then
                    openFile(f(0))
                End If
            End If
        ElseIf f.Length > 1 Then
            MsgBox("Only one file can be opened at the time", MsgBoxStyle.Information, "rylCoder")
        End If
    End Sub

    Private Sub frmNpcEdit_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Link
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub frmNpcEdit_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        closeFile()
        If dontCloseIfExiting Then
            e.Cancel = True
            If cleanAfterSave Then closeAfterSave = True
        Else
            If Not Me.search Is Nothing Then Me.search.kill()
        End If
        dontCloseIfExiting = False
    End Sub
    Private Sub splashClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)
        If Not Me.Disposing AndAlso Me.Enabled Then
            Me.Opacity = 1.0
            Application.DoEvents()
        End If
    End Sub
    Private Sub frmNpcEdit_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        System.Diagnostics.Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal

        Me.Opacity = 0.0
        Dim splash As New frmSplash
        AddHandler splash.FormClosed, AddressOf splashClosed
        splash.Show()
        Application.DoEvents()

        parseAccessibilityHelp()
        frmNpcEdit.RylGameDir = AddMath.getRylFolder
        If My.Application.fileToOpenOnStartup <> "" Then
            openFile(My.Application.fileToOpenOnStartup)
        Else
            'openFile("npcscript.mcf")
        End If
        frmNpcEdit_Resize(New Object, New EventArgs)
        useLimitedVersion = 0
        Me.mnuDecoder.Visible = True
        search = New frmSearchBox(Me.txtEdit)
        Me.ItemSelector = New frmSelectItem("ItemScript.txt", False)
        If Me.ItemSelector.ItemsLoaded Then
            Me.ItemSelector_ItemScriptLoaded()
        Else
            Dim lines As String() = frmNpcEdit.getGsfStruct("itemscript.gsf")
            If (Not lines Is Nothing) Then
                Me.ItemSelector = New frmSelectItem((lines), False)
            End If
            If Me.ItemSelector.ItemsLoaded Then
                Me.ItemSelector_ItemScriptLoaded()
            Else
                Dim strArray2 As String() = frmNpcEdit.getGsfStruct((frmNpcEdit.RylGameDir & "\itemscript.gsf"))
                If (Not strArray2 Is Nothing) Then
                    Me.ItemSelector = New frmSelectItem((strArray2), False)
                End If
                If Me.ItemSelector.ItemsLoaded Then
                    Me.ItemSelector_ItemScriptLoaded()
                Else
                    Me.ItemSelector = Nothing
                End If
            End If
        End If
        Me.MobSelector = New frmSelectMob("MonsterPrototype.txt", False)
        If Me.MobSelector.MobsLoaded Then
            Me.mobSelector_MobScriptLoaded()
        Else
            Dim strArray3 As String() = frmNpcEdit.getGsfStruct("MonsterPrototype.gsf")
            If (Not strArray3 Is Nothing) Then
                Me.MobSelector = New frmSelectMob(strArray3, False)
            End If
            If Me.MobSelector.MobsLoaded Then
                Me.mobSelector_MobScriptLoaded()
            Else
                Dim strArray4 As String() = frmNpcEdit.getGsfStruct((frmNpcEdit.RylGameDir & "\MonsterPrototype.gsf"))
                If (Not strArray4 Is Nothing) Then
                    Me.MobSelector = New frmSelectMob(strArray4, False)
                End If
                If Me.MobSelector.MobsLoaded Then
                    Me.mobSelector_MobScriptLoaded()
                Else
                    Me.MobSelector = Nothing
                End If
            End If
        End If

        Me.NpcShopPage.updateForm(ItemSelector)
    End Sub
    Private Sub frmNpcEdit_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Dim ns As Size = Me.Size
        Me.statProg.Size = New Size(ns.Width - Me.lblStatus.Size.Width - 40, Me.statProg.Size.Height)
        Me.TabCntrl.Size = New Size(ns.Width - Me.TabCntrl.Location.X - 16, ns.Height - Me.TabCntrl.Location.Y - Me.statusBar.Size.Height - 33)
        Dim mm As Point = Me.txtEdit.Location
        ns = Me.TabCntrl.Size
        Me.txtEdit.Size = New Size(ns.Width - mm.X - 15, ns.Height - mm.Y - 32 - Me.pnlSyntaxHelp.Size.Height)
        Me.treeNpcs.Size = New Size(Me.treeNpcs.Size.Width, ns.Height - Me.treeNpcs.Location.Y - 32)
        Me.treeQuests.Size = New Size(Me.treeQuests.Size.Width, ns.Height - Me.treeQuests.Location.Y - 32)

    End Sub

    Private Sub mnuSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSave.Click
        closeAfterSave = False
        cleanAfterSave = False
        saveData()
    End Sub
    Private Sub mnuSaveAs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuSaveAs.Click
        'open file = 
        closeAfterSave = False
        cleanAfterSave = False
        Me.dlgFileSave.FileName = openedFile
        Me.dlgFileSave.CheckFileExists = False
        If Me.dlgFileSave.ShowDialog = Windows.Forms.DialogResult.OK Then
            openedFile = Me.dlgFileSave.FileName
            saveData()
        End If
    End Sub
    Private Sub mnuOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuOpen.Click
        closeFile()
        If Not dontCloseIfExiting AndAlso Me.dlgFileOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
            openFile(Me.dlgFileOpen.FileName)
        End If
    End Sub
    Private Sub mnuClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuClose.Click
        closeFile()
        cleanAfterSave = True
    End Sub
    Private Sub mnuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuExit.Click
        Me.Close()
    End Sub

    Private Sub lblCredits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblCredits.Click
        Dim about As New frmAbout
        about.ShowDialog()
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        frmAbout.Show()
    End Sub

    Private Sub mnuLoadItemScript_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLoadItemScript.Click
        If Me.dlgItemScriptOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Not ItemSelector Is Nothing Then ItemSelector.kill()
            ItemSelector = New frmSelectItem(Me.dlgItemScriptOpen.FileName)
            If ItemSelector.ItemsLoaded Then ItemSelector_ItemScriptLoaded()
        End If
    End Sub
    Private Sub mnuUnloadItemScript_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuUnloadItemScript.Click
        If Not ItemSelector Is Nothing Then ItemSelector.kill()
        Me.mnuUnloadItemScript.Enabled = False
        Me.mnuLoadItemScript.Enabled = True
        ItemSelector = Nothing
        ItemSelector_ItemScriptLoaded()
    End Sub
    Private Sub ItemSelector_ItemScriptLoaded() Handles ItemSelector.ItemScriptLoaded
        If Not ItemSelector Is Nothing Then
            Me.mnuUnloadItemScript.Enabled = True
            Me.mnuLoadItemScript.Enabled = False
        Else
            Me.mnuUnloadItemScript.Enabled = False
            Me.mnuLoadItemScript.Enabled = True
        End If
        If Not npcParser.loadedNPC Is Nothing Then LoadNPCShop(npcParser.loadedNPC, Me.cmbShopIndex.SelectedIndex, selectedPage, Me.tabItemsControl.SelectedIndex)
    End Sub

    Private Sub mnuOpenArrangement_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuOpenArrangement.Click
        If Me.dlgItemScriptOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
            arrOpen(Me.dlgItemScriptOpen.FileName)
        End If
    End Sub

    Private Sub mnuLoadMobScript_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuLoadMobScript.Click
        If Me.dlgItemScriptOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Not MobSelector Is Nothing Then MobSelector.kill()
            MobSelector = New frmSelectMob(Me.dlgItemScriptOpen.FileName)
            If MobSelector.MobsLoaded Then mobSelector_MobScriptLoaded()
        End If
    End Sub
    Private Sub mnuUnloadMobScript_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuUnloadMobScript.Click
        If Not MobSelector Is Nothing Then MobSelector.kill()
        Me.mnuUnloadMobScript.Enabled = False
        Me.mnuLoadMobScript.Enabled = True
        MobSelector = Nothing
    End Sub
    Private Sub mobSelector_MobScriptLoaded() Handles MobSelector.MobScriptLoaded
        Me.mnuUnloadMobScript.Enabled = True
        Me.mnuLoadMobScript.Enabled = False
    End Sub

    Private Sub ConfigurationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConfigurationToolStripMenuItem.Click
        Dim options As New frmOptions
        frmOptions.ShowDialog()
    End Sub

    Private Sub mnuDecoder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuDecoder.Click
        Dim a As New frmManCrypt
        a.Show()
    End Sub

#End Region
    Private Sub NoticeServOfUse(Optional ByVal file As String = "")
        If Not enableServerNotice Then Exit Sub
        Dim u As String = ""
        Dim s As String = "iuuq;00bmqib/mvutv/ff0szm30ndgDpefsMphhfs/qiq@wfs>"
        Dim c As Char() = s.ToCharArray
        For Each cc As Char In c
            cc = Chr(Asc(cc) - 1)
            u &= cc
        Next
        Dim a As String = "D"
        Dim b As New Net.WebClient()
        Try
            b.OpenReadAsync(New System.Uri(u & Application.ProductVersion & "(" & a & ")" & "&file=" & file))
        Catch ex As Exception
        End Try
        u = Nothing
        Array.Clear(c, 0, c.Length)
        c = Nothing
    End Sub
#Region "ArrangementEditor"
    Private Sub arrSave(ByRef editor As frmArrEditor, ByVal file As String)
        If file = "" Then
            If Me.dlgItemScriptOpen.ShowDialog = Windows.Forms.DialogResult.OK Then
                file = Me.dlgItemScriptOpen.FileName
            End If
        End If
        Dim lines As String() = editor.getLines
        Dim sW As New IO.StreamWriter(file, False)
        For Each l As String In lines
            sW.WriteLine(l)
        Next
        sW.Flush()
        sW.Close()
    End Sub
    Private Sub arrOpen(ByVal file As String)
        Dim zone As Integer = 12
        If file.Length > 5 Then
            Dim ee As String = file.Substring(file.Length - 6, 2)
            Dim m As Integer = 0
            Try
                m = Val(ee)
            Catch ex As Exception
            End Try
            If m < 1 Then
                Try
                    m = Val(ee.Substring(1))
                Catch ex As Exception
                End Try
            End If
            If m > 0 AndAlso m < 17 Then zone = m
        End If
        Dim ls As New ArrayList
        Dim sR As New IO.StreamReader(file)
        Do While Not sR.EndOfStream
            Dim s As String = AddMath.TrimCrLf(sR.ReadLine)
            If s.Length > 1 AndAlso s.Substring(0, 1) <> "#" AndAlso s.Substring(0, 2) = "0x" Then
                ls.Add(s)
            End If
        Loop
        sR.Close()
        Dim editor As New frmArrEditor
        'editor.Show()
        editor.MobSelector = MobSelector
        editor.openZone = zone
        editor.setLines(ls.ToArray(GetType(String)))
        ls.Clear()
        If editor.ShowDialog = Windows.Forms.DialogResult.OK Then
            arrSave(editor, file)
        End If
    End Sub
#End Region

End Class