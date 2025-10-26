<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cntShopView
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.cntMnuNPCItem = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuNPCItemsAdd = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuNPCItemsEdit = New System.Windows.Forms.ToolStripMenuItem
        Me.mnuNPCItemsDelete = New System.Windows.Forms.ToolStripMenuItem
        Me.lstSoldItems = New System.Windows.Forms.ListBox
        Me.tipItems = New System.Windows.Forms.ToolTip(Me.components)
        Me.cntMnuNPCItemEdit = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem
        Me.cntMnuItemAdd = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripMenuItem
        Me.cntMnuNPCItemEdit.SuspendLayout()
        Me.cntMnuItemAdd.SuspendLayout()
        Me.SuspendLayout()
        '
        'cntMnuNPCItem
        '
        Me.cntMnuNPCItem.Name = "cntMnuNPCItem"
        Me.cntMnuNPCItem.Size = New System.Drawing.Size(117, 70)
        '
        'mnuNPCItemsAdd
        '
        Me.mnuNPCItemsAdd.Name = "mnuNPCItemsAdd"
        Me.mnuNPCItemsAdd.Size = New System.Drawing.Size(152, 22)
        Me.mnuNPCItemsAdd.Text = "&Add"
        '
        'mnuNPCItemsEdit
        '
        Me.mnuNPCItemsEdit.Name = "mnuNPCItemsEdit"
        Me.mnuNPCItemsEdit.Size = New System.Drawing.Size(116, 22)
        Me.mnuNPCItemsEdit.Text = "&Edit"
        '
        'mnuNPCItemsDelete
        '
        Me.mnuNPCItemsDelete.Name = "mnuNPCItemsDelete"
        Me.mnuNPCItemsDelete.Size = New System.Drawing.Size(116, 22)
        Me.mnuNPCItemsDelete.Text = "&Delete"
        '
        'lstSoldItems
        '
        Me.lstSoldItems.ContextMenuStrip = Me.cntMnuNPCItem
        Me.lstSoldItems.FormattingEnabled = True
        Me.lstSoldItems.Location = New System.Drawing.Point(0, 0)
        Me.lstSoldItems.Name = "lstSoldItems"
        Me.lstSoldItems.Size = New System.Drawing.Size(290, 316)
        Me.lstSoldItems.TabIndex = 3
        '
        'tipItems
        '
        Me.tipItems.AutoPopDelay = 20000
        Me.tipItems.InitialDelay = 500
        Me.tipItems.IsBalloon = True
        Me.tipItems.ReshowDelay = 100
        Me.tipItems.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.tipItems.ToolTipTitle = "Info"
        '
        'cntMnuNPCItemEdit
        '
        Me.cntMnuNPCItemEdit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuNPCItemsEdit, Me.mnuNPCItemsDelete})
        Me.cntMnuNPCItemEdit.Name = "cntMnuNPCItem"
        Me.cntMnuNPCItemEdit.Size = New System.Drawing.Size(117, 48)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(32, 19)
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(32, 19)
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(32, 19)
        '
        'cntMnuItemAdd
        '
        Me.cntMnuItemAdd.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuNPCItemsAdd})
        Me.cntMnuItemAdd.Name = "cntMnuNPCItem"
        Me.cntMnuItemAdd.Size = New System.Drawing.Size(153, 48)
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(32, 19)
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(32, 19)
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        Me.ToolStripMenuItem6.Size = New System.Drawing.Size(32, 19)
        '
        'cntShopView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.rylCoder.My.Resources.Resources.npcShopBg
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Controls.Add(Me.lstSoldItems)
        Me.Name = "cntShopView"
        Me.Size = New System.Drawing.Size(290, 313)
        Me.cntMnuNPCItemEdit.ResumeLayout(False)
        Me.cntMnuItemAdd.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cntMnuNPCItem As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents mnuNPCItemsAdd As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNPCItemsEdit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuNPCItemsDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstSoldItems As System.Windows.Forms.ListBox
    Friend WithEvents tipItems As System.Windows.Forms.ToolTip
    Friend WithEvents cntMnuNPCItemEdit As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cntMnuItemAdd As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As System.Windows.Forms.ToolStripMenuItem

End Class
