Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports System.Web
Imports System.Collections.Generic
Imports SAPSync

Public Class SyncServer
    Public Sub start()
        Dim listener As HttpListener = New HttpListener()
        Dim context As HttpListenerContext

        Try
            Debug.WriteLine("Preparando Servidor")
            listener.Prefixes.Add("http://*:8081/")
            If Not listener.IsListening Then
                listener.Start()
                context = listener.GetContext()
                Debug.WriteLine("Servidor Iniciado")
                While listener.IsListening
                    context = listener.GetContext()
                    '' Dim data_text As StreamReader = New StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd()
                    Dim request As HttpListenerRequest = context.Request
                    Dim charToTrim As String = "/,? "
                    Dim rawRequest As String = request.RawUrl.Trim(charToTrim.ToCharArray())
                    Dim params As String() = rawRequest.Split(New String() {"&"}, StringSplitOptions.RemoveEmptyEntries)
                    Dim _request As New Dictionary(Of String, String)

                    If params.Length > 0 Then
                        For Each data As String In params
                            Dim rq As String() = data.Split("=")
                            _request.Add(rq(0), rq(1))

                        Next
                        If _request.ContainsKey("action") Then
                            Debug.WriteLine(String.Format("Accion: {0}", _request("action")))
                            ' Clase
                            Dim magicType As Type = Type.GetType("SAPSync")
                            Dim magicConstructor As ConstructorInfo = magicType.GetConstructor(Type.EmptyTypes)
                            Dim magicClassObject As Object = magicConstructor.Invoke(New Object() {})
                            ' Metodo
                            Dim magicMethod As MethodInfo = magicType.GetMethod(_request("action"))
                            ' Dim magicValue As Object = magicMethod.Invoke(magicClassObject, New Object() {100})
                            Dim magicValue As Object = magicMethod.Invoke(magicClassObject, Nothing)
                        End If
                    End If
                End While
                context.Response.StatusCode = 200
                context.Response.StatusDescription = "Ok"
                context.Response.Close()
            Else
                Debug.WriteLine("No Iniciado")
            End If
        Catch ex As Exception
            Debug.WriteLine("Error!: " + ex.StackTrace)
        End Try
    End Sub
End Class
