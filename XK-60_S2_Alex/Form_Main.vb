'' ########################################################
'' ############## Created by Alex Ramos  ##################
'' ########################################################
'' #### This program comes with absolutely no warranty ####
'' ########################################################
'' ############## For Bein Sports, Doha ###################
'' ########################################################
'' ###### alex@videocrew.pt ##################### 2018 ####
'' ########################################################
'' ########## It's all free to use and modify #############
'' ########################################################
''

'' WakeMeOnLan is Copyright (c) 2011 - 2018 Nir Sofer
'' More info and download on their site
'' https://www.nirsoft.net/utils/wake_on_lan.html

''------------------------------------------------------------------------------------------------------------------------------------
''------------------------------------------------  Some Help here  ------------------------------------------------------------------
''------------------------------------------------------------------------------------------------------------------------------------
'' Keep WakeMeOnLan in the same dir as this app
'' Aplication settings file is named "user.config"
'' Settings location "C:\Users\[user name]\AppData\Local\[appName]\"
''------------------------------------------------------------------------------------------------------------------------------------
'' SetBacklightLED(int ConnectedDevice, int ButtonID, int Bank, int State)
'' Button ID starts at 1 (for example, an XK-24 has buttons 1-24).
'' Bank is 0 (blue) or 1 (red). 
'' State is 0 = off; 1 = on; 2 = flash.
''------------------------------------------------------------------------------------------------------------------------------------
'' To send commands to Watchout call Send_to_watchout() function
'' To Run a Timeline in Cue Start call StartControlCue() function
'' To Stop a Timeline in Cue Stop call StopControlCue() function
'' and pass the command.
''-------------------------------------------------------------------------------------------------------------------------------------
'' Examples: 
'' Send_to_watchout("gotoControlCue " & Chr(34) & "Cue_Name" & Chr(34) & " false " & Chr(34) & "TimeLine_Name" & Chr(34) & vbCrLf)
'' Send_to_watchout("run " & Chr(34) & "TimeLine_Name" & Chr(34) & vbCrLf)
'' StartControlCue("TimeLine_Name")
'' ATTENTION  StartControlCue() and StopControlCue() will place the cursor at Start or Stop Cue and Run the Timeline
''-------------------------------------------------------------------------------------------------------------------------------------

Imports System.Net
Imports System.Threading
Imports System.IO

Public Class Form_Main

    Dim TCPClient As Sockets.TcpClient
    Dim TCPClientStream As Sockets.NetworkStream
    Dim ip As String = "127.0.0.1"
    Dim netPort As String = "3040"
    Dim cwd As String = My.Application.Info.DirectoryPath                                               ' gets current working directory of the app

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Xk60_80_1.SetBacklightIntensity(0, 255, 255)
        Xk60_80_1.SetFlashFrequency(0, 15)
        For i = 1 To 80
            Xk60_80_1.SetBacklightLED(0, i, 0, 0)                                                       ' makes all butons Blue LEDs on Xk60 to off state
            Xk60_80_1.SetBacklightLED(0, i, 1, 0)                                                       ' makes all butons Red LEDs on Xk60 to off state
        Next
        For Each check_box As Control In Me.Controls                                                    ' creates Handlers for the eventual click on checkbox links the click action with a Sub routine
            If (TypeOf check_box Is CheckBox) Then
                AddHandler check_box.Click, AddressOf CheckBox_pressed                                  ' if a click happens on one checkbox calls the Sub CheckBox_pressed and passes the CheckBox info
                Dim context_menu As New ContextMenuStrip With {.Name = check_box.Name,                  ' creates an ContextMenuStrip to add to each checkbox
                                                               .ShowCheckMargin = False,
                                                               .ShowImageMargin = False,
                                                               .AutoSize = True}
                Dim menu_item_1 As New ToolStripTextBox With {.Name = "button_name",                    ' adds textbox item to the ContextMenuStrip
                                                              .ToolTipText = "Button Text",
                                                              .Text = My.Settings.Item(check_box.Name & "_button_name"),
                                                              .AutoSize = False,
                                                              .Width = 360}
                Dim menu_item_2 As New ToolStripTextBox With {.Name = "timeline_name",                  ' adds textbox item to the ContextMenuStrip
                                                              .ToolTipText = "Timeline Name",
                                                              .Text = My.Settings.Item(check_box.Name & "_timeline_name"),
                                                              .AutoSize = False,
                                                              .Width = 360}
                Dim menu_item_3 As New ToolStripMenuItem With {.Name = check_box.Name,                  ' adds text item to the ContextMenuStrip for save button
                                                               .Text = "     --> Press here to save changes <--",
                                                               .ToolTipText = "Saves settings",
                                                               .Font = New Font("Courier New", 10, FontStyle.Bold),
                                                               .TextAlign = ContentAlignment.MiddleCenter,
                                                               .BackColor = Color.DarkSeaGreen,
                                                               .ForeColor = Color.Red,
                                                               .AutoSize = True}
                context_menu.Items.Add(menu_item_1)                                                     ' add item button name textbox ContextMenuStrip
                context_menu.Items.Add(menu_item_2)                                                     ' add timeline name textbox item ContextMenuStrip
                context_menu.Items.Add(menu_item_3)                                                     ' add save button item ContextMenuStrip
                check_box.ContextMenuStrip = context_menu                                               ' add ContextMenuStrip to ChechBox
                check_box.Text = My.Settings.Item(check_box.Name & "_button_name")                      ' loads checkbox name from my.settings
                check_box.Font = New Font("Microsoft Sans Serif", 8)                                    ' sets the font to 8
                check_box.TabIndex = check_box.Name.Remove(0, 8)                                        ' sets tab index to same as name number
                AddHandler menu_item_3.Click, AddressOf Contex_menu_save                                ' handles the save button
            End If
        Next
        Form_auto_prepare.TextBox1.Text = My.Settings.Auto_prepare_timeline                             'load timelines from setting to the textbox
        Form_auto_start.TextBox1.Text = My.Settings.Auto_load_timeline                                  'load timelines from setting to the textbox

        ''--------------------------- Delete the next lines to remove WakeMeOnLan not found error --------------------------------------- 
        Try
            Shell(cwd & "\WakeMeOnLan.exe /wakeupiprange 192.168.1.2 192.168.1.60")                       ' WakesUp display machines
        Catch EX As Exception
            MsgBox("!! WakeMeOnLan not found on " & cwd & "\WakeMeOnLan.exe !!      
                    Wake signal not send to displays...", MsgBoxStyle.Exclamation)                     ' WakeMeOnLan.exe must be on the root dir of the app
        End Try
        Thread.Sleep(2500)                                                                             ' wait 2.5 secounds and send WakesUp display machines again as safe measure
        Try
            Shell(cwd & "\WakeMeOnLan.exe /wakeupiprange 192.168.1.2 192.168.1.60")                       ' WakesUp display machines
        Catch EX As Exception
            MsgBox("!! WakeMeOnLan not found on " & cwd & "\WakeMeOnLan.exe !!      
                    Wake signal not send to displays...", MsgBoxStyle.Exclamation)                     ' WakeMeOnLan.exe must be on the root dir of the app
        End Try
        ''----------------------------- Stop deleting here you need the rest -------------------------------------------------------------
    End Sub
    ' action for the RIGHT clicked CheckBox, AddHandler defined on form1 MyBase.load
    Private Sub Contex_menu_save(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim button_name = sender.GetCurrentParent().Items(0).Text.ToString()                            ' gets the ToolStripTextBox input text
        Dim timeline_name = sender.GetCurrentParent().Items(1).Text.ToString()                          ' gets the ToolStripTextBox input text
        My.Settings.Item(sender.GetCurrentParent().Name.ToString & "_button_name") = button_name        ' adds buttton_name to corresponding setting line
        My.Settings.Item(sender.GetCurrentParent().Name.ToString & "_timeline_name") = timeline_name    ' adds the time_line to corresponding setting line
        My.Settings.Save()                                                                              ' saves settings
        CType(Me.Controls(sender.GetCurrentParent().name), CheckBox).Text = button_name                 ' CType converts the string to CheckBox type object to be able to change/access properties. Makes the button name show when you press SAVE
        'MsgBox(sender.GetCurrentParent().name, MsgBoxStyle.Critical) ' for degub           
    End Sub
    ' action for the clicked CheckBox  (soft button), AddHandler defined on form1 MyBase.load
    Private Sub CheckBox_pressed(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim key_pressed As String = sender.name.ToString().Remove(0, 8)                                 ' gets the name of the checked CheckBox and removes the first 8 charater, get just number ...01 ...21
        If sender.checked = True Then
            Call Xk60_80_1.SetBacklightLED(0, key_pressed, 1, 2)                                        ' makes Xk60 Key Red LED blink
            Call StartControlCue(sender.ContextMenuStrip.Items(1).Text)                                 ' sends the command to StartControlCue() with the ContextMenuStrip.Items(1) value Timeline name on the corresponding button
            'MsgBox(sender.ContextMenuStrip.Items(1).Text & " for debug", MsgBoxStyle.Critical)         ' for debug
        Else
            Call Xk60_80_1.SetBacklightLED(0, key_pressed, 1, 0)                                        ' makes Xk60 Key Red LED blink Stop
            Call StopControlCue(sender.ContextMenuStrip.Items(1).Text)                                  ' sends the command to StartControlCue() with the ContextMenuStrip.Items(1) value Timeline name on the corresponding button
            'MsgBox(sender.ContextMenuStrip.Items(1).Text & " for debug", MsgBoxStyle.Critical)         ' for debug
        End If
    End Sub
    ' Reads when a KEY is pressed on the Xk60 controller action for the KEY, AddHandler defined on form1 MyBase.load
    Private Sub Xk60_80_1_ButtonChange(ByVal e As XK_60_80.XKeyEventArgs) Handles Xk60_80_1.ButtonChange
        Dim key_pressed = e.CID.ToString().Remove(0, 2)                                                 ' converts CID to string and removes first 2 digits left to right. Ex: 1001 to 01
        Dim soft_key As CheckBox = Me.Controls.OfType(Of CheckBox).First(Function(c) c.Name = "CheckBox" & key_pressed) ' searchs form1 for a checkbox named CheckBox + number from key_pressed
        If e.PressState = True AndAlso soft_key.Checked = False Then                                    ' button is pressed
            soft_key.Checked = True                                                                     ' sets the checkbox (soft button) found on the previous line to checked 
            Call Xk60_80_1.SetBacklightLED(0, key_pressed, 1, 2)                                        ' makes Xk60 Key Red LED blink
            Call StartControlCue(soft_key.ContextMenuStrip.Items(1).Text)                               ' sends the command to StartControlCue() with the ContextMenuStrip.Items(1) value Timeline name on the corresponding button
            soft_key.ForeColor = Color.White                                                            ' changes the button (soft button) text color to white
            'MsgBox(soft_key.ContextMenuStrip.Items(1).Text & " for debug", MsgBoxStyle.Critical)       ' for debug
        ElseIf e.PressState = True AndAlso soft_key.Checked = True Then                                 ' button is pressed
            soft_key.Checked = False                                                                    ' sets the checkbox (soft button) to not checked
            Call Xk60_80_1.SetBacklightLED(0, key_pressed, 1, 0)                                        ' makes Xk60 Key Red LED blink Stop
            Call StopControlCue(soft_key.ContextMenuStrip.Items(1).Text)                                ' sends the command to StartControlCue() with the ContextMenuStrip.Items(1) value Timeline name on the corresponding button
            soft_key.ForeColor = Color.Black                                                            ' changes the button (soft button) text color to black
            'MsgBox(soft_key.ContextMenuStrip.Items(1).Text & " for debug", MsgBoxStyle.Critical)       ' for debug
        End If
    End Sub
    '' Sends the commands to watchout with the sms received parameter
    Private Sub Send_to_watchout(ByVal sms As String)
        Try
            TCPClient = New Sockets.TcpClient(ip, netPort)
            TCPClientStream = TCPClient.GetStream()
            Dim sendbytes() As Byte = System.Text.Encoding.ASCII.GetBytes(sms & vbCrLf)
            TCPClient.Client.Send(sendbytes)
            Me.MenuStrip1.Items.Item(3).Text = "online"                                                  ' online offline icon on top bar
            Me.MenuStrip1.Items.Item(3).Image = My.Resources.Green_Light_Icon                            ' online offline icon on top bar
            If Form_messages.Visible = True Then
                Form_messages.TextBox1.Text += (DateTime.Now & vbNewLine & sms & vbCrLf)                 'send TCP sms to message form
            End If
        Catch ex As Exception
            MsgBox("Watchout not found...
                    Start Watchout and try again", MsgBoxStyle.Information)
            Me.MenuStrip1.Items.Item(3).Text = "offline"                                                 ' online offline icon on top bar
            Me.MenuStrip1.Items.Item(3).Image = My.Resources.Red_Light_Icon                              ' online offline icon on top bar
        End Try
    End Sub
    ' formats string to Goto Cue and Start Aux timeline, Timeline name is passed by sms argument
    Private Sub StartControlCue(ByVal sms As String)
        If sms <> "" Then                                                                               ' if sms if empty doent send command to watchout, prevents Message timeline error on Watchout
            Call Send_to_watchout("gotoControlCue " & Chr(34) & "Start" & Chr(34) & " false " & Chr(34) & sms & Chr(34) & vbCrLf &
                                  "run " & Chr(34) & sms & Chr(34) & vbCrLf)
        End If
    End Sub
    ' formats string to Goto Cue and Stop Aux timeline, Timeline name is passed by sms argument
    Private Sub StopControlCue(ByVal sms As String)
        If sms <> "" Then                                                                               ' if sms if empty doent send command to watchout, prevents Message timeline error on Watchout
            Call Send_to_watchout("gotoControlCue " & Chr(34) & "Stop" & Chr(34) & " false " & Chr(34) & sms & Chr(34) & vbCrLf &
                                  "run " & Chr(34) & sms & Chr(34) & vbCrLf)
        End If
    End Sub
    ' startup setting for the studio
    Private Sub Studio_startup()
        Call Send_to_watchout("online" & " true" & vbCrLf)                                              ' sets watchout online
        'Thread.Sleep(1000)                                                                             ' wait 1 second after going Online, before sending commands
        For Each line In Form_auto_start.TextBox1.Lines
            If line <> "" Then                                                                          ' only sends the lines with text
                'MsgBox(line, MsgBoxStyle.Critical)                                                     ' for debug
                Call StartControlCue(line)
            End If
        Next
        For Each line In Form_auto_prepare.TextBox1.Lines
            If line <> "" Then                                                                          ' only sends the lines with text
                'MsgBox(line, MsgBoxStyle.Critical)                                                     ' for debug
                Call StopControlCue(line)
            End If
        Next
    End Sub
    ' timer to allow display machines to startup and then start the show
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If ProgressBar1.Value = 120 Then
            Timer1.Enabled = False
            ProgressBar1.Visible = False
            Label2.Visible = False
            Call Studio_startup()                                                                        ' all initial settings and timelines to startwith the studio go here
        End If
        If ProgressBar1.Value < 120 Then
            ProgressBar1.Value = ProgressBar1.Value + 1
        End If
    End Sub
    ' shortcut to bypass timer, just press on GUI ProgressBar
    Private Sub ProgressBar1_Click(sender As Object, e As EventArgs) Handles ProgressBar1.Click
        Timer1.Interval = 1
    End Sub
    ' shows auto load timelines form
    Private Sub DefaultStartupItensToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultStartupItensToolStripMenuItem.Click
        Form_auto_start.Show()
    End Sub
    ' shows auto prepare timelines form
    Private Sub AutoPrepareTimeLinesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoPrepareTimeLinesToolStripMenuItem.Click
        Form_auto_prepare.Show()
    End Sub
    ' shows messages window
    Private Sub Messages_Click(sender As Object, e As EventArgs) Handles Messages.Click
        Form_messages.Show()
    End Sub
    ' shows about form
    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles About.Click
        Form_about.Show()
    End Sub
    ' online button, only to go online NOT offline
    Private Sub online_offline_Click(sender As Object, e As EventArgs) Handles online_offline.Click
        Studio_startup()
    End Sub
    ' on closing and exiting the app
    Private Sub Form_Main_Closed(sender As Object, e As EventArgs) Handles Me.FormClosed
        For i = 1 To 80
            Xk60_80_1.SetBacklightLED(0, i, 0, 0)                                                        ' makes all butons Blue LEDs on Xk60 to off state
            Xk60_80_1.SetBacklightLED(0, i, 1, 0)                                                        ' makes all butons Red LEDs on Xk60 to off state
        Next
        'TCPClient.Client.Disconnect(True)                                                                ' closes TCP connection
        GC.Collect()
        Thread.Sleep(1000)                                                                               ' gives time to the previous code to run before closing                             
    End Sub

End Class
