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

Public Class CNpcParser
    Public NPCs As npcStruct() = {}
    Private notFoundNPCLines As New ArrayList 'LineWithPos() = {}
    Private Functions As CMcfBase.SFunction() = {}
    Public loadedNPC As npcStruct = Nothing
    Public RYLVersion As Integer = 0
    Public Structure NPCline
        Dim NPCID As Long
        Dim Params As CMcfBase.SParamElem()
        Dim Type As knownType
        Dim pos As Long
        Dim ownerFunc As Integer
        Public Enum knownType
            EAddItem = 1
            ESetPosition = 2
            EAddPopup = 3
            EAddWords = 4
            EAddDialog = 5
            ESetNPC = 6
            EAddQuestWords = 7
            EAddZoneMove = 8
            EAddQuest = 9
            ESetDropBase = 10
            ESetDropGrade = 11
            ESetNPCAttribute = 12
        End Enum
        Public Shared TypesStrings As String() = { _
            "", _
            "AddItem", _
            "SetPosition", _
            "AddPopup", _
            "AddWords", _
            "AddDialog", _
            "SetNPC", _
            "AddQuestWords", _
            "AddZoneMove", _
            "AddQuest", _
            "SetDropBase", _
            "SetDropGrade", _
            "SetNPCAttribute" _
        }
    End Structure
    Public Class npcStruct
        Public id As Long
        Public iLines As New ArrayList 'NPCLine
        Public RYLversion As Integer = 0
        Public Sub New()
        End Sub
        Public Sub New(ByVal aId As Long, ByVal version As Integer)
            id = aId
            RYLversion = version
        End Sub
        Public Sub AddLine(ByVal com As NPCline)
            If iLines.Count > 0 Then com.ownerFunc = CType(iLines(0), NPCline).ownerFunc
            iLines.Add(com)
        End Sub
        Public Sub DeleteLine(ByVal line As NPCline)
            iLines.Remove(line)
        End Sub
        Public ReadOnly Property Name() As String
            Get
                For Each l As NPCline In iLines
                    If l.Type = NPCline.knownType.ESetNPC Then
                        If RYLversion = 1 Then
                            Return l.Params(4).value
                        Else
                            Return l.Params(5).value
                        End If

                    End If
                Next
                Return ""
            End Get
        End Property
        Public ReadOnly Property Map() As Long
            Get
                For Each l As NPCline In iLines
                    If l.Type = NPCline.knownType.ESetNPC Then
                        Return l.Params(0).value
                    End If
                Next
                Return "??"
            End Get
        End Property
        Public Property Lines(Optional ByVal type As NPCline.knownType = Nothing) As NPCline()
            Get
                Dim tLines As New ArrayList
                For Each l As NPCline In iLines
                    If type = Nothing OrElse l.Type = type Then
                        tLines.Add(l)
                    End If
                Next
                Return tLines.ToArray(GetType(NPCline))
            End Get
            Set(ByVal value As NPCline())
                Dim tLines As New ArrayList
                For Each l As NPCline In iLines
                    If Not type = Nothing AndAlso l.Type <> type Then
                        tLines.Add(l)
                    End If
                Next
                For Each v As NPCline In value
                    tLines.Add(v)
                Next
                iLines = tLines
            End Set
        End Property
        Default Public Property Item(ByVal index As Integer) As NPCline
            Get
                Return iLines(index)
            End Get
            Set(ByVal value As NPCline)
                iLines(index) = value
            End Set
        End Property
        Public Function indexOfPos(ByVal pos As Long)
            Dim i As Long = 0
            For Each l As NPCline In iLines
                If l.pos = pos Then
                    Return i
                End If
                i += 1
            Next
            Return -1
        End Function
        Public Sub SwitchPositions(ByVal pos1 As Long, ByVal pos2 As Long)
            Dim index1 As Long = indexOfPos(pos1)
            Dim index2 As Long = indexOfPos(pos2)
            If index1 >= 0 AndAlso index2 >= 0 AndAlso index1 <> index2 Then
                Dim item1 As NPCline = iLines(index1)
                Dim item2 As NPCline = iLines(index2)
                item1.pos = pos2
                item2.pos = pos1
                iLines(index1) = item2
                iLines(index2) = item1
            End If
        End Sub
        Public Sub setParameter(ByVal pos As Long, ByVal paramIndex As Integer, ByVal value As Object)
            Dim index As Long = indexOfPos(pos)
            If index >= 0 Then
                Dim item As NPCline = iLines(index)
                item.Params(paramIndex).value = value
                iLines(index) = item
            End If
        End Sub
        Public Sub setPos(ByVal pos As Long, ByVal newPos As Long)
            Dim index As Long = indexOfPos(pos)
            If index >= 0 Then
                Dim item As NPCline = iLines(index)
                item.pos = newPos
                iLines(index) = item
            End If
        End Sub
    End Class
    Private Structure LineWithPos
        Dim line As CMcfBase.SScriptLine
        Dim pos As Long
        Dim ownerFunction As Integer
    End Structure
    Private Function indexOfNPCinLines(ByVal NPCid As Long) As Long
        Dim i As Long = 0
        For Each npc As npcStruct In NPCs
            If npc.id = NPCid Then Return i
            i += 1
        Next
        Return -1
    End Function
    Public Sub Parse(ByVal funcs As CMcfBase.SFunction())
        Array.Resize(NPCs, 0)
        Functions = funcs
        Dim npclines As New ArrayList
        Dim i As Long = 0
        For Each f As CMcfBase.SFunction In funcs
            For Each v As CMcfBase.SScriptLine In f.data
                Dim l As Integer = Array.IndexOf(NPCline.TypesStrings, funcs(v.callTo).name)
                If l >= 0 Then
                    Dim nLine As New NPCline
                    nLine.Type = l
                    nLine.Params = v.parameters
                    nLine.pos = i
                    nLine.ownerFunc = f.id
                    npclines.Add(nLine)

                    If nLine.Type = NPCline.knownType.ESetNPC Then
                        If RYLVersion < 1 Then RYLVersion = IIf(nLine.Params.Length > 5, 2, 1)
                        ReDim Preserve NPCs(UBound(NPCs) + 1)
                        NPCs(UBound(NPCs)) = New npcStruct(nLine.Params(1).value, RYLVersion)
                        nLine.NPCID = nLine.Params(1).value
                        NPCs(UBound(NPCs)).AddLine(nLine)
                    Else
                        Dim npcI As Long = indexOfNPCinLines(nLine.Params(0).value)
                        If npcI >= 0 Then
                            nLine.NPCID = nLine.Params(0).value
                            NPCs(npcI).AddLine(nLine)
                        Else
                            Dim vv As New LineWithPos
                            vv.line = v
                            vv.pos = i
                            vv.ownerFunction = f.id
                            notFoundNPCLines.Add(vv)
                        End If
                    End If
                Else
                    Dim vv As New LineWithPos
                    vv.line = v
                    vv.pos = i
                    vv.ownerFunction = f.id
                    notFoundNPCLines.Add(vv)
                End If
                i += 1
            Next
        Next
    End Sub
    Public Function GetFunctions() As CMcfBase.SFunction()
        Dim lines As New ArrayList 'linewith pos
        Dim ucnt As Long = 0
        For i As Long = 0 To NPCs.Length - 1
            Dim ls As NPCline() = NPCs(i).Lines()
            For Each l As NPCline In ls
                Dim ln As New CMcfBase.SScriptLine
                Dim lp As New LineWithPos
                ln.parameters = l.Params
                For Each f As CMcfBase.SFunction In Functions
                    If f.name = NPCline.TypesStrings(l.Type) Then
                        ln.callTo = f.id
                        Exit For
                    End If
                Next
                lp.ownerFunction = l.ownerFunc
                lp.line = ln
                lp.pos = l.pos
                lines.Add(lp)
                ucnt += 1
            Next
        Next
        For Each l As LineWithPos In notFoundNPCLines
            lines.Add(l)
            ucnt += 1
        Next
        lines.Sort(New linePosComparer)
        Dim ki As Long = 0
        For i As Integer = 0 To Functions.Length - 1
            Functions(i).data = New CMcfBase.SScriptLine() {}
        Next
        For Each l As LineWithPos In lines
            Dim f As CMcfBase.SFunction = Functions(l.ownerFunction)
            ReDim Preserve f.data(UBound(f.data) + 1)
            f.data(UBound(f.data)) = l.line
            Functions(l.ownerFunction) = f
        Next
        Return Functions
    End Function
    Public Class linePosComparer
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As LineWithPos = CType(x, LineWithPos)
            Dim lY As LineWithPos = CType(y, LineWithPos)
            Return New CaseInsensitiveComparer().Compare(lX.pos, lY.pos)
        End Function
    End Class
    Public Class NPClinePosComparer
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As NPCline = CType(x, NPCline)
            Dim lY As NPCline = CType(y, NPCline)
            Return New CaseInsensitiveComparer().Compare(lX.pos, lY.pos)
        End Function
    End Class
    Public Class FreePoslinePosComparer
        Implements IComparer
        Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
           Implements IComparer.Compare
            Dim lX As freePosItem = CType(x, freePosItem)
            Dim lY As freePosItem = CType(y, freePosItem)
            Return New CaseInsensitiveComparer().Compare(lX.pos, lY.pos)
        End Function
    End Class
    Public Structure NPCTextItem
        Dim line As NPCline
        Dim text As String
        Dim paraIndex As Integer
        Dim Tag As Object
    End Structure
    Private Structure freePosItem
        Dim npcL As NPCline
        Dim sLine As CMcfBase.SScriptLine
        Dim pos As Long
        Dim isNPCLine As Boolean
    End Structure
    Public Sub FreePosition(ByVal pos As Long, Optional ByVal length As Integer = 1)
        Dim lines As New ArrayList
        For Each npc As npcStruct In NPCs
            For Each l As NPCline In npc.iLines
                Dim nFPitem As New freePosItem
                nFPitem.pos = l.pos
                nFPitem.isNPCLine = True
                nFPitem.npcL = l
                lines.Add(nFPitem)
            Next
            npc.iLines.Clear()
        Next
        If notFoundNPCLines.Count > 0 Then
            For Each l As LineWithPos In notFoundNPCLines
                Dim nFPitem As New freePosItem
                nFPitem.pos = l.pos
                nFPitem.isNPCLine = False
                nFPitem.sLine = l.line
                lines.Add(nFPitem)
            Next
            notFoundNPCLines.Clear()
        End If
        lines.Sort(New FreePoslinePosComparer)
        Dim nPos As Long = 0
        For Each l As freePosItem In lines
            If nPos = pos Then nPos += length
            If l.isNPCLine Then
                Dim nL As NPCline = l.npcL
                nL.pos = nPos
                NPCs(indexOfNPCinLines(nL.NPCID)).AddLine(nL)
            Else
                Dim nL As New LineWithPos
                nL.line = l.sLine
                nL.pos = nPos
                notFoundNPCLines.Add(nL)
            End If
            nPos += 1
        Next
    End Sub
End Class
