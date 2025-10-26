<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cntQuestPhaseLineEditor
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.lblFuncDesc = New System.Windows.Forms.Label
        Me.lnkClose = New System.Windows.Forms.LinkLabel
        Me.lnkSave = New System.Windows.Forms.LinkLabel
        Me.flowParas = New System.Windows.Forms.FlowLayoutPanel
        Me.cmbFunction = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lnkDelete = New System.Windows.Forms.LinkLabel
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lnkDelete)
        Me.GroupBox1.Controls.Add(Me.lblFuncDesc)
        Me.GroupBox1.Controls.Add(Me.lnkClose)
        Me.GroupBox1.Controls.Add(Me.lnkSave)
        Me.GroupBox1.Controls.Add(Me.flowParas)
        Me.GroupBox1.Controls.Add(Me.cmbFunction)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(289, 373)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Quest Phase Line Editor"
        '
        'lblFuncDesc
        '
        Me.lblFuncDesc.AutoEllipsis = True
        Me.lblFuncDesc.AutoSize = True
        Me.lblFuncDesc.Location = New System.Drawing.Point(6, 45)
        Me.lblFuncDesc.Name = "lblFuncDesc"
        Me.lblFuncDesc.Size = New System.Drawing.Size(102, 13)
        Me.lblFuncDesc.TabIndex = 5
        Me.lblFuncDesc.Text = "Function description"
        Me.lblFuncDesc.UseMnemonic = False
        '
        'lnkClose
        '
        Me.lnkClose.AutoSize = True
        Me.lnkClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(186, Byte))
        Me.lnkClose.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline
        Me.lnkClose.Location = New System.Drawing.Point(274, 0)
        Me.lnkClose.Name = "lnkClose"
        Me.lnkClose.Size = New System.Drawing.Size(15, 13)
        Me.lnkClose.TabIndex = 4
        Me.lnkClose.TabStop = True
        Me.lnkClose.Text = "X"
        '
        'lnkSave
        '
        Me.lnkSave.AutoSize = True
        Me.lnkSave.Location = New System.Drawing.Point(250, 351)
        Me.lnkSave.Name = "lnkSave"
        Me.lnkSave.Size = New System.Drawing.Size(32, 13)
        Me.lnkSave.TabIndex = 3
        Me.lnkSave.TabStop = True
        Me.lnkSave.Text = "Save"
        '
        'flowParas
        '
        Me.flowParas.AutoScroll = True
        Me.flowParas.Location = New System.Drawing.Point(3, 72)
        Me.flowParas.Name = "flowParas"
        Me.flowParas.Size = New System.Drawing.Size(279, 276)
        Me.flowParas.TabIndex = 2
        '
        'cmbFunction
        '
        Me.cmbFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbFunction.FormattingEnabled = True
        Me.cmbFunction.Location = New System.Drawing.Point(63, 21)
        Me.cmbFunction.Name = "cmbFunction"
        Me.cmbFunction.Size = New System.Drawing.Size(220, 21)
        Me.cmbFunction.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(51, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Function:"
        '
        'lnkDelete
        '
        Me.lnkDelete.AutoSize = True
        Me.lnkDelete.Location = New System.Drawing.Point(190, 351)
        Me.lnkDelete.Name = "lnkDelete"
        Me.lnkDelete.Size = New System.Drawing.Size(38, 13)
        Me.lnkDelete.TabIndex = 6
        Me.lnkDelete.TabStop = True
        Me.lnkDelete.Text = "Delete"
        '
        'cntQuestPhaseLineEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "cntQuestPhaseLineEditor"
        Me.Size = New System.Drawing.Size(290, 376)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cmbFunction As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents flowParas As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lnkClose As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkSave As System.Windows.Forms.LinkLabel
    Friend WithEvents lblFuncDesc As System.Windows.Forms.Label
    Friend WithEvents lnkDelete As System.Windows.Forms.LinkLabel

End Class
