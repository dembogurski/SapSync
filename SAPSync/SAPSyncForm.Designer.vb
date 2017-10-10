<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SAPSyncForm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SAPSyncForm))
        Me.ButtonConnect = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.server = New System.Windows.Forms.TextBox()
        Me.user = New System.Windows.Forms.TextBox()
        Me.passw = New System.Windows.Forms.TextBox()
        Me.ButtonStockXVentas = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ms_user = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ms_passw = New System.Windows.Forms.TextBox()
        Me.ButtonSincPagos = New System.Windows.Forms.Button()
        Me.ButtonNotaCredito = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.logTextArea = New System.Windows.Forms.RichTextBox()
        Me.ButtonReservas = New System.Windows.Forms.Button()
        Me.TimerStockXVentas = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonUpdateBatchNumber = New System.Windows.Forms.Button()
        Me.ButtonAjustes = New System.Windows.Forms.Button()
        Me.ButtonRemisiones = New System.Windows.Forms.Button()
        Me.ButtonAjustesNeg = New System.Windows.Forms.Button()
        Me.label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.sap_passw = New System.Windows.Forms.TextBox()
        Me.TimerAjustesPos = New System.Windows.Forms.Timer(Me.components)
        Me.TimerAjustesNeg = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonSincCompras = New System.Windows.Forms.Button()
        Me.TimerCotiz = New System.Windows.Forms.Timer(Me.components)
        Me.TimerCompras = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonMigrarClientes = New System.Windows.Forms.Button()
        Me.ButtonMigrarLotes = New System.Windows.Forms.Button()
        Me.ButtonMigrarArticulos = New System.Windows.Forms.Button()
        Me.ButtonCobroCuotas = New System.Windows.Forms.Button()
        Me.ButtonSincClientes = New System.Windows.Forms.Button()
        Me.TimerSincClientes = New System.Windows.Forms.Timer(Me.components)
        Me.TimerReservas = New System.Windows.Forms.Timer(Me.components)
        Me.TimerRemisiones = New System.Windows.Forms.Timer(Me.components)
        Me.TimerCobroCuotas = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonAsientos = New System.Windows.Forms.Button()
        Me.TimerAsientos = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonDisconect = New System.Windows.Forms.Button()
        Me.ButtonUpdatePriceList = New System.Windows.Forms.Button()
        Me.company_db = New System.Windows.Forms.ComboBox()
        Me.ButtonUserTables = New System.Windows.Forms.Button()
        Me.ButtonDeleteClienes = New System.Windows.Forms.Button()
        Me.TimerUptateLotes = New System.Windows.Forms.Timer(Me.components)
        Me.TimerPagos = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonMigrarProv = New System.Windows.Forms.Button()
        Me.sap_user = New System.Windows.Forms.ComboBox()
        Me.CheckBoxAutoConnect = New System.Windows.Forms.CheckBox()
        Me.counter = New System.Windows.Forms.Label()
        Me.TimerConnect = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonCheckSapCodes = New System.Windows.Forms.Button()
        Me.ButtonUpdateCliData = New System.Windows.Forms.Button()
        Me.ButtonExtractosBanc = New System.Windows.Forms.Button()
        Me.ButtonUpdateUbic = New System.Windows.Forms.Button()
        Me.TimerNotasCredito = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSyncBancExtract = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonCall = New System.Windows.Forms.Button()
        Me.TextBoxLimite = New System.Windows.Forms.TextBox()
        Me.ButtonFraccionamientos = New System.Windows.Forms.Button()
        Me.TimerFracPos = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonFraccNegativo = New System.Windows.Forms.Button()
        Me.TimerFracNeg = New System.Windows.Forms.Timer(Me.components)
        Me.UpdateCatFactura = New System.Windows.Forms.Button()
        Me.ButtonCancelPagos = New System.Windows.Forms.Button()
        Me.TimerCancelPagos = New System.Windows.Forms.Timer(Me.components)
        Me.TextBoxDocEntry = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TextBoxPrefix = New System.Windows.Forms.TextBox()
        Me.TextBoxFolio = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.ComboBoxTipo = New System.Windows.Forms.ComboBox()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonConnect
        '
        Me.ButtonConnect.Location = New System.Drawing.Point(171, 29)
        Me.ButtonConnect.Name = "ButtonConnect"
        Me.ButtonConnect.Size = New System.Drawing.Size(286, 32)
        Me.ButtonConnect.TabIndex = 0
        Me.ButtonConnect.Text = "Conectar a Marijoa SAP"
        Me.ButtonConnect.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 133)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "MySQL User:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(31, 166)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "MySQL Passw:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(31, 100)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "MySQL Server"
        '
        'server
        '
        Me.server.Location = New System.Drawing.Point(120, 98)
        Me.server.Name = "server"
        Me.server.Size = New System.Drawing.Size(144, 20)
        Me.server.TabIndex = 5
        Me.server.Text = "192.168.2.220"
        '
        'user
        '
        Me.user.Location = New System.Drawing.Point(120, 133)
        Me.user.Name = "user"
        Me.user.Size = New System.Drawing.Size(141, 20)
        Me.user.TabIndex = 6
        Me.user.Text = "sync"
        '
        'passw
        '
        Me.passw.Location = New System.Drawing.Point(120, 166)
        Me.passw.Name = "passw"
        Me.passw.Size = New System.Drawing.Size(139, 20)
        Me.passw.TabIndex = 7
        Me.passw.Text = "case"
        Me.passw.UseSystemPasswordChar = True
        '
        'ButtonStockXVentas
        '
        Me.ButtonStockXVentas.BackColor = System.Drawing.Color.Transparent
        Me.ButtonStockXVentas.Enabled = False
        Me.ButtonStockXVentas.Location = New System.Drawing.Point(34, 212)
        Me.ButtonStockXVentas.Name = "ButtonStockXVentas"
        Me.ButtonStockXVentas.Size = New System.Drawing.Size(124, 28)
        Me.ButtonStockXVentas.TabIndex = 8
        Me.ButtonStockXVentas.Text = "Sinc Stock x Ventas"
        Me.ButtonStockXVentas.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(287, 136)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "MSSQL User:"
        '
        'ms_user
        '
        Me.ms_user.Location = New System.Drawing.Point(380, 133)
        Me.ms_user.Name = "ms_user"
        Me.ms_user.Size = New System.Drawing.Size(143, 20)
        Me.ms_user.TabIndex = 10
        Me.ms_user.Text = "sa"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(287, 169)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "MSSQL Passw:"
        '
        'ms_passw
        '
        Me.ms_passw.Location = New System.Drawing.Point(381, 164)
        Me.ms_passw.Name = "ms_passw"
        Me.ms_passw.Size = New System.Drawing.Size(141, 20)
        Me.ms_passw.TabIndex = 12
        Me.ms_passw.Text = "Marijoa123."
        Me.ms_passw.UseSystemPasswordChar = True
        '
        'ButtonSincPagos
        '
        Me.ButtonSincPagos.Enabled = False
        Me.ButtonSincPagos.Location = New System.Drawing.Point(34, 246)
        Me.ButtonSincPagos.Name = "ButtonSincPagos"
        Me.ButtonSincPagos.Size = New System.Drawing.Size(124, 28)
        Me.ButtonSincPagos.TabIndex = 14
        Me.ButtonSincPagos.Text = "Sinc Pagos Facturas"
        Me.ButtonSincPagos.UseVisualStyleBackColor = True
        '
        'ButtonNotaCredito
        '
        Me.ButtonNotaCredito.Enabled = False
        Me.ButtonNotaCredito.Location = New System.Drawing.Point(34, 312)
        Me.ButtonNotaCredito.Name = "ButtonNotaCredito"
        Me.ButtonNotaCredito.Size = New System.Drawing.Size(124, 31)
        Me.ButtonNotaCredito.TabIndex = 15
        Me.ButtonNotaCredito.Text = "Sinc Notas de Credito"
        Me.ButtonNotaCredito.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.logTextArea)
        Me.Panel1.Location = New System.Drawing.Point(34, 471)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(672, 218)
        Me.Panel1.TabIndex = 16
        '
        'logTextArea
        '
        Me.logTextArea.Dock = System.Windows.Forms.DockStyle.Fill
        Me.logTextArea.Location = New System.Drawing.Point(0, 0)
        Me.logTextArea.Name = "logTextArea"
        Me.logTextArea.Size = New System.Drawing.Size(672, 218)
        Me.logTextArea.TabIndex = 0
        Me.logTextArea.Text = ""
        '
        'ButtonReservas
        '
        Me.ButtonReservas.Enabled = False
        Me.ButtonReservas.Location = New System.Drawing.Point(170, 211)
        Me.ButtonReservas.Name = "ButtonReservas"
        Me.ButtonReservas.Size = New System.Drawing.Size(154, 28)
        Me.ButtonReservas.TabIndex = 17
        Me.ButtonReservas.Text = "Sinc Reservas"
        Me.ButtonReservas.UseVisualStyleBackColor = True
        '
        'TimerStockXVentas
        '
        Me.TimerStockXVentas.Interval = 15000
        '
        'ButtonUpdateBatchNumber
        '
        Me.ButtonUpdateBatchNumber.Enabled = False
        Me.ButtonUpdateBatchNumber.Location = New System.Drawing.Point(330, 313)
        Me.ButtonUpdateBatchNumber.Name = "ButtonUpdateBatchNumber"
        Me.ButtonUpdateBatchNumber.Size = New System.Drawing.Size(135, 30)
        Me.ButtonUpdateBatchNumber.TabIndex = 18
        Me.ButtonUpdateBatchNumber.Text = "Actualizar Lotes"
        Me.ButtonUpdateBatchNumber.UseVisualStyleBackColor = True
        '
        'ButtonAjustes
        '
        Me.ButtonAjustes.Enabled = False
        Me.ButtonAjustes.Location = New System.Drawing.Point(170, 246)
        Me.ButtonAjustes.Name = "ButtonAjustes"
        Me.ButtonAjustes.Size = New System.Drawing.Size(154, 28)
        Me.ButtonAjustes.TabIndex = 19
        Me.ButtonAjustes.Text = "Ajustes(+)"
        Me.ButtonAjustes.UseVisualStyleBackColor = True
        '
        'ButtonRemisiones
        '
        Me.ButtonRemisiones.Enabled = False
        Me.ButtonRemisiones.Location = New System.Drawing.Point(170, 280)
        Me.ButtonRemisiones.Name = "ButtonRemisiones"
        Me.ButtonRemisiones.Size = New System.Drawing.Size(154, 26)
        Me.ButtonRemisiones.TabIndex = 21
        Me.ButtonRemisiones.Text = "Sinc Remisiones"
        Me.ButtonRemisiones.UseVisualStyleBackColor = True
        '
        'ButtonAjustesNeg
        '
        Me.ButtonAjustesNeg.Enabled = False
        Me.ButtonAjustesNeg.Location = New System.Drawing.Point(330, 246)
        Me.ButtonAjustesNeg.Name = "ButtonAjustesNeg"
        Me.ButtonAjustesNeg.Size = New System.Drawing.Size(135, 28)
        Me.ButtonAjustesNeg.TabIndex = 22
        Me.ButtonAjustesNeg.Text = "Ajustes(-)"
        Me.ButtonAjustesNeg.UseVisualStyleBackColor = True
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Location = New System.Drawing.Point(589, 99)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(93, 13)
        Me.label6.TabIndex = 23
        Me.label6.Text = "SAP Company DB"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(589, 128)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(53, 13)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "SAP User"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(589, 159)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(62, 13)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "SAP Passw"
        '
        'sap_passw
        '
        Me.sap_passw.Location = New System.Drawing.Point(714, 156)
        Me.sap_passw.Name = "sap_passw"
        Me.sap_passw.Size = New System.Drawing.Size(167, 20)
        Me.sap_passw.TabIndex = 28
        Me.sap_passw.Text = "rootdba"
        Me.sap_passw.UseSystemPasswordChar = True
        '
        'TimerAjustesPos
        '
        Me.TimerAjustesPos.Interval = 5000
        '
        'TimerAjustesNeg
        '
        Me.TimerAjustesNeg.Interval = 5000
        '
        'ButtonSincCompras
        '
        Me.ButtonSincCompras.Enabled = False
        Me.ButtonSincCompras.Location = New System.Drawing.Point(330, 280)
        Me.ButtonSincCompras.Name = "ButtonSincCompras"
        Me.ButtonSincCompras.Size = New System.Drawing.Size(135, 26)
        Me.ButtonSincCompras.TabIndex = 29
        Me.ButtonSincCompras.Text = "Sinc Compras"
        Me.ButtonSincCompras.UseVisualStyleBackColor = True
        '
        'TimerCotiz
        '
        Me.TimerCotiz.Interval = 5000
        '
        'TimerCompras
        '
        Me.TimerCompras.Interval = 12000
        '
        'ButtonMigrarClientes
        '
        Me.ButtonMigrarClientes.Enabled = False
        Me.ButtonMigrarClientes.Location = New System.Drawing.Point(714, 212)
        Me.ButtonMigrarClientes.Name = "ButtonMigrarClientes"
        Me.ButtonMigrarClientes.Size = New System.Drawing.Size(171, 28)
        Me.ButtonMigrarClientes.TabIndex = 30
        Me.ButtonMigrarClientes.Text = "Migrar Clientes"
        Me.ButtonMigrarClientes.UseVisualStyleBackColor = True
        '
        'ButtonMigrarLotes
        '
        Me.ButtonMigrarLotes.Enabled = False
        Me.ButtonMigrarLotes.Location = New System.Drawing.Point(714, 411)
        Me.ButtonMigrarLotes.Name = "ButtonMigrarLotes"
        Me.ButtonMigrarLotes.Size = New System.Drawing.Size(171, 25)
        Me.ButtonMigrarLotes.TabIndex = 31
        Me.ButtonMigrarLotes.Text = "Migrar Piezas (Lotes)"
        Me.ButtonMigrarLotes.UseVisualStyleBackColor = True
        '
        'ButtonMigrarArticulos
        '
        Me.ButtonMigrarArticulos.Enabled = False
        Me.ButtonMigrarArticulos.Location = New System.Drawing.Point(714, 280)
        Me.ButtonMigrarArticulos.Name = "ButtonMigrarArticulos"
        Me.ButtonMigrarArticulos.Size = New System.Drawing.Size(171, 26)
        Me.ButtonMigrarArticulos.TabIndex = 32
        Me.ButtonMigrarArticulos.Text = "Migrar Articulos (OITM)"
        Me.ButtonMigrarArticulos.UseVisualStyleBackColor = True
        '
        'ButtonCobroCuotas
        '
        Me.ButtonCobroCuotas.Enabled = False
        Me.ButtonCobroCuotas.Location = New System.Drawing.Point(34, 279)
        Me.ButtonCobroCuotas.Name = "ButtonCobroCuotas"
        Me.ButtonCobroCuotas.Size = New System.Drawing.Size(124, 27)
        Me.ButtonCobroCuotas.TabIndex = 33
        Me.ButtonCobroCuotas.Text = "Sinc Cobro Cuotas"
        Me.ButtonCobroCuotas.UseVisualStyleBackColor = True
        '
        'ButtonSincClientes
        '
        Me.ButtonSincClientes.Enabled = False
        Me.ButtonSincClientes.Location = New System.Drawing.Point(330, 211)
        Me.ButtonSincClientes.Name = "ButtonSincClientes"
        Me.ButtonSincClientes.Size = New System.Drawing.Size(135, 27)
        Me.ButtonSincClientes.TabIndex = 34
        Me.ButtonSincClientes.Text = "Sinc Clientes"
        Me.ButtonSincClientes.UseVisualStyleBackColor = True
        '
        'TimerSincClientes
        '
        Me.TimerSincClientes.Interval = 2000
        '
        'TimerReservas
        '
        Me.TimerReservas.Interval = 16000
        '
        'TimerRemisiones
        '
        Me.TimerRemisiones.Interval = 12000
        '
        'TimerCobroCuotas
        '
        Me.TimerCobroCuotas.Interval = 7000
        '
        'ButtonAsientos
        '
        Me.ButtonAsientos.Enabled = False
        Me.ButtonAsientos.Location = New System.Drawing.Point(170, 313)
        Me.ButtonAsientos.Name = "ButtonAsientos"
        Me.ButtonAsientos.Size = New System.Drawing.Size(154, 30)
        Me.ButtonAsientos.TabIndex = 35
        Me.ButtonAsientos.Text = "Sinc Asientos"
        Me.ButtonAsientos.UseVisualStyleBackColor = True
        '
        'TimerAsientos
        '
        Me.TimerAsientos.Interval = 10000
        '
        'ButtonDisconect
        '
        Me.ButtonDisconect.Enabled = False
        Me.ButtonDisconect.Location = New System.Drawing.Point(475, 27)
        Me.ButtonDisconect.Name = "ButtonDisconect"
        Me.ButtonDisconect.Size = New System.Drawing.Size(286, 32)
        Me.ButtonDisconect.TabIndex = 36
        Me.ButtonDisconect.Text = "Desconectar"
        Me.ButtonDisconect.UseVisualStyleBackColor = True
        '
        'ButtonUpdatePriceList
        '
        Me.ButtonUpdatePriceList.Enabled = False
        Me.ButtonUpdatePriceList.Location = New System.Drawing.Point(714, 313)
        Me.ButtonUpdatePriceList.Name = "ButtonUpdatePriceList"
        Me.ButtonUpdatePriceList.Size = New System.Drawing.Size(171, 30)
        Me.ButtonUpdatePriceList.TabIndex = 39
        Me.ButtonUpdatePriceList.Text = "Actualizar Listas Precios"
        Me.ButtonUpdatePriceList.UseVisualStyleBackColor = True
        '
        'company_db
        '
        Me.company_db.FormattingEnabled = True
        Me.company_db.Items.AddRange(New Object() {"MARIJOA_SAP", "MARIJOASA_PRUEBA"})
        Me.company_db.Location = New System.Drawing.Point(714, 96)
        Me.company_db.Name = "company_db"
        Me.company_db.Size = New System.Drawing.Size(167, 21)
        Me.company_db.TabIndex = 40
        Me.company_db.Text = "MARIJOA_SAP"
        '
        'ButtonUserTables
        '
        Me.ButtonUserTables.Enabled = False
        Me.ButtonUserTables.Location = New System.Drawing.Point(714, 349)
        Me.ButtonUserTables.Name = "ButtonUserTables"
        Me.ButtonUserTables.Size = New System.Drawing.Size(171, 30)
        Me.ButtonUserTables.TabIndex = 41
        Me.ButtonUserTables.Text = "Migrar Tablas de Usuario"
        Me.ButtonUserTables.UseVisualStyleBackColor = True
        '
        'ButtonDeleteClienes
        '
        Me.ButtonDeleteClienes.Enabled = False
        Me.ButtonDeleteClienes.Location = New System.Drawing.Point(714, 182)
        Me.ButtonDeleteClienes.Name = "ButtonDeleteClienes"
        Me.ButtonDeleteClienes.Size = New System.Drawing.Size(171, 28)
        Me.ButtonDeleteClienes.TabIndex = 42
        Me.ButtonDeleteClienes.Text = "Eliminar Clientes"
        Me.ButtonDeleteClienes.UseVisualStyleBackColor = True
        '
        'TimerUptateLotes
        '
        Me.TimerUptateLotes.Interval = 4000
        '
        'TimerPagos
        '
        Me.TimerPagos.Interval = 14000
        '
        'ButtonMigrarProv
        '
        Me.ButtonMigrarProv.Enabled = False
        Me.ButtonMigrarProv.Location = New System.Drawing.Point(714, 246)
        Me.ButtonMigrarProv.Name = "ButtonMigrarProv"
        Me.ButtonMigrarProv.Size = New System.Drawing.Size(171, 28)
        Me.ButtonMigrarProv.TabIndex = 31
        Me.ButtonMigrarProv.Text = "Migrar Proveedores"
        Me.ButtonMigrarProv.UseVisualStyleBackColor = True
        '
        'sap_user
        '
        Me.sap_user.FormattingEnabled = True
        Me.sap_user.Items.AddRange(New Object() {"Sistema", "Develop", "SapPHP"})
        Me.sap_user.Location = New System.Drawing.Point(714, 123)
        Me.sap_user.Name = "sap_user"
        Me.sap_user.Size = New System.Drawing.Size(167, 21)
        Me.sap_user.TabIndex = 43
        Me.sap_user.Text = "Sistema"
        '
        'CheckBoxAutoConnect
        '
        Me.CheckBoxAutoConnect.AutoSize = True
        Me.CheckBoxAutoConnect.Location = New System.Drawing.Point(290, 101)
        Me.CheckBoxAutoConnect.Name = "CheckBoxAutoConnect"
        Me.CheckBoxAutoConnect.Size = New System.Drawing.Size(94, 17)
        Me.CheckBoxAutoConnect.TabIndex = 44
        Me.CheckBoxAutoConnect.Text = "Auto Conectar"
        Me.CheckBoxAutoConnect.UseVisualStyleBackColor = True
        '
        'counter
        '
        Me.counter.AutoSize = True
        Me.counter.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.counter.Location = New System.Drawing.Point(456, 87)
        Me.counter.Name = "counter"
        Me.counter.Size = New System.Drawing.Size(44, 31)
        Me.counter.TabIndex = 45
        Me.counter.Text = "10"
        '
        'TimerConnect
        '
        Me.TimerConnect.Enabled = True
        Me.TimerConnect.Interval = 1000
        '
        'ButtonCheckSapCodes
        '
        Me.ButtonCheckSapCodes.AutoEllipsis = True
        Me.ButtonCheckSapCodes.Location = New System.Drawing.Point(714, 382)
        Me.ButtonCheckSapCodes.Name = "ButtonCheckSapCodes"
        Me.ButtonCheckSapCodes.Size = New System.Drawing.Size(171, 23)
        Me.ButtonCheckSapCodes.TabIndex = 46
        Me.ButtonCheckSapCodes.Text = "Check SAP Codes"
        Me.ButtonCheckSapCodes.UseVisualStyleBackColor = True
        '
        'ButtonUpdateCliData
        '
        Me.ButtonUpdateCliData.Enabled = False
        Me.ButtonUpdateCliData.Location = New System.Drawing.Point(714, 471)
        Me.ButtonUpdateCliData.Name = "ButtonUpdateCliData"
        Me.ButtonUpdateCliData.Size = New System.Drawing.Size(171, 28)
        Me.ButtonUpdateCliData.TabIndex = 47
        Me.ButtonUpdateCliData.Text = "Update Clientes"
        Me.ButtonUpdateCliData.UseVisualStyleBackColor = True
        '
        'ButtonExtractosBanc
        '
        Me.ButtonExtractosBanc.Enabled = False
        Me.ButtonExtractosBanc.Location = New System.Drawing.Point(34, 349)
        Me.ButtonExtractosBanc.Name = "ButtonExtractosBanc"
        Me.ButtonExtractosBanc.Size = New System.Drawing.Size(124, 30)
        Me.ButtonExtractosBanc.TabIndex = 48
        Me.ButtonExtractosBanc.Text = "Subir Extractos Banc"
        Me.ButtonExtractosBanc.UseVisualStyleBackColor = True
        '
        'ButtonUpdateUbic
        '
        Me.ButtonUpdateUbic.Location = New System.Drawing.Point(714, 440)
        Me.ButtonUpdateUbic.Name = "ButtonUpdateUbic"
        Me.ButtonUpdateUbic.Size = New System.Drawing.Size(171, 25)
        Me.ButtonUpdateUbic.TabIndex = 49
        Me.ButtonUpdateUbic.Text = "Actualizar Ubicaciones"
        Me.ButtonUpdateUbic.UseVisualStyleBackColor = True
        '
        'TimerNotasCredito
        '
        Me.TimerNotasCredito.Interval = 14000
        '
        'TimerSyncBancExtract
        '
        Me.TimerSyncBancExtract.Interval = 18000
        '
        'ButtonCall
        '
        Me.ButtonCall.Location = New System.Drawing.Point(34, 426)
        Me.ButtonCall.Name = "ButtonCall"
        Me.ButtonCall.Size = New System.Drawing.Size(124, 28)
        Me.ButtonCall.TabIndex = 50
        Me.ButtonCall.Text = "CallFN"
        Me.ButtonCall.UseVisualStyleBackColor = True
        '
        'TextBoxLimite
        '
        Me.TextBoxLimite.Location = New System.Drawing.Point(714, 70)
        Me.TextBoxLimite.Name = "TextBoxLimite"
        Me.TextBoxLimite.Size = New System.Drawing.Size(90, 20)
        Me.TextBoxLimite.TabIndex = 51
        Me.TextBoxLimite.Text = "700"
        '
        'ButtonFraccionamientos
        '
        Me.ButtonFraccionamientos.Enabled = False
        Me.ButtonFraccionamientos.Location = New System.Drawing.Point(330, 351)
        Me.ButtonFraccionamientos.Name = "ButtonFraccionamientos"
        Me.ButtonFraccionamientos.Size = New System.Drawing.Size(135, 28)
        Me.ButtonFraccionamientos.TabIndex = 52
        Me.ButtonFraccionamientos.Text = "Fraccionamientos (+)"
        Me.ButtonFraccionamientos.UseVisualStyleBackColor = True
        '
        'TimerFracPos
        '
        Me.TimerFracPos.Interval = 2000
        '
        'ButtonFraccNegativo
        '
        Me.ButtonFraccNegativo.Enabled = False
        Me.ButtonFraccNegativo.Location = New System.Drawing.Point(170, 351)
        Me.ButtonFraccNegativo.Name = "ButtonFraccNegativo"
        Me.ButtonFraccNegativo.Size = New System.Drawing.Size(154, 28)
        Me.ButtonFraccNegativo.TabIndex = 53
        Me.ButtonFraccNegativo.Text = "Fraccionamientos (-)"
        Me.ButtonFraccNegativo.UseVisualStyleBackColor = True
        '
        'TimerFracNeg
        '
        Me.TimerFracNeg.Interval = 2000
        '
        'UpdateCatFactura
        '
        Me.UpdateCatFactura.Location = New System.Drawing.Point(710, 613)
        Me.UpdateCatFactura.Name = "UpdateCatFactura"
        Me.UpdateCatFactura.Size = New System.Drawing.Size(171, 28)
        Me.UpdateCatFactura.TabIndex = 54
        Me.UpdateCatFactura.Text = "Update Folio Facturas"
        Me.UpdateCatFactura.UseVisualStyleBackColor = True
        '
        'ButtonCancelPagos
        '
        Me.ButtonCancelPagos.Enabled = False
        Me.ButtonCancelPagos.Location = New System.Drawing.Point(34, 385)
        Me.ButtonCancelPagos.Name = "ButtonCancelPagos"
        Me.ButtonCancelPagos.Size = New System.Drawing.Size(124, 28)
        Me.ButtonCancelPagos.TabIndex = 55
        Me.ButtonCancelPagos.Text = "Cancelar Pagos"
        Me.ButtonCancelPagos.UseVisualStyleBackColor = True
        '
        'TimerCancelPagos
        '
        Me.TimerCancelPagos.Interval = 36000
        '
        'TextBoxDocEntry
        '
        Me.TextBoxDocEntry.Location = New System.Drawing.Point(772, 512)
        Me.TextBoxDocEntry.Name = "TextBoxDocEntry"
        Me.TextBoxDocEntry.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxDocEntry.TabIndex = 56
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(712, 515)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(52, 13)
        Me.Label9.TabIndex = 57
        Me.Label9.Text = "DocNum:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(712, 563)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(32, 13)
        Me.Label10.TabIndex = 58
        Me.Label10.Text = "Folio:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(712, 537)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(23, 13)
        Me.Label11.TabIndex = 59
        Me.Label11.Text = "PF:"
        '
        'TextBoxPrefix
        '
        Me.TextBoxPrefix.Location = New System.Drawing.Point(772, 537)
        Me.TextBoxPrefix.Name = "TextBoxPrefix"
        Me.TextBoxPrefix.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxPrefix.TabIndex = 60
        Me.TextBoxPrefix.Text = "FV"
        '
        'TextBoxFolio
        '
        Me.TextBoxFolio.Location = New System.Drawing.Point(772, 560)
        Me.TextBoxFolio.Name = "TextBoxFolio"
        Me.TextBoxFolio.Size = New System.Drawing.Size(109, 20)
        Me.TextBoxFolio.TabIndex = 61
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(713, 594)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(31, 13)
        Me.Label12.TabIndex = 62
        Me.Label12.Text = "Tipo:"
        '
        'ComboBoxTipo
        '
        Me.ComboBoxTipo.FormattingEnabled = True
        Me.ComboBoxTipo.Items.AddRange(New Object() {"No Modificar", "Contado", "Credito"})
        Me.ComboBoxTipo.Location = New System.Drawing.Point(772, 586)
        Me.ComboBoxTipo.Name = "ComboBoxTipo"
        Me.ComboBoxTipo.Size = New System.Drawing.Size(109, 21)
        Me.ComboBoxTipo.TabIndex = 63
        Me.ComboBoxTipo.Text = "No Modificar"
        '
        'SAPSyncForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(929, 742)
        Me.Controls.Add(Me.ComboBoxTipo)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.TextBoxFolio)
        Me.Controls.Add(Me.TextBoxPrefix)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.TextBoxDocEntry)
        Me.Controls.Add(Me.ButtonCancelPagos)
        Me.Controls.Add(Me.UpdateCatFactura)
        Me.Controls.Add(Me.ButtonFraccNegativo)
        Me.Controls.Add(Me.ButtonFraccionamientos)
        Me.Controls.Add(Me.TextBoxLimite)
        Me.Controls.Add(Me.ButtonCall)
        Me.Controls.Add(Me.ButtonUpdateUbic)
        Me.Controls.Add(Me.ButtonExtractosBanc)
        Me.Controls.Add(Me.ButtonUpdateCliData)
        Me.Controls.Add(Me.ButtonCheckSapCodes)
        Me.Controls.Add(Me.counter)
        Me.Controls.Add(Me.CheckBoxAutoConnect)
        Me.Controls.Add(Me.sap_user)
        Me.Controls.Add(Me.ButtonMigrarProv)
        Me.Controls.Add(Me.ButtonDeleteClienes)
        Me.Controls.Add(Me.ButtonUserTables)
        Me.Controls.Add(Me.company_db)
        Me.Controls.Add(Me.ButtonUpdatePriceList)
        Me.Controls.Add(Me.ButtonDisconect)
        Me.Controls.Add(Me.ButtonAsientos)
        Me.Controls.Add(Me.ButtonSincClientes)
        Me.Controls.Add(Me.ButtonCobroCuotas)
        Me.Controls.Add(Me.ButtonMigrarArticulos)
        Me.Controls.Add(Me.ButtonMigrarLotes)
        Me.Controls.Add(Me.ButtonMigrarClientes)
        Me.Controls.Add(Me.ButtonSincCompras)
        Me.Controls.Add(Me.sap_passw)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.ButtonAjustesNeg)
        Me.Controls.Add(Me.ButtonRemisiones)
        Me.Controls.Add(Me.ButtonAjustes)
        Me.Controls.Add(Me.ButtonUpdateBatchNumber)
        Me.Controls.Add(Me.ButtonReservas)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ButtonNotaCredito)
        Me.Controls.Add(Me.ButtonSincPagos)
        Me.Controls.Add(Me.ms_passw)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.ms_user)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.ButtonStockXVentas)
        Me.Controls.Add(Me.passw)
        Me.Controls.Add(Me.user)
        Me.Controls.Add(Me.server)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ButtonConnect)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "SAPSyncForm"
        Me.Text = "SAPSyncForm"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ButtonConnect As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents server As System.Windows.Forms.TextBox
    Friend WithEvents user As System.Windows.Forms.TextBox
    Friend WithEvents passw As System.Windows.Forms.TextBox
    Friend WithEvents ButtonStockXVentas As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ms_user As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ms_passw As System.Windows.Forms.TextBox
    Friend WithEvents ButtonSincPagos As System.Windows.Forms.Button
    Friend WithEvents ButtonNotaCredito As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents logTextArea As System.Windows.Forms.RichTextBox
    Friend WithEvents ButtonReservas As System.Windows.Forms.Button
    Friend WithEvents TimerStockXVentas As System.Windows.Forms.Timer
    Friend WithEvents ButtonUpdateBatchNumber As System.Windows.Forms.Button
    Friend WithEvents ButtonAjustes As System.Windows.Forms.Button
    Friend WithEvents ButtonRemisiones As System.Windows.Forms.Button
    Friend WithEvents ButtonAjustesNeg As System.Windows.Forms.Button
    Friend WithEvents label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents sap_passw As System.Windows.Forms.TextBox
    Friend WithEvents TimerAjustesPos As System.Windows.Forms.Timer
    Friend WithEvents TimerAjustesNeg As System.Windows.Forms.Timer
    Friend WithEvents ButtonSincCompras As System.Windows.Forms.Button
    Friend WithEvents TimerCotiz As System.Windows.Forms.Timer
    Friend WithEvents TimerCompras As System.Windows.Forms.Timer
    Friend WithEvents ButtonMigrarClientes As System.Windows.Forms.Button
    Friend WithEvents ButtonMigrarLotes As System.Windows.Forms.Button
    Friend WithEvents ButtonMigrarArticulos As System.Windows.Forms.Button
    Friend WithEvents ButtonCobroCuotas As System.Windows.Forms.Button
    Friend WithEvents ButtonSincClientes As System.Windows.Forms.Button
    Friend WithEvents TimerSincClientes As System.Windows.Forms.Timer
    Friend WithEvents TimerReservas As System.Windows.Forms.Timer
    Friend WithEvents TimerRemisiones As System.Windows.Forms.Timer
    Friend WithEvents TimerCobroCuotas As System.Windows.Forms.Timer
    Friend WithEvents ButtonAsientos As System.Windows.Forms.Button
    Friend WithEvents TimerAsientos As System.Windows.Forms.Timer
    Friend WithEvents ButtonDisconect As System.Windows.Forms.Button
    Friend WithEvents ButtonUpdatePriceList As System.Windows.Forms.Button
    Friend WithEvents company_db As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonUserTables As System.Windows.Forms.Button
    Friend WithEvents ButtonDeleteClienes As System.Windows.Forms.Button
    Friend WithEvents TimerUptateLotes As System.Windows.Forms.Timer
    Friend WithEvents TimerPagos As System.Windows.Forms.Timer
    Friend WithEvents ButtonMigrarProv As System.Windows.Forms.Button
    Friend WithEvents sap_user As System.Windows.Forms.ComboBox
    Friend WithEvents CheckBoxAutoConnect As System.Windows.Forms.CheckBox
    Friend WithEvents counter As System.Windows.Forms.Label
    Friend WithEvents TimerConnect As System.Windows.Forms.Timer
    Friend WithEvents ButtonCheckSapCodes As System.Windows.Forms.Button
    Friend WithEvents ButtonUpdateCliData As System.Windows.Forms.Button
    Friend WithEvents ButtonExtractosBanc As System.Windows.Forms.Button
    Friend WithEvents ButtonUpdateUbic As System.Windows.Forms.Button
    Friend WithEvents TimerNotasCredito As System.Windows.Forms.Timer
    Friend WithEvents TimerSyncBancExtract As System.Windows.Forms.Timer
    Friend WithEvents ButtonCall As System.Windows.Forms.Button
    Friend WithEvents TextBoxLimite As System.Windows.Forms.TextBox
    Friend WithEvents ButtonFraccionamientos As System.Windows.Forms.Button
    Friend WithEvents TimerFracPos As System.Windows.Forms.Timer
    Friend WithEvents ButtonFraccNegativo As System.Windows.Forms.Button
    Friend WithEvents TimerFracNeg As System.Windows.Forms.Timer
    Friend WithEvents UpdateCatFactura As System.Windows.Forms.Button
    Friend WithEvents ButtonCancelPagos As System.Windows.Forms.Button
    Friend WithEvents TimerCancelPagos As System.Windows.Forms.Timer
    Friend WithEvents TextBoxDocEntry As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TextBoxPrefix As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxFolio As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxTipo As System.Windows.Forms.ComboBox

End Class
