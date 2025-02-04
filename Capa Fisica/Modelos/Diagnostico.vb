﻿Public Class Diagnostico
    Inherits ModeloConexion

    Public Informacion As String
    Public Prioridad As String
    Public SolicitaChat As String
    Public IdDiagnostico As String
    Public CiPaciente As String
    Public NombreMedico As String
    Public IdSintoma As String
    Public IdPatologia As String
    Public Opcion As Integer

    Public Sub New(username As String, password As String)
        MyBase.New(username, password)

    End Sub

    Public Sub CrearDiagnostico()
        Comando.CommandText = "INSERT INTO DIAGNOSTICO VALUES(LAST_INSERT_ID(),'" + Me.Prioridad + "','" + Me.Informacion + "'," + Me.SolicitaChat + ")"

        Comando.ExecuteNonQuery()

    End Sub

    Public Sub EnviarDiagnosticoAMedico()
        Comando.CommandText = "INSERT INTO RECIBE VALUES(" + Me.IdDiagnostico + ",'Medico',1)"

        Comando.ExecuteNonQuery()

    End Sub

    Public Function UltimoDiagnostico()
        Me.Comando.CommandText = "SELECT MAX(IdDiagnostico) FROM Diagnostico"

        Return Comando.ExecuteScalar().ToString()
    End Function

    Public Function UltimoDiagnosticoEnRecibe()
        Me.Comando.CommandText = "SELECT MAX(IdDiagnostico) FROM Recibe"

        Return Comando.ExecuteScalar().ToString()
    End Function

    Public Function ObtenerDiagnosticos()

        Comando.CommandText = "Select Diagnostico.Prioridad,Recibe.IdDiagnostico As Id,Informacion,Genera.FechaHora,CiPersona
                               From Recibe,Diagnostico,Genera
                               Where Recibe.IdDiagnostico=Diagnostico.IdDiagnostico
                               And EstadoSesion=1
                               And Genera.IdDiagnostico=Diagnostico.IdDiagnostico
                               And (NombreMedico='" + Me.NombreMedico + "' Or NombreMedico='Medico')
                               Group by Genera.IdDiagnostico
                               Order by
                                      Case
                                         when Diagnostico.Prioridad= 'Alta' Then 1 
                                         when Diagnostico.Prioridad= 'Media' Then 2
                                         when Diagnostico.Prioridad= 'Baja' Then 3
                                        END;"

        Reader = Comando.ExecuteReader()
        Return Reader

    End Function

    Public Sub PacienteTieneSintomas()
        Comando.CommandText = "INSERT INTO TIENE VALUES('" + Me.CiPaciente + "','" + Me.IdSintoma + "')"

        Comando.ExecuteNonQuery()
    End Sub

    Public Sub PacienteGeneraDiagnostico()
        Comando.CommandText = "INSERT INTO GENERA VALUES('" + Me.CiPaciente + "',LAST_INSERT_ID(),'" + Me.IdSintoma + "',now())"

        Comando.ExecuteNonQuery()
    End Sub

    Public Sub ModificarNombreEnRecibe()
        Comando.CommandText = "UPDATE RECIBE SET NOMBREMEDICO='" + Me.NombreMedico + "' WHERE IDDIAGNOSTICO=" + Me.IdDiagnostico + ""

        Comando.ExecuteNonQuery()
    End Sub

    Public Sub AgregarPatologiaADiagnostico()
        Comando.CommandText = "INSERT INTO PATOLOGIAS VALUES(" + Me.IdDiagnostico + "," + Me.IdPatologia + ")"

        Comando.ExecuteNonQuery()

    End Sub

    Public Function ListarDiagnosticos()
        Comando.CommandText = "select Diagnostico.IdDiagnostico as Id,Informacion,Date(FechaHora) as Fecha
                              from diagnostico,Genera 
                              group by Id 
                              order by Id Desc;"

        Reader = Comando.ExecuteReader()
        Return Reader
    End Function

    Public Function ObtenerDiagnosticosAntiguos()
        If Me.Opcion = 1 Then
            Comando.CommandText = "Select Fecha,CONCAT( Nombre, "" "", Apellido ) AS Medico ,Informacion,Sesion
                                   From (select Sesion,date(Fechahora) as Fecha,Medico.Nombre As Nombre,Medico.Apellido As Apellido
                                   from persona,chatea,Medico
                                   where Ci=De 
                                   And chatea.Para=Medico.NombreUsuario
                                   And De='" + Me.CiPaciente + "' 
                                   and para!='Medico' 
                                   group by sesion) as A
                                   Join
                                   (Select * 
                                   From Diagnostico) as B
                                   On a.Sesion=B.IdDiagnostico
                                   Order by Fecha Desc"

            Reader = Comando.ExecuteReader()
            Return Reader
        Else
            Comando.CommandText = "Select Recibe.IdDiagnostico As Sesion,Diagnostico.Prioridad,Date(Genera.FechaHora) as Fecha,Informacion
                                   From Recibe,Diagnostico,Genera
                                   Where Recibe.IdDiagnostico=Diagnostico.IdDiagnostico
                                   And EstadoSesion=0
                                   And Genera.IdDiagnostico=Diagnostico.IdDiagnostico
                                   And NombreMedico='" + Me.NombreMedico + "'
                                   Group by Genera.IdDiagnostico
                                   Order by Fecha;"

            Reader = Comando.ExecuteReader()
            Return Reader
        End If
    End Function

End Class
