

Imports MySql.Data.MySqlClient
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports System.Globalization
Imports System.IO
Imports System.Threading




Public Class SAPSyncForm

    Public oCnn As SAPbobsCOM.Company

    Public iError As Integer
    Public sError As String

    Dim IVA As Integer = 0 ' 10

    'Dim MysqlConn As MySqlConnection
    'Dim MysqlConnDet As MySqlConnection
    'Dim MysqlAux As MySqlConnection
    'Dim MysqlConnGastos As MySqlConnection
    'Dim MysqlCuotas As MySqlConnection
    Dim ms_con As New SqlConnection
    Dim estabs As Hashtable
    Dim MySQLConnectionString As String
    Dim MSSQLConnectionString As String
    Dim MySQLConnectionStringMarijoa As String

    Dim conectado As Boolean = False

    Dim cont As Integer = 0
    Dim correctRates = 0

    Dim ejecutando As Boolean = False

    Dim ejecutandoFacturaVentas As Boolean = False
    Dim ejecutandoRemisiones As Boolean = False
    Dim ejecutandoReservas As Boolean = False
    Dim ejecutandoClientes As Boolean = False
    Dim ejecutandoPagos As Boolean = False
    Dim ejecutandoAjustesPositivos As Boolean = False
    Dim ejecutandoAjustesNegativos As Boolean = False
    Dim ejecutandoCobroCuotas As Boolean = False
    Dim ejecutandoCompras As Boolean = False
    Dim ejecutandoNotasCredito As Boolean = False
    Dim ejecutandoAsientos As Boolean = False
    Dim ejecutandoLotes As Boolean = False
    Dim ejecutandoFraccionamientosPositivos As Boolean = False
    Dim ejecutandoFraccionamientosNegativos As Boolean = False
    Dim ejecutandoCancelacionPagos As Boolean = False
    Dim ejecutandoExtractosBancarios As Boolean = False

    Dim fallas_migracion = 0
    Dim filas_migracion = 0
    Dim LIMITE = 1000
    Dim ERROR_MIGRACION = 0




    Private Sub SAPSyncForm_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Try
            If conectado Then
                oCnn.Disconnect()
                oCnn = Nothing
                GC.Collect()
            End If

        Catch ex As Exception
            log(ex.Message)
        End Try
    End Sub

    Private Sub SAPSyncForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MySQLConnectionString = "server=" & server.Text & ";" & "user id=" & user.Text & ";" & "password=" & passw.Text & ";" & "database=marijoa_sap" ' marijoa_sap esto ya es fijo
        MySQLConnectionStringMarijoa = "server=localhost; port=3306;user=douglas; password=case; database=marijoa" ' marijoa

        'TimerStockXVentas.Enabled = True
    End Sub

    Public Function conectar()
        Dim habilitar As Boolean = True
        Try
            TimerConnect.Enabled = False

            If ConectaDI() = True Then
                Me.Text = "Conectados a " & oCnn.CompanyName
                ButtonStockXVentas.Enabled = habilitar
                ButtonSincPagos.Enabled = habilitar
                ButtonReservas.Enabled = habilitar
                ButtonAjustes.Enabled = habilitar
                ButtonAjustesNeg.Enabled = habilitar
                ButtonNotaCredito.Enabled = habilitar
                ButtonRemisiones.Enabled = habilitar
                ButtonUpdateBatchNumber.Enabled = habilitar
                ButtonSincCompras.Enabled = habilitar
                ButtonMigrarClientes.Enabled = habilitar
                ButtonMigrarProv.Enabled = habilitar
                ButtonMigrarLotes.Enabled = habilitar
                ButtonCobroCuotas.Enabled = habilitar
                ButtonSincClientes.Enabled = habilitar
                ButtonAsientos.Enabled = habilitar
                ButtonMigrarArticulos.Enabled = habilitar
                ButtonUpdatePriceList.Enabled = habilitar
                ButtonUserTables.Enabled = habilitar
                ButtonUpdateCliData.Enabled = habilitar
                ButtonExtractosBanc.Enabled = habilitar
                ButtonFraccNegativo.Enabled = habilitar
                ButtonFraccionamientos.Enabled = habilitar
                ButtonCheckSapCodes.Enabled = habilitar
                ButtonCall.Enabled = habilitar
                ButtonUpdateUbic.Enabled = habilitar
                UpdateCatFactura.Enabled = habilitar
                ButtonCancelPagos.Enabled = habilitar
                'ButtonDeleteClienes.Enabled = True
                loadEstablecimientos()
                TimerCotiz.Enabled = True
                ButtonDisconect.Enabled = True
                Dim company As String = company_db.Text


                'For Each itemChecked In sap_company_db.CheckedItems
                'company = itemChecked.ToString()
                ' Next
                log(sap_user.Text & " conectado a: " & company)

                MSSQLConnectionString = "Data Source=" & server.Text & ";Initial Catalog=" & company & ";Persist Security Info=True;User ID=sa;Password=" & ms_passw.Text & ""

                iniciarTimmers()


                'Timers
            Else
                log(" No se puede conectar a SAP: reintentando")
                detenerTimers()
                conectar()
            End If
        Catch ex As Exception
            log(" Exception al tratar de conectar a SAP: " & ex.StackTrace)
        End Try
        Return Nothing
    End Function

    Private Sub ButtonConnect_Click(sender As Object, e As EventArgs) Handles ButtonConnect.Click
        conectar()
    End Sub
    Private Function ConectaDI() As Boolean
        Try

            Dim company As String = company_db.Text
            'For Each itemChecked In sap_company_db.CheckedItems
            'company = itemChecked.ToString()
            'log("Item with title: " & quote & itemChecked.ToString() & quote & ", is checked. Checked state is: " & sap_company_db.GetItemCheckState(sap_company_db.Items.IndexOf(itemChecked)).ToString())
            ' Next


            oCnn = New SAPbobsCOM.Company
            oCnn.Server = "SERVER" 'Fijo
            oCnn.CompanyDB = company
            oCnn.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
            oCnn.UserName = sap_user.Text
            oCnn.Password = sap_passw.Text
            oCnn.DbUserName = "sa" 'FIJO NO SE PUEDE CAMBIAR
            oCnn.DbPassword = ms_passw.Text
            oCnn.UseTrusted = False
            oCnn.LicenseServer = server.Text
            iError = oCnn.Connect

            If iError <> 0 Then
                oCnn.GetLastError(iError, sError)
                Throw New Exception(sError)
            End If
            ButtonConnect.Enabled = False
            conectado = True

            logTextArea.Text = ""
            Return True

        Catch ex As Exception

            Thread.Sleep(2000)
            log("Error:" & sError & " " & iError)
            Return False
        End Try
    End Function
    Private Function iniciarTimmers() As Boolean
        checkCotiz()
        If correctRates <= 0 Then
            TimerStockXVentas.Enabled = True
            TimerAjustesNeg.Enabled = True
            TimerAjustesPos.Enabled = True
            TimerCompras.Enabled = True
            TimerSincClientes.Enabled = True
            TimerCobroCuotas.Enabled = True
            TimerReservas.Enabled = True
            TimerAsientos.Enabled = True
            TimerUptateLotes.Enabled = True
            TimerPagos.Enabled = True
            TimerRemisiones.Enabled = True
            TimerNotasCredito.Enabled = True
            TimerSyncBancExtract.Enabled = True
            TimerFracPos.Enabled = True
            TimerFracNeg.Enabled = True
            TimerCancelPagos.Enabled = True
            log("todo ok iniciando sinchronizer...")
            TimerCotiz.Interval = 3600000
        End If

        Return True
    End Function

    Private Function detenerTimers() As Boolean
        TimerStockXVentas.Enabled = False
        TimerAjustesNeg.Enabled = False
        TimerAjustesPos.Enabled = False
        TimerCompras.Enabled = False
        TimerSincClientes.Enabled = False
        TimerCobroCuotas.Enabled = False
        TimerReservas.Enabled = False
        TimerAsientos.Enabled = False
        TimerUptateLotes.Enabled = False
        TimerPagos.Enabled = False
        TimerRemisiones.Enabled = False
        TimerNotasCredito.Enabled = False
        TimerSyncBancExtract.Enabled = False
        Return True
    End Function

    Private Function checkCotiz()
        Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        ms_con.ConnectionString = MSSQLConnectionString
        ms_con.Open()

        correctRates = 0

        Try
            MysqlAux.Open()


            Dim Query As String = "SELECT m_cod, m_descri FROM monedas WHERE m_ref = 'No'"
            Dim Command As New MySqlCommand(Query, MysqlAux)
            Dim Reader As MySqlDataReader = Command.ExecuteReader()
            While Reader.Read()
                Dim m_cod = Reader.GetString("m_cod")

                Dim sql As String = "SELECT Rate FROM ORTT WHERE Currency = '" & m_cod & "' and RateDate = CAST(CURRENT_TIMESTAMP AS DATE)"
                Dim ms_cmd As New SqlCommand(sql, ms_con)

                Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
                If ms_reader.HasRows Then
                    While ms_reader.Read()
                        Dim Rate As Double = ms_reader.GetDecimal(0)
                        log("Cotizacion Moneda " & m_cod & "  " & Rate & " Ok ")
                    End While
                Else
                    log("Debe establecer la cotizacion para la Moneda " & m_cod & "")

                    PlaySystemSound()
                    correctRates += 1
                End If

                ms_reader.Close()

            End While
            Reader.Close()
            MysqlAux.Close()
            ms_con.Close()

        Catch myerror As Exception
            log("Cannot connect to database (F:checkCotiz): " & myerror.Message)
        End Try


        Return Nothing

    End Function

    Sub PlaySystemSound()
        Console.Beep(2000, 4)
    End Sub

    Private Function substring(str As String, longitud As Int32) As String
        Return Microsoft.VisualBasic.Left(str, longitud)
    End Function



    ' Verifica si una Factura ya ha sido enviada a SAP con Anterioridad, en caso de que si Retorna True en caso de que no Retorna False
    Private Function chequearFactura(Nro As Integer) As Boolean
        Dim ms_conf As New SqlConnection
        Try

            ms_conf.ConnectionString = MSSQLConnectionString
            ms_conf.Open()

            'log("Connection to SQL Server Opened")

            Dim checkQuery As String = "SELECT U_Nro_Interno FROM OINV WHERE U_Nro_Interno = " & Nro & ";"
            'log(checkQuery)
            Dim ms_cmd As New SqlCommand(checkQuery, ms_conf)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            While ms_reader.Read()
                Dim existe = ms_reader(0)
                'log("La Factura Nro: " & Nro & " Not Nothing")
                Return True
            End While
            Return False

        Catch ex As Exception
            log("Error while connecting to SQL Server. Factura Nro:  " & Nro & "  " & ex.Message)
        Finally
            ms_conf.Close() 'Whether there is error or not. Close the connection.
        End Try
        Return False
    End Function

    ' Verifica si una Nota de Remision ya ha sido enviada a SAP con Anterioridad, en caso de que si Retorna True en caso de que no Retorna False
    Private Function chequearNotaRemision(Nro As Integer) As Boolean
        Dim ms_conr As New SqlConnection
        Try

            ms_conr.ConnectionString = MSSQLConnectionString
            ms_conr.Open()


            'log("Connection to SQL Server Opened")

            Dim checkQuery As String = "SELECT U_Nro_Interno FROM OWTR WHERE U_Nro_Interno = " & Nro & ";"
            'log(checkQuery)
            Dim ms_cmd As New SqlCommand(checkQuery, ms_conr)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            While ms_reader.Read()
                Dim existe = ms_reader(0)
                'log("La Factura Nro: " & Nro & " Not Nothing")
                Return True
            End While
            Return False

        Catch ex As Exception
            log("Error while connecting to SQL Server. Nota de Remision Nro:  " & Nro & "  " & ex.Message)
        Finally
            ms_conr.Close() 'Whether there is error or not. Close the connection.
        End Try
        Return False
    End Function

    Private Function getDatosFactura(Nro As Integer) As String()
        Dim ms_cond As New SqlConnection
        Try
            ms_cond.ConnectionString = MSSQLConnectionString
            ms_cond.Open()


            Dim query As String = "SELECT TOP 1 DocEntry,CardCode,CardName,CONVERT(VARCHAR(10), DocDate, 103) AS DocDate,IsNull(Address,'') as Address FROM OINV WHERE U_Nro_Interno =  " & Nro & ";"

            Dim ms_cmd As New SqlCommand(query, ms_cond)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            While ms_reader.Read()


                Dim DocEntry As Integer = ms_reader(0)
                Dim CardCode As String = ms_reader(1)
                Dim CardName As String = ms_reader(2)
                Dim DocDate As String = ms_reader(3)
                Dim Address As String = ms_reader(4)


                'log("DocEntry " & DocEntry & " CardCode " & CardCode)
                Return New String() {DocEntry.ToString, CardCode, CardName, DocDate, Address}

            End While


        Catch ex As Exception
            log("Error while connecting to SQL Server. Factura Nro:  " & Nro & "  " & ex.Message)
        Finally

            ms_cond.Close() 'Whether there is error or not. Close the connection.

        End Try
        Return New String() {""}
    End Function

    Private Function getCountryName(CountryCode As String) As String
        ms_con.ConnectionString = MSSQLConnectionString
        ms_con.Open()
        Dim query As String = "SELECT  Name  FROM  OCRY WHERE Code = '" & CountryCode & "' "
        Dim ms_cmd As New SqlCommand(query, ms_con)
        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
        ms_reader.Read()
        Dim Name As String = ms_reader(0)
        ms_con.Close()
        Return Name

    End Function

    ' Funcion para cargar el Arreglo con los Establecimientos
    Private Function loadEstablecimientos()

        Dim MysqlEstabs As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        estabs = New Hashtable()
        Try
            MysqlEstabs.Open()
            Dim sucsQuery As String = "SELECT suc,estab_cont FROM sucursales WHERE estab_cont IS NOT NULL;"
            Dim sucCommand As New MySqlCommand(sucsQuery, MysqlEstabs)
            Dim sucReader As MySqlDataReader
            sucReader = sucCommand.ExecuteReader()
            While sucReader.Read()
                Dim suc = sucReader.GetString("suc")
                Dim estab = sucReader.GetString("estab_cont")
                estabs.Add(suc, estab)
                'log("Establecimiento " & suc & "  " & estab)
            End While
            sucReader.Close()
            MysqlEstabs.Close()
        Catch myerror As Exception
            log("Cannot connect to database (F:loadEstablecimientos): " & myerror.Message)
        End Try
        Return Nothing
    End Function


    Private Function getTimbrado(factura As Integer, suc As String, pdv As String, tipo As String) As String

        Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

        Dim timbrado As String = ""
        Try
            MysqlAux.Open()
            Dim q = "SELECT timbrado FROM factura_cont WHERE fact_nro = '" & factura & "' AND suc = '" & suc & "' AND pdv_cod = '" & pdv & "' AND tipo_doc = '" & tipo & "' AND estado = 'Cerrada'"
            Dim sucCommand As New MySqlCommand(q, MysqlAux)
            Dim sucReader As MySqlDataReader
            sucReader = sucCommand.ExecuteReader()

            While sucReader.Read()
                timbrado = sucReader.GetString("timbrado")
                'log("timbrado " & timbrado & "  ")
            End While
            sucReader.Close()
            MysqlAux.Close()
            Return timbrado
        Catch myerror As Exception
            log("Cannot connect to database (F:getTimbrado): " & myerror.Message & " Factura: " & factura)
        End Try
        Return timbrado
    End Function

    Private Function getEfectivoXCobroFactura(f_nro As Integer, fact_nro_or_trans_nro As String) As Double

        Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)


        Dim total As Double = 0
        Try
            MysqlAux.Open()
            Dim q = "SELECT SUM(entrada_ref-salida_ref) AS entrada  FROM efectivo WHERE estado = 'Pendiente' AND e_sap IS NULL and f_nro = " & f_nro & ";"

            If (fact_nro_or_trans_nro = "TRANS_NUM") Then
                q = "SELECT  SUM(entrada_ref-salida_ref) AS entrada  FROM efectivo WHERE estado = 'Pendiente' AND e_sap IS NULL and trans_num = " & f_nro & ";"
            End If

            Dim Command As New MySqlCommand(q, MysqlAux)
            Dim Reader As MySqlDataReader
            Reader = Command.ExecuteReader()

            While Reader.Read()
                If Not Reader.IsDBNull(0) Then
                    total = Reader.GetDouble("entrada")
                    If (total < 0) Then
                        total = 0
                    End If
                Else
                    total = 0
                End If
                'log("entrada " & total & "  ")
            End While
            Reader.Close()
            MysqlAux.Close()
            Return total
        Catch myerror As Exception
            log("Cannot connect to database (F:getEfectivoXCobroFactura): " & myerror.Message)
        End Try
        Return total
    End Function

    Function setPagoEnviado(f_nro As Integer) As Boolean
        Dim MysqlAuxEnv As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        Try
            MysqlAuxEnv.Open()
            Dim updQuery As String = "update factura_venta set e_sap = 2 where f_nro = " & f_nro & ";"
            Dim updCommand As New MySqlCommand(updQuery, MysqlAuxEnv)
            updCommand.ExecuteNonQuery()
        Catch myerror As Exception
            log("Cannot connect to database (F:setPagoEnviado): " & myerror.Message)
            Return False
        Finally
            MysqlAuxEnv.Close()
        End Try
        Return True
    End Function

    Function reconnect() As Boolean
        log("Se ha perdido la conexion con la base de datos. Reconectando...")
        detenerTimers()

        conectar()
        Return Nothing
    End Function

    Public Function SincronizarFacturasVentas()
        If (Not ejecutandoFacturaVentas) Then

            ejecutandoFacturaVentas = True
            'Console.WriteLine("Sincronizando Facturas de Ventas")
            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnDet As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlCuotas As MySqlConnection = New MySqlConnection(MySQLConnectionString)


            Dim oinv As SAPbobsCOM.Documents
            Dim cuotas As SAPbobsCOM.Document_Installments

            Try
                MysqlConn.Open()
                MysqlConnDet.Open()
                MysqlCuotas.Open()
               

                Dim mySelectQuery As String = "SELECT f_nro,fact_nro,pdv_cod, cod_cli,tipo_doc_cli,cliente,usuario,fecha,DATE_FORMAT(fecha_cierre,'%d/%m/%Y') AS fecha_lat,DATE_FORMAT(fecha_cierre,'%Y%m%d') as fechaSAP,ruc_cli,suc,cat,floor(total) as total,total_desc,total_bruto,cod_desc,cant_cuotas,moneda,cotiz,IF(nro_orden IS NULL,'',nro_orden) AS nro_orden,orden_cli,orden_valor FROM factura_venta WHERE estado = 'Cerrada' AND empaque = 'Si' AND e_sap IS NULL limit 4"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                Dim Nro = 0

                Try

                    While myReader.Read()
                        oinv = oCnn.GetBusinessObject(BoObjectTypes.oInvoices)

                        Nro = myReader.GetInt32("f_nro")

                        MyExec("UPDATE factura_venta set e_sap = 5 where f_nro = " & Nro)  ' e_sap = 5 = En proceso de Envio

                        Dim CardCode = myReader.GetString("cod_cli")
                        Dim CardName = myReader.GetString("cliente")
                        Dim Vendedor = myReader.GetString("usuario")
                        Dim Total = myReader.GetDouble("total")
                        Dim DocDueDate = myReader.GetString("fecha_lat")
                        Dim SUC = myReader.GetString("suc")
                        Dim estab = estabs.Item(SUC)
                        Dim Moneda = myReader.GetString("moneda")
                        Dim Cotiz = myReader.GetDouble("cotiz")
                        Dim cant_cuotas = myReader.GetInt32("cant_cuotas")
                        Dim TipoDocCli = myReader.GetString("tipo_doc_cli")
                        Dim nro_orden = myReader.GetString("nro_orden")
                        Dim CAT = myReader.GetInt32("cat")
                        Dim orden_cli = ""
                        Dim orden_valor = 0
                        If (nro_orden <> "") Then
                            orden_cli = myReader.GetString("orden_cli")
                            orden_valor = myReader.GetDouble("orden_valor")
                        End If

                        Dim PaymentGroup = -1 'CONTADO

                        ajustesPositivosXFactura(Nro, Vendedor, Moneda, Cotiz, SUC, DocDueDate, "Sistema")

                        If (cant_cuotas > 0) Then
                            'log("Cantidad de Cuotas " & cant_cuotas)
                            PaymentGroup = 1 'CREDITO  
                        End If

                        'log("DocDueDate " & DocDueDate & "  " & Date.Now)

                        'Dim DocDate = DateTime.Now.ToString("yyyyMMdd")

                        ' log("estab " & estab & "  " & DocDate)

                        Dim FacturaLegal As String = ""
                        If Not myReader.IsDBNull(1) Then
                            FacturaLegal = myReader.GetString("fact_nro").ToString()
                        End If

                        Dim PDV As String = Nothing
                        If Not myReader.IsDBNull(2) Then
                            PDV = myReader.GetString("pdv_cod").ToString()
                        End If

                        'log("Cliente " & CardCode & "  " & CardName)

                        If chequearFactura(Nro) Then
                            log("La Factura Nro: " & Nro & " ya ha sido enviada a SAP con Anterioridad no se puede enviar...")
                            MyExec("UPDATE factura_venta set e_sap = 1 where f_nro = " & Nro)
                        Else

                            oinv.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                            oinv.Series = 4  ' NNM1
                            oinv.CardCode = CardCode
                            oinv.CardName = Microsoft.VisualBasic.Left(CardName.ToString, 100) ' Max Length 100
                            oinv.UserFields.Fields.Item("U_vendedor").Value = Vendedor
                            oinv.UserFields.Fields.Item("U_SUC").Value = SUC
                            oinv.DocCurrency = Moneda
                            oinv.DocRate = Cotiz
                            oinv.PaymentGroupCode = PaymentGroup ' Definir Tabla OCTG   -1 CONTADO, 1 CREDITO 

                            If (nro_orden <> "") Then
                                oinv.ImportFileNum = nro_orden
                                oinv.UserFields.Fields.Item("U_ord_cliente").Value = orden_cli
                                oinv.UserFields.Fields.Item("U_ord_valor").Value = orden_valor
                            End If

                            oinv.UserFields.Fields.Item("U_CAT").Value = CAT

                            oinv.DocDate = DocDueDate 'Fecha de Contabilizacion
                            If (cant_cuotas = 0) Then
                                oinv.DocDueDate = DocDueDate
                            End If
                            'oinv.DocTotal = Total
                            oinv.DiscountPercent = 0

                            If FacturaLegal <> "" Then
                                oinv.FolioPrefixString = "FV"
                                oinv.FolioNumber = FacturaLegal
                                If (TipoDocCli <> "C.I. Diplomatica") Then
                                    oinv.VatPercent = 0 'IVA siempre va ser 0
                                Else
                                    oinv.VatPercent = 0
                                End If
                                oinv.JournalMemo = Microsoft.VisualBasic.Left("F.V. (D) - " & CardCode & " " & CardName & "", 50) ' Max Length 60
                                oinv.UserFields.Fields.Item("U_SER_PE").Value = PDV
                                oinv.UserFields.Fields.Item("U_SER_EST").Value = estabs.Item(SUC)
                                oinv.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "SI"
                                Dim timbrado As String = getTimbrado(FacturaLegal, SUC, PDV, "Factura")
                                oinv.UserFields.Fields.Item("U_NUM_AUTOR").Value = timbrado
                            Else
                                oinv.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
                                oinv.VatPercent = 0
                                oinv.JournalMemo = Microsoft.VisualBasic.Left("F.V. (ND) - " & CardCode & " " & CardName & "", 50)
                            End If

                            'Obtener datos del Detalle de Factura
                            Dim detalle As String = "SELECT codigo,lote,um_prod,descrip, um_cod, cantidad,ROUND(subtotal / cantidad,0) as precio_neto, descuento, subtotal,  ROUND(subtotal)  as total_linea, kg_med,cant_med,cod_falla_e,falla_real,fuera_rango,IF(estado_venta IS NULL,'Normal',estado_venta) AS estado_venta FROM fact_vent_det WHERE f_nro = " & Nro & " AND (e_sap = 0 or e_sap is null) "
                            Dim myDetCommand As New MySqlCommand(detalle, MysqlConnDet)
                            Dim myDetReader As MySqlDataReader
                            myDetReader = myDetCommand.ExecuteReader()

                            Dim AccountCode = getCuentaContable("4.1.1.1", SUC, Moneda)

                            'log("AccountCode:" & AccountCode)

                            Dim line As Integer = 0
                            While myDetReader.Read()
                                'log("Linea " & line)
                                Dim codigo = myDetReader.GetString("codigo")
                                Dim lote = myDetReader.GetString("lote")
                                Dim um_prod = myDetReader.GetString("um_prod")
                                'Dim cant_med = myDetReader.GetDouble("cant_med")
                                Dim cantidad = myDetReader.GetDouble("cantidad")
                                Dim descrip = myDetReader.GetString("descrip")
                                Dim precio_neto = myDetReader.GetDouble("precio_neto")
                                Dim subtotal = myDetReader.GetDouble("subtotal")
                                Dim descuento = myDetReader.GetDouble("descuento")
                                Dim total_linea = myDetReader.GetDouble("total_linea")
                                Dim estado_venta = myDetReader.GetString("estado_venta")
                                'Dim total_linea = precio_neto * cantidad
                                ' Console.WriteLine("Datos Fila:  cantidad:" & cantidad & " precio_neto: " & precio_neto & " subtotal:" & subtotal & "  total_linea:" & total_linea)
                                If line > 0 Then
                                    oinv.Lines.Add()
                                End If
                                line += 1


                                If (TipoDocCli = "C.I. Diplomatica" Or FacturaLegal = "") Then
                                    oinv.Lines.TaxCode = "IVA_EXE" 'NO Siempre es IVA_10 si es Diplomatico es IVA_EXE
                                Else
                                    oinv.Lines.TaxCode = "IVA_EXE" ' "IVA_10" Ya no se usara IVA_10
                                End If

                                If String.IsNullOrEmpty(FacturaLegal) Then
                                    oinv.Lines.TaxLiable = BoYesNoEnum.tNO
                                Else
                                    oinv.Lines.TaxLiable = BoYesNoEnum.tYES
                                End If

                                oinv.Lines.ItemCode = codigo
                                oinv.Lines.Quantity = cantidad
                                oinv.Lines.Price = precio_neto
                                oinv.Lines.LineTotal = total_linea  'Total de la Linea

                                oinv.Lines.ItemDescription = Microsoft.VisualBasic.Left(descrip, 100) ' MaxLength 100
                                oinv.Lines.WarehouseCode = SUC
                                'oinv.Lines.AccountCode = "4.1.1.1.01"  'Cuenta del Mayor Venta de Mercaderias  

                                oinv.Lines.AccountCode = AccountCode

                                'Lote
                                oinv.Lines.BatchNumbers.BatchNumber = lote
                                oinv.Lines.BatchNumbers.Quantity = cantidad
                                oinv.Lines.BatchNumbers.Location = SUC
                                oinv.Lines.UserFields.Fields.Item("U_Descuento").Value = descuento.ToString
                                oinv.Lines.UserFields.Fields.Item("U_estado_venta").Value = estado_venta


                            End While
                            myDetReader.Close()
                            ' Establecer cuotas si es que tiene
                            ' -1 CONTADO
                            '  1 CREDITO
                            If (cant_cuotas > 0) Then

                                Dim myCuotasCommand As New MySqlCommand("SELECT monto_ref,monto_s_total,dias,DATE_FORMAT(vencimiento,'%Y/%m/%d') as venc_sap, DATE_FORMAT(vencimiento,'%d/%m/%Y') AS fecha_venc ,porcentaje FROM cuotas WHERE f_nro = " & Nro & ";", MysqlCuotas)
                                Dim myCuotasReader As MySqlDataReader
                                myCuotasReader = myCuotasCommand.ExecuteReader()

                                cuotas = oinv.Installments
                                Dim InstallmentID = 0
                                While myCuotasReader.Read()
                                    Dim FechaVenc = myCuotasReader.GetString("fecha_venc")
                                    'Dim Porcentaje = myCuotasReader.GetDouble("porcentaje")
                                    Dim monto_s_total = myCuotasReader.GetDouble("monto_s_total")

                                    If (InstallmentID > 0) Then
                                        cuotas.Add()
                                    End If
                                    cuotas.SetCurrentLine(InstallmentID)
                                    cuotas.DueDate = FechaVenc


                                    'cuotas.Percentage = Porcentaje
                                    cuotas.Total = monto_s_total

                                    InstallmentID += 1

                                End While
                                myCuotasReader.Close()
                            End If

                            iError = oinv.Add
                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log("Error: " & iError & "  " & sError & "  Factura: " & Nro)
                                MyExec("UPDATE factura_venta set e_sap = 3 where f_nro = " & Nro)
                                'Throw New Exception(sError) ' No Cortar cuando hay error proseguir con las demas facturas
                            Else
                                Dim DocEntry As String = oCnn.GetNewObjectKey

                                cambiarEstadoFactura(Nro, 1)

                                log("Identificador Factura Venta DocEntry: " & DocEntry)

                                'ajustesPositivosXFactura(Nro, Vendedor, Moneda, Cotiz, SUC, DocDueDate, "Sistema") ' Ajustes Positivos Sistema Primero
                                ajustesNegativosXFactura(Nro, Vendedor, Moneda, Cotiz, SUC, DocDueDate, "Sistema") ' Ajustes Negativos Sistema Primero

                                'log("ajustesPositivos  ")
                                ajustesPositivosXFactura(Nro, Vendedor, Moneda, Cotiz, SUC, DocDueDate, "%") ' Ajustes Positivos 
                                'log("ajustesNegativosXFactura  ")
                                ajustesNegativosXFactura(Nro, Vendedor, Moneda, Cotiz, SUC, DocDueDate, "%") ' Ajustes Negativos


                            End If


                        End If

                        'log((Nro & ", " & myReader & ", " & myReader("cod_cli") & ", " & myReader("cliente")))
                    End While
                    myReader.Close()
                Catch ex As Exception
                    log("Error COM Factura Venta:  " & ex.Message & " Factura " & Nro & ", D:" & ex.StackTrace)
                    MyExec("UPDATE factura_venta set e_sap = 3 where f_nro = " & Nro)
                End Try

                MysqlConn.Close()
                MysqlConnDet.Close()
                MysqlCuotas.Close()


            Catch ex As Exception
                log("MySqlException Error al Migrar Factura Venta: Cannot connect to database: " & ex.Message)
            End Try

            'MysqlConn.Close()
            'MysqlConnDet.Close()
            'MysqlCuotas.Close()

            ejecutandoFacturaVentas = False

        End If


        Return Nothing
    End Function
    Private Sub ButtonStockXVentas_Click(sender As Object, e As EventArgs) Handles ButtonStockXVentas.Click
        SincronizarFacturasVentas()
    End Sub

    Public Function cambiarEstadoFactura(Nro As Integer, estado As Integer)
        Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

        Try
            MysqlAux.Open()
            Dim updQuery As String = "update factura_venta set e_sap = 1 where f_nro = " & Nro & ";"
            Dim updCommand As New MySqlCommand(updQuery, MysqlAux)
            updCommand.ExecuteNonQuery()
            MysqlAux.Close()
        Catch myerror As Exception
            log("Cannot connect to database: SQL: update factura_venta set e_sap = 1 where f_nro = " & Nro & " " & myerror.Message)
        End Try
        Return Nothing
    End Function
    Public Function ajustesPositivosXFactura(Nro As Integer, Vendedor As String, Moneda As String, Cotiz As Double, SUC As String, DocDueDate As Date, usuario_aj As String)
        Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)


        Dim entrada As SAPbobsCOM.Documents

        ' Si el Usuario es Sistema es para los casos que se debe ajustar antes de enviar la factura

        Try
            MyConn.Open()

            entrada = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenEntry)


            Dim detalle As String = "SELECT id_ajuste,usuario,codigo,lote,tipo,ajuste,p_costo,valor_ajuste,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,um,suc,estado FROM ajustes WHERE ajuste > 0 AND signo = '+' AND  f_nro = " & Nro & " and usuario like '" & usuario_aj & "' and (e_sap = 0 or e_sap is null)"
            Dim myDetCommand As New MySqlCommand(detalle, MyConn)
            Dim myDetReader As MySqlDataReader
            myDetReader = myDetCommand.ExecuteReader()

            If (myDetReader.HasRows) Then

                entrada.Reference2 = Nro
                entrada.Series = 25  ' NNM1
                entrada.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                entrada.UserFields.Fields.Item("U_vendedor").Value = Vendedor
                entrada.UserFields.Fields.Item("U_SUC").Value = SUC
                entrada.DocCurrency = Moneda
                entrada.DocRate = Cotiz
                entrada.GroupNumber = -1 'Ultimo Precio
                entrada.DocDate = Date.Now
                entrada.DocDueDate = DocDueDate
                entrada.JournalMemo = Microsoft.VisualBasic.Left("Ajuste (+) FV:" & Nro & " Aumento en Salida ", 50) ' Max Length 50
                entrada.Comments = Microsoft.VisualBasic.Left("Ajuste (+) FV:" & Nro & " Aumento en Salida ", 254) ' Max Length 254

                Dim usuario = ""

                'Detalle
                Dim line As Integer = 0
                While myDetReader.Read()


                    Dim codigo = myDetReader.GetString("codigo")
                    Dim lote = myDetReader.GetString("lote")
                    'Dim um_prod = myDetReader.GetString("um_prod")
                    Dim ajuste = myDetReader.GetDouble("ajuste")
                    Dim um = myDetReader.GetString("um")
                    usuario = myDetReader.GetString("usuario")
                    Dim precio_neto = myDetReader.GetDouble("p_costo")
                    Dim subtotal = myDetReader.GetDouble("valor_ajuste")
                    Dim Tipo = myDetReader.GetString("tipo")
                    Dim Motivo = myDetReader.GetString("motivo")


                    If line > 0 Then
                        entrada.Lines.Add()
                    End If
                    line += 1


                    entrada.Lines.ItemCode = codigo
                    entrada.Lines.WarehouseCode = SUC
                    entrada.Lines.Quantity = ajuste
                    'entrada.Lines.Price = precio_neto
                    entrada.Lines.UnitPrice = precio_neto
                    'entrada.Lines.LineTotal = subtotal 'Total de la Linea

                    entrada.Lines.AccountCode = "4.1.2.1.07"  'Cuenta del Mayor

                    'Lote
                    entrada.Lines.BatchNumbers.BatchNumber = lote
                    entrada.Lines.BatchNumbers.Quantity = ajuste



                End While
                entrada.UserFields.Fields.Item("U_Usuario").Value = usuario


                iError = entrada.Add
                If iError <> 0 Then
                    oCnn.GetLastError(iError, sError)

                    log(iError & " " & sError & " Ajuste Entrada: " & Nro)
                Else
                    Dim DocEntry As String = oCnn.GetNewObjectKey
                    log("Ajuste Positivo Identificador DocEntry: " & DocEntry)

                    Try

                        Dim updQuery As String = "update ajustes set e_sap = 1 where f_nro = " & Nro & "  and usuario like '" & usuario_aj & "' and signo = '+' "  ' Actualizar Ajustes de esta Factura"
                        MyExec(updQuery)
                        
                    Catch myerror As Exception
                        log("Cannot connect to database (F: ajustesPositivosXFactura): " & myerror.Message)
                    End Try

                End If

            End If
            myDetReader.Close()
            MyConn.Close()

        Catch ex As Exception
            log("Error COM Ajustes (+): " & ex.Message)
        End Try

      
        Return True

    End Function

    Function getStockLote(codigo As String, lote As String, suc As String) As Double
        Try
            Dim ms_con As New SqlConnection
            ms_con.ConnectionString = MSSQLConnectionString
            ms_con.Open()


            Dim query As String = "SELECT  Quantity - isNull(i.IsCommited,0)   as StockReal FROM   OIBT i where i.ItemCode = '" & codigo & "' and i.BatchNum = '" & lote & "' and WhsCode = '" & suc & "'"

            Dim ms_cmd As New SqlCommand(query, ms_con)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            If ms_reader.HasRows Then
                ms_reader.Read()
                Dim StockReal As Double = ms_reader(0)
                'log("Cuenta Contable para  " & Sucursal & " Cuenta  " & Cuenta & "   " & AcctCode)
                Return StockReal
            Else
                Return 0
            End If

            ms_reader.Close()

        Catch ex As Exception
            log("Error en consulta stock real lote: getStockLote()  " & codigo & " Lote " & lote)
        Finally

            ms_con.Close() 'Whether there is error or not. Close the connection.

        End Try
        Return 0

    End Function

    Public Function ajustesNegativosXFactura(Nro As Integer, Vendedor As String, Moneda As String, Cotiz As Double, SUC As String, DocDueDate As Date, usuario_aj As String)
        Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        Dim MyAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

        Dim salida As SAPbobsCOM.Documents

        Try
            MyConn.Open()
            MyAux.Open()
            salida = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenExit)


            Dim detalle As String = "SELECT id_ajuste,usuario,codigo,lote,tipo,ajuste,p_costo,valor_ajuste,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,um,suc,estado FROM ajustes WHERE ajuste > 0 AND signo = '-' AND  f_nro = " & Nro & " AND usuario like '" & usuario_aj & "' AND (e_sap = 0 or e_sap is null)"
            Dim myDetCommand As New MySqlCommand(detalle, MyConn)
            Dim myDetReader As MySqlDataReader
            myDetReader = myDetCommand.ExecuteReader()

            If (myDetReader.HasRows) Then

                salida.Reference2 = Nro
                salida.Series = 26  ' NNM1
                salida.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                salida.UserFields.Fields.Item("U_vendedor").Value = Vendedor
                salida.UserFields.Fields.Item("U_SUC").Value = SUC

                salida.DocCurrency = Moneda
                salida.DocRate = Cotiz
                salida.GroupNumber = -1 'Ultimo Precio
                salida.DocDate = Date.Now
                salida.DocDueDate = DocDueDate
                salida.JournalMemo = Microsoft.VisualBasic.Left("Ajuste (-) FV:" & Nro & " Disminucion en Salida ", 50) ' Max Length 50

                salida.Comments = Microsoft.VisualBasic.Left("Ajuste (-) FV:" & Nro & " Disminucion en Salida ", 254) ' Max Length 254

                Dim usuario = ""
                Dim tmp_lote = ""

                'Detalle
                Dim line As Integer = 0
                While myDetReader.Read()


                    Dim codigo = myDetReader.GetString("codigo")
                    Dim lote = myDetReader.GetString("lote")
                    'Dim um_prod = myDetReader.GetString("um_prod")
                    Dim ajuste = myDetReader.GetDouble("ajuste")
                    Dim um = myDetReader.GetString("um")
                    usuario = myDetReader.GetString("usuario")
                    Dim precio_neto = myDetReader.GetDouble("p_costo")
                    Dim subtotal = myDetReader.GetDouble("valor_ajuste")
                    Dim Tipo = myDetReader.GetString("tipo")
                    Dim Motivo = myDetReader.GetString("motivo")
                    tmp_lote = lote
                    If line > 0 Then
                        salida.Lines.Add()
                    End If
                    line += 1

                    'Control de stock para que no descuente negativo
                    Dim StockReal = getStockLote(codigo, lote, SUC)
                    If (ajuste > StockReal) Then
                        ajuste = StockReal
                    End If

                    salida.Lines.ItemCode = codigo
                    salida.Lines.WarehouseCode = SUC
                    salida.Lines.Quantity = ajuste
                    'salida.Lines.Price = precio_neto
                    salida.Lines.UnitPrice = precio_neto
                    'salida.Lines.LineTotal = subtotal 'Total de la Linea


                    salida.Lines.AccountCode = "6.1.4.1.17"  'Cuenta del Mayor Mermas en Stock

                    'Lote
                    salida.Lines.BatchNumbers.BatchNumber = lote
                    salida.Lines.BatchNumbers.Quantity = ajuste


                End While
                salida.UserFields.Fields.Item("U_Usuario").Value = usuario

                iError = salida.Add


                If iError <> 0 Then
                    oCnn.GetLastError(iError, sError)
                    log("Error en ajustesNegativosXFactura : Ajuste Salida  Nro: " & Nro & " Lote: " & tmp_lote & "    " & iError & "  " & sError)
                Else
                    Dim DocEntry As String = oCnn.GetNewObjectKey
                    log("Ajuste Negativo Identificador DocEntry: " & DocEntry)

                    Try
                        Dim updQuery As String = "update ajustes set e_sap = 1 where f_nro = " & Nro & " and usuario like '" & usuario_aj & "' and signo = '-' ;" ' Actualizar Ajustes de esta Factura
                        Dim updCommand As New MySqlCommand(updQuery, MyAux)
                        updCommand.ExecuteNonQuery()
                    Catch myerror As Exception
                        log("Cannot connect to database (F: ajustesNegativosXFactura): " & myerror.Message)
                    End Try

                End If

            End If
            myDetReader.Close()
            MyConn.Close()
            MyAux.Close()
        Catch ex As Exception
            log("Error COM Ajustes (-): " & ex.Message)
        End Try

        Return True

    End Function

    Private Function getCuentaContable(Cuenta As String, Sucursal As String, Moneda As String) As String
        Try
            Dim ms_con As New SqlConnection
            ms_con.ConnectionString = MSSQLConnectionString
            ms_con.Open()


            Dim query As String = "SELECT AcctCode FROM OACT WHERE AcctCode like '" & Cuenta & "%' AND ExportCode = '" & Sucursal & "' and  ( ActCurr = '" & Moneda & "' or  ActCurr = '##');"

            Dim ms_cmd As New SqlCommand(query, ms_con)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            If ms_reader.HasRows Then
                ms_reader.Read()
                Dim AcctCode As String = ms_reader(0)
                'log("Cuenta Contable para  " & Sucursal & " Cuenta  " & Cuenta & "   " & AcctCode)
                Return AcctCode
            Else
                Return ""
                log("Cuenta Contable no definida para  " & Sucursal & "  Cuenta  " & Cuenta & "   ")
            End If

            ms_reader.Close()

        Catch ex As Exception
            log("Error en consulta de cuenta contable por sucursal: getCuentaContable()  " & Cuenta & "   " & Sucursal & "  " & Moneda & " " & ex.Message)
        Finally

            ms_con.Close() 'Whether there is error or not. Close the connection.

        End Try
        Return Cuenta
    End Function

    Public Function SincronizarPagosRecibidos()
        If (Not ejecutandoPagos) Then
            ejecutandoPagos = True
            Dim MysqlConnPagos As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlCuotas As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim MysqlConnTarjetas As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnCheques As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim f_nro = 0

            Try
                Dim paym As SAPbobsCOM.Payments

                Try
                    MysqlConnPagos.Open()
                    MysqlConnTarjetas.Open()
                    MysqlConnCheques.Open()
                    MysqlCuotas.Open()

                    Dim mySelectQuery As String = "SELECT f_nro, suc,total_bruto,cant_cuotas,moneda  FROM factura_venta WHERE control_caja = 'Si' AND e_sap = 1 limit 4;"
                    Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConnPagos)

                    Dim myReader As MySqlDataReader = myCommand.ExecuteReader()
                    Dim linea = 0

                    While myReader.Read()

                        paym = oCnn.GetBusinessObject(BoObjectTypes.oIncomingPayments)

                        Dim flag As Boolean = False

                        f_nro = myReader.GetInt32("f_nro")
                        Dim suc = myReader.GetString("suc")
                        Dim cant_cuotas = myReader.GetInt32("cant_cuotas")
                        Dim total_factura = myReader.GetDouble("total_bruto")
                        Dim moneda = myReader.GetString("moneda")

                        'Buscar datos de la Factura en SAP  MSSQL 
                        Dim datos As String() = getDatosFactura(f_nro)
                        Dim DocEntry As Integer = CInt(datos(0))

                        Dim CardCode As String = datos(1)
                        Dim CardName As String = datos(2)
                        Dim DocDate As String = datos(3)


                        'Dim DocDate As DateTime = DateTime.ParseExact(DocDateString, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                        Dim Address As String = datos(4)

                        paym.CardCode = CardCode
                        paym.CardName = CardName
                        paym.ControlAccount = ""
                        paym.DocDate = CDate(DocDate)
                        paym.Address = Address
                        paym.Invoices.DocEntry = DocEntry

                        'paym.Invoices.SetCurrentLine(linea)

                        ' Por cada Factura buscar sus Pagos en Efectivo Tarjeta y Cheques

                        'Pagos en Efectivo
                        Dim PagoEnEfectivo As Double = getEfectivoXCobroFactura(f_nro, "FACTURA")
                        paym.CashSum = PagoEnEfectivo
                        If (PagoEnEfectivo > 0) Then
                            flag = True
                        End If

                        paym.CashAccount = getCuentaContable("1.1.1.2", suc, moneda)  ' Por ahora despues Agregar un campo en el Plan de Cuentas que represente a la Sucursal. 

                        paym.JournalRemarks = Microsoft.VisualBasic.Left("Pago Rec. Ref: " & f_nro & " " & CardCode & " " & CardName & "", 50)


                        'Transferencia bancaria
                        'paym.TransferAccount = "1.1.1.2.01"
                        'paym.TransferDate = CDate(DocDate)
                        'paym.TransferSum = PagoEnEfectivo

                        Dim qCuotas As String = "SELECT id_cuota,monto,monto_s_total - monto as pago_parcial  FROM cuotas WHERE monto_s_total - monto > 0 and f_nro =  " & f_nro & ""
                        Dim cuotasCommand As New MySqlCommand(qCuotas, MysqlCuotas)
                        Dim cuotasReader As MySqlDataReader = cuotasCommand.ExecuteReader()

                        If (cuotasReader.HasRows()) Then
                            flag = True
                        End If

                        'Para pagos parciales de las cuotas
                        Dim line = 0

                        While cuotasReader.Read()
                            Dim monto As Double = cuotasReader.GetDouble("monto")
                            Dim pago_parcial As Double = cuotasReader.GetDouble("pago_parcial")

                            Dim id_cuota As Integer = cuotasReader.GetInt32("id_cuota")
                            If (line > 0) Then
                                paym.Invoices.Add()
                            End If

                            'paym.Invoices.SetCurrentLine(line)
                            paym.Invoices.InvoiceType = 13
                            paym.Invoices.DocEntry = DocEntry
                            paym.Invoices.DocLine = line
                            paym.Invoices.InstallmentId = id_cuota
                            paym.Invoices.SumApplied = pago_parcial
                            'paym.Invoices.UserFields.Fields.Item("u_valor_real_cuota").Value = monto
                            'paym.userfields.fields.item("u_valor_real_cuota").value = valor_real_cuota

                            line += 1
                            flag = True
                        End While
                        cuotasReader.Close()


                        'Tarjetas y/o Convenios
                        Dim qTarjetas As String = "SELECT cod_conv as CreditCard,nombre as CreditCardName ,tipo,voucher,monto,DATE_FORMAT(fecha_acred,'%d/%m/%Y') AS fecha_acreed_lat,DATE_FORMAT(  DATE_ADD(fecha,INTERVAL 365 DAY),'%d/%m/%Y')  AS CardValidUntil, neto,DATE_FORMAT(fecha_ret,'%d/%m/%Y') AS fecha_ret,timbrado_ret FROM convenios WHERE f_nro = " & f_nro & " AND estado = 'Pendiente' AND e_sap IS NULL"
                        Dim tarjetasCommand As New MySqlCommand(qTarjetas, MysqlConnTarjetas)
                        Dim tarjReader As MySqlDataReader
                        tarjReader = tarjetasCommand.ExecuteReader()

                        Dim tarjetasI As Integer = 0

                        While tarjReader.Read()

                            Dim CreditCard As String = tarjReader.GetString("CreditCard")
                            Dim Voucher As String = tarjReader.GetString("voucher")
                            Dim Monto As Double = tarjReader.GetDouble("monto")
                            Dim FechaAcredit As String = tarjReader.GetString("fecha_acreed_lat")
                            Dim CardValidUntil As String = tarjReader.GetString("CardValidUntil")
                            Dim FechaRet As String = tarjReader.GetString("fecha_ret")
                            Dim Timbrado As String = tarjReader.GetString("timbrado_ret")

                            'paym.CreditCards.SetCurentLine(tarjetasI)

                            If (tarjetasI > 0) Then
                                paym.CreditCards.Add()
                            End If


                            paym.CreditCards.AdditionalPaymentSum = 0
                            paym.CreditCards.CardValidUntil = CardValidUntil

                            If (CreditCard = 17) Then
                                paym.CreditCards.CreditAcct = "1.1.2.3.10"  ' Tarjetas a Acreditar  Cuando es Retencion debe ser 1.1.2.3.10
                                paym.CreditCards.OwnerIdNum = Timbrado
                                paym.CreditCards.UserFields.Fields.Item("U_fecharet").Value = FechaRet
                            Else
                                paym.CreditCards.CreditAcct = "1.1.2.3.02"  ' Tarjetas a Acreditar 
                            End If


                            paym.CreditCards.CreditCard = CreditCard
                            paym.CreditCards.CreditCardNumber = Microsoft.VisualBasic.Left(Voucher, 64) 'Aqui Guardare el Voucher No tenemos el Numero de Tarjeta
                            paym.CreditCards.VoucherNum = Microsoft.VisualBasic.Left(Voucher, 20)

                            paym.CreditCards.CreditSum = Monto
                            paym.CreditCards.CreditType = 1  ' No se que es esto
                            paym.CreditCards.FirstPaymentDue = FechaAcredit
                            paym.CreditCards.FirstPaymentSum = Monto
                            paym.CreditCards.NumOfCreditPayments = 1
                            paym.CreditCards.NumOfPayments = 1
                            paym.CreditCards.PaymentMethodCode = 1

                            tarjetasI += 1
                            flag = True
                        End While
                        tarjReader.Close()
                        'Cheques
                        Dim qCheques As String = "SELECT nro_cheque as CheckNumber, id_banco AS BankCode,cuenta as AccounttNum,DATE_FORMAT(fecha_emis,'%d/%m/%Y') AS fecha_emis_lat,valor_ref FROM cheques_ter WHERE f_nro = " & f_nro & " AND estado = 'Pendiente' AND e_sap IS NULL ORDER BY nro_cheque ASC"

                        Dim chequesCommand As New MySqlCommand(qCheques, MysqlConnCheques)
                        Dim chqReader As MySqlDataReader = chequesCommand.ExecuteReader()

                        Dim chequesI As Integer = 0

                        While chqReader.Read()

                            Dim CheckNumber As Integer = chqReader.GetInt64("CheckNumber")
                            Dim AccounttNum As String = chqReader.GetString("AccounttNum")

                            Dim BankCode As String = chqReader.GetString("BankCode")

                            Dim Valor_ref As Double = chqReader.GetDouble("valor_ref")
                            Dim DueDate As String = chqReader.GetString("fecha_emis_lat")

                            'paym.Checks.SetCurentLine(chequesI)

                            If (chequesI > 0) Then
                                paym.Checks.Add()
                            End If

                            paym.Checks.AccounttNum = AccounttNum
                            paym.Checks.BankCode = BankCode
                            paym.Checks.CountryCode = "PY"

                            paym.Checks.Branch = suc
                            paym.Checks.CheckNumber = CheckNumber
                            paym.Checks.CheckSum = Valor_ref
                            paym.Checks.Details = Microsoft.VisualBasic.Left("Cobro " & CardName & "", 60)
                            paym.Checks.DueDate = CDate(DueDate)
                            paym.Checks.Trnsfrable = 0
                            paym.Checks.CheckAccount = "1.1.1.3.01"  ' Cheques Recibidos
                            'paym.Checks.CheckAccount = getCuentaContable("1.1.1.3 ", suc, moneda)

                            chequesI += 1
                            flag = True
                        End While
                        chqReader.Close()

                        If (flag) Then
                            iError = paym.Add
                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log("Error Pagos Recibidos por Factura Nro:" & f_nro & " " & " " & iError & "  " & sError)

                            Else
                                Dim NroEntrada As String = oCnn.GetNewObjectKey
                                log("Identificador de Pago DocEntry: " & NroEntrada & " Factura " & f_nro)
                                setPagoEnviado(f_nro)

                            End If
                        Else
                            log("Factura a Credito No se encontro ningun metodo de pago   " & f_nro & " ")
                            setPagoEnviado(f_nro)
                        End If
                        linea += 1
                    End While

                    myReader.Close()
                    MysqlConnPagos.Close()
                Catch ex As Exception
                    log("Error al Sincronizar Pago Factura: " & f_nro & " Mysql problem Pagos Recibidos" & ex.Message & " Factura: " & f_nro)
                End Try

            Catch ex As Exception
                log("Error COM Pagos Recibidos:   " & ex.Message & " Factura: " & f_nro)
                log("Error al Sincronizar Pago Factura Error COM: " & ex.Message & " Factura: " & f_nro)
            Finally
                'ejecutandoPagos = False
            End Try
            ejecutandoPagos = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonSincPagos_Click(sender As Object, e As EventArgs) Handles ButtonSincPagos.Click
        SincronizarPagosRecibidos()
    End Sub


    Public Function log(str As String) As Boolean

        'logTextArea.SelectionStart = logTextArea.Text.Length + 1
        Try
            Using w As StreamWriter = File.AppendText("SycnLog.txt")
                w.WriteLine("{0} {1}: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), str)
                ' w.WriteLine("  {0}", vbNewLine)
            End Using
        Catch ex As Exception

            Console.WriteLine("Error in Log: " & ex.Message)
        End Try
         
        Return True
    End Function

     
    Public Function SincronizarNotasCredito()
        If (Not ejecutandoNotasCredito) Then
            ejecutandoNotasCredito = True
            Dim MysqlConnNC As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnNCDet As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim orin As SAPbobsCOM.Documents

            Try
                MysqlConnNC.Open()
                MysqlConnNCDet.Open()
                Dim mySelectQuery As String = "SELECT n_nro, cod_cli, cliente, ruc_cli, usuario, req_auth, autorizado_por, f_nro, moneda,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat, hora, suc, tipo, total, saldo, fact_nro, pdv_cod, tipo_fact,vendedor,cat FROM nota_credito WHERE e_sap IS NULL AND estado = 'Cerrada'"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConnNC)

                Dim myReader As MySqlDataReader = myCommand.ExecuteReader()

                Try

                    While myReader.Read()

                        orin = oCnn.GetBusinessObject(BoObjectTypes.oCreditNotes)

                        Dim Nro = myReader.GetInt32("n_nro")
                        Dim FacturaInterna = myReader.GetInt32("f_nro")
                        Dim FacturaLegal = myReader.GetString("fact_nro")
                        Dim CardCode = myReader.GetString("cod_cli")
                        Dim CardName = myReader.GetString("cliente")
                        Dim usuario = myReader.GetString("usuario")
                        Dim vendedor = myReader.GetString("vendedor")
                        Dim autorizado_por = myReader.GetString("autorizado_por")
                        Dim cat = myReader.GetInt32("cat")

                        Dim Total = myReader.GetDouble("total")
                        Dim saldo = myReader.GetDouble("saldo")
                        Dim DocDueDate = myReader.GetString("fecha_lat")
                        Dim SUC = myReader.GetString("suc")
                        Dim PDV = myReader.GetString("pdv_cod")


                        orin.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                        orin.DocCurrency = "G$"
                        orin.Series = 10  'NNM1
                        orin.CardCode = CardCode
                        orin.CardName = Microsoft.VisualBasic.Left(CardName.ToString, 100) ' Max Length 100
                        orin.UserFields.Fields.Item("U_Usuario").Value = usuario
                        orin.Indicator = "03"

                        If FacturaLegal <> "" Then
                            orin.FolioPrefixString = "NC"
                            orin.FolioNumber = FacturaLegal
                            orin.VatPercent = IVA
                            orin.JournalMemo = Microsoft.VisualBasic.Left("N.C. (D) - " & CardCode & " " & CardName & "", 50) ' Max Length 60
                            orin.UserFields.Fields.Item("U_SER_PE").Value = PDV
                            orin.UserFields.Fields.Item("U_SER_EST").Value = estabs.Item(SUC)
                            orin.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "SI"
                            Dim timbrado As String = getTimbrado(FacturaLegal, SUC, PDV, "Nota de Credito")
                            orin.UserFields.Fields.Item("U_NUM_AUTOR").Value = timbrado
                        Else
                            orin.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
                            orin.VatPercent = 0
                            orin.JournalMemo = Microsoft.VisualBasic.Left("N.C. (ND) - " & CardCode & " " & CardName & "", 50)
                        End If

                        orin.UserFields.Fields.Item("U_vendedor").Value = vendedor
                        orin.UserFields.Fields.Item("U_CAT").Value = cat

                        orin.DocDate = Date.Now
                        orin.DocDueDate = DocDueDate
                        'orin.DocTotal = Total
                        orin.DiscountPercent = 0
                        orin.Comments = Microsoft.VisualBasic.Left("Autorizado por: " & autorizado_por & " Factura Interna: " & FacturaInterna, 100) ' MaxLength 100

                        'Obtener datos del Detalle de Reserva
                        Dim detalle As String = "SELECT codigo,lote,descrip,cantidad,precio_unit,um_prod,subtotal FROM nota_credito_det WHERE n_nro = " & Nro
                        Dim myDetCommand As New MySqlCommand(detalle, MysqlConnNCDet)
                        Dim myDetReader As MySqlDataReader = myDetCommand.ExecuteReader()

                        Dim AccountCode = getCuentaContable("4.1.1.1", SUC, "G$")

                        Dim line As Integer = 0
                        While myDetReader.Read()

                            Dim codigo = myDetReader.GetString("codigo")
                            Dim lote = myDetReader.GetString("lote")
                            Dim descrip = myDetReader.GetString("descrip")
                            Dim cantidad = myDetReader.GetDouble("cantidad")
                            Dim precio = myDetReader.GetDouble("precio_unit")
                            Dim subtotal = myDetReader.GetDouble("subtotal")

                            'Dim subt = precio * cantidad
                            'log(Total & " " & subtotal & " " & precio & "  " & cantidad & " " & subt)

                            If line > 0 Then
                                orin.Lines.Add()
                            End If

                            line += 1
                            orin.Lines.TaxCode = "IVA_EXE"  'Siempre es IVA_EXE

                            orin.Lines.ItemCode = codigo
                            orin.Lines.Quantity = cantidad
                            orin.Lines.Price = precio
                            orin.Lines.LineTotal = subtotal 'Total de la Linea

                            orin.Lines.ItemDescription = Microsoft.VisualBasic.Left(descrip, 100) ' MaxLength 100
                            orin.Lines.WarehouseCode = SUC
                            'orin.Lines.AccountCode = "4.1.1.1.01"  'Cuenta del Mayor
                            orin.Lines.AccountCode = AccountCode

                            'Lote
                            orin.Lines.BatchNumbers.BatchNumber = lote
                            orin.Lines.BatchNumbers.Quantity = cantidad
                            orin.Lines.BatchNumbers.Location = SUC
                        End While
                        myDetReader.Close()

                        iError = orin.Add()
                        If iError <> 0 Then
                            oCnn.GetLastError(iError, sError)
                            log(iError & "  " & sError)
                            'Throw New Exception(sError) ' No Cortar cuando hay error proseguir con las demas facturas
                        Else
                            Dim DocEntry As String = oCnn.GetNewObjectKey
                            log("Identificador Nota de Credito x Devolucion DocEntry: " & DocEntry)

                            Try
                                
                                Dim updQuery As String = "update nota_credito set e_sap = 1 where n_nro = " & Nro & ";"
                                MyExec(updQuery)
                            Catch myerror As Exception
                                log("Cannot connect to database on Reservas: " & myerror.Message)
                            End Try

                        End If

                    End While
                    myReader.Close()
                Catch ex As Exception
                    log("Error ORIN: " & ex.Message)
                End Try

                

            Catch ex As Exception
                log("Error COM: " & ex.Message)
            End Try

            MysqlConnNC.Close()
            MysqlConnNCDet.Close()

            ejecutandoNotasCredito = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonNotaCredito_Click(sender As Object, e As EventArgs) Handles ButtonNotaCredito.Click
        SincronizarNotasCredito()
    End Sub

    Private Function SincronizarReservas()
        If (Not ejecutandoReservas) Then
            ejecutandoReservas = True
            'Console.WriteLine("Sincronizando Reservas")

            'Verificar todas las Reservas que esten con estado = 'Pendiente' and e_sap = is null
            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnDet As MySqlConnection = New MySqlConnection(MySQLConnectionString)


            Dim IVA As Integer = 10

            Dim ordr As SAPbobsCOM.Documents


            Try
                MysqlConn.Open()
                MysqlConnDet.Open()


                Dim mySelectQuery As String = "SELECT nro_reserva,cod_cli,usuario,suc,fecha,DATE_FORMAT(fecha_cierre,'%d/%m/%Y') AS fecha_lat,ruc_cli,cliente,cat,valor_total_ref,minimo_senia_ref,senia_entrega_ref FROM reservas WHERE estado = 'Pendiente' AND e_sap IS NULL;"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                Try


                    While myReader.Read()
                        ordr = oCnn.GetBusinessObject(BoObjectTypes.oOrders)

                        Dim Nro = myReader.GetInt32("nro_reserva")
                        Dim CardCode = myReader.GetString("cod_cli")
                        Dim CardName = myReader.GetString("cliente")
                        Dim Vendedor = myReader.GetString("usuario")
                        Dim Total = myReader.GetDouble("valor_total_ref")
                        Dim Senia = myReader.GetDouble("senia_entrega_ref")
                        Dim DocDueDate = myReader.GetString("fecha_lat")
                        Dim SUC = myReader.GetString("suc")

                        ordr.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                        ordr.DocCurrency = "G$"
                        ordr.Series = 13    ' NNM1
                        ordr.CardCode = CardCode
                        ordr.CardName = Microsoft.VisualBasic.Left(CardName.ToString, 100) ' Max Length 100
                        ordr.UserFields.Fields.Item("U_vendedor").Value = Vendedor

                        ordr.DocRate = 1
                        'ordr.PaymentGroupCode = PaymentGroup ' Definir Tabla OCTG   -1 CONTADO, 1 CREDITO 

                        ordr.DocDate = Date.Now
                        ordr.DocDueDate = DocDueDate
                        ordr.DocTotal = Total
                        ordr.DiscountPercent = 0


                        'Obtener datos del Detalle de Reserva
                        Dim detalle As String = "SELECT codigo,lote,descrip,cantidad,precio,um,subtotal FROM reservas_det WHERE nro_reserva = " & Nro
                        Dim myDetCommand As New MySqlCommand(detalle, MysqlConnDet)
                        Dim myDetReader As MySqlDataReader
                        myDetReader = myDetCommand.ExecuteReader()

                        Dim line As Integer = 0
                        While myDetReader.Read()
                            'log("Linea " & line)
                            Dim codigo = myDetReader.GetString("codigo")
                            Dim lote = myDetReader.GetString("lote")
                            Dim descrip = myDetReader.GetString("descrip")
                            Dim cantidad = myDetReader.GetDouble("cantidad")
                            Dim precio = myDetReader.GetDouble("precio")
                            Dim subtotal = myDetReader.GetDouble("subtotal")

                            If line > 0 Then
                                ordr.Lines.Add()
                            End If

                            line += 1
                            ordr.Lines.TaxCode = "IVA_EXE"  'Siempre es IVA_EXE


                            ordr.Lines.ItemCode = codigo
                            ordr.Lines.Quantity = cantidad
                            ordr.Lines.Price = precio
                            ordr.Lines.LineTotal = subtotal 'Total de la Linea

                            ordr.Lines.ItemDescription = Microsoft.VisualBasic.Left(descrip, 100) ' MaxLength 100
                            ordr.Lines.WarehouseCode = SUC
                            ordr.Lines.AccountCode = "4.1.1.1.01"  'Cuenta del Mayor
                            'Lote
                            ordr.Lines.BatchNumbers.BatchNumber = lote
                            ordr.Lines.BatchNumbers.Quantity = cantidad
                            ordr.Lines.BatchNumbers.Location = SUC


                        End While
                        myDetReader.Close()

                        iError = ordr.Add
                        If iError <> 0 Then
                            oCnn.GetLastError(iError, sError)
                            log(iError & "  " & sError & "Reserva Nro: " & Nro)

                        Else
                            Dim DocEntry As String = oCnn.GetNewObjectKey
                            log("Identificador Reservas DocEntry: " & DocEntry)

                            Try

                                Dim updQuery As String = "update reservas set e_sap = 1 where nro_reserva = " & Nro & ";"
                                MyExec(updQuery)
                                 
                            Catch myerror As Exception
                                log("Cannot connect to database on Reservas: " & myerror.Message)
                            End Try

                        End If

                    End While

                Catch ex As Exception
                    log("Error Reservas: " & ex.Message)
                End Try

                myReader.Close()
                MysqlConn.Close()
            Catch myerror As Exception
                log("Cannot connect to database (F: SincronizarReservas): " & myerror.Message)
                MysqlConn.Close()
            End Try
            ejecutandoReservas = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonReservas_Click(sender As Object, e As EventArgs) Handles ButtonReservas.Click
        SincronizarReservas()
    End Sub



    Private Sub TimerStockXVentas_Tick(sender As Object, e As EventArgs) Handles TimerStockXVentas.Tick
        Try
            Dim ThreadFacturas As New Thread(AddressOf Me.SincronizarFacturasVentas)
            ThreadFacturas.Start()

        Catch ex As Exception
            log("ThreadFacturas " + ex.StackTrace)
        End Try
    End Sub

    Function getAbsEntry(codigo As String, lote As String) As Int32
        Dim ms_con As New SqlConnection
        ms_con.ConnectionString = MSSQLConnectionString
        ms_con.Open()

        Dim AbsEntry As Int32 = 0

        Try

            Dim sql As String = "SELECT o.AbsEntry as AbsEntry , i.ItemCode,i.BatchNum  FROM  OIBT i inner join  OBTN o on i.ItemCode = o.ItemCode and  o.DistNumber = i.BatchNum  inner join OITM it  on i.ItemCode = it.ItemCode and it.ItemCode = '" & codigo & "' and i.BatchNum = '" & lote & "'"
            Dim ms_cmd As New SqlCommand(sql, ms_con)

            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            If ms_reader.HasRows Then
                ms_reader.Read()
                AbsEntry = ms_reader(0)
            End If
            ms_reader.Close()

            Return AbsEntry

        Catch ex As Exception
            log("Error al obtener AbsEntry: " & ex.StackTrace)
            Return 0
        End Try
        ms_con.Close()

        Return AbsEntry
    End Function

    Public Function SincronizarLotes()
        If (Not ejecutandoLotes) Then
            ejecutandoLotes = True

            'Console.WriteLine("Sincronizando Lotes")
            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Try

                MysqlConn.Open()

                Dim sql = "SELECT id,codigo,lote,FP,f1,f2,f3,tara,gramaje,ancho,suc FROM edicion_lotes WHERE e_sap = 0"

                Dim myCommand As New MySqlCommand(sql, MysqlConn)

                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                While myReader.Read()

                    Dim oCompanyService As SAPbobsCOM.CompanyService = oCnn.GetCompanyService()

                    Dim oBatchNumberService As SAPbobsCOM.BatchNumberDetailsService
                    Dim oBatchNumberDetailParams As SAPbobsCOM.BatchNumberDetailParams
                    Dim oBatchNumberDetail As SAPbobsCOM.BatchNumberDetail
                    Dim inum As String

                    oBatchNumberService = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.BatchNumberDetailsService)
                    oBatchNumberDetailParams = oBatchNumberService.GetDataInterface(SAPbobsCOM.BatchNumberDetailsServiceDataInterfaces.bndsBatchNumberDetailParams)

                    Dim id As Int32 = myReader.GetInt32("id")
                    Dim codigo = myReader.GetString("codigo")
                    Dim lote = myReader.GetString("lote")
                    Dim suc = myReader.GetString("suc")

                    Dim FP As String = Nothing
                    Dim F1 As Double = Nothing
                    Dim F2 As Double = Nothing
                    Dim F3 As Double = Nothing
                    Dim tara As Int32 = Nothing
                    Dim gramaje As Double = Nothing
                    Dim ancho As Double = Nothing

                    If Not myReader.IsDBNull(3) Then
                        FP = myReader.GetString("FP")
                    End If
                    If Not myReader.IsDBNull(4) Then
                        F1 = myReader.GetDouble("f1")
                    End If
                    If Not myReader.IsDBNull(5) Then
                        F2 = myReader.GetDouble("f2")
                    End If
                    If Not myReader.IsDBNull(6) Then
                        F3 = myReader.GetDouble("f3")
                    End If
                    If Not myReader.IsDBNull(7) Then
                        tara = myReader.GetInt32("tara")
                    End If
                    If Not myReader.IsDBNull(8) Then
                        gramaje = myReader.GetDouble("gramaje")
                    End If
                    If Not myReader.IsDBNull(9) Then
                        ancho = myReader.GetDouble("ancho")
                    End If


                    Dim AbsEntry = getAbsEntry(codigo, lote)
                    If (AbsEntry > 0) Then
                        oBatchNumberDetailParams.DocEntry = AbsEntry 'AbsEntry
                        oBatchNumberDetail = oBatchNumberService.Get(oBatchNumberDetailParams)
                        inum = oBatchNumberDetail.Batch
                        'oBatchNumberDetail.Status = BoDefaultBatchStatus.dbs_Locked

                        If (FP <> Nothing) Then
                            'oBatchNumberDetail.UserFields().Item("U_fin_pieza").Value = FP
                        End If
                        If (F1 <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_F1").Value = F1
                        End If
                        If (F2 <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_F2").Value = F2
                        End If
                        If (F3 <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_F3").Value = F3
                        End If
                        If (tara <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_tara").Value = tara
                        End If
                        If (gramaje <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_gramaje").Value = gramaje
                        End If
                        If (ancho <> Nothing) Then
                            oBatchNumberDetail.UserFields().Item("U_ancho").Value = ancho
                        End If
                        Try
                            'oBatchNumberService.Update(oBatchNumberDetail)

                            Try

                                oBatchNumberService.Update(oBatchNumberDetail)

                                MsExcec("UPDATE OIBT SET U_fin_pieza = '" & FP & "'  WHERE ItemCode = '" & codigo & "' and BatchNum = '" & lote & "' and WhsCode = '" & suc & "'")

                                 
                                Dim updQuery As String = "UPDATE edicion_lotes SET e_sap = 1 WHERE id =  " & id & ";"
                                MyExec(updQuery)
                                 
                            Catch myerror As Exception
                                log("Cannot update BatchNumbers: " & myerror.Message)
                            End Try

                        Catch myerror As Exception
                            log("2-Cannot update BatchNumbers: " & myerror.Message)
                        End Try

                    Else
                        log("Error al actualizar Codigo: " & codigo & " lote :" & lote & " AbsEntry: " & AbsEntry)
                    End If


                End While
                myReader.Close()

            Catch ex As Exception
                log("BatchNumber Update error:  " & ex.StackTrace)
            End Try
            MysqlConn.Close()
            ejecutandoLotes = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonUpdateBatchNumber_Click(sender As Object, e As EventArgs) Handles ButtonUpdateBatchNumber.Click
        SincronizarLotes()
    End Sub

    Public Function SincronizarAjustesPositivos()
        If (Not ejecutandoAjustesPositivos) Then
            ejecutandoAjustesPositivos = True
            'Console.WriteLine("Sincronizando Ajustes (+)")
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
             
            Dim entrada As SAPbobsCOM.Documents

            Dim id_ajuste = 0

            Try
                MyConn.Open()
 

                Dim detalle As String = "SELECT id_ajuste,usuario,codigo,lote,tipo,ajuste,p_costo,valor_ajuste,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,um,suc,estado FROM ajustes WHERE ajuste > 0 AND signo = '+' AND  f_nro = 0 and e_sap = 0 limit 1"
                Dim myDetCommand As New MySqlCommand(detalle, MyConn)
                Dim myDetReader As MySqlDataReader = myDetCommand.ExecuteReader()

                If (myDetReader.HasRows) Then

                    'Ajustes Positivos
                    entrada = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenEntry)

                    While myDetReader.Read()

                        id_ajuste = myDetReader.GetString("id_ajuste")
                        Dim codigo = myDetReader.GetString("codigo")
                        Dim lote = myDetReader.GetString("lote")
                        'Dim um_prod = myDetReader.GetString("um_prod")
                        Dim ajuste = myDetReader.GetDouble("ajuste")
                        Dim um = myDetReader.GetString("um")
                        Dim usuario = myDetReader.GetString("usuario")
                        Dim precio_neto = myDetReader.GetDouble("p_costo")
                        Dim subtotal = myDetReader.GetDouble("valor_ajuste")
                        Dim Tipo = myDetReader.GetString("tipo")
                        Dim Motivo = myDetReader.GetString("motivo")
                        Dim fecha_lat = myDetReader.GetString("fecha_lat")
                        Dim suc = myDetReader.GetString("suc")

                        entrada.Reference2 = id_ajuste
                        entrada.Series = 25  ' NNM1
                        entrada.UserFields.Fields.Item("U_Nro_Interno").Value = id_ajuste
                        entrada.UserFields.Fields.Item("U_Usuario").Value = usuario
                        entrada.UserFields.Fields.Item("U_SUC").Value = suc

                        entrada.DocCurrency = "G$"
                        'entrada.DocRate = 1
                        'entrada.GroupNumber = -1 'Ultimo Precio
                        entrada.DocDate = Date.Now
                        entrada.DocDueDate = fecha_lat
                        entrada.JournalMemo = Microsoft.VisualBasic.Left("Ajuste (+) Nro:" & id_ajuste & " " & Tipo & " ", 50) ' Max Length 50

                        entrada.Comments = Microsoft.VisualBasic.Left("Constrol Empaque Ajuste (+) Nro:" & id_ajuste & "  " & Tipo & " " & Motivo & "", 254) ' Max Length 254


                        entrada.Lines.ItemCode = codigo
                        entrada.Lines.WarehouseCode = suc
                        entrada.Lines.Quantity = ajuste
                        'entrada.Lines.Price = precio_neto
                        entrada.Lines.UnitPrice = precio_neto
                        'entrada.Lines.LineTotal = subtotal 'Total de la Linea


                        entrada.Lines.AccountCode = "4.1.2.1.07"  ' Sobrante en Stock

                        'Lote
                        entrada.Lines.BatchNumbers.BatchNumber = lote
                        entrada.Lines.BatchNumbers.Quantity = ajuste

                        iError = entrada.Add
                        If iError <> 0 Then
                            oCnn.GetLastError(iError, sError)
                            log(iError & "  " & sError & "  ID Ajuste " & id_ajuste)

                            MyExec("update ajustes set e_sap = 3 where id_ajuste = " & id_ajuste & " ;")
                            
                        Else
                            Dim DocEntry As String = oCnn.GetNewObjectKey
                            log("Ajuste Positivo Identificador DocEntry: " & DocEntry)

                            Try
                                Dim updQuery As String = "update ajustes set e_sap = 1 where id_ajuste = " & id_ajuste & " ;" ' Actualizar Ajustes 
                                MyExec("update ajustes set e_sap = 1 where id_ajuste = " & id_ajuste & " ;")
                                 
                            Catch myerror As Exception
                                log("Cannot connect to database (F:SincronizarAjustesPositivos): " & myerror.Message)
                            End Try

                        End If

                    End While

                End If
                myDetReader.Close()

            Catch ex As Exception
                log("Error  Ajustes (+): " & ex.Message & "  ID Ajuste " & id_ajuste)
                MyExec("update ajustes set e_sap = 3 where id_ajuste = " & id_ajuste & " ;") ' Actualizar Ajustes  
                 
            End Try

            MyConn.Close()

            ejecutandoAjustesPositivos = False
        End If
        Return Nothing
    End Function


    Private Sub ButtonAjustes_Click(sender As Object, e As EventArgs) Handles ButtonAjustes.Click ' Solo Ajustes que no son por Factura
        SincronizarAjustesPositivos()
    End Sub
    Public Function SincronizarAjustesNegativos()
        If (Not ejecutandoAjustesNegativos) Then
            ejecutandoAjustesNegativos = True

            'Console.WriteLine("Sincronizando Ajustes (-)")
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            ' Dim MyAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim salida As SAPbobsCOM.Documents

            Dim id_ajuste = 0

            Try
                MyConn.Open()



                Dim detalle As String = "SELECT id_ajuste,usuario,codigo,lote,tipo,ajuste,p_costo,valor_ajuste,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,um,suc,estado FROM ajustes WHERE ajuste > 0 AND signo = '-' AND  f_nro = 0 AND e_sap = 0 limit 1"
                Dim myDetCommand As New MySqlCommand(detalle, MyConn)
                Dim myDetReader As MySqlDataReader = myDetCommand.ExecuteReader()

                If (myDetReader.HasRows) Then

                    salida = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenExit)
                    'Ajustes Negativos

                    While myDetReader.Read()

                        id_ajuste = myDetReader.GetString("id_ajuste")
                        Dim codigo = myDetReader.GetString("codigo")
                        Dim lote = myDetReader.GetString("lote")
                        'Dim um_prod = myDetReader.GetString("um_prod")
                        Dim ajuste = myDetReader.GetDouble("ajuste")
                        Dim um = myDetReader.GetString("um")
                        Dim usuario = myDetReader.GetString("usuario")
                        Dim precio_neto = myDetReader.GetDouble("p_costo")
                        Dim subtotal = myDetReader.GetDouble("valor_ajuste")
                        Dim Tipo = myDetReader.GetString("tipo")
                        Dim Motivo = myDetReader.GetString("motivo")
                        Dim DocDueDate = myDetReader.GetString("fecha_lat")
                        Dim suc = myDetReader.GetString("suc")



                        'Control de stock para que no descuente negativo
                        Dim StockReal = getStockLote(codigo, lote, suc)
                        If (ajuste > StockReal) Then
                            ajuste = StockReal
                        End If

                        salida.Reference2 = id_ajuste
                        salida.Series = 26  ' NNM1

                        salida.UserFields.Fields.Item("U_Nro_Interno").Value = id_ajuste
                        salida.UserFields.Fields.Item("U_Usuario").Value = usuario
                        salida.UserFields.Fields.Item("U_SUC").Value = suc

                        salida.DocCurrency = "G$"
                        'salida.DocRate = Cotiz

                        'salida.GroupNumber = -1 'Ultimo Precio
                        salida.DocDate = Date.Now
                        salida.DocDueDate = DocDueDate
                        salida.JournalMemo = Microsoft.VisualBasic.Left("Ajuste (-) Nro:" & id_ajuste & " " & Tipo & " ", 50) ' Max Length 50
                        salida.Comments = Microsoft.VisualBasic.Left("Constrol Empaque Ajuste (-) Nro:" & id_ajuste & " " & Tipo & " " & Motivo & " ", 254) ' Max Length 254


                        salida.Lines.ItemCode = codigo
                        salida.Lines.Quantity = ajuste
                        'salida.Lines.Price = precio_neto
                        salida.Lines.UnitPrice = precio_neto
                        'salida.Lines.LineTotal = subtotal 'Total de la Linea
                        salida.Lines.WarehouseCode = suc

                        salida.Lines.AccountCode = "6.1.4.1.17"  'Cuenta del Mayor Mermas en Stock

                        'Lote
                        salida.Lines.BatchNumbers.BatchNumber = lote
                        salida.Lines.BatchNumbers.Quantity = ajuste

                        iError = salida.Add

                        If iError <> 0 Then
                            oCnn.GetLastError(iError, sError)
                            log(iError & "  " & sError & "  ID Ajuste " & id_ajuste)

                            MyExec("update ajustes set e_sap = 3 where id_ajuste = " & id_ajuste & " ;")
                            
                        Else
                            Dim DocEntry As String = oCnn.GetNewObjectKey
                            log("Ajuste Negativo Identificador DocEntry: " & DocEntry)

                            Try

                                MyExec("update ajustes set e_sap = 1 where id_ajuste = " & id_ajuste & " ;")
                               
                            Catch myerror As Exception
                                log("Cannot connect to database: " & myerror.Message)
                            End Try

                        End If

                    End While
                End If
                myDetReader.Close()
            Catch ex As Exception
                log("Error Ajustes (-): " & ex.Message & "  ID Ajuste " & id_ajuste)
                MyExec("update ajustes set e_sap = 3 where id_ajuste = " & id_ajuste & " ;") ' Actualizar Ajustes                  
            End Try

            MyConn.Close()
            ejecutandoAjustesNegativos = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonAjustesNeg_Click(sender As Object, e As EventArgs) Handles ButtonAjustesNeg.Click
        SincronizarAjustesNegativos()
    End Sub

    Public Function SincronizarRemisiones()
        If (Not ejecutandoRemisiones) Then
            ejecutandoRemisiones = True
            'Console.WriteLine("Sincronizando Remisiones")


            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnDet As MySqlConnection = New MySqlConnection(MySQLConnectionString)


            Try
                Dim owtr As SAPbobsCOM.IStockTransfer 'Nota de Remision

                MysqlConn.Open()
                MysqlConnDet.Open()

                Try

                    Dim mySelectQuery As String = "select n_nro, DATE_FORMAT(fecha,'%d/%m/%Y') as fecha_lat,hora,usuario,recepcionista,suc,suc_d,DATE_FORMAT(fecha_cierre,'%d/%m/%Y') as fecha_cierre_lat,hora_cierre,obs FROM nota_remision where estado = 'Cerrada' and e_sap = 0;"
                    Dim cmd As New MySqlCommand(mySelectQuery, MysqlConn)

                    Dim myReader As MySqlDataReader = cmd.ExecuteReader()

                    While myReader.Read()

                        owtr = oCnn.GetBusinessObject(BoObjectTypes.oStockTransfer)

                        Dim Nro = myReader.GetInt32("n_nro")

                        Dim fecha_lat = myReader.GetString("fecha_lat")
                        Dim hora = myReader.GetString("hora")
                        Dim usuario = myReader.GetString("usuario")
                        Dim recepcionista = myReader.GetString("recepcionista")
                        Dim origen = myReader.GetString("suc")
                        Dim destino = myReader.GetString("suc_d")
                        Dim fecha_cierre_lat = myReader.GetString("fecha_cierre_lat")
                        Dim hora_cierre = myReader.GetString("hora_cierre")
                        Dim obs = myReader.GetString("obs")

                        'Controlar si este Numero ya no hasido enviado con anterioridad

                        If chequearNotaRemision(Nro) Then  ' OWTR
                            log("La Nota Remision Nro: " & Nro & " ya ha sido enviada a SAP con Anterioridad no se puede enviar...")
                            MyExec("UPDATE nota_remision set e_sap = 1 where f_nro = " & Nro)
                        Else

                            owtr.UserFields.Fields.Item("U_Nro_Interno").Value = Nro
                            owtr.DocDate = Date.Now
                            owtr.DueDate = Date.Now 'fecha_cierre_lat
                            'owtr.VehiclePlate = '########## Interesante Implementar ########
                            owtr.Series = 27
                            owtr.PriceList = -1

                            'owtr.CardCode = CardCode
                            'owtr.CardName = Microsoft.VisualBasic.Left(CardName.ToString, 100) ' Max Length 100
                            owtr.UserFields.Fields.Item("U_vendedor").Value = usuario
                            owtr.UserFields.Fields.Item("U_Usuario").Value = recepcionista
                            owtr.FromWarehouse = origen
                            owtr.ToWarehouse = destino

                            owtr.JournalMemo = Microsoft.VisualBasic.Left("Traslado Merc. Nro: " & Nro & " " & origen & "=>" & destino, 50) ' MaxLength 50 Caracteres Esto va en el Diario
                            owtr.Comments = Microsoft.VisualBasic.Left(obs, 50) ' MaxLength  50 Caracteres



                            'Obtener datos del Detalle de Reserva
                            Dim detalle As String = "SELECT codigo,lote,um_prod,descrip,cantidad,cant_inicial,gramaje,ancho,kg_env,kg_rec,cant_calc_env,cant_calc_rec,tara FROM nota_rem_det WHERE  e_sap = 0 and n_nro = " & Nro
                            Dim myDetCommand As New MySqlCommand(detalle, MysqlConnDet)
                            Dim myDetReader As MySqlDataReader
                            myDetReader = myDetCommand.ExecuteReader()


                            Dim line As Integer = 0
                            While myDetReader.Read()
                                'log("Linea " & line)
                                Dim codigo = myDetReader.GetString("codigo")
                                Dim lote = myDetReader.GetString("lote")
                                Dim um_prod = myDetReader.GetString("um_prod")
                                Dim descrip = myDetReader.GetString("descrip")
                                Dim cantidad = myDetReader.GetDouble("cantidad")
                                Dim cant_inicial = myDetReader.GetDouble("cant_inicial")
                                Dim gramaje = myDetReader.GetDouble("gramaje")
                                Dim ancho = myDetReader.GetDouble("ancho")
                                Dim kg_env = myDetReader.GetDouble("kg_env")
                                Dim kg_rec = myDetReader.GetDouble("kg_rec")
                                Dim cant_calc_env = myDetReader.GetDouble("cant_calc_env")
                                Dim cant_calc_rec = myDetReader.GetDouble("cant_calc_rec")
                                Dim tara = myDetReader.GetDouble("tara")

                                If line > 0 Then
                                    owtr.Lines.Add()
                                End If
                                line += 1

                                owtr.Lines.ItemCode = codigo
                                owtr.Lines.Quantity = cantidad
                                owtr.Lines.ItemDescription = Microsoft.VisualBasic.Left(descrip, 100) ' MaxLength 100

                                owtr.Lines.BatchNumbers.BatchNumber = lote
                                owtr.Lines.BatchNumbers.Quantity = cantidad

                                owtr.Lines.UserFields.Fields.Item("U_cant_inicial").Value = cant_inicial
                                owtr.Lines.UserFields.Fields.Item("U_gramaje").Value = gramaje
                                owtr.Lines.UserFields.Fields.Item("U_ancho").Value = ancho
                                owtr.Lines.UserFields.Fields.Item("U_kg_env").Value = kg_env
                                owtr.Lines.UserFields.Fields.Item("U_kg_rec").Value = kg_rec
                                owtr.Lines.UserFields.Fields.Item("U_cant_calc_env").Value = cant_calc_env
                                owtr.Lines.UserFields.Fields.Item("U_cant_calc_rec").Value = cant_calc_rec
                                owtr.Lines.UserFields.Fields.Item("U_tara").Value = tara
                            End While
                            myDetReader.Close()

                            iError = owtr.Add
                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log("Error Remisiones  " & iError & "  " & sError & " Nro: " & Nro)
                                MyExec("UPDATE nota_remision set e_sap = 3 where n_nro = " & Nro)
                            Else
                                Dim DocEntry As String = oCnn.GetNewObjectKey
                                log("Identificador Remidiones DocEntry: " & DocEntry)

                                Try
                                    Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)
                                    MysqlAux.Open()
                                    Dim updQuery As String = "update nota_remision set e_sap = 1 where n_nro = " & Nro & ";"
                                    Dim updCommand As New MySqlCommand(updQuery, MysqlAux)
                                    updCommand.ExecuteNonQuery()
                                    MysqlAux.Close()
                                Catch myerror As Exception
                                    log("Cannot connect to database on Reservas: " & myerror.Message)
                                End Try

                            End If

                        End If

                    End While
                    myReader.Close()
                    MysqlConn.Close()
                    MysqlConnDet.Close()

                Catch ex As Exception
                    log("Error COM: " & ex.Message)
                End Try

            Catch myerror As Exception
                log("Cannot connect to database: " & myerror.Message)
            End Try

            ejecutandoRemisiones = False
        End If
        Return Nothing
    End Function

    Private Sub ButtonRemisiones_Click(sender As Object, e As EventArgs) Handles ButtonRemisiones.Click
        SincronizarRemisiones()
    End Sub

    Private Sub TimerAjustesPos_Tick(sender As Object, e As EventArgs) Handles TimerAjustesPos.Tick
        Try
            Dim ThreadAjustesPositivos As New Thread(AddressOf Me.SincronizarAjustesPositivos)
            ThreadAjustesPositivos.Start()
        Catch ex As Exception
            log("Error en ThreadAjustesPositivos " & ex.StackTrace)
        End Try

    End Sub

    Private Sub TimerAjustesNeg_Tick(sender As Object, e As EventArgs) Handles TimerAjustesNeg.Tick
        Try
            Dim ThreadAjustesNegativos As New Thread(AddressOf Me.SincronizarAjustesNegativos)
            ThreadAjustesNegativos.Start()
        Catch ex As Exception
            log("Error en ThreadAjustesNegativos " & ex.StackTrace)
        End Try
    End Sub

    Public Function SincronizarCompras()
        If (Not ejecutandoCompras) Then
            ejecutandoCompras = True

            'Console.WriteLine("Sincronizando Compras")
            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnDet As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            

            Dim doc As SAPbobsCOM.Documents


            Try
                MysqlConn.Open()
                MysqlConnDet.Open()


                Dim mySelectQuery As String = "select id_ent, suc,invoice,tipo_doc_sap,cod_prov,proveedor,date_format(fecha_fact,'%d/%m/%Y') as fecha_fact,moneda,cotiz,pais_origen,estado,coment,usuario,timbrado from  entrada_merc where estado = 'Cerrada' and e_sap is null;"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()


                While myReader.Read()
                    Dim ref = myReader.GetInt32("id_ent")
                    Dim suc = myReader.GetString("suc")
                    Dim invoice = myReader.GetString("invoice")
                    Dim tipo_doc_sap = myReader.GetString("tipo_doc_sap")
                    Dim CardCode = myReader.GetString("cod_prov")
                    Dim CardName = myReader.GetString("proveedor")
                    Dim fecha = myReader.GetString("fecha_fact")
                    Dim moneda = myReader.GetString("moneda")
                    Dim cotiz = myReader.GetDouble("cotiz")
                    Dim codigo_pais_origen = myReader.GetString("pais_origen")
                    Dim estado = myReader.GetString("estado")
                    Dim coment = myReader.GetString("coment")
                    Dim usuario = myReader.GetString("usuario")
                    Dim timbrado = myReader.GetString("timbrado")
                    Dim origen As String = "Internacional"

                    If (codigo_pais_origen.Equals("PY")) Then
                        origen = "Nacional"
                    End If

                    Dim pais_origen = getCountryName(codigo_pais_origen)

                    If (tipo_doc_sap.Equals("OPDN")) Then
                        doc = oCnn.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes)
                        doc.VatPercent = 0
                        doc.Series = 17  'NNM1  
                        doc.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
                        doc.JournalMemo = substring("E.M.C. (ND) - " & CardCode & " " & CardName & "", 50)
                        doc.UserFields.Fields.Item("U_Estado").Value = "En Transito"
                    ElseIf (tipo_doc_sap.Equals("OPCH")) Then 'OPCH
                        'doc = oCnn.GetBusinessObject(BoObjectTypes.oPurchaseInvoices)   14 Es la serie para oPurchaseInvoices
                        doc = oCnn.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes)
                        doc.Series = 17  'NNM1 14
                        doc.VatPercent = 0 'IVA Siempre es 0
                        doc.JournalMemo = substring("F.P. (D) - " & CardCode & " " & CardName & "", 50)
                        doc.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "SI"
                        doc.UserFields.Fields.Item("U_NUM_AUTOR").Value = timbrado
                        'doc.UserFields.Fields.Item("U_SER_PE").Value = PDV
                        doc.UserFields.Fields.Item("U_SER_EST").Value = estabs.Item(suc)
                        doc.UserFields.Fields.Item("U_Estado").Value = "En Transito"
                    Else 'OIGN 
                        doc = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenEntry)
                        doc.Series = 25  'NNM1 Obj 59
                        doc.VatPercent = 0 'IVA
                        doc.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
                        doc.JournalMemo = substring("E.M.I. (ND) - " & CardCode & " " & CardName & "", 50)
                        doc.UserFields.Fields.Item("U_Estado").Value = "Cerrada"
                    End If

                    If (tipo_doc_sap.Equals("OPDN") Or tipo_doc_sap.Equals("OPCH")) Then
                        doc.NumAtCard = invoice
                        doc.CardCode = CardCode
                        doc.CardName = CardName
                        doc.DocCurrency = moneda
                        doc.DocRate = cotiz
                    Else ' oign
                        doc.Comments = "Sobrante de Mercaderias Factura/Invoice (" & invoice & ")"
                    End If


                    doc.FolioNumber = ref

                    doc.DocDate = fecha
                    doc.Comments = coment


                    doc.UserFields.Fields.Item("U_Nro_Interno").Value = ref
                    doc.UserFields.Fields.Item("U_SUC").Value = suc

                    doc.UserFields.Fields.Item("U_Usuario").Value = usuario
                    doc.UserFields.Fields.Item("U_Pais_Origen").Value = pais_origen
                    doc.UserFields.Fields.Item("U_Origen").Value = origen

                    'Obtener datos del Detalle de Reserva
                    Dim detalle As String = "SELECT id_det, nro_pedido, id_pack,store_no, bale,if( piece is not null,piece,0) as piece, codigo, lote, descrip, um, cod_catalogo, fab_color_cod, precio, cantidad, subtotal, color, color_comb, design,composicion, ancho, gramaje,obs,um_prod,cant_calc,cod_pantone, nro_lote_fab FROM entrada_det WHERE id_ent = " & ref & " ORDER BY store_no asc,bale asc,codigo asc,color asc;"
                    Dim myDetCommand As New MySqlCommand(detalle, MysqlConnDet)
                    Dim myDetReader As MySqlDataReader
                    myDetReader = myDetCommand.ExecuteReader()


                    Dim AccountCode = getCuentaContable("1.1.3.1", suc, moneda)

                    Dim line As Integer = 0
                    While myDetReader.Read()

                        Dim id_det = myDetReader.GetInt32("id_det")
                        Dim nro_pedido = myDetReader.GetInt32("nro_pedido")
                        Dim id_pack = myDetReader.GetInt32("id_pack")
                        Dim store_no = myDetReader.GetString("store_no")
                        Dim bale = myDetReader.GetInt32("bale")
                        Dim piece = myDetReader.GetInt32("piece")
                        Dim codigo = myDetReader.GetString("codigo")
                        Dim lote = myDetReader.GetString("lote")
                        Dim descrip = myDetReader.GetString("descrip")
                        Dim umc = myDetReader.GetString("um")

                        '      log("Linea " & line & "  " & codigo & " " & descrip)

                        Dim fab_color_cod = myDetReader.GetString("cod_catalogo") & "-" & myDetReader.GetString("fab_color_cod")
                        Dim precio = myDetReader.GetDouble("precio")
                        Dim cantidad = myDetReader.GetDouble("cantidad")
                        Dim subtotal = myDetReader.GetDouble("subtotal")


                        Dim color = myDetReader.GetString("color")
                        Dim color_comb = myDetReader.GetString("color_comb")
                        Dim design = myDetReader.GetString("design")
                        Dim composicion = myDetReader.GetString("composicion")
                        Dim gramaje = myDetReader.GetDouble("gramaje")
                        Dim ancho = myDetReader.GetDouble("ancho")
                        Dim obs = myDetReader.GetString("obs")
                        Dim um_prod = myDetReader.GetString("um_prod")
                        Dim cant_calc = myDetReader.GetDouble("cant_calc")
                        Dim cod_pantone = myDetReader.GetString("cod_pantone")
                        Dim nro_lote_fab = myDetReader.GetString("nro_lote_fab")


                        If line > 0 Then
                            doc.Lines.Add()
                        End If
                        'doc.Lines.SetCurrentLine(line)
                        line += 1


                        doc.Lines.TaxCode = "IVA_EXE"  ' Siempre es IVA_EXE Segun mail de Arnaldo
                        If (tipo_doc_sap.Equals("OPDN") Or tipo_doc_sap.Equals("OIGN")) Then
                            doc.Lines.TaxLiable = BoYesNoEnum.tYES
                        Else
                            doc.Lines.TaxLiable = BoYesNoEnum.tNO
                        End If
                        doc.Lines.ItemCode = codigo
                        doc.Lines.Quantity = cant_calc

                        doc.Lines.Price = precio
                        doc.Lines.LineTotal = subtotal   ' Total de Linea
                        doc.Lines.UserFields.Fields.Item("U_cant_inicial").Value = cant_calc
                        doc.Lines.UserFields.Fields.Item("U_color").Value = cod_pantone
                        doc.Lines.UserFields.Fields.Item("U_ancho").Value = ancho
                        doc.Lines.UserFields.Fields.Item("U_gramaje").Value = gramaje

                        doc.Lines.ItemDescription = substring(descrip, 100) ' MaxLength 100                        
                        doc.Lines.WarehouseCode = suc
                        doc.Lines.CostingCode = suc

                        'doc.Lines.AccountCode = "1.1.3.1.01" ' Cuenta del Mayor  Mercaderias

                        doc.Lines.AccountCode = AccountCode

                        '// Lote
                        doc.Lines.BatchNumbers.BatchNumber = lote
                        doc.Lines.BatchNumbers.Quantity = cant_calc
                        doc.Lines.BatchNumbers.Location = suc
                        'doc.Lines.BatchNumbers.Status = 2   '0 Liberado,  1 Acceso Denegado   2 Bloqueado
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comercial").Value = cod_pantone
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_quty_c_um").Value = cantidad  ' Cantidad Real Comprada
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comb").Value = color_comb
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_cod_fabric").Value = fab_color_cod
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_design").Value = design
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_prov_mar").Value = store_no
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_umc").Value = umc
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_bag").Value = bale
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_ancho").Value = ancho
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_gramaje").Value = gramaje
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_nro_lote_fab").Value = nro_lote_fab ' Nro de Teñido o Lote de Fabricacion
                        doc.Lines.BatchNumbers.UserFields.Fields.Item("U_estado_venta").Value = "Normal"

                    End While
                    myDetReader.Close()

                    iError = doc.Add
                    If iError <> 0 Then
                        oCnn.GetLastError(iError, sError)
                        log("Error Entrada de Mercaderias " & iError & "  " & sError)
                    Else
                        Dim DocEntry As String = oCnn.GetNewObjectKey
                        log("Identificador Entrada de Mercaderias DocEntry: " & DocEntry)
                        bloquearLotes(DocEntry)

                        Try
                            Dim updQuery As String = "update entrada_merc set e_sap = 1, sap_doc = " & DocEntry & "  where id_ent = " & ref & ";"
                            MyExec(updQuery)
                        Catch myerror As Exception
                            log("Cannot connect to database: " & myerror.Message)
                        End Try
                    End If


                    'log((Nro & ", " & myReader & ", " & myReader("cod_cli") & ", " & myReader("cliente")))
                End While
                MysqlConn.Close()
                MysqlConnDet.Close()

            Catch ex As Exception
                log("Error Entrada de Mercaderias 2:  " & ex.Message & " STACK TRACE: " & ex.StackTrace)
            End Try
            ejecutandoCompras = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonSincCompras_Click(sender As Object, e As EventArgs) Handles ButtonSincCompras.Click
        SincronizarCompras()
    End Sub

    Private Function bloquearLotes(DocEntry As Integer)
        'Try

        '    Using ms_con As New SqlClient.SqlConnection(MSSQLConnectionString)

        '        ms_con.Open()

        '        Dim updt1 As String = "UPDATE OIBT set Status = 2 where BaseEntry = " & DocEntry

        '        Dim ms_cmd As New SqlCommand(updt1, ms_con)
        '        ms_cmd.ExecuteNonQuery()
        '        ms_con.Close()

        '    End Using
        '    Using ms_con As New SqlClient.SqlConnection(MSSQLConnectionString)

        '        ms_con.Open()

        '        Dim updt2 As String = "UPDATE OBTN set Status = 2 from OIBT i, OBTN o where  i.BatchNum = o.DistNumber AND i.BaseEntry = " & DocEntry

        '        Dim ms_cmd As New SqlCommand(updt2, ms_con)
        '        ms_cmd.ExecuteNonQuery()
        '        ms_con.Close()

        '        log("Ok Lotes Bloqueados...")
        '    End Using


        'Catch ex As Exception
        '    log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        'End Try
        Return 1
    End Function

    Private Sub TimerCotiz_Tick(sender As Object, e As EventArgs) Handles TimerCotiz.Tick
        'Verifica si se ha corregido la cotizacion cada 5 Segundos en caso de que no no levanta los demas Timers
        'iniciarTimmers()
    End Sub

    Private Sub TimerCompras_Tick(sender As Object, e As EventArgs) Handles TimerCompras.Tick
        Try
            Dim ThreadCompras As New Thread(AddressOf Me.SincronizarCompras)
            ThreadCompras.Start()
        Catch ex As Exception
            log("Error en ThreadCompras " & ex.StackTrace)
        End Try
    End Sub



    Private Sub ButtonMigrarArticulos_Click(sender As Object, e As EventArgs) Handles ButtonMigrarLotes.Click
        ' Pasos Ejecutar esto en el Server
        ' SET SESSION old_passwords=0;
        ' SET PASSWORD FOR douglas=PASSWORD('case');
        ' Abrir PuTTy ir a SSH Tunels agregar Source Port 3306 Destination 127.0.0.1:3306 Add
        ' Detener MySQL Local
        ' Detener firewall services iptables stop


        'If (Not ejecutando) Then

        '    LIMITE = TextBoxLimite.Text

        '    ejecutando = True
        '    Dim indexA = 0
        '    Dim indexB = 9660
        '    Dim usuario_sap As String = sap_user.Text

        '    If (usuario_sap = "Sistema") Then
        '        indexA = 0
        '        indexB = 9860
        '    ElseIf (usuario_sap = "Develop") Then
        '        indexA = 0
        '        indexB = 10000
        '    End If


        '    Dim inicio = DateTime.Now
        '    log(inicio)


        '    Dim fallas As String = ""
        '    Dim correctas As Integer = 0
        '    Dim firstID = 0
        '    Dim lastID = 0

        '    Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)

        '    Try
        '        Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)


        '        MysqlConn.Open()

        '        ' Corregir valores nulos
        '        '
        '        'MysqlAux.Open()
        '        'Dim sql_correction As String = "select corregir_datos_prod();"
        '        'Dim updcommand As New MySqlCommand(sql_correction, MysqlAux)
        '        'updcommand.ExecuteNonQuery()
        '        'MysqlAux.Close()


        '        Dim doc As SAPbobsCOM.Documents

        '        Dim msql As New SqlConnection(MSSQLConnectionString)
        '        msql.Open()


        '        'Dim cabecera = "SELECT DISTINCT c_ref,c_prov, UPPER(prov_nombre) AS proveedor,c_nac_int AS tipo FROM mnt_prod p, mov_compras c, mnt_prov v WHERE p.p_ref = c_ref AND c.c_prov = prov_cod AND p_cant > 0 AND  p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' and prod_fin_pieza != 'Tr' )   AND p_sap_cod in (" & sap_codes & ")  AND p.p_migrated IS NULL AND c.p_migrated IS NULL  GROUP BY p_ref order by p_ref desc LIMIT 50000"
        '        Dim cabecera = "SELECT DISTINCT c_ref,c_prov, UPPER(prov_nombre) AS proveedor,c_nac_int AS tipo FROM mnt_prod p, mov_compras c, mnt_prov v WHERE p.p_ref = c_ref AND c.c_prov = prov_cod AND p_cant > 0 AND  p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' and prod_fin_pieza != 'Tr' )   AND p_sap_cod IS NOT NULL AND p.p_migrated IS NULL AND c.p_migrated IS NULL AND p_ref BETWEEN " & indexA & " AND " & indexB & "   GROUP BY p_ref order by p_ref desc LIMIT 50000"

        '        ' Se excluye los de Doña Victoria

        '        Dim myCommand As New MySqlCommand(cabecera, MysqlConn)

        '        Dim readerCab As MySqlDataReader
        '        readerCab = myCommand.ExecuteReader()



        '        lastID = 0

        '        While readerCab.Read()
        '            Dim linea_actual = 0
        '            Dim c_ref = readerCab.GetInt32("c_ref")
        '            Dim c_prov = readerCab.GetInt32("c_prov")
        '            Dim proveedor = readerCab.GetString("proveedor")
        '            Dim tipo = readerCab.GetString("tipo")

        '            'Create the Inventory Gen Entry object
        '            doc = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenEntry) 'Cambiar por OIQI
        '            doc.Series = 25  'NNM1 Obj 59
        '            doc.VatPercent = 0
        '            doc.Reference2 = c_ref
        '            doc.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
        '            doc.JournalMemo = substring("Mig. Fact.:" & c_ref & ", Prov:" & c_prov & ", " & proveedor & " " & tipo, 50)
        '            doc.Comments = substring("Migracion Fact.:" & c_ref & ", Prov:" & c_prov & ", " & proveedor & " " & tipo, 254)

        '            doc.UserFields.Fields.Item("U_Nro_Interno").Value = c_ref
        '            doc.UserFields.Fields.Item("U_SUC").Value = "00"
        '            doc.UserFields.Fields.Item("U_Estado").Value = "Cerrada"
        '            doc.UserFields.Fields.Item("U_Usuario").Value = "douglas"

        '            doc.UserFields.Fields.Item("U_Origen").Value = tipo
        '            If (tipo = "Nacional") Then
        '                doc.UserFields.Fields.Item("U_Pais_Origen").Value = "Paraguay"
        '            Else
        '                doc.UserFields.Fields.Item("U_Pais_Origen").Value = "China"
        '            End If
        '            Dim MysqlConnProd As MySqlConnection
        '            MysqlConnProd = New MySqlConnection(MySQLConnectionStringMarijoa)
        '            MysqlConnProd.Open()
        '            Try

        '                ' Se excluye los FP y Tr
        '                'Dim mySelectQuery As String = "SELECT id,p_ref,p_sap_cod, p_cod AS lote,p_padre, p_factura, p_local , p_cant,p_cant_compra, ROUND( p_compra + ( (p_compra * p_porc_recargo) / 100)) AS p_compra, p_valmin, p_precio_1, p_precio_2, p_precio_3, p_precio_4, p_precio_5, p_precio_6, p_precio_7, p_ancho,p_gram, p_gram_m,p_tara,CONCAT(IF(p_foto = 0,0, p_ref) ,'/',p_foto) AS img,p_echo_en,p_color,p_pantone,prod_fin_pieza,p_descri FROM mnt_prod WHERE p_cant > 0 AND p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' and prod_fin_pieza != 'Tr' ) AND p_sap_cod  in (" & sap_codes & ")  AND p_migrated IS NULL and p_ref = " & c_ref & " GROUP BY p_ref,p_cod order by id asc  LIMIT " & LIMITE & " "
        '                Dim mySelectQuery As String = "SELECT id,p_ref,p_sap_cod, p_cod AS lote,p_padre, p_factura, p_local , p_cant,p_cant_compra, ROUND( p_compra + ( (p_compra * p_porc_recargo) / 100)) AS p_compra, p_valmin, p_precio_1, p_precio_2, p_precio_3, p_precio_4, p_precio_5, p_precio_6, p_precio_7, p_ancho,p_gram, p_gram_m,if(p_tara is null,0, p_tara) as p_tara,CONCAT(IF(p_foto = 0,0, p_ref) ,'/',p_foto) AS img,p_echo_en,p_color,p_pantone,prod_fin_pieza,p_descri,IF((p_descri REGEXP '\\({1,2}F[1-9]{1}[1-7]{0,1}\\){1,2}'),1,0) AS Fx, IF((p_descri REGEXP '\\({1,2}F[1-9]{1}[1-7]{0,1}\\){1,2}'),p_descri,'') AS  Notas FROM mnt_prod WHERE p_cant > 0 AND p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' and prod_fin_pieza != 'Tr' ) AND p_sap_cod IS NOT NULL  AND p_migrated IS NULL and p_ref = " & c_ref & " GROUP BY p_ref,p_cod order by id asc  LIMIT " & LIMITE & " "

        '                'Console.Write(mySelectQuery)

        '                Dim myCommandDet As New MySqlCommand(mySelectQuery, MysqlConnProd)
        '                Dim readerProd As MySqlDataReader
        '                readerProd = myCommandDet.ExecuteReader()
        '                Dim line = 0
        '                If (readerProd.HasRows()) Then
        '                    While readerProd.Read()

        '                        'Dim p_ref As Int16 = readerProd.GetInt16("p_ref")
        '                        Dim ItemCode = readerProd.GetString("p_sap_cod")
        '                        Dim lote = readerProd.GetString("lote")
        '                        'existeLote(lote)
        '                        If (existeLote(lote) < 1) Then 'Si este lote existe debe pasar al siguiente sin migrar
        '                            lastID = readerProd.GetInt32("id")
        '                            If (line = 0) Then
        '                                firstID = readerProd.GetInt32("id")
        '                            End If

        '                            Dim p_padre = readerProd.GetString("p_padre")
        '                            Dim factura = readerProd.GetString("p_factura")
        '                            Dim suc = readerProd.GetString("p_local")
        '                            Dim p_cant = readerProd.GetDouble("p_cant")
        '                            Dim p_cant_compra = readerProd.GetDouble("p_cant_compra")
        '                            Dim p_compra = readerProd.GetDouble("p_compra")
        '                            Dim p_valmin = readerProd.GetDouble("p_valmin")
        '                            Dim p_precio_1 = readerProd.GetInt32("p_precio_1")
        '                            Dim p_precio_2 = readerProd.GetInt32("p_precio_2")
        '                            Dim p_precio_3 = readerProd.GetInt32("p_precio_3")
        '                            Dim p_precio_4 = readerProd.GetInt32("p_precio_4")
        '                            Dim p_precio_5 = readerProd.GetInt32("p_precio_5")
        '                            Dim p_precio_6 = readerProd.GetInt32("p_precio_6")
        '                            Dim p_precio_7 = readerProd.GetInt32("p_precio_7")

        '                            Dim desc1 As Double = 0
        '                            Dim desc2 As Double = 0
        '                            Dim desc3 As Double = 0
        '                            Dim desc4 As Double = 0
        '                            Dim desc5 As Double = 0
        '                            Dim desc6 As Double = 0
        '                            Dim desc7 As Double = 0



        '                            Dim ancho = readerProd.GetDouble("p_ancho")
        '                            Dim gramaje = readerProd.GetDouble("p_gram")
        '                            Dim gramaje_m = readerProd.GetDouble("p_gram_m")
        '                            Dim tara = readerProd.GetInt32("p_tara")
        '                            Dim img = readerProd.GetString("img")
        '                            Dim color = readerProd.GetString("p_color")
        '                            Dim Pantone = readerProd.GetString("p_pantone")
        '                            Dim estado = readerProd.GetString("prod_fin_pieza")
        '                            Dim p_descri = readerProd.GetString("p_descri")
        '                            Dim Fx = readerProd.GetInt32("Fx")
        '                            Dim Notas = readerProd.GetString("Notas")
        '                            'log(">> " & linea_actual)
        '                            linea_actual += 1

        '                            ' Obtener el Codigo Pantone
        '                            'Dim queryCheck As String = "select Code as Pantone from [@EXX_COLOR_COMERCIAL] where Name = '" & color & "'"
        '                            'Dim ms_cmd As New SqlCommand(queryCheck, msql)
        '                            'Dim reader As SqlDataReader = ms_cmd.ExecuteReader()

        '                            'Dim Pantone As String = color
        '                            'If (reader.HasRows()) Then
        '                            'reader.Read()
        '                            'pantone = reader(0)
        '                            'End If
        '                            'reader.Close()
        '                            If (Pantone.Length < 1) Then
        '                                Pantone = color
        '                            End If

        '                            'Descuentos para establecer precios correctos
        '                            Dim queryPrecios As String = "SELECT PriceList,Price from  ITM1 WHERE ItemCode = '" & ItemCode & "' and PriceList < 8"
        '                            Dim ms_cmd_precios As New SqlCommand(queryPrecios, msql)
        '                            Dim readerPrecios As SqlDataReader = ms_cmd_precios.ExecuteReader()

        '                            Try

        '                                While readerPrecios.Read()

        '                                    Dim PriceList = readerPrecios("PriceList")
        '                                    Dim ItemPrice = readerPrecios("Price")
        '                                    If (ItemPrice = 0) Then
        '                                        readerProd.Close()
        '                                        readerPrecios.Close()

        '                                        log("Precio " & PriceList & " NO ESTABLECIDO para Articulo " & ItemCode)

        '                                        Throw New System.Exception("Precio " & PriceList & " NO ESTABLECIDO para Articulo " & ItemCode)
        '                                    End If

        '                                    If PriceList = 1 Then
        '                                        desc1 = 100 - ((p_precio_1 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 2 Then
        '                                        desc2 = 100 - ((p_precio_2 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 3 Then
        '                                        desc3 = 100 - ((p_precio_3 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 4 Then
        '                                        desc4 = 100 - ((p_precio_4 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 5 Then
        '                                        desc5 = 100 - ((p_precio_5 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 6 Then
        '                                        desc6 = 100 - ((p_precio_6 * 100) / ItemPrice)
        '                                    End If
        '                                    If PriceList = 7 Then
        '                                        desc7 = 100 - ((p_precio_7 * 100) / ItemPrice)
        '                                    End If
        '                                End While
        '                                readerPrecios.Close()
        '                            Catch ex As Exception
        '                                log("Precio  NO ESTABLECIDO para Articulo " & ItemCode)
        '                                readerPrecios.Close()
        '                            Finally
        '                                readerPrecios.Close()
        '                            End Try
        '                            ' log(desc1 & " " & desc2 & " " & desc3 & " " & desc4 & " " & desc5 & " " & desc6 & " " & desc7)


        '                            If line > 0 Then
        '                                doc.Lines.Add()
        '                            End If
        '                            line += 1



        '                            doc.Lines.TaxCode = "IVA_EXE"  ' Siempre es IVA_EXE 
        '                            doc.Lines.TaxLiable = BoYesNoEnum.tYES
        '                            doc.Lines.ItemCode = ItemCode
        '                            doc.Lines.Quantity = p_cant

        '                            Dim subtotal = p_compra * p_cant

        '                            doc.Lines.Price = p_compra
        '                            doc.Lines.LineTotal = subtotal  ' Total de Linea
        '                            doc.Lines.UserFields.Fields.Item("U_cant_inicial").Value = p_cant_compra
        '                            doc.Lines.UserFields.Fields.Item("U_color").Value = Pantone
        '                            doc.Lines.UserFields.Fields.Item("U_ancho").Value = ancho
        '                            doc.Lines.UserFields.Fields.Item("U_gramaje").Value = gramaje


        '                            'doc.Lines.ItemDescription = substring(descrip, 100) ' MaxLength 100                        
        '                            doc.Lines.WarehouseCode = suc

        '                            doc.Lines.AccountCode = "2.1.1.6.01" ' Cuenta del Mayor  SALDOS INICIALES

        '                            Dim notas_ As String = substring("Migrado " & Notas, 140)

        '                            '// Lote
        '                            doc.Lines.BatchNumbers.BatchNumber = lote
        '                            doc.Lines.BatchNumbers.Quantity = p_cant
        '                            doc.Lines.BatchNumbers.Location = suc
        '                            'doc.Lines.BatchNumbers.Status = 2   '0 Liberado,  1 Acceso Denegado   2 Bloqueado
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comercial").Value = substring(Pantone, 30)
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_quty_c_um").Value = p_cant  ' Cantidad Real Comprada
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_equiv").Value = p_cant  ' Cantidad Real Comprada
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comb").Value = substring(color, 30)

        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_color_cod_fabric").Value = substring(p_descri, 10)
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_design").Value = "No definido"
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_prov_mar").Value = c_prov.ToString
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_umc").Value = "Mts"
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_bag").Value = 1
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_ancho").Value = ancho
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_gramaje").Value = gramaje
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_nro_lote_fab").Value = "1" ' Nro de Teñido o Lote de Fabricacion
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_img").Value = img
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_padre").Value = p_padre
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_notas").Value = notas_
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_fin_pieza").Value = "No"
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_tara").Value = tara

        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_F1").Value = Fx


        '                            Dim EstadoVenta = "Normal"
        '                            If (estado = "R") Then
        '                                EstadoVenta = "Retazo"
        '                            Else
        '                                Dim terminacion As Integer = Integer.Parse(Microsoft.VisualBasic.Right(lote, 2))
        '                                If (terminacion <= 13) Then
        '                                    estado = "Oferta"
        '                                End If
        '                            End If


        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_estado_venta").Value = EstadoVenta

        '                            'Descuentos
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc1").Value = desc1
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc2").Value = desc2
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc3").Value = desc3
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc4").Value = desc4
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc5").Value = desc5
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc6").Value = desc6
        '                            doc.Lines.BatchNumbers.UserFields.Fields.Item("U_desc7").Value = desc7
        '                        Else
        '                            MysqlAux.Open()
        '                            Dim updQuery As String = "update mnt_prod set p_migrated = 1 where p_cod = '" & lote & "' and p_sap_cod IS NOT NULL ;"
        '                            Dim updCommandx As New MySqlCommand(updQuery, MysqlAux)
        '                            updCommandx.ExecuteNonQuery()
        '                            MysqlAux.Close()
        '                        End If

        '                    End While
        '                End If  'HasRows()


        '                Dim nErr As Long
        '                Dim errMsg As String = ""
        '                If (line > 0) Then ' Solo si hay lineas
        '                    nErr = doc.Add()
        '                    If (0 <> nErr) Then
        '                        Call oCnn.GetLastError(nErr, errMsg)
        '                        fallas = fallas & "," & c_ref
        '                        log("Failed to migrate Ref: " & c_ref & "  " & nErr & "  " & errMsg)
        '                        log("Max Line: " & line)
        '                        fallas_migracion += 1
        '                        GC.Collect()
        '                    Else
        '                        'Dim objCode As String

        '                        Dim DocEntry As String = oCnn.GetNewObjectKey

        '                        MysqlAux.Open()
        '                        Dim updQuery As String = "update mnt_prod set p_migrated = 1 where p_ref = " & c_ref & " and p_sap_cod IS NOT NULL and id >= " & firstID & " and id <= " & lastID & ";"
        '                        Dim updCommandx As New MySqlCommand(updQuery, MysqlAux)
        '                        updCommandx.ExecuteNonQuery()
        '                        MysqlAux.Close()
        '                        log("Migracion exitosa:  Ref: " & c_ref & " DocEntrey = " & DocEntry)
        '                        correctas += 1
        '                        GC.Collect()
        '                    End If
        '                Else
        '                    GC.Collect()
        '                End If
        '                readerProd.Close()
        '                MysqlConnProd.Close()
        '            Catch ex As Exception
        '                log("Error  2 : " & c_ref & " Linea: " & linea_actual & " " & ex.Message & "  DETALLE STACK TRACE: " & ex.StackTrace & " " & ex.Source)
        '                fallas_migracion += 1

        '            End Try

        '        End While

        '        readerCab.Close()

        '    Catch ex As Exception

        '        log("Error Global : 1   " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '    End Try

        '    log("Fin de Migracion!")
        '    log("Fallas: " & fallas)
        '    log("Correctas: " & correctas)
        '    log("Fallas: " & fallas)
        '    log("Correctas: " & correctas)

        '    log("#####################  Actualizando Series con los nuevos lotes  #####################")
        '    Try
        '        Dim sql_server_conn As New SqlConnection(MSSQLConnectionString)
        '        Dim sql_server_upd As New SqlConnection(MSSQLConnectionString)
        '        sql_server_conn.Open()
        '        sql_server_upd.Open()

        '        Dim ms_cmd As New SqlCommand("select Name from [@SERIES_LOTES]", sql_server_conn)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim YearCode As String = ms_reader.GetString(0)
        '            Dim ms_max As New SqlCommand("select top 1 BatchNum AS MAXBTN from OIBT where BatchNum like '%" & YearCode & "'  and BatchNum not like '%R%' order by BatchNum + 0 desc", sql_server_upd)
        '            Dim max_reader As SqlDataReader = ms_max.ExecuteReader()
        '            Dim maximo As String = 1
        '            If (max_reader.HasRows) Then
        '                max_reader.Read()
        '                maximo = max_reader.GetString(0)
        '            Else
        '                maximo = 1
        '            End If

        '            If (maximo > 1) Then
        '                Dim ps As String = maximo.ToString
        '                Dim longitud = maximo.ToString.Length

        '                Dim purecode As String = Microsoft.VisualBasic.Left(maximo, longitud - 2)
        '                'log("Maximo " & YearCode & "  purecode " & purecode)
        '                If (IsNumeric(purecode)) Then
        '                    Dim NexCode = CInt(purecode) + 1
        '                    updateSeries(YearCode, NexCode)
        '                Else
        '                    log("No Numeric  " & YearCode)
        '                End If
        '            Else
        '                updateSeries(YearCode, 1)
        '            End If
        '            max_reader.Close()
        '        End While

        '        ms_reader.Close()
        '        sql_server_upd.Close()
        '        sql_server_conn.Close()

        '    Catch ex As Exception
        '        log("Error al Actualizar Series: " & ex.StackTrace)
        '    End Try

        '    Dim fin = DateTime.Now
        '    log(fin)
        '    Dim tiempo As Integer
        '    tiempo = DateDiff(DateInterval.Minute, inicio, fin)
        '    log("Tiempo Transcurrido Migracion Lotes: " & tiempo & " minutos")
        '    ejecutando = False
        '    If (fallas_migracion > 0) Then
        '        LIMITE = 500
        '        ERROR_MIGRACION += 1
        '        If (ERROR_MIGRACION < 10) Then
        '            log("Ocurrieron Fallas: Intentanto con " & LIMITE)
        '            ButtonMigrarLotes.PerformClick()
        '        End If
        '    Else 'Verificar si hay mas para Migrar
        '        Try
        '            MysqlAux.Open()
        '            Dim cantQuery As String = "SELECT  COUNT(p_cod) AS cant  FROM mnt_prod p, mov_compras c, mnt_prov v WHERE p.p_ref = c_ref AND c.c_prov = prov_cod AND p_cant > 0 AND  p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' AND prod_fin_pieza != 'Tr' )   AND p_sap_cod IS NOT NULL AND p.p_migrated IS NULL "
        '            Dim updCommandx As New MySqlCommand(cantQuery, MysqlAux)
        '            Dim myReader As MySqlDataReader
        '            myReader = updCommandx.ExecuteReader()
        '            myReader.Read()
        '            Dim cant = myReader.GetInt32("cant")
        '            If (cant > 0) Then
        '                'MysqlAux.Close()
        '                'ButtonMigrarLotes.PerformClick()
        '            End If
        '            MysqlAux.Close()
        '        Catch ex As Exception
        '            log("Error en el control de cantidad de facturas no migradas " & ex.StackTrace)
        '        End Try
        '    End If
        'End If
    End Sub
    Function updateSeries(YearCode As String, NexCode As Integer)
        Try
            Dim upd_conn As New SqlConnection(MSSQLConnectionString)
            upd_conn.Open()
            Dim sql As String = "update  [@SERIES_LOTES] set U_Serie = " & NexCode & " where Name = '" & YearCode & "'"
            Dim ms_max_upd As New SqlCommand(sql, upd_conn)
            ms_max_upd.ExecuteNonQuery()
            upd_conn.Close()
            log("Serie " & YearCode & "  Actualizada a:  " & NexCode)
        Catch ex As Exception
            log("Error al Actualizar Series: YearCode " & YearCode & ex.StackTrace)
        End Try
        Return True
    End Function
    Private Function existeLote(Lote As Integer) As Integer
        Return 0
        'Try
        '    Dim msql As New SqlConnection(MSSQLConnectionString)
        '    msql.Open()
        '    Dim queryCheck As String = "select count(*) as Cant from OIBT where BatchNum =  '" & Lote & "'"
        '    Dim ms_cmd As New SqlCommand(queryCheck, msql)
        '    Dim reader As SqlDataReader = ms_cmd.ExecuteReader()

        '    Dim Cant As Integer = 0
        '    If (reader.HasRows()) Then
        '        reader.Read()
        '        Cant = reader(0)
        '    End If
        '    reader.Close()
        '    msql.Close()
        '    Return Cant
        'Catch ex As Exception
        '    log("Error Consultar existencia de lote " & Lote & " " & ex.StackTrace)
        '    Return 0
        'End Try

    End Function

    Private Sub ButtonMigrarClientes_Click(sender As Object, e As EventArgs) Handles ButtonMigrarClientes.Click
        'Dim MySQLConnectionStringMarijoa = "server=127.0.0.1; port=3306;user id=plus; password=case; database=marijoa" ' marijoa
        'Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        'Try
        '    Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

        '    MysqlConn.Open()

        '    Dim mySelectQuery As String = "SELECT cod_cli, tipo_doc, ci_ruc, nombre, cat, suc, tel, email,date_format(fecha_nac,'%d-%m-%Y') as fecha_nac, pais, estado, ciudad, dir, ocupacion, situacion, e_sap FROM clientes WHERE e_sap = 0"

        '    Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

        '    Dim myReaderCli As MySqlDataReader
        '    myReaderCli = myCommand.ExecuteReader()

        '    Dim msql As New SqlConnection(MSSQLConnectionString)
        '    msql.Open()


        '    'Create the BusinessPartners object
        '    Dim obp As SAPbobsCOM.BusinessPartners
        '    obp = oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners) 'Calls BusinessPartners object

        '    While myReaderCli.Read()

        '        Dim cod_cli = myReaderCli.GetString("cod_cli")
        '        Dim tipo_doc = myReaderCli.GetString("tipo_doc")
        '        Dim ci_ruc = myReaderCli.GetString("ci_ruc")
        '        Dim nombre = myReaderCli.GetString("nombre")
        '        Dim cat = myReaderCli.GetInt32("cat")
        '        Dim suc = myReaderCli.GetString("suc")
        '        Dim tel = myReaderCli.GetString("tel")
        '        Dim email = myReaderCli.GetString("email")
        '        Dim fecha_nac = myReaderCli.GetString("fecha_nac")
        '        Dim pais = myReaderCli.GetString("pais")
        '        Dim estado = myReaderCli.GetString("estado")
        '        Dim ciudad = myReaderCli.GetString("ciudad")
        '        Dim dir = myReaderCli.GetString("dir")
        '        Dim ocupacion = myReaderCli.GetString("ocupacion")
        '        Dim situacion = myReaderCli.GetString("situacion")
        '        Dim tipo = "local"

        '        If ci_ruc.IndexOf("-") Then
        '            tipo = "local"
        '        Else
        '            tipo = "extranjero"
        '        End If

        '        ' Consultar en la base actual si ruc ya no existe

        '        Dim queryCheck As String = "SELECT COUNT(*) AS CANT_RUC,CardCode FROM OCRD WHERE LicTradNum = '" & ci_ruc & "' group by LicTradNum,CardCode"

        '        Dim ms_cmd As New SqlCommand(queryCheck, msql)
        '        Dim reader As SqlDataReader = ms_cmd.ExecuteReader()



        '        If (reader.HasRows() = False) Then
        '            ' log("################### Cliente encontrado " & ci_ruc & "  " & nombre)
        '            obp.CardName = nombre

        '            obp.CardCode = "C01"

        '            obp.CardType = 0
        '            obp.FederalTaxID = ci_ruc

        '            'obp.Series = 0
        '            obp.Series = 65
        '            obp.Phone1 = tel
        '            obp.City = ciudad
        '            obp.Indicator = "01"
        '            obp.BilltoDefault = "1"
        '            obp.Address = dir
        '            obp.PriceListNum = cat
        '            obp.Currency = "G$"

        '            If (pais.ToUpper = "BRASIL") Then
        '                obp.Country = "BR"
        '                tipo_doc = "R.G."
        '            ElseIf (pais.ToUpper = "ARGENTINA") Then
        '                obp.Country = "AR"
        '                tipo_doc = "D.N.I."
        '            Else
        '                obp.Country = "PY"
        '                tipo_doc = "C.I."
        '            End If

        '            obp.AdditionalID = ocupacion  '// Funciono una vez AddID            
        '            obp.UnifiedFederalTaxID = situacion  '// VatIdUnCmp

        '            If tipo = "local" Then
        '                obp.DebitorAccount = "1.1.2.2.01"
        '            Else
        '                obp.DebitorAccount = "1.1.2.2.02" '// Clientes del Exterior
        '            End If

        '            '// FechaNac
        '            obp.UserFields.Fields.Item("U_fecha_nac").Value = fecha_nac
        '            obp.UserFields.Fields.Item("U_tipo_doc").Value = tipo_doc

        '            Dim nErr As Long
        '            Dim errMsg As String = ""

        '            If (0 <> obp.Add()) Then

        '                Call oCnn.GetLastError(nErr, errMsg)


        '                log("Failed to add an business partner " & ci_ruc & "  " & nErr & " " & errMsg)
        '            Else
        '                'Dim objCode As String

        '                MyExec("update clientes set e_sap = 1 where cod_cli = '" & cod_cli & "';") 

        '                log("Cliente Migrado con exito " & ci_ruc & "  " & nombre)
        '            End If
        '        Else
        '            ' Verificar si el RUC Tiene * y Quitarle
        '            reader.Read()
        '            Dim CardCode As String = reader(1)
        '            If (ci_ruc.IndexOf("*") > -1) Then
        '                Try
        '                    Dim ci_ruc_sin_asterisco As String = ci_ruc.Replace("*", "").Replace("/", "")
        '                    If (obp.GetByKey(CardCode) = True) Then
        '                        obp.FederalTaxID = ci_ruc_sin_asterisco

        '                        Dim nErr As Long
        '                        Dim errMsg As String = ""

        '                        If (0 <> obp.Update()) Then
        '                            Call oCnn.GetLastError(nErr, errMsg)
        '                            log("Error al actualizar RUC Cliente" & nErr & "" & errMsg)
        '                        Else
        '                            log("RUC Cliente actualizado " & ci_ruc & " " & ci_ruc_sin_asterisco)
        '                        End If

        '                    End If
        '                Catch ex As Exception
        '                    log("Error al actualizar RUC Cliente" & ex.StackTrace)
        '                End Try
        '            End If

        '            MyExec("update clientes set e_sap = 1 where cod_cli = '" & cod_cli & "';")

        '        End If

        '        reader.Close()

        '    End While
        '    log("FIN DE MIGRACION DE CLIENTES!!!")

        '    myReaderCli.Close()
        'Catch ex As Exception
        '    log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)

        '    ' Me chupa un Guevo el error segui ejecutando!!!
        '    ButtonMigrarClientes.PerformClick()
        'End Try
    End Sub
    Public Function SincronizarClientes()
        If (Not ejecutandoClientes) Then
            ejecutandoClientes = True
            'Console.WriteLine("Sincronizando clientes")
            Dim MysqlConnClientes As MySqlConnection
            MysqlConnClientes = New MySqlConnection(MySQLConnectionString)
            Try
                 
                MysqlConnClientes.Open()

                Dim mySelectQuery As String = "SELECT cod_cli, tipo_doc,tipo, ci_ruc, nombre, cat, suc, tel, email,date_format(fecha_nac,'%d-%m-%Y') as fecha_nac, pais, estado, ciudad, dir, ocupacion, situacion,usuario, e_sap FROM clientes WHERE e_sap = 0"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConnClientes)

                Dim myReaderCli As MySqlDataReader
                myReaderCli = myCommand.ExecuteReader()

                Dim msql As New SqlConnection(MSSQLConnectionString)
                msql.Open()


                'Create the BusinessPartners object
                Dim obp As SAPbobsCOM.BusinessPartners
                obp = oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners) 'Calls BusinessPartners object

                While myReaderCli.Read()

                    Dim cod_cli = myReaderCli.GetString("cod_cli")
                    Dim tipo_doc = myReaderCli.GetString("tipo_doc")
                    Dim ci_ruc = myReaderCli.GetString("ci_ruc")
                    Dim nombre = myReaderCli.GetString("nombre")
                    Dim cat = myReaderCli.GetInt32("cat")
                    Dim suc = myReaderCli.GetString("suc")
                    Dim tel = myReaderCli.GetString("tel")
                    Dim email = myReaderCli.GetString("email")
                    Dim fecha_nac = myReaderCli.GetString("fecha_nac")
                    Dim pais = myReaderCli.GetString("pais")
                    Dim estado = myReaderCli.GetString("estado")
                    Dim ciudad = myReaderCli.GetString("ciudad")
                    Dim dir = myReaderCli.GetString("dir")
                    Dim ocupacion = myReaderCli.GetString("ocupacion")
                    Dim situacion = myReaderCli.GetString("situacion")
                    Dim usuario = myReaderCli.GetString("usuario")
                    Dim tipo = myReaderCli.GetString("tipo")

                    ' Consultar en la base actual si ruc ya no existe

                    Dim queryCheck As String = "SELECT COUNT(*) AS CANT_RUC FROM OCRD WHERE LicTradNum = '" & ci_ruc & "' and LicTradNum <> '80005190-4'"  ' Por seguridad nada mas El del MEC esta Permitido

                    Dim ms_cmd As New SqlCommand(queryCheck, msql)
                    Dim reader As SqlDataReader = ms_cmd.ExecuteReader()
                    reader.Read()
                    Dim cant_rucs As Integer = reader(0)
                    reader.Close()

                    If (cant_rucs < 1) Then

                        obp.CardName = nombre
                        obp.CardCode = "C01" 'cod_cli

                        obp.CardType = 0
                        obp.FederalTaxID = ci_ruc
                        obp.Series = 65
                        obp.Phone1 = tel
                        obp.City = ciudad
                        obp.Indicator = "01"
                        obp.BilltoDefault = "1"
                        obp.Address = dir
                        obp.AdditionalID = ocupacion  '// Funciono una vez AddID            
                        obp.UnifiedFederalTaxID = situacion  '// VatIdUnCmp
                        obp.Currency = "G$"

                        If tipo = "local" Then
                            obp.DebitorAccount = "1.1.2.2.01"
                        Else
                            obp.DebitorAccount = "1.1.2.2.02" '// Clientes del Exterior
                        End If

                        If (tipo_doc = "C.I. Diplomatica") Then
                            obp.GroupCode = 102
                        Else
                            obp.GroupCode = 100
                        End If

                        '// FechaNac
                        obp.UserFields.Fields.Item("U_fecha_nac").Value = fecha_nac
                        obp.UserFields.Fields.Item("U_tipo_doc").Value = tipo_doc
                        obp.UserFields.Fields.Item("U_usuario").Value = usuario
                        obp.UserFields.Fields.Item("U_suc").Value = suc

                        Dim nErr As Long
                        Dim errMsg As String = ""

                        If (0 <> obp.Add()) Then

                            Call oCnn.GetLastError(nErr, errMsg)

                            log("Failed to add an business partner " & ci_ruc & "  " & nErr & " " & errMsg)
                        Else
                            'Dim objCode As String
                            
                            MyExec("update clientes set e_sap = 1 where cod_cli = '" & cod_cli & "';")


                            log("Cliente Migrado con exito " & ci_ruc & "  " & nombre)
                        End If
                    Else
                        MyExec("update clientes set e_sap = 1 where cod_cli = '" & cod_cli & "';") 

                        'log("Cliente migrado con anterioridad " & ci_ruc & "  " & nombre)
                    End If

                End While
                myReaderCli.Close()
                MysqlConnClientes.Close()
                msql.Close()
            Catch ex As Exception
                log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
                reconnect()
            End Try
            ejecutandoClientes = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonSincClientes_Click(sender As Object, e As EventArgs) Handles ButtonSincClientes.Click
        SincronizarClientes()
    End Sub


    Private Sub ButtonMigrarDatosBasicos_Click(sender As Object, e As EventArgs) Handles ButtonMigrarArticulos.Click
        'If (Not ejecutando) Then
        '    ejecutando = True
        '    Dim MSSQLConnectionStringPrueba = "Data Source=" & server.Text & ";Initial Catalog=MARIJOASA_PRUEBA;Persist Security Info=True;User ID=sa;Password=" & ms_passw.Text & ""
        '    Dim sql_server_conn As New SqlConnection(MSSQLConnectionStringPrueba)

        '    'sql_server_conn.Open()

        '    Dim inicio = DateTime.Now
        '    log(inicio)

        '    ' Corregir valores nulos
        '    Try
        '        Dim oitm As SAPbobsCOM.Items


        '        ' sql_server_conn.Close()


        '        sql_server_conn.Open()

        '        Dim sql As String = "select  ItemCode,ItemName, ManBtchNum,InvntItem,SellItem,PrchseItem,InvntryUom,ItemType,ItmsGrpCod,U_EST_VENT,ISNULL(U_LIGAMENTO,''),ISNULL(U_COMBINACION,'') as U_COMBINACION,ISNULL(U_ESPECIFICA,'') as U_ESPECIFICA,ISNULL(U_ACABADO,'') as U_ACABADO,ISNULL(U_TIPO,'') as U_TIPO,ISNULL(U_ESTETITCA,'') as U_ESTETITCA,ISNULL(U_COMPOSICION,'') as U_COMPOSICION, " &
        '        "ISNULL(U_TEMPORADA,'') as U_TEMPORADA,ISNULL(U_ANCHO,0) as U_ANCHO,ISNULL(U_GRAMAJE_PROM,0) as U_GRAMAJE_PROM,U_NOMBRE_COM,ISNULL(U_ESPESOR,0) as U_ESPESOR,ISNULL(U_RENDIMIENTO,1) as U_RENDIMIENTO,VatLiable   from OITM  where frozenFor = 'N'"

        '        Dim ms_cmd As New SqlCommand(sql, sql_server_conn)

        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()



        '            oitm = oCnn.GetBusinessObject(BoObjectTypes.oItems)

        '            Dim ItemCode As String = ms_reader.GetString(0)
        '            Dim ItemName As String = ms_reader.GetString(1)
        '            Dim ManBtchNum As String = ms_reader.GetString(2)
        '            Dim InvntItem As String = ms_reader.GetString(3)
        '            Dim SellItem As String = ms_reader.GetString(4)
        '            Dim PrchseItem As String = ms_reader.GetString(5)
        '            Dim InvntryUom As String = ms_reader.GetString(6)
        '            Dim ItemType As String = ms_reader.GetString(7)
        '            Dim ItmsGrpCod As Int16 = ms_reader.GetInt16(8)
        '            Dim U_EST_VENT As String = ms_reader.GetString(9)
        '            Dim U_LIGAMENTO As String = ms_reader.GetString(10)
        '            Dim U_COMBINACION As String = ms_reader.GetString(11)
        '            Dim U_ESPECIFICA As String = ms_reader.GetString(12)
        '            Dim U_ACABADO As String = ms_reader.GetString(13)
        '            Dim U_TIPO As String = ms_reader.GetString(14)
        '            Dim U_ESTETITCA As String = ms_reader.GetString(15)
        '            Dim U_COMPOSICION As String = ms_reader.GetString(16)
        '            Dim U_TEMPORADA As String = ms_reader.GetString(17)
        '            Dim U_ANCHO As Decimal = ms_reader.GetDecimal(18)
        '            Dim U_GRAMAJE_PROM As Int32 = ms_reader.GetInt32(19)
        '            Dim U_NOMBRE_COM As String = ms_reader.GetString(20)
        '            Dim U_ESPESOR As Decimal = ms_reader.GetDecimal(21)
        '            Dim U_RENDIMIENTO As Decimal = ms_reader.GetDecimal(22)
        '            Dim VatLiable As String = ms_reader.GetString(23)

        '            log(" ItemCode  " + ItemCode + "  " + ItemName)

        '            Dim ManageBatchNumbers = BoYesNoEnum.tYES
        '            If (ManBtchNum <> "Y") Then
        '                ManageBatchNumbers = BoYesNoEnum.tNO
        '            End If
        '            Dim InventoryItem = BoYesNoEnum.tYES
        '            If (InvntItem <> "Y") Then
        '                InventoryItem = BoYesNoEnum.tNO
        '            End If
        '            Dim SalesItem = BoYesNoEnum.tYES
        '            If (SellItem <> "Y") Then
        '                InventoryItem = BoYesNoEnum.tNO
        '            End If

        '            Dim PurchaseItem = BoYesNoEnum.tYES
        '            If (PrchseItem <> "Y") Then
        '                PurchaseItem = BoYesNoEnum.tNO
        '            End If
        '            Dim ItemTypeSAP = SAPbobsCOM.ItemTypeEnum.itItems
        '            If (ItemType <> "I") Then
        '                ItemTypeSAP = SAPbobsCOM.ItemTypeEnum.itLabor
        '            End If


        '            oitm.ItemCode = ItemCode
        '            oitm.ItemName = ItemName
        '            oitm.ManageBatchNumbers = ManageBatchNumbers
        '            oitm.InventoryItem = InventoryItem
        '            oitm.SalesItem = SalesItem
        '            oitm.PurchaseItem = PurchaseItem
        '            oitm.InventoryUOM = InvntryUom
        '            oitm.ItemType = ItemTypeSAP
        '            oitm.VatLiable = BoYesNoEnum.tYES
        '            oitm.ItemsGroupCode = ItmsGrpCod
        '            oitm.UserFields.Fields.Item("U_EST_VENT").Value = U_EST_VENT
        '            oitm.UserFields.Fields.Item("U_LIGAMENTO").Value = U_LIGAMENTO
        '            oitm.UserFields.Fields.Item("U_COMBINACION").Value = U_COMBINACION
        '            oitm.UserFields.Fields.Item("U_ESPECIFICA").Value = U_ESPECIFICA
        '            oitm.UserFields.Fields.Item("U_ACABADO").Value = U_ACABADO
        '            oitm.UserFields.Fields.Item("U_TIPO").Value = U_TIPO
        '            oitm.UserFields.Fields.Item("U_ESTETITCA").Value = U_ESTETITCA
        '            oitm.UserFields.Fields.Item("U_COMPOSICION").Value = U_COMPOSICION
        '            oitm.UserFields.Fields.Item("U_TEMPORADA").Value = U_TEMPORADA
        '            oitm.UserFields.Fields.Item("U_GRAMAJE_PROM").Value = U_GRAMAJE_PROM
        '            oitm.UserFields.Fields.Item("U_ANCHO").Value = CDbl(U_ANCHO)
        '            oitm.UserFields.Fields.Item("U_NOMBRE_COM").Value = U_NOMBRE_COM
        '            oitm.UserFields.Fields.Item("U_ESPESOR").Value = CDbl(U_ESPESOR)
        '            oitm.UserFields.Fields.Item("U_RENDIMIENTO").Value = CDbl(U_RENDIMIENTO)

        '            iError = oitm.Add()
        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)
        '                log("Error al Migrar Articulos  Codigo " & iError & "  " & sError)
        '                Dim aErr As String = Replace(sError, "'", "")
        '                log("Error al Migrar Articulos  Codigo: " & ItemCode & " Error:  " & aErr)
        '            Else
        '                Dim ObjKey As String = oCnn.GetNewObjectKey
        '                'log("Identificador de Migracion de Articulo " & ObjKey & " " & ItemCode & "  " & ItemName)

        '            End If
        '        End While

        '        sql_server_conn.Close()


        '    Catch ex As Exception

        '        log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '    End Try
        '    Dim fin = DateTime.Now
        '    log(fin)
        '    Dim tiempo As Integer
        '    tiempo = DateDiff(DateInterval.Minute, inicio, fin)
        '    log("Tiempo Transcurrido Migracion Articulos: " & tiempo)
        '    ejecutando = False
        'End If
    End Sub

    Public Function getDocEntryFacturaVenta(DocNum As Integer)
        Dim DocEntry = 0
        Dim ms_conDocEntry As New SqlConnection
        Try

            ms_conDocEntry.ConnectionString = MSSQLConnectionString
            ms_conDocEntry.Open()
            Dim sql = "select DocEntry from oinv where DocNum = " & DocNum & ";"
            Dim ms_cmd As New SqlCommand(sql, ms_conDocEntry)
            Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
            ms_reader.Read()
            DocEntry = ms_reader(0)
            ms_reader.Close()

        Catch ex As Exception
            ms_conDocEntry.Close()
        End Try

        Return DocEntry
    End Function

    Public Function SincronizarCobroCuotas()
        If (Not ejecutandoCobroCuotas) Then
            ejecutandoCobroCuotas = True
            'Console.WriteLine("Sincronizando cuotas")
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MyConnDetalle As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MyConnTransfer As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim MysqlConnTarjetas As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MysqlConnCheques As MySqlConnection = New MySqlConnection(MySQLConnectionString)


            Try
                Dim paym As SAPbobsCOM.Payments
                paym = oCnn.GetBusinessObject(BoObjectTypes.oIncomingPayments)

                Try
                    MyConn.Open()
                    MyConnDetalle.Open()
                    MyConnTransfer.Open()
                    MysqlConnTarjetas.Open()
                    MysqlConnCheques.Open()

                    Dim mySelectQuery As String = "SELECT p.id_pago AS trans_num,suc,moneda,cotiz,fecha,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,cod_cli,cliente,if(folio_num is null,'',folio_num) AS folio_num,if(pdv_cod is null,'',pdv_cod) as pdv_cod  FROM  pagos_recibidos p WHERE   p.e_sap IS NULL AND control_caja = 'Si'"

                    Dim myCommand As New MySqlCommand(mySelectQuery, MyConn)

                    Dim myReader As MySqlDataReader
                    myReader = myCommand.ExecuteReader()

                    Dim linea = 0
                    While myReader.Read()
                        Dim flag As Boolean = False
                        Dim trans_num = myReader.GetInt32("trans_num")
                        Dim suc = myReader.GetString("suc")
                        Dim CardCode = myReader.GetString("cod_cli")
                        Dim CardName = myReader.GetString("cliente")
                        Dim DocDate = myReader.GetString("fecha_lat")
                        Dim ReciboLegal = myReader.GetString("folio_num")
                        Dim PDV = myReader.GetString("pdv_cod")
                        Dim Address As String = ""

                        paym.CardCode = CardCode
                        paym.CardName = CardName
                        paym.ControlAccount = ""
                        paym.DocDate = CDate(DocDate)
                        paym.Address = Address
                        paym.Remarks = "Nro Cobro: " & trans_num
                        paym.Invoices.SetCurrentLine(linea)

                        If ReciboLegal <> "" Then
                            paym.UserFields.Fields.Item("U_SER_PE").Value = PDV
                            paym.UserFields.Fields.Item("U_SER_EST").Value = estabs.Item(suc)
                            paym.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "SI"
                            Dim timbrado As String = getTimbrado(ReciboLegal, suc, PDV, "Recibo")
                            paym.UserFields.Fields.Item("U_NUM_AUTOR").Value = timbrado
                            paym.CounterReference = ReciboLegal
                            paym.JournalRemarks = Microsoft.VisualBasic.Left("REC (D) - " & CardCode & " " & CardName & "", 50) ' Max Length 60 
                        Else
                            paym.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "NO"
                            'paym.JournalMemo = Microsoft.VisualBasic.Left("REC (ND) - " & CardCode & " " & CardName & "", 50) ' Max Length 60
                            paym.JournalRemarks = Microsoft.VisualBasic.Left("REC (ND) - " & CardCode & " " & CardName & "", 50) ' Max Length 60
                        End If


                        Dim sqlDetalle As String = "SELECT sap_doc,id_cuota,entrega_actual,if(folio_num is null,'',folio_num) AS FacturaCorrespondiente FROM pago_rec_det WHERE id_pago = " & trans_num
                        Dim myCommandDetalle As New MySqlCommand(sqlDetalle, MyConnDetalle)
                        Dim myReaderDetalle As MySqlDataReader = myCommandDetalle.ExecuteReader()

                        Dim line = 0

                        While myReaderDetalle.Read()
                            If (line > 0) Then
                                paym.Invoices.Add()
                            End If

                            Dim DocNum = myReaderDetalle.GetInt32("sap_doc")
                            Dim ID_cuota = myReaderDetalle.GetInt32("id_cuota")
                            Dim entrega_actual = myReaderDetalle.GetDouble("entrega_actual")
                            Dim FacturaLegal = myReaderDetalle.GetString("FacturaCorrespondiente")

                            Dim DocEntry = getDocEntryFacturaVenta(DocNum)

                            'paym.Invoices.SetCurrentLine(line)
                            paym.Invoices.InvoiceType = 13
                            paym.Invoices.DocEntry = DocEntry
                            paym.Invoices.DocLine = line
                            paym.Invoices.InstallmentId = ID_cuota
                            paym.Invoices.SumApplied = entrega_actual
                            'FacturaLegal Ya hace solo Automaticamente no hace falta relacionar

                            line += 1
                            flag = True
                        End While
                        myReaderDetalle.Close()

                        'Pagos en Efectivo
                        Dim PagoEnEfectivo As Double = getEfectivoXCobroFactura(trans_num, "TRANS_NUM")
                        paym.CashSum = PagoEnEfectivo

                        'paym.Invoices.DocEntry = DocEntry

                        'paym.CashAccount = "1.1.1.2.01" ' Por ahora despues Agregar un campo en el Plan de Cuentas que represente a la Sucursal. 
                        paym.CashAccount = getCuentaContable("1.1.1.2", suc, "G$")
                        paym.JournalRemarks = Microsoft.VisualBasic.Left("Pago Rec.(Cuota) Id Pago: " & trans_num & " " & CardCode & " " & CardName & "", 50)



                        'Tarjetas y/o Convenios
                        Dim qTarjetas As String = "SELECT cod_conv as CreditCard,nombre as CreditCardName ,tipo,voucher,monto,DATE_FORMAT(fecha_acred,'%d/%m/%Y') AS fecha_acreed_lat,DATE_FORMAT(  DATE_ADD(fecha,INTERVAL 365 DAY),'%d/%m/%Y')  AS CardValidUntil, neto,DATE_FORMAT(fecha_ret,'%d/%m/%Y') AS fecha_ret,timbrado_ret FROM convenios WHERE trans_num = " & trans_num & " AND estado = 'Pendiente' AND e_sap IS NULL"
                        Dim tarjetasCommand As New MySqlCommand(qTarjetas, MysqlConnTarjetas)
                        Dim tarjReader As MySqlDataReader
                        tarjReader = tarjetasCommand.ExecuteReader()

                        Dim tarjetasI As Integer = 0

                        While tarjReader.Read()

                            Dim CreditCard As String = tarjReader.GetString("CreditCard")
                            Dim Voucher As String = tarjReader.GetString("voucher")
                            Dim Monto As String = tarjReader.GetDouble("monto")
                            Dim FechaAcredit As String = tarjReader.GetString("fecha_acreed_lat")
                            Dim CardValidUntil As String = tarjReader.GetString("CardValidUntil")
                            Dim PaymentMethodCode As String = CreditCard ' Es igual al codigo de Tarjeta de Credito o Convenio 
                            Dim FechaRet As String = tarjReader.GetString("fecha_ret")
                            Dim Timbrado As String = tarjReader.GetString("timbrado_ret")


                            If (tarjetasI > 0) Then
                                paym.CreditCards.Add()
                            End If
                            paym.CreditCards.SetCurrentLine(tarjetasI)
                            paym.CreditCards.AdditionalPaymentSum = 0
                            paym.CreditCards.CardValidUntil = CardValidUntil


                            If (CreditCard = 17) Then
                                paym.CreditCards.CreditAcct = "1.1.2.3.10"  ' Tarjetas a Acreditar  Cuando es Retencion debe ser 1.1.2.3.10
                                paym.CreditCards.OwnerIdNum = Timbrado
                                paym.CreditCards.UserFields.Fields.Item("U_fecharet").Value = FechaRet
                            Else
                                paym.CreditCards.CreditAcct = "1.1.2.3.02"  ' Tarjetas a Acreditar 
                            End If


                            paym.CreditCards.CreditCard = CreditCard
                            paym.CreditCards.CreditCardNumber = Microsoft.VisualBasic.Left(Voucher, 64) 'Aqui Guardare el Voucher No tenemos el Numero de Tarjeta
                            paym.CreditCards.VoucherNum = Microsoft.VisualBasic.Left(Voucher, 20)

                            paym.CreditCards.CreditSum = Monto
                            paym.CreditCards.CreditType = 1  ' No se que es esto
                            paym.CreditCards.FirstPaymentDue = FechaAcredit
                            paym.CreditCards.FirstPaymentSum = Monto
                            paym.CreditCards.NumOfCreditPayments = 1
                            paym.CreditCards.NumOfPayments = 1
                            paym.CreditCards.PaymentMethodCode = PaymentMethodCode

                            tarjetasI += 1
                            flag = True
                        End While
                        tarjReader.Close()

                        'Cheques
                        Dim qCheques As String = "SELECT nro_cheque as CheckNumber, id_banco AS BankCode,cuenta as AccounttNum,DATE_FORMAT(fecha_emis,'%d/%m/%Y') AS fecha_emis_lat,valor_ref FROM cheques_ter WHERE trans_num = " & trans_num & " AND estado = 'Pendiente' AND e_sap IS NULL order by nro_cheque asc"

                        Dim chequesCommand As New MySqlCommand(qCheques, MysqlConnCheques)
                        Dim chqReader As MySqlDataReader
                        chqReader = chequesCommand.ExecuteReader()
                        Dim chequesI As Integer = 0
                        While chqReader.Read()

                            Dim CheckNumber As Integer = chqReader.GetInt32("CheckNumber")
                            Dim AccounttNum As String = chqReader.GetString("AccounttNum")

                            Dim BankCode As String = chqReader.GetString("BankCode")

                            Dim Valor_ref As String = chqReader.GetString("valor_ref")
                            Dim DueDate As String = chqReader.GetString("fecha_emis_lat")

                            If (chequesI > 0) Then
                                paym.Checks.Add()
                            End If

                            ' paym.Checks.SetCurrentLine(chequesI)

                            paym.Checks.AccounttNum = AccounttNum
                            paym.Checks.BankCode = BankCode
                            paym.Checks.CountryCode = "PY"
                            paym.Checks.Branch = suc
                            paym.Checks.CheckNumber = CheckNumber
                            paym.Checks.CheckSum = Valor_ref
                            paym.Checks.Details = Microsoft.VisualBasic.Left("Cobro " & CardName & "", 60)
                            paym.Checks.DueDate = CDate(DueDate)
                            paym.Checks.Trnsfrable = 0
                            paym.Checks.CheckAccount = "1.1.1.3.01"  ' Cheques Recibidos

                            chequesI += 1
                            flag = True
                        End While
                        chqReader.Close()

                        'Transferencia bancaria
                        Dim qTransfer As String = "SELECT COUNT(*) as cantTransf, SUM(entrada) AS TransfereciaBancaria,DATE_FORMAT(fecha ,'%d/%m/%Y') AS fecha_lat,cuenta FROM bcos_ctas_mov WHERE trans_num  = " & trans_num & " AND estado = 'Pendiente' AND e_sap IS NULL"

                        Dim transferCommand As New MySqlCommand(qTransfer, MyConnTransfer)
                        Dim transferReader As MySqlDataReader = transferCommand.ExecuteReader()

                        While transferReader.Read()
                            Dim TransfereciaBancaria As Double = 0
                            Dim TransferDueDate As String = 0

                            Dim cantTransf As Integer = transferReader.GetInt32("cantTransf")

                            If (cantTransf > 0) Then
                                Dim cuenta = transferReader.GetString("cuenta")
                                TransfereciaBancaria = transferReader.GetDouble("TransfereciaBancaria")
                                TransferDueDate = transferReader.GetString("fecha_lat")
                                paym.TransferAccount = getCuentaContableBanco(cuenta) ' Cambiar esta Cuenta
                                paym.TransferDate = CDate(TransferDueDate)
                                paym.TransferSum = TransfereciaBancaria
                            End If

                        End While
                        transferReader.Close()

                        If (flag) Then
                            iError = paym.Add
                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log(" Cobro Cuotas  Trans Num:  " & trans_num & " " & iError & "  " & sError)

                            Else
                                Dim NroEntrada As String = oCnn.GetNewObjectKey
                                log("Identificador de Pago cuotas DocEntry: " & NroEntrada)
                                Dim MyUpdate As MySqlConnection = New MySqlConnection(MySQLConnectionString)

                                MyUpdate.Open()
                                Dim updQuery As String = "UPDATE pagos_recibidos SET e_sap = 1 WHERE id_pago =   " & trans_num & ";"
                                Dim updCommand As New MySqlCommand(updQuery, MyUpdate)
                                updCommand.ExecuteNonQuery()
                                MyUpdate.Close()

                            End If
                        Else
                            log("Flag False why?")
                            'setPagoEnviado(f_nro)
                        End If
                        linea += 1
                    End While

                    myReader.Close()
                    MyConn.Close()
                    MysqlConnTarjetas.Close()
                    MyConnTransfer.Close()
                    MysqlConnCheques.Close()
                    MyConnDetalle.Close()
                Catch myerror As Exception
                    log("Cannot connect to database (F: SincronizarCobroCuotas): " & myerror.Message)
                End Try

            Catch ex As Exception
                log("Error COM Cobro Cuotas: " & ex.Message)
            End Try
            ejecutandoCobroCuotas = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonCobroCuotas_Click(sender As Object, e As EventArgs) Handles ButtonCobroCuotas.Click
        SincronizarCobroCuotas()
    End Sub


    Private Sub TimerSincClientes_Tick(sender As Object, e As EventArgs) Handles TimerSincClientes.Tick
        Try
            Dim ThreadClientes As New Thread(AddressOf Me.SincronizarClientes)
            ThreadClientes.Start()
        Catch ex As Exception
            log("Error en ThreadClientes " + ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerReservas_Tick(sender As Object, e As EventArgs) Handles TimerReservas.Tick
        Try
            Dim ThreadReservas As New Thread(AddressOf Me.SincronizarReservas)
            ThreadReservas.Start()
        Catch ex As Exception
            log("Error en ThreadReservas " + ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerRemisiones_Tick(sender As Object, e As EventArgs) Handles TimerRemisiones.Tick
        Try
            Dim ThreadRemisiones As New Thread(AddressOf Me.SincronizarRemisiones)
            ThreadRemisiones.Start()
        Catch ex As Exception
            log("Error en ThreadRemisiones " + ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerCobroCuotas_Tick(sender As Object, e As EventArgs) Handles TimerCobroCuotas.Tick
        Try
            Dim ThreadCobroCuotas As New Thread(AddressOf Me.SincronizarCobroCuotas)
            ThreadCobroCuotas.Start()
        Catch ex As Exception
            log("Error en ThreadCobroCuotas " + ex.StackTrace)
        End Try
    End Sub

    Public Function SincronizarAsientos()
        If (Not ejecutandoAsientos) Then
            ejecutandoAsientos = True
            'Console.WriteLine("Sincronizando Asientos")
            Dim MyCont As MySqlConnection = New MySqlConnection(MySQLConnectionString)
            Dim MyConnDetalle As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim cab As String = "SELECT id_asiento,fecha,DATE_FORMAT(fecha,'%Y%m%d') as fecha_sap,usuario,id_frac,descrip FROM asientos WHERE e_sap IS NULL LIMIT 10"

            Try
                Dim asiento As SAPbobsCOM.JournalEntries

                MyCont.Open()
                MyConnDetalle.Open()

                Dim cabCommand As New MySqlCommand(cab, MyCont)
                Dim cabReader As MySqlDataReader = cabCommand.ExecuteReader()


                While cabReader.Read()

                    asiento = oCnn.GetBusinessObject(BoObjectTypes.oJournalEntries)

                    Dim id_asiento = cabReader.GetInt32("id_asiento")
                    Dim fecha = cabReader.GetString("fecha_sap")
                    Dim usuario = cabReader.GetString("usuario")
                    Dim id_frac = cabReader.GetInt32("id_frac")
                    Dim descrip = cabReader.GetString("descrip")

                    asiento.TaxDate = Now
                    asiento.Reference = id_frac
                    asiento.Memo = substring(descrip, 50)
                    asiento.Reference2 = usuario
                    asiento.Reference3 = id_asiento

                    Dim detCommand As New MySqlCommand(" SELECT id_det,cuenta,nombre_cuenta,debe,haber, IF(suc IS NULL,'N/A',suc) AS norma_reparto FROM asientos_det WHERE id_asiento =" & id_asiento, MyConnDetalle)
                    Dim detReader As MySqlDataReader = detCommand.ExecuteReader()

                    Dim line = 0

                    While detReader.Read()
                        Dim id_det = detReader.GetInt32("id_det")
                        Dim cuenta = detReader.GetString("cuenta")
                        Dim nombre_cuenta = detReader.GetString("nombre_cuenta")
                        Dim debe = detReader.GetDouble("debe")
                        Dim haber = detReader.GetDouble("haber")
                        Dim norma_reparto = detReader.GetString("norma_reparto")

                        If (line > 0) Then
                            asiento.Lines.Add()
                        End If

                        'asiento.SetCurrentLine(line)

                        asiento.Lines.AccountCode = cuenta
                        'asiento.Lines.ContraAccount = "1.1.3.1.05"
                        asiento.Lines.Debit = debe
                        asiento.Lines.Credit = haber
                        asiento.Lines.DueDate = Now
                        asiento.Lines.ReferenceDate1 = Now
                        'asiento.Lines.ShortName = nombre_cuenta
                        asiento.Lines.TaxDate = Now
                        If (norma_reparto <> "N/A") Then
                            asiento.Lines.CostingCode = norma_reparto
                        End If
                        line += 1
                    End While

                    detReader.Close()


                    iError = asiento.Add()
                    If iError <> 0 Then
                        oCnn.GetLastError(iError, sError)

                        log(iError & " Asiento Contable  " & sError & "   ")
                    Else
                        Dim NroEntrada As String = oCnn.GetNewObjectKey
                        log("Identificador de Asiento DocEntry: " & NroEntrada)
                        
                        MyExec("UPDATE asientos SET e_sap = 1 WHERE id_asiento = " & id_asiento & " ;")
                         
                    End If

                End While
                cabReader.Close()
                MyConnDetalle.Close()
                MyCont.Close()

            Catch ex As Exception
                log("Error COM: " & ex.Message)
            End Try
            ejecutandoAsientos = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonAsientos_Click(sender As Object, e As EventArgs) Handles ButtonAsientos.Click
        SincronizarAsientos()
    End Sub

    Private Sub TimerAsientos_Tick(sender As Object, e As EventArgs) Handles TimerAsientos.Tick
        Try
            Dim ThreadAsientos As New Thread(AddressOf Me.SincronizarAsientos)
            ThreadAsientos.Start()
        Catch ex As Exception
            log("Error en ThreadAsientos " + ex.StackTrace)
        End Try
    End Sub

    Private Sub ButtonDisconect_Click(sender As Object, e As EventArgs) Handles ButtonDisconect.Click
        Try
            If conectado Then
                oCnn.Disconnect()
                oCnn = Nothing
                GC.Collect()
          
                TimerStockXVentas.Enabled = False
                TimerAjustesNeg.Enabled = False
                TimerAjustesPos.Enabled = False
                TimerCompras.Enabled = False
                TimerSincClientes.Enabled = False
                TimerCobroCuotas.Enabled = False
                TimerReservas.Enabled = False
                TimerAsientos.Enabled = False
                TimerUptateLotes.Enabled = False
                TimerPagos.Enabled = False
                TimerRemisiones.Enabled = False
                TimerNotasCredito.Enabled = False
                TimerSyncBancExtract.Enabled = False
                TimerFracPos.Enabled = False
                TimerFracNeg.Enabled = False
                TimerCancelPagos.Enabled = False

                log("Disconected...")
                End
            End If
        Catch ex As Exception
            log(ex.Message)
        End Try
    End Sub

    Private Sub ButtonUpdatePriceList_Click(sender As Object, e As EventArgs) Handles ButtonUpdatePriceList.Click
        'If (Not ejecutando) Then
        '    ejecutando = True
        '    Dim MSSQLConnectionStringPrueba = "Data Source=" & server.Text & ";Initial Catalog=MARIJOASA_PRUEBA;Persist Security Info=True;User ID=sa;Password=" & ms_passw.Text & ""
        '    Dim sql_server_conn_prices As New SqlConnection(MSSQLConnectionStringPrueba)
        '    Dim sql_server_conn As New SqlConnection(MSSQLConnectionString)

        '    Dim inicio = DateTime.Now
        '    log(inicio)

        '    'sql_server_conn.Open()


        '    ' Corregir valores nulos
        '    Try
        '        Dim oitm As SAPbobsCOM.Items


        '        oitm = oCnn.GetBusinessObject(BoObjectTypes.oItems)

        '        sql_server_conn.Open()
        '        sql_server_conn_prices.Open()

        '        Dim sql As String = "select  ItemCode from  OITM "

        '        Dim ms_cmd As New SqlCommand(sql, sql_server_conn)

        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim ItemCode As String = ms_reader.GetString(0)
        '            log("ItemCode : " & ItemCode)
        '            If (oitm.GetByKey(ItemCode)) Then

        '                Dim sql_prices As String = "select Price,PriceList,Currency,Factor from ITM1 where ItemCode = '" & ItemCode & "'"
        '                Dim price_cmd As New SqlCommand(sql_prices, sql_server_conn_prices)
        '                Dim price_reader As SqlDataReader = price_cmd.ExecuteReader()
        '                While price_reader.Read()
        '                    Dim Price As Decimal = price_reader.GetDecimal(0)
        '                    Dim PriceList As Int16 = price_reader.GetInt16(1)
        '                    'log("Price : " & Price & " Price List:  " & PriceList)
        '                    oitm.PriceList.SetCurrentLine(PriceList - 1)
        '                    oitm.PriceList.Price = CDbl(Price)
        '                    oitm.PriceList.Currency = "G$"
        '                    iError = oitm.Update()
        '                    If iError <> 0 Then
        '                        oCnn.GetLastError(iError, sError)

        '                        log(iError & " Error al Migrar Listas de precios  " & sError & "   ")
        '                    Else
        '                        log("Price List Updated: " & ItemCode & "  Lista Precio:   " & PriceList & " Precio: " & Price)
        '                    End If
        '                End While
        '                price_reader.Close()
        '            End If
        '        End While
        '        ms_reader.Close()
        '        sql_server_conn.Close()
        '        sql_server_conn_prices.Close()

        '    Catch ex As Exception
        '        log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '    End Try
        '    Dim fin = DateTime.Now
        '    log(fin)
        '    Dim tiempo As Integer
        '    tiempo = DateDiff(DateInterval.Minute, inicio, fin)
        '    log("Tiempo Transcurrido Migracion Listas de Precios: " & tiempo & "Minutos")
        '    ejecutando = False
        'End If
    End Sub

    Public Function truncateUserTables(userTable As String)
        'Dim sql_server As New SqlConnection(MSSQLConnectionString)
        'Try
        '    Dim table As SAPbobsCOM.UserTable = oCnn.UserTables.Item(userTable)
        '    sql_server.Open()
        '    Dim sql As String = "select  Code from  [@" & userTable & "]"
        '    Dim ms_cmd As New SqlCommand(sql, sql_server)
        '    Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '    While ms_reader.Read()
        '        Dim Code As String = ms_reader.GetString(0)
        '        If table.GetByKey(Code) Then
        '            iError = table.Remove()

        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)
        '                log(iError & " " & userTable & "" & sError & "   ")
        '            End If
        '        End If
        '    End While

        'Catch ex As Exception
        '    sql_server.Close()
        'End Try
        'sql_server.Close()
        Return True
    End Function

    Private Sub ButtonUserTables_Click(sender As Object, e As EventArgs) Handles ButtonUserTables.Click
        'If (Not ejecutando) Then
        '    ejecutando = True
        '    Dim MSSQLConnectionStringPrueba = "Data Source=" & server.Text & ";Initial Catalog=MARIJOASA_PRUEBA;Persist Security Info=True;User ID=sa;Password=" & ms_passw.Text & ""
        '    Dim sql_server As New SqlConnection(MSSQLConnectionStringPrueba)

        '    truncateUserTables("DESIGN_PATTERNS")
        '    truncateUserTables("EXX_COLOR_COMERCIAL")
        '    truncateUserTables("NOMBRES_COMERCIALES")
        '    truncateUserTables("POLITICA_CORTES")


        '    Try
        '        Dim table As SAPbobsCOM.UserTable = oCnn.UserTables.Item("DESIGN_PATTERNS")

        '        sql_server.Open()
        '        Dim sql As String = "select  Code,Name,U_estado from  [@DESIGN_PATTERNS]"
        '        Dim ms_cmd As New SqlCommand(sql, sql_server)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim Code As String = ms_reader.GetString(0)
        '            Dim Name As String = ms_reader.GetString(1)
        '            Dim Estado As String = ms_reader.GetString(2)
        '            'log("Code : " & Code & " Name: " & Name & " Estado: " & Estado)

        '            table.Code = Code
        '            table.Name = Name
        '            table.UserFields.Fields.Item("U_estado").Value = Estado
        '            iError = table.Add()

        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)

        '                log(iError & " DESIGN_PATTERNS  " & sError & "   ")
        '            Else
        '                log("DESIGN_PATTERNS agregado  " & Name)
        '            End If
        '        End While
        '        ms_reader.Close()
        '    Catch ex As Exception
        '        log("Error:  " & ex.StackTrace)
        '    End Try
        '    sql_server.Close()


        '    '#########################      EXX_COLOR_COMERCIAL     #########################
        '    Try
        '        Dim table As SAPbobsCOM.UserTable = oCnn.UserTables.Item("EXX_COLOR_COMERCIAL")

        '        sql_server.Open()
        '        Dim sql As String = "select  Code,Name,IsNull(U_rgb,'')  as U_rgb,IsNull(U_cod_interno,'')  as U_cod_interno,U_estado from  [@EXX_COLOR_COMERCIAL] where U_estado = 'Activo' or U_estado is null"
        '        Dim ms_cmd As New SqlCommand(sql, sql_server)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim Code As String = ms_reader.GetString(0)
        '            Dim Name As String = ms_reader.GetString(1)
        '            Dim U_rgb As String = ms_reader.GetString(2)
        '            Dim U_cod_interno As String = ms_reader.GetString(3)


        '            table.Code = Code
        '            table.Name = Name
        '            table.UserFields.Fields.Item("U_rgb").Value = U_rgb
        '            table.UserFields.Fields.Item("U_cod_interno").Value = U_cod_interno
        '            table.UserFields.Fields.Item("U_estado").Value = "Activo"
        '            iError = table.Add()

        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)

        '                log(iError & " EXX_COLOR_COMERCIAL  " & sError & "   ")
        '            Else
        '                log("EXX_COLOR_COMERCIAL agregado  " & Name)
        '            End If
        '        End While
        '        ms_reader.Close()
        '    Catch ex As Exception
        '        log("Error:  " & ex.StackTrace)
        '    End Try
        '    sql_server.Close()

        '    '#########################      NOMBRES_COMERCIALES     #########################
        '    Try
        '        Dim table As SAPbobsCOM.UserTable = oCnn.UserTables.Item("NOMBRES_COMERCIALES")

        '        sql_server.Open()
        '        Dim sql As String = "select  Code,Name,IsNull(U_CodSector,'')  as U_CodSector,IsNull(U_nombre_com,'')  as U_nombre_com,U_Estado from  [@NOMBRES_COMERCIALES]"
        '        Dim ms_cmd As New SqlCommand(sql, sql_server)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim Code As String = ms_reader.GetString(0)
        '            Dim Name As String = ms_reader.GetString(1)
        '            Dim U_CodSector As Int16 = ms_reader.GetInt16(2)
        '            Dim U_nombre_com As String = ms_reader.GetString(3)
        '            Dim Estado As String = ms_reader.GetString(4)

        '            table.Code = Code
        '            table.Name = Name
        '            table.UserFields.Fields.Item("U_CodSector").Value = U_CodSector
        '            table.UserFields.Fields.Item("U_nombre_com").Value = U_nombre_com
        '            table.UserFields.Fields.Item("U_estado").Value = Estado
        '            iError = table.Add()

        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)

        '                log(iError & " NOMBRES_COMERCIALES  " & sError & "   ")
        '            Else
        '                log("NOMBRE_COMERCIAL agregado  " & U_nombre_com)
        '            End If
        '        End While
        '        ms_reader.Close()
        '    Catch ex As Exception
        '        log("Error:  " & ex.StackTrace)
        '    End Try
        '    sql_server.Close()


        '    '#########################      politica de cortes     #########################
        '    Try
        '        Dim table As SAPbobsCOM.UserTable = oCnn.UserTables.Item("politica_cortes")

        '        sql_server.Open()
        '        Dim sql As String = "select  code,name, u_codigo,u_suc,u_politica from  [@politica_cortes]"
        '        Dim ms_cmd As New SqlCommand(sql, sql_server)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        While ms_reader.Read()
        '            Dim code As String = ms_reader.GetString(0)
        '            Dim name As String = ms_reader.GetString(1)
        '            Dim u_codigo As String = ms_reader.GetString(2)
        '            Dim u_suc As String = ms_reader.GetString(3)
        '            Dim u_politica As String = ms_reader.GetString(4)

        '            table.Code = code
        '            table.Name = name
        '            table.UserFields.Fields.Item("u_codigo").Value = u_codigo
        '            table.UserFields.Fields.Item("u_suc").Value = u_suc
        '            table.UserFields.Fields.Item("u_politica").Value = u_politica
        '            iError = table.Add()

        '            If iError <> 0 Then
        '                oCnn.GetLastError(iError, sError)

        '                log(iError & " politica_cortes  " & sError & "   ")
        '            Else
        '                log("politica_cortes agregada  " & u_codigo & "   " & u_politica)
        '            End If
        '        End While
        '        ms_reader.Close()
        '    Catch ex As Exception
        '        log("error:  " & ex.StackTrace)
        '    End Try
        '    sql_server.Close()
        '    ejecutando = False
        'End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonDeleteClienes.Click
        'Dim msql As New SqlConnection(MSSQLConnectionString)

        'Try

        '    msql.Open()
        '    Dim obp As SAPbobsCOM.BusinessPartners
        '    obp = oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners) 'Calls BusinessPartners object

        '    Dim clientes As String = "SELECT CardCode FROM OCRD"

        '    Dim ms_cmd_precios As New SqlCommand(clientes, msql)
        '    Dim reader As SqlDataReader = ms_cmd_precios.ExecuteReader()



        '    While reader.Read()

        '        Dim CardCode = reader("CardCode")

        '        If (obp.GetByKey(CardCode) = True) Then

        '            Dim nErr As Long
        '            Dim errMsg As String = ""

        '            If (0 <> obp.Remove()) Then
        '                Call oCnn.GetLastError(nErr, errMsg)
        '                log("Error al Eliminar RUC Cliente" & nErr & "" & errMsg)
        '            Else
        '                log("Cliente  " & CardCode & " eliminado ")
        '            End If

        '        End If
        '    End While
        '    reader.Close()
        'Catch ex As Exception
        '    log("Error:  " & ex.StackTrace)
        'End Try
        'msql.Close()
    End Sub

    Private Sub ButtonTest_Click(sender As Object, e As EventArgs)
        '  ajustesNegativosXFactura(3, "douglas", "G$", 1, "02", "19/01/2017", "Sistema")
    End Sub

    Private Sub TimerPagos_Tick(sender As Object, e As EventArgs) Handles TimerPagos.Tick
        Try
            Dim ThreadPagosRecibidos As New Thread(AddressOf Me.SincronizarPagosRecibidos)
            ThreadPagosRecibidos.Start()
        Catch ex As Exception
            log("Error:  hreadPagosRecibidos" & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerUptateLotes_Tick(sender As Object, e As EventArgs) Handles TimerUptateLotes.Tick
        Try
            Dim ThreadLotes As New Thread(AddressOf Me.SincronizarLotes)
            ThreadLotes.Start()
        Catch ex As Exception
            log("Error:  ThreadLotes" & ex.StackTrace)
        End Try
    End Sub

    Private Sub ButtonMigrarProv_Click(sender As Object, e As EventArgs) Handles ButtonMigrarProv.Click
        'Dim MySQLConnectionStringMarijoa = "server=127.0.0.1; port=3306;user id=plus; password=case; database=marijoa" ' marijoa
        'Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)
        'Try
        '    Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)

        '    MysqlConn.Open()

        '    Dim mySelectQuery As String = "select prov_cod,IF(prov_ruc IS NULL OR prov_ruc = '',prov_cod,prov_ruc) as RUC,prov_nombre,prov_dir,prov_ciudad,prov_pais,prov_tel,prov_fax,prov_mail from mnt_prov where e_sap is null"

        '    Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

        '    Dim myReaderCli As MySqlDataReader
        '    myReaderCli = myCommand.ExecuteReader()

        '    Dim msql As New SqlConnection(MSSQLConnectionString)
        '    msql.Open()


        '    'Create the BusinessPartners object
        '    Dim obp As SAPbobsCOM.BusinessPartners
        '    obp = oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners) 'Calls BusinessPartners object

        '    While myReaderCli.Read()
        '        Dim prov_cod = myReaderCli.GetString("prov_cod")
        '        Dim prov_nombre = myReaderCli.GetString("prov_nombre")
        '        Dim ci_ruc = myReaderCli.GetString("RUC")

        '        Dim tel = myReaderCli.GetString("prov_tel")
        '        Dim fax = myReaderCli.GetString("prov_fax")
        '        Dim email = myReaderCli.GetString("prov_mail")

        '        Dim pais = myReaderCli.GetString("prov_pais")

        '        Dim ciudad = myReaderCli.GetString("prov_ciudad")
        '        Dim dir = myReaderCli.GetString("prov_dir")
        '        Dim ocupacion = "Proveedor"
        '        Dim situacion = "Independiente"
        '        Dim tipo = "local"
        '        Dim tipo_doc = "C.I."
        '        If ci_ruc.IndexOf("-") Then
        '            tipo = "local"
        '        Else
        '            tipo = "extranjero"
        '        End If

        '        ' Consultar en la base actual si ruc ya no existe

        '        Dim queryCheck As String = "SELECT COUNT(*) AS CANT_RUC,CardCode FROM OCRD WHERE LicTradNum = '" & ci_ruc & "' and CardType = 'S' group by LicTradNum,CardCode"

        '        Dim ms_cmd As New SqlCommand(queryCheck, msql)
        '        Dim reader As SqlDataReader = ms_cmd.ExecuteReader()

        '        If (reader.HasRows() = False) Then
        '            ' log("################### Cliente encontrado " & ci_ruc & "  " & nombre)
        '            obp.CardName = prov_nombre

        '            obp.CardCode = "P01"

        '            obp.CardType = BoCardTypes.cSupplier
        '            obp.FederalTaxID = ci_ruc
        '            'obp.Series = 0
        '            obp.Series = 66
        '            obp.Phone1 = substring(tel, 19)
        '            obp.Fax = fax
        '            obp.EmailAddress = email
        '            obp.City = ciudad
        '            obp.Indicator = "01"
        '            obp.BilltoDefault = "1"
        '            obp.Address = dir
        '            obp.PriceListNum = 1
        '            obp.Currency = "G$"

        '            If (pais.ToUpper = "BRASIL") Then
        '                obp.Country = "BR"
        '                tipo_doc = "R.G."
        '            ElseIf (pais.ToUpper = "ARGENTINA") Then
        '                obp.Country = "AR"
        '                tipo_doc = "D.N.I."
        '            Else
        '                obp.Country = "PY"
        '                tipo_doc = "C.I."
        '            End If

        '            obp.AdditionalID = ocupacion  '// Funciono una vez AddID            
        '            obp.UnifiedFederalTaxID = situacion  '// VatIdUnCmp

        '            If tipo = "local" Then
        '                obp.DebitorAccount = "2.1.1.1.01"
        '            Else
        '                obp.DebitorAccount = "2.1.1.1.02" '// Proveedores del Exterior
        '            End If

        '            '// FechaNac
        '            'obp.UserFields.Fields.Item("U_fecha_nac").Value = fecha_nac
        '            obp.UserFields.Fields.Item("U_tipo_doc").Value = tipo_doc

        '            Dim nErr As Long
        '            Dim errMsg As String = ""

        '            If (0 <> obp.Add()) Then
        '                Call oCnn.GetLastError(nErr, errMsg)
        '                log("Failed to add an business partner Proveedor " & ci_ruc & "  " & nErr & " " & errMsg)
        '            Else
        '                'Dim objCode As String
        '                MysqlAux.Open()
        '                Dim updQuery As String = "update mnt_prov set e_sap = 1 where prov_cod = '" & prov_cod & "';"
        '                Dim updCommand As New MySqlCommand(updQuery, MysqlAux)
        '                updCommand.ExecuteNonQuery()
        '                MysqlAux.Close()

        '                log("Proveedor Migrado con exito " & ci_ruc & "  " & prov_nombre)
        '            End If
        '        Else
        '            'Proveedor ya existe
        '        End If
        '        reader.Close()

        '    End While
        '    log("FIN DE MIGRACION DE PROVEEDORES!!!")
        '    myReaderCli.Close()
        'Catch ex As Exception
        '    log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '    log("Error : " & ex.Message & " STACK TRACE: " & ex.StackTrace)


        'End Try
    End Sub

    Private Sub CheckBoxAutoConnect_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAutoConnect.CheckedChanged
        TimerConnect.Enabled = False
    End Sub

    Private Sub TimerConnect_Tick(sender As Object, e As EventArgs) Handles TimerConnect.Tick
        Dim valor As Integer = counter.Text
        If (valor = 0) Then
            TimerConnect.Enabled = False
            ButtonConnect.PerformClick()
        Else
            valor -= 1
            counter.Text = valor
        End If
    End Sub

    Private Sub ButtonCheckSapCodes_Click(sender As Object, e As EventArgs) Handles ButtonCheckSapCodes.Click


        'MessageBox.Show("Verifique que las cotizaciones esten establecidas... NO OLVIDAR UPDATE mnt_prod SET p_migrated = NULL")

        'Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)
        'Dim ms_con As New SqlConnection
        'ms_con.ConnectionString = MSSQLConnectionString
        'ms_con.Open()

        'Dim sap_codes As String = "SELECT DISTINCT p_sap_cod  FROM mnt_prod p, mov_compras c, mnt_prov v WHERE p.p_ref = c_ref AND c.c_prov = prov_cod AND p_cant > 0 AND  p_local not in ('26','08','09','11','12') AND (prod_fin_pieza != 'Si' AND prod_fin_pieza != 'Tr' )   AND p_sap_cod IS NOT NULL AND p.p_migrated IS NULL AND c.p_migrated IS NULL  GROUP BY p_sap_cod ORDER BY p_sap_cod    ASC"

        'Try

        '    MysqlConn.Open()
        '    Dim myCommand As New MySqlCommand(sap_codes, MysqlConn)

        '    Dim myReader As MySqlDataReader
        '    myReader = myCommand.ExecuteReader()

        '    While myReader.Read()
        '        Dim p_sap_cod = myReader.GetString("p_sap_cod")
        '        Dim sql As String = "SELECT count(*) as Cant  FROM  OITM  WHERE ItemCode = '" & p_sap_cod & "'"
        '        Dim ms_cmd As New SqlCommand(sql, ms_con)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
        '        ms_reader.Read()
        '        Dim cant = ms_reader(0)
        '        If (cant = 0) Then
        '            log("Error: ItemCode " & p_sap_cod & "  no definido.")
        '        End If
        '        ms_reader.Close()
        '    End While
        '    myReader.Close()
        '    MysqlConn.Close()


        'Catch ex As Exception
        '    log("Error al obtener AbsEntry: " & ex.StackTrace)
        '    ms_con.Close()

        'Finally

        '    MysqlConn.Close()
        '    ms_con.Close()
        'End Try
        'log("Fin Check: ItemCodes..")
    End Sub

    Private Sub ButtonUpdateCliData_Click(sender As Object, e As EventArgs) Handles ButtonUpdateCliData.Click
        'Dim MySQLConnectionStringMarijoa As MySqlConnection = "server=127.0.0.1; port=3306;user id=plus; password=case; database=marijoa" ' marijoa
        'Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)
        'Try

        '    MysqlConn.Open()

        '    Dim mySelectQuery As String = "SELECT DISTINCT cli_ci,cli_dir,cli_limit FROM mnt_cli c, factura f, cuotas ct WHERE f.fact_cli_ci = c.cli_ci AND f.fact_nro = ct.ct_ref AND f.fact_estado = 'Cerrada' AND f.fact_fecha > '2015-01-01' AND cli_ci != 'XXXX'"

        '    Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

        '    Dim myReaderCli As MySqlDataReader
        '    myReaderCli = myCommand.ExecuteReader()

        '    Dim msql As New SqlConnection(MSSQLConnectionString)
        '    msql.Open()


        '    'Create the BusinessPartners object
        '    Dim obp As SAPbobsCOM.BusinessPartners
        '    obp = oCnn.GetBusinessObject(BoObjectTypes.oBusinessPartners) 'Calls BusinessPartners object

        '    While myReaderCli.Read()

        '        Dim ci_ruc = myReaderCli.GetString("cli_ci")
        '        Dim cli_dir = myReaderCli.GetString("cli_dir")
        '        Dim cli_limit = myReaderCli.GetDouble("cli_limit")


        '        ' Consultar en la base actual si ruc ya no existe

        '        Dim queryCheck As String = "SELECT COUNT(*) AS CANT_RUC,CardCode FROM OCRD WHERE LicTradNum = '" & ci_ruc & "' group by LicTradNum,CardCode"

        '        Dim ms_cmd As New SqlCommand(queryCheck, msql)
        '        Dim reader As SqlDataReader = ms_cmd.ExecuteReader()


        '        If (reader.HasRows()) Then
        '            reader.Read()
        '            Dim CardCode As String = reader(1)

        '            If (obp.GetByKey(CardCode) = True) Then

        '                obp.CreditLimit = cli_limit
        '                obp.Address = substring(cli_dir, 100)


        '                Dim nErr As Long
        '                Dim errMsg As String = ""

        '                If (0 <> obp.Update()) Then
        '                    Call oCnn.GetLastError(nErr, errMsg)
        '                    log("Error al actualizar Cliente" & nErr & "" & errMsg & "  " & ci_ruc)
        '                Else
        '                    log(" Cliente actualizado " & ci_ruc & " ")
        '                End If

        '            End If

        '        Else
        '            log("Cliente no existe...")
        '        End If

        '        reader.Close()

        '    End While
        '    log("FIN DE ACTUALIZACION DE CLIENTES!!!")
        '    myReaderCli.Close()
        '    MysqlConn.Close()
        'Catch ex As Exception
        '    log("Error ACTUALIZACION: " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '    MysqlConn.Close()
        'End Try
    End Sub

    Function sincronizarExtractosBancarios()
        If (Not ejecutandoExtractosBancarios) Then
            ejecutandoExtractosBancarios = True
            'Verificar todas las Reservas que esten con estado = 'Pendiente' and e_sap = is null
            Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            'Sequence	AcctCode	Ref	DueDate  	Memo	DebAmount	CredAmnt

            Dim obnk As SAPbobsCOM.BankPages


            Try
                MysqlConn.Open()

                Dim mySelectQuery As String = "SELECT id_ext,e.cuenta,fecha_reg,fecha_trans,cod_mov,concepto,debe,haber,c.cta_cont,e_sap FROM  extractos_ext e, bcos_ctas c  WHERE e.cuenta = c.cuenta  AND confirmado = 'Si' and e_sap IS NULL"

                Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)

                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                While myReader.Read()
                    obnk = oCnn.GetBusinessObject(BoObjectTypes.oBankPages)

                    Dim id_ext = myReader.GetInt32("id_ext")
                    Dim cuenta = myReader.GetString("cuenta")
                    Dim fecha_reg = myReader.GetString("fecha_reg")
                    Dim fecha_trans = myReader.GetString("fecha_trans")
                    Dim cod_mov = myReader.GetString("cod_mov")
                    Dim concepto = myReader.GetString("concepto")

                    Dim cta_cont = myReader.GetString("cta_cont")
                    Dim debe = myReader.GetDouble("debe")
                    Dim haber = myReader.GetDouble("haber")

                    Console.WriteLine(id_ext & "  " & cuenta & " " & fecha_reg & " " & cod_mov & " " & concepto)

                    obnk.AccountCode = cta_cont

                    If (debe > 0) Then
                        obnk.DebitAmount = debe
                    Else
                        obnk.CreditAmount = haber
                    End If

                    obnk.DueDate = fecha_trans
                    obnk.Memo = concepto
                    obnk.Reference = cod_mov

                    obnk.ExternalCode = cod_mov

                    Dim nErr As Long
                    Dim errMsg As String = ""

                    If (0 <> obnk.Add()) Then
                        Call oCnn.GetLastError(nErr, errMsg)
                        log("Failed to add   obankPages   " & nErr & " " & errMsg)
                    Else
                        
                        MyExec("update extractos_ext set e_sap = 1 where id_ext = " & id_ext & ";")
                        'Console.WriteLine(fecha_reg & "  " & fecha_trans & "  " & cod_mov & "  " & concepto & "  " & debe & "  " & haber)

                    End If
                End While

            Catch ex As Exception
                log("Error ButtonExtractosBanc : " & ex.Message & " STACK TRACE: " & ex.StackTrace)
            End Try

            MysqlConn.Close()

            ejecutandoExtractosBancarios = False

        End If

        Return Nothing
    End Function
    Private Sub ButtonExtractosBanc_Click(sender As Object, e As EventArgs) Handles ButtonExtractosBanc.Click
        sincronizarExtractosBancarios()
    End Sub


    Private Sub ButtonUpdateUbic_Click(sender As Object, e As EventArgs) Handles ButtonUpdateUbic.Click
        'ejecutando = False

        'If (Not ejecutando) Then
        '    ejecutando = True
        '    'truncateUserTables("REG_UBIC")
        '    Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)
        '    Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionStringMarijoa)
        '    Try
        '        Dim ms_con As New SqlConnection(MSSQLConnectionString)
        '        Dim ms_con_aux As New SqlConnection(MSSQLConnectionString)
        '        ms_con.Open()
        '        ms_con_aux.Open()

        '        MysqlConn.Open()
        '        MysqlAux.Open()

        '        Dim query As String = "SELECT ItemCode, BatchNum FROM   OIBT i where  WhsCode = '00' and U_ubic  = ''"
        '        Dim ms_cmd As New SqlCommand(query, ms_con)
        '        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()


        '        Dim oCompanyService As SAPbobsCOM.CompanyService = oCnn.GetCompanyService()

        '        Dim oBatchNumberService As SAPbobsCOM.BatchNumberDetailsService
        '        Dim oBatchNumberDetailParams As SAPbobsCOM.BatchNumberDetailParams
        '        Dim oBatchNumberDetail As SAPbobsCOM.BatchNumberDetail
        '        Dim inum As String

        '        oBatchNumberService = oCompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.BatchNumberDetailsService)
        '        oBatchNumberDetailParams = oBatchNumberService.GetDataInterface(SAPbobsCOM.BatchNumberDetailsServiceDataInterfaces.bndsBatchNumberDetailParams)


        '        While (ms_reader.Read())
        '            Dim ItemCode As String = ms_reader(0)
        '            Dim BatchNum As String = ms_reader(1)

        '            Dim mySelectQuery As String = "SELECT operacion,suc,estante,fila,col FROM ubic WHERE codigo = '" & BatchNum & "' ORDER BY id DESC LIMIT 1"
        '            Dim myCommand As New MySqlCommand(mySelectQuery, MysqlConn)
        '            Dim myReader As MySqlDataReader
        '            myReader = myCommand.ExecuteReader()
        '            If (myReader.HasRows()) Then
        '                myReader.Read()
        '                Try
        '                    Dim operacion = myReader.GetString("operacion")
        '                    Dim suc = myReader.GetString("suc")
        '                    Dim estante = myReader.GetString("estante")
        '                    Dim fila = myReader.GetInt32("fila")
        '                    Dim col = myReader.GetInt32("col")

        '                    'Dim updt = "UPDATE OIBT SET U_ubic = '" & estante & "-" & fila & "-" & col & "'  WHERE ItemCode = '" & ItemCode & "' and BatchNum = '" & BatchNum & "' and WhsCode = '" & suc & "'"
        '                    'Dim myUpd As New MySqlCommand(updt, MysqlAux)
        '                    'myUpd.ExecuteNonQuery()

        '                    Dim AbsEntry = getAbsEntry(ItemCode, BatchNum)

        '                    If (AbsEntry > 0) Then
        '                        oBatchNumberDetailParams.DocEntry = AbsEntry 'AbsEntry
        '                        oBatchNumberDetail = oBatchNumberService.Get(oBatchNumberDetailParams)
        '                        inum = oBatchNumberDetail.Batch
        '                        Dim ubic = estante & "-" & fila & "-" & col
        '                        oBatchNumberDetail.UserFields().Item("U_ubic").Value = ubic

        '                        oBatchNumberService.Update(oBatchNumberDetail)
        '                        log("Actualizando ubicacion de Lote: " + BatchNum + " a: " + ubic)
        '                    End If



        '                    Dim maxsql = "SELECT ISNULL( MAX(Code + 0),0) + 1 as MAXIMO from [@REG_UBIC] "
        '                    Dim maxCommand As New SqlCommand(maxsql, ms_con_aux)
        '                    Dim maxReader As SqlDataReader
        '                    maxReader = maxCommand.ExecuteReader()
        '                    maxReader.Read()
        '                    Dim max = maxReader(0)
        '                    maxReader.Close()

        '                    Dim ins = "INSERT INTO [@REG_UBIC] (Code,Name,U_codigo,U_lote,U_suc,U_tipo,U_nombre,U_nro_pallet,U_fila,U_col, U_fecha_hora) values(" & max & "," & max & ",'" & ItemCode & "','" & BatchNum & "', '" & suc & "','Estante','" & estante & "',''," & fila & "," & col & ", CURRENT_TIMESTAMP )"
        '                    Dim insCommand As New SqlCommand(ins, ms_con_aux)
        '                    insCommand.ExecuteNonQuery()

        '                Catch ex As Exception
        '                    log("Error Ubic: " & ex.Message & " STACK TRACE: " & ex.StackTrace & " Lote " & BatchNum)
        '                End Try
        '            Else
        '                log("Ubicacion de Lote: " + BatchNum + " No encontrada ")

        '            End If
        '            myReader.Close()
        '        End While
        '        ms_reader.Close()
        '        MysqlConn.Close()
        '        MysqlAux.Close()
        '        ms_con_aux.Close()
        '        ms_con.Close()
        '    Catch ex As Exception
        '        log("Error Ubic: " & ex.Message & " STACK TRACE: " & ex.StackTrace)
        '        MysqlConn.Close()
        '        MysqlAux.Close()

        '    End Try
        'End If
    End Sub

    Private Sub TimerNotasCredito_Tick(sender As Object, e As EventArgs) Handles TimerNotasCredito.Tick
        Try
            Dim ThreadNotasCredito As New Thread(AddressOf Me.SincronizarNotasCredito)
            ThreadNotasCredito.Start()
        Catch ex As Exception
            log("Error ThreadNotasCredito: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub TimerSyncBancExtract_Tick(sender As Object, e As EventArgs) Handles TimerSyncBancExtract.Tick
        Try
            Dim ThreadExtractosBancarios As New Thread(AddressOf Me.sincronizarExtractosBancarios)
            ThreadExtractosBancarios.Start()
        Catch ex As Exception
            log("Error ThreadExtractosBancarios: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ButtonCall_Click(sender As Object, e As EventArgs) Handles ButtonCall.Click
        ajustesNegativosXFactura(431, "mathiasv", "G$", 1, "02", Date.Now, "%")
    End Sub

    Private Sub ButtonFraccionamientos_Click(sender As Object, e As EventArgs) Handles ButtonFraccionamientos.Click
        SincronizarFraccionamientosPositivos()
    End Sub

    Private Sub TimerFracPos_Tick(sender As Object, e As EventArgs) Handles TimerFracPos.Tick
        Try
            Dim ThreadFraccioonamientosPositivos As New Thread(AddressOf Me.SincronizarFraccionamientosPositivos)
            ThreadFraccioonamientosPositivos.Start()
        Catch ex As Exception
            log("Error ThreadFraccioonamientosPositivos: " & ex.StackTrace)
        End Try
    End Sub

    Private Function getCuentaContableBanco(nro_cuenta As String)
        '
        Dim MysqlAux As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        Try
            MysqlAux.Open()
            Dim qry = "SELECT cta_cont FROM bcos_ctas WHERE cuenta = '" & nro_cuenta & "'"
            Dim myCommand As New MySqlCommand(qry, MysqlAux)
            Dim myReader As MySqlDataReader
            myReader = myCommand.ExecuteReader()
            myReader.Read()
            Dim cta_cont = myReader.GetString("cta_cont")
            Return cta_cont
        Catch ex As Exception
            MysqlAux.Close()
        End Try
        Return Nothing
    End Function


    Public Function SincronizarFraccionamientosPositivos()
        If (Not ejecutandoFraccionamientosPositivos) Then
            ejecutandoFraccionamientosPositivos = True
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim ms As New SqlConnection
            ms.ConnectionString = MSSQLConnectionString

            Dim entrada As SAPbobsCOM.Documents

            Try
                MyConn.Open()

                ms.Open()


                Dim mySelectQuery As String = "SELECT id_frac,usuario,codigo,lote,tipo,signo,cantidad,um,p_costo,valor,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,tara,kg_desc,ancho,gramaje,suc,padre,cta_cont FROM fraccionamientos WHERE signo = '+' AND  e_sap = 0"
                Dim myCommand As New MySqlCommand(mySelectQuery, MyConn)
                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                While myReader.Read()
                    Dim id_frac = 0
                    Try
                        entrada = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenEntry)

                        id_frac = myReader.GetInt32("id_frac")
                        Dim usuario = myReader.GetString("usuario")
                        Dim codigo = myReader.GetString("codigo")
                        Dim lote = myReader.GetString("lote")
                        Dim tipo = myReader.GetString("tipo")
                        Dim signo = myReader.GetString("signo")
                        Dim motivo = myReader.GetString("motivo")
                        Dim cantidad = myReader.GetDouble("cantidad")
                        Dim p_costo = myReader.GetDouble("p_costo")
                        Dim valor = myReader.GetDouble("valor")
                        Dim um = myReader.GetString("um")
                        Dim fecha_lat = myReader.GetString("fecha_lat")
                        Dim hora = myReader.GetString("hora")
                        Dim tara = myReader.GetInt32("tara")
                        Dim gramaje = CDbl(myReader.GetDouble("gramaje"))
                        Dim kg_desc = myReader.GetDouble("kg_desc")
                        Dim ancho = myReader.GetDouble("ancho")
                        Dim suc = myReader.GetString("suc")
                        Dim padre = myReader.GetString("padre")
                        Dim cuenta = myReader.GetString("cta_cont")

                        entrada.Reference2 = id_frac
                        entrada.Series = 25  ' NNM1
                        entrada.UserFields.Fields.Item("U_Nro_Interno").Value = id_frac
                        entrada.UserFields.Fields.Item("U_Usuario").Value = usuario
                        entrada.UserFields.Fields.Item("U_SUC").Value = suc
                        entrada.DocCurrency = "G$"

                        entrada.DocDate = Date.Now
                        entrada.DocDueDate = fecha_lat
                        entrada.JournalMemo = Microsoft.VisualBasic.Left("Fraccionamiento (+) :" & id_frac & " ", 50) ' Max Length 50
                        entrada.Comments = Microsoft.VisualBasic.Left("" & motivo & " ID:" & id_frac & "  ", 254) ' Max Length 254
                        ''entrada.DocTotal = valor

                        entrada.Lines.ItemCode = codigo
                        entrada.Lines.WarehouseCode = suc
                        entrada.Lines.Quantity = cantidad
                        entrada.Lines.UnitPrice = p_costo
                        entrada.Lines.AccountCode = "1.1.3.1.05"  'Cuenta del Mayor  Fracionamiento de Productos
                        'Lote
                        entrada.Lines.BatchNumbers.BatchNumber = lote
                        entrada.Lines.BatchNumbers.Quantity = cantidad
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_quty_c_um").Value = cantidad
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_ancho").Value = ancho
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_kg_desc").Value = kg_desc
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_padre").Value = padre
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_tara").Value = tara
                        entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_gramaje").Value = gramaje

                        'Demas datos del Hijo
                        Dim datos = "SELECT isnull(U_img,'') as U_img,U_F1,U_F2,U_F3,U_factor_precio,ISNULL(U_ubic,'') as U_ubic ,isnull(U_color_comercial,'00-0000') as U_color_comercial,isnull(U_color_comb,'') as U_color_comb,isnull(U_color_cod_fabric,'') as U_color_cod_fabric,isnull(U_prov_mar,'') as U_prov_mar,isnull(U_bag,'') as U_bag,isnull(U_umc,'') as U_umc,U_ancho,U_gramaje_m,isnull(U_design,'') as U_design,isnull(U_rec,'') as U_rec,isnull(U_estado_venta,'Normal') as U_estado_venta,U_desc1,U_desc2,U_desc3,U_desc4,U_desc5,U_desc6,U_desc7 from OIBT where ItemCode = '" & codigo & "' and BatchNum = '" & padre & "' "

                        Dim ms_cmd As New SqlCommand(datos, ms)

                        Dim ms_reader As SqlDataReader = ms_cmd.ExecuteReader()
                        If ms_reader.Read() Then
                            Dim U_img = ms_reader(0)
                            Dim U_F1 = CDbl(ms_reader(1))
                            Dim U_F2 = CDbl(ms_reader(2))
                            Dim U_F3 = CDbl(ms_reader(3))
                            Dim U_factor_precio = ms_reader(4)
                            Dim U_ubic = ms_reader(5)
                            Dim U_color_comercial = ms_reader(6)
                            Dim U_color_comb = ms_reader(7)
                            Dim U_color_cod_fabric = ms_reader(8)
                            Dim U_prov_mar = ms_reader(9)
                            Dim U_bag = ms_reader(10)
                            Dim U_umc = ms_reader(11)

                            Dim U_gramaje_m = CDbl(ms_reader(13))
                            Dim U_design = ms_reader(14)
                            Dim U_rec = ms_reader(15)
                            Dim U_estado_venta = ms_reader(16)
                            Dim U_desc1 = CDbl(ms_reader(17))
                            Dim U_desc2 = CDbl(ms_reader(18))
                            Dim U_desc3 = CDbl(ms_reader(19))
                            Dim U_desc4 = CDbl(ms_reader(20))
                            Dim U_desc5 = CDbl(ms_reader(21))
                            Dim U_desc6 = CDbl(ms_reader(22))
                            Dim U_desc7 = CDbl(ms_reader(23))
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_img").Value = U_img
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_F1").Value = U_F1
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_F2").Value = U_F2
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_F3").Value = U_F3
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_factor_precio").Value = U_factor_precio
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_ubic").Value = U_ubic
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comercial").Value = U_color_comercial
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_color_comb").Value = U_color_comb
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_color_cod_fabric").Value = U_color_cod_fabric
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_prov_mar").Value = U_prov_mar
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_bag").Value = U_bag
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_umc").Value = U_umc

                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_gramaje_m").Value = U_gramaje_m
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_design").Value = U_design
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_rec").Value = U_rec
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_estado_venta").Value = U_estado_venta
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc1").Value = U_desc1
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc2").Value = U_desc2
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc3").Value = U_desc3
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc4").Value = U_desc4
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc5").Value = U_desc5
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc6").Value = U_desc6
                            entrada.Lines.BatchNumbers.UserFields.Fields.Item("U_desc7").Value = U_desc7

                            'Dim upd = "UPDATE OIBT SET U_img = '" & U_img & "', U_F1= '" & U_F1 & "', U_F2= '" & U_F2 & "', U_F3= '" & U_F3 & "', U_factor_precio= '" & U_factor_precio & "', U_ubic= '" & U_ubic & "', U_color_comercial= '" & U_color_comercial & "', U_color_comb= '" & U_color_comb & "', U_color_cod_fabric= '" & U_color_cod_fabric & "', U_prov_mar= '" & U_prov_mar & "', U_bag= '" & U_bag & "', U_umc= '" & U_umc & "',  U_ancho = " & ancho & ", U_gramaje = " & gramaje & ", U_gramaje_m= " & U_gramaje_m & ",U_design= '" & U_design & "', U_rec= '" & U_rec & "', U_estado_venta= '" & U_estado_venta & "', U_desc1= " & U_desc1 & ", U_desc2= " & U_desc2 & ", U_desc3= " & U_desc3 & ", U_desc4= " & U_desc4 & ", U_desc5= " & U_desc5 & ", U_desc6= " & U_desc6 & ", U_desc7 = " & U_desc7 & ", U_padre = '" & padre & "',U_tara = " & tara & ",U_kg_desc = " & kg_desc & ", U_quty_ticket = 0,U_equiv =" & cantidad & "  WHERE BatchNum = '" & lote & "'; "

                        End If

                        ms_reader.Close() 'Este fue agregado por el siguiente mensaje Ya existe un DataReader ligado a una 

                        iError = entrada.Add

                        If iError <> 0 Then
                            oCnn.GetLastError(iError, sError)
                            log(iError & " " & sError & " Fraccionamiento (+): ID:" & id_frac)
                        Else
                            Dim DocEntry As String = oCnn.GetNewObjectKey
                            log("Fracionamiento (+) DocEntry: " & DocEntry)
                            Try

                                Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 1 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado"
                                MyExec(updQuery)

                            Catch ex As Exception
                                log("Error al actualizar datos del lote hijo Fraccionamiento (+): " & ex.StackTrace)
                                'Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 3 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado a 3 = error "
                                'Dim updCommand As New MySqlCommand(updQuery, MyAux)
                                'updCommand.ExecuteNonQuery()
                            Finally

                            End Try

                        End If


                    Catch ex As Exception
                        log("ALERTA Fraccionamiento (+) NO EJECUTADO  " & ex.StackTrace)
                        'Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 3 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado a 3 = error"
                        'Dim updCommand As New MySqlCommand(updQuery, MyAux)
                        'updCommand.ExecuteNonQuery()
                        'MyAux.Close()
                        'ms.Close()
                    End Try

                End While
                myReader.Close()


            Catch ex As Exception
                log("Error Fraccionamiento (+) No se puede conectar a la base de datos " & ex.Message)
            Finally

            End Try

            MyConn.Close()

            ms.Close()
            ejecutandoFraccionamientosPositivos = False
        End If
        Return Nothing
    End Function



    Private Sub TimerFracNeg_Tick(sender As Object, e As EventArgs) Handles TimerFracNeg.Tick
        Try
            Dim ThreadFraccioonamientosNegativos As New Thread(AddressOf Me.SincronizarFraccionamientosNegatitivos)
            ThreadFraccioonamientosNegativos.Start()
        Catch ex As Exception
            log("Error ThreadFraccioonamientosPositivos: " & ex.StackTrace)
        End Try
    End Sub

    Private Sub ButtonFraccNegativo_Click(sender As Object, e As EventArgs) Handles ButtonFraccNegativo.Click
        SincronizarFraccionamientosNegatitivos()
    End Sub

    Public Function MyExec(command As String)
        Dim myCon As MySqlConnection = New MySqlConnection(MySQLConnectionString)

        Try
            myCon.Open()
            Dim cmd As New MySqlCommand(command, myCon)
            cmd.ExecuteNonQuery()
            myCon.Close()
        Catch ex As Exception
            log("Error No se pudo conectar (MyExec): " & ex.StackTrace)
            Return False
        End Try
        Return True
    End Function

    Public Function MsExcec(command As String)
        Dim msc As New SqlConnection
        msc.ConnectionString = MSSQLConnectionString
        Try
            msc.Open()
            Dim cmd As New SqlCommand(command, msc)
            cmd.ExecuteNonQuery()
            msc.Close()
        Catch ex As Exception
            log("Error No se pudo conectar (MsExec): " & ex.StackTrace)
            Return False
        End Try
        Return True
    End Function
    Public Function SincronizarFraccionamientosNegatitivos()
        If (Not ejecutandoFraccionamientosNegativos) Then
            ejecutandoFraccionamientosNegativos = True
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)

            Dim ms As New SqlConnection
            ms.ConnectionString = MSSQLConnectionString

            Dim salida As SAPbobsCOM.Documents

            Try
                MyConn.Open()

                ms.Open()

                Dim mySelectQuery As String = "SELECT id_frac,usuario,codigo,lote,tipo,signo,cantidad,um,p_costo,valor,motivo,DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_lat,hora,tara,kg_desc,ancho,gramaje,suc,cta_cont FROM fraccionamientos WHERE signo = '-' AND  e_sap = 0"
                Dim myCommand As New MySqlCommand(mySelectQuery, MyConn)
                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                While myReader.Read()
                    Dim id_frac = 0
                    Try
                        salida = oCnn.GetBusinessObject(BoObjectTypes.oInventoryGenExit)

                        id_frac = myReader.GetInt32("id_frac")
                        Dim usuario = myReader.GetString("usuario")
                        Dim codigo = myReader.GetString("codigo")
                        Dim lote = myReader.GetString("lote")
                        Dim tipo = myReader.GetString("tipo")
                        Dim signo = myReader.GetString("signo")
                        Dim motivo = myReader.GetString("motivo")
                        Dim cantidad = myReader.GetDouble("cantidad")
                        Dim p_costo = myReader.GetDouble("p_costo")
                        Dim valor = myReader.GetDouble("valor")
                        Dim um = myReader.GetString("um")
                        Dim fecha_lat = myReader.GetString("fecha_lat")
                        Dim hora = myReader.GetString("hora")
                        Dim tara = myReader.GetDouble("tara")
                        Dim gramaje = myReader.GetDouble("gramaje")
                        Dim kg_desc = myReader.GetDouble("kg_desc")
                        'Dim ancho = myReader.GetDouble("ancho")
                        Dim suc = myReader.GetString("suc")

                        Dim cuenta = myReader.GetString("cta_cont")

                        salida.Reference2 = id_frac
                        salida.Series = 26  ' NNM1
                        salida.UserFields.Fields.Item("U_Nro_Interno").Value = id_frac
                        salida.UserFields.Fields.Item("U_Usuario").Value = usuario
                        salida.UserFields.Fields.Item("U_SUC").Value = suc
                        salida.DocCurrency = "G$"

                        salida.DocDate = Date.Now
                        salida.DocDueDate = fecha_lat
                        salida.JournalMemo = Microsoft.VisualBasic.Left("Fraccionamiento (-) :" & id_frac & " ", 50) ' Max Length 50
                        salida.Comments = Microsoft.VisualBasic.Left("" & motivo & " ID:" & id_frac & "  ", 254) ' Max Length 254

                        salida.Lines.ItemCode = codigo
                        salida.Lines.WarehouseCode = suc
                        salida.Lines.Quantity = cantidad
                        salida.Lines.UnitPrice = p_costo
                        salida.Lines.AccountCode = "1.1.3.1.05"  'Cuenta del Mayor  Fracionamiento de Productos

                        'Lote
                        salida.Lines.BatchNumbers.BatchNumber = lote
                        salida.Lines.BatchNumbers.Quantity = cantidad

                        iError = salida.Add

                        If (cantidad > 0) Then

                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log(iError & " " & sError & " Fraccionamiento (-): ID:" & id_frac)
                            Else
                                Dim DocEntry As String = oCnn.GetNewObjectKey
                                log("Fracionamiento (-) DocEntry: " & DocEntry)
                                Try

                                    Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 1 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado"
                                    MyExec(updQuery)

                                Catch ex As Exception
                                    log("Error al actualizar datos del lote hijo Fraccionamiento (-): " & ex.StackTrace)
                                    Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 3 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado a 3 = error "
                                    MyExec(updQuery)
                                End Try

                            End If
                        Else
                            Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 1 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado xque es 0"
                            MyExec(updQuery)
                        End If

                    Catch ex As Exception
                        log("Error Fraccionamiento (-) COM " & ex.Message)
                        Dim updQuery As String = "UPDATE fraccionamientos SET e_sap = 3 WHERE id_frac = " & id_frac & ";"  ' Actualizar estado a 3 = error"
                        MyExec(updQuery)
                    End Try

                End While
                myReader.Close()

                'log("Connection to SQL Server Opened")

            Catch ex As Exception
                log("Error Fraccionamiento (+) No se puede conectar a la base de datos " & ex.Message)
           
            End Try

            MyConn.Close()

            ms.Close() 'Whether there is error or not. Close the connection.

            ejecutandoFraccionamientosNegativos = False
        End If
        Return Nothing
    End Function

    Private Sub Button_UpdateCatFactura(sender As Object, e As EventArgs) Handles UpdateCatFactura.Click
        Dim MysqlConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)
        Try

            MysqlConn.Open()

            Dim msql As New SqlConnection(MSSQLConnectionString)
            msql.Open()

            Dim DocNum = TextBoxDocEntry.Text

            Dim msQuery As String = "select DocEntry,U_Nro_Interno,FolioPref, FolioNum, CardCode,CardName,U_SUC,U_vendedor from OINV where DocNum = " & DocNum & "  "


            Dim ms_cmd As New SqlCommand(msQuery, msql)
            Dim reader As SqlDataReader = ms_cmd.ExecuteReader()



            'Create the BusinessPartners object


            Dim OldU_Nro_Interno = 0

            While reader.Read()

                Dim DocEntry As Integer = reader(0)
                Dim U_Nro_Interno As Integer = reader(1)


                Dim oinv As SAPbobsCOM.Documents
                oinv = oCnn.GetBusinessObject(BoObjectTypes.oInvoices)


                    If (oinv.GetByKey(DocEntry) = True) Then


                    Dim Prefix = TextBoxPrefix.Text
                    Dim Folio = Integer.Parse(TextBoxFolio.Text)
                    Dim Tipo = ComboBoxTipo.Text

                    If (Prefix = "") Then
                        oinv.FolioPrefixString = vbNullString
                        oinv.FolioNumber = vbNullString
                    Else
                        oinv.FolioPrefixString = Prefix
                        oinv.FolioNumber = Folio
                    End If

                    If (Tipo <> "No Modificar") Then
                        If (Tipo = "Contado") Then
                            oinv.PaymentGroupCode = -1 ' Contado
                        Else
                            oinv.PaymentGroupCode = 1  ' Credito
                        End If
                    End If


                    Dim nErr As Long
                    Dim errMsg As String = ""

                    If (0 <> oinv.Update()) Then
                        Call oCnn.GetLastError(nErr, errMsg)
                        log("Error al Update Factura" & nErr & "" & errMsg & "  " & DocEntry)
                    Else
                        log("Factura Updated " & DocEntry & " ")
                    End If

                End If

            End While

            log("FIN DE ACTUALIZACION DE Factura!!!")
            reader.Close()
            MysqlConn.Close()
            msql.Close()
        Catch ex As Exception
            log("Error ACTUALIZACION: " & ex.Message & " STACK TRACE: " & ex.StackTrace)
            MysqlConn.Close()
        End Try
    End Sub

    Public Function cancelarPagos()

        If (Not ejecutandoCancelacionPagos) Then
            ejecutandoCancelacionPagos = True
            Dim MyConn As MySqlConnection = New MySqlConnection(MySQLConnectionString)


            Dim pagos_recibidos As SAPbobsCOM.Payments

            Try
                MyConn.Open()

                Dim mySelectQuery As String = "SELECT f_nro,id_pago FROM pagos_cancelados WHERE e_sap = 0 LIMIT 5"
                Dim myCommand As New MySqlCommand(mySelectQuery, MyConn)
                Dim myReader As MySqlDataReader
                myReader = myCommand.ExecuteReader()

                While myReader.Read()
                    Dim f_nro = 0
                    Dim id_pago = 0
                    Try
                        pagos_recibidos = oCnn.GetBusinessObject(BoObjectTypes.oIncomingPayments)

                        f_nro = myReader.GetInt32("f_nro")
                        id_pago = myReader.GetInt32("id_pago")

                        If (pagos_recibidos.GetByKey(id_pago) = True) Then

                            iError = pagos_recibidos.Cancel()


                            If iError <> 0 Then
                                oCnn.GetLastError(iError, sError)
                                log(iError & " " & sError & " Cancelar Pagos Recibidos ID:" & id_pago)
                            Else
                                Try
                                     
                                    MyExec("UPDATE pagos_cancelados SET e_sap = 1 WHERE id_pago = " & id_pago & ";")
                                    MyExec("UPDATE factura_venta SET e_sap = 1,control_caja = null WHERE  f_nro =  " & f_nro & ";")
                                    log("Pago cancelado Nro: " & id_pago & " Factura: " & f_nro)

                                Catch ex As Exception
                                    log("Error al actualizar pagos_cancelados " & ex.StackTrace)
                                End Try

                            End If
                        End If

                    Catch ex As Exception
                        log("Error Cancelar Pagos Recibidos ID:" & id_pago & "  " & ex.Message)
                    End Try


                End While
                myReader.Close()


            Catch ex As Exception
                log("Error Cancelar Pagos Recibidos " & ex.Message)
            End Try

            MyConn.Close()

            ejecutandoCancelacionPagos = False
        End If
        Return Nothing
    End Function
    Private Sub ButtonCancelPagos_Click(sender As Object, e As EventArgs) Handles ButtonCancelPagos.Click
        cancelarPagos()
    End Sub

    
    Private Sub TimerCancelPagos_Tick(sender As Object, e As EventArgs) Handles TimerCancelPagos.Tick
        Try
            Dim ThreadCancelarPagos As New Thread(AddressOf Me.cancelarPagos)
            ThreadCancelarPagos.Start()
        Catch ex As Exception
            log("Error TimerCancelPagos: " & ex.StackTrace)
        End Try
    End Sub
End Class






