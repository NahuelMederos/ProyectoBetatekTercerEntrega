﻿
Imports Capa_Fisica

Module ControladorTiene

    Public Sub GuardarSintomasPersona(ci As String, idSintoma As String)
        Dim p As New Tiene

        p.CiPersona = ci
        p.IdSintoma = idSintoma


        p.GuardarSintomasDePersona()

    End Sub


End Module
