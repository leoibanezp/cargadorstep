Namespace DTE3TableAdapters


    Partial Public Class sp_STEP_carga_DeltaTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class


    Partial Public Class sp_STEP_carga_CatalogoTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class STEP_repuestoTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class
    Partial Public Class STEP_repuesto_PropiedadTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class STEP_SKUTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class STEP_logTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class STEP_catalogo_cargaTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

End Namespace
