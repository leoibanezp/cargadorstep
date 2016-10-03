Namespace GestionTableAdapters

    Partial Public Class sp_InsertaMP_cargadorDinamicoTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class MaestroProducto_cargadorTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class maestroProducto_paisOrigenTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class sp_actualiza_MaestroProducto_STEPTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class

    Partial Public Class sp_carga_MaestroProducto_STEPTableAdapter

        Public Sub SeteaCommandTimeout(ByVal valor As Integer)
            For i As Integer = 0 To Me.CommandCollection.Length - 1
                If Me.CommandCollection(i) IsNot Nothing Then
                    Me.CommandCollection(i).CommandTimeout = valor
                End If
            Next
        End Sub

    End Class


End Namespace
