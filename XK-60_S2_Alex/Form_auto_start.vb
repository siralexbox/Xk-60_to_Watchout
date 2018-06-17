Public Class Form_auto_start

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = My.Settings.Auto_load_timeline
    End Sub

    Private Sub Form2_Closing(sender As Object, e As EventArgs) Handles MyBase.Closing
        My.Settings.Auto_load_timeline = TextBox1.Text
    End Sub

End Class