<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cntQuestPhaseLine
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
        Me.flowMain = New System.Windows.Forms.FlowLayoutPanel
        Me.SuspendLayout()
        '
        'flowMain
        '
        Me.flowMain.Location = New System.Drawing.Point(0, 0)
        Me.flowMain.Name = "flowMain"
        Me.flowMain.Size = New System.Drawing.Size(279, 21)
        Me.flowMain.TabIndex = 0
        Me.flowMain.WrapContents = False
        '
        'cntQuestPhaseLine
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.flowMain)
        Me.DoubleBuffered = True
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "cntQuestPhaseLine"
        Me.Size = New System.Drawing.Size(282, 21)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents flowMain As System.Windows.Forms.FlowLayoutPanel

End Class
