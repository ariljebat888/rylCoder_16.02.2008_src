<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOptions))
        Me.txtRylFolder = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnSetRylFolder = New System.Windows.Forms.Button
        Me.btnSetFileTypes = New System.Windows.Forms.Button
        Me.chkFileTypeMcf = New System.Windows.Forms.CheckBox
        Me.chkFileTypeGsf = New System.Windows.Forms.CheckBox
        Me.chkFileTypeSkey = New System.Windows.Forms.CheckBox
        Me.chkFileTypeGcmds = New System.Windows.Forms.CheckBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnBrowse = New System.Windows.Forms.Button
        Me.dlgBrowse = New System.Windows.Forms.FolderBrowserDialog
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtRylFolder
        '
        Me.txtRylFolder.Location = New System.Drawing.Point(70, 12)
        Me.txtRylFolder.Name = "txtRylFolder"
        Me.txtRylFolder.Size = New System.Drawing.Size(278, 20)
        Me.txtRylFolder.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Ryl Folder:"
        '
        'btnSetRylFolder
        '
        Me.btnSetRylFolder.Location = New System.Drawing.Point(423, 12)
        Me.btnSetRylFolder.Name = "btnSetRylFolder"
        Me.btnSetRylFolder.Size = New System.Drawing.Size(58, 20)
        Me.btnSetRylFolder.TabIndex = 2
        Me.btnSetRylFolder.Text = "Set"
        Me.btnSetRylFolder.UseVisualStyleBackColor = True
        '
        'btnSetFileTypes
        '
        Me.btnSetFileTypes.Location = New System.Drawing.Point(115, 84)
        Me.btnSetFileTypes.Name = "btnSetFileTypes"
        Me.btnSetFileTypes.Size = New System.Drawing.Size(75, 23)
        Me.btnSetFileTypes.TabIndex = 3
        Me.btnSetFileTypes.Text = "Set file types"
        Me.btnSetFileTypes.UseVisualStyleBackColor = True
        '
        'chkFileTypeMcf
        '
        Me.chkFileTypeMcf.AutoSize = True
        Me.chkFileTypeMcf.Checked = True
        Me.chkFileTypeMcf.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFileTypeMcf.Location = New System.Drawing.Point(6, 19)
        Me.chkFileTypeMcf.Name = "chkFileTypeMcf"
        Me.chkFileTypeMcf.Size = New System.Drawing.Size(50, 17)
        Me.chkFileTypeMcf.TabIndex = 4
        Me.chkFileTypeMcf.Text = "*.mcf"
        Me.chkFileTypeMcf.UseVisualStyleBackColor = True
        '
        'chkFileTypeGsf
        '
        Me.chkFileTypeGsf.AutoSize = True
        Me.chkFileTypeGsf.Checked = True
        Me.chkFileTypeGsf.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFileTypeGsf.Location = New System.Drawing.Point(6, 42)
        Me.chkFileTypeGsf.Name = "chkFileTypeGsf"
        Me.chkFileTypeGsf.Size = New System.Drawing.Size(47, 17)
        Me.chkFileTypeGsf.TabIndex = 5
        Me.chkFileTypeGsf.Text = "*.gsf"
        Me.chkFileTypeGsf.UseVisualStyleBackColor = True
        '
        'chkFileTypeSkey
        '
        Me.chkFileTypeSkey.AutoSize = True
        Me.chkFileTypeSkey.Checked = True
        Me.chkFileTypeSkey.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFileTypeSkey.Location = New System.Drawing.Point(6, 65)
        Me.chkFileTypeSkey.Name = "chkFileTypeSkey"
        Me.chkFileTypeSkey.Size = New System.Drawing.Size(55, 17)
        Me.chkFileTypeSkey.TabIndex = 6
        Me.chkFileTypeSkey.Text = "*.skey"
        Me.chkFileTypeSkey.UseVisualStyleBackColor = True
        '
        'chkFileTypeGcmds
        '
        Me.chkFileTypeGcmds.AutoSize = True
        Me.chkFileTypeGcmds.Checked = True
        Me.chkFileTypeGcmds.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFileTypeGcmds.Location = New System.Drawing.Point(6, 88)
        Me.chkFileTypeGcmds.Name = "chkFileTypeGcmds"
        Me.chkFileTypeGcmds.Size = New System.Drawing.Size(64, 17)
        Me.chkFileTypeGcmds.TabIndex = 7
        Me.chkFileTypeGcmds.Text = "*.gcmds"
        Me.chkFileTypeGcmds.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkFileTypeMcf)
        Me.GroupBox1.Controls.Add(Me.btnSetFileTypes)
        Me.GroupBox1.Controls.Add(Me.chkFileTypeGcmds)
        Me.GroupBox1.Controls.Add(Me.chkFileTypeGsf)
        Me.GroupBox1.Controls.Add(Me.chkFileTypeSkey)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 47)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(196, 119)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "FileTypes"
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(354, 12)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(58, 20)
        Me.btnBrowse.TabIndex = 9
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'dlgBrowse
        '
        Me.dlgBrowse.Description = "Folder of your RYL installation"
        '
        'frmOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(493, 181)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSetRylFolder)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtRylFolder)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmOptions"
        Me.Text = "Options"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtRylFolder As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnSetRylFolder As System.Windows.Forms.Button
    Friend WithEvents btnSetFileTypes As System.Windows.Forms.Button
    Friend WithEvents chkFileTypeMcf As System.Windows.Forms.CheckBox
    Friend WithEvents chkFileTypeGsf As System.Windows.Forms.CheckBox
    Friend WithEvents chkFileTypeSkey As System.Windows.Forms.CheckBox
    Friend WithEvents chkFileTypeGcmds As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents dlgBrowse As System.Windows.Forms.FolderBrowserDialog
End Class
