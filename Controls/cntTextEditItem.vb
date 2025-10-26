Public Class cntTextEditItem
    Private iTextitem As CNpcParser.NPCTextItem
    Public Event NPCTextChanged(ByRef sender As cntTextEditItem, ByVal line As CNpcParser.NPCTextItem, ByVal newText As String)
    Public Property NPCText() As String
        Get
            If Me.txtText.Lines.Length > 0 Then
                Dim lines() As String = Me.txtText.Lines
                If Trim(lines(UBound(lines))) = vbNewLine Then Array.Resize(lines, lines.Length - 1)
                Return String.Join("\\", lines)
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            Dim lines() As String = value.Split(New String() {"\\"}, StringSplitOptions.None)
            Me.txtText.Lines = lines
        End Set
    End Property

    Public Property Command() As String
        Get
            Return Me.lblCommand.Text
        End Get
        Set(ByVal value As String)
            Me.lblCommand.Text = Trim(value)
        End Set
    End Property

    Public Property TextItem() As CNpcParser.NPCTextItem
        Get
            Return iTextitem
        End Get
        Set(ByVal value As CNpcParser.NPCTextItem)
            NPCText() = value.text
            iTextitem = value
        End Set
    End Property
    Public Property Small() As Boolean
        Get
            Return Not Me.txtText.Multiline
        End Get
        Set(ByVal value As Boolean)
            If value Then
                Dim txtheight As Integer = Me.txtText.Size.Height
                Me.txtText.Multiline = False
                Me.txtText.ScrollBars = ScrollBars.None
                Me.Size = New Size(Me.Size.Width, Me.Size.Height - (txtheight - Me.txtText.Size.Height))
            End If
        End Set
    End Property
    Private Sub txtText_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtText.TextChanged
        RaiseEvent NPCTextChanged(Me, TextItem, NPCText)
    End Sub
End Class
