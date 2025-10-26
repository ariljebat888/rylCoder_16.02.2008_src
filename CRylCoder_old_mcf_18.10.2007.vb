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

Imports System.Reflection
Imports System.Diagnostics

Public Class CMcfCoder
#Region "Data"
    Friend Shared xorKey() As Byte = {&HAC, &H29, &H55, &H42}
#End Region
    Public Enum Col
        EFirstCol = 0
        ESecondCol = 1
        EThirdCol = 2
        EForthCol = 3
    End Enum

    Public Shared Function DeCryptByte(ByVal num As Byte, Optional ByVal column As Col = Col.EFirstCol) As Byte
        Return (num Xor xorKey(column))
    End Function

    Public Shared Function EnCryptByte(ByVal num As Byte, Optional ByVal column As Col = Col.EFirstCol) As Byte
        Return (num Xor xorKey(column))
    End Function

    Public Shared Function DeCryptArea(ByRef data As Byte(), Optional ByVal startCol As Col = Col.EFirstCol) As Byte()
        Dim out(data.Length - 1) As Byte
        Dim pos As Integer = startCol
        Dim i As Long = 0
        For Each b As Byte In data
            out(i) = DeCryptByte(b, pos)
            pos += 1
            i += 1
            If pos > 3 Then pos = 0
        Next
        Return out
    End Function

    Public Shared Function EnCryptArea(ByRef data As Byte(), Optional ByVal startCol As Col = Col.EFirstCol) As Byte()
        Dim out(data.Length - 1) As Byte
        Dim pos As Integer = startCol
        Dim i As Long = 0
        For Each b As Byte In data
            out(i) = EnCryptByte(b, pos)
            pos += 1
            i += 1
            If pos > 3 Then pos = 0
        Next
        Return out
    End Function

    Private Shared Function printCol(ByVal column As Col) As Integer
        Select Case column
            Case Col.EFirstCol : Return 0
            Case Col.ESecondCol : Return 1
            Case Col.EThirdCol : Return 2
            Case Col.EForthCol : Return 3
        End Select
    End Function

    Public Shared Function getCol(ByVal index As Long) As Col
        Return ModulusFromDivination(index, 4)
    End Function

    Public Shared Function ModulusFromDivination(ByVal nr1 As Long, ByVal nr2 As Long) As Integer
        Dim div As Double = nr1 / nr2
        Dim rDiv As Long = Math.Floor(div)
        If div = rDiv Then
            Return 0
        Else
            'Dim fp As Long = Convert.ToInt64(div.ToString.Split(New Char() {".", ","})(0), 10)
            Return nr1 - (nr2 * rDiv)
        End If
    End Function

    Public Shared Function position(ByRef data As Byte(), ByRef searchFor As Byte(), Optional ByVal startFrom As Long = 0, Optional ByVal numberOfMaxresults As Integer = 0, Optional ByVal Length As Long = 0) As Long()
        Dim poses As Long() = {}
        For index As Long = startFrom To IIf(Length > 0, startFrom + Length, data.Length - 1)
            If data.Length - index >= searchFor.Length Then
                Dim arr As Byte() = {}
                For i As Integer = 0 To searchFor.Length - 1
                    ReDim Preserve arr(i)
                    arr(i) = data(index + i)
                Next
                If compareArr(arr, searchFor) Then
                    ReDim Preserve poses(UBound(poses) + 1)
                    poses(UBound(poses)) = index
                    If poses.Length = numberOfMaxresults Then Exit For
                End If
            Else
                Exit For
            End If
        Next
        Return poses
    End Function

    Private Shared Function compareArr(ByRef d1 As Byte(), ByRef d2 As Byte()) As Boolean
        For i As Integer = 0 To d1.Length - 1
            If UBound(d2) < i OrElse d1(i) <> d2(i) Then Return False
        Next
        Return True
    End Function

    Public Shared Function SpliceArr(ByRef data As Byte(), ByVal startPos As Long, ByVal endPos As Long) As Byte()
        If data Is Nothing Then Return New Byte() {}
        If endPos - startPos <= 0 OrElse startPos >= data.Length OrElse endPos >= data.Length Then Return New Byte() {}
        Dim out(endPos - startPos) As Byte
        Dim i As Long = 0
        For index As Long = startPos To endPos
            out(i) = data(index)
            i += 1
        Next
        Return out
    End Function

    Public Shared Function SpliceArrByLen(ByRef data As Byte(), ByVal startPos As Long, ByVal length As Long) As Byte()
        Return SpliceArr(data, startPos, startPos + length - 1)
    End Function
End Class

Public Class CGsfCoder
    Public Shared gsfName As String() = {"ItemScript", "MonsterProtoType", "Chemical", "Script1", "SkillScript", "SpecialCompensation", "MineralVein"}
    Public Enum gsfType
        EItemScript = 0
        EMonsterProtoType = 1
        EChemical = 2
        EScript1 = 3
        ESkillScript = 4
        ESpecialCompensation = 5
        EMineralVein = 6
    End Enum
    Public Structure GsfFile
        Dim picture As Byte()
        Dim gsfData As Byte()
        Dim type As gsfType
        Dim version As Integer
    End Structure
    Public Enum DataType
        ENull = 0
        EBool = 1
        EInteger = 2
        EFloat = 3
        EString = 4
    End Enum
    Public Structure SParamElem
        Dim value As Object
        Dim type As DataType
        'Dim opDataLen As Integer
    End Structure
    Public Structure STableLine
        Dim params As SParamElem()
    End Structure

#Region "Data"
    Friend Shared typeCodes As Integer() = {17073, 60006, 41094, 17073, 50407, 84703, 214233}
    Friend Shared xorDat As String() = { _
        "A3 49 DC EA 09 B7 01 A4 A1 11 11 8E 80 35 5B DD 38 D5 4E 36 0C A2 BB 05 36 57 2E 98 BE 88 3C 28 43 63 A0 E9 E1 6D 51 CB", _
        "4D 62 84 43 89 C7 89 83 65 29 53 95 7C C0 A1 0C DB D7 04 D8 6A D1 73 1D 21 67 86 8D A4 A0 34 BD 31 20 61 0E E9 63 B4 C0", _
        "A3 49 DC EA 09 B7 01 A4 A1 11 11 8E 80 35 5B DD 38 D5 4E 36 0C A2 BB 05 36 57 2E 98 BE 88 3C 28 43 63 A0 E9 E1 6D 51 CB", _
        "34 B5 B2 3D 7D 43 8C C0 21 25 CD B6 53 76 CE 5D D4 87 CA 84 81 CB 5E 04 BA 69 3E 65 DE 21 8A 63 62 71 90 87 0A 52 28 44", _
        "34 B5 B2 3D 7D 43 8C C0 21 25 CD B6 53 76 CE 5D D4 87 CA 84 81 CB 5E 04 BA 69 3E 65 DE 21 8A 63 62 71 90 87 0A 52 28 44", _
        "A3 49 DC EA 09 B7 01 A4 A1 11 11 8E 80 35 5B DD 38 D5 4E 36 0C A2 BB 05 36 57 2E 98 BE 88 3C 28 43 63 A0 E9 E1 6D 51 CB", _
        "4D 62 84 43 89 C7 89 83 65 29 53 95 7C C0 A1 0C DB D7 04 D8 6A D1 73 1D 21 67 86 8D A4 A0 34 BD 31 20 61 0E E9 63 B4 C0 " _
    }
    Private Const saveBuffer As Long = 10 * 1024 * 1024
#End Region
    Private Structure testIdLine
        Dim id As Integer
        Dim line As String
    End Structure
    Private Shared Function lineForId(ByVal coll As ArrayList, ByVal id As Integer) As String
        For Each l As testIdLine In coll
            If l.id = id Then
                Return l.line
            End If
        Next
        Return ""
    End Function
    Public Shared Function Struct2text(ByRef table As STableLine(), ByVal type As gsfType, Optional ByVal version As Integer = 0) As String()
        Dim struct As SGsfDataStructure = getStructureInfo(type)
        Dim lines(table.Length + 8) As String
        Dim lcnt As Integer = 9
        lines(0) = "///////////////////////////////////////////////////////"
        lines(1) = "//"
        lines(2) = "// " & [Enum].GetName(GetType(gsfType), type).Substring(1) & IIf(version > 0, " ver. " & version, "")
        lines(3) = "//"
        lines(4) = "// Created by rylCoder " & Application.ProductVersion.Substring(0, Application.ProductVersion.Length - 2) & " © 2006 & 2007 AlphA"
        lines(5) = "//"
        lines(6) = "///////////////////////////////////////////////////////"
        lines(7) = ""
        lines(8) = ""
        If struct.redirections.Length < 1 Then
            lines(8) = "//"
            Dim cells As SGsfDataCell() = struct.cells
            Array.Sort(cells, New CDataCellColComparer)
            For Each s As SGsfDataCell In cells
                If Not s.hide Then lines(8) &= s.name & vbTab
            Next
            lines(8) = lines(8).Substring(0, lines(8).Length - vbTab.Length)
        End If
        For Each t As CGsfCoder.STableLine In table
            lines(lcnt) = New String("")
            For Each p As CGsfCoder.SParamElem In t.params
                lines(lcnt) &= p.value & vbTab
            Next
            If lines(lcnt).Length > 0 Then lines(lcnt) = lines(lcnt).Substring(0, lines(lcnt).Length - vbTab.Length)
            lcnt += 1
        Next
        ''for working on gsfStruct only!
        ''once its ready this part has to be deleted
        'Dim larr As New ArrayList
        'larr.Add(lines(8))
        'Dim sr As New IO.StreamReader("MonsterPrototype.txt")
        'Dim l2arr As New ArrayList
        'sr.ReadLine()
        'Do While Not sr.EndOfStream
        '    Dim l As String = sr.ReadLine.Trim
        '    Dim id As Integer = Val(l.Split(" ")(0))
        '    Dim c As New testIdLine
        '    c.id = id
        '    c.line = l
        '    l2arr.Add(c)
        'Loop
        'sr.Close()
        'For j As Integer = 9 To lines.Length - 1
        '    Dim id As Integer = Val(lines(j).Split(" ")(0))
        '    Dim l As String = lineForId(l2arr, id)
        '    If l <> "" Then
        '        larr.Add(lines(j))
        '        Dim cc As String() = l.Split(vbTab)
        '        larr.Add(String.Join(vbTab, cc))
        '    End If
        'Next
        'lines = larr.ToArray(GetType(String))
        ''til(here)
        Return lines
    End Function
    Public Shared Function Text2Struct(ByRef lines As String()) As STableLine()
        'Dim t As String = "E" & lines(2).Substring(3)
        'Dim type As gsfType = [Enum].Parse(GetType(gsfType), t, True)
        Dim table As New ArrayList
        For i As Integer = 0 To lines.Length - 1
            Dim l As String = lines(i).Trim()
            If l <> "" AndAlso l.Length > 0 AndAlso (l.Length < 2 OrElse l.Substring(0, 2) <> "//") Then
                Dim splices As String() = l.Split(vbTab)
                Dim params(splices.Length - 1) As SParamElem
                For k As Integer = 0 To splices.Length - 1
                    params(k) = CreateParamElem(DataType.EString, splices(k))
                Next
                Dim tl As New STableLine
                tl.params = params
                table.Add(tl)
            End If
        Next
        Return table.ToArray(GetType(STableLine))
    End Function

    Public Shared Function Data2Struct(ByRef data As Byte(), ByVal type As gsfType, Optional ByVal testVersion As Integer = 0, Optional ByRef resultVersion As Integer = 0) As STableLine()
        Dim struct As SGsfDataStructure = getStructureInfo(type, testVersion)
        If struct.version > 0 Then resultVersion = struct.version
        Dim out As New ArrayList
        ' ------ rules ------
        Dim enableEmptyLineIgnore As Boolean = False
        Dim emptyLineIgnoreCol As Integer = 0
        Dim emptyLineIgnoreVal As Object = Nothing
        Dim emptyLineIgnoreRepeat As Integer = 0
        Dim emptyLineCounter As Integer = 0
        Dim emptyLineCounterActive As Boolean = False
        If Not struct.rules Is Nothing Then
            Dim n As Xml.XmlNode = struct.rules.SelectSingleNode("repeatemptyline")
            If Not n Is Nothing Then
                enableEmptyLineIgnore = True
                emptyLineIgnoreCol = n.Attributes.GetNamedItem("listentocol").Value - 1
                emptyLineIgnoreVal = n.Attributes.GetNamedItem("onvalue").Value
                'If Val(emptyLineIgnoreVal) = emptyLineIgnoreVal Then emptyLineIgnoreVal = Val(emptyLineIgnoreVal)
                emptyLineIgnoreRepeat = n.Attributes.GetNamedItem("repeat").Value
            End If
        End If
        ' ---- end rules ----
        If struct.cellsSize > 0 Then
            Dim sr As New IO.BinaryReader(New IO.MemoryStream(data))
            Do While sr.BaseStream.Position < sr.BaseStream.Length
                Dim params As New ArrayList
                Dim lineEnd As Boolean = False
                Do While Not lineEnd
                    For Each p As SGsfDataCell In struct.cells
                        Dim e As SParamElem = CreateParamElem(getParamElemType(p.dataType), readCell(sr, p))
                        'If p.col = 2 Then Debug.WriteLine(e.value)
                        If p.replace > 0 Then
                            e.type = DataType.EString
                            For Each rep As SGsfReplacementTable In struct.replacements
                                If rep.id = p.replace Then
                                    For Each repE As SGsfReplacementElem In rep.elems
                                        If repE.fromItem = e.value Then
                                            e.value = repE.toItem
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        ElseIf p.multiplier > 0 Then
                            e.value = e.value * p.multiplier
                        End If
                        If Not p.hide Then params.Add(e)
                    Next
                    If struct.hasLineSplit AndAlso struct.lineSpplit.len > 0 Then
                        Dim e As SParamElem = CreateParamElem(getParamElemType(struct.lineSpplit.dataType), readCell(sr, struct.lineSpplit))
                        If e.value = struct.lineSpplit.value Then
                            lineEnd = True
                        Else
                            lineEnd = False
                            sr.BaseStream.Seek(struct.lineSpplit.len * (-1), IO.SeekOrigin.Current)
                        End If
                    Else
                        lineEnd = True
                    End If
                Loop
                If emptyLineCounterActive Then
                    If emptyLineCounter = emptyLineIgnoreRepeat Then
                        emptyLineCounterActive = False
                    End If
                    emptyLineCounter += 1
                End If
                If params.Count > 0 Then
                    Dim nl As New STableLine
                    Dim paramsL(params.Count - 1) As SParamElem
                    If struct.hasColNums Then
                        Dim sI As Integer = 0
                        Dim usedCols As New ArrayList
                        For Each p As SGsfDataCell In struct.cells
                            If p.col > 0 AndAlso Not p.hide Then
                                paramsL(p.col - 1) = params(sI)
                                If usedCols.IndexOf(p.col) >= 0 Then
                                    Throw New Exception("Col " & p.col & " used multiple times")
                                End If
                                usedCols.Add(p.col)
                                sI += 1
                            End If
                        Next
                    Else
                        paramsL = params.ToArray(GetType(SParamElem))
                    End If
                    nl.params = redirectCells(struct, paramsL)
                    'Dim ka As SParamElem() = redirectCells(struct, nl.params, False)
                    If Not emptyLineCounterActive Then
                        out.Add(nl)
                    End If

                    If enableEmptyLineIgnore AndAlso nl.params(emptyLineIgnoreCol).value = emptyLineIgnoreVal AndAlso Not emptyLineCounterActive Then
                        emptyLineCounterActive = True
                        emptyLineCounter = 0
                    End If
                    'Debug.WriteLine(sr.BaseStream.Position & ": " & params(22).value)
                End If
            Loop
            sr.Close()
        End If
        Return out.ToArray(GetType(STableLine))
    End Function
    Public Shared Function Struct2Data(ByRef table As STableLine(), ByVal type As gsfType, Optional ByVal version As Integer = 0) As Byte()
        Dim struct As SGsfDataStructure = getStructureInfo(type, version)
        Dim buff(saveBuffer - 1) As Byte
        Dim sw As New IO.BinaryWriter(New IO.MemoryStream(buff))
        ' ------ rules ------
        Dim enableEmptyLineIgnore As Boolean = False
        Dim emptyLineIgnoreCol As Integer = 0
        Dim emptyLineIgnoreVal As Object = Nothing
        Dim emptyLineIgnoreRepeat As Integer = 0
        Dim emptyLineCounter As Integer = 0
        Dim emptyLineCounterActive As Boolean = False
        If Not struct.rules Is Nothing Then
            Dim n As Xml.XmlNode = struct.rules.SelectSingleNode("repeatemptyline")
            If Not n Is Nothing Then
                enableEmptyLineIgnore = True
                emptyLineIgnoreCol = n.Attributes.GetNamedItem("listentocol").Value - 1
                emptyLineIgnoreVal = n.Attributes.GetNamedItem("onvalue").Value
                'If Val(emptyLineIgnoreVal) = emptyLineIgnoreVal Then emptyLineIgnoreVal = Val(emptyLineIgnoreVal)
                emptyLineIgnoreRepeat = n.Attributes.GetNamedItem("repeat").Value
            End If
        End If
        ' ---- end rules ----
        Dim row As Integer = 0
        Do While row < table.Length OrElse emptyLineCounterActive
            Dim params As SParamElem() = {}
            If emptyLineCounterActive Then
                If emptyLineCounter = emptyLineIgnoreRepeat Then
                    emptyLineCounterActive = False
                End If
                emptyLineCounter += 1
            End If
            If Not emptyLineCounterActive Then params = redirectCells(struct, table(row).params, False)
            If enableEmptyLineIgnore AndAlso Not emptyLineCounterActive AndAlso params(emptyLineIgnoreCol).value = emptyLineIgnoreVal Then
                emptyLineCounterActive = True
                emptyLineCounter = 0
            End If
            If struct.hasColNums AndAlso (Not emptyLineCounterActive OrElse emptyLineCounter = 0) Then
                Dim sI As Integer = 0
                Dim paramsL(params.Length - 1) As SParamElem
                For Each p As SGsfDataCell In struct.cells
                    If p.col > 0 AndAlso Not p.hide Then
                        paramsL(sI) = params(p.col - 1)
                        sI += 1
                    End If
                Next
                params = paramsL
            End If
            Dim colSn As Integer = 0
            Dim colEn As Integer = 0
            Dim mCol As Integer = params.Length - 1
            If struct.cells.Length - 1 > mCol Then mCol = struct.cells.Length - 1
            For col As Integer = 0 To mCol
                If colSn > struct.cells.Length - 1 Then colSn = 0
                Dim p As SGsfDataCell = struct.cells(colSn)
                Dim v As Object = 0
                If p.hide Then
                    v = p.value
                ElseIf emptyLineCounterActive AndAlso emptyLineCounter > 0 Then
                    If p.dataType Is GetType(Char()) Then
                        v = ""
                    ElseIf p.dataType Is GetType(Char) Then
                        v = Chr(0)
                    Else
                        v = 0
                    End If
                Else
                    v = params(colEn).value
                End If
                '---------------------- replacement start ----------------------
                If p.replace > 0 AndAlso (Not emptyLineCounterActive OrElse emptyLineCounter = 0) Then
                    For Each rep As SGsfReplacementTable In struct.replacements
                        If rep.id = p.replace Then
                            For Each repE As SGsfReplacementElem In rep.elems
                                If repE.toItem = v Then
                                    v = repE.fromItem
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                ElseIf p.multiplier > 0 Then
                    v = v / p.multiplier
                End If
                '---------------------- replacement end ------------------------
                writeCell(sw, p, v)
                colSn += 1
                If Not p.hide Then colEn += 1 '1 round late when hide
            Next
            If struct.hasLineSplit Then
                writeCell(sw, struct.lineSpplit, struct.lineSpplit.value)
            End If
            If Not emptyLineCounterActive OrElse emptyLineCounter = 0 Then row += 1
        Loop
        Dim len As Long = sw.BaseStream.Position
        sw.Seek(0, IO.SeekOrigin.Begin)
        Dim out(len - 1) As Byte
        sw.BaseStream.Read(out, 0, len)
        sw.Close()
        Return out
    End Function

    Public Shared Function Crypt(ByRef file As GsfFile) As Byte()
        Dim cr As Byte() = CryptArea(file.gsfData, file.type)
        Dim out(file.picture.Length + cr.Length - 1) As Byte
        Array.ConstrainedCopy(file.picture, 0, out, 0, file.picture.Length)
        Array.ConstrainedCopy(cr, 0, out, file.picture.Length + 0, cr.Length)
        Return out
    End Function
    Public Shared Function DeCrypt(ByRef data As Byte()) As GsfFile 'for full picture
        Dim gf As New GsfFile
        gf.type = getGsfType(data)
        Dim foundPos As Long = getFileSplitPos(data)
        Dim image(foundPos - 1) As Byte
        Dim gsfdata(data.Length - foundPos - 1) As Byte
        Array.ConstrainedCopy(data, 0, image, 0, image.Length)
        Array.ConstrainedCopy(data, foundPos, gsfdata, 0, gsfdata.Length)
        gf.picture = image
        gf.gsfData = DeCryptArea(gsfdata, gf.type)
        Return gf
    End Function

    Public Shared Function CryptArea(ByVal data As Byte(), ByVal type As gsfType) As Byte()
        xorDataArea(data, GetXorData(type))
        For i As Integer = 0 To data.Length - 1
            Dim b As Byte = data(i)
            If b > &H7F Then
                data(i) = (b - &H80) * 2 + 1
            Else
                data(i) = b * 2
            End If
        Next
        Dim compData As Byte() = Compress(data)
        Dim ndata(compData.Length + 3) As Byte
        AddMath.SetUInt32inBytes(ndata, data.Length, 0)
        Array.ConstrainedCopy(compData, 0, ndata, 4, compData.Length)
        Return ndata
    End Function
    Public Shared Function DeCryptArea(ByRef data As Byte(), ByVal type As gsfType) As Byte() 'for gsf data only
        Dim len As Integer = AddMath.getUInt32(0, data)
        Dim sliceData(data.Length - 5) As Byte
        Array.ConstrainedCopy(data, 4, sliceData, 0, sliceData.Length)
        Dim decompData As Byte() = DeCompress(sliceData, len)
        For i As Integer = 0 To decompData.Length - 1
            Dim b As Byte = decompData(i)
            If b Mod 2 Then
                decompData(i) = (b - 1) / 2 + &H80
            Else
                decompData(i) = b / 2
            End If
        Next
        xorDataArea(decompData, GetXorData(type))
        Return decompData
    End Function

    Private Shared Function Compress(ByRef data As Byte()) As Byte()
        Return FischR.Wrapper.Compress(data)
    End Function
    Private Shared Function DeCompress(ByRef data As Byte(), ByVal unCompLength As Integer) As Byte()
        Return FischR.Wrapper.Decompress(data, unCompLength)
    End Function

    Private Shared Function GetXorData(ByVal type As gsfType) As Byte()
        Dim xorStr As String = ""
        If type > -1 AndAlso type < xorDat.Length Then
            xorStr = xorDat(type)
        End If
        Dim slices As String() = xorStr.Trim.Split(" ")
        Dim out(slices.Length - 1) As Byte
        For i As Integer = 0 To slices.Length - 1
            If slices(i).Trim <> "" Then out(i) = Byte.Parse(slices(i), Globalization.NumberStyles.HexNumber)
        Next
        Return out
    End Function
    Private Shared Sub xorDataArea(ByRef data As Byte(), ByRef key As Byte())
        Dim col As Integer = 0
        For i As Integer = 0 To data.Length - 1
            If col > key.Length - 1 Then
                col = 0
            End If
            Dim vi As Byte = data(i)
            Dim vo As Byte = vi Xor key(col)
            data(i) = vo
            col += 1
        Next
    End Sub
    Public Shared Function getGsfType(ByRef file As Byte()) As gsfType
        Dim typec As UInt32 = AddMath.getUInt32(0, file)
        Dim found As Boolean = False
        For j As Integer = 0 To typeCodes.Length - 1
            If typeCodes(j) = typec Then
                If j = gsfType.EItemScript Then
                    If file.Length < 50 * 1024 Then j = gsfType.EScript1
                End If
                Return j
            End If
        Next
        Throw New Exception("Unrecognized gsf file type")
    End Function
    Public Shared Function getFileSplitPos(ByRef data As Byte()) As Long
        Dim lB As Byte = 0
        Dim i As Long = 0
        For Each b As Byte In data
            If lB = &HFF AndAlso b = &HD9 AndAlso data(i + 4) = 0 AndAlso data(i + 5) = 0 AndAlso (data(i + 1) <> &H38 OrElse data(i + 1) <> &H42 OrElse data(i + 1) <> &H49 OrElse data(i + 1) <> &H4D) Then
                Return i + 1
            End If
            lB = b
            i += 1
        Next
        Throw New Exception("GSF data cant be found inside this file")
    End Function
    Public Shared Function CreateParamElem(ByVal type As DataType, ByVal value As Object) As SParamElem
        Dim a As New SParamElem
        a.type = type
        a.value = value
        Return a
    End Function

    Private Structure SGsfReplacementElem
        Dim fromItem As Integer
        Dim toItem As String
    End Structure
    Private Structure SGsfReplacementTable
        Dim id As Integer
        Dim elems As SGsfReplacementElem()
    End Structure
    Private Structure SGsfRedirElem
        Dim name As String
        Dim fromCol As Integer
        Dim toCol As Integer
    End Structure
    Private Structure SGsfRedirTableTestItem
        Dim values As Object()
    End Structure
    Private Structure SGsfRedirTable
        Dim name As String
        Dim useAsHeader As Boolean
        Dim values As SGsfRedirTableTestItem()
        Dim elems As SGsfRedirElem()
    End Structure
    Private Structure SGsfDataCell
        Dim name As String
        Dim dataType As Type
        Dim len As Integer
        Dim value As Integer
        Dim hide As Boolean
        Dim col As Integer
        Dim replace As Integer
        Dim hex As Boolean
        Dim endTag As Byte
        Dim multiplier As Integer
        Dim redirCase As Integer
    End Structure
    Private Structure SGsfDataStructure
        Dim cells As SGsfDataCell()
        Dim lineSpplit As SGsfDataCell
        Dim hasLineSplit As Boolean
        Dim cellsSize As Integer
        Dim hasColNums As Boolean
        Dim replacements As SGsfReplacementTable()
        Dim redirections As SGsfRedirTable()
        Dim rules As Xml.XmlNode
        Dim version As Integer
    End Structure
    Private Shared Function getStructureInfo(ByVal type As gsfType, Optional ByVal version As Integer = 0) As SGsfDataStructure
        Dim x As New Xml.XmlDataDocument
        Dim t As String = [Enum].GetName(GetType(gsfType), type).Substring(1)
        Dim appN As String() = Application.ExecutablePath.Split(New Char() {"\", "/"})
        Dim appF As String = Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - appN(UBound(appN)).Length)
        If IO.File.Exists(appF & "gsfStruct.xml") Then
            x.Load(appF & "gsfStruct.xml")
        Else
            x.LoadXml(Global.rylCoder.My.Resources.gsfStruct)
        End If
        Dim fileN As Xml.XmlNode = Nothing
        If version < 0 Then
            Dim fNs As Xml.XmlNodeList = x.SelectNodes("gsf/file[@name='" & t & "']")
            Dim ind As Integer = Math.Abs(version) - 1
            If ind > fNs.Count - 1 Then
                Throw New GsfVersionLoopOutOfRange("Not supported GSF version")
            End If
            fileN = fNs.Item(ind)
        Else
            fileN = x.SelectSingleNode("gsf/file[@name='" & t & "'" & IIf(version > 0, " and @version=" & version, "") & "]")
        End If
        If fileN Is Nothing Then Throw New NotSupportedException("This GSF file is not supported")
        Dim dataS As New SGsfDataStructure
        If Not fileN.Attributes.GetNamedItem("version") Is Nothing Then dataS.version = AddMath.resolveInteger(fileN.Attributes.GetNamedItem("version").Value)
        Dim structN As Xml.XmlNode = fileN.SelectSingleNode("structure")
        If Not structN Is Nothing Then
            Dim cellsN As Xml.XmlNode = structN.SelectSingleNode("cells")
            Dim cellsNs As Xml.XmlNodeList = structN.SelectNodes("cells/cell")
            Dim splitN As Xml.XmlNode = structN.SelectSingleNode("linesplit")
            If Not cellsN Is Nothing Then
                dataS.cellsSize = AddMath.resolveInteger(cellsN.Attributes.GetNamedItem("size").Value)
                If Not cellsN.Attributes.GetNamedItem("hascolumns") Is Nothing Then dataS.hasColNums = IIf(AddMath.resolveInteger(cellsN.Attributes.GetNamedItem("hascolumns").Value) > 0, True, False)
            End If
            If Not splitN Is Nothing Then
                Dim cell As Xml.XmlNode = splitN
                Dim ca As New SGsfDataCell
                If Not cell.Attributes.GetNamedItem("type") Is Nothing Then ca.dataType = AddMath.resolveDataType(cell.Attributes.GetNamedItem("type").Value)
                If Not cell.Attributes.GetNamedItem("type") Is Nothing Then ca.len = AddMath.resolveDataTypeLen(cell.Attributes.GetNamedItem("type").Value)
                If Not cell.Attributes.GetNamedItem("hide") Is Nothing Then ca.hide = IIf(AddMath.resolveInteger(cell.Attributes.GetNamedItem("hide").Value) > 0, True, False)
                If Not cell.Attributes.GetNamedItem("hex") Is Nothing Then ca.hex = IIf(AddMath.resolveInteger(cell.Attributes.GetNamedItem("hex").Value) > 0, True, False)
                If Not cell.Attributes.GetNamedItem("name") Is Nothing Then ca.name = cell.Attributes.GetNamedItem("name").Value
                If Not cell.Attributes.GetNamedItem("val") Is Nothing Then ca.value = AddMath.resolveInteger(cell.Attributes.GetNamedItem("val").Value)
                If Not cell.Attributes.GetNamedItem("col") Is Nothing Then ca.col = AddMath.resolveInteger(cell.Attributes.GetNamedItem("col").Value)
                dataS.lineSpplit = ca
                dataS.hasLineSplit = True
            End If
            If cellsNs.Count > 0 Then
                Dim cArr(cellsNs.Count - 1) As SGsfDataCell
                Dim i As Integer = 0
                For Each cell As Xml.XmlNode In cellsNs
                    Dim ca As New SGsfDataCell
                    If Not cell.Attributes.GetNamedItem("type") Is Nothing Then ca.dataType = AddMath.resolveDataType(cell.Attributes.GetNamedItem("type").Value)
                    If Not cell.Attributes.GetNamedItem("type") Is Nothing Then ca.len = AddMath.resolveDataTypeLen(cell.Attributes.GetNamedItem("type").Value)
                    If Not cell.Attributes.GetNamedItem("hide") Is Nothing Then ca.hide = IIf(AddMath.resolveInteger(cell.Attributes.GetNamedItem("hide").Value) > 0, True, False)
                    If Not cell.Attributes.GetNamedItem("hex") Is Nothing Then ca.hex = IIf(AddMath.resolveInteger(cell.Attributes.GetNamedItem("hex").Value) > 0, True, False)
                    If Not cell.Attributes.GetNamedItem("name") Is Nothing Then ca.name = cell.Attributes.GetNamedItem("name").Value
                    If Not cell.Attributes.GetNamedItem("val") Is Nothing Then ca.value = AddMath.resolveInteger(cell.Attributes.GetNamedItem("val").Value)
                    If Not cell.Attributes.GetNamedItem("col") Is Nothing Then ca.col = AddMath.resolveInteger(cell.Attributes.GetNamedItem("col").Value)
                    If Not cell.Attributes.GetNamedItem("replace") Is Nothing Then ca.replace = AddMath.resolveInteger(cell.Attributes.GetNamedItem("replace").Value)
                    If Not cell.Attributes.GetNamedItem("endtag") Is Nothing Then ca.endTag = AddMath.resolveInteger(cell.Attributes.GetNamedItem("endtag").Value)
                    If Not cell.Attributes.GetNamedItem("multiplier") Is Nothing Then ca.multiplier = AddMath.resolveInteger(cell.Attributes.GetNamedItem("multiplier").Value)
                    If Not cell.Attributes.GetNamedItem("redir_case") Is Nothing Then ca.redirCase = AddMath.resolveInteger(cell.Attributes.GetNamedItem("redir_case").Value)
                    cArr(i) = ca
                    i += 1
                Next
                dataS.cells = cArr
            End If
        End If
        Dim repsN As Xml.XmlNodeList = fileN.SelectNodes("replacements/replacement")
        Dim reps As SGsfReplacementTable() = {}
        If repsN.Count > 0 Then
            ReDim reps(repsN.Count - 1)
            Dim j As Integer = 0
            For Each rN As Xml.XmlNode In repsN
                Dim rep As New SGsfReplacementTable
                rep.id = rN.Attributes.GetNamedItem("id").Value
                Dim repENs As Xml.XmlNodeList = rN.SelectNodes("elem")
                Dim repelems As New ArrayList
                For Each repEN As Xml.XmlNode In repENs
                    Dim repE As New SGsfReplacementElem
                    repE.fromItem = AddMath.resolveInteger(repEN.Attributes.GetNamedItem("from").Value)
                    repE.toItem = repEN.Attributes.GetNamedItem("to").Value
                    repelems.Add(repE)
                Next
                rep.elems = repelems.ToArray(GetType(SGsfReplacementElem))
                reps(j) = rep
                j += 1
            Next
        End If
        dataS.replacements = reps
        Dim redirN As Xml.XmlNodeList = fileN.SelectNodes("redirections/redirection")
        Dim redirs As SGsfRedirTable() = {}
        If redirN.Count > 0 Then
            ReDim redirs(redirN.Count - 1)
            Dim j As Integer = 0
            For Each rN As Xml.XmlNode In redirN
                Dim red As New SGsfRedirTable
                red.name = rN.Attributes.GetNamedItem("name").Value
                Dim vs As String() = rN.Attributes.GetNamedItem("values").Value.Split(";")
                Dim vals(vs.Length - 1) As SGsfRedirTableTestItem
                Dim jj As Integer = 0
                For Each vv As String In vs
                    vals(jj) = New SGsfRedirTableTestItem
                    vals(jj).values = vv.Split(",")
                    jj += 1
                Next
                red.values = vals
                Dim redENs As Xml.XmlNodeList = rN.SelectNodes("cell")
                Dim redelems As New ArrayList
                jj = 1
                For Each redEN As Xml.XmlNode In redENs
                    Dim redE As New SGsfRedirElem
                    redE.fromCol = AddMath.resolveInteger(redEN.Attributes.GetNamedItem("col").Value)
                    redE.name = redEN.Attributes.GetNamedItem("name").Value
                    redE.toCol = jj
                    If redE.fromCol > 0 Then
                        redelems.Add(redE)
                        jj += 1
                    End If
                Next
                red.elems = redelems.ToArray(GetType(SGsfRedirElem))
                redirs(j) = red
                j += 1
            Next
        End If
        dataS.redirections = redirs
        dataS.rules = fileN.SelectSingleNode("rules")
        Return dataS
    End Function
    Private Shared Function getParamElemType(ByRef type As Type) As DataType
        If type Is GetType(Byte) Then
            Return DataType.EInteger
        ElseIf type Is GetType(Int16) Then
            Return DataType.EInteger
        ElseIf type Is GetType(UInt16) Then
            Return DataType.EInteger
        ElseIf type Is GetType(Int32) Then
            Return DataType.EInteger
        ElseIf type Is GetType(UInt32) Then
            Return DataType.EInteger
        ElseIf type Is GetType(Char) Then
            Return DataType.EString
        ElseIf type Is GetType(Char()) Then
            Return DataType.EString
        End If
        Return DataType.ENull
    End Function
    Private Shared Function readCell(ByRef reader As IO.BinaryReader, ByRef cell As SGsfDataCell) As Object
        If cell.dataType Is GetType(Byte) Then
            Return addHextag(reader.ReadByte(), cell)
        ElseIf cell.dataType Is GetType(Int16) Then
            Return addHextag(reader.ReadInt16, cell)
        ElseIf cell.dataType Is GetType(UInt16) Then
            Return addHextag(reader.ReadUInt16, cell)
        ElseIf cell.dataType Is GetType(Int32) Then
            Return addHextag(reader.ReadInt32, cell)
        ElseIf cell.dataType Is GetType(UInt32) Then
            Return addHextag(reader.ReadUInt32, cell)
        ElseIf cell.dataType Is GetType(Char) Then
            Return Chr(reader.ReadByte)
        ElseIf cell.dataType Is GetType(Single) Then
            Return reader.ReadSingle
        ElseIf cell.dataType Is GetType(Char()) Then
            Dim bs As Byte() = reader.ReadBytes(cell.len)
            Dim ms As String = ""
            Dim i As Integer = 0
            Do While i < bs.Length AndAlso bs(i) <> cell.endTag AndAlso bs(i) <> 0
                ms &= Chr(bs(i))
                i += 1
            Loop
            Return ms
        End If
        Return 0
    End Function
    Private Shared Sub writeCell(ByRef writer As IO.BinaryWriter, ByRef cell As SGsfDataCell, ByVal data As Object)
        If cell.dataType Is GetType(Byte) Then
            Dim d As Byte = 0
            If Not cell.hex OrElse data.ToString.Length < 3 Then
                d = CType(data, Byte)
            Else
                d = Convert.ToByte(data.substring(2), 16)
            End If
            writer.Write(d)
        ElseIf cell.dataType Is GetType(Int16) Then
            Dim d As Int16 = 0
            If Not cell.hex OrElse data.ToString.Length < 3 Then
                d = CType(data, Int16)
            Else
                d = Convert.ToInt16(data.substring(2), 16)
            End If
            writer.Write(d)
        ElseIf cell.dataType Is GetType(UInt16) Then
            Dim d As UInt16 = 0
            If Not cell.hex OrElse data.ToString.Length < 3 Then
                d = CType(data, UInt16)
            Else
                d = Convert.ToUInt16(data.substring(2), 16)
            End If
            writer.Write(d)
        ElseIf cell.dataType Is GetType(Int32) Then
            Dim d As Int32 = 0
            If Not cell.hex OrElse data.ToString.Length < 3 Then
                d = CType(data, Int32)
            Else
                d = Convert.ToInt32(data.substring(2), 16)
            End If
            writer.Write(d)
        ElseIf cell.dataType Is GetType(UInt32) Then
            Dim d As UInt32 = 0
            If Not cell.hex OrElse data.ToString.Length < 3 Then
                d = CType(data, UInt32)
            Else
                d = Convert.ToUInt32(data.substring(2), 16)
            End If
            writer.Write(d)
        ElseIf cell.dataType Is GetType(Char) Then
            Dim d As Char = CType(data, Char)
            writer.Write(d)
        ElseIf cell.dataType Is GetType(Single) Then
            Dim d As Single = CType(data, Single)
            writer.Write(d)
        ElseIf cell.dataType Is GetType(Char()) Then
            Dim d(cell.len - 1) As Byte
            If Not data Is Nothing Then
                Dim s As String = CType(data, String)
                For i As Integer = 0 To s.Length - 1
                    d(i) = Asc(s(i))
                Next
                If s.Length > 0 AndAlso s.Length < d.Length Then d(s.Length) = cell.endTag
                writer.Write(d)
            End If
        End If
    End Sub
    Private Shared Function addHextag(ByVal obj As Object, ByRef cell As SGsfDataCell) As Object
        If cell.hex AndAlso Val(obj) > 0 Then
            Return "0x" & Hex(obj).ToUpper
        Else
            Return obj
        End If
    End Function
    Protected Class CDataCellColComparer
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As SGsfDataCell = CType(x, SGsfDataCell)
            Dim ly As SGsfDataCell = CType(y, SGsfDataCell)
            Return IIf(lX.col > ly.col, 1, IIf(lX.col < ly.col, -1, 0))
        End Function
    End Class
    Public Class GsfVersionLoopOutOfRange
        Inherits System.Exception
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
    End Class
    Private Shared Function redirectCells(ByRef struct As SGsfDataStructure, ByRef cells As SParamElem(), Optional ByVal data2structDir As Boolean = True, Optional ByVal rowID As Integer = 0) As SParamElem()
        '1  0   2   3   200 1   true    1   2   3   200 1
        '0  0   1   7   100 0    ==>    1   7   100 0
        '2  0   3   9   200 1   false   2   3   9   100 1
        '0  0   1   4   150 2   <==     1   4   150 2
        If struct.redirections.Length < 1 Then Return cells
        Dim redirTable As SGsfRedirTable = Nothing
        Dim out As New ArrayList 'SParamElem() = {}
        Dim tableFound As Boolean = False
        If data2structDir Then
            Dim toComp As Object() = {}
            For i As Integer = 0 To struct.cells.Length - 1
                If struct.cells(i).redirCase > 0 Then
                    Dim c As Integer = i
                    If struct.hasColNums Then c = struct.cells(i).col - 1
                    ReDim Preserve toComp(UBound(toComp) + 1)
                    toComp(UBound(toComp)) = cells(c).value
                End If
            Next
            For Each tab As SGsfRedirTable In struct.redirections
                For Each e As SGsfRedirTableTestItem In tab.values
                    Dim vs As Integer = 0
                    Dim match As Integer = 0
                    For Each v As Object In e.values
                        If v = "*" OrElse v = toComp(vs) Then match += 1
                        vs += 1
                    Next
                    If match = e.values.Length Then
                        redirTable = tab
                        tableFound = True
                        Exit For
                    End If
                Next
            Next
            If tableFound Then
                Dim col As Integer = 1
                Dim len As Integer = 0
                For k As Integer = 0 To cells.Length - 1
                    out.Add(Nothing)
                Next
                For Each v As SParamElem In cells
                    Dim redE As SGsfRedirElem = Nothing
                    Dim redEfound As Boolean = False
                    For Each e As SGsfRedirElem In redirTable.elems
                        If e.fromCol = col Then
                            redE = e
                            redEfound = True
                            Exit For
                        End If
                    Next
                    If redEfound Then
                        out(redE.toCol - 1) = v
                        len += 1
                    End If
                    col += 1
                Next
                Dim nA As New ArrayList
                For k As Integer = 0 To out.Count - 1
                    If Not out(k) Is Nothing Then nA.Add(out(k))
                Next
                out = nA
            Else
                'Debug.WriteLine("GSF redirect table not found on loading for: " & String.Join(" - ", AddMath.ObjArrToStr(toComp)))
                Return cells
            End If
        Else
            Dim toComp As Object() = {}
            Dim numToComp As Integer = 0
            If cells.Length < struct.cells.Length Then 'if the col count is same as the main struct it doesnt use a redir template
                For i As Integer = 0 To struct.cells.Length - 1
                    If struct.cells(i).redirCase > 0 Then
                        numToComp += 1
                    End If
                Next
                For i As Integer = cells.Length - numToComp To cells.Length - 1
                    ReDim Preserve toComp(UBound(toComp) + 1)
                    toComp(UBound(toComp)) = cells(i).value
                Next
                For Each tab As SGsfRedirTable In struct.redirections
                    For Each e As SGsfRedirTableTestItem In tab.values
                        Dim vs As Integer = 0
                        Dim match As Integer = 0
                        For Each v As Object In e.values
                            If v = "*" OrElse v = toComp(vs) Then match += 1
                            vs += 1
                        Next
                        If match = e.values.Length Then
                            redirTable = tab
                            tableFound = True
                            Exit For
                        End If
                    Next
                Next
                If tableFound Then
                    Dim col As Integer = 1
                    Dim len As Integer = 0
                    Dim sCells(struct.cells.Length - 1) As SGsfDataCell
                    Array.Copy(struct.cells, sCells, struct.cells.Length)
                    Array.Sort(sCells, New CDataCellColComparer)
                    For k As Integer = 0 To sCells.Length - 1
                        If Not sCells(k).hide Then
                            Dim obj As Object = sCells(k).value
                            If sCells(k).replace > 0 Then
                                For Each rep As SGsfReplacementTable In struct.replacements
                                    If rep.id = sCells(k).replace Then
                                        For Each repE As SGsfReplacementElem In rep.elems
                                            If repE.fromItem = obj Then
                                                obj = repE.toItem
                                                Exit For
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                            out.Add(CreateParamElem(DataType.EInteger, obj))
                        End If
                    Next
                    For Each v As SParamElem In cells
                        Dim redE As SGsfRedirElem = Nothing
                        Dim redEfound As Boolean = False
                        For Each e As SGsfRedirElem In redirTable.elems
                            If e.toCol = col Then
                                redE = e
                                redEfound = True
                                Exit For
                            End If
                        Next
                        If redEfound Then
                            out(redE.fromCol - 1) = v
                            len += 1
                        End If
                        col += 1
                    Next
                Else
                    Debug.WriteLine("GSF redirect table not found on saving for: " & String.Join(" - ", AddMath.ObjArrToStr(toComp)))
                    Return cells
                End If
            Else
                Return cells
            End If
        End If
        Return out.ToArray(GetType(SParamElem))
    End Function
End Class

Public Class CMcfBase
    Protected decScript As Byte() = {}
    Protected iScriptLines As SScriptLine() = {}
    Protected iFunctions As SFunction() = {}
    Protected Shared scriptSectionStartTag As Byte() = {&H55, &H89, &HE5}
    Protected Shared scriptSectionStopTag As Byte() = {&H89, &HEC, &H5D, &HC3}
    Protected Const scriptSectionSpaceTag As Byte = &H90
    Protected Const scriptAreaSplitTag As Byte = &HCC
    Protected Const scriptSpaceMultiplier As Integer = 3
    Protected RYLVersion As Integer = 0
    Protected FileType As EFileType = EFileType.EUnknown

    Public Enum EFileType
        EUnknown = 0
        ENpcScript = 1
        EQuest = 2
        EScript = 3
    End Enum
    Public Shared SFileType As String() = {"Unknown", "NPC Script", "Quest", "Script"}
    Public Structure SFunction
        Dim id As Integer
        Dim sumCode As Integer
        Dim parameterCount As Integer
        Dim name As String
        Dim index As UInt32 'used only for sorting to get parameter counts
        Dim parameterTypes() As DataType
        Dim returnType As DataType
        Dim showInHeader As Boolean
        Public Overrides Function ToString() As String
            Return "[" & id & "] " & name & ", sum:" & AddMath.Hex2(sumCode) & ", return:" & [Enum].GetName(GetType(DataType), returnType)
        End Function
    End Structure
    Protected Structure SOffStr
        Dim off As Long
        Dim str As String
    End Structure
    Public Structure SScriptLine
        Dim func As SFunction
        Dim func2 As SFunction
        Dim parameters() As SParamElem
        Dim isPointer As Boolean
    End Structure
    Public Enum DataType
        ENull = 0 'e.g. Void
        EBool = 1
        EInteger = 2
        EFloat = 3
        EString = 4
    End Enum
    Public Structure SParamElem
        Dim value As Object
        Dim type As DataType
    End Structure
    Public Shared DataTypeString As String() = {"null", "bool", "int", "float", "string"}

    Public ReadOnly Property ScriptLines() As SScriptLine()
        Get
            Return iScriptLines
        End Get
    End Property
    Public ReadOnly Property Functions() As SFunction()
        Get
            Return iFunctions
        End Get
    End Property
    Public ReadOnly Property Data() As Byte()
        Get
            Return decScript
        End Get
    End Property
    Protected Class CFunctionSorter
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As SFunction = CType(x, SFunction)
            Dim lY As SFunction = CType(y, SFunction)
            Return New CaseInsensitiveComparer().Compare(lX.index, lY.index)
        End Function
    End Class
    Protected Class CFunctionSorterByName
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As SFunction = CType(x, SFunction)
            Dim lY As SFunction = CType(y, SFunction)
            Return New CaseInsensitiveComparer().Compare(lX.name, lY.name)
        End Function
    End Class
    Public Shared Function CreateParamElem(ByVal type As DataType, ByVal value As Object) As SParamElem
        Dim a As New SParamElem
        a.type = type
        a.value = value
        Return a
    End Function
    Protected Sub lookForFileType()
        Dim Ryl2NpcScriptFunctions As String() = {"AddWords", "SetPosition", "SetNPC", "AddDialog", "AddItem", "AddZoneMove", "AddQuest", "SetDropGrade", "SetDropBase", "AddQuestWords", "AddPopup", "SetNPCAttribute"}
        Dim Ryl2QuestFunctions As String() = {"QuestEnd", "QuestSkillPointBonus", "QuestStart", "QuestType", "QuestArea", "QuestTitle", "QuestDesc", "QuestShortDesc", "QuestIcon", "QuestCompleteSave", "QuestLevel", "QuestAward", "AddPhase", "Phase_Target", "Trigger_Start", "Trigger_Puton", "Trigger_Geton", "Trigger_Talk", "Trigger_Kill", "Trigger_Pick", "Trigger_Fame", "Trigger_LevelTalk", "Else", "Event_Disappear", "Event_Get", "Event_Spawn", "Event_MonsterDrop", "Event_Award", "Event_MsgBox", "Event_Phase", "Event_End", "Event_AwardItem", "Event_AddQuest", "Event_Move", "Event_TheaterMode"}
        Dim Ryl1NpcScriptFunctions As String() = {"AddWords", "SetPosition", "SetNPC", "AddDialog", "AddItem", "AddSkillBook", "AddZoneMove", "AddQuest"}
        Dim Ryl2ScriptFunctions As String() = {"AddString", "AddString", "RylNation"}
        Dim Ryl1QuestFunction As String() = {"QuestTitle", "QuestDesc", "QuestShortDesc", "QuestIcon", "QuestCompleteSave", "QuestLevel", "QuestAward", "AddPhase", "Phase_Target", "Trigger_Start", "Trigger_Puton", "Trigger_Geton", "Trigger_Talk", "Trigger_Kill", "Trigger_Pick", "Else", "Event_Disappear", "Event_Get", "Event_Spawn", "Event_MonsterDrop", "Event_Award", "Event_MsgBox", "Event_Phase", "Event_End"}

        Dim r2n% = 0, r2q% = 0, r1n% = 0, r2s% = 0, r1q% = 0
        Dim questStartFound As Boolean = False
        For Each f As SFunction In iFunctions
            If Array.IndexOf(Ryl2NpcScriptFunctions, f.name) >= 0 Then r2n += 1
            If Array.IndexOf(Ryl2QuestFunctions, f.name) >= 0 Then r2q += 1
            If Array.IndexOf(Ryl1NpcScriptFunctions, f.name) >= 0 Then r1n += 1
            If Array.IndexOf(Ryl2ScriptFunctions, f.name) >= 0 Then r2s += 1
            If Array.IndexOf(Ryl1QuestFunction, f.name) >= 0 Then r1q += 1
            If f.name = "QuestStart" Then questStartFound = True
        Next
        If r2n = Ryl2NpcScriptFunctions.Length Then
            RYLVersion = 2
            FileType = EFileType.ENpcScript
        ElseIf r2q = Ryl2QuestFunctions.Length Then
            RYLVersion = 2
            FileType = EFileType.EQuest
        ElseIf r1n = Ryl1NpcScriptFunctions.Length Then
            RYLVersion = 1
            FileType = EFileType.ENpcScript
        ElseIf r2s = Ryl2ScriptFunctions.Length Then
            RYLVersion = 2
            FileType = EFileType.EScript
        ElseIf r1q = Ryl1QuestFunction.Length AndAlso questStartFound = False Then
            RYLVersion = 1
            FileType = EFileType.EQuest
        Else
            RYLVersion = 0
            FileType = EFileType.EUnknown
        End If
    End Sub
End Class

Public Class CMcfDecompiler
    Inherits CMcfBase
    Private pointers As Long() = {}
    Private traceStart&, traceLength&, textStart&, textLength&, scriptStart&, scriptLength&, functionsStart&, functionsLength&
    Private Sub setMainOffsets()
        traceStart& = 3 * 4
        traceLength = AddMath.getUInt32(4, decScript) * 4

        textStart = traceStart + traceLength + 12
        textLength = AddMath.getUInt32(traceStart + traceLength + 4, decScript)

        scriptStart = textStart + textLength
        scriptLength = AddMath.getUInt32(traceStart + traceLength + 8, decScript)

        functionsStart = scriptStart + scriptLength
        functionsLength = decScript.Length - functionsStart

        ReDim pointers(traceLength / 4 - 1)
        Dim pos As Long = 12
        For i As Long = 0 To traceLength / 4 - 1
            pointers(i) = AddMath.getUInt32(pos, decScript)
            pos += 4
        Next
    End Sub
    Private Function getFunctions() As SFunction()
        Dim functionCount As Integer = AddMath.getUInt32(functionsStart, decScript)
        Dim pos As Long = functionsStart + 4
        Dim functions(functionCount - 1) As SFunction
        For i As Integer = 1 To functionCount
            Dim fName As String = ""
            Do While decScript(pos) <> &HA
                fName &= Chr(decScript(pos))
                pos += 1
            Loop
            pos += 1
            Dim traceC As UInt32 = AddMath.getUInt32(pos, decScript)
            pos += 4
            Dim index As UInt32 = AddMath.getUInt32(pos, decScript)
            pos += 4
            Dim func As New SFunction
            func.name = fName
            func.parameterTypes = New DataType() {}
            ' 0xABBB BBBB <- up to 7 parameter types and the first one is return value, 0 for void
            Dim paraListH As Char() = Hex(traceC).ToCharArray
            If paraListH.Length = 8 Then
                func.returnType = Val(paraListH(0))
                paraListH = Hex(traceC - Val(paraListH(0)) * &H10000000).ToCharArray
            Else
                func.returnType = DataType.ENull
            End If
            If paraListH.Length > 0 AndAlso Not (paraListH.Length = 1 AndAlso paraListH(0) = "0") Then
                Array.Reverse(paraListH)
                For Each c As Char In paraListH
                    ReDim Preserve func.parameterTypes(UBound(func.parameterTypes) + 1)
                    func.parameterTypes(UBound(func.parameterTypes)) = Val(c)
                Next
            End If
            func.index = index
            functions(i - 1) = func
        Next
        Array.Sort(functions, New CFunctionSorter)
        For i As Integer = 0 To functionCount - 1
            functions(i).parameterCount = functions(i).parameterTypes.Length
            functions(i).sumCode = scriptLength + (&HC + functions(i).parameterCount * 6 - 1) - functions(i).index
            functions(i).showInHeader = (functions(i).returnType = DataType.ENull)
            functions(i).id = i
            'functions(i).index = 0
            Debug.WriteLine(functions(i).ToString() & " :: 0x" & AddMath.Hex2(functions(i).index))
        Next
        Return functions
    End Function
    Private Function getScriptLines(ByVal functions() As SFunction) As SScriptLine()
        Dim txt() As SOffStr = getTexts()
        Dim dataEndTag As Byte() = scriptSectionStopTag
        'AddMath.addBytesToEnd(dataEndTag, AddMath.MultiplyBytes(scriptAreaSplitTag, scriptSpaceMultiplier))

        Dim dataStartTag As Byte() = {} 'AddMath.MultiplyBytes(scriptSectionSpaceTag, scriptSpaceMultiplier)
        AddMath.addBytesToEnd(dataStartTag, scriptSectionStartTag)
        AddMath.addBytesToEnd(dataStartTag, New Byte() {&HB9})
        Dim dataStartArr() As Long = AddMath.position(decScript, dataStartTag, scriptStart, -1, scriptLength)
        Dim dataStart As Long = AddMath.position(decScript, dataStartTag, scriptStart, 1, scriptLength)(0) + dataStartTag.Length - 1
        Dim dataEndArr() As Long = AddMath.position(decScript, dataEndTag, dataStart, -1, scriptLength)
        Dim dataEnd As Long = dataEndArr(UBound(dataEndArr))
        Dim pos As Long = dataEnd - 4 'move to first uint32
        Dim out As New ArrayList
        Dim tmpL As New SScriptLine
        Dim tmpParC As Integer = 0
        Dim endOff As Long = 0
        Dim tmpIndex As UInt32 = 0
        Dim tmpU As UInt32 = 0
        Dim pause As Boolean = False
        Dim mainFunctionStartOffsets As New List(Of Long)()
        mainFunctionStartOffsets.AddRange(dataStartArr)
        Dim mainFunctionEndOffsets As New List(Of Long)()
        mainFunctionEndOffsets.AddRange(dataEndArr)
        Dim mainFunctionPosCorrection As Integer = 0
        Dim ongoingFunction As Integer = IIf(functions(UBound(functions)).showInHeader = False, UBound(functions), -1)
        Do While pos >= dataStart - 4
            pos -= 1
            If mainFunctionStartOffsets.Contains(pos + 2) AndAlso mainFunctionStartOffsets.IndexOf(pos + 2) > 0 Then
                pause = True
            ElseIf pause AndAlso mainFunctionEndOffsets.Contains(pos + 4) Then
                pause = False
                mainFunctionPosCorrection = scriptSectionStartTag.Length + scriptSectionStopTag.Length
                If ongoingFunction >= 0 Then ongoingFunction -= 1
            ElseIf Not pause Then
                tmpU = AddMath.getUInt32(pos + 1, decScript)
                Dim f1 As Byte = decScript(pos)
                Dim f2 As Byte = decScript(pos - 1)
                If (f1 = &HC4 AndAlso f2 = &H81) OrElse pos = dataStart - 5 Then
                    If tmpIndex > 0 Then
                        Dim startOff As Long = pos + 1 + 4 + mainFunctionPosCorrection
                        mainFunctionPosCorrection = 0
                        Dim len As Long = endOff - startOff
                        Dim offEnd As Long = ((scriptLength + scriptStart) - startOff)
                        Dim code As UInt32 = 0
                        If offEnd > tmpIndex Then
                            code = offEnd - tmpIndex + 6 'magic 6 <- secret
                        Else
                            Dim tmpCode As UInt64 = &H100000000
                            tmpCode = tmpCode - tmpIndex + offEnd + 6
                            code = tmpCode
                        End If
                        For Each f As SFunction In functions
                            If f.sumCode = code AndAlso tmpParC = f.parameterCount Then tmpL.func = f
                        Next
                        'Debug.WriteLine("Found line [" & AddMath.Hex2(offEnd) & "]: " & tmpL.func.ToString())
                        Array.Reverse(tmpL.parameters)
                        Dim fParams As DataType() = tmpL.func.parameterTypes
                        For pI As Integer = 0 To tmpL.parameters.Length - 1
                            tmpL.parameters(pI).type = fParams(pI)
                            Select Case fParams(pI)
                                Case DataType.EFloat
                                    tmpL.parameters(pI).value = AddMath.DecToSingle(tmpL.parameters(pI).value)
                                Case DataType.EString
                                    For Each txtE As SOffStr In txt
                                        If txtE.off = tmpL.parameters(pI).value Then
                                            tmpL.parameters(pI).value = txtE.str
                                            Exit For
                                        End If
                                    Next
                            End Select
                        Next
                        out.Add(tmpL)
                    End If
                    tmpL = New SScriptLine
                    tmpL.parameters = New SParamElem() {}
                    If ongoingFunction >= 0 Then tmpL.func2 = functions(ongoingFunction)
                    tmpParC = tmpU / 4
                    endOff = pos + 1 + 4
                    pos -= 5
                ElseIf f1 = &HE8 Then
                    tmpIndex = tmpU
                    pos -= 4
                ElseIf f1 = &HB9 Then
                    ReDim Preserve tmpL.parameters(UBound(tmpL.parameters) + 1)
                    tmpL.parameters(UBound(tmpL.parameters)) = New SParamElem
                    tmpL.parameters(UBound(tmpL.parameters)).value = tmpU
                    pos -= 4
                End If
                If f2 = &H51 Then
                    pos -= 1
                End If
            End If
        Loop
        out.Reverse()
        Return out.ToArray(GetType(SScriptLine))
    End Function
    Private Function getTexts() As SOffStr()
        Dim out() As SOffStr = {}
        Dim tmpStr As String = ""
        Dim tmpOff As Long = 0
        For i As Long = 0 To textLength - 1
            Dim c As Byte = decScript(textStart + i)
            If c = &H0 Then
                Dim struc As New SOffStr
                struc.off = tmpOff
                struc.str = tmpStr
                tmpStr = ""
                tmpOff = i + 1
                ReDim Preserve out(UBound(out) + 1)
                out(UBound(out)) = struc
            Else
                tmpStr &= Chr(c)
            End If
        Next
        Return out
    End Function
    Public Sub Decompile(ByVal decData() As Byte)
        decScript = decData
        setMainOffsets()
        iFunctions = getFunctions()
        iScriptLines = getScriptLines(iFunctions)
    End Sub
    Public ReadOnly Property RYLFileVersion() As Integer
        Get
            If RYLVersion < 1 Then lookForFileType()
            Return RYLVersion
        End Get
    End Property
    Public ReadOnly Property RYLFileType() As EFileType
        Get
            If RYLVersion < 1 Then lookForFileType()
            Return FileType
        End Get
    End Property
End Class

Public Class CMcfCompiler
    Inherits CMcfBase
    Private pointers As Long() = {}
    Private headerLength&, textLength&, scriptLength&, functionsLength&
    Private stringsToAdd As SOffStr() = {}

    Public Sub Compile(ByVal ScriptLines As SScriptLine(), ByVal Functions As SFunction())
        iScriptLines = ScriptLines
        iFunctions = Functions
        decScript = New Byte() {}
        Dim txt As Byte() = createTextArea()
        textLength = txt.Length
        Dim data As Byte() = createDataArea()
        scriptLength = data.Length
        Dim funcS As Byte() = createFunctionArea()
        functionsLength = funcS.Length
        Dim h As Byte() = createHeader()
        headerLength = h.Length
        Dim modulus As Integer = CMcfCoder.ModulusFromDivination(headerLength + textLength + scriptLength + functionsLength, 4)
        Dim j As Integer = 0
        For i As Integer = functionsLength - modulus To functionsLength - 1
            funcS(i) = CMcfCoder.EnCryptByte(&H0, CType(j, CMcfCoder.Col))
            j += 1
        Next
        AddMath.addBytesToEnd(decScript, h)
        AddMath.addBytesToEnd(decScript, txt)
        AddMath.addBytesToEnd(decScript, data)
        AddMath.addBytesToEnd(decScript, funcS)
    End Sub

    Private Function createHeader() As Byte()
        'Dim pointers() As UInt32 = {}
        Dim outB(3 * 4 + pointers.Length * 4 + 3 * 4 - 1) As Byte

        AddMath.SetUInt32inBytes(outB, pointers.Length, 4)

        Dim pos As Long = 3 * 4
        For Each pntr As UInt32 In pointers
            AddMath.SetUInt32inBytes(outB, pntr, pos)
            pos += 4
        Next
        outB(pos) = &H1C 'dunno
        AddMath.SetUInt32inBytes(outB, textLength, pos + 4) 'text area size
        AddMath.SetUInt32inBytes(outB, scriptLength, pos + 8) 'script area size
        Return outB
    End Function
    Private Function createTextArea() As Byte() 'and parse the scriptlines parameters
        Dim out As Byte() = {}
        Dim sLC As Long = 0
        For Each sL As SScriptLine In iScriptLines
            Dim parC As Long = 0
            For Each par As SParamElem In sL.parameters
                If par.type = DataType.EString Then
                    Dim str As String = par.value
                    Dim off As Long = getStrOff(str, stringsToAdd)
                    If off < 0 Then
                        off = out.Length
                        ReDim Preserve stringsToAdd(UBound(stringsToAdd) + 1)
                        stringsToAdd(UBound(stringsToAdd)) = New SOffStr
                        stringsToAdd(UBound(stringsToAdd)).off = off
                        stringsToAdd(UBound(stringsToAdd)).str = str

                        ReDim Preserve out(off + str.Length)
                        Dim i As Long = 0
                        For Each c As Char In str
                            out(off + i) = Asc(c)
                            i += 1
                        Next
                        out(off + i) = &H0
                    End If
                    'iScriptLines(sLC).parameters(parC).value = off
                ElseIf par.type = DataType.EFloat Then
                    iScriptLines(sLC).parameters(parC).value = AddMath.SingleToDec(par.value)
                End If
                parC += 1
            Next
            sLC += 1
        Next
        Return out
    End Function
    Private Function createDataArea() As Byte()
        Dim txtPointers As New ArrayList
        Dim endingSize As Long = scriptSectionStopTag.Length + scriptSpaceMultiplier
        For Each f As SFunction In iFunctions
            endingSize += f.parameterCount * scriptSpaceMultiplier + 5 + scriptSectionStartTag.Length + scriptSectionStopTag.Length
        Next
        Dim lines As SScriptLine() = iScriptLines
        Array.Reverse(lines)
        Dim byteLines As New ArrayList
        Dim offset As Long = endingSize
        For lIndex As Long = 0 To lines.Length - 1
            Dim line As SScriptLine = lines(lIndex)
            Dim byteLine(line.parameters.Length * 6 + 11 - 1) As Byte
            Dim pos As Long = 0
            offset += byteLine.Length
            For Each p As SParamElem In line.parameters
                If p.type = DataType.EString Then
                    p.value = getStrOff(p.value, stringsToAdd)
                End If
                byteLine(pos) = &HB9
                AddMath.SetUInt32inBytes(byteLine, p.value, pos + 1)
                If p.type = DataType.EString Then txtPointers.Add(offset - pos - 1)
                byteLine(pos + 5) = &H51
                pos += 6
            Next
            byteLine(pos) = &HE8
            AddMath.SetUInt32inBytes(byteLine, (offset - line.func.sumCode + 6), pos + 1) 'index  'magic 6 <- secret
            byteLine(pos + 5) = &H81
            byteLine(pos + 6) = &HC4
            AddMath.SetUInt32inBytes(byteLine, line.parameters.Length * 4, pos + 7) 'param count
            byteLines.Add(byteLine)
        Next
        byteLines.Reverse()
        offset += scriptSectionStartTag.Length + scriptSpaceMultiplier
        ' we know the right size now. Lets make the function indexes, thereafter sort and parameter list
        For i As Integer = 0 To iFunctions.Length - 1
            iFunctions(i).index = offset + (&HC + iFunctions(i).parameterCount * 6 - 1) - iFunctions(i).sumCode
        Next
        Array.Sort(iFunctions, New CFunctionSorter)
        'txt pointers re'calculation
        Dim k As Long = 0
        ReDim pointers(txtPointers.Count - 1)
        For Each p As Long In txtPointers
            pointers(k) = offset - p
            k += 1
        Next
        Array.Sort(pointers)
        txtPointers = Nothing

        Dim out(offset - 1) As Byte
        offset = 0
        For i As Integer = 0 To scriptSpaceMultiplier
            out(i + offset) = scriptSectionSpaceTag
        Next
        offset += scriptSpaceMultiplier
        For i As Integer = 0 To scriptSectionStartTag.Length - 1
            out(i + offset) = scriptSectionStartTag(i)
        Next
        offset += scriptSectionStartTag.Length
        For lIndex As Long = 0 To byteLines.Count - 1 ' copy
            Dim lB As Byte() = byteLines(lIndex)
            For Each b As Byte In lB
                out(offset) = b
                offset += 1
            Next
        Next
        For i As Integer = 0 To scriptSectionStopTag.Length - 1
            out(i + offset) = scriptSectionStopTag(i)
        Next
        offset += scriptSectionStopTag.Length
        For i As Integer = 0 To scriptSpaceMultiplier ' area split
            out(i + offset) = scriptAreaSplitTag
        Next
        offset += scriptSpaceMultiplier
        For Each f As SFunction In iFunctions
            For i As Integer = 0 To scriptSectionStartTag.Length - 1
                out(i + offset) = scriptSectionStartTag(i)
            Next
            offset += scriptSectionStartTag.Length
            For i As Integer = 1 To 5 + f.parameterCount * scriptSpaceMultiplier
                out(offset) = scriptSectionSpaceTag
                offset += 1
            Next
            For i As Integer = 0 To scriptSectionStopTag.Length - 1
                out(i + offset) = scriptSectionStopTag(i)
            Next
            offset += scriptSectionStopTag.Length
        Next
        Return out
    End Function
    Private Function createFunctionArea() As Byte()
        Dim fS As New ArrayList
        Dim offSet As Long = 0
        Array.Sort(iFunctions, New CFunctionSorterByName)
        For Each f As SFunction In iFunctions
            Dim line(f.name.Length + 1 + 2 * 4 - 1) As Byte
            Dim lOff As Long = 0
            For Each c As Char In f.name
                line(lOff) = Asc(c)
                lOff += 1
            Next
            line(lOff) = &HA
            Dim paraListH(f.parameterCount - 1) As String
            Dim i As Integer = 0
            For Each p As DataType In f.parameterTypes
                paraListH(i) = CType(CType(p, Integer), String)
                i += 1
            Next
            Dim par As UInt32 = 0
            If paraListH.Length > 0 Then
                Array.Reverse(paraListH)
                par = Convert.ToUInt32(String.Join("", paraListH), 16)
            End If
            AddMath.SetUInt32inBytes(line, par, lOff + 1)
            lOff += 5
            AddMath.SetUInt32inBytes(line, f.index, lOff)
            lOff += 4
            offSet += lOff
            fS.Add(line)
        Next
        Dim out(offSet + 4 + 16 - 1) As Byte
        AddMath.SetUInt32inBytes(out, iFunctions.Length, 0)
        offSet = 4
        For Each l As Byte() In fS
            For Each b As Byte In l
                out(offSet) = b
                offSet += 1
            Next
        Next
        Dim ending As Byte() = {&H4, &H0, &H0, &H0, &H10, &H0, &H0, &H0, &H14, &H0, &H0, &H0, &H18, &H0, &H0, &H0}
        For Each e As Byte In ending
            out(offSet) = e
            offSet += 1
        Next
        Return out
    End Function

    Private Function getStrOff(ByVal str As String, ByRef OffStrArr As SOffStr()) As Long
        For Each pr As SOffStr In OffStrArr
            If pr.str = str Then
                Return pr.off
            End If
        Next
        Return -1
    End Function
End Class

Public Class AddMath
    Private Shared badChars As String() = {vbNewLine, vbCr, vbLf}
    Public Shared Function TrimCrLf(ByVal txt As String) As String
        For Each t As String In badChars
            txt.Replace(t, "")
        Next
        Return Trim(txt)
    End Function
    Public Shared Function ObjArrToStr(ByRef objA As Object()) As String()
        Dim o(objA.Length - 1) As String
        Dim i As Integer = 0
        For Each ob As Object In objA
            o(i) = ob.ToString
            i += 1
        Next
        Return o
    End Function
    Public Shared Function resolveDataType(ByVal type As String) As Type
        If Left(type, 5) = "char[" Then Return GetType(Char())
        Select Case type
            Case "byte" : Return GetType(Byte)
            Case "i16" : Return GetType(Int16)
            Case "ui16" : Return GetType(UInt16)
            Case "i32" : Return GetType(Int32)
            Case "ui32" : Return GetType(UInt32)
            Case "char" : Return GetType(Char)
            Case "float" : Return GetType(Single)
            Case Else
                Throw New Exception("Unsupported data type")
        End Select
    End Function
    Public Shared Function resolveDataTypeLen(ByVal type As String) As Integer
        If Left(type, 5) = "char[" Then Return resolveInteger(type.Substring(5, type.Length - 6))
        Select Case type
            Case "byte" : Return 1
            Case "i16" : Return 2
            Case "ui16" : Return 2
            Case "i32" : Return 4
            Case "ui32" : Return 4
            Case "char" : Return 1
            Case "float" : Return 4
            Case Else
                Throw New Exception("Unsupported data type")
        End Select
    End Function
    Public Shared Function resolveInteger(ByVal num As String) As Integer
        If num.Length > 2 AndAlso num.Substring(0, 2) = "0x" Then
            Return Integer.Parse(num.Substring(2), Globalization.NumberStyles.HexNumber)
        ElseIf num = "" Then
            Return 0
        Else
            Return Integer.Parse(num)
        End If
    End Function
    Public Shared Function getUInt32(ByVal pos As Long, ByRef arr As Byte()) As UInt32
        Dim out As UInt32
        For i As Integer = 3 To 0 Step -1
            Dim t As Byte = arr(i + pos)
            out += t * Math.Pow(&H100, i)
        Next
        Return out
    End Function
    Public Shared Function UInt32toBytes(ByVal val As UInt32) As Byte()
        Dim out(3) As Byte
        Dim str As String = Hex(val)
        Dim firstB As String = ""
        If CMcfCoder.ModulusFromDivination(str.Length, 2) > 0 Then
            firstB = str.Substring(0, 1)
            str = str.Substring(1)
        End If
        Dim poss As Integer = str.Length - 2
        Dim ks As Integer = 0
        Do While poss >= 0
            Dim k As String = ""
            k = str.Substring(poss, 2)
            Dim b As Byte = Convert.ToByte(k, 16)
            out(ks) = b
            ks += 1
            poss -= 2
        Loop
        If firstB <> "" Then out(ks) = Convert.ToByte(firstB, 16)
        Return out
    End Function
    Public Shared Sub SetUInt32inBytes(ByRef data As Byte(), ByVal val As UInt32, ByVal pos As Long)
        Dim str As String = Hex(val)
        Dim firstB As String = ""
        If CMcfCoder.ModulusFromDivination(str.Length, 2) > 0 Then
            firstB = str.Substring(0, 1)
            str = str.Substring(1)
        End If
        Dim poss As Integer = str.Length - 2
        Dim ks As Integer = 0
        Do While poss >= 0
            Dim k As String = ""
            k = str.Substring(poss, 2)
            Dim b As Byte = Convert.ToByte(k, 16)
            data(ks + pos) = b
            ks += 1
            poss -= 2
        Loop
        If firstB <> "" Then
            data(ks + pos) = Convert.ToByte(firstB, 16)
            ks += 1
        End If
        If ks < 4 Then
            For i As Integer = ks To 3
                data(pos + ks) = &H0
            Next
        End If
    End Sub
    Public Shared Sub addBytesToEnd(ByRef toWhere As Byte(), ByRef whatToAdd As Byte())
        ReDim Preserve toWhere(UBound(toWhere) + whatToAdd.Length)
        Dim k As Integer = 0
        For i As Long = toWhere.Length - whatToAdd.Length To toWhere.Length - 1
            toWhere(i) = whatToAdd(k)
            k += 1
        Next
    End Sub
    Public Shared Sub debugByteArr(ByRef arr As Byte())
        Dim i As Integer = 0
        Dim col As Integer = 0
        Debug.WriteLine("--- Start of dump ---")
        For Each bb As Byte In arr
            Debug.Write(Hex2(bb) & " ")
            If i = 3 Then Debug.Write("  ")
            i += 1
            If i > 3 Then
                i = 0
                col += 1
            End If
            If col > 4 Then
                col = 0
                Debug.WriteLine("")
            End If
        Next
        Debug.WriteLine("")
        Debug.WriteLine("--- End of dump ---")
    End Sub
    Public Shared Function Hex2(ByVal nr As Long) As String
        Dim o As String = "" & Hex(nr).ToUpper
        If o.Length < 2 Then o = "0" & o
        Return o
    End Function
    Public Shared Function position(ByRef data As Byte(), ByRef searchFor As Byte(), Optional ByVal startFrom As Long = 0, Optional ByVal numberOfMaxresults As Integer = 0, Optional ByVal Length As Long = 0) As Long()
        Dim poses As Long() = {}
        For index As Long = startFrom To IIf(Length > 0, startFrom + Length, data.Length - 1)
            If data.Length - index >= searchFor.Length Then
                Dim arr As Byte() = {}
                For i As Integer = 0 To searchFor.Length - 1
                    ReDim Preserve arr(i)
                    arr(i) = data(index + i)
                Next
                If compareArr(arr, searchFor) Then
                    ReDim Preserve poses(UBound(poses) + 1)
                    poses(UBound(poses)) = index
                    If numberOfMaxresults > 0 AndAlso poses.Length = numberOfMaxresults Then Exit For
                End If
            Else
                Exit For
            End If
        Next
        Return poses
    End Function
    Public Shared Function compareArr(ByRef d1 As Byte(), ByRef d2 As Byte()) As Boolean
        For i As Integer = 0 To d1.Length - 1
            If UBound(d2) < i OrElse d1(i) <> d2(i) Then Return False
        Next
        Return True
    End Function
    Public Shared Function MultiplyBytes(ByVal data As Byte, ByVal multiplier As Integer) As Byte()
        Dim out(multiplier - 1) As Byte
        For i As Integer = 0 To multiplier - 1
            out(i) = data
        Next
        Return out
    End Function
    Public Shared Function SingleToDec(ByVal pos As Single)
        Return getUInt32(0, BitConverter.GetBytes(pos))
    End Function
    Public Shared Function DecToSingle(ByVal dec As Long)
        Return BitConverter.ToSingle(UInt32toBytes(dec), 0)
    End Function
    Public Shared Function IntArr2StrArr(ByRef iArr As Integer()) As String()
        Dim out As New ArrayList
        For Each i As Integer In iArr
            out.Add(i.ToString)
        Next
        Return out.ToArray(GetType(String))
    End Function
    Public Class Array
        Public Shared Function IndexOfArray(ByRef searchIn As Integer()(), ByRef searchFor As Integer()) As Integer
            Dim ind As Integer = 0
            For Each ob() As Integer In searchIn
                Dim found As Boolean = True
                If searchFor.Length <> ob.Length Then found = False
                If found Then
                    For k As Integer = 0 To ob.Length - 1
                        If ob(k) <> searchFor(k) Then
                            found = False
                            Exit For
                        End If
                    Next
                End If
                If found Then Return ind
                ind += 1
            Next
            Return -1
        End Function
    End Class
    Public Shared Function resizeImage(ByRef img As Bitmap, ByVal sX As Integer, ByVal sY As Integer, ByVal dW As Integer, ByVal dH As Integer, Optional ByVal zoomMultiplier As Single = 1) As Bitmap
        Dim recOrg As New Rectangle(sX, sY, Math.Round(dW / zoomMultiplier), Math.Round(dH / zoomMultiplier))
        Dim bmpOut As New Bitmap(dW, dH, img.PixelFormat)
        Dim grpOut As Graphics = Graphics.FromImage(bmpOut)
        Dim recDes As New Rectangle(0, 0, dW, dH)
        grpOut.DrawImage(img, recDes, recOrg, GraphicsUnit.Pixel)
        Return bmpOut
    End Function
    Public Shared Function getRylFolder() As String
        Try
            Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", False)
            Dim str3 As String
            For Each str3 In key.GetSubKeyNames
                Dim key2 As Microsoft.Win32.RegistryKey = key.OpenSubKey(str3, False)
                Dim str2 As String = ""
                Try
                    str2 = key2.GetValue("DisplayName", "").ToString
                Catch exception1 As Exception
                End Try
                If ((str2.IndexOf("RYL") <> -1) OrElse (str2.IndexOf("R.Y.L") <> -1)) Then
                    Try
                        Dim str4 As String = key2.GetValue("InstallLocation", "").ToString
                        If ((str4.Length <= 1) OrElse ((Convert.ToString(str4.Chars((str4.Length - 1))) <> "/") AndAlso (Convert.ToString(str4.Chars((str4.Length - 1))) <> "\"))) Then
                            Return str4
                        End If
                        Return str4.Substring(0, (str4.Length - 1))
                    Catch exception4 As Exception
                    End Try
                End If
            Next
        Catch exception5 As Exception
        End Try
        Return ""
    End Function
    Public Shared Sub setRylFolder(ByVal folder As String)
        Dim key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", False)
        Dim str3 As String
        For Each str3 In key.GetSubKeyNames
            Dim key2 As Microsoft.Win32.RegistryKey = key.OpenSubKey(str3, False)
            Dim str2 As String = ""
            Try
                str2 = key2.GetValue("DisplayName", "").ToString
            Catch exception1 As Exception
            End Try
            If ((str2.IndexOf("RYL") <> -1) OrElse (str2.IndexOf("R.Y.L") <> -1)) Then
                Dim key3 As Microsoft.Win32.RegistryKey = key.OpenSubKey(str3, True)
                key3.SetValue("InstallLocation", folder)
                Exit Sub
            End If
        Next
        key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", True)
        Dim nKey As Microsoft.Win32.RegistryKey = key.CreateSubKey("R.Y.L")
        nKey.SetValue("DisplayName", "R.Y.L")
        nKey.SetValue("InstallLocation", folder)
    End Sub
    Public Enum FileType
        GsfFile
        McfFile
        SkeyGcmdsFile
        Unknown
    End Enum
    Public Shared Function fileTypeFromName(ByVal file As String) As FileType
        Dim ext As String = IO.Path.GetExtension(file).ToLower()
        Select Case ext
            Case ".gsf" : Return FileType.GsfFile
            Case ".mcf" : Return FileType.McfFile
            Case ".skey" : Return FileType.SkeyGcmdsFile
            Case ".gcmds" : Return FileType.SkeyGcmdsFile
            Case Else : Return FileType.Unknown
        End Select
    End Function
    Public Shared Sub sharedDataOverWrite(ByVal file As String)
        Dim lines As String() = {}
        If Not IO.File.Exists(file) Then Return
        Try
            lines = IO.File.ReadAllLines(file)
        Catch ex As Exception
            Return
        End Try
        For Each line As String In lines
            Try
                line = line.Trim()
                If line <> String.Empty AndAlso Not line.StartsWith("//") Then
                    Dim splice1 As String() = line.Split("=")
                    Dim splice2 As String() = splice1(0).Trim().Split(".")
                    Dim type As String = splice2(0)
                    Dim fileType As String = splice2(1)
                    Dim index As Integer = 0
                    If UBound(splice2) > 1 Then index = splice2(2)
                    Dim data As String = splice1(1).Trim()
                    Select Case fileType
                        Case "mcf"
                            If type = "xor" Then
                                CMcfCoder.xorKey = parseByteArray(data)
                            End If
                        Case "gcmds"
                            If type = "xor" Then
                                CGcmdsCoder.key = parseByteArray(data)
                            End If
                        Case "gsf"
                            If type = "xor" Then
                                CGsfCoder.xorDat(index) = data
                            ElseIf type = "off" Then
                                CGsfCoder.typeCodes(index) = data
                            End If
                        Case "global"
                            If type = "usageNotice" Then
                                If Not data = "1" AndAlso Not data = "0" Then Throw New ArgumentException("Value can be 1 or 0")
                                frmNpcEdit.enableServerNotice = (data = "1")
                            End If
                    End Select
                End If
            Catch ex As Exception
                Dim sw As IO.StreamWriter = IO.File.AppendText(file)
                sw.WriteLine("//" & Date.Now.ToString())
                sw.WriteLine("//" & vbTab & "Line: " & line)
                sw.WriteLine("//" & vbTab & "Exception: " & ex.Message)
                sw.WriteLine("//" & vbTab & "Source: " & ex.StackTrace.Replace(vbNewLine, vbNewLine & "//" & vbTab & vbTab))
                sw.Flush()
                sw.Close()
            End Try
        Next
    End Sub
    Public Shared Function parseByteArray(ByVal line As String) As Byte()
        Dim xorStr As String = line
        Dim slices As String() = xorStr.Trim.Split(" ")
        Dim out(slices.Length - 1) As Byte
        For i As Integer = 0 To slices.Length - 1
            If slices(i).Trim <> "" Then out(i) = Byte.Parse(slices(i), Globalization.NumberStyles.HexNumber)
        Next
        Return out
    End Function
End Class

Public Class CGcmdsCoder

#Region "Data"
    Friend Shared key As Byte() = {&H5A, &H5F, &H61, &H6C, &H6C, &H5F, &H41, &H5F, &H33, &H44}
#End Region

    Public Shared Function Decode(ByRef data As Byte()) As String
        Dim kPos As Integer = 0
        Dim mStr(data.Length - 1) As Char
        For i As Integer = 0 To data.Length - 1
            If kPos > key.Length - 1 Then kPos = 0
            mStr(i) = Chr(data(i) Xor key(kPos))
            kPos += 1
        Next
        Dim txt As New String(mStr, 0, mStr.Length)
        Dim header As String = "" & _
         "///////////////////////////////////////////////////////" & vbNewLine & _
         "//" & vbNewLine & _
         "// Gcmds & skey structure ver. 1.0" & vbNewLine & _
         "//" & vbNewLine & _
         "// Created by rylCoder " & Application.ProductVersion.Substring(0, Application.ProductVersion.Length - 2) & " © 2006 & 2007 AlphA" & vbNewLine & _
         "//" & vbNewLine & _
         "///////////////////////////////////////////////////////" & vbNewLine & vbNewLine
        Return header & txt
    End Function
    Public Shared Function Encode(ByRef lines As String()) As Byte()
        Dim kPos As Integer = 0
        Dim str As String = ""
        For Each l As String In lines
            If l.Length < 2 OrElse l.Substring(0, 2) <> "//" Then
                str &= l & vbNewLine
            End If
        Next
        If str.Length > vbNewLine.Length Then str = str.Substring(0, str.Length - vbNewLine.Length)
        Dim data(str.Length - 1) As Byte
        For i As Integer = 0 To data.Length - 1
            If kPos > key.Length - 1 Then kPos = 0
            data(i) = Asc(str(i)) Xor key(kPos)
            kPos += 1
        Next
        Return data
    End Function
End Class

Public Class Asm
    <System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.Demand, Name:="FullTrust")> _
    Public Shared Function Run(ByVal resource As String, ByVal clas As String, ByVal func As String, ByVal args As Object()) As Object
        Dim a As Assembly = Assembly.Load(CType(New System.Resources.ResourceManager("rylCoder.Resources", Assembly.GetExecutingAssembly()).GetObject(resource), Byte()))
        Dim t As Type = a.GetType(clas)
        Dim clas2 As Object = Activator.CreateInstance(t)
        Return t.InvokeMember(func, BindingFlags.Default Or BindingFlags.InvokeMethod, Nothing, clas2, args)
    End Function
End Class