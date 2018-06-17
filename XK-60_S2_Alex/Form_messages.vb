Public Class Form_messages
    Private Sub Form_messages_Load(sender As Object, e As EventArgs) Handles MyBase.Closing
        GC.Collect()
        Threading.Thread.Sleep(500)
    End Sub

End Class