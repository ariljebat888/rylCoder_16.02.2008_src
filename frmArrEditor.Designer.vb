<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmArrEditor
    Inherits rylCoder.frmMap

    'Form overrides dispose to clean up the component list.
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
        Me.btnDrawVisible = New System.Windows.Forms.Button
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.numMaxNumMobs = New System.Windows.Forms.NumericUpDown
        Me.Label6 = New System.Windows.Forms.Label
        Me.numRadius = New System.Windows.Forms.NumericUpDown
        Me.numDensity = New System.Windows.Forms.NumericUpDown
        Me.Label5 = New System.Windows.Forms.Label
        Me.btnMultiplayPoint = New System.Windows.Forms.Button
        Me.cmbMultiStyle = New System.Windows.Forms.ComboBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.btnShowThisTypeMob = New System.Windows.Forms.Button
        Me.lnkMobName = New System.Windows.Forms.LinkLabel
        Me.btnCreateNewParty = New System.Windows.Forms.Button
        Me.btnShowThisParty = New System.Windows.Forms.Button
        Me.txtResArea = New System.Windows.Forms.TextBox
        Me.txtPartyId = New System.Windows.Forms.TextBox
        Me.txtMobId = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnDrawJpgMap = New System.Windows.Forms.Button
        Me.dlgSaveJpg = New System.Windows.Forms.SaveFileDialog
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.numMaxNumMobs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numDensity, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnDrawVisible
        '
        Me.btnDrawVisible.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDrawVisible.Location = New System.Drawing.Point(648, 98)
        Me.btnDrawVisible.Name = "btnDrawVisible"
        Me.btnDrawVisible.Size = New System.Drawing.Size(127, 21)
        Me.btnDrawVisible.TabIndex = 0
        Me.btnDrawVisible.Text = "Draw visible mobs"
        Me.btnDrawVisible.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.GroupBox3)
        Me.GroupBox2.Controls.Add(Me.btnShowThisTypeMob)
        Me.GroupBox2.Controls.Add(Me.lnkMobName)
        Me.GroupBox2.Controls.Add(Me.btnCreateNewParty)
        Me.GroupBox2.Controls.Add(Me.btnShowThisParty)
        Me.GroupBox2.Controls.Add(Me.txtResArea)
        Me.GroupBox2.Controls.Add(Me.txtPartyId)
        Me.GroupBox2.Controls.Add(Me.txtMobId)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(641, 125)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(146, 345)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Selected mob"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.numMaxNumMobs)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.numRadius)
        Me.GroupBox3.Controls.Add(Me.numDensity)
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.btnMultiplayPoint)
        Me.GroupBox3.Controls.Add(Me.cmbMultiStyle)
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Location = New System.Drawing.Point(0, 193)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(146, 152)
        Me.GroupBox3.TabIndex = 10
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Multiply"
        '
        'numMaxNumMobs
        '
        Me.numMaxNumMobs.Location = New System.Drawing.Point(54, 99)
        Me.numMaxNumMobs.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.numMaxNumMobs.Name = "numMaxNumMobs"
        Me.numMaxNumMobs.Size = New System.Drawing.Size(80, 20)
        Me.numMaxNumMobs.TabIndex = 8
        Me.numMaxNumMobs.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 101)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(42, 13)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = "Max nr:"
        '
        'numRadius
        '
        Me.numRadius.Location = New System.Drawing.Point(54, 47)
        Me.numRadius.Maximum = New Decimal(New Integer() {2000, 0, 0, 0})
        Me.numRadius.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numRadius.Name = "numRadius"
        Me.numRadius.Size = New System.Drawing.Size(80, 20)
        Me.numRadius.TabIndex = 6
        Me.numRadius.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'numDensity
        '
        Me.numDensity.Location = New System.Drawing.Point(54, 73)
        Me.numDensity.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.numDensity.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numDensity.Name = "numDensity"
        Me.numDensity.Size = New System.Drawing.Size(80, 20)
        Me.numDensity.TabIndex = 5
        Me.numDensity.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 75)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(45, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Density:"
        '
        'btnMultiplayPoint
        '
        Me.btnMultiplayPoint.Location = New System.Drawing.Point(6, 125)
        Me.btnMultiplayPoint.Name = "btnMultiplayPoint"
        Me.btnMultiplayPoint.Size = New System.Drawing.Size(128, 21)
        Me.btnMultiplayPoint.TabIndex = 0
        Me.btnMultiplayPoint.Text = "Multiply"
        Me.btnMultiplayPoint.UseVisualStyleBackColor = True
        '
        'cmbMultiStyle
        '
        Me.cmbMultiStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMultiStyle.FormattingEnabled = True
        Me.cmbMultiStyle.Items.AddRange(New Object() {"Circle", "Circle X", "Circle Rand", "Rectangle", "Rectangle X", "Rectangle Rand"})
        Me.cmbMultiStyle.Location = New System.Drawing.Point(6, 19)
        Me.cmbMultiStyle.Name = "cmbMultiStyle"
        Me.cmbMultiStyle.Size = New System.Drawing.Size(128, 21)
        Me.cmbMultiStyle.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 49)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Radius:"
        '
        'btnShowThisTypeMob
        '
        Me.btnShowThisTypeMob.Location = New System.Drawing.Point(6, 112)
        Me.btnShowThisTypeMob.Name = "btnShowThisTypeMob"
        Me.btnShowThisTypeMob.Size = New System.Drawing.Size(128, 21)
        Me.btnShowThisTypeMob.TabIndex = 10
        Me.btnShowThisTypeMob.Text = "Show only this type"
        Me.btnShowThisTypeMob.UseVisualStyleBackColor = True
        '
        'lnkMobName
        '
        Me.lnkMobName.AutoSize = True
        Me.lnkMobName.Location = New System.Drawing.Point(6, 16)
        Me.lnkMobName.Name = "lnkMobName"
        Me.lnkMobName.Size = New System.Drawing.Size(39, 13)
        Me.lnkMobName.TabIndex = 9
        Me.lnkMobName.TabStop = True
        Me.lnkMobName.Text = "[name]"
        '
        'btnCreateNewParty
        '
        Me.btnCreateNewParty.Location = New System.Drawing.Point(6, 166)
        Me.btnCreateNewParty.Name = "btnCreateNewParty"
        Me.btnCreateNewParty.Size = New System.Drawing.Size(128, 21)
        Me.btnCreateNewParty.TabIndex = 7
        Me.btnCreateNewParty.Text = "Create new party"
        Me.btnCreateNewParty.UseVisualStyleBackColor = True
        '
        'btnShowThisParty
        '
        Me.btnShowThisParty.Location = New System.Drawing.Point(6, 139)
        Me.btnShowThisParty.Name = "btnShowThisParty"
        Me.btnShowThisParty.Size = New System.Drawing.Size(128, 21)
        Me.btnShowThisParty.TabIndex = 6
        Me.btnShowThisParty.Text = "Show only this party"
        Me.btnShowThisParty.UseVisualStyleBackColor = True
        '
        'txtResArea
        '
        Me.txtResArea.Location = New System.Drawing.Point(63, 87)
        Me.txtResArea.Name = "txtResArea"
        Me.txtResArea.Size = New System.Drawing.Size(71, 20)
        Me.txtResArea.TabIndex = 5
        '
        'txtPartyId
        '
        Me.txtPartyId.Location = New System.Drawing.Point(63, 61)
        Me.txtPartyId.Name = "txtPartyId"
        Me.txtPartyId.Size = New System.Drawing.Size(71, 20)
        Me.txtPartyId.TabIndex = 4
        '
        'txtMobId
        '
        Me.txtMobId.Location = New System.Drawing.Point(63, 35)
        Me.txtMobId.Name = "txtMobId"
        Me.txtMobId.Size = New System.Drawing.Size(71, 20)
        Me.txtMobId.TabIndex = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 90)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(54, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Res Area:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(23, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(34, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Party:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(36, 38)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(21, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "ID:"
        '
        'btnDrawJpgMap
        '
        Me.btnDrawJpgMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDrawJpgMap.Location = New System.Drawing.Point(648, 470)
        Me.btnDrawJpgMap.Name = "btnDrawJpgMap"
        Me.btnDrawJpgMap.Size = New System.Drawing.Size(127, 21)
        Me.btnDrawJpgMap.TabIndex = 9
        Me.btnDrawJpgMap.Text = "E&xport mob map"
        Me.btnDrawJpgMap.UseVisualStyleBackColor = True
        '
        'dlgSaveJpg
        '
        Me.dlgSaveJpg.Filter = "PNG image(*.png)|*.png|Bitmap image (*.bmp)|*.bmp|GIF image (*.gif)|*.gif|JPEG im" & _
            "age (*.jpg)|*.jpg|All Files(*.*)|*.*"
        '
        'frmArrEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(797, 534)
        Me.Controls.Add(Me.btnDrawJpgMap)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.btnDrawVisible)
        Me.MinimumSize = New System.Drawing.Size(813, 570)
        Me.Name = "frmArrEditor"
        Me.Text = "Arrangement editor"
        Me.Controls.SetChildIndex(Me.btnDrawVisible, 0)
        Me.Controls.SetChildIndex(Me.GroupBox2, 0)
        Me.Controls.SetChildIndex(Me.btnDrawJpgMap, 0)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.numMaxNumMobs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numRadius, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numDensity, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnDrawVisible As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtResArea As System.Windows.Forms.TextBox
    Friend WithEvents txtPartyId As System.Windows.Forms.TextBox
    Friend WithEvents txtMobId As System.Windows.Forms.TextBox
    Friend WithEvents btnCreateNewParty As System.Windows.Forms.Button
    Friend WithEvents btnShowThisParty As System.Windows.Forms.Button
    Friend WithEvents btnDrawJpgMap As System.Windows.Forms.Button
    Friend WithEvents lnkMobName As System.Windows.Forms.LinkLabel
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents btnMultiplayPoint As System.Windows.Forms.Button
    Friend WithEvents numRadius As System.Windows.Forms.NumericUpDown
    Friend WithEvents numDensity As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cmbMultiStyle As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnShowThisTypeMob As System.Windows.Forms.Button
    Friend WithEvents numMaxNumMobs As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents dlgSaveJpg As System.Windows.Forms.SaveFileDialog

End Class
