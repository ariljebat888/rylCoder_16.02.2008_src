Public Class cntShopView
    Private gridView As Boolean = False
    Private images As New List(Of PictureBox)
    Private imageSlots(,) As Integer
    Private iSelect As frmSelectItem = Nothing
    Private Const NUM_SLOTS_HORI = 8
    Private Const NUM_SLOTS_VERT = 12
    Private Const WIDTH_SEPERATOR = 1
    Private Const WIDTH_SLOT = 25
    Private Const HEIGHT_SLOT = 25
    Public Event AddItemRequest(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event EditItemRequest(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event DeleteRequest(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event MenuOpening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    Public Sub updateForm(ByVal selector As frmSelectItem)
        iSelect = selector
        If frmNpcEdit.RylGameDir <> "" AndAlso Not selector Is Nothing Then
            gridView = True
            Me.lstSoldItems.Visible = False
            Me.Size = Me.BackgroundImage.Size
            imageSlots = New Integer(NUM_SLOTS_VERT, NUM_SLOTS_HORI) {}
            Me.Location += New Point((Me.lstSoldItems.Width - Me.BackgroundImage.Width) / 2, 0)
            Me.ContextMenuStrip = cntMnuItemAdd
        Else
            Me.lstSoldItems.Visible = True
            Me.Size = Me.lstSoldItems.Size
        End If
    End Sub
    Private Function newItemLocation(ByVal imgSlots As Size, ByVal itemID As Integer) As Point
        Dim tmpSlots(NUM_SLOTS_VERT - 1)() As Boolean

        For y As Integer = 0 To NUM_SLOTS_VERT - 1
            For x As Integer = 0 To NUM_SLOTS_HORI - 1
                Dim id As Integer = imageSlots(y, x)
                If id < 1 AndAlso y + imgSlots.Height <= NUM_SLOTS_VERT AndAlso x + imgSlots.Width <= NUM_SLOTS_HORI Then
                    Dim gotRoom As Boolean = True
                    For y2 As Integer = y To y + imgSlots.Height - 1
                        For x2 As Integer = x To x + imgSlots.Width - 1
                            If gotRoom AndAlso imageSlots(y2, x2) > 0 Then gotRoom = False
                        Next
                    Next
                    If gotRoom Then
                        For y2 As Integer = y To y + imgSlots.Height - 1
                            For x2 As Integer = x To x + imgSlots.Width - 1
                                imageSlots(y2, x2) = itemID
                            Next
                        Next
                        Return New Point(x * (WIDTH_SEPERATOR + WIDTH_SLOT) + WIDTH_SEPERATOR, y * (WIDTH_SEPERATOR + HEIGHT_SLOT) + WIDTH_SEPERATOR)
                    End If
                End If
            Next
        Next
        Return New Point(-1, -1)
    End Function
    Public Sub AddItem(ByVal item As frmNpcEdit.ShopItem)
        Me.lstSoldItems.Items.Add(item)
        If gridView Then
            Dim gItem As frmSelectItem.GameItem = iSelect.getGameItem(item.itemId)
            Dim slots As New Size(1, 1)
            Dim img As Bitmap = Nothing
            If gItem.ID > 0 Then
                img = frmSelectItem.bitmapForItem(gItem)
                slots = New Point(gItem.slotsX, gItem.slotsY)
            End If
            If img Is Nothing Then img = Global.rylCoder.My.Resources.npcShopNotFound
            Dim loc As Point = newItemLocation(slots, item.itemId)
            If loc.X >= 0 AndAlso loc.Y >= 0 Then
                img.MakeTransparent(Color.Black)
                Dim pB As New PictureBox()
                pB.Tag = New imageItemTag(item, Me.lstSoldItems.Items.Count - 1)
                pB.BackColor = Color.Transparent
                pB.Image = img
                pB.Size = New Size(slots.Width * WIDTH_SLOT + (slots.Width - 1) * WIDTH_SEPERATOR, slots.Height * HEIGHT_SLOT + (slots.Height - 1) * WIDTH_SEPERATOR)
                pB.Location = loc
                Me.Controls.Add(pB)
                images.Add(pB)
                pB.ContextMenuStrip = cntMnuNPCItemEdit
                AddHandler pB.MouseDown, AddressOf imageItem_Click
                AddHandler pB.MouseDoubleClick, AddressOf imageItem_DoubleClick
                tipItems.SetToolTip(pB, item.ToString())
            End If
        End If
    End Sub
    Public Function IndexOnList(ByVal itemID As Integer) As Integer
        Dim k As Integer = 0
        For Each item As frmNpcEdit.ShopItem In Me.lstSoldItems.Items
            If item.itemId = itemID Then Return k
            k += 1
        Next
        Return -1
    End Function
    Public Sub ClearShop()
        Me.lstSoldItems.Items.Clear()
        If gridView Then
            Me.BackgroundImage = Global.rylCoder.My.Resources.npcShopBg
            tipItems.RemoveAll()
            imageSlots = New Integer(NUM_SLOTS_VERT, NUM_SLOTS_HORI) {}
            For Each cnt As Control In images
                Me.Controls.Remove(cnt)
                cnt.Dispose()
            Next
            images = New List(Of PictureBox)
        End If
    End Sub
    Public Property SelectedIndex() As Integer
        Get
            Return Me.lstSoldItems.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If value >= 0 AndAlso value <= Me.lstSoldItems.Items.Count Then
                Me.lstSoldItems.SelectedIndex = value
                activateImageItem(images(value))
            End If
        End Set
    End Property
    Public ReadOnly Property SelectedItem() As frmNpcEdit.ShopItem
        Get
            Dim i As Integer = SelectedIndex
            If i >= 0 AndAlso i < Me.lstSoldItems.Items.Count Then
                Return Me.lstSoldItems.Items(i)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property Count() As Integer
        Get
            Return Me.lstSoldItems.Items.Count
        End Get
    End Property
    Public Property Items(ByVal index As Integer) As frmNpcEdit.ShopItem
        Get
            Return Me.lstSoldItems.Items(index)
        End Get
        Set(ByVal value As frmNpcEdit.ShopItem)
            Me.lstSoldItems.Items(index) = value
        End Set
    End Property

    Private Sub mnuNPCItemsAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuNPCItemsAdd.Click
        RaiseEvent AddItemRequest(Me.mnuNPCItemsAdd, New EventArgs)
    End Sub
    Private Sub mnuNPCItemsDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuNPCItemsDelete.Click
        RaiseEvent DeleteRequest(Me.mnuNPCItemsDelete, New EventArgs)
    End Sub
    Private Sub mnuNPCItemsEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuNPCItemsEdit.Click
        RaiseEvent EditItemRequest(Me.mnuNPCItemsEdit, New EventArgs)
    End Sub
    Private Sub cntMnuNPCItem_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cntMnuNPCItem.Opening
        RaiseEvent MenuOpening(Me.cntMnuNPCItem, e)
    End Sub

    Private Sub imageItem_Click(ByVal sender As Object, ByVal e As MouseEventArgs)
        activateImageItem(CType(sender, PictureBox))
    End Sub
    Private Sub imageItem_DoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs)
        activateImageItem(sender)
        RaiseEvent EditItemRequest(sender, New EventArgs)
    End Sub
    Private Sub activateImageItem(ByVal imgItem As PictureBox)
        Dim tag As imageItemTag = CType(imgItem.Tag, imageItemTag)
        Me.BackgroundImage = Global.rylCoder.My.Resources.npcShopBg
        For Each picBox As PictureBox In images
            Dim pTag As imageItemTag = CType(picBox.Tag, imageItemTag)
            If pTag.activated Then
                pTag.activated = False
                picBox.Tag = pTag
            End If
        Next
        tag.activated = True
        imgItem.Tag = tag
        Me.lstSoldItems.SelectedIndex = tag.index
        Dim gr As Graphics = Graphics.FromImage(Me.BackgroundImage)
        gr.DrawRectangle(Pens.BlueViolet, imgItem.Location.X, imgItem.Location.Y, imgItem.Width - 1, imgItem.Height - 1)
        gr.Flush()
        imgItem.Refresh()
    End Sub
    Private Structure imageItemTag
        Public sItem As frmNpcEdit.ShopItem
        Public activated As Boolean
        Public index As Integer
        Public Sub New(ByVal item As frmNpcEdit.ShopItem, ByVal i As Integer)
            sItem = item
            index = i
            activated = False
        End Sub
    End Structure
End Class
