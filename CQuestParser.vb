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

Public Class CQuestParser
    'Private Const CIdLen As Integer = 4 'like 0xF704 => Len("F704")
    Public Quests() As Quest = {}
    Private iFuncs As CMcfBase.SFunction() = Nothing
    'Private UnknownLines As New List(Of QLine) ' we dont use positions here so cant really seperate them
    Public openQuest As Quest = Nothing
    Public OpenPhase As QuestPhase = Nothing
    Public rylVersion As Integer = 2
    Public Function AddNewQuest() As Quest
        Dim q As New Quest
        If rylVersion = 1 Then
            q.IdString = "myQuest" & Now.Millisecond.ToString()

            Dim f As New CMcfBase.SFunction
            f.data = New CMcfBase.SScriptLine() {}
            f.id = UBound(iFuncs) + 1
            f.isExternal = False
            f.name = q.IdString
            f.parameterTypes = New CMcfBase.DataType() {}
            f.returnType = CMcfBase.DataType.EInteger
            ReDim Preserve iFuncs(UBound(iFuncs) + 1)
            iFuncs(UBound(iFuncs)) = f

            Dim ql As New QLine() 'do first manually to set the owner function
            ql.params = New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(CMcfBase.DataType.EBool, 1)}
            ql.ownerFunction = UBound(iFuncs)
            ql.Type = QLine.KnownType.EQuestCompleteSave
            q.AddLine(ql)

            q.CreateLine(QLine.KnownType.EQuestTitle, CMcfBase.DataType.EString, "New quest")
            q.CreateLine(QLine.KnownType.EQuestLevel, CMcfBase.DataType.EString, "LV 1~95")
            q.CreateLine(QLine.KnownType.EQuestAward, CMcfBase.DataType.EString, "- Experience 1\\- Gold 1")
            q.CreateLine(QLine.KnownType.EQuestDesc, CMcfBase.DataType.EString, "New quest description")
            q.CreateLine(QLine.KnownType.EQuestShortDesc, CMcfBase.DataType.EString, "")
            q.CreateLine(QLine.KnownType.EQuestIcon, New CMcfBase.SParamElem() { _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EString, "Quest_misc01.dds"), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 84), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 114), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 126), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 156) _
            })
        Else
            Dim prevId As Integer = 0
            For Each mq As Quest In Quests
                Dim qid As Integer = mq.Id
                If qid > &HF000 Then qid -= &HF000
                If qid > prevId Then prevId = qid
            Next
            q.Id = prevId + 1
            q.CreateLine(QLine.KnownType.EQuestStart, New CMcfBase.SParamElem() { _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, q.Id), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 1), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 95), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, &HFF0FFF), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 0), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 0), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EBool, 0) _
            })
            q.CreateLine(QLine.KnownType.EQuestType, New CMcfBase.SParamElem() { _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 0), _
                CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, 1) _
            })
            q.CreateLine(QLine.KnownType.EQuestCompleteSave, CMcfBase.DataType.EBool, 1)
            q.CreateLine(QLine.KnownType.EQuestTitle, CMcfBase.DataType.EString, "New quest")
            q.CreateLine(QLine.KnownType.EQuestLevel, CMcfBase.DataType.EString, "LV 1")
            q.CreateLine(QLine.KnownType.EQuestAward, CMcfBase.DataType.EString, "- Experience 1\\- Gold 1")
            q.CreateLine(QLine.KnownType.EQuestDesc, CMcfBase.DataType.EString, "New quest description")
            q.CreateLine(QLine.KnownType.EQuestShortDesc, CMcfBase.DataType.EString, "")
            q.CreateLine(QLine.KnownType.EQuestEnd)
        End If

        ReDim Preserve Quests(UBound(Quests) + 1)
        Quests(UBound(Quests)) = q
        Return Quests(UBound(Quests))
    End Function
    Public Sub DeleteQuest(ByRef q As Quest)
        Dim arrl As New ArrayList
        For Each l As Quest In Quests
            If Not l Is q Then arrl.Add(l)
        Next
        Quests = arrl.ToArray(GetType(Quest))
    End Sub
    Public Sub Parse(ByVal funcs As CMcfBase.SFunction())
        iFuncs = funcs
        Array.Resize(Quests, 0)
        If rylVersion = 1 Then
            For Each f As CMcfBase.SFunction In funcs
                If Not f.isExternal AndAlso f.name <> "" AndAlso f.data.Length > 0 Then
                    Dim q As New Quest(f.id, f.name)
                    For Each l As CMcfBase.SScriptLine In f.data
                        Dim qL As New QLine(l, QLine.String2Type(funcs(l.callTo).name))
                        qL.ownerFunction = f.id
                        q.AddLine(qL)
                    Next
                    ReDim Preserve Quests(UBound(Quests) + 1)
                    Quests(UBound(Quests)) = q
                Else
                    'For Each l As CMcfBase.SScriptLine In f.data
                    '    UnknownLines.Add(New QLine(l, QLine.String2Type(funcs(l.callTo).name)))
                    'Next
                End If
            Next
        Else
            Dim goingQ As Long = -1
            For Each f As CMcfBase.SFunction In funcs
                For Each l As CMcfBase.SScriptLine In f.data
                    Dim qL As New QLine(l, QLine.String2Type(funcs(l.callTo).name))
                    qL.ownerFunction = f.id
                    If qL.Type = QLine.KnownType.EQuestStart Then
                        Dim qId As Integer = qL.params(0).value
                        Dim q As New Quest(qId)
                        goingQ += 1
                        q.AddLine(qL)
                        ReDim Preserve Quests(goingQ)
                        Quests(goingQ) = q
                    ElseIf goingQ >= 0 Then
                        Quests(goingQ).AddLine(qL)
                    Else
                        'UnknownLines.Add(qL)
                    End If
                Next
            Next
        End If
    End Sub
    Public Function GetFunctions() As CMcfBase.SFunction()
        For i As Integer = 0 To iFuncs.Length - 1
            iFuncs(i).data = New CMcfBase.SScriptLine() {}
        Next
        For i As Long = 0 To Quests.Length - 1
            Dim ls As QLine() = Quests(i).iLines
            For Each l As QLine In ls
                Dim ln As New CMcfBase.SScriptLine
                ln.parameters = l.params
                For Each f As CMcfBase.SFunction In iFuncs
                    If f.name = QLine.Type2String(l.Type) Then
                        ln.callTo = f.id
                        Exit For
                    End If
                Next
                Dim f2 As CMcfBase.SFunction = iFuncs(l.ownerFunction)
                ReDim Preserve f2.data(UBound(f2.data) + 1)
                f2.data(UBound(f2.data)) = ln
                f2.name = Quests(i).IdString
                iFuncs(l.ownerFunction) = f2
            Next
        Next
        Dim funcs As New List(Of CMcfBase.SFunction)
        For i As Integer = 0 To iFuncs.Length - 1
            If iFuncs(i).isExternal OrElse iFuncs(i).data.Length > 0 OrElse iFuncs(i).name = "" Then funcs.Add(iFuncs(i))
        Next
        iFuncs = funcs.ToArray()
        Return iFuncs
    End Function
    Public Class QLine
        Public params As CMcfBase.SParamElem() = {}
        Public Type As KnownType
        Public ownerFunction As Integer = 0
        Public Sub New()
        End Sub
        Public Sub New(ByVal line As CMcfBase.SScriptLine, ByVal aType As KnownType)
            params = line.parameters
            Type = aType
        End Sub
        Public Enum KnownType
            EQuestEnd
            EQuestSkillPointBonus
            EQuestStart
            EQuestType
            EQuestArea
            EQuestTitle
            EQuestDesc
            EQuestShortDesc
            EQuestIcon
            EQuestCompleteSave
            EQuestLevel
            EQuestAward
            EAddPhase
            EPhase_Target
            ETrigger_Start
            ETrigger_Puton
            ETrigger_Geton
            ETrigger_Talk
            ETrigger_Kill
            ETrigger_Pick
            ETrigger_Fame
            ETrigger_LevelTalk
            EElse
            EEvent_Disappear
            EEvent_Get
            EEvent_Spawn
            EEvent_MonsterDrop
            EEvent_Award
            EEvent_MsgBox
            EEvent_Phase
            EEvent_End
            EEvent_AwardItem
            EEvent_AddQuest
            EEvent_Move
            EEvent_TheaterMode
        End Enum
        Public Shared Function String2Type(ByVal txt As String) As KnownType
            Return KnownType.Parse(GetType(KnownType), "E" & txt)
        End Function
        Public Shared Function Type2String(ByVal type As KnownType) As String
            Return type.ToString.Substring(1)
        End Function
    End Class
    Public Class Quest
        Public Id As Integer
        Public IdString As String 'for ryl1
        Public iLines() As QLine = {}
        Public Sub New()
        End Sub
        Public Sub New(ByVal aId As Integer, Optional ByVal aIdString As String = "")
            Id = aId
            IdString = aIdString
        End Sub
        Public Sub AddLine(ByVal com As QLine)
            If iLines.Length > 0 Then com.ownerFunction = iLines(0).ownerFunction
            ReDim Preserve iLines(UBound(iLines) + 1)
            iLines(UBound(iLines)) = com
        End Sub
        Public Sub CreateLine(ByVal type As QLine.KnownType)
            CreateLine(type, New CMcfBase.SParamElem() {})
        End Sub
        Public Sub CreateLine(ByVal type As QLine.KnownType, ByVal paramType As CMcfBase.DataType, ByVal obj As Object)
            CreateLine(type, New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(paramType, obj)})
        End Sub
        Public Sub CreateLine(ByVal type As QLine.KnownType, ByRef params As CMcfBase.SParamElem())
            Dim nql As New QLine
            nql.Type = type
            nql.params = params
            Me.AddLine(nql)
        End Sub
        Public Sub DeleteLine(ByRef line As QLine)
            Dim arrl As New ArrayList
            For Each l As QLine In iLines
                If Not l Is line Then arrl.Add(l)
            Next
            iLines = arrl.ToArray(GetType(QLine))
        End Sub
        Public ReadOnly Property Name() As String
            Get
                Dim ls As QLine() = Me.Lines(QLine.KnownType.EQuestTitle)
                If ls.Length = 1 Then
                    Return ls(0).params(0).value
                Else
                    Return "0x" & AddMath.Hex2(Id)
                End If
            End Get
        End Property
        Public Overrides Function ToString() As String
            Dim ls As QLine() = Me.Lines(QLine.KnownType.EQuestLevel)
            Dim lvl As String = "LV ??"
            If ls.Length = 1 Then
                lvl = ls(0).params(0).value
            End If
            Return "[" & lvl & "]" & Me.Name
        End Function
        Public Property Lines(Optional ByVal type As QLine.KnownType = Nothing) As QLine()
            Get
                Dim tLines As New ArrayList
                For Each l As QLine In iLines
                    If type = Nothing OrElse l.Type = type Then
                        tLines.Add(l)
                    End If
                Next
                Return tLines.ToArray(GetType(QLine))
            End Get
            Set(ByVal value As QLine())
                Dim tLines As New ArrayList
                For Each l As QLine In iLines
                    If Not type = Nothing AndAlso l.Type <> type Then
                        tLines.Add(l)
                    End If
                Next
                For Each v As QLine In value
                    tLines.Add(v)
                Next
                iLines = tLines.ToArray(GetType(QLine()))
            End Set
        End Property
        Public ReadOnly Property Maps() As Integer()
            Get
                Dim zSs As New ArrayList
                Dim ls As QLine() = Me.Lines(QLine.KnownType.EAddPhase)
                For Each l As QLine In ls
                    If zSs.IndexOf(Convert.ToInt32(l.params(0).value)) < 0 Then zSs.Add(Convert.ToInt32(l.params(0).value))
                Next
                zSs.Sort()
                Return zSs.ToArray(GetType(Integer))
            End Get
        End Property
        Public Function Phases() As QuestPhase()
            Dim phaseLines As New ArrayList 'type=QLine
            Dim out As New ArrayList 'type=QuestPhase
            Dim add As Boolean = False
            For Each l As QLine In iLines
                If l.Type = QLine.KnownType.EAddPhase Then
                    If phaseLines.Count > 0 Then
                        out.Add(New QuestPhase(phaseLines.ToArray(GetType(QLine))))
                        phaseLines.Clear()
                    End If
                    add = True
                ElseIf l.Type = QLine.KnownType.EQuestEnd Then
                    If phaseLines.Count > 0 Then
                        out.Add(New QuestPhase(phaseLines.ToArray(GetType(QLine))))
                        phaseLines.Clear()
                    End If
                    add = False
                    Exit For
                End If
                If add Then phaseLines.Add(l)
            Next
            If add AndAlso phaseLines.Count > 0 Then
                out.Add(New QuestPhase(phaseLines.ToArray(GetType(QLine))))
                phaseLines.Clear()
            End If
            Return out.ToArray(GetType(QuestPhase))
        End Function
        Public Function getLinesForPhase(ByVal nr As Integer) As QLine()
            Dim phaseLines As New ArrayList 'type=QLine
            Dim adding As Boolean = False
            For Each l As QLine In iLines
                If l.Type = QLine.KnownType.EAddPhase Then
                    If l.params(1).value = nr Then
                        adding = True
                    Else
                        adding = False
                    End If
                ElseIf l.Type = QLine.KnownType.EQuestEnd Then
                    Exit For
                End If
                If adding Then phaseLines.Add(l)
            Next
            Return phaseLines.ToArray(GetType(QLine))
        End Function

    End Class
    Public Function questIndexForID(ByVal id As Integer) As Integer
        Dim i As Integer = 0
        For Each q As Quest In Quests
            If q.Id = id Then
                Return i
            End If
            i += 1
        Next
        Return -1
    End Function
    Public Class QuestPhase
        Public Shared lvl3functions As QLine.KnownType() = {QLine.KnownType.ETrigger_Fame, QLine.KnownType.ETrigger_Geton, QLine.KnownType.ETrigger_Kill, QLine.KnownType.ETrigger_LevelTalk, QLine.KnownType.ETrigger_Pick, QLine.KnownType.ETrigger_Puton, QLine.KnownType.ETrigger_Start, QLine.KnownType.ETrigger_Talk}
        Public Shared lvl4functions As QLine.KnownType() = {QLine.KnownType.EEvent_AddQuest, QLine.KnownType.EEvent_Award, QLine.KnownType.EEvent_AwardItem, QLine.KnownType.EEvent_Disappear, QLine.KnownType.EEvent_End, QLine.KnownType.EEvent_Get, QLine.KnownType.EEvent_MonsterDrop, QLine.KnownType.EEvent_Move, QLine.KnownType.EEvent_MsgBox, QLine.KnownType.EEvent_Phase, QLine.KnownType.EEvent_Spawn, QLine.KnownType.EEvent_TheaterMode}
        Public Id As Integer = 0 'zero based
        Public Zone As Integer = 0
        Public Name As String = ""
        Public phaseF As QLine = Nothing
        Public mapPointers() As Point = {}
        Public mainFunction As QLine = Nothing
        Public mainFunctionSiblings() As QLine = {}
        Public ElseFunction As QLine = Nothing
        Public elseSiblings() As QLine = {}
        Public Sub New()
        End Sub
        Public Sub New(ByVal lines As QLine())
            If lines(0).Type <> QLine.KnownType.EAddPhase Then Throw New Exception("Invalid Quest Phase")
            Zone = lines(0).params(0).value
            Id = lines(0).params(1).value
            Name = lines(0).params(2).value
            phaseF = lines(0)
            Dim running As Integer = 0
            Dim lineBuf As New ArrayList
            For i As Integer = 1 To lines.Length - 1
                Dim l As QLine = lines(i)
                If l.Type = QLine.KnownType.EPhase_Target Then
                    ReDim Preserve mapPointers(UBound(mapPointers) + 1)
                    mapPointers(UBound(mapPointers)) = New Point(l.params(0).value, l.params(1).value)
                ElseIf l.Type = QLine.KnownType.EElse Then
                    ElseFunction = l
                    If running = 1 AndAlso lineBuf.Count > 0 Then
                        mainFunctionSiblings = lineBuf.ToArray(GetType(QLine))
                        lineBuf.Clear()
                    End If
                    running = 2
                ElseIf Array.IndexOf(lvl3functions, l.Type) >= 0 Then
                    mainFunction = l
                    running = 1
                ElseIf running > 0 Then
                    lineBuf.Add(l)
                End If
            Next
            If running > 0 AndAlso lineBuf.Count > 0 Then
                If running = 1 Then
                    mainFunctionSiblings = lineBuf.ToArray(GetType(QLine))
                ElseIf running = 2 Then
                    elseSiblings = lineBuf.ToArray(GetType(QLine))
                End If
                lineBuf.Clear()
            End If
        End Sub
        'Public Function GetQLines() As QLine()
        '    Dim out(mapPointers.Length + IIf(Not mainFunction Is Nothing, 1 + mainFunctionSiblings.Length, 0) + IIf(hasElseFunction, 1 + elseSiblings.Length, 0)) As QLine
        '    phaseF.params(0).value = Zone
        '    phaseF.params(1).value = Id
        '    phaseF.params(2).value = Name
        '    out(0) = phaseF
        '    Dim pos As Integer = 1
        '    For Each p As Point In mapPointers
        '        Dim nQL As New QLine()
        '        nQL.Type = QLine.KnownType.EPhase_Target
        '        nQL.params = New CMcfBase.SParamElem() {CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, p.X), CMcfBase.CreateParamElem(CMcfBase.DataType.EInteger, p.Y)}
        '        out(pos) = nQL
        '        pos += 1
        '    Next
        '    If Not mainFunction Is Nothing Then
        '        out(pos) = mainFunction
        '        pos += 1
        '        For Each f As QLine In mainFunctionSiblings
        '            out(pos) = f
        '            pos += 1
        '        Next
        '    End If
        '    If hasElseFunction Then
        '        Dim nQL As New QLine()
        '        nQL.params = New CMcfBase.SParamElem() {}
        '        nQL.Type = QLine.KnownType.EElse
        '        out(pos) = nQL
        '        pos += 1
        '        For Each f As QLine In elseSiblings
        '            out(pos) = f
        '            pos += 1
        '        Next
        '    End If
        '    Return out
        'End Function
    End Class
#Region "Enums"
    Public Class QClassEnum
        Public Enum base
            EAll = &HFF0FFF 'Everyone
            EAkhans = &HFF0000
            EHumans = &HFFF
            E0xFC0FF0 = &HFC0FF0 '<- WTF is this?!?!
            E0xFC0FFF = &HFC0FFF '<- WTF is this too?!?! (fight the hard fight)

            EFighter = &H1
            ERogue = &H2
            EMage = &H4
            EAcolyte = &H8
            ECombatant = &H10000
            EOfficiator = &H20000

            EDefender = &H10 'Fighter
            EWarrior = &H20
            EAssasin = &H40 'Rogue
            EArcher = &H80
            ESourcerer = &H100 'Mage
            EEnchanter = &H200
            EPriest = &H400 'Acolyte
            ECleric = &H800
            EGunner = &H100000 'Combatant
            ETemplar = &H40000
            EAttacker = &H80000
            ERune = &H200000 'Officiator
            ELife = &H400000
            EShadow = &H800000
        End Enum
        Public Shared Function Type2String(ByVal type As base) As String
            Return type.ToString.Substring(1)
        End Function
    End Class
    Public Class QEQGradeEnum
        Public Enum base
            EAAA = 0
            EAA = 1
            EA = 2
            EB = 3
            EC = 4
            ED = 5
            EF = 6
        End Enum
        Public Shared Function Type2String(ByVal type As base) As String
            Return type.ToString.Substring(1)
        End Function
    End Class
    Public Class QEQTypeEnum
        Public Enum base

            'Human armor
            ECON__ARMOR = 3
            ECON__HELM = 4
            ECON__GLOVE = 5
            ECON__BOOT = 6
            EDEX__ARMOUR = 7
            EDEX__HELM = 8
            EDEX__GLOVE = 9
            EDEX__BOOTS = 10

            'Human Weapon
            EONEHANDED__SWORD = 11
            ETWOHANDED__SWORD = 12
            EONEHANDED__AXE = 13
            ETWOHANDED__AXE = 14
            EONEHANDED__BLUNT = 15
            ETWOHANDED__BLUNT = 16
            EBOW = 17
            ECROSSBOW = 18
            ESTAFF = 19
            EDAGGER = 20
            ESHIELD = 21

            'Ak'Kan Armor
            ECON__BODY = 22
            ECON__HEAD = 23
            ECON__PELVIS = 24
            ECON__PROTECT_ARM = 25
            EDEX__BODY = 26
            EDEX__HEAD = 27
            EDEX__PELVIS = 28
            EDEX__PROTECT_ARM = 29

            'Ak'Kan Weapon
            ECOM__BLUNT = 30
            ECOM__SWORD = 31
            EOPP__HAMMER = 32
            EOPP__AXE = 33
            EOPP__SLUSHER = 34
            EOPP__TALON = 35
            EOPP__SYTHE = 36

            'Skillarm
            ESKILL_ARM_GUARD = 37
            ESKILL_ARM_ATTACK = 38
            ESKILL_ARM_GUN = 39
            ESKILL_ARM_KNIFE = 40
        End Enum
        Public Shared Function Type2String(ByVal type As base) As String
            Dim s As String = type.ToString.Substring(1)
            Dim k As String() = s.Split("__")
            If k.Length = 2 Then
                s = k(1) & "(" & k(0) & ")"
            End If
            Return s.Replace("_", " ")
        End Function
    End Class
    Public Class QNationEnum
        Public Enum base
            EAll = 0
            EKartefant = 1
            EMerkhaida = 2
            EGod_Pirates = 3
        End Enum
        Public Shared Function Type2String(ByVal type As base) As String
            Return type.ToString.Substring(1).Replace("_", " ")
        End Function
    End Class
    Public Class QTypeEnum
        Public Enum base
            ETalk_To_NPC = 0
            EUse_Item = 1
            EGo_Somewhere = 2
        End Enum
        Public Shared Function Type2String(ByVal type As base) As String
            Return type.ToString.Substring(1).Replace("_", " ")
        End Function
    End Class
#End Region

End Class
