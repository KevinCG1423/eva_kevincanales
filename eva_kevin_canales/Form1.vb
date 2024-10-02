Imports MySql.Data.MySqlClient

Public Class Form1
    Private connectionString As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connectionString = "Server=localhost;Database=registropersonas;Uid=root;Pwd=;"
        LoadComunAs()
    End Sub

    Private Sub LoadComunAs()
        Using conn As New MySqlConnection(connectionString)
            Dim cmd As New MySqlCommand("SELECT NombreComuna FROM comuna", conn)
            Try
                conn.Open()
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    ComboBox1.Items.Add(reader("NombreComuna").ToString())
                End While
            Catch ex As Exception
                MessageBox.Show("Error al cargar comunas: " & ex.Message)
            Finally
                conn.Close()
            End Try
        End Using
    End Sub

    Private Function ValidateFields() As Boolean
        If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
           String.IsNullOrWhiteSpace(TextBox2.Text) OrElse
           String.IsNullOrWhiteSpace(TextBox3.Text) OrElse
           ComboBox1.SelectedItem Is Nothing Then
            MessageBox.Show("Por favor, complete todos los campos obligatorios.")
            Return False
        End If
        Return True
    End Function

    Private Sub ClearFields()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        RadioButton1.Checked = False
        RadioButton2.Checked = False
        RadioButton3.Checked = False
        ComboBox1.SelectedIndex = -1
        TextBox5.Clear()
        TextBox6.Clear()
        TextBox1.Focus()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ValidateFields() Then
            Dim rut = TextBox1.Text
            Dim nombre = TextBox2.Text
            Dim apellido = TextBox3.Text
            Dim sexo = If(RadioButton1.Checked, "Masculino", If(RadioButton2.Checked, "Femenino", "No especifica"))
            Dim comuna = ComboBox1.SelectedItem.ToString()
            Dim ciudad = TextBox5.Text
            Dim observacion = TextBox6.Text

            Using conn As New MySqlConnection(connectionString)
                Dim cmd As New MySqlCommand("INSERT INTO personas (RUT, Nombre, Apellido, Sexo, Comuna, Ciudad, Observacion) VALUES (@RUT, @Nombre, @Apellido, @Sexo, @Comuna, @Ciudad, @Observacion)", conn)
                cmd.Parameters.AddWithValue("@RUT", rut)
                cmd.Parameters.AddWithValue("@Nombre", nombre)
                cmd.Parameters.AddWithValue("@Apellido", apellido)
                cmd.Parameters.AddWithValue("@Sexo", sexo)
                cmd.Parameters.AddWithValue("@Comuna", comuna)
                cmd.Parameters.AddWithValue("@Ciudad", ciudad)
                cmd.Parameters.AddWithValue("@Observacion", observacion)

                Try
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Registro guardado exitosamente.")
                    ClearFields()
                Catch ex As Exception
                    MessageBox.Show("Error al guardar registro: " & ex.Message)
                Finally
                    conn.Close()
                End Try
            End Using
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim rut = TextBox1.Text

        If String.IsNullOrWhiteSpace(rut) Then
            MessageBox.Show("Por favor, ingrese un RUT para buscar.")
            Return
        End If

        Using conn As New MySqlConnection(connectionString)
            Dim cmd As New MySqlCommand("SELECT * FROM personas WHERE RUT = @RUT", conn)
            cmd.Parameters.AddWithValue("@RUT", rut)

            Try
                conn.Open()
                Dim reader = cmd.ExecuteReader()
                If reader.Read() Then
                    TextBox2.Text = reader("Nombre").ToString()
                    TextBox3.Text = reader("Apellido").ToString()
                    If reader("Sexo").ToString() = "Masculino" Then
                        RadioButton1.Checked = True
                    ElseIf reader("Sexo").ToString() = "Femenino" Then
                        RadioButton2.Checked = True
                    Else
                        RadioButton3.Checked = True
                    End If
                    ComboBox1.SelectedItem = reader("Comuna").ToString()
                    TextBox5.Text = reader("Ciudad").ToString()
                    TextBox6.Text = reader("Observacion").ToString()
                Else
                    MessageBox.Show("RUT no encontrado.")
                End If
            Catch ex As Exception
                MessageBox.Show("Error al buscar registro: " & ex.Message)
            Finally
                conn.Close()
            End Try
        End Using
    End Sub
End Class
