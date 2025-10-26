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

Public Class CScriptParser
    Private iFunctions As CMcfBase.SFunction() = {}
    Private iTxtLines As String() = {}
    Public RYLVersion As Integer = 0
    Public RYLFileType As CMcfBase.EFileType = CMcfBase.EFileType.EUnknown
    Private Shared iCulture As New System.Globalization.CultureInfo("en-US")
    Public Shared hexNumbers As CheckState = CheckState.Indeterminate
    Public LineFeed As String = vbLf

    Public Function Struct2File(ByRef functions As CMcfBase.SFunction(), ByVal fileName As String) As Boolean
        Try
            Struct2TXT(functions)
            Dim sW As New IO.StreamWriter(fileName, False)
            For Each l As String In iTxtLines
                sW.WriteLine(l)
            Next
            sW.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
        Return True
    End Function
    Public Sub Struct2TXT(ByRef functions As CMcfBase.SFunction())
        iFunctions = functions
        If iFunctions.Length < 1 Then
            iTxtLines = New String() {"//////// Please Load a file in first ////////"}
            Exit Sub
        End If
        Dim lS As New List(Of String)
        lS.Add("///////////////////////////////////////")
        lS.Add("//")
        If RYLVersion > 0 Then lS.Add("// RYL " & RYLVersion & " " & CMcfBase.SFileType(RYLFileType) & "")
        lS.Add("//")
        lS.Add("////////////// Functions //////////////")
        For Each f As CMcfBase.SFunction In iFunctions
            If f.isExternal Then
                Dim l As String = CMcfBase.DataTypeString(f.returnType) & " " & f.name & "("
                If f.parameterTypes.Length > 0 Then
                    For Each p As CMcfBase.DataType In f.parameterTypes
                        l &= CMcfBase.DataTypeString(p) & ", "
                    Next
                    l = l.Substring(0, l.Length - 2)
                End If
                l &= ");"
                lS.Add(l)
            End If
        Next
        lS.Add("///////////////////////////////////////")

        For Each f As CMcfBase.SFunction In iFunctions
            If Not f.isExternal Then
                If f.name <> "" Then 'Main function has no name
                    lS.Add("")
                    lS.Add("")
                    Dim l As String = CMcfBase.DataTypeString(f.returnType) & " " & f.name & "("
                    If f.parameterTypes.Length > 0 Then
                        For Each p As CMcfBase.DataType In f.parameterTypes
                            l &= CMcfBase.DataTypeString(p) & ", "
                        Next
                        l = l.Substring(0, l.Length - 2)
                    End If
                    l &= ")"
                    lS.Add(l)
                    lS.Add("{")
                End If
                For Each sL As CMcfBase.SScriptLine In f.data
                    Dim l As String = iFunctions(sL.callTo).name & "("
                    If sL.parameters.Length > 0 Then
                        For Each fp As CMcfBase.SParamElem In sL.parameters
                            Select Case fp.type
                                Case CMcfBase.DataType.EFloat
                                    Dim tmp As Single = fp.value
                                    l &= tmp.ToString(iCulture)
                                Case CMcfBase.DataType.EInteger
                                    If hexNumbers = CheckState.Checked Then
                                        l &= "0x" & Hex(fp.value)
                                    ElseIf hexNumbers = CheckState.Indeterminate Then
                                        l &= IIf(fp.value >= &HF00000, "0x" & Hex(fp.value), fp.value)
                                    Else
                                        l &= fp.value
                                    End If

                                Case CMcfBase.DataType.EString
                                    Dim tmp As String = fp.value
                                    l &= ControlChars.Quote & tmp.Replace(ControlChars.Quote, "\" & ControlChars.Quote) & ControlChars.Quote
                                Case CMcfBase.DataType.EBool
                                    l &= IIf(fp.value, "true", "false")
                            End Select
                            l &= ", "
                        Next
                        l = l.Substring(0, l.Length - 2)
                    End If
                    l &= ");"
                    lS.Add(addFormatting(l, iFunctions(sL.callTo)))
                Next
                If f.name <> "" Then lS.Add("}")
            End If
        Next
        lS.Add("")
        lS.Add("")
        lS.Add("////////// Created on " & Now() & " by rylCoder ////////////")
        iTxtLines = lS.ToArray()
    End Sub
    Public Function File2struct(ByVal fileName As String) As Boolean
        Try
            Dim sR As New IO.StreamReader(fileName)
            Dim txt As String = sR.ReadToEnd
            sR.Close()
            TXT2struct(txt)
        Catch ex As textException
            MsgBox(ex.ToString & " on line " & (ex.row + 1) & " on pos " & (ex.position + 1))
            Return False
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
        Return True
    End Function
    Public Shared Function detectLineFeed(ByRef txt As String) As String
        If txt.IndexOf(vbNewLine, 0) >= 0 Then
            Return vbNewLine
        ElseIf txt.IndexOf(vbLf, 0) >= 0 Then
            Return vbLf
        End If
        Return vbNewLine
    End Function
    Public Sub TXT2struct(ByRef txt As String)
        LineFeed = detectLineFeed(txt)
        TXT2struct(RemoveComments(txt).Split(LineFeed), False)
    End Sub
    Public Sub TXT2Struct(ByVal Lines As String(), Optional ByVal ParseComments As Boolean = True)
        Dim orgText As String = String.Join(LineFeed, Lines)
        If ParseComments Then Lines = RemoveComments(String.Join(LineFeed, Lines)).Split(LineFeed)
        Dim badChars As Char() = {vbNewLine, vbCr, vbLf, vbTab}
        Dim funcs As New List(Of CMcfBase.SFunction)
        Dim funcData As New List(Of List(Of CMcfBase.SScriptLine))

        ' Main function definition
        funcs.Add(CMcfBase.CreateMainFunction())
        funcData.Add(New List(Of CMcfBase.SScriptLine))

        Dim ongoingFunction As Integer = 0 'default is "main"

        For index As Long = 0 To Lines.Length - 1
            Dim line As String = Trim(Lines(index))
            If line.IndexOfAny(badChars) >= 0 Then
                For Each c As Char In badChars
                    line = line.Replace(c, "")
                Next
                line = line.Trim()
            End If

            If line.StartsWith("{") OrElse line.EndsWith("{") Then
                ongoingFunction = funcs.Count - 1 'The last function that was added to the array
                line = line.Trim("{"c)
            End If
            If line.StartsWith("}") OrElse line.EndsWith("}") Then
                ongoingFunction = 0 'Set back to main
                line = line.Trim("}"c)
            End If
            If line <> "" Then
                Dim functionFound As Boolean = False
                For Each sDataType As String In CMcfBase.DataTypeString
                    If line.StartsWith(sDataType & " ") Then
                        Try
                            funcs.Add(line2func(line, funcs.Count))
                        Catch ex As textException
                            ex.row = index + 1
                            ex.overallPos = orgText.IndexOf(Lines(index))
                            Throw
                        Catch ex As Exception
                            Dim ex2 As New textException(ex.Message, index + 1)
                            ex2.overallPos = orgText.IndexOf(Lines(index))
                            Throw ex2
                        End Try
                        funcData.Add(New List(Of CMcfBase.SScriptLine))
                        functionFound = True
                        Exit For
                    End If
                Next
                Try
                    If Not functionFound Then
                        funcData(ongoingFunction).Add(line2struct(line, funcs))
                    End If
                Catch ex As textException
                    ex.row = index + 1
                    ex.overallPos = orgText.IndexOf(Lines(index))
                    Throw
                Catch ex As Exception
                    Dim ex2 As New textException(ex.Message, index + 1)
                    ex2.overallPos = orgText.IndexOf(Lines(index))
                    Throw ex2
                End Try
            End If
        Next

        For i As Integer = 0 To funcs.Count - 1
            Dim f As CMcfBase.SFunction = funcs(i)
            f.data = funcData(i).ToArray()
            funcs(i) = f
        Next
        iTxtLines = Lines
        iFunctions = funcs.ToArray()
    End Sub
    Public ReadOnly Property TxtLines() As String()
        Get
            Return iTxtLines
        End Get
    End Property
    Public ReadOnly Property Functions() As CMcfBase.SFunction()
        Get
            Return iFunctions
        End Get
    End Property
    Private Function RemoveComments(ByVal sText As String) As String
        Const sStart$ = "/*", sEnd$ = "*/", sLine = "//"

        Dim bIsComment As Boolean = False
        Dim bInQuotes As Boolean = False

        Dim lPos%, N%
        Dim lastNline As Long = 0
        If Len(Trim$(sText)) = 0 Then RemoveComments = vbNullString : Exit Function

        For N = 1 To Len(sText)
            Select Case Mid$(sText, N, 1)
                Case Chr(34) 'quote
                    '
                    If Not bIsComment AndAlso ((N > 1 AndAlso bInQuotes AndAlso sText(N - 2) <> "\") OrElse (N < 2 OrElse Not bInQuotes OrElse sText(N - 3) = "\")) Then bInQuotes = Not bInQuotes
                    'Case "'"
                    '    If Not (bIsComment Or bInQuotes) Then
                    '        Mid(sText, N) = StrDup(Len(sText) - N + 1, Chr(0))
                    '        Exit For
                    '    End If
                Case Else
                    Select Case Mid$(sText, N, 2)
                        Case sLine
                            If Not (bIsComment Or bInQuotes) Then
                                Dim deb As Boolean = False
                                Dim lineend As Integer = sText.IndexOf(LineFeed, N + 1)
                                If lineend < 0 Then lineend = sText.Length
                                Mid$(sText, N, lineend - N + 1) = StrDup(lineend - N + 1, Chr(0))
                                N = N + 1
                            End If
                        Case sStart
                            If Not bInQuotes Then
                                lPos = N
                                bIsComment = True
                                N = N + 1
                            End If
                        Case sEnd
                            If Not bInQuotes Then
                                If lPos = 0 Then lPos = 1
                                Mid$(sText, lPos, N + Len(sEnd) - lPos) = StrDup(N + Len(sEnd) - lPos, Chr(0))
                                N = N + 1
                                bIsComment = False
                            End If
                        Case vbNewLine
                            If Not bIsComment Then
                                If bInQuotes Then
                                    Dim exText As String = Mid$(sText, N - 10, 20)
                                    Dim ex As New textException(7, 0, N - lastNline - 2)
                                    ex.overallPos = lastNline + 1
                                    Throw ex
                                End If
                            End If
                    End Select
            End Select
            If Mid$(sText, N, LineFeed.Length) = LineFeed Then lastNline = N
        Next N

        If bIsComment Then
            If lPos = 0 Then lPos = 1
            Mid$(sText, lPos, N + Len(sEnd) - lPos) = StrDup(N + Len(sEnd) - lPos, Chr(0))
        End If

        RemoveComments = Replace(sText, Chr(0), "")
        If Len(Trim$(RemoveComments)) Then RemoveComments = RemoveComments & LineFeed
    End Function
    Private Shared Function line2struct(ByVal txt As String, ByRef funcs As List(Of CMcfBase.SFunction)) As CMcfBase.SScriptLine
        Dim out As New CMcfBase.SScriptLine

        If txt.IndexOf("(") < 0 OrElse txt.IndexOf(");") < 0 Then Throw New textException(1, 0, txt.Length)
        Dim a As String() = {txt.Substring(0, txt.IndexOf("(")), txt.Substring(txt.IndexOf("(") + 1)}

        Dim b As String() = {a(1).Substring(0, a(1).LastIndexOf(");"))}
        'If b(0).Length < 1 Then Throw New textException(3, a(0).Length, 0) 'cose we have Else();

        ' parsing parameters
        Dim qIn As Boolean = False
        Dim pCh As Char = Chr(0)
        Dim buffer As String = ""
        Dim pars As New ArrayList
        For kl As Integer = 0 To b(0).Length - 1
            Dim ch As Char = b(0)(kl)
            If ch = ControlChars.Quote AndAlso pCh <> "\" Then
                qIn = Not qIn
                buffer &= ch
            Else
                If Not qIn AndAlso ch = "," Then
                    pars.Add(buffer)
                    buffer = ""
                Else
                    buffer &= ch
                End If
            End If
            pCh = ch
        Next
        If buffer <> "" Then pars.Add(buffer)

        Dim type As Integer = -1
        For i As Integer = 0 To funcs.Count - 1
            If Trim(a(0)) = funcs(i).name AndAlso funcs(i).parameterTypes.Length = pars.Count Then
                type = i
            End If
        Next
        ' end parsing parameters
        If type < 0 Then Throw New textException(2, 0, a(0).Length)
        out.callTo = type
        If pars.Count > 0 Then
            Dim pS(pars.Count - 1) As CMcfBase.SParamElem
            Dim pos As Integer = 0
            For i As Integer = 0 To pars.Count - 1
                Try
                    pS(i) = param2elem(Trim(pars(i)), funcs(out.callTo).parameterTypes(i))
                Catch ex As FormatException
                    Throw New textException(6, pos + a(0).Length, pars(i).Length)
                Catch ex As OverflowException
                    Throw New textException(5, pos + a(0).Length, pars(i).Length)
                End Try
                pos += pars(i).length + 1
            Next
            out.parameters = pS
        Else
            out.parameters = New CMcfBase.SParamElem() {}
        End If
        'If pars.Count <> out.func.parameterCount Then Throw New textException(4, a(0).Length + 1, b(0).Length)
        pars = Nothing


        Return out
    End Function
    Private Shared Function param2elem(ByVal param As String, ByVal type As CMcfBase.DataType) As CMcfBase.SParamElem
        Dim out As New CMcfBase.SParamElem
        out.type = type
        Select Case out.type
            Case CMcfBase.DataType.EBool
                out.value = (param = "true")
            Case CMcfBase.DataType.EFloat
                out.value = Single.Parse(param, iCulture)
            Case CMcfBase.DataType.EInteger
                If param.Length > 2 AndAlso param.Substring(0, 2) = "0x" Then
                    out.value = Convert.ToUInt32(param.Substring(2), 16)
                Else
                    out.value = UInt32.Parse(param)
                End If
            Case CMcfBase.DataType.EString
                out.value = param.Substring(1, param.Length - 2)
        End Select
        Return out
    End Function
    Private Shared Function line2func(ByVal line As String, ByVal id As Integer) As CMcfBase.SFunction
        If line.IndexOf("(") < 0 OrElse line.IndexOf(")") < 0 Then Throw New textException(1, 0, line.Length)
        Dim func As New CMcfBase.SFunction
        Dim a As String() = line.Split("(")
        Dim c As String() = a(0).Split(" ")
        For i As Integer = 0 To CMcfBase.DataTypeString.Length - 1
            If c(0) = CMcfBase.DataTypeString(i) Then
                func.returnType = CType(i, CMcfBase.DataType)
            End If
        Next
        func.name = c(1)
        func.isExternal = (a(1).Split(")").Length > 1 AndAlso a(1).Split(")")(1) = ";")
        Dim b As String() = a(1).Split(")")(0).Split(",")
        Dim pS As New List(Of CMcfBase.DataType)
        If b.Length > 1 OrElse b(0).Trim() <> "" Then
            For i As Integer = 0 To b.Length - 1
                Dim k As Integer = Array.IndexOf(CMcfBase.DataTypeString, b(i).Trim())
                If k > 0 Then
                    pS.Add(CType(k, CMcfBase.DataType))
                Else
                    Throw New textException(5, 0, line.Length)
                End If
            Next
        End If
        func.parameterTypes = pS.ToArray()
        func.id = id
        Return func
    End Function
    Private Function addFormatting(ByVal line As String, ByVal func As CMcfBase.SFunction) As String
        Dim Ryl2QuestFunctions As String() = {"QuestSkillPointBonus", "QuestType", "QuestArea", "QuestTitle", "QuestDesc", "QuestShortDesc", "QuestIcon", "QuestCompleteSave", "QuestLevel", "QuestAward", "AddPhase", "Phase_Target", "Trigger_Start", "Trigger_Puton", "Trigger_Geton", "Trigger_Talk", "Trigger_Kill", "Trigger_Pick", "Trigger_Fame", "Trigger_LevelTalk", "Else", "Event_Disappear", "Event_Get", "Event_Spawn", "Event_MonsterDrop", "Event_Award", "Event_MsgBox", "Event_Phase", "Event_End", "Event_AwardItem", "Event_AddQuest", "Event_Move", "Event_TheaterMode"}
        If RYLFileType = CMcfBase.EFileType.EQuest Then
            Dim defLvl As Integer = 3
            Dim lvl0 As String() = {"QuestEnd", "QuestStart"}
            Dim lvl1 As String() = {"Quest", "AddPhase"}
            Dim lvl2 As String() = {"Phase_", "Trigger_", "Else"}
            For Each l As String In lvl0
                If func.name.StartsWith(l) Then
                    GoTo addEnters
                End If
            Next
            For Each l As String In lvl1
                If func.name.StartsWith(l) Then
                    line = vbTab & line
                    GoTo addEnters
                End If
            Next
            For Each l As String In lvl2
                If func.name.StartsWith(l) Then
                    line = vbTab & vbTab & line
                    GoTo addEnters
                End If
            Next
            line = vbTab & vbTab & vbTab & line
            GoTo addEnters
        ElseIf RYLFileType = CMcfBase.EFileType.ENpcScript Then
            If func.name = "SetNPC" Then GoTo addEnters
            line = vbTab & line
            GoTo addEnters
        End If
        'Return line
addEnters:
        If func.name = "SetNPC" OrElse func.name = "QuestStart" Then
            line = LineFeed & LineFeed & line
        ElseIf func.name = "AddPhase" Then
            line = LineFeed & line
        End If
        Return line
    End Function
End Class

Public Class textException
    Inherits Exception
    Public Sub New(ByVal nr As Integer, ByVal pos As Long, ByVal len As Long, Optional ByVal line As Long = 0)
        code = nr
        position = pos
        length = len
        row = line
    End Sub
    Public Sub New(ByVal text As String, Optional ByVal line As Long = 0)
        code = &HFF
        otherInf = text
        row = line
    End Sub
    Public code As Integer
    Public position As Long
    Public overallPos As Long
    Public length As Long
    Public row As Long
    Private otherInf As String
    Public Overrides Function ToString() As String
        Select Case code
            Case 1 : Return "Non valid line. ( or ); is missing"
            Case 2 : Return "Non valid line. Unknown function or incorrect parameter count"
                'Case 3 : Return "Non valid line. No function parameters"
            Case 4 : Return "Non valid line. Function parameter count invalid"
            Case 5 : Return "Non valid line. Function parameter out of range"
            Case 6 : Return "Non valid line. Function parameter is not in HEX format"
            Case 7 : Return "String parameter not ended"
            Case &HFF : Return otherInf
        End Select
        Return "Non valid script"
    End Function
End Class