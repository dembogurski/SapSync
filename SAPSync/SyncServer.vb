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
    Private SAPConn As Dictionary(Of String, String)
    Private SAPSync As SAPSync
    Private context As HttpListenerContext
    Private response As Stream
    Private listener As HttpListener
    Private action As String
    Private params As Dictionary(Of String, String)

    ' Inicializar variables
    Public Sub New(MyConnStr As String, MSConnStr As String, SAPConn As SAPbobsCOM.Company)
        'Dim MySQLConnectionStringMarijoa As String = "server=192.168.2.220; port=3306;user=sync; password=case; database=marijoa_sap"
        'Dim MSSQLConnectionStringMarijoa As String = "Data Source=192.168.2.220;Initial Catalog=MARIJOASA_PRUEBA;Persist Security Info=True;User ID=sa;Password=Marijoa123."
        Me.listener = New HttpListener()
        Me.SAPSync = New SAPSync(MyConnStr, MSConnStr, SAPConn)
    End Sub
    ' Iniciar Servidor
    Public Sub start()
        'Me.SAPSync.connectToSAP()

        Debug.WriteLine("Preparando Servidor")
        Me.listener.Prefixes.Add("http://*:8081/")
        Me.listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous
        Me.listener.Start()
        While True
            Try
                ProcessRequest()
            Catch ex As Exception

            End Try
        End While
    End Sub
    Private Sub ProcessRequest()
        Dim startNew = Stopwatch.StartNew()
        Dim result As IAsyncResult = Me.listener.BeginGetContext(AddressOf ListenerCallback, Me.listener)
        result.AsyncWaitHandle.WaitOne()
        result.AsyncWaitHandle.Close()

    End Sub
    Private Sub ListenerCallback(result As IAsyncResult)
        Dim ct As HttpListenerContext = Me.listener.EndGetContext(result)
        Dim b_respuesta As Byte()
        Dim request As HttpListenerRequest = ct.Request
        Dim charToTrim As String = "/,? "
        Dim rawRequest As String = request.RawUrl.Trim(charToTrim.ToCharArray())
        Dim params As String() = rawRequest.Split(New String() {"&"}, StringSplitOptions.RemoveEmptyEntries)
        Dim _request As New Dictionary(Of String, String)
        Dim response = ct.Response.OutputStream
        Dim errorStatus As Boolean = False
        Dim respuesta As Dictionary(Of String, String) = New Dictionary(Of String, String)

        respuesta("status") = "ok"
        respuesta("msj") = ""

        If params.Length > 0 Then
            For Each data As String In params
                Dim rq As String() = data.Split("=")
                Try
                    _request.Add(rq(0), rq(1))
                Catch ex As Exception
                    Debug.WriteLine("Error!: " + ex.StackTrace)
                    respuesta("status") = "error"
                    respuesta("msj") = ex.StackTrace
                    errorStatus = True
                End Try
            Next

            If _request.ContainsKey("action") And Not errorStatus Then
                Debug.WriteLine(String.Format("Accion: {0}", _request("action")))
                Dim json As String = ""
                Try
                    respuesta = CallByName(SAPSync, _request("action"), Microsoft.VisualBasic.CallType.Method, _request)
                Catch ex As Exception
                    Debug.WriteLine("Error llamada funcion {0}, Source: {2}, StackTrace {1}", _request("action"), ex.StackTrace, ex.Source)
                    respuesta("status") = "error"
                    If String.Compare(ex.Source.ToString(), "System.MissingMemberException") = 0 Then
                        respuesta("msj") = String.Format("Funcion {0} no existe", _request("action"))
                    Else
                        respuesta("msj") = ex.StackTrace
                    End If
                    errorStatus = True
                End Try


                If Not errorStatus Then
                    If _request.ContainsKey("callback") Then
                        respuesta("callback") = _request("callback")
                        json = Me.dicToJSON(respuesta)
                    Else
                        json = Me.dicToJSON(respuesta)
                    End If
                    respuesta("msj") = json
                End If
            End If
            b_respuesta = System.Text.Encoding.UTF8.GetBytes(respuesta("msj"))
            response.Write(b_respuesta, 0, b_respuesta.Length)
            Thread.Sleep(1000)
            ct.Response.StatusCode = 200
            ct.Response.StatusDescription = respuesta("status")
            ct.Response.Close()
        End If
    End Sub
   
    ' Convertir Dictionary String, String a JSON String
    ' Agregar al parametro el callback si lo tiene
    Private Function dicToJSON(data As Dictionary(Of String, String)) As String
        Dim json As String = "{"

        For Each respuesta As KeyValuePair(Of String, String) In data
            If respuesta.Key <> "callback" Then
                json &= """" & respuesta.Key & """:""" & respuesta.Value & ""","
            End If
        Next

        json = json.Trim(New Char() {","}) & "}"

        If data.ContainsKey("callback") Then
            json = data("callback") & "(" & json & ")"
        End If

        Return json
    End Function
End Class
