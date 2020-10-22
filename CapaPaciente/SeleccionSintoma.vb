﻿Imports Capa_Logica

Public Class SeleccionSintoma

    Private Sub SeleccionSintoma_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim tabla As New DataTable
        tabla.Load(ControladorSintoma.ListarNombreSintomas)
        GrillaSintomas.DataSource = tabla
    End Sub

    'Son declarados en private para que se mantengan sus valores despues de ser usados por el boton seleccionar'
    Private contador As Integer = 0
    Private nombreList(contador)

    Private contador2 As Integer = 0
    Private PosiblePatologiaList(contador2)

    Private contador3 As Integer = 0
    Private PatologiasSeguras(contador3)

    'presult nos deja ver los sintomas que se seleccionaron'
    Private presult As String

    'Nos permite ver todas las patologias posibles en un array'
    Private OtrasPatologiasResult As String

    Private Sub btnSeleccionarSintoma_Click_1(sender As Object, e As EventArgs) Handles btnSeleccionarSintoma.Click


        Dim nombre As String

        'Por cada item seleccionado en el datagrid, se guardan los nombres en el array nombreList'
        For Each selectedItem As DataGridViewRow In GrillaSintomas.Rows
            Dim chk As DataGridViewCheckBoxCell = selectedItem.Cells(Check.Name)
            If chk.Value IsNot Nothing AndAlso chk.Value = True Then
                nombre = selectedItem.Cells("NOMBRE").Value
                ReDim Preserve nombreList(contador)
                nombreList(contador) = nombre
                contador += 1
            End If

        Next selectedItem


        Dim TablaOtrasPatologias As New DataTable
        TablaOtrasPatologias.Load(ControladorAsociar.ObtenerOtrasPatologias(nombreList))
        GrillaOtrasPatologias.DataSource = TablaOtrasPatologias


        Dim PosiblePatologia As String

        'Crea un array con todas las posibles patologias'
        For Each Item As DataGridViewRow In GrillaOtrasPatologias.Rows
            PosiblePatologia = Item.Cells("NOMBRE").Value
            ReDim Preserve PosiblePatologiaList(contador2)
            PosiblePatologiaList(contador2) = PosiblePatologia
            contador2 += 1
        Next

        'Compara la cantidad de sintomas totales que tiene una patologia, con los sintomas seleccionados por el usuario para cada patologia en la lista "Otras posibles patologias"'
        'En el caso de que la cantidad sea la misma, se agrega la patologia a un nuevo array llamado PatologiasSeguras'
        For Each PosiblePat As String In PosiblePatologiaList
            Dim Cuenta1 As Integer = Convert.ToInt32(ControladorAsociar.ObtenerPatologiasCompletas(PosiblePat))
            Dim Cuenta2 As Integer = Convert.ToInt32(ControladorAsociar.ObtenerAparicionesdePatologiaenBusqueda(PosiblePat, nombreList))
            If (Cuenta1 = Cuenta2) Then
                ReDim Preserve PatologiasSeguras(contador3)
                PatologiasSeguras(contador3) = PosiblePat
                contador3 += 1
            End If

        Next

        Dim ResultadoFinal As String = ""
        Dim StringSintomas As String = ""
        Dim Prio As String
        Dim PrioridadDiagnostico As String = "Baja"


        'Por cada Patologia en PatologiasSeguras se agrega al String ResultadoFinal y se utiliza la prioridad mas alta para el diagnostico'
        For Each Patologia As String In PatologiasSeguras
            ResultadoFinal &= Patologia + ", "
            Try
                Prio = ControladorPatologia.ObtenerPrioridad(Patologia)
                If Prio = "Alta" Then
                    PrioridadDiagnostico = "Alta"
                ElseIf Prio = "Media" And PrioridadDiagnostico IsNot "Alta" Then
                    PrioridadDiagnostico = "Media"
                End If
            Catch
            End Try
        Next


        If ResultadoFinal = ", " Then
            ResultadoFinal = "No hay ningun diagnostico seguro."
        Else
            ResultadoFinal = "Las posibles patologias segun los sintomas seleccionados son: " + ResultadoFinal
            ResultadoFinal = ResultadoFinal.Remove(ResultadoFinal.Length - 4) + "."

        End If

        Dim IdSintomasList As New List(Of String)

        For Each Sint In nombreList
            StringSintomas &= Sint + ", "
            IdSintomasList.Add(ControladorSintoma.ObtenerIdSintoma(Sint))
        Next

        StringSintomas = StringSintomas.Remove(StringSintomas.Length - 2) + "."


        Dim SolicitarChat As DialogResult
        SolicitarChat = MessageBox.Show(ResultadoFinal + Environment.NewLine + Environment.NewLine + "        ¿Desea solicitar un chat con un medico?", "Solicitar Chat", MessageBoxButtons.YesNo)
        If SolicitarChat = DialogResult.No Then
            ControladorDiagnostico.CrearDiagnostico(StringSintomas, ResultadoFinal, Sesion.CI, PrioridadDiagnostico, "False", IdSintomasList)
        Else
            ControladorDiagnostico.CrearDiagnostico(StringSintomas, ResultadoFinal, Sesion.CI, PrioridadDiagnostico, "True", IdSintomasList)
            ChatPaciente.Diagnostico = ResultadoFinal
            ChatPaciente.Prioridad = PrioridadDiagnostico
            ChatPaciente.Sintomas = StringSintomas
            ChatPaciente.Visible = True
            Me.Close()
        End If


        btnSeleccionarSintoma.Enabled = False
    End Sub


    Private Sub btnSolicitarChat_Click(sender As Object, e As EventArgs) Handles btnSolicitarChat.Click
        ChatPaciente.Visible = True
        Me.Close()
    End Sub


End Class
