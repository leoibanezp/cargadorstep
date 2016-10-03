Imports System.Configuration

Imports System.Xml
Imports CargadorStepForm.GestionTableAdapters
Imports CargadorStepForm.AMFotosTableAdapters
Imports CargadorStepForm.DTE3TableAdapters
Imports CargadorStepForm.Utilities.FTP
Imports System.IO

Public Class Form1

    Private Sub btn_cargarTablas_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_cargarTablas.Click
        'Credenciales FTP
        Dim HostFTP As String = ConfigurationManager.AppSettings("FTP_HOST")
        Dim UsrFTP As String = ConfigurationManager.AppSettings("FTP_USR")
        Dim ClaveUSRFTP As String = ConfigurationManager.AppSettings("FTP_PSW")

        'Path Productos
        Dim CarpetaXMLProductosFull As String = ConfigurationManager.AppSettings("DirXMLProductoFTP_Full")
        Dim CarpetaXMLProductosDelta As String = ConfigurationManager.AppSettings("DirXMLProductoFTP_Delta")
        Dim CarpetaXMLProductosCargadosOK As String = System.Configuration.ConfigurationManager.AppSettings("DirXMLImgProductoFTPCargadosOK")
        Dim CarpetaXMLProductosCargadosConErrores As String = System.Configuration.ConfigurationManager.AppSettings("DirXMLImgProductoFTPCargadosConErrores")

        'Path Proformas
        Dim CarpetaXMLProformaFull As String = ConfigurationManager.AppSettings("DirXMLProforma_Full")
        Dim CarpetaXMLProformaDelta As String = ConfigurationManager.AppSettings("DirXMLProforma")
        Dim CarpetaXMLProformaCargadosOK As String = System.Configuration.ConfigurationManager.AppSettings("DirXMLProforma_CargadosOK")
        Dim CarpetaXMLProformaCargadosConErrores As String = System.Configuration.ConfigurationManager.AppSettings("DirXMLProforma_CargadosConErrores")

        'Path Imagenes
        Dim CarpetaIMGProducto As String = ConfigurationManager.AppSettings("DirImgProductoFTP")
        Dim DirXMLImgProductoFTP_Delta As String = ConfigurationManager.AppSettings("DirXMLImgProductoFTP_Delta")
        Dim DirXMLImgProductoFTP_Full As String = ConfigurationManager.AppSettings("DirXMLImgProductoFTP_Full")
        Dim DirXMLImgProductoFTPCargadosOK As String = ConfigurationManager.AppSettings("DirXMLImgProductoFTPCargadosOK")
        Dim DirXMLImgProductoFTPCargadosConErrores As String = ConfigurationManager.AppSettings("DirXMLImgProductoFTPCargadosConErrores")

        'Path Aplication Records
        Dim CarpetaXMLACES_Full As String = ConfigurationManager.AppSettings("DirXMLACES_Full")
        Dim CarpetaXMLACES_Delta As String = ConfigurationManager.AppSettings("DirXMLACES_Delta")
        Dim CarpetaXMLACES_CargadosOK As String = ConfigurationManager.AppSettings("DirXMLACES_CargadosOK")
        Dim CarpetaXMLACES_CargadosConErrores As String = ConfigurationManager.AppSettings("DirXMLACES_CargadosConErrores")


        'Carga Detalle Proforma
        '   Carga Full
        CargarProforma(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLProformaFull, CarpetaXMLProformaCargadosOK, CarpetaXMLProformaCargadosConErrores, "Full")

        '   Carga "Delta"
        CargarProforma(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLProformaDelta, CarpetaXMLProformaCargadosOK, CarpetaXMLProformaCargadosConErrores, "Delta")

        'Productos
        '   Carga Full
        CargarProductos(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLProductosFull, CarpetaXMLProductosCargadosOK, CarpetaXMLProductosCargadosConErrores, "Full")
        '   Carga Delta 
        CargarProductos(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLProductosDelta, CarpetaXMLProductosCargadosOK, CarpetaXMLProductosCargadosConErrores, "Delta")


        'XML Imágenes
        '   Carga Full
        CargarIMGsTrabajosYProductos(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaIMGProducto, DirXMLImgProductoFTP_Full, DirXMLImgProductoFTPCargadosOK, DirXMLImgProductoFTPCargadosConErrores, "Full")
        '   Carga Delta
        CargarIMGsTrabajosYProductos(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaIMGProducto, DirXMLImgProductoFTP_Delta, DirXMLImgProductoFTPCargadosOK, DirXMLImgProductoFTPCargadosConErrores, "Delta")


        'Aplication Records
        '   Carga Full
        CargarStepCatalogo(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLACES_Full, CarpetaXMLACES_CargadosOK, CarpetaXMLACES_CargadosConErrores, "Full")
        '   Carga Delta
        CargarStepCatalogo(HostFTP, UsrFTP, ClaveUSRFTP, CarpetaXMLACES_Delta, CarpetaXMLACES_CargadosOK, CarpetaXMLACES_CargadosConErrores, "Delta")


    End Sub


    Public Sub CargarProductos(ByVal HostFTP As String, ByVal UsrFTP As String, ByVal ClaveUSRFTP As String, ByVal CarpetaXMLProductos As String, ByVal CarpetaXMLProductosCargadosOK As String, ByVal CarpetaXMLProductosCargadosConErrores As String, ByVal TipoCarga2 As String)
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter
        Dim Msg As String = "", TipoCarga As String = ""
        Dim UsuarioCarga As String = "TestAdmin"

        Dim strSQLFields As String
        Dim strSQLValues As String
        Dim strSQL As String
        Dim spInsertaDinamico As sp_InsertaMP_cargadorDinamicoTableAdapter = New sp_InsertaMP_cargadorDinamicoTableAdapter
        Dim adMPCargador As MaestroProducto_cargadorTableAdapter = New MaestroProducto_cargadorTableAdapter
        Dim adPaisOrigen As maestroProducto_paisOrigenTableAdapter = New maestroProducto_paisOrigenTableAdapter
        Dim adPosProductoAgrup As vw_pos_producto_agrupacionTableAdapter = New vw_pos_producto_agrupacionTableAdapter
        Dim advwMaestroProducto As vw_maestroProductoTableAdapter = New vw_maestroProductoTableAdapter
        Dim fila As Integer = 0
        Dim strCodCategoria As String, strCodSubcategoria As String, strDescPaisOrigen As String

        Dim m_xmld As XmlDocument
        Dim m_nodelist As XmlNodeList
        Dim m_node As XmlElement, m_node2 As XmlElement

        m_xmld = New XmlDocument()

        '----
        Try

            If CarpetaXMLProductos <> "" Then

                Dim ftp As New FTPclient(HostFTP, UsrFTP, ClaveUSRFTP)                      'Conectar con FTP
                Dim dirList As FTPdirectory = ftp.ListDirectoryDetail(CarpetaXMLProductos)  'Enlista la carpeta principal
                Dim filesOnly As FTPdirectory = dirList.GetFiles()                          'Enlista solo los archivos del directorio

                For Each file As FTPfileInfo In filesOnly                                   'Recorrer cada archivo en el directorio
                    Try
                        If file.Extension = "xml" Then
                            Dim streamReader2 As StreamReader
                            streamReader2 = ftp.Download2(file.FullName)

                            m_xmld.Load(streamReader2)
                            m_nodelist = m_xmld.SelectNodes("STEP-ProductInformation/Products/Product")


                            If fila = 0 Then adMPCargador.EliminaPorLogin(UsuarioCarga)


                            If (m_nodelist Is Nothing) = False Then


                                For Each m_node In m_nodelist
                                    strSQLFields = "INSERT INTO MaestroProducto_cargador ("
                                    strSQLValues = "SELECT "

                                    TipoCarga = ""

                                    Dim IDSKU As String = m_node.Attributes.GetNamedItem("ID").Value.ToString()
                                    Dim SKU As String = ""
                                    Dim ValidaSKU As String = ""

                                    Select Case m_node.Attributes.GetNamedItem("UserTypeID").Value.ToString()
                                        Case "SubCategoria"
                                            m_node = m_node.Item("Product")
                                        Case "Categoria"
                                            m_node = m_node.Item("Product").Item("Product")
                                        Case "SubAgrupacion"
                                            m_node = m_node.Item("Product").Item("Product").Item("Product")
                                    End Select


                                    For Each m_node2 In m_node.Item("Values")
                                        If (m_node2.HasAttribute("Changed") = True) And (TipoCarga = "") Then TipoCarga = "Update"

                                        Dim varValor As String

                                        If (m_node2.LocalName = "Value") Then
                                            varValor = m_node2.InnerText
                                        Else ' es "Multivalue"
                                            If (m_node2.InnerXml.Contains("ID=") = True) Then                   'determina si tag posee Atributo "ID"
                                                varValor = m_node2.FirstChild.Attributes("ID").Value.ToString
                                            Else
                                                varValor = m_node2.InnerText
                                            End If
                                        End If

                                        Select Case (m_node2.Attributes.GetNamedItem("AttributeID").Value.ToString)
                                            Case "AP_SKU"
                                                strSQLFields = strSQLFields & "SKU,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                                SKU = varValor

                                                ValidaSKU = advwMaestroProducto.ValidaExistenciaSKU1(SKU)
                                            Case "SAP_MAKTX" '"A"
                                                strSQLFields = strSQLFields & "descripcionInterna,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_DESCRIPCION VENTA" '"B"
                                                strSQLFields = strSQLFields & "descripcionVenta,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_TIPO PRODUCTO" '"C"
                                                strSQLFields = strSQLFields & "tipoProducto,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_ESTADO" '"D"
                                                strSQLFields = strSQLFields & "estado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_COD.FAMILIA" '"E"
                                                strSQLFields = strSQLFields & "codFamilia,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "DER_CAT"  '"F"                                
                                                strCodCategoria = adPosProductoAgrup.GetCodCategoria(Trim(varValor))
                                                If strCodCategoria = "" Then strCodCategoria = "0000"

                                                strSQLFields = strSQLFields & "codCategoria,"
                                                strSQLValues = strSQLValues & "'" & strCodCategoria & "',"
                                            Case "DER_SUBCAT"  '"G"
                                                strCodSubcategoria = adPosProductoAgrup.GetCodSubcategoria(Trim(varValor))
                                                If strCodSubcategoria = "" Then strCodSubcategoria = "0000"

                                                strSQLFields = strSQLFields & "codSubCategoria,"
                                                strSQLValues = strSQLValues & "'" & strCodSubcategoria & "',"
                                            Case "AP_COD.RELACIONADO" '"H"
                                                strSQLFields = strSQLFields & "codRelacionado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD" '"I"
                                                strSQLFields = strSQLFields & "unidad,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD MEDIDA" '"J"
                                                strSQLFields = strSQLFields & "unidadMedida,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD MEDIDA CONV" '"K"
                                                strSQLFields = strSQLFields & "unidadMedidaConv,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_UNIDAD COMPRA" '"L"
                                                strSQLFields = strSQLFields & "unidadCompra,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD COMPRA CONV" '"M"
                                                strSQLFields = strSQLFields & "unidadCompraConv,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_UNIDAD LOGISTICA" '"N"
                                                strSQLFields = strSQLFields & "unidadLogistica,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD LOGISTICA CONV" '"O"
                                                strSQLFields = strSQLFields & "unidadLogisticaConv,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_UNIDAD ESTADISTICA" '"P"
                                                strSQLFields = strSQLFields & "unidadEstadistica,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_UNIDAD ESTADISTICA CONV" '"Q"
                                                strSQLFields = strSQLFields & "unidadEstadisticaConv,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "SAP_EANNR_1"  '"R"
                                                strSQLFields = strSQLFields & "codigoBarra,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_COD.COMPRA" '"S"
                                                strSQLFields = strSQLFields & "codigoCompra,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_OVLPN" '"T"
                                                strSQLFields = strSQLFields & "codigoFabricante,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_COD.AUTOPLANET" '"U"
                                                strSQLFields = strSQLFields & "codigoAutoplanet,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_DESCRIPCION INGLES" '"V"
                                                strSQLFields = strSQLFields & "descIngles,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_TIPO.MATERIAL"  '"W"
                                                strSQLFields = strSQLFields & "tipoMaterial,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "DER_SUBAGRU"  '"X"
                                                strSQLFields = strSQLFields & "subagrupacion,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_HERKL" '"Y"   'ellos envian codigoSAP
                                                strDescPaisOrigen = Trim(adPaisOrigen.GetDescPaisOrigen(Trim(varValor)))
                                                If strDescPaisOrigen = "" Then strDescPaisOrigen = Trim(varValor)

                                                strSQLFields = strSQLFields & "paisOrigen,"
                                                strSQLValues = strSQLValues & "'" & strDescPaisOrigen & "',"
                                            Case "AP_TIPO.CODIGO" '"Z"
                                                strSQLFields = strSQLFields & "tipoCodigo,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_LISTA PRECIO NORMAL" '"A1"
                                                strSQLFields = strSQLFields & "listaPrecioNormal,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_LISTA PRECIO PERSONAL" '"B1"
                                                strSQLFields = strSQLFields & "listaPrecioPersonal,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_LISTA PRECIO 06" '"C1"
                                                strSQLFields = strSQLFields & "listaPrecio06,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_PRECIO MERMA" '"D1"
                                                strSQLFields = strSQLFields & "precioMerma,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_PRECIO DEVOLUCION" '"E1"
                                                strSQLFields = strSQLFields & "precioDevolucion,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_IVA"   '"F1"
                                                strSQLFields = strSQLFields & "IVA,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ",", ".") & ","
                                            Case "AP_STOCK SEGURIDAD" '"G1"
                                                strSQLFields = strSQLFields & "stockSeguridad,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ",", ".") & ","
                                            Case "AP_ESTACIONALIDAD" '"H1"
                                                strSQLFields = strSQLFields & "estacionalidad,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_SOBRESTOCK" '"I1"
                                                strSQLFields = strSQLFields & "sobreStock,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MIN.PLANOGRAMA" '"J1"
                                                strSQLFields = strSQLFields & "minPlanograma,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MAX.PLANOGRAMA" '"K1"
                                                strSQLFields = strSQLFields & "MaxPlanograma,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_TAMANO ESTUCHE" '"L1"
                                                strSQLFields = strSQLFields & "tamanoEstuche,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_TAMANO PACK" '"M1"
                                                strSQLFields = strSQLFields & "tamanoPack,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "SAP_MFRPN"    '"N1"
                                                strSQLFields = strSQLFields & "impRetail,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_CANT.POR AUTO" '"O1"
                                                strSQLFields = strSQLFields & "cantPorAuto,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_ORIG.REABAS"  '"P1"
                                                strSQLFields = strSQLFields & "origReabas,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_GEWEI"  '"Q1"
                                                strSQLFields = strSQLFields & "peso,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_EXTWG"  '"R1"
                                                strSQLFields = strSQLFields & "fabricante,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_RESPONSABLE"  '"S1"
                                                strSQLFields = strSQLFields & "responsable,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_EXTENSION"  '"T1"
                                                strSQLFields = strSQLFields & "extension,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_RUT POR DEFECTO" '"U1"
                                                strSQLFields = strSQLFields & "rutPorDefecto,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_RUT 1" '"V1"
                                                strSQLFields = strSQLFields & "rut1,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_COSTO 1" '"W1"
                                                strSQLFields = strSQLFields & "costo1,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_COD.COMPRA 1" '"X1"
                                                strSQLFields = strSQLFields & "CodCompra1,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_MOQ 1" '"Y1"
                                                strSQLFields = strSQLFields & "MOQ1,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_RUT 2"  '"Z1"
                                                strSQLFields = strSQLFields & "rut2,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_COSTO 2" '"A2"
                                                strSQLFields = strSQLFields & "costo2,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_COD.COMPRA 2"  '"B2"
                                                strSQLFields = strSQLFields & "codCompra2,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MOQ 2"  '"C2"
                                                strSQLFields = strSQLFields & "MOQ2,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_RUT 3"  '"D2"
                                                strSQLFields = strSQLFields & "rut3,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_COSTO 3"  '"E2"
                                                strSQLFields = strSQLFields & "costo3,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_COD.COMPRA 3"  '"F2"
                                                strSQLFields = strSQLFields & "codCompra3,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MOQ 3"  '"G2"
                                                strSQLFields = strSQLFields & "MOQ3,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_RUT 4"  '"H2"
                                                strSQLFields = strSQLFields & "rut4,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_COSTO 4" '"I2"
                                                strSQLFields = strSQLFields & "Costo4,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_COD.COMPRA 4"  '"J2"
                                                strSQLFields = strSQLFields & "codCompra4,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MOQ 4"  '"K2"
                                                strSQLFields = strSQLFields & "MOQ4,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_RUT 5"  '"L2"
                                                strSQLFields = strSQLFields & "rut5,"
                                                strSQLValues = strSQLValues & "'" & Replace(varValor, ".", "") & "',"
                                            Case "AP_COSTO 5"  '"M2"
                                                strSQLFields = strSQLFields & "costo5,"
                                                strSQLValues = strSQLValues & "" & Replace(varValor, ".", "") & ","
                                            Case "AP_COD.COMPRA 5"  '"N2"
                                                strSQLFields = strSQLFields & "codCompra5,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_MOQ 5"  '"O2"
                                                strSQLFields = strSQLFields & "MOQ5,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_ALARMADO"  '"P2"
                                                strSQLFields = strSQLFields & "alarmado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_ETIQUETADO"  '"Q2"
                                                strSQLFields = strSQLFields & "etiquetado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_TRADUCCION"  '"R2"
                                                strSQLFields = strSQLFields & "traduccion,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_EMBOLSADO"  '"S2"
                                                strSQLFields = strSQLFields & "embolsado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "AP_ENZUNCHADO"  '"T2"
                                                strSQLFields = strSQLFields & "enzunchado,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_LAENG"  '"U2"
                                                strSQLFields = strSQLFields & "largo,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_BREIT"  '"V2"
                                                strSQLFields = strSQLFields & "ancho,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_HOEHE"  '"W2"
                                                strSQLFields = strSQLFields & "alto,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_EAN11_2" '"X2"
                                                strSQLFields = strSQLFields & "codigoBarraAlt1,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_EAN11_3" '"Y2"
                                                strSQLFields = strSQLFields & "codigoBarraAlt2,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_EAN11_4" '"Z2"
                                                strSQLFields = strSQLFields & "codigoBarraAlt3,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"
                                            Case "SAP_EAN11_5"  '"A3"
                                                strSQLFields = strSQLFields & "codigoBarraAlt4,"
                                                strSQLValues = strSQLValues & "'" & varValor & "',"

                                            Case "AP_STOCK MIN. V. MACK." '"D3"
                                                strSQLFields = strSQLFields & "stockMin1,"
                                                strSQLValues = strSQLValues & "" & varValor & ","

                                                'aprovecha a indicar valores para resto de tiendas no consideradas en xml
                                                strSQLFields = strSQLFields & "stockMin10,stockMin11,stockMin12,"
                                                strSQLValues = strSQLValues & "" & varValor & "," & varValor & "," & varValor & ","

                                            Case "AP_STOCK MAX. V. MACK." '"E3"
                                                strSQLFields = strSQLFields & "stockMax1,"
                                                strSQLValues = strSQLValues & "" & varValor & ","

                                                'aprovecha a indicar valores para resto de tiendas no consideradas en xml
                                                strSQLFields = strSQLFields & "stockMax10,stockMax11,stockMax12,"
                                                strSQLValues = strSQLValues & "" & varValor & "," & varValor & "," & varValor & ","

                                            Case "AP_STOCK MIN. MAIPU" '"F3"
                                                strSQLFields = strSQLFields & "stockMin2,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. MAIPU" '"G3"
                                                strSQLFields = strSQLFields & "stockMax2,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. LFU" '"H3"
                                                strSQLFields = strSQLFields & "stockMin3,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. LFU" '"I3"
                                                strSQLFields = strSQLFields & "stockMax3,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. PENALOLEN" '"J3"
                                                strSQLFields = strSQLFields & "stockMin4,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. PENALOLEN" '"K3"
                                                strSQLFields = strSQLFields & "stockMax4,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. PTE ALTO"  '"L3"
                                                strSQLFields = strSQLFields & "stockMin5,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. PTE ALTO"  '"M3"
                                                strSQLFields = strSQLFields & "stockMax5,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. QUILICURA" '"N3"
                                                strSQLFields = strSQLFields & "stockMin6,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. QUILICURA" '"O3"
                                                strSQLFields = strSQLFields & "stockMax6,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. GRAN AVENIDA" '"P3"
                                                strSQLFields = strSQLFields & "stockMin7,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. GRAN AVENIDA" '"Q3"
                                                strSQLFields = strSQLFields & "stockMax7,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. LO BLANCO" '"R3"
                                                strSQLFields = strSQLFields & "stockMin8,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. LO BLANCO" '"S3"
                                                strSQLFields = strSQLFields & "stockMax8,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MIN. MAIPU ALVI" '"T3"
                                                strSQLFields = strSQLFields & "stockMin9,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                            Case "AP_STOCK MAX. MAIPU ALVI" '"U3"
                                                strSQLFields = strSQLFields & "stockMax9,"
                                                strSQLValues = strSQLValues & "" & varValor & ","
                                        End Select
                                    Next
                                    fila = fila + 1
                                    strSQLFields = Microsoft.VisualBasic.Left(strSQLFields, Microsoft.VisualBasic.Len(strSQLFields) - 1) & " , login, fila) "
                                    strSQLValues = Microsoft.VisualBasic.Left(strSQLValues, Microsoft.VisualBasic.Len(strSQLValues) - 1) & " ,'" & UsuarioCarga & "' ," & CStr(fila) & ""

                                    strSQL = Trim(strSQLFields & " " & strSQLValues)

                                    'Exporta datos a tabla MaestroProducto_cargador
                                    spInsertaDinamico.sp_InsertaMP_cargadorDinamico(strSQL)

                                    Dim adStep_SKU As STEP_SKUTableAdapter = New STEP_SKUTableAdapter

                                    'Insertar ID y SKU asociado
                                    adStep_SKU.EliminarSiExiste(IDSKU)
                                    adStep_SKU.Insert(IDSKU, SKU)

                                    'Si no existe SKU, se valida para crearlo
                                    If ValidaSKU = "" Then TipoCarga = "Insert"

                                    'Valida si corresponde Update o Insert según TAG 'Changed' + Log
                                    If TipoCarga = "Update" Then
                                        Dim adActualizaMP As sp_actualiza_MaestroProducto_STEPTableAdapter = New sp_actualiza_MaestroProducto_STEPTableAdapter
                                        adActualizaMP.sp_actualiza_MaestroProducto_STEP(UsuarioCarga, fila)
                                    Else
                                        Dim adCargaMP As sp_carga_MaestroProducto_STEPTableAdapter = New sp_carga_MaestroProducto_STEPTableAdapter
                                        adCargaMP.sp_carga_MaestroProducto_STEP(UsuarioCarga, fila)
                                    End If
                                Next
                                'Mueve archivo de carga
                                '''''''''''''''''ftp.FtpRename(file.FullName, CarpetaXMLProductosCargadosOK & file.NameOnly & ".xml")

                                streamReader2.Close()
                            Else
                                '''''''''''''''''ftp.FtpRename(file.FullName, CarpetaXMLProductosCargadosConErrores & file.NameOnly & ".xml")
                            End If

                        End If

                    Catch ex As Exception
                        Select Case ex.Message
                            Case "Referencia a objeto no establecida como instancia de un objeto."
                                Msg = "Revise los datos del producto."
                            Case Else
                                Msg = ex.Message
                        End Select
                        adLog.Insert(Date.Now, "MaestroProducto", 0, "Error en el archivo: " & file.Filename & " " & Msg)
                    End Try
                Next
                If Msg.ToString <> "" Then
                    adLog.Insert(Date.Now, "MaestroProducto", 0, "Proceso Carga finalizado con problemas")
                Else
                    adLog.Insert(Date.Now, "MaestroProducto", 0, "Proceso Carga de MaestroProducto finalizado.")
                End If

            End If

        Catch ex As Exception
            adLog.Insert(Date.Now, "MaestroProducto", 0, "Error en la lectura del archivo XML : " & ex.Message)
        End Try
        '----

    End Sub

    Public Sub CargarIMGsTrabajosYProductos(ByVal HostFTP As String, ByVal UsrFTP As String, ByVal ClaveUSRFTP As String, ByVal CarpetaIMGProducto As String, ByVal CarpetaXMLIMGProducto As String, ByVal CarpetaDestinoXMLimg As String, ByVal DirXMLImgProductoFTPCargadosConErrores As String, ByVal TipoCarga As String)

        Dim Msg As String = ""

        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        Try

            If CarpetaIMGProducto <> "" Then

                'Conectar con FTP
                Dim ftp As New FTPclient(HostFTP, UsrFTP, ClaveUSRFTP)

                'Enlista la carpeta principal, enlista los XML
                Dim dirList As FTPdirectory = ftp.ListDirectoryDetail(CarpetaXMLIMGProducto)

                'Enlista solo los archivos del directorio (xml)
                Dim filesOnly As FTPdirectory = dirList.GetFiles()

                'Recorrer cada archivo
                For Each file As FTPfileInfo In filesOnly

                    If file.Extension = "xml" Then

                        Dim ArchivoXML As XmlDocument
                        ArchivoXML = New XmlDocument()
                        Dim ListaNodosXML As XmlNodeList
                        Dim TipoAssetNodo As XmlNode
                        Dim TipoAssetAtributo As String
                        Dim ArchivoStreamReader As StreamReader
                        ArchivoStreamReader = ftp.Download2(file.FullName)

                        ArchivoXML.Load(ArchivoStreamReader)

                        ListaNodosXML = ArchivoXML.SelectNodes("STEP-ProductInformation/Assets/Asset")
                        TipoAssetNodo = ArchivoXML.SelectSingleNode("STEP-ProductInformation/Assets")

                        Dim NombreRepuesto As String = ""
                        Dim UbicacionIMG As String = ""

                        TipoAssetAtributo = ""

                        If (TipoAssetNodo Is Nothing) = False Then

                            For Each AssetNodo In TipoAssetNodo
                                TipoAssetAtributo = AssetNodo.Attributes.GetNamedItem("UserTypeID").Value
                            Next


                            If (TipoAssetAtributo = "ProductImage") Then

                                If (ListaNodosXML.Item(0).OuterXml.Contains("AssetPushLocation") = True) Then

                                    Dim NombreIMG As String

                                    For Each nodoXML In ListaNodosXML

                                        NombreIMG = nodoXML.Item("Name").InnerText

                                        For Each SubNodo In nodoXML

                                            If SubNodo.Name = "AssetPushLocation" Then

                                                If SubNodo.Attributes.GetNamedItem("ConfigurationID").Value = "AssetDelivery" Then
                                                    'Es una imagen a buscar

                                                    UbicacionIMG = SubNodo.InnerText

                                                    Dim ArregloDirectorioIMG() As String = UbicacionIMG.Split("/")
                                                    Dim NumeroDeSubCarpetas As Integer = ArregloDirectorioIMG.Length()

                                                    Dim PropiedadesDeLaIMG() As String = NombreIMG.Split("_") 'Debe tener 3 propiedades en el nombre de la imagen: CodigoFabricante, CodMaterialSAP y NumeroDeFoto

                                                    If PropiedadesDeLaIMG.Length() = 3 Then

                                                        Dim DirectorioDeLaImagen As String = DespejaUbicacionIMG(UbicacionIMG, CarpetaIMGProducto)
                                                        Dim ArchivoIMGStreamReader As StreamReader

                                                        Dim ExisteLaIMG As Boolean = ftp.FtpFileExists(DirectorioDeLaImagen)

                                                        If ExisteLaIMG = True Then

                                                            ArchivoIMGStreamReader = ftp.Download2(DirectorioDeLaImagen)

                                                            GrabarFotosProducto(ArchivoIMGStreamReader.BaseStream, 1000, 100, PropiedadesDeLaIMG(0), PropiedadesDeLaIMG(2), PropiedadesDeLaIMG(1))

                                                            ArchivoIMGStreamReader.Close()
                                                        Else
                                                            'La ubicacion que entrega el XML no retorna imagen
                                                            adLog.Insert(Now(), "Cargador IMG Producto", Nothing, "La imagen '" & NombreIMG & ".jpg' No existe en el directorio('" & DirectorioDeLaImagen & "') señalado por el archivo XML('" & file.Filename & "')")
                                                        End If

                                                    Else
                                                        'La imagen no cumple con los tres datos necesarios para cargarla... CodigoFabricante, MaterialSAP, NumeroDeFoto
                                                        adLog.Insert(Now(), "Cargador IMG Producto", Nothing, "La imagen '" & NombreIMG & "' mencionada en el archivo '" & file.Filename & "' no cumple con la nomenclatura necesaria para cargarla")
                                                    End If
                                                End If
                                            End If

                                        Next

                                    Next
                                Else
                                    'El archivo XML cargado no contiene el tag AssetPushLocation con la ubicacion de la imagen
                                    adLog.Insert(Now(), "Cargador IMG Producto", Nothing, "El archivo '" & file.Filename & "' no contiene el nodo AssetPushLocation con la ubicación de la imagen")
                                End If

                            ElseIf (TipoAssetAtributo = "JobImage") Then

                                Dim IDTrabajo As Integer
                                Dim NombreTrabajo As String = ""
                                Dim adTrabajo As STEP_trabajoTableAdapter = New STEP_trabajoTableAdapter

                                If (ListaNodosXML.Item(0).OuterXml.Contains("AssetPushLocation") = True) Then


                                    For Each nodoXML In ListaNodosXML

                                        NombreTrabajo = nodoXML.Item("Name").InnerText
                                        IDTrabajo = adTrabajo.GetIDTrabajoPorNombre(NombreTrabajo)


                                        For Each SubNodo In nodoXML

                                            If SubNodo.Name = "AssetPushLocation" Then

                                                If SubNodo.Attributes.GetNamedItem("ConfigurationID").Value = "AssetDelivery" Then
                                                    'Es una imagen a buscar

                                                    UbicacionIMG = SubNodo.InnerText

                                                    Dim DirectorioDeLaImagen As String = DespejaUbicacionIMG(UbicacionIMG, CarpetaIMGProducto)
                                                    Dim ArchivoIMGStreamReader As StreamReader

                                                    Dim ExisteLaIMG As Boolean = ftp.FtpFileExists(DirectorioDeLaImagen)

                                                    If ExisteLaIMG = True Then

                                                        If IDTrabajo <> 0 Then

                                                            ArchivoIMGStreamReader = ftp.Download2(DirectorioDeLaImagen)

                                                            GrabarFotosTrabajo(ArchivoIMGStreamReader.BaseStream, 720, 440, IDTrabajo)

                                                            ArchivoIMGStreamReader.Close()
                                                        Else
                                                            adLog.Insert(Now(), "Cargador IMG Job", Nothing, "El Job '" & NombreTrabajo & "' mencionado en el archivo '" & file.Filename & "' no existe como tal en la base de datos")
                                                        End If

                                                    Else
                                                        'La ubicacion que entrega el XML no entrega ninguna imagen
                                                        adLog.Insert(Now(), "Cargador IMG Job", Nothing, "La imagen '" & DirectorioDeLaImagen & "' no existe en el directorio mencionado por el XML '" & file.Filename & "'")
                                                    End If


                                                End If
                                            End If

                                        Next

                                    Next
                                Else
                                    'El archivo XML cargado no contiene el tag AssetPushLocation con la ubicacion de la imagen
                                    adLog.Insert(Now(), "Cargador IMG Job", Nothing, "El archivo '" & file.Filename & "' no contiene el nodo AssetPushLocation con la ubicación de la imagen")
                                End If

                            End If
                            'Mueve archivo de carga
                            '''''''''''''''''ftp.FtpRename(file.FullName, CarpetaDestinoXMLimg & file.NameOnly & ".xml")

                        Else
                            adLog.Insert(Now(), "Cargador IMG Producto", Nothing, "Problemas al cargar el archivo '" & file.Filename & "'")
                            'Mueve archivo de carga
                            '''''''''''''''''ftp.FtpRename(file.FullName, DirXMLImgProductoFTPCargadosConErrores & file.NameOnly & ".xml")
                        End If
                        If (ArchivoStreamReader Is Nothing) = False Then
                            ArchivoStreamReader.Close()
                        End If
                    End If
                Next

            End If


        Catch ex As Exception
            adLog.Insert(Date.Now, "Cargador IMG ", 0, "Error en el cargador de IMG, detalle : " & ex.Message)
        End Try

    End Sub

    Public Function DespejaUbicacionIMG(ByVal ubicacionSegunXML As String, ByVal PathUbicacionAppConfig As String) As String
        'Elimina un Sub directorio AssetDelivery si en el path del app.config y el archivo XML lo tienen
        Dim UbicacionFinal As String = ""
        ubicacionSegunXML = "/" & ubicacionSegunXML

        Dim BuscadorEnPath As Integer = PathUbicacionAppConfig.IndexOf("AssetDelivery")
        Dim BuscadorEnXML As Integer = ubicacionSegunXML.IndexOf("AssetDelivery")


        If BuscadorEnPath = -1 And BuscadorEnXML > 0 Then
            'AssetDelivery Aparece solo en el archivo XML
            UbicacionFinal = PathUbicacionAppConfig.Substring(0) & ubicacionSegunXML.Substring(1)

        ElseIf BuscadorEnPath > 0 And BuscadorEnXML > 0 Then
            'La palabra AssetDelivery aparece en ambos lugares, hay que eliminar una
            UbicacionFinal = PathUbicacionAppConfig.Substring(0, BuscadorEnPath) & ubicacionSegunXML.Substring(1)
        Else
            UbicacionFinal = PathUbicacionAppConfig & ubicacionSegunXML
        End If

        Return UbicacionFinal

    End Function

    Public Sub GrabarFotosProducto(ByVal data As IO.Stream, ByVal anchoImageGrande As Integer, ByVal anchoImageMini As Integer, ByVal ID As String, ByVal orden As Integer, ByVal codMaterial As String)

        Dim original_image As System.Drawing.Image = Nothing
        Dim final_image As System.Drawing.Bitmap = Nothing
        Dim final_image2 As System.Drawing.Bitmap = Nothing
        Dim graphic As System.Drawing.Graphics = Nothing
        Dim graphic2 As System.Drawing.Graphics = Nothing
        Dim dataOut As System.IO.MemoryStream = New System.IO.MemoryStream()
        Dim dataOut2 As System.IO.MemoryStream = New System.IO.MemoryStream()

        Try
            original_image = System.Drawing.Image.FromStream(data)

            Dim width As Integer = original_image.Width
            Dim height As Integer = original_image.Height
            Dim target_width As Integer = anchoImageGrande
            Dim target_widthMini As Integer = anchoImageMini

            Dim target_height As Integer = anchoImageGrande
            Dim target_heightMini As Integer = anchoImageMini

            Dim new_width, new_height As Integer
            Dim new_width2, new_height2 As Integer

            Dim target_ratio As Double = target_width / target_height
            Dim image_ratio As Double = width / height

            Dim target_ratio2 As Double = target_widthMini / target_heightMini

            If target_ratio > image_ratio Then
                new_height = target_height
                new_width = Math.Floor(image_ratio * target_height)
            Else
                new_height = Math.Floor(target_width / image_ratio)
                new_width = target_width
            End If

            If target_ratio2 > image_ratio Then
                new_height2 = target_heightMini
                new_width2 = Math.Floor(image_ratio * target_heightMini)
            Else
                new_height2 = Math.Floor(target_widthMini / image_ratio)
                new_width2 = target_widthMini
            End If

            final_image = New System.Drawing.Bitmap(target_width, target_height)
            final_image2 = New System.Drawing.Bitmap(target_widthMini, target_heightMini)

            graphic = System.Drawing.Graphics.FromImage(final_image)
            graphic2 = System.Drawing.Graphics.FromImage(final_image2)

            graphic.FillRectangle(New System.Drawing.SolidBrush(System.Drawing.Color.White), New System.Drawing.Rectangle(0, 0, target_width, target_height))
            graphic2.FillRectangle(New System.Drawing.SolidBrush(System.Drawing.Color.White), New System.Drawing.Rectangle(0, 0, target_widthMini, target_heightMini))

            Dim paste_x As Integer = (target_width - new_width) / 2
            Dim paste_x2 As Integer = (target_widthMini - new_width2) / 2

            Dim paste_y As Integer = (target_height - new_height) / 2
            Dim paste_y2 As Integer = (target_heightMini - new_height2) / 2

            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic  '/* new way */
            graphic2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic  '/* new way */

            graphic.DrawImage(original_image, paste_x, paste_y, new_width, new_height)
            graphic2.DrawImage(original_image, paste_x2, paste_y2, new_width2, new_height2)

            Dim fotoTA As fotoTableAdapter = New fotoTableAdapter

            'Se eliminan las fotos previas
            fotoTA.BorraMaterial(codMaterial, orden)

            final_image.Save(dataOut, System.Drawing.Imaging.ImageFormat.Jpeg)

            'Se inserta la imagen de tamaño normal
            fotoTA.InsertaNormal(ID, codMaterial, orden, dataOut.GetBuffer())

            final_image2.Save(dataOut2, System.Drawing.Imaging.ImageFormat.Jpeg)

            'Se inserta la imagen de tamaño mini
            fotoTA.InsertarMini(dataOut2.GetBuffer(), ID, orden)

        Catch ex As Exception


        End Try

        ' // Clean up
        If Not final_image Is Nothing Then
            final_image.Dispose()
        End If
        If Not final_image2 Is Nothing Then
            final_image2.Dispose()
        End If
        If Not graphic Is Nothing Then
            graphic.Dispose()
        End If
        If Not graphic2 Is Nothing Then
            graphic2.Dispose()
        End If
        If Not original_image Is Nothing Then
            original_image.Dispose()
        End If


    End Sub

    Public Sub GrabarFotosTrabajo(ByVal data As IO.Stream, ByVal AnchoImage As Integer, ByVal AltoImage As Integer, ByVal ID As String)
        Dim original_image As System.Drawing.Image = Nothing
        Dim final_image As System.Drawing.Bitmap = Nothing
        Dim graphic As System.Drawing.Graphics = Nothing
        Dim dataOut As System.IO.MemoryStream = New System.IO.MemoryStream()

        Dim adTrabajoIMG As GR_trabajoFotoTableAdapter = New GR_trabajoFotoTableAdapter

        Try
            original_image = System.Drawing.Image.FromStream(data)
            Dim width As Integer = original_image.Width
            Dim height As Integer = original_image.Height
            Dim target_width As Integer = AnchoImage
            Dim target_height As Integer = AltoImage

            Dim new_width, new_height As Integer

            Dim target_ratio As Double = target_width / target_height
            Dim image_ratio As Double = width / height

            If target_ratio > image_ratio Then
                new_height = target_height
                new_width = Math.Floor(image_ratio * target_height)
            Else
                new_height = Math.Floor(target_width / image_ratio)
                new_width = target_width
            End If

            final_image = New System.Drawing.Bitmap(target_width, target_height)
            graphic = System.Drawing.Graphics.FromImage(final_image)
            graphic.FillRectangle(New System.Drawing.SolidBrush(System.Drawing.Color.White), New System.Drawing.Rectangle(0, 0, target_width, target_height))

            Dim paste_x As Integer = (target_width - new_width) / 2
            Dim paste_y As Integer = (target_height - new_height) / 2

            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic  '/* new way */
            graphic.DrawImage(original_image, paste_x, paste_y, new_width, new_height)

            final_image.Save(dataOut, System.Drawing.Imaging.ImageFormat.Jpeg)

            adTrabajoIMG.Delete(ID)

            adTrabajoIMG.Insert(ID, dataOut.GetBuffer())


        Catch ex As Exception

        End Try
    End Sub

    Public Sub CargarStepCatalogo(ByVal HostFTP As String, ByVal UsrFTP As String, ByVal ClaveUSRFTP As String, ByVal CarpetaXMLACES As String, ByVal CarpetaXMLACES_Cargados As String, ByVal CarpetaXMLACES_FullCargaErrores As String, ByVal TipoCarga As String)
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        If CarpetaXMLACES <> "" Then

            Try

                Dim ftp As New FTPclient(HostFTP, UsrFTP, ClaveUSRFTP)                      'Conectar con FTP
                Dim dirList As FTPdirectory = ftp.ListDirectoryDetail(CarpetaXMLACES)  'Enlista la carpeta principal
                Dim filesOnly As FTPdirectory = dirList.GetFiles()
                Dim adCatalogo As STEP_catalogoTableAdapter = New STEP_catalogoTableAdapter


                Dim DocumentoXML As XmlDocument
                Dim ColeccionNodos As XmlNodeList
                Dim IDCatalogo As Integer
                Dim idVehiculo As String
                Dim idMotor As Integer?
                Dim Cantidad As String
                Dim idSubCategoria As String
                Dim idSKU As String
                Dim NumNotas As Integer = 0
                Dim ArrDesNotas As String()
                ReDim ArrDesNotas(32)
                Dim IdNota3, IdNota5, IdNota7, IdNota9, IdNota11, IdNota13, IdNota15, IdNota17, IdNota19, IdNota21, IdNota23, IdNota25, IdNota27, IdNota29, IdNota31 As Integer?

                DocumentoXML = New XmlDocument()

                For Each archivo As FTPfileInfo In filesOnly
                    If archivo.Extension = "xml" Then
                        Dim streamReader2 As StreamReader
                        streamReader2 = ftp.Download2(archivo.FullName)

                        DocumentoXML.Load(streamReader2)
                        ColeccionNodos = DocumentoXML.SelectNodes("ACES/App")

                        If (ColeccionNodos Is Nothing) = False Then
                            For Each nodo In ColeccionNodos
                                If (nodo Is Nothing) = False Then

                                    If nodo.InnerXml.Contains("id=") = True Then
                                        IDCatalogo = nodo.Attributes.GetNamedItem("id").Value
                                    End If

                                    NumNotas = 0

                                    For Each SubNodo In nodo
                                        Select Case SubNodo.Name()
                                            Case "BaseVehicle"
                                                idVehiculo = SubNodo.Attributes.GetNamedItem("id").Value
                                            Case "EngineBase"
                                                idMotor = CInt(SubNodo.Attributes.GetNamedItem("id").Value)
                                            Case "Qty"
                                                Cantidad = SubNodo.InnerText
                                            Case "PartType"
                                                idSubCategoria = SubNodo.Attributes.GetNamedItem("id").Value
                                            Case "Part"
                                                idSKU = SubNodo.InnerText
                                            Case "Note"
                                                NumNotas = NumNotas + 1
                                        End Select


                                    Next

                                    'Eliminar e insertar
                                    If IDCatalogo <> 0 Then

                                        If idMotor = 0 Then idMotor = Nothing

                                        Try
                                            adCatalogo.EliminarSiExiste(idVehiculo & idSubCategoria & idSKU)
                                            adCatalogo.Insert(idVehiculo & idSubCategoria & idSKU, idSubCategoria, idSKU, idMotor, Nothing, _
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                                              Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, _
                                                              Nothing, Nothing, Nothing, Now())

                                        Catch ex As Exception
                                            adLog.Insert(Now(), "Cargador de Step Catalogo ", Nothing, "Error al insertar el registro n° '" & IDCatalogo & "' desde el xml '" & archivo.Filename & "', Tipo de carga '" & TipoCarga & "', detalle : " & ex.Message)
                                        End Try

                                        If NumNotas > 0 Then
                                            'Insertar notas

                                            Dim indice As Integer = 1
                                            Dim IDNota As Integer = 0
                                            Dim DesNota As String = ""

                                            For Each SubNodo In nodo
                                                If SubNodo.Name() = "Note" Then
                                                    If (SubNodo.HasAttribute("id") = True) Then
                                                        IDNota = SubNodo.Attributes.GetNamedItem("id").Value
                                                        DesNota = SubNodo.InnerText()

                                                        ArrDesNotas(indice) = IDNota
                                                        ArrDesNotas(indice + 1) = DesNota
                                                        indice = indice + 2
                                                    Else
                                                        ArrDesNotas(1) = ""
                                                        adLog.Insert(Now(), "Cargador de Step Catalogo ", Nothing, "Error al insertar el registro n° '" & IDCatalogo & "' desde el xml '" & archivo.Filename & "', el tag Note no tiene atributo ID")
                                                    End If

                                                End If
                                            Next

                                            If ArrDesNotas(1).ToString <> "" Then

                                                If ArrDesNotas(3) Is Nothing Then IdNota3 = Nothing Else IdNota3 = ArrDesNotas(3)
                                                If ArrDesNotas(5) Is Nothing Then IdNota5 = Nothing Else IdNota5 = ArrDesNotas(5)
                                                If ArrDesNotas(7) Is Nothing Then IdNota7 = Nothing Else IdNota7 = ArrDesNotas(7)
                                                If ArrDesNotas(9) Is Nothing Then IdNota9 = Nothing Else IdNota9 = ArrDesNotas(9)
                                                If ArrDesNotas(11) Is Nothing Then IdNota11 = Nothing Else IdNota11 = ArrDesNotas(11)
                                                If ArrDesNotas(13) Is Nothing Then IdNota13 = Nothing Else IdNota13 = ArrDesNotas(13)
                                                If ArrDesNotas(15) Is Nothing Then IdNota15 = Nothing Else IdNota15 = ArrDesNotas(15)
                                                If ArrDesNotas(17) Is Nothing Then IdNota17 = Nothing Else IdNota17 = ArrDesNotas(17)
                                                If ArrDesNotas(19) Is Nothing Then IdNota19 = Nothing Else IdNota19 = ArrDesNotas(19)
                                                If ArrDesNotas(21) Is Nothing Then IdNota21 = Nothing Else IdNota21 = ArrDesNotas(21)
                                                If ArrDesNotas(23) Is Nothing Then IdNota23 = Nothing Else IdNota23 = ArrDesNotas(23)
                                                If ArrDesNotas(25) Is Nothing Then IdNota25 = Nothing Else IdNota25 = ArrDesNotas(25)
                                                If ArrDesNotas(27) Is Nothing Then IdNota27 = Nothing Else IdNota27 = ArrDesNotas(27)
                                                If ArrDesNotas(29) Is Nothing Then IdNota29 = Nothing Else IdNota29 = ArrDesNotas(29)
                                                If ArrDesNotas(31) Is Nothing Then IdNota31 = Nothing Else IdNota31 = ArrDesNotas(31)

                                                Try
                                                    'Insertar Notas
                                                    adCatalogo.InsertaNotas(ArrDesNotas(1), ArrDesNotas(2), IdNota3, ArrDesNotas(4), IdNota5, ArrDesNotas(6), _
                                                                            IdNota7, ArrDesNotas(8), IdNota9, ArrDesNotas(10), IdNota11, ArrDesNotas(12), _
                                                                            IdNota13, ArrDesNotas(14), IdNota15, ArrDesNotas(16), IdNota17, ArrDesNotas(18), _
                                                                            IdNota19, ArrDesNotas(20), IdNota21, ArrDesNotas(22), IdNota23, ArrDesNotas(24), _
                                                                            IdNota25, ArrDesNotas(26), IdNota27, ArrDesNotas(28), IdNota29, ArrDesNotas(30), _
                                                                            IdNota31, ArrDesNotas(32), Now(), idVehiculo & idSubCategoria & idSKU)

                                                Catch ex As Exception
                                                    adLog.Insert(Now(), "Cargador de Step Catalogo", Nothing, "Error al insertar las notas del IDVehiculo " & idVehiculo & " que está en el archivo '" & archivo.Filename & "', detalle del error : " & ex.Message)
                                                End Try
                                            End If


                                            NumNotas = 0

                                        End If
                                    End If
                                End If
                            Next
                            'Mueve archivo
                            '''''''''''''''''ftp.FtpRename(archivo.FullName, CarpetaXMLACES_Cargados & archivo.NameOnly & ".xml")
                        Else
                            adLog.Insert(Now(), "Cargador de Step Catalogo", Nothing, "Error al intentar cargar la información del archivo '" & archivo.Filename & ", Tipo de carga '" & TipoCarga & "'")
                            'Mueve archivo con error
                            '''''''''''''''''ftp.FtpRename(archivo.FullName, CarpetaXMLACES_FullCargaErrores & archivo.NameOnly & ".xml")
                        End If

                    End If

                Next
            Catch ex As Exception
                adLog.Insert(Now(), "Cargador de Step Catalogo", Nothing, "Error al cargar la tabla Step Catalogo, Detalle: " & ex.Message)
            End Try
        End If
    End Sub

    Public Sub CargarProforma(ByVal HostFTP As String, ByVal UsrFTP As String, ByVal ClaveUSRFTP As String, ByVal CarpetaXMLProformas As String, ByVal CarpetaXMLProformasCargadasOK As String, ByVal CarpetaXMLProformaCargadosConErrores As String, ByVal TipoCarga2 As String)
        Dim Msg As String = "", TipoCarga As String = ""

        Dim DocumentoXML As XmlDocument
        Dim m_nodelist As XmlNodeList

        Dim numErrores As Integer

        DocumentoXML = New XmlDocument()
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter
        Dim NombreArchivo As String = ""
        Try

            Dim ftp As New FTPclient(HostFTP, UsrFTP, ClaveUSRFTP)                      'Conectar con FTP
            Dim dirList As FTPdirectory = ftp.ListDirectoryDetail(CarpetaXMLProformas)  'Enlista la carpeta principal
            Dim filesOnly As FTPdirectory = dirList.GetFiles()

            For Each file As FTPfileInfo In filesOnly
                If file.Extension = "xml" Then

                    Dim streamReader2 As StreamReader
                    streamReader2 = ftp.Download2(file.FullName)

                    DocumentoXML.Load(streamReader2)

                    NombreArchivo = file.Filename
                    'Carga Motor
                    m_nodelist = DocumentoXML.SelectNodes("STEP-ProductInformation/Classifications/Classification/Classification [@ID='ACESSTRUCTURE']/Classification [@ID='AENGINEGROUP']/Classification [@ID='AENGINECONFIGS']")

                    numErrores = CargarMotor(m_nodelist)

                    'carga STEP Vehiculo
                    m_nodelist = DocumentoXML.SelectNodes("STEP-ProductInformation/Classifications/Classification/Classification [@ID='ACESSTRUCTURE']/Classification [@ID='AVEHICLEGROUP']/Classification [@ID='ABASEVEHICLES']")

                    numErrores = numErrores + CargarVehiculo(m_nodelist)

                    'Carga Step Trabajo
                    m_nodelist = DocumentoXML.SelectNodes("STEP-ProductInformation/Classifications/Classification/Classification")

                    numErrores = numErrores + CargarTrabajo(m_nodelist)

                    'Carga Step Subcategoria
                    m_nodelist = DocumentoXML.SelectNodes("STEP-ProductInformation/Classifications/Classification/Classification")

                    numErrores = numErrores + CargarSubcategoria(m_nodelist)

                    'Carga Step Notas
                    m_nodelist = DocumentoXML.SelectNodes("STEP-ProductInformation/Classifications/Classification/Classification")

                    numErrores = numErrores + CargarNotas(m_nodelist)

                    If numErrores = 0 Or numErrores = -1 Then
                        'Archivo cargado existosamente
                        '''''''''''''''''ftp.FtpRename(file.FullName, CarpetaXMLProformasCargadasOK & file.NameOnly & ".xml")
                    Else
                        'Archivo con errores
                        '''''''''''''''''ftp.FtpRename(file.FullName, CarpetaXMLProformaCargadosConErrores & file.NameOnly & ".xml")
                    End If


                End If

            Next


        Catch ex As Exception
            'Error general de la carga de la proformas
            adLog.Insert(Now(), "Cargador Proforma", Nothing, "Error al carga archivo de proforma: " & ex.Message & ", nombre archivo : " & NombreArchivo)

        End Try

    End Sub

    Public Function CargarMotor(ByVal ListaDeNodos As XmlNodeList) As Integer
        Dim NumErrores As Integer
        Dim idMotor0 As Integer = 0
        Dim arrMotor0 As Array
        Dim strMotor0 As String = ""
        Dim adSTEP_Motor As STEP_motorTableAdapter = New STEP_motorTableAdapter
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        If (ListaDeNodos Is Nothing) = False Then
            For Each m_node In ListaDeNodos                                       'recorre nodo buscado
                If m_node.Attributes.GetNamedItem("ID").Value.ToString = "AENGINECONFIGS" Then

                    For Each m_node2 In m_node.SelectNodes("Classification [@UserTypeID='AENGINECONFIGMANUFACTUR']")

                        For Each m_node3 In m_node2.GetElementsByTagName("Classification")
                            arrMotor0 = Split(m_node3.Attributes("ID").Value.ToString, "@")
                            idMotor0 = arrMotor0(1)

                            strMotor0 = m_node3.InnerText

                            If (idMotor0 <> Nothing) And (idMotor0 <> 0) Then
                                Try
                                    'delete previo
                                    adSTEP_Motor.EliminarPorMotorID(idMotor0)

                                    'inserta datos
                                    adSTEP_Motor.Insert(idMotor0, Strings.Left(Trim(strMotor0), 50))
                                    NumErrores = 0
                                Catch ex As Exception
                                    adLog.Insert(Now(), "Cargador STEP Motor", Nothing, "Error en carga STEP Motor: " & ex.Message)
                                    NumErrores = NumErrores + 1
                                End Try
                            End If
                        Next
                    Next
                End If
            Next
        Else
            'No encontró la lista de nodos para realizar la carga de motores
            NumErrores = -1
        End If

        Return NumErrores
    End Function

    Public Function CargarVehiculo(ByVal ListaDeNodos As XmlNodeList) As Integer
        Dim numErrores As Integer

        Dim strMarca As String = ""
        Dim arrMarca As Array
        Dim strModelo As String = ""
        Dim idMarca As Integer = 0
        Dim arrModelo As Array
        Dim idModelo As Integer = 0
        Dim strAño As String = ""
        Dim arrAño As Array
        Dim idAño As Integer = 0
        Dim arrVehiculo As Array
        Dim idVehiculo As Integer = 0
        Dim arrMotor As Array
        Dim idMotor As Integer = 0

        Dim adSTEP_Vehiculo As STEP_vehiculoTableAdapter = New STEP_vehiculoTableAdapter
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        If (ListaDeNodos Is Nothing) = False Then

            For Each m_node In ListaDeNodos                                       'recorre nodo buscado
                If m_node.Attributes.GetNamedItem("ID").Value.ToString = "ABASEVEHICLES" Then

                    For Each m_node2 In m_node.SelectNodes("Classification")
                        If m_node2.Attributes.GetNamedItem("UserTypeID").Value.ToString = "AMAKE" Then  'Marca
                            strMarca = m_node2.Item("Name").InnerText.ToString
                            arrMarca = Split(m_node2.Attributes("ID").Value.ToString, "@")
                            If arrMarca(1) <> Nothing Then idMarca = arrMarca(1)

                            For Each m_node3 In m_node2.SelectNodes("Classification")
                                If m_node3.Attributes.GetNamedItem("UserTypeID").Value.ToString = "AMAKEMODEL" Then  'Modelo
                                    strModelo = m_node3.Item("Name").InnerText.ToString
                                    arrModelo = Split(m_node3.Attributes("ID").Value.ToString, "@")
                                    If arrModelo(1) <> Nothing Then idModelo = arrModelo(1)

                                    For Each m_node4 In m_node3.SelectNodes("Classification [@UserTypeID='ABASEVEHICLE']")  'Año

                                        strAño = m_node4.FirstChild.InnerText 'm_node4.FirstChild.Item("Name").InnerText.ToString
                                        arrAño = Split(m_node4.Attributes("ID").Value.ToString, "@")
                                        If arrAño(1) <> Nothing Then idAño = arrAño(1)

                                        For Each m_node5 In m_node4.SelectNodes("Classification [@UserTypeID='AVEHICLE']")  'Vehiculo ID

                                            arrVehiculo = Split(m_node5.Attributes("ID").Value.ToString, "@")
                                            If arrVehiculo(1) <> Nothing Then idVehiculo = arrVehiculo(1)


                                            For Each m_node6 In m_node5.SelectNodes("MetaData/Value [@AttributeID='DER_COMBOID']")  'Motor ID

                                                arrMotor = Split(m_node6.FirstChild.InnerText.ToString, "@")
                                                If (arrMotor.Length - 1) > 1 Then
                                                    If InStr(1, arrMotor(1).ToString, "AENGINECONFIG") > 0 Then 'existe ID Motor

                                                        Try
                                                            idMotor = arrMotor(2)
                                                            'eliminación previa por vehiculo y motor
                                                            adSTEP_Vehiculo.EliminarPorVehiculoMotor(idVehiculo, idMotor)

                                                            'inserta registro en tabla
                                                            adSTEP_Vehiculo.Insert(idVehiculo, idAño, strAño, idMarca, strMarca, idModelo, strModelo, idMotor)
                                                            numErrores = 0

                                                        Catch ex As Exception
                                                            numErrores = numErrores + 1
                                                            adLog.Insert(Date.Now, "Maestro_STEPVehiculo", 0, "Error al intentar cargar el vehículo: " & idVehiculo.ToString & ", error: " & ex.Message)
                                                        End Try
                                                    Else 'si no hay motor genera registro Log
                                                        adLog.Insert(Date.Now, "Maestro_STEPVehiculo", 0, "Vehiculo ID : " & idVehiculo.ToString & " no posee Motor.")
                                                    End If
                                                Else    'si no hay motor genera registro Log
                                                    adLog.Insert(Date.Now, "Maestro_STEPVehiculo", 0, "Vehiculo ID : " & idVehiculo.ToString & " no posee Motor.")
                                                End If

                                            Next

                                        Next

                                    Next

                                End If
                            Next

                        End If
                    Next

                End If
            Next

        Else
            'No encontró la lista de nodos para realizar la carga de Step Vehiculo
            numErrores = -1
        End If

        Return numErrores
    End Function

    Public Function CargarTrabajo(ByVal ListaDeNodos As XmlNodeList) As Integer
        Dim numErrores As Integer = 0

        Dim IDTrabajo As String = ""
        Dim DescTrabajo As String = ""
        Dim ArrIDSubcatTrabajo As String()
        Dim IDSubcatTrabajo As String = ""

        Dim adSTEP_Trabajo As STEP_trabajoTableAdapter = New STEP_trabajoTableAdapter
        Dim adTrabajoSKU As STEP_trabajo_SKUTableAdapter = New STEP_trabajo_SKUTableAdapter
        Dim adSTEP_TrabajoSubCat As STEP_trabajo_SubcategoriaTableAdapter = New STEP_trabajo_SubcategoriaTableAdapter
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        If (ListaDeNodos Is Nothing) = False Then

            For Each m_node In ListaDeNodos                                       'recorre nodo buscado
                If m_node.Attributes.GetNamedItem("ID").Value.ToString = "JOBS" Then
                    For Each m_node2 In m_node.ChildNodes                       'recorre sub nodos con datos

                        If (m_node2.HasAttributes = True) Then
                            If (m_node2.Attributes.GetNamedItem("UserTypeID").Value.ToString = "JOB") Then

                                IDTrabajo = m_node2.Attributes.GetNamedItem("ID").Value.ToString
                                DescTrabajo = m_node2.InnerText
                                If InStr(1, IDTrabajo, "@") > 0 Then 'busca ID Trabajo válido
                                    Dim arrIDTrabajo As Array = Split(IDTrabajo, "@")
                                    Try
                                        'Elimina previa por IDTrabajo
                                        adSTEP_Trabajo.EliminarPorIDTrabajo(arrIDTrabajo(1))

                                        'Inserta datos
                                        adSTEP_Trabajo.Insert(Trim(arrIDTrabajo(1)), Trim(DescTrabajo))
                                        numErrores = 0

                                    Catch ex As Exception
                                        numErrores = numErrores + 1
                                        adLog.Insert(Now(), "Maestro trabajos", Nothing, "Error al insertar trabajo, detalle: " & ex.Message)
                                    End Try
                                    'Si el nodo del trabajo tiene mas de 1 nodos hijos, quiere decir que tiene SKUs a los cuales hace referencia
                                    If (m_node2.ChildNodes.Count > 1) Then

                                        Dim NombreReferencia As String

                                        For Each NodoJobs In m_node2

                                            NombreReferencia = NodoJobs.Name()

                                            If NombreReferencia = "ProductReference" Then

                                                If (NodoJobs.HasAttributes = True) Then

                                                    If (NodoJobs.Attributes.GetNamedItem("Type").Value.ToString = "ProductToJob") Then

                                                        Dim ID_sku As String = NodoJobs.Attributes.GetNamedItem("ProductID").Value.ToString
                                                        Dim ID_Trabajo As String = arrIDTrabajo(1)
                                                        Try
                                                            'Eliminar si existe previamente el trabajo
                                                            adTrabajoSKU.EliminarSiExiste(ID_sku)

                                                            'Insertar nuevo registro
                                                            adTrabajoSKU.Insert(ID_Trabajo, ID_sku)
                                                            numErrores = 0

                                                        Catch ex As Exception
                                                            numErrores = numErrores + 1
                                                            adLog.Insert(Now(), "Maestro trabajos SKU", Nothing, "Error al insertar trabajo SKU, detalle: " & ex.Message)
                                                        End Try
                                                    End If



                                                End If
                                            ElseIf NombreReferencia = "ClassificationCrossReference" Then
                                                'Trabajo asociado a subcategoria

                                                If (NodoJobs.HasAttributes = True) Then

                                                    If NodoJobs.Attributes.GetNamedItem("Type").Value.ToString = "JobPartType" Then

                                                        ArrIDSubcatTrabajo = NodoJobs.Attributes.GetNamedItem("ClassificationID").Value.ToString.Split("@")


                                                        If ArrIDSubcatTrabajo.Length = 2 Then

                                                            IDSubcatTrabajo = ArrIDSubcatTrabajo(1)
                                                            Try
                                                                adSTEP_TrabajoSubCat.EliminarSiExiste(IDSubcatTrabajo)

                                                                adSTEP_TrabajoSubCat.Insert(arrIDTrabajo(1), IDSubcatTrabajo)
                                                                numErrores = 0

                                                            Catch ex As Exception
                                                                numErrores = numErrores + 1
                                                                adLog.Insert(Now(), "Maestro trabajos Subcategoria", Nothing, "Error al insertar trabajo Subcategoria, detalle: " & ex.Message)
                                                            End Try
                                                        End If

                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If

                                End If

                            End If
                        End If
                    Next
                End If
            Next

        Else
            'No encontró la lista de nodos para realiar la carga de los trabajos
            numErrores = -1
        End If
        Return numErrores
    End Function

    Public Function CargarSubcategoria(ByVal ListaDeNodos As XmlNodeList) As Integer
        Dim NumErrores As Integer = 0
        Dim NombreCategoria As String = ""
        Dim ArrIDCategoria As String()
        Dim IDCategoria As String = ""
        Dim NombreSubCategoria As String = ""
        Dim ArrIDSubCategoria As String()
        Dim IDSubCategoria As String = ""
        Dim ArrClasificacion As String()
        Dim TipoClasificacion As String = ""
        Dim ordenPregunta As Integer?

        Dim adStepSubcategoria As STEP_subcategoriaTableAdapter = New STEP_subcategoriaTableAdapter
        Dim adStepSubCatConsejo As STEP_subcategoria_ConsejoTableAdapter = New STEP_subcategoria_ConsejoTableAdapter
        Dim adPregunta As STEP_subcategoria_preguntaTableAdapter = New STEP_subcategoria_preguntaTableAdapter
        Dim adStepLog As STEP_logTableAdapter = New STEP_logTableAdapter

        ordenPregunta = Nothing
        If (ListaDeNodos Is Nothing) = False Then
            For Each nodo In ListaDeNodos

                If nodo.Attributes.GetNamedItem("ID").Value.ToString = "ACESSTRUCTURE" Then

                    For Each Nodo20 In nodo.ChildNodes
                        If (Nodo20.HasAttributes = True) Then
                            If Nodo20.Attributes.GetNamedItem("ID").Value.ToString = "APARTSCATEGORIZATION" Then
                                For Each Nodo2 In Nodo20.ChildNodes
                                    If Nodo2.Name() = "Classification" Then
                                        For Each Nodo3 In Nodo2.ChildNodes
                                            If Nodo3.Name() = "Classification" Then
                                                For Each Nodo4 In Nodo3.ChildNodes
                                                    If Nodo4.Name() = "Name" Then

                                                        NombreCategoria = Nodo4.InnerText
                                                        ArrIDCategoria = Nodo3.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                        If ArrIDCategoria.Length = 2 Then IDCategoria = ArrIDCategoria(1)

                                                    End If

                                                    If Nodo4.Name() = "Classification" Then
                                                        For Each Nodo5 In Nodo4.ChildNodes
                                                            If Nodo5.Name() = "Name" Then
                                                                NombreSubCategoria = Nodo5.InnerText
                                                                ArrIDSubCategoria = Nodo4.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                                If ArrIDSubCategoria.Length = 2 Then IDSubCategoria = ArrIDSubCategoria(1)

                                                                If (Nodo4.ChildNodes.Count > 1) Then
                                                                    For Each Nodo6 In Nodo4.ChildNodes
                                                                        'Buscando posible consejo asociado
                                                                        If Nodo6.Name() = "ClassificationCrossReference" Then
                                                                            If (Nodo6.HasAttributes = True) Then
                                                                                ArrClasificacion = Nodo6.Attributes.GetNamedItem("ClassificationID").Value.ToString.Split("@")

                                                                                If ArrClasificacion.Length = 2 Then

                                                                                    TipoClasificacion = ArrClasificacion(0)

                                                                                    If UCase(TipoClasificacion) = "CONSEJO" Then
                                                                                        Try
                                                                                            adStepSubCatConsejo.EliminarSiExiste(IDSubCategoria)

                                                                                            adStepSubCatConsejo.Insert(IDSubCategoria, ArrClasificacion(1))
                                                                                            NumErrores = 0
                                                                                        Catch ex As Exception
                                                                                            NumErrores = NumErrores + 1
                                                                                            adStepLog.Insert(Now(), "Maestro subcategoria consejo", Nothing, "Error al insertar subcategoria consejo, detalle: " & ex.Message)
                                                                                        End Try
                                                                                    ElseIf UCase(TipoClasificacion) = "QUESTION" Then

                                                                                        For Each Nodo7 In Nodo6.childNodes
                                                                                            If Nodo7.Name() = "MetaData" Then
                                                                                                For Each Nodo8 In Nodo7.ChildNodes
                                                                                                    If Nodo8.Name() = "Value" Then
                                                                                                        ordenPregunta = CInt(Nodo8.InnerText())
                                                                                                    End If
                                                                                                Next
                                                                                            End If
                                                                                        Next
                                                                                        Try
                                                                                            adPregunta.EliminarSiExiste(IDSubCategoria, ArrClasificacion(1))
                                                                                            adPregunta.Insert(IDSubCategoria, ArrClasificacion(1), ordenPregunta, Now())
                                                                                            ordenPregunta = Nothing
                                                                                        Catch ex As Exception
                                                                                            adStepLog.Insert(Now(), "Maestro subcategoria pregunta", Nothing, "Error al insertar subcategoria pregunta, detalle: " & ex.Message)
                                                                                        End Try
                                                                                    End If
                                                                                End If
                                                                            End If

                                                                        End If
                                                                    Next
                                                                Else
                                                                    Try
                                                                        adStepSubcategoria.EliminarSiExiste(IDSubCategoria)

                                                                        adStepSubcategoria.Insert(IDCategoria, NombreCategoria, IDSubCategoria, NombreSubCategoria)

                                                                        NumErrores = 0
                                                                    Catch ex As Exception
                                                                        NumErrores = NumErrores + 1
                                                                        adStepLog.Insert(Now(), "Maestro subcategoria", Nothing, "Error al insertar subcategoria, detalle: " & ex.Message)
                                                                    End Try
                                                                End If
                                                            End If


                                                        Next
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                Next

                            End If
                        End If


                    Next


                End If

            Next
        Else
            NumErrores = -1
        End If
        Return NumErrores
    End Function

    Public Function CargarNotas(ByVal ListaDeNodos As XmlNodeList) As Integer

        Dim adNota As STEP_notaTableAdapter = New STEP_notaTableAdapter
        Dim adConsejo As STEP_consejoTableAdapter = New STEP_consejoTableAdapter
        Dim adPregunta As STEP_preguntaTableAdapter = New STEP_preguntaTableAdapter
        Dim adLog As STEP_logTableAdapter = New STEP_logTableAdapter

        Dim numErrores As Integer = 0
        Dim TipoNota As String
        Dim ArrNotasCabecera As String()
        Dim IDNotaCabecera As String = ""
        Dim ArrNotas As String()
        Dim IDNota As String = ""
        Dim DescNota As String = ""
        Dim Mensaje As String = ""
        Dim ArrPreguntas As String()
        Dim IDPregunta As String = ""
        Dim DescPregunta As String = ""
        Dim ArrConsejoCabecera As String()
        Dim ArrConsejo As String()
        Dim IDConsejo As String = ""
        Dim DescConsejo As String = ""
        Dim ArrAplicationNote As String()
        Dim IDAplicationNote As String = ""
        Dim DescAplicationNote As String = ""
        Dim ArrRespuesta As String()
        Dim DesRespuesta As String = ""

        If (ListaDeNodos Is Nothing) = False Then
            For Each nodo In ListaDeNodos

                If nodo.Attributes.GetNamedItem("ID").Value.ToString = "APPLICATIONNOTES" Then
                    For Each nodo2 In nodo.ChildNodes
                        If nodo2.Name() = "Classification" Then
                            If (nodo2.HasAttributes = True) Then
                                TipoNota = nodo2.Attributes.GetNamedItem("ID").Value.ToString

                                Select Case TipoNota
                                    Case "SubGroup_Notas"
                                        Try
                                            For Each nodo3 In nodo2.ChildNodes
                                                If nodo3.Name() = "Classification" Then
                                                    If (nodo3.HasAttributes = True) Then
                                                        ArrNotasCabecera = nodo3.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                        If ArrNotasCabecera.Length = 2 Then
                                                            IDNotaCabecera = ArrNotasCabecera(1)
                                                            For Each nodo4 In nodo3.ChildNodes
                                                                If nodo4.Name() = "Classification" Then
                                                                    If (nodo4.HasAttributes = True) Then
                                                                        ArrNotas = nodo4.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                                        If ArrNotas.Length = 2 Then
                                                                            IDNota = ArrNotas(1)
                                                                            For Each nodo5 In nodo4.ChildNodes
                                                                                If nodo5.Name() = "Name" Then

                                                                                    DescNota = nodo5.InnerText
                                                                                    Try
                                                                                        'Insertar nota
                                                                                        adNota.EliminarSiExiste(IDNota)

                                                                                        adNota.Insert(IDNota, "Nota", Nothing, DescNota)
                                                                                        numErrores = 0
                                                                                    Catch ex As Exception
                                                                                        numErrores = numErrores + 1
                                                                                        adLog.Insert(Now(), "Maestro notas", Nothing, "Error al insertar nota, detalle: " & ex.Message)
                                                                                    End Try
                                                                                End If

                                                                            Next
                                                                        End If
                                                                    End If
                                                                End If
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            Next

                                        Catch ex As Exception

                                            Mensaje = Mensaje & "Error al cargar las notas : " & ex.Message
                                            numErrores = numErrores + 1
                                            Exit For

                                        End Try

                                    Case "SubGroup_Preguntas"

                                        Try

                                            For Each nodo3 In nodo2.ChildNodes
                                                If nodo3.Name() = "Classification" Then
                                                    If (nodo3.HasAttributes = True) Then
                                                        ArrPreguntas = nodo3.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                        If ArrPreguntas.Length = 2 Then
                                                            IDPregunta = ArrPreguntas(1).ToString
                                                            For Each nodo4 In nodo3.ChildNodes
                                                                If nodo4.Name() = "Name" Then
                                                                    DescPregunta = nodo4.InnerText
                                                                End If

                                                                If (nodo3.ChildNodes.Count > 1) Then
                                                                    If nodo4.Name() = "Classification" Then
                                                                        ArrRespuesta = nodo4.Attributes.GetNamedItem("ID").Value.ToString.Split("@")

                                                                        If ArrRespuesta.Length = 2 Then

                                                                            If UCase(ArrRespuesta(0).ToString) = "ANSWER" Then
                                                                                For Each nodo5 In nodo4.ChildNodes
                                                                                    If nodo5.Name() = "Name" Then
                                                                                        DesRespuesta = nodo5.InnerText()
                                                                                        Try

                                                                                            adPregunta.EliminarSiExisteRespuesta(ArrRespuesta(1))
                                                                                            adPregunta.Insert(IDPregunta, DescPregunta, ArrRespuesta(1), DesRespuesta, Now())
                                                                                            numErrores = 0
                                                                                        Catch ex As Exception
                                                                                            numErrores = numErrores + 1
                                                                                            adLog.Insert(Now(), "Maestro pregunta", Nothing, "Error al insertar pregunta respuesta, detalle: " & ex.Message)
                                                                                        End Try

                                                                                    End If
                                                                                Next
                                                                            End If
                                                                        End If
                                                                    End If

                                                                Else
                                                                    Try
                                                                        adPregunta.EliminarSiExistePregunta(IDPregunta)
                                                                        adPregunta.Insert(IDPregunta, DescPregunta, Nothing, Nothing, Now())
                                                                    Catch ex As Exception
                                                                        numErrores = numErrores + 1
                                                                        adLog.Insert(Now(), "Maestro pregunta", Nothing, "Error al insertar pregunta, detalle: " & ex.Message)
                                                                    End Try
                                                                End If
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            Next

                                        Catch ex As Exception
                                            Mensaje = Mensaje & "Error al cargar las notas de las preguntas : " & ex.Message
                                            numErrores = numErrores + 1
                                            Exit For

                                        End Try

                                    Case "Consejos"

                                        Try

                                            For Each nodo3 In nodo2.ChildNodes
                                                If nodo3.Name() = "Classification" Then
                                                    If (nodo3.HasAttributes = True) Then
                                                        ArrConsejoCabecera = nodo3.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                        If ArrConsejoCabecera.Length = 2 Then
                                                            For Each nodo4 In nodo3.ChildNodes
                                                                If nodo4.Name() = "Classification" Then
                                                                    If (nodo4.HasAttributes = True) Then
                                                                        ArrConsejo = nodo4.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                                                        If ArrConsejo.Length = 2 Then
                                                                            IDConsejo = ArrConsejo(1).ToString
                                                                            For Each nodo5 In nodo4.ChildNodes
                                                                                If nodo5.Name() = "Name" Then
                                                                                    DescConsejo = nodo4.InnerText
                                                                                    Try
                                                                                        'Insertar consejo
                                                                                        adConsejo.EliminarSiExiste(ArrConsejoCabecera(1), IDConsejo)
                                                                                        adConsejo.Insert(ArrConsejoCabecera(1), IDConsejo, DescConsejo)
                                                                                    Catch ex As Exception
                                                                                        adLog.Insert(Now(), "Maestro consejo", Nothing, "Error al insertar consejo, detalle: " & ex.Message)
                                                                                        numErrores = numErrores + 1
                                                                                    End Try
                                                                                End If
                                                                            Next
                                                                        End If

                                                                    End If
                                                                End If
                                                            Next
                                                        End If
                                                    End If
                                                End If
                                            Next

                                        Catch ex As Exception
                                            Mensaje = Mensaje & "Error en la carga de notas para los consejos : " & ex.Message
                                            numErrores = numErrores + 1
                                            Exit For
                                        End Try

                                End Select


                                If InStr(1, nodo2.Attributes.GetNamedItem("ID").Value.ToString, "@") > 0 Then
                                    Try
                                        ArrAplicationNote = nodo2.Attributes.GetNamedItem("ID").Value.ToString.Split("@")
                                        IDAplicationNote = ArrAplicationNote(1).ToString
                                        For Each nodo3 In nodo2.ChildNodes
                                            If nodo3.Name() = "Name" Then
                                                DescAplicationNote = nodo3.InnerText
                                                Try

                                                    'Insertar AplicationNote
                                                    adNota.EliminarSiExiste(IDAplicationNote)

                                                    adNota.Insert(IDAplicationNote, "Nota_Aplicacion", Nothing, DescAplicationNote)

                                                Catch ex As Exception
                                                    adLog.Insert(Now(), "Maestro notas", Nothing, "Error al insertar nota, detalle: " & ex.Message)
                                                    numErrores = numErrores + 1
                                                End Try
                                            End If
                                        Next
                                    Catch ex As Exception
                                        Mensaje = Mensaje & "Error al cargar notas de las aplicaciones : " & ex.Message
                                        numErrores = numErrores + 1
                                    End Try
                                End If


                            End If
                        End If
                    Next

                End If

            Next
        Else
            'Problemas al leer el archivo
            numErrores = -1
        End If
        Return numErrores
    End Function



End Class
