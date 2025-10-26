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

Public Class frmSelectItem
    Private closeB As Boolean = False
    Public ItemScript() As GameItem
    Private openedId As Long = 0
    Public Event ItemSelected(ByVal sender As frmSelectItem, ByVal item As Long, ByVal prevItem As Long)
    Public Event ItemScriptLoaded()
    Public ItemsLoaded As Boolean = False
    Private returnId As Long = 0

#Region "ItemScript Desc"
    '0ID	 402
    '1ItemName	Half Vest
    '2FieldModelName	breast.r3s
    '3AttachedModelName	training_breast
    '4SpriteDDS	item011
    '5MinX	8
    '6MinY	109
    '7MaxX	63
    '8MaxY	186
    '9SizeX	3
    '10SizeY	4
    '11EffectSound	armor1.wav
    '12ItemType	CON_ARMOUR
    '13TypeName	Armor (CON)
    '14AbleExchangeNDrop	O (big o)
    '15AbleSell		O (big o)
    '16MinDropLevel	5
    '17MaxDropLevel	11
    '18Price	206
    '19OptionPrice	O
    '20BlackPrice 2611
    '21MedalPrice	
    '22ClassLimit	
    '23LimitStat	
    '24Limit	
    '25Durability	
    '26MaxDurability	
    '27MinDamage	
    '28MaxDamage	
    '29Defence	
    '30Block	
    '31DropOption	
    '32Grade	
    '33CriticalType	
    '34AttackRange	
    '35MaxSocketNum	
    '36MagicResist	
    '37Evade	
    '38CoolDown	
    '39MaxHP	
    '40MaxMP	
    'HPRegen	
    'MPRegen	
    'Speed	
    'SkillPoint	
    'Frost	
    'Fire	
    'Electro	
    'Darkness
    '	0	3120	CON	30	100	100	0	0	7	0	30	X	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0
#End Region

    Public Structure GameItem
        Dim ID As Long
        Dim ItemName As String
        Dim TypeName As String
        Dim Price As Long
        Dim BlackPrice As Long
        Dim MedalPrice As Long
        Dim LimitStat As String
        Dim Limit As Integer
        Dim SpriteDDS As String
        Dim SpriteRect As Rectangle
        Dim slotsX As Integer
        Dim slotsY As Integer
    End Structure

    Private Class listItem
        Public iName As String
        Public iId As Long
        Public igItem As GameItem
        Public Sub New(ByVal name As String, ByVal id As Long, ByVal gItem As GameItem)
            iName = name
            iId = id
            igItem = gItem
        End Sub
        Public Overrides Function ToString() As String
            Return iName
        End Function
    End Class

    Private Function str2GameItem(ByVal txt As String) As GameItem
        Dim arr() As String = txt.Split(vbTab)
        Dim item As New GameItem
        For i As Integer = 0 To arr.Length - 1
            Dim l As String = arr(i)
            Select Case i
                Case 0 : item.ID = l
                Case 1 : item.ItemName = l
                Case 4 : item.SpriteDDS = l & ".dds"
                Case 5 : item.SpriteRect.X = l
                Case 6 : item.SpriteRect.Y = l
                Case 7 : item.SpriteRect.Width = l - item.SpriteRect.X
                Case 8 : item.SpriteRect.Height = l - item.SpriteRect.Y
                Case 9 : item.slotsX = l
                Case 10 : item.slotsY = l
                Case 13 : item.TypeName = l
                Case 18 : item.Price = l
                Case 20 : item.BlackPrice = l
                Case 21 : item.MedalPrice = l
                Case 23 : item.LimitStat = l
                Case 24 : item.Limit = l
            End Select
        Next
        Return item
    End Function

    Public Sub New(ByVal file As String, Optional ByVal interactive As Boolean = True)
        Me.closeB = False
        Me.openedId = 0
        Me.ItemsLoaded = False
        Me.returnId = 0
        Try
            Dim list As New ArrayList
            Dim reader As New IO.StreamReader(file)
            reader.ReadLine()
            Do While Not reader.EndOfStream
                list.Add(reader.ReadLine)
            Loop
            reader.Close()
            Dim lines As String() = DirectCast(list.ToArray(GetType(String)), String())
            Me.construct((lines), interactive)
        Catch exception1 As Exception
            Dim exception As Exception = exception1
            If interactive Then
                Interaction.MsgBox(exception.Message, MsgBoxStyle.OkOnly, Nothing)
            End If
        End Try
    End Sub



    Private Sub construct(ByRef lines As String(), Optional ByVal interactive As Boolean = True)
        InitializeComponent()
        Array.Resize(ItemScript, 0)
        Me.cmbItemType.Items.Clear()
        Try
            Dim str As String
            For Each str In lines
                If ((str.Length > 1) AndAlso (str.Substring(0, 2) <> "//")) Then
                    ReDim Preserve ItemScript(UBound(ItemScript) + 1)
                    ItemScript(UBound(ItemScript)) = str2GameItem(str)
                End If
            Next
        Catch ex As Exception
            If interactive Then MsgBox(ex.Message)
            Exit Sub
        End Try
        ItemsLoaded = True
        RaiseEvent ItemScriptLoaded()
        Dim ites As New ArrayList()
        For Each item As GameItem In ItemScript
            If ites.IndexOf(item.TypeName) < 0 AndAlso Trim(item.TypeName) <> "" Then
                ites.Add(Trim(item.TypeName))
            End If
        Next
        ites.Sort()
        Me.cmbItemType.Items.Add("<-- OFF -->")
        Me.cmbItemType.Items.AddRange(ites.ToArray(GetType(String)))
        setFilter()
    End Sub

    Public Sub New(ByRef lines As String(), Optional ByVal interactive As Boolean = True)
        Me.closeB = False
        Me.openedId = 0
        Me.ItemsLoaded = False
        Me.returnId = 0
        Me.construct((lines), interactive)
    End Sub


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        'Me.Close()
        Dim item As Long = 0
        If Not Me.lstItems.SelectedItem Is Nothing Then item = Me.lstItems.SelectedItem.iID
        returnId = item
        Me.Hide()
        RaiseEvent ItemSelected(Me, item, openedId)
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        'Me.Close()
        Me.Hide()
    End Sub
    Public Sub kill()
        ItemScript = Nothing
        closeB = True
        Me.Close()
    End Sub
    Private Sub frmSelectItem_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not closeB AndAlso Not e.CloseReason = CloseReason.WindowsShutDown AndAlso Not e.CloseReason = CloseReason.TaskManagerClosing AndAlso Not e.CloseReason = CloseReason.FormOwnerClosing Then e.Cancel = True
        Me.Hide()
    End Sub
    Public Function open(Optional ByVal itemId As Long = 0) As Long
        openedId = itemId
        returnId = 0
        If itemId > 0 Then
            Dim found As Boolean = False
            Dim i As Long = 0
            For Each it As listItem In Me.lstItems.Items
                If it.iId = itemId Then
                    Me.lstItems.SelectedIndex = i

                    found = True
                    Exit For
                End If
                i += 1
            Next
            If Not found Then
                Me.cmbItemType.SelectedIndex = 0
                Me.txtItemName.Text = ""
                Me.txtItemID.Text = itemId
                setFilter()
                If Me.lstItems.Items.Count > 0 Then Me.lstItems.SelectedIndex = 0
            End If
        End If
        Me.ShowDialog()
        Return returnId
    End Function

    Public Sub setFilter()
        Dim type As String = Me.cmbItemType.SelectedItem
        Dim name As String = Me.txtItemName.Text
        Dim id As Long = Val(Me.txtItemID.Text)
        If type = "<-- OFF -->" Then type = ""
        Me.lstItems.Items.Clear()
        For Each s As GameItem In ItemScript
            If (name = "" OrElse s.ItemName.IndexOf(name) >= 0) AndAlso (id = 0 OrElse s.ID.ToString.IndexOf(id.ToString) >= 0) AndAlso (type = "" OrElse s.TypeName = type) Then
                Dim lI As New listItem("[" & s.ID & "] " & s.ItemName & ", " & s.TypeName & ", " & s.Limit & " " & s.LimitStat & ", " & s.Price & "€", s.ID, s)
                Me.lstItems.Items.Add(lI)
            End If
        Next
    End Sub

    Private Sub cmbItemType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbItemType.SelectedIndexChanged
        setFilter()
    End Sub

    Private Sub txtItemName_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtItemName.TextChanged
        setFilter()
    End Sub

    Private Sub txtItemID_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtItemID.TextChanged
        setFilter()
    End Sub

    Private Sub lstItems_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstItems.SelectedIndexChanged
        Dim item As listItem = lstItems.SelectedItem
        Me.imgSprite.Image = Nothing
        If Not item.iId < 1 Then
            If frmNpcEdit.RylGameDir <> String.Empty Then
                Dim bmp As Bitmap = bitmapForItem(item.igItem)
                If Not bmp Is Nothing Then
                    Dim middle As Integer = Me.imgSprite.Location.X + Me.imgSprite.Size.Width / 2
                    Me.imgSprite.Image = bmp
                    Me.imgSprite.Size = New Size(bmp.Width, bmp.Height)
                    Me.imgSprite.Left = middle - bmp.Width / 2
                End If
            End If
        End If
    End Sub

    Public Shared Function bitmapForItem(ByVal item As GameItem) As Bitmap
        If frmNpcEdit.RylGameDir <> String.Empty Then
            Return FischR.Wrapper.LoadDDS(frmNpcEdit.RylGameDir & "\texture\interface\item\" & item.SpriteDDS, item.SpriteRect)
        Else
            Return Nothing
        End If
    End Function

    Public Function bitmapForItem(ByVal itemID As Integer) As Bitmap
        For Each item As GameItem In ItemScript
            If item.ID = itemID Then
                Return bitmapForItem(item)
            End If
        Next
        Return Nothing
    End Function

    Public Function getGameItem(ByVal itemID As Integer) As GameItem
        For Each item As GameItem In ItemScript
            If item.ID = itemID Then
                Return item
            End If
        Next
        Return Nothing
    End Function
End Class
