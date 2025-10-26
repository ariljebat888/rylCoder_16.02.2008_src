<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cntTextEditItem
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
        Me.lblCommand = New System.Windows.Forms.Label
        Me.txtText = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblCommand
        '
        Me.lblCommand.AutoSize = True
        Me.lblCommand.Location = New System.Drawing.Point(0, -1)
        Me.lblCommand.Name = "lblCommand"
        Me.lblCommand.Size = New System.Drawing.Size(74, 13)
        Me.lblCommand.TabIndex = 0
        Me.lblCommand.Text = "Command row"
        '
        'txtText
        '
        Me.txtText.AcceptsReturn = True
        Me.txtText.Location = New System.Drawing.Point(0, 15)
        Me.txtText.Multiline = True
        Me.txtText.Name = "txtText"
        Me.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtText.Size = New System.Drawing.Size(324, 113)
        Me.txtText.TabIndex = 1
        Me.txtText.WordWrap = False
        '
        'cntTextEditItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.txtText)
        Me.Controls.Add(Me.lblCommand)
        Me.Name = "cntTextEditItem"
        Me.Size = New System.Drawing.Size(327, 145)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblCommand As System.Windows.Forms.Label
    Public WithEvents txtText As System.Windows.Forms.TextBox

End Class
