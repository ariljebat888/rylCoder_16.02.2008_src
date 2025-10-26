<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMap
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMap))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.OK_Button = New System.Windows.Forms.Button
        Me.picMap = New System.Windows.Forms.PictureBox
        Me.txtLocationY = New System.Windows.Forms.TextBox
        Me.txtLocationX = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.flowMapThumb = New System.Windows.Forms.FlowLayoutPanel
        Me.pnlMap = New System.Windows.Forms.Panel
        Me.btnDeleteMulti = New System.Windows.Forms.Button
        Me.sldZoom = New System.Windows.Forms.TrackBar
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.picMap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.pnlMap.SuspendLayout()
        CType(Me.sldZoom, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(646, 495)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(133, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(69, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(60, 22)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(60, 22)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'picMap
        '
        Me.picMap.Image = CType(resources.GetObject("picMap.Image"), System.Drawing.Image)
        Me.picMap.InitialImage = Nothing
        Me.picMap.Location = New System.Drawing.Point(0, 0)
        Me.picMap.Name = "picMap"
        Me.picMap.Size = New System.Drawing.Size(512, 512)
        Me.picMap.TabIndex = 1
        Me.picMap.TabStop = False
        '
        'txtLocationY
        '
        Me.txtLocationY.AcceptsReturn = True
        Me.txtLocationY.Location = New System.Drawing.Point(76, 19)
        Me.txtLocationY.MaxLength = 12
        Me.txtLocationY.Multiline = True
        Me.txtLocationY.Name = "txtLocationY"
        Me.txtLocationY.Size = New System.Drawing.Size(50, 20)
        Me.txtLocationY.TabIndex = 4
        '
        'txtLocationX
        '
        Me.txtLocationX.AcceptsReturn = True
        Me.txtLocationX.Location = New System.Drawing.Point(20, 19)
        Me.txtLocationX.MaxLength = 12
        Me.txtLocationX.Multiline = True
        Me.txtLocationX.Name = "txtLocationX"
        Me.txtLocationX.Size = New System.Drawing.Size(50, 20)
        Me.txtLocationX.TabIndex = 3
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtLocationX)
        Me.GroupBox1.Controls.Add(Me.txtLocationY)
        Me.GroupBox1.Location = New System.Drawing.Point(641, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(146, 53)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Location"
        '
        'flowMapThumb
        '
        Me.flowMapThumb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.flowMapThumb.AutoScroll = True
        Me.flowMapThumb.AutoScrollMargin = New System.Drawing.Size(1, 1)
        Me.flowMapThumb.AutoScrollMinSize = New System.Drawing.Size(1, 1)
        Me.flowMapThumb.Location = New System.Drawing.Point(12, 12)
        Me.flowMapThumb.Name = "flowMapThumb"
        Me.flowMapThumb.Size = New System.Drawing.Size(105, 489)
        Me.flowMapThumb.TabIndex = 6
        '
        'pnlMap
        '
        Me.pnlMap.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlMap.Controls.Add(Me.picMap)
        Me.pnlMap.Location = New System.Drawing.Point(123, 12)
        Me.pnlMap.Name = "pnlMap"
        Me.pnlMap.Size = New System.Drawing.Size(512, 512)
        Me.pnlMap.TabIndex = 8
        '
        'btnDeleteMulti
        '
        Me.btnDeleteMulti.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteMulti.Location = New System.Drawing.Point(648, 71)
        Me.btnDeleteMulti.Name = "btnDeleteMulti"
        Me.btnDeleteMulti.Size = New System.Drawing.Size(125, 21)
        Me.btnDeleteMulti.TabIndex = 9
        Me.btnDeleteMulti.Text = "Delete selected"
        Me.btnDeleteMulti.UseVisualStyleBackColor = True
        Me.btnDeleteMulti.Visible = False
        '
        'sldZoom
        '
        Me.sldZoom.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.sldZoom.Location = New System.Drawing.Point(12, 507)
        Me.sldZoom.Maximum = 12
        Me.sldZoom.Minimum = 1
        Me.sldZoom.Name = "sldZoom"
        Me.sldZoom.Size = New System.Drawing.Size(105, 45)
        Me.sldZoom.TabIndex = 10
        Me.sldZoom.TickStyle = System.Windows.Forms.TickStyle.None
        Me.sldZoom.Value = 1
        '
        'frmMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(798, 536)
        Me.Controls.Add(Me.sldZoom)
        Me.Controls.Add(Me.btnDeleteMulti)
        Me.Controls.Add(Me.pnlMap)
        Me.Controls.Add(Me.flowMapThumb)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMap"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Map"
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.picMap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.pnlMap.ResumeLayout(False)
        CType(Me.sldZoom, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents picMap As System.Windows.Forms.PictureBox
    Friend WithEvents txtLocationY As System.Windows.Forms.TextBox
    Friend WithEvents txtLocationX As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents flowMapThumb As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents pnlMap As System.Windows.Forms.Panel
    Friend WithEvents btnDeleteMulti As System.Windows.Forms.Button
    Friend WithEvents sldZoom As System.Windows.Forms.TrackBar

End Class
