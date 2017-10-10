Imports MySql.Data.MySqlClient
Imports System.Data.SqlClient
Imports System.Web
' Clase para llamada asincrona a traves del SyncServer
' Las funciones agregadas reciben una variable REQUEST tipo Dictionary(Of String, String)) Con todos los parametros que se pasan por url
' Deben devolver una variable Dictionary(Of String, String) con status y msj para la respuesta del servidor
Public Class SAPSync
    Private oCnn As SAPbobsCOM.Company
    Private MySQLConnectionString As String
    Private MSSQLConnectionString As String
    Private connect As Boolean
    Private ejecutandoFacturaVentas As Boolean
    Public Sub New(MyCS As String, MSCS As String, SAPConn As SAPbobsCOM.Company)
        Me.connect = False
        Me.MySQLConnectionString = MyCS
        Me.MSSQLConnectionString = MSCS
        Me.oCnn = New SAPbobsCOM.Company
        Me.oCnn.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
        Me.oCnn.DbUserName = "sa" 'FIJO NO SE PUEDE CAMBIAR
        Me.oCnn.Server = "SERVER" 'Fijo
        Me.oCnn.UseTrusted = False
        Me.ejecutandoFacturaVentas = False
        Me.oCnn = SAPConn
    End Sub

    ' Actualiza los datos de clientes
    Public Function actualizarCliente(REQUEST As Dictionary(Of String, String)) As Dictionary(Of String, String)
        Dim respuesta As New Dictionary(Of String, String)
        Dim oBusinessPartners As SAPbobsCOM.BusinessPartners = Me.oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners)
        Dim chData As String = ""
        ' Si enviaron CardCode
        If REQUEST.ContainsKey("CardCode") Then
            oBusinessPartners.GetByKey(REQUEST("CardCode"))
            For Each data As KeyValuePair(Of String, String) In REQUEST
                Dim valor As String = Trim(HttpUtility.UrlDecode(data.Value))
                Select Case data.Key
                    Case "CardName"
                        chData = String.Format("(CardName: {0} > {1}){3}{2}", oBusinessPartners.CardName, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.CardName = valor
                    Case "Phone1"
                        chData = String.Format("(Phone1: {0} > {1}){3}{2}", oBusinessPartners.Phone1, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.Phone1 = valor
                    Case "U_fecha_nac"
                        chData = String.Format("(U_fecha_nac: {0} > {1}){3}{2}", oBusinessPartners.UserFields.Fields.Item("U_fecha_nac").Value, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.UserFields.Fields.Item("U_fecha_nac").Value = valor
                    Case "LicTradNum"
                        chData = String.Format("(FederalTaxID: {0} > {1}){3}{2}", oBusinessPartners.FederalTaxID, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.FederalTaxID = valor
                    Case "City"
                        chData = String.Format("(City: {0} > {1}){3}{2}", oBusinessPartners.City, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.City = valor
                    Case "Country"
                        chData = String.Format("(Country: {0} > {1}){3}{2}", oBusinessPartners.Country, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.Country = valor
                    Case "Address"
                        chData = String.Format("(Address: {0} > {1}){3}{2}", oBusinessPartners.Address, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.Address = valor
                    Case "AddID"
                        ' Ocupacion
                        chData = String.Format("(Ocupacion: {0} > {1}){3}{2}", oBusinessPartners.AdditionalID, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.AdditionalID = valor
                    Case "VatIdUnCmp"
                        ' Situacion
                        chData = String.Format("(Situacion: {0} > {1}){3}{2}", oBusinessPartners.UnifiedFederalTaxID, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.UnifiedFederalTaxID = valor
                    Case "ListNum"
                        chData = String.Format("(Categoria: {0} > {1}){3}{2}", oBusinessPartners.PriceListNum, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.PriceListNum = valor
                    Case "U_tipo_doc"
                        chData = String.Format("(Tipo Doc: {0} > {1}){3}{2}", oBusinessPartners.UserFields.Fields.Item("U_tipo_doc").Value, valor, chData, If(chData.Length > 0, ", ", ""))
                        oBusinessPartners.UserFields.Fields.Item("U_tipo_doc").Value = valor
                        If (valor = "C.I. Diplomatica") Then
                            oBusinessPartners.GroupCode = 102
                        Else
                            oBusinessPartners.GroupCode = 100
                        End If
                End Select
                Debug.WriteLine(data.Key)
            Next

            Dim estado = oBusinessPartners.Update()
            If estado <> 0 Then
                respuesta("msj") = oCnn.GetLastErrorDescription()
                respuesta("status") = "error"
            Else
                respuesta("msj") = "Se actualizaron los datos correctamente"
                respuesta("status") = "ok"
                Dim myCon As MySqlConnection = New MySqlConnection(MySQLConnectionString)
                Try
                    Dim query As String = String.Format("INSERT INTO logs (usuario,fecha,hora,accion,tipo,doc_num,data) values ('{0}', date(now()), time(now()),'Actualizar Cliente', 'Cliente', '{1}','{2}')", REQUEST("user"), REQUEST("CardCode"), chData)

                    myCon.Open()
                    Dim cmd As New MySqlCommand(query, myCon)
                    Debug.WriteLine(cmd.ExecuteNonQuery())
                    myCon.Close()
                Catch ex As Exception
                    Debug.WriteLine("Error No se pudo conectar (MyExec): " & ex.StackTrace)
                End Try
            End If
        Else
            respuesta("status") = "error"
            respuesta("msj") = "Debe proporcionar CardCode"
        End If
        Return respuesta
    End Function

    Public Function testMyDB(REQUEST As Dictionary(Of String, String)) As Dictionary(Of String, String)
        Dim My_con As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        Dim respuesta As New Dictionary(Of String, String)
        Dim valor As String = ""
        My_con.Open()


        Dim Query As String = "SELECT cliente FROM factura_venta WHERE f_nro = 154825;"
        Dim Command As New MySqlCommand(Query, My_con)
        Dim Reader As MySqlDataReader = Command.ExecuteReader()

        While Reader.Read()
            valor = Reader.GetString("cliente")
        End While

        My_con.Close()

        respuesta("status") = "ok"
        respuesta("msj") = valor
        Return respuesta

    End Function

    Public Function testMSDB(REQUEST As Dictionary(Of String, String)) As Dictionary(Of String, String)
        Dim msql As New SqlConnection(MSSQLConnectionString)
        msql.Open()
        Dim msQuery As String = "SELECT ItemName FROM OITM WHERE ItemCode = 'AC001'"
        Dim ms_cmd As New SqlCommand(msQuery, msql)
        Dim reader As SqlDataReader = ms_cmd.ExecuteReader()
        Dim valor As String = ""
        Dim respuesta As Dictionary(Of String, String) = New Dictionary(Of String, String)
        While reader.Read()
            valor = reader(0)
        End While
        msql.Close()
        respuesta("status") = "ok"
        respuesta("msj") = valor
        Return respuesta
    End Function
End Class
