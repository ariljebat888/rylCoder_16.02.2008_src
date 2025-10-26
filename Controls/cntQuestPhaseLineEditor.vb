Public Class cntQuestPhaseLineEditor
    Public line As CQuestParser.QLine = Nothing
    Public level As Integer = 0
    Private iSyntax As Xml.XmlElement = Nothing
    Public Event Save(ByRef sender As cntQuestPhaseLineEditor, ByRef line As CQuestParser.QLine)
    Public Event Close(ByRef sender As cntQuestPhaseLineEditor)
    Public Event Delete(ByRef sender As cntQuestPhaseLineEditor, ByRef line As CQuestParser.QLine)
    Public Event NeedItemName(ByRef sender As cntQuestPhaseLineEditor, ByVal id As Long, ByRef name As String)
    Public Event NeedItemSelect(ByRef sender As cntQuestPhaseLineEditor, ByRef sender2 As LinkLabel)
    Public Event NeedMobName(ByRef sender As cntQuestPhaseLineEditor, ByVal id As Long, ByRef name As String)
    Public Event NeedMobSelect(ByRef sender As cntQuestPhaseLineEditor, ByRef sender2 As LinkLabel)
    Private currentParaCount As Integer = 0
    Private currentF As CQuestParser.QLine.KnownType

    Public Sub New(ByRef qLine As CQuestParser.QLine, ByVal syntax As Xml.XmlElement)
        line = qLine
        iSyntax = syntax
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Dim funcs() As CQuestParser.QLine.KnownType = {}
        If Array.IndexOf(CQuestParser.QuestPhase.lvl3functions, line.Type) >= 0 Then level = 3
        If Array.IndexOf(CQuestParser.QuestPhase.lvl4functions, line.Type) >= 0 Then level = 4
        If level < 1 Then Exit Sub
        If level = 3 Then funcs = CQuestParser.QuestPhase.lvl3functions
        If level = 4 Then funcs = CQuestParser.QuestPhase.lvl4functions
        ' Add any initialization after the InitializeComponent() call.
        For Each e As CQuestParser.QLine.KnownType In funcs
            Me.cmbFunction.Items.Add(New frmNpcEdit.cmbItem(e, CQuestParser.QLine.Type2String(e)))
        Next
    End Sub
    Private Sub drawProps(ByVal func As CQuestParser.QLine.KnownType)
        Me.flowParas.SuspendLayout()
        Me.flowParas.Controls.Clear()
        Me.lblFuncDesc.Text = ""
        If Not iSyntax Is Nothing Then
            Dim fNodes As Xml.XmlNodeList = iSyntax.SelectNodes("func[@name='" & CQuestParser.QLine.Type2String(func) & "' and @level=" & level & "]")
            If fNodes.Count = 1 Then
                Me.lblFuncDesc.Text = fNodes(0).Attributes.GetNamedItem("desc").Value
                currentParaCount = fNodes(0).ChildNodes.Count
                currentF = func
                If fNodes(0).ChildNodes.Count > 0 Then
                    Dim i As Integer = 0
                    For Each pNode As Xml.XmlNode In fNodes(0).ChildNodes
                        Dim type As String = pNode.Attributes.GetNamedItem("type").Value
                        Dim name As String = pNode.Attributes.GetNamedItem("name").Value
                        Dim desc As String = pNode.Attributes.GetNamedItem("desc").Value
                        Dim txtI As New CNpcParser.NPCTextItem
                        txtI.paraIndex = i
                        Select Case type
                            Case "int"
                                txtI.Tag = CMcfBase.DataType.EInteger
                                If line.Type <> func Then txtI.text = "0"
                            Case "string"
                                txtI.Tag = CMcfBase.DataType.EString
                                If line.Type <> func Then txtI.text = New String("")
                            Case "float"
                                txtI.Tag = CMcfBase.DataType.EFloat
                                If line.Type <> func Then txtI.text = "0"
                            Case "bool"
                                txtI.Tag = CMcfBase.DataType.EBool
                                If line.Type <> func Then txtI.text = "0"
                        End Select
                        If line.Type = func Then
                            txtI.text = line.params(i).value
                        End If
                        If type = "int" AndAlso name.ToLower = "item id" Then
                            Dim lbl As New Label
                            lbl.Width = Me.flowParas.Width - 6
                            lbl.Text = IIf(name.ToLower <> desc.ToLower, "[" & name & "] ", "") & desc
                            lbl.Margin = New Padding(3, 0, 3, 0)
                            Me.flowParas.Controls.Add(lbl)
                            Dim obj As New LinkLabel
                            Dim itemName As String = ""
                            Dim itemId As Long = IIf(txtI.text.Length > 0, txtI.text, 0)
                            RaiseEvent NeedItemName(Me, itemId, itemName)
                            obj.Text = "Item: " & IIf(itemId > 0, itemName, "none")
                            obj.Tag = txtI
                            obj.Width = Me.flowParas.Width - 6
                            obj.Margin = New Padding(3, 0, 3, 3)
                            AddHandler obj.LinkClicked, AddressOf linkParam_Click
                            Me.flowParas.Controls.Add(obj)
                        ElseIf type = "int" AndAlso name.ToLower = "monster id" Then
                            Dim lbl As New Label
                            lbl.Width = Me.flowParas.Width - 6
                            lbl.Text = IIf(name.ToLower <> desc.ToLower, "[" & name & "] ", "") & desc
                            lbl.Margin = New Padding(3, 0, 3, 0)
                            Me.flowParas.Controls.Add(lbl)
                            Dim obj As New LinkLabel
                            Dim itemName As String = ""
                            Dim mobId As Long = IIf(txtI.text.Length > 0, txtI.text, 0)
                            RaiseEvent NeedMobName(Me, mobId, itemName)
                            obj.Text = "Mob: " & IIf(mobId > 0, itemName, "none")
                            obj.Tag = txtI
                            obj.Width = Me.flowParas.Width - 6
                            obj.Margin = New Padding(3, 0, 3, 3)
                            AddHandler obj.LinkClicked, AddressOf linkParam2_Click
                            Me.flowParas.Controls.Add(obj)
                        Else
                            Dim obj As New cntTextEditItem()
                            AddHandler obj.NPCTextChanged, AddressOf TxtItem_NPCTextChanged
                            Dim wDiff As Integer = obj.Width - obj.txtText.Width
                            obj.Width = Me.flowParas.Width - 6
                            obj.txtText.Width = obj.Width - wDiff
                            If type <> "string" Then
                                obj.Small = True
                            End If
                            If type = "int" AndAlso (name.ToLower = "quest id" OrElse name.ToLower = "npc id" OrElse desc.ToLower = "npc id") Then
                                txtI.text = "0x" & Hex(Val(txtI.text))
                            End If
                            obj.TextItem = txtI
                            obj.Tag = txtI
                            obj.Command = IIf(name.ToLower <> desc.ToLower, "[" & name & "] ", "") & desc
                            Me.flowParas.Controls.Add(obj)
                        End If
                        i += 1
                    Next
                Else
                    Me.flowParas.Controls.Add(New frmNpcEdit.namedLabel("Function has no parameters", True))
                End If
                Me.lnkSave.Enabled = True
            Else
                Me.lnkSave.Enabled = False
                Me.flowParas.Controls.Add(New frmNpcEdit.namedLabel("Function not found in Syntax XML", True))
            End If
        Else
            Me.lnkSave.Enabled = False
            Me.flowParas.Controls.Add(New frmNpcEdit.namedLabel("Syntax XML not loaded", True))
        End If
        Me.flowParas.ResumeLayout()
    End Sub
    Private Sub linkParam_Click(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs)
        RaiseEvent NeedItemSelect(Me, sender)
        Dim nName As String = ""
        If Val(sender.tag.text) > 0 Then
            RaiseEvent NeedItemName(Me, sender.tag.text, nName)
            sender.text = "Item: " & nName
        Else
            sender.Text = "Item: none"
        End If
    End Sub
    Private Sub linkParam2_Click(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs)
        RaiseEvent NeedMobSelect(Me, sender)
        Dim nName As String = ""
        If Val(sender.tag.text) > 0 Then
            RaiseEvent NeedMobName(Me, sender.tag.text, nName)
            sender.text = "Mob: " & nName
        Else
            sender.Text = "Mob: none"
        End If
    End Sub
    Private Sub lnkClose_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkClose.LinkClicked
        RaiseEvent Close(Me)
    End Sub

    Private Sub lnkSave_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkSave.LinkClicked
        Dim paraList(currentParaCount - 1) As CMcfBase.SParamElem
        Dim i As Integer = 0
        For Each c As Control In Me.flowParas.Controls
            If Not c.Tag Is Nothing Then
                Dim val As Object = Nothing
                Dim tI As CNpcParser.NPCTextItem = CType(c.Tag, CNpcParser.NPCTextItem)
                Try
                    Select Case CType(tI.Tag, CMcfBase.DataType)
                        Case CMcfBase.DataType.EBool : val = IIf(val(tI.text) > 0, 1, 0)
                        Case CMcfBase.DataType.EFloat : val = Single.Parse(tI.text)
                        Case CMcfBase.DataType.EInteger
                            If tI.text.Length > 2 AndAlso tI.text.Substring(0, 2).ToLower = "0x" Then
                                val = Convert.ToUInt32(tI.text, 16)
                            Else
                                val = UInt32.Parse(tI.text)
                            End If
                        Case CMcfBase.DataType.EString : val = tI.text
                    End Select
                Catch ex As Exception
                    MsgBox(ex.Message, MsgBoxStyle.Critical, "Save error on " & (i + 1) & "'th parameter")
                    Exit Sub
                End Try

                paraList(i) = CMcfBase.CreateParamElem(CType(tI.Tag, CMcfBase.DataType), val)
                i += 1
            End If
        Next
        line.Type = currentF
        line.params = paraList
        RaiseEvent Save(Me, line)
    End Sub

    Private Sub lnkDelete_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkDelete.LinkClicked
        RaiseEvent Delete(Me, line)
    End Sub

    Private Sub cmbFunction_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbFunction.SelectedIndexChanged
        drawProps(CType(cmbFunction.SelectedItem, frmNpcEdit.cmbItem).iItem)
    End Sub

    Private Sub cntQuestPhaseLineEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        frmNpcEdit.cmbItem.setComboSelected(Me.cmbFunction, line.Type)
    End Sub

    Private Sub TxtItem_NPCTextChanged(ByRef sender As cntTextEditItem, ByVal line As CNpcParser.NPCTextItem, ByVal newText As String)
        line.text = newText
        sender.Tag = line
    End Sub
End Class
