Public Class Form_auto_prepare

    Private Sub Form_auto_prepare_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = My.Settings.Auto_prepare_timeline
    End Sub

    Private Sub Form_auto_prepare_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        My.Settings.Auto_prepare_timeline = TextBox1.Text
    End Sub

End Class