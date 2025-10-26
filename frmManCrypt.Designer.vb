<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmManCrypt
    Inherits System.Windows.Forms.Form

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
        Me.txtFileIn = New System.Windows.Forms.TextBox
        Me.txtFileOut = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.chkEncrypt = New System.Windows.Forms.CheckBox
        Me.btnGo = New System.Windows.Forms.Button
        Me.StatusPanel = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.fileOpen = New System.Windows.Forms.OpenFileDialog
        Me.fileSave = New System.Windows.Forms.SaveFileDialog
        Me.btnInBrowse = New System.Windows.Forms.Button
        Me.btnOutBrowse = New System.Windows.Forms.Button
        Me.chkGsf = New System.Windows.Forms.CheckBox
        Me.StatusPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtFileIn
        '
        Me.txtFileIn.Location = New System.Drawing.Point(70, 29)
        Me.txtFileIn.Name = "txtFileIn"
        Me.txtFileIn.Size = New System.Drawing.Size(323, 20)
        Me.txtFileIn.TabIndex = 0
        '
        'txtFileOut
        '
        Me.txtFileOut.Location = New System.Drawing.Point(70, 55)
        Me.txtFileOut.Name = "txtFileOut"
        Me.txtFileOut.Size = New System.Drawing.Size(323, 20)
        Me.txtFileOut.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(43, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(21, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "IN:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(31, 58)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(33, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "OUT:"
        '
        'chkEncrypt
        '
        Me.chkEncrypt.AutoSize = True
        Me.chkEncrypt.Location = New System.Drawing.Point(245, 87)
        Me.chkEncrypt.Name = "chkEncrypt"
        Me.chkEncrypt.Size = New System.Drawing.Size(68, 17)
        Me.chkEncrypt.TabIndex = 4
        Me.chkEncrypt.Text = "Encrypt?"
        Me.chkEncrypt.UseVisualStyleBackColor = True
        '
        'btnGo
        '
        Me.btnGo.Location = New System.Drawing.Point(319, 81)
        Me.btnGo.Name = "btnGo"
        Me.btnGo.Size = New System.Drawing.Size(74, 26)
        Me.btnGo.TabIndex = 5
        Me.btnGo.Text = "Go"
        Me.btnGo.UseVisualStyleBackColor = True
        '
        'StatusPanel
        '
        Me.StatusPanel.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.StatusPanel.Location = New System.Drawing.Point(0, 125)
        Me.StatusPanel.Name = "StatusPanel"
        Me.StatusPanel.Size = New System.Drawing.Size(461, 22)
        Me.StatusPanel.TabIndex = 6
        Me.StatusPanel.Text = "StatusStrip1"
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(38, 17)
        Me.lblStatus.Text = "Ready"
        '
        'fileOpen
        '
        Me.fileOpen.RestoreDirectory = True
        '
        'fileSave
        '
        Me.fileSave.RestoreDirectory = True
        '
        'btnInBrowse
        '
        Me.btnInBrowse.Location = New System.Drawing.Point(399, 27)
        Me.btnInBrowse.Name = "btnInBrowse"
        Me.btnInBrowse.Size = New System.Drawing.Size(52, 23)
        Me.btnInBrowse.TabIndex = 7
        Me.btnInBrowse.Text = "Browse"
        Me.btnInBrowse.UseVisualStyleBackColor = True
        '
        'btnOutBrowse
        '
        Me.btnOutBrowse.Location = New System.Drawing.Point(399, 53)
        Me.btnOutBrowse.Name = "btnOutBrowse"
        Me.btnOutBrowse.Size = New System.Drawing.Size(52, 23)
        Me.btnOutBrowse.TabIndex = 8
        Me.btnOutBrowse.Text = "Browse"
        Me.btnOutBrowse.UseVisualStyleBackColor = True
        '
        'chkGsf
        '
        Me.chkGsf.AutoSize = True
        Me.chkGsf.Checked = True
        Me.chkGsf.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkGsf.Location = New System.Drawing.Point(127, 87)
        Me.chkGsf.Name = "chkGsf"
        Me.chkGsf.Size = New System.Drawing.Size(64, 17)
        Me.chkGsf.TabIndex = 9
        Me.chkGsf.Text = "Gsf file?"
        Me.chkGsf.UseVisualStyleBackColor = True
        '
        'frmManCrypt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(461, 147)
        Me.Controls.Add(Me.chkGsf)
        Me.Controls.Add(Me.btnOutBrowse)
        Me.Controls.Add(Me.btnInBrowse)
        Me.Controls.Add(Me.StatusPanel)
        Me.Controls.Add(Me.btnGo)
        Me.Controls.Add(Me.chkEncrypt)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtFileOut)
        Me.Controls.Add(Me.txtFileIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "frmManCrypt"
        Me.Text = "RYL files decrypt/encrypt"
        Me.StatusPanel.ResumeLayout(False)
        Me.StatusPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtFileIn As System.Windows.Forms.TextBox
    Friend WithEvents txtFileOut As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents chkEncrypt As System.Windows.Forms.CheckBox
    Friend WithEvents btnGo As System.Windows.Forms.Button
    Friend WithEvents StatusPanel As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents fileOpen As System.Windows.Forms.OpenFileDialog
    Friend WithEvents fileSave As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnInBrowse As System.Windows.Forms.Button
    Friend WithEvents btnOutBrowse As System.Windows.Forms.Button
    Friend WithEvents chkGsf As System.Windows.Forms.CheckBox

End Class
