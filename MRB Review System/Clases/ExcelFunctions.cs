using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.OpenXml4Net.OPC;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MRB_Review_System
{
    class ExcelFunctions
    {     
        /// <summary>Convierte un DataTable en un archivo de Excel (xls o Xlsx) y lo guarda en disco.</summary>
        /// <param name="pDatos">Datos de la Tabla a guardar. Usa el nombre de la tabla como nombre de la Hoja</param>
        /// <param name="pFilePath">Ruta del archivo donde se guarda.</param>
        public static string DataTable_To_Excel(DataTable pDatos, string pFilePath)
        {
            string Error = null;

            try
            {
                if (pDatos != null && pDatos.Rows.Count > 0)
                {
                    IWorkbook workbook = null;
                    ISheet worksheet = null;

                    IsBussy:
                    try
                    {
                        FileStream stream = new FileStream(pFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    }
                    catch (Exception)
                    {
                        goto IsBussy;
                    }

                    using (FileStream stream = new FileStream(pFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        string Ext = System.IO.Path.GetExtension(pFilePath); //<-Extension del archivo
                        switch (Ext.ToLower())
                        {
                            case ".xls":
                                HSSFWorkbook workbookH = new HSSFWorkbook();
                                NPOI.HPSF.DocumentSummaryInformation dsi = NPOI.HPSF.PropertySetFactory.CreateDocumentSummaryInformation();
                                dsi.Company = "Cutcsa"; dsi.Manager = "Departamento Informatico";
                                workbookH.DocumentSummaryInformation = dsi;
                                workbook = workbookH;
                                break;

                            case ".xlsx": workbook = new XSSFWorkbook(); break;
                        }

                        worksheet = workbook.CreateSheet(pDatos.TableName); //<-Usa el nombre de la tabla como nombre de la Hoja

                        //CREAR EN LA PRIMERA FILA LOS TITULOS DE LAS COLUMNAS
                        int iRow = 0;
                        if (pDatos.Columns.Count > 0)
                        {
                            int iCol = 0;
                            IRow fila = worksheet.CreateRow(iRow);
                            foreach (DataColumn columna in pDatos.Columns)
                            {
                                ICell cell = fila.CreateCell(iCol, CellType.String);
                                cell.SetCellValue(columna.ColumnName);
                                iCol++;
                            }
                            iRow++;
                        }

                        //FORMATOS PARA CIERTOS TIPOS DE DATOS
                        ICellStyle _doubleCellStyle = workbook.CreateCellStyle();
                        _doubleCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.###");

                        ICellStyle _intCellStyle = workbook.CreateCellStyle();
                        _intCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");

                        ICellStyle _boolCellStyle = workbook.CreateCellStyle();
                        _boolCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("BOOLEAN");

                        ICellStyle _dateCellStyle = workbook.CreateCellStyle();
                        _dateCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd-MM-yyyy");

                        ICellStyle _dateTimeCellStyle = workbook.CreateCellStyle();
                        _dateTimeCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd-MM-yyyy HH:mm:ss");

                        //AHORA CREAR UNA FILA POR CADA REGISTRO DE LA TABLA
                        foreach (DataRow row in pDatos.Rows)
                        {
                            IRow fila = worksheet.CreateRow(iRow);
                            int iCol = 0;
                            foreach (DataColumn column in pDatos.Columns)
                            {
                                ICell cell = null; //<-Representa la celda actual                               
                                object cellValue = row[iCol]; //<- El valor actual de la celda

                                switch (column.DataType.ToString())
                                {
                                    case "System.Boolean":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Boolean);

                                            if (Convert.ToBoolean(cellValue)) { cell.SetCellFormula("TRUE()"); }
                                            else { cell.SetCellFormula("FALSE()"); }

                                            cell.CellStyle = _boolCellStyle;
                                        }
                                        break;

                                    case "System.String":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.String);
                                            cell.SetCellValue(Convert.ToString(cellValue));
                                        }
                                        break;

                                    case "System.Int32":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Numeric);
                                            cell.SetCellValue(Convert.ToInt32(cellValue));
                                            cell.CellStyle = _intCellStyle;
                                        }
                                        break;
                                    case "System.Int64":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Numeric);
                                            cell.SetCellValue(Convert.ToInt64(cellValue));
                                            cell.CellStyle = _intCellStyle;
                                        }
                                        break;
                                    case "System.Decimal":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Numeric);
                                            cell.SetCellValue(Convert.ToDouble(cellValue));
                                            cell.CellStyle = _doubleCellStyle;
                                        }
                                        break;
                                    case "System.Double":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Numeric);
                                            cell.SetCellValue(Convert.ToDouble(cellValue));
                                            cell.CellStyle = _doubleCellStyle;
                                        }
                                        break;

                                    case "System.DateTime":
                                        if (cellValue != DBNull.Value)
                                        {
                                            cell = fila.CreateCell(iCol, CellType.Numeric);
                                            cell.SetCellValue(Convert.ToDateTime(cellValue));

                                            //Si No tiene valor de Hora, usar formato dd-MM-yyyy
                                            DateTime cDate = Convert.ToDateTime(cellValue);
                                            if (cDate != null && cDate.Hour > 0) { cell.CellStyle = _dateTimeCellStyle; }
                                            else { cell.CellStyle = _dateCellStyle; }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                iCol++;
                            }
                            iRow++;
                        }

                        workbook.Write(stream);
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return Error;
        }


        /// <summary>Abre un archivo de Excel (xls o xlsx) y lo convierte en un DataTable.
        /// LA PRIMERA FILA DEBE CONTENER LOS NOMBRES DE LOS CAMPOS.</summary>
        /// <param name="pRutaArchivo">Ruta completa del archivo a abrir.</param>
        /// <param name="pHojaIndex">Número (basado en cero) de la hoja que se desea abrir. 0 es la primera hoja.</param>
        public static DataTable Excel_To_DataTable(string pRutaArchivo, int pHojaIndex)
        {
            // --------------------------------- //
            /* REFERENCIAS:
             * NPOI.dll
             * NPOI.OOXML.dll
             * NPOI.OpenXml4Net.dll */
            // --------------------------------- //
            /* USING:
             * using NPOI.SS.UserModel;
             * using NPOI.HSSF.UserModel;
             * using NPOI.XSSF.UserModel; */
            // AUTOR: Ing. Jhollman Chacon R. 2015
            // --------------------------------- //

            DataTable Tabla = null;
            try
            {
                if (System.IO.File.Exists(pRutaArchivo))
                {

                    IWorkbook workbook = null;  //IWorkbook determina si es xls o xlsx              
                    ISheet worksheet = null;
                    string first_sheet_name = "";

                    using (FileStream FS = new FileStream(pRutaArchivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = WorkbookFactory.Create(FS);          //Abre tanto XLS como XLSX
                        worksheet = workbook.GetSheetAt(pHojaIndex);    //Obtener Hoja por indice
                        first_sheet_name = worksheet.SheetName;         //Obtener el nombre de la Hoja

                        Tabla = new DataTable(first_sheet_name);
                        Tabla.Rows.Clear();
                        Tabla.Columns.Clear();

                        // Leer Fila por fila desde la primera
                        for (int rowIndex = 0; rowIndex <= worksheet.LastRowNum; rowIndex++)
                        {
                            DataRow NewReg = null;
                            IRow row = worksheet.GetRow(rowIndex);
                            IRow row2 = null;
                            IRow row3 = null;

                            if (rowIndex == 0)
                            {
                                row2 = worksheet.GetRow(rowIndex + 1); //Si es la Primera fila, obtengo tambien la segunda para saber el tipo de datos
                                row3 = worksheet.GetRow(rowIndex + 2); //Y la tercera tambien por las dudas
                            }

                            if (row != null) //null is when the row only contains empty cells 
                            {
                                if (rowIndex > 0) NewReg = Tabla.NewRow();

                                int colIndex = 0;
                                //Leer cada Columna de la fila
                                foreach (ICell cell in row.Cells)
                                {
                                    object valorCell = null;
                                    string cellType = "";
                                    string[] cellType2 = new string[2];

                                    if (rowIndex == 0) //Asumo que la primera fila contiene los titlos:
                                    {
                                        for (int i = 0; i < 2; i++)
                                        {
                                            ICell cell2 = null;
                                            if (i == 0)
                                            {
                                                cell2 = row2.GetCell(cell.ColumnIndex);
                                            }
                                            else
                                            {
                                                cell2 = row3.GetCell(cell.ColumnIndex);
                                            }

                                            if (cell2 != null)
                                            {
                                                switch (cell2.CellType)
                                                {
                                                    case CellType.Blank: break;
                                                    case CellType.Boolean: cellType2[i] = "System.Boolean"; break;
                                                    case CellType.String: cellType2[i] = "System.String"; break;
                                                    case CellType.Numeric:
                                                        if (HSSFDateUtil.IsCellDateFormatted(cell2)) { cellType2[i] = "System.DateTime"; }
                                                        else
                                                        {
                                                            cellType2[i] = "System.Double";  //valorCell = cell2.NumericCellValue;
                                                        }
                                                        break;

                                                    case CellType.Formula:
                                                        bool continuar = true;
                                                        switch (cell2.CachedFormulaResultType)
                                                        {
                                                            case CellType.Boolean: cellType2[i] = "System.Boolean"; break;
                                                            case CellType.String: cellType2[i] = "System.String"; break;
                                                            case CellType.Numeric:
                                                                if (HSSFDateUtil.IsCellDateFormatted(cell2)) { cellType2[i] = "System.DateTime"; }
                                                                else
                                                                {
                                                                    try
                                                                    {
                                                                        //DETERMINAR SI ES BOOLEANO
                                                                        if (cell2.CellFormula == "TRUE()") { cellType2[i] = "System.Boolean"; continuar = false; }
                                                                        if (continuar && cell2.CellFormula == "FALSE()") { cellType2[i] = "System.Boolean"; continuar = false; }
                                                                        if (continuar) { cellType2[i] = "System.Double"; continuar = false; }
                                                                    }
                                                                    catch { }
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                    default:
                                                        cellType2[i] = "System.String"; break;
                                                }
                                            }
                                        }

                                        //Resolver las diferencias de Tipos
                                        if (cellType2[0] == cellType2[1]) { cellType = cellType2[0]; }
                                        else
                                        {
                                            if (cellType2[0] == null) cellType = cellType2[1];
                                            if (cellType2[1] == null) cellType = cellType2[0];
                                            if (cellType == "") cellType = "System.String";
                                        }

                                        //Obtener el nombre de la Columna
                                        string colName = "Column_{0}";
                                        try { colName = cell.StringCellValue; }
                                        catch { colName = string.Format(colName, colIndex); }

                                        //Verificar que NO se repita el Nombre de la Columna
                                        foreach (DataColumn col in Tabla.Columns)
                                        {
                                            if (col.ColumnName == colName) colName = string.Format("{0}_{1}", colName, colIndex);
                                        }

                                        //Agregar el campos de la tabla:
                                        DataColumn codigo = new DataColumn(colName, System.Type.GetType(cellType));
                                        Tabla.Columns.Add(codigo); colIndex++;
                                    }
                                    else
                                    {
                                        //Las demas filas son registros:
                                        switch (cell.CellType)
                                        {
                                            case CellType.Blank: valorCell = DBNull.Value; break;
                                            case CellType.Boolean: valorCell = cell.BooleanCellValue; break;
                                            case CellType.String: valorCell = cell.StringCellValue; break;
                                            case CellType.Numeric:
                                                if (HSSFDateUtil.IsCellDateFormatted(cell)) { valorCell = cell.DateCellValue; }
                                                else { valorCell = cell.NumericCellValue; }
                                                break;
                                            case CellType.Formula:
                                                switch (cell.CachedFormulaResultType)
                                                {
                                                    case CellType.Blank: valorCell = DBNull.Value; break;
                                                    case CellType.String: valorCell = cell.StringCellValue; break;
                                                    case CellType.Boolean: valorCell = cell.BooleanCellValue; break;
                                                    case CellType.Numeric:
                                                        if (HSSFDateUtil.IsCellDateFormatted(cell)) { valorCell = cell.DateCellValue; }
                                                        else { valorCell = cell.NumericCellValue; }
                                                        break;
                                                }
                                                break;
                                            default: valorCell = cell.StringCellValue; break;
                                        }
                                        //Agregar el nuevo Registro
                                        if (cell.ColumnIndex <= Tabla.Columns.Count - 1) NewReg[cell.ColumnIndex] = valorCell;
                                    }
                                }
                            }
                            if (rowIndex > 0) Tabla.Rows.Add(NewReg);
                        }
                        Tabla.AcceptChanges();
                    }
                }
                else
                {
                    throw new Exception("ERROR 404: El archivo especificado NO existe.");
                }
            }
            catch (Exception ex)
            {
                Tabla = null;
                MessageBox.Show(ex.Message, "DataBase Busy");
            }
            return Tabla;
        }
    }
}

