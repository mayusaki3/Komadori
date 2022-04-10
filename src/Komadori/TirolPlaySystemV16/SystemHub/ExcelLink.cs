using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// Microsoft Excelと連携するためのクラスです。
    /// </summary>
    [DebuggerNonUserCode]
    public class ExcelLink : Component
    {
        #region インナークラス

        #region ComHelperクラス

        /// <summary>
        /// COMオブジェクトを扱います。
        /// </summary>
        private static class ComHelper
        {
            #region インナークラス

            #region ComObjectクラス

            /// <summary>
            /// COMオブジェクトに名前を付けて記憶します。
            /// </summary>
            public class ComObjectClass
            {
                #region 構築・破棄

                /// <summary>
                /// XControls.XExcelLink.ComHelper.ComObjectClass クラスの新しいインスタンスを初期化します。
                /// </summary>
                /// <param name="name">名前</param>
                /// <param name="comobject">COMオブジェクト</param>
                public ComObjectClass(string name, object comobject)
                {
                    this.name = name;
                    this.comobject = comobject;
                }

                #endregion

                #region プロパティ

                #region Nameプロパティ

                private string name = "";
                /// <summary>
                /// COMオブジェクトの名前を参照または設定します。
                /// </summary>
                public string Name
                {
                    get
                    {
                        return name;
                    }
                    set
                    {
                        name = value;
                    }
                }

                #endregion

                #region ComObjectプロパティ

                private object comobject = null;
                /// <summary>
                /// COMオブジェクトを参照または設定します。
                /// </summary>
                public object ComObject
                {
                    get
                    {
                        return comobject;
                    }
                    set
                    {
                        comobject = value;
                    }
                }

                #endregion

                #endregion
            }

            #endregion

            #endregion

            #region 変数

            /// <summary>
            /// COMオブジェクトを保持
            /// </summary>
            private static ArrayList comOnjectBag = new ArrayList();

            #endregion

            #region プロパティ

            #region ExcelVersionプロパティ

            private static float excelversion = 0F;
            /// <summary>
            /// Excelのバージョンを参照または設定します。
            /// </summary>
            public static float ExcelVersion
            {
                get
                {
                    return excelversion;
                }
                set
                {
                    excelversion = value;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region オブジェクトプロパティ取得 (GetObject)

            /// <summary>
            /// オブジェクトのプロパティを取得します。
            /// </summary>
            /// <param name="obj">対象オブジェクト</param>
            /// <param name="prop">プロパティ名</param>
            /// <param name="parms">パラメータ配列</param>
            /// <returns>取得したオブジェクト</returns>
            public static object GetObject(object obj, string prop, object[] parms)
            {
                object o = null;
                try
                {
                    o = obj.GetType().InvokeMember(prop, BindingFlags.GetProperty, null, obj, parms);
                }
                catch (Exception es)
                {
                    throw new ApplicationException(FirstExceptionMessage(es));
                }
                return o;
            }

            #endregion

            #region オブジェクトプロパティ設定 (SetObject)

            /// <summary>
            /// オブジェクトのプロパティを設定します。
            /// </summary>
            /// <param name="obj">対象オブジェクト</param>
            /// <param name="prop">プロパティ名</param>
            /// <param name="parms">パラメータ配列</param>
            public static void SetObject(object obj, string prop, object[] parms)
            {
                try
                {
                    obj.GetType().InvokeMember(prop, BindingFlags.SetProperty, null, obj, parms);
                }
                catch (Exception es)
                {
                    throw new ApplicationException(FirstExceptionMessage(es));
                }
            }

            #endregion

            #region オブジェクトメソッド実行 (InvokeObject)

            /// <summary>
            /// オブジェクトのメソッドを実行する。
            /// </summary>
            /// <param name="obj">対象オブジェクト</param>
            /// <param name="method">メソッド名</param>
            public static void InvokeObject(object obj, string method)
            {
                try
                {
                    obj.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, obj, null);
                }
                catch (Exception es)
                {
                    throw new ApplicationException(FirstExceptionMessage(es));
                }
            }

            /// <summary>
            /// オブジェクトのメソッドを実行する。
            /// </summary>
            /// <param name="obj">対象オブジェクト</param>
            /// <param name="method">メソッド名</param>
            /// <param name="parms">パラメータ配列</param>
            public static void InvokeObject(object obj, string method, object[] parms)
            {
                try
                {
                    obj.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, obj, parms);
                }
                catch (Exception es)
                {
                    throw new ApplicationException(FirstExceptionMessage(es));
                }
            }

            /// <summary>
            /// オブジェクトのメソッドを実行する。
            /// </summary>
            /// <param name="obj">対象オブジェクト</param>
            /// <param name="method">メソッド名</param>
            /// <param name="parms">パラメータ配列</param>
            /// <param name="modify">参照渡し設定配列</param>
            public static void InvokeObject(object obj, string method, ref object[] parms, ParameterModifier[] modify)
            {
                try
                {
                    obj.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, obj, parms, modify, null, null);
                }
                catch (Exception es)
                {
                    throw new ApplicationException(FirstExceptionMessage(es));
                }
            }

            #endregion

            #region 最初に発生した例外メッセージを取得 (FirstExceptionMessage)

            /// <summary>
            /// 最初に発生した例外メッセージを返します。
            /// </summary>
            /// <param name="es">例外クラス</param>
            /// <returns>メッセージ</returns>
            public static string FirstExceptionMessage(Exception es)
            {
                string msg = "";
                Exception e = es;
                while (e != null)
                {
                    msg = e.Message;
                    e = e.InnerException;
                }
                return msg;
            }

            #endregion

            #endregion
        }

        #endregion

        #region CellClassクラス

        /// <summary>
        /// セルクラスです。
        /// </summary>
        public class CellClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.CellClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="cells">セルコレクションオブジェクト</param>
            /// <param name="row">セルの行位置</param>
            /// <param name="column">セルの桁位置</param>
            public CellClass(object cells, int row, int column)
            {
                cell = null;
                try
                {
                    object[] parm = new object[2];
                    parm[0] = row;
                    parm[1] = column;
                    cell = ComHelper.GetObject(cells, "Item", parm);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~CellClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Cellオブジェクト開放
                if (cell != null)
                {
                    Marshal.FinalReleaseComObject(cell);
                    cell = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// Cellオブジェクト
            /// </summary>
            private object cell = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// セルのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("セルのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Textプロパティ

            /// <summary>
            /// セルのテキストの設定または取得します。
            /// </summary>
            [
                Category("動作"),
                Description("セルのテキストの設定または取得します。"),

            ]
            public string Text
            {
                get
                {
                    string rt = "";
                    if (!isFailed)
                    {
                        rt = ComHelper.GetObject(cell, "Text", null).ToString();
                    }
                    return rt;
                }
                set
                {
                    object[] parm = new object[1];
                    parm[0] = value;
                    ComHelper.SetObject(cell, "Value", parm);
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region CellsClassクラス

        /// <summary>
        /// セルコレクションクラスです。
        /// </summary>
        public class CellsClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.CellsClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="worksheet">ワークシートオブジェクト</param>
            public CellsClass(object worksheet)
            {
                cells = null;
                try
                {
                    cells = ComHelper.GetObject(worksheet, "Cells", null);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~CellsClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Cellオブジェクト解放
                if (thisCell != null)
                {
                    thisCell.Dispose();
                    thisCell = null;
                }

                // Cellsオブジェクト開放
                if (cells != null)
                {
                    Marshal.FinalReleaseComObject(cells);
                    cells = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// Cellsオブジェクト
            /// </summary>
            private object cells = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// セルコレクションのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("セルコレクションのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Countプロパティ

            /// <summary>
            /// セルの数を返します。
            /// </summary>
            [
                Category("動作"),
                Description("セルの数を返します。シート全体のセルを表すなどの場合には -1 が返ります。"),

            ]
            public int Count
            {
                get
                {
                    int rt = 0;
                    if (!isFailed)
                    {
                        try
                        {
                            rt = int.Parse(ComHelper.GetObject(cells, "Count", null).ToString());
                        }
                        catch
                        {
                            rt = -1;
                        }
                    }
                    return rt;
                }
            }

            #endregion

            #region this[row, column]

            private CellClass thisCell = null;
            public CellClass this[int row, int column]
            {
                get
                {
                    if (!isFailed)
                    {
                        if (thisCell != null)
                        {
                            thisCell.Dispose();
                            thisCell = null;
                        }
                        thisCell = new CellClass(cells, row, column);
                    }
                    return thisCell;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region EntireColumnClassクラス

        /// <summary>
        /// 選択カラム全体クラスです。
        /// </summary>
        public class EntireColumnClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.EntireColumnClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="range">レンジオブジェクト</param>
            public EntireColumnClass(object range)
            {
                entireColumn = null;
                try
                {
                    entireColumn = ComHelper.GetObject(range, "EntireColumn", null);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~EntireColumnClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // EntireColumnオブジェクト開放
                if (entireColumn != null)
                {
                    Marshal.FinalReleaseComObject(entireColumn);
                    entireColumn = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// EntireColumnオブジェクト
            /// </summary>
            private object entireColumn = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// 選択カラム全体のインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("選択カラム全体のインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Hiddenプロパティ

            /// <summary>
            /// 現在の範囲を非表示にするかを設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("現在の範囲を非表示にするかを設定または取得します。")
            ]
            public bool Hidden
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(entireColumn, "Hidden", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(entireColumn, "Hidden", parms);
                    }
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region EntireRowClassクラス

        /// <summary>
        /// 選択行全体クラスです。
        /// </summary>
        public class EntireRowClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.EntireRowClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="range">レンジオブジェクト</param>
            public EntireRowClass(object range)
            {
                entireRow = null;
                try
                {
                    entireRow = ComHelper.GetObject(range, "EntireRow", null);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~EntireRowClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // EntireRowオブジェクト開放
                if (entireRow != null)
                {
                    Marshal.FinalReleaseComObject(entireRow);
                    entireRow = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// EentireRowオブジェクト
            /// </summary>
            private object entireRow = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// 選択行全体のインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("選択行全体のインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Hiddenプロパティ

            /// <summary>
            /// 現在の範囲を非表示にするかを設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("現在の範囲を非表示にするかを設定または取得します。")
            ]
            public bool Hidden
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(entireRow, "Hidden", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(entireRow, "Hidden", parms);
                    }
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region RangeClassクラス

        /// <summary>
        /// レンジコレクションクラスです。
        /// </summary>
        public class RangeClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.RangeClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="worksheet">ワークシートオブジェクト</param>
            /// <param name="range">セルの範囲</param>
            public RangeClass(object worksheet, string range)
            {
                this.range = null;
                try
                {
                    // Rangeオブジェクト取得
                    object[] parm = new object[2];
                    parm[0] = range;
                    parm[1] = Type.Missing;
                    this.range = ComHelper.GetObject(worksheet, "Range", parm);

                    // Cellsオブジェクト取得
                    cells = new CellsClass(this.range);

                    // EntireColumnオブジェクト取得
                    ecol = new EntireColumnClass(this.range);

                    // EntireRowオブジェクト取得
                    erow = new EntireRowClass(this.range);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~RangeClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Cellsオブジェクト解放
                if (cells != null)
                {
                    cells.Dispose();
                    cells = null;
                }

                // Rangeオブジェクト開放
                if (range != null)
                {
                    Marshal.FinalReleaseComObject(range);
                    range = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// Rangeオブジェクト
            /// </summary>
            private object range = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// レンジのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("レンジのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Cellsプロパティ

            private CellsClass cells = null;
            /// <summary>
            /// セルコレクションのインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("セルコレクションのインスタンスを参照します。")
            ]
            public CellsClass Cells
            {
                get
                {
                    return cells;
                }
            }

            #endregion

            #region EntireColumnプロパティ

            private EntireColumnClass ecol = null;
            /// <summary>
            /// 選択カラム全体のインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("選択カラム全体のインスタンスを参照します。")
            ]
            public EntireColumnClass EntireColumn
            {
                get
                {
                    return ecol;
                }
            }

            #endregion

            #region EntireRowプロパティ

            private EntireRowClass erow = null;
            /// <summary>
            /// 選択行全体のインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("選択行全体のインスタンスを参照します。")
            ]
            public EntireRowClass EntireRow
            {
                get
                {
                    return erow;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region WorksheetClassクラス

        /// <summary>
        /// ワークシートクラスです。
        /// </summary>
        public class WorksheetClass
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.WorksheetClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="worksheets">ワークシートコレクションオブジェクト</param>
            /// <param name="index">シートのインデックス</param>
            public WorksheetClass(object worksheets, int index)
            {
                Initialize(worksheets, index);
            }

            /// <summary>
            /// XControls.XExcelLink.WorksheetClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="worksheets">ワークシートコレクションオブジェクト</param>
            /// <param name="name">シート名</param>
            public WorksheetClass(object worksheets, string name)
            {
                Initialize(worksheets, name);
            }

            /// <summary>
            /// インスタンスを初期化します。
            /// </summary>
            /// <param name="worksheets">ワークシートコレクションオブジェクト</param>
            /// <param name="obj">シートを表すオブジェクト</param>
            private void Initialize(object worksheets, object obj)
            {
                try
                {
                    // Worksheetオブジェクト取得
                    object[] parm = new object[1];
                    parm[0] = obj;
                    worksheet = ComHelper.GetObject(worksheets, "Item", parm);

                    // Cellsオブジェクト取得
                    cells = new CellsClass(worksheet);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~WorksheetClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Cellsオブジェクト解放
                if (cells != null)
                {
                    cells.Dispose();
                    cells = null;
                }

                // Worksheetオブジェクト解放
                if (worksheet != null)
                {
                    Marshal.FinalReleaseComObject(worksheet);
                    worksheet = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// worksheetオブジェクト
            /// </summary>
            private object worksheet = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// ワークシートのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークシートのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Nameプロパティ

            /// <summary>
            /// ワークシートの名前を設定または取得します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークシートの名前を設定または取得します。")
            ]
            public string Name
            {
                get
                {
                    string nam = "";
                    if (!isFailed)
                    {
                        nam = ComHelper.GetObject(worksheet, "Name", null).ToString();
                    }
                    return nam;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parm = new object[1];
                        parm[0] = value;
                        ComHelper.SetObject(worksheet, "Name", parm);
                    }
                }
            }

            #endregion

            #region Cellsプロパティ

            private CellsClass cells = null;
            /// <summary>
            /// セルコレクションのインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("セルコレクションのインスタンスを参照します。")
            ]
            public CellsClass Cells
            {
                get
                {
                    return cells;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region Refメソッド

            /// <summary>
            /// ワークシートオブジェクトを参照します。
            /// </summary>
            public object Ref()
            {
                return worksheet;
            }

            #endregion

            #region GetRangeメソッド

            /// <summary>
            /// レンジのインスタンスを取得します。
            /// </summary>
            /// <param name="range">レンジ指定文字列</param>
            /// <returns>レンジのインスタンス</returns>
            public RangeClass GetRange(string range)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);

                // RangeClassインスタンス取得
                return new RangeClass(worksheet, range);
            }

            #endregion

            #region ReleaseRangeメソッド

            /// <summary>
            /// レンジのインスタンスを開放します。
            /// </summary>
            /// <param name="excel">RangeClassのインスタンス</param>
            public void ReleaseRange(ref RangeClass range)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);

                // RangeClassインスタンス開放
                if (range != null)
                {
                    range.Dispose();
                    range = null;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region WorksheetsClassクラス

        /// <summary>
        /// ワークシートコレクションクラスです。
        /// </summary>
        public class WorksheetsClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.WorksheetsClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="workbook">ワークブックオブジェクト</param>
            public WorksheetsClass(object workbook)
            {
                worksheets = null;
                try
                {
                    worksheets = ComHelper.GetObject(workbook, "Worksheets", null);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~WorksheetsClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Worksheetsオブジェクト開放
                if (worksheets != null)
                {
                    Marshal.FinalReleaseComObject(worksheets);
                    worksheets = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// Worksheetsオブジェクト
            /// </summary>
            private object worksheets = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// ワークシートコレクションのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークシートコレクションのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Countプロパティ

            /// <summary>
            /// ワークシートの数を返します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークシートの数を返します。"),

            ]
            public int Count
            {
                get
                {
                    int rt = 0;
                    if (!isFailed)
                    {
                        rt = int.Parse(ComHelper.GetObject(worksheets, "Count", null).ToString());
                    }
                    return rt;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region NewSheetメソッド

            /// <summary>
            /// 新しいワークシートを追加します。
            /// </summary>
            public void NewSheet()
            {
                ComHelper.InvokeObject(worksheets, "Add");
            }

            #endregion

            #region Deleteメソッド

            /// <summary>
            /// ワークシートを削除します。
            /// </summary>
            /// <param name="index">削除するワークシートの位置</param>
            public void Delete(int index)
            {
                WorksheetClass sheet = null;
                try
                {
                    sheet = GetSheetInstance(index);
                    ComHelper.InvokeObject(sheet.Ref(), "Delete");
                }
                finally
                {
                    if (sheet != null)
                    {
                        ReleaseSheetInstance(ref sheet);
                    }
                }
            }

            #endregion

            #region Copyメソッド

            /// <summary>
            /// ワークシートをコピーします。
            /// </summary>
            /// <param name="FromIndex">コピーするワークシートの位置</param>
            /// <param name="ToIndex">コピー先の位置</param>
            public void Copy(int FromIndex, int ToIndex)
            {
                WorksheetClass sheet1 = null;
                WorksheetClass sheet2 = null;
                try
                {
                    int last = Count;
                    int toidx = ToIndex;
                    bool isAfter = false;
                    if (ToIndex == last + 1)
                    {
                        toidx = last;
                        isAfter = true;
                    }
                    sheet1 = GetSheetInstance(FromIndex);
                    sheet2 = GetSheetInstance(toidx);
                    object[] parm = new object[2];
                    if (isAfter)
                    {
                        parm[0] = Type.Missing;
                        parm[1] = sheet2.Ref();
                    }
                    else
                    {
                        parm[0] = sheet2.Ref();
                        parm[1] = Type.Missing;
                    }
                    ComHelper.InvokeObject(sheet1.Ref(), "Copy", parm);
                }
                finally
                {
                    if (sheet1 != null)
                    {
                        ReleaseSheetInstance(ref sheet1);
                    }
                    if (sheet2 != null)
                    {
                        ReleaseSheetInstance(ref sheet2);
                    }
                }
            }

            #endregion

            #region Moveメソッド

            /// <summary>
            /// ワークシートを移動します。
            /// </summary>
            /// <param name="FromIndex">移動するワークシートの位置</param>
            /// <param name="ToIndex">移動先の位置</param>
            public void Move(int FromIndex, int ToIndex)
            {
                WorksheetClass sheet1 = null;
                WorksheetClass sheet2 = null;
                try
                {
                    bool isAfter = false;
                    if (FromIndex < ToIndex)
                    {
                        isAfter = true;
                    }
                    sheet1 = GetSheetInstance(FromIndex);
                    sheet2 = GetSheetInstance(ToIndex);
                    object[] parm = new object[2];
                    if (isAfter)
                    {
                        parm[0] = Type.Missing;
                        parm[1] = sheet2.Ref();
                    }
                    else
                    {
                        parm[0] = sheet2.Ref();
                        parm[1] = Type.Missing;
                    }
                    ComHelper.InvokeObject(sheet1.Ref(), "Move", parm);
                }
                finally
                {
                    if (sheet1 != null)
                    {
                        ReleaseSheetInstance(ref sheet1);
                    }
                    if (sheet2 != null)
                    {
                        ReleaseSheetInstance(ref sheet2);
                    }
                }
            }

            #endregion

            #region GetSheetInstanceメソッド

            /// <summary>
            /// ワークシートのインスタンスを取得します。
            /// </summary>
            /// <param name="index">ワークシートのインデックス</param>
            /// <returns>ワークシートのインスタンスを取得します。</returns>
            public WorksheetClass GetSheetInstance(int index)
            {
                // WorksheetClassインスタンス取得
                return new WorksheetClass(worksheets, index);
            }

            /// <summary>
            /// ワークシートのインスタンスを取得します。
            /// </summary>
            /// <param name="name">ワークシート名</param>
            /// <returns>ワークシートのインスタンスを取得します。</returns>
            public WorksheetClass GetSheetInstance(string name)
            {
                // WorksheetClassインスタンス取得
                return new WorksheetClass(worksheets, name);
            }

            #endregion

            #region ReleaseSheetInstanceメソッド

            /// <summary>
            /// ワークシートのインスタンスを開放します。
            /// </summary>
            /// <param name="excel">ワークシートのインスタンス</param>
            public void ReleaseSheetInstance(ref WorksheetClass worksheet)
            {
                // WorksheetClassインスタンス開放
                if (worksheet != null)
                {
                    worksheet.Dispose();
                    worksheet = null;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region WorkbookClassクラス

        /// <summary>
        /// ワークブッククラスです。
        /// </summary>
        public class WorkbookClass
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.WorkbookClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="workbooks">ワークブックコレクションオブジェクト</param>
            /// <param name="index">ワークブックのインデックス</param>
            public WorkbookClass(object workbooks, int index)
            {
                try
                {
                    // Workbookオブジェクト取得
                    object[] parm = new object[1];
                    parm[0] = index;
                    workbook = ComHelper.GetObject(workbooks, "Item", parm);

                    // Worksheetsオブジェクト取得
                    worksheets = new WorksheetsClass(workbook);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~WorkbookClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                if (workbook != null)
                {
                    Marshal.FinalReleaseComObject(workbook);
                    workbook = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// workbookオブジェクト
            /// </summary>
            private object workbook = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// ワークブックのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークブックのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が true の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が true の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Nameプロパティ

            /// <summary>
            /// ワークブックの名前を取得します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークブックの名前を取得します。")
            ]
            public string Name
            {
                get
                {
                    string nam = "";
                    if (!isFailed)
                    {
                        nam = ComHelper.GetObject(workbook, "Name", null).ToString();
                    }
                    return nam;
                }
            }

            #endregion

            #region FullNameプロパティ

            /// <summary>
            /// ワークブックのパスを含む名前を取得します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークブックのパスを含む名前を取得します。")
            ]
            public string FullName
            {
                get
                {
                    string nam = "";
                    if (!isFailed)
                    {
                        nam = ComHelper.GetObject(workbook, "FullName", null).ToString();
                    }
                    return nam;
                }
            }

            #endregion

            #region Pathプロパティ

            /// <summary>
            /// ワークブックのパスを取得します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークブックのパスを取得します。")
            ]
            public string Path
            {
                get
                {
                    string nam = "";
                    if (!isFailed)
                    {
                        nam = ComHelper.GetObject(workbook, "Path", null).ToString();
                    }
                    return nam;
                }
            }

            #endregion

            #region Savedプロパティ

            /// <summary>
            /// ワークブックが保存済みかを設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("ワークブックが保存済みかを設定または取得します。")
            ]
            public bool Saved
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(workbook, "Saved", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(workbook, "Saved", parms);
                    }
                }
            }

            #endregion

            #region Authorプロパティ

            /// <summary>
            /// 作成者を設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(false),
                Description("作成者を設定または取得します。")
            ]
            public string Author
            {
                get
                {
                    string rt = "";
                    if (!isFailed)
                    {
                        rt = ComHelper.GetObject(workbook, "Author", null).ToString();
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(workbook, "Author", parms);
                    }
                }
            }

            #endregion

            #region Commentsプロパティ

            /// <summary>
            /// コメントを設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(false),
                Description("コメントを設定または取得します。"),
                EditorAttribute(
                    typeof(System.ComponentModel.Design.MultilineStringEditor),
                    typeof(System.Drawing.Design.UITypeEditor))
            ]
            public string Comments
            {
                get
                {
                    string rt = "";
                    if (!isFailed)
                    {
                        rt = ComHelper.GetObject(workbook, "Comments", null).ToString();
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(workbook, "Comments", parms);
                    }
                }
            }

            #endregion

            #region Titleプロパティ

            /// <summary>
            /// タイトルを設定または取得します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(false),
                Description("タイトルを設定または取得します。")
            ]
            public string Title
            {
                get
                {
                    string rt = "";
                    if (!isFailed)
                    {
                        rt = ComHelper.GetObject(workbook, "Title", null).ToString();
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(workbook, "Title", parms);
                    }
                }
            }

            #endregion

            #region Worksheetsプロパティ

            private WorksheetsClass worksheets = null;
            /// <summary>
            /// ワークシートコレクションのインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークシートコレクションのインスタンスを参照します。")
            ]
            public WorksheetsClass Worksheets
            {
                get
                {
                    return worksheets;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region SaveAsメソッド

            /// <summary>
            /// 名前を付けてワークブックを保存します。
            /// </summary>
            /// <param name="filename"></param>
            public void SaveAs(string filename)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                object[] parm = new object[1];
                parm[0] = filename;
                ComHelper.InvokeObject(workbook, "SaveAs", parm);
            }

            #endregion

            #region Saveメソッド

            /// <summary>
            /// ワークブックを保存します。
            /// </summary>
            public void Save()
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                ComHelper.InvokeObject(workbook, "Save");
            }

            #endregion

            #region Closeメソッド

            /// <summary>
            /// ワークブックを閉じます。
            /// </summary>
            public void Close()
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                ComHelper.InvokeObject(workbook, "Close");
            }

            #endregion

            #endregion
        }

        #endregion

        #region WorkbooksClassクラス

        /// <summary>
        /// ワークブックコレクションクラスです。
        /// </summary>
        public class WorkbooksClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.WorkbooksClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="excel">Excelアプリケーションオブジェクト</param>
            public WorkbooksClass(object excel)
            {
                workbooks = null;
                try
                {
                    workbooks = ComHelper.GetObject(excel, "Workbooks", null);
                }
                catch (Exception es)
                {
                    // 失敗
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// インスタンスを破棄します。
            /// </summary>
            ~WorkbooksClass()
            {
                Dispose();
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Workbooksオブジェクト開放
                if (workbooks != null)
                {
                    Marshal.FinalReleaseComObject(workbooks);
                    workbooks = null;
                }
            }

            #endregion

            #region 変数

            /// <summary>
            /// Workbooksオブジェクト
            /// </summary>
            private object workbooks = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// ワークブックコレクションのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークブックコレクションのインスタンス取得に失敗したかを返します。")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が True の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が True の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Countプロパティ

            /// <summary>
            /// ワークブックの数を返します。
            /// </summary>
            [
                Category("動作"),
                Description("ワークブックの数を返します。"),

            ]
            public int Count
            {
                get
                {
                    int rt = 0;
                    if (!isFailed)
                    {
                        rt = int.Parse(ComHelper.GetObject(workbooks, "Count", null).ToString());
                    }
                    return rt;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region OpenBookメソッド

            /// <summary>
            /// Excelブックをオープンします。
            /// </summary>
            /// <param name="filename">Excelブックファイル名</param>
            public void OpenBook(string filename)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);

                Uri appdir = new Uri(Application.StartupPath + "\\");
                Uri reldir = new Uri(appdir, filename);
                string file = System.Uri.UnescapeDataString(appdir.MakeRelativeUri(reldir).ToString());
                file = file.Replace("/", "\\");
                Uri fulldir = new Uri(appdir, System.Environment.ExpandEnvironmentVariables(file));
                
                int pcount = 15;
                if (ComHelper.ExcelVersion < 10F) pcount = 12;    // Excel 2000対応
                object[] parm = new object[pcount];
                parm[0] = fulldir.LocalPath;
                for (int i = 1; i < pcount; i++)
                {
                    parm[i] = Type.Missing;
                }
                ComHelper.InvokeObject(workbooks, "Open", parm);
            }

            #endregion

            #region NewBookメソッド

            /// <summary>
            /// Excelブックを新規作成します。
            /// </summary>
            public void NewBook()
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                try
                {
                    ComHelper.InvokeObject(workbooks, "Add");
                }
                catch (Exception es)
                {
                    throw es;
                }
            }

            #endregion

            #region GetBookInstanceメソッド

            /// <summary>
            /// ワークブックのインスタンスを取得します。
            /// </summary>
            /// <param name="index">ワークブックのインデックス</param>
            /// <returns>ワークブックのインスタンス</returns>
            public WorkbookClass GetBookInstance(int index)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);

                // WorkbookClassインスタンス取得
                return new WorkbookClass(workbooks, index);
            }

            #endregion

            #region ReleaseBookInstanceメソッド

            /// <summary>
            /// ワークブックのインスタンスを開放します。
            /// </summary>
            /// <param name="excel">ワークブックのインスタンス</param>
            public void ReleaseBookInstance(ref WorkbookClass workbook)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);

                // WorkbookClassインスタンス開放
                if (workbook != null)
                {
                    workbook.Dispose();
                    workbook = null;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region ApplicationClassクラス

        /// <summary>
        /// Excelアプリケーションクラスです。
        /// </summary>
        public class ApplicationClass : IDisposable
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.XExcelLink.ApplicationClass クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ApplicationClass()
            {
                excel = null;
                try
                {
                    // Excelアプリケーション取得
                    Type classType = Type.GetTypeFromProgID("Excel.Application");
                    if (classType == null)
                    {
                        throw new ApplicationException("Excelアプリケーションがインストールされていません。");
                    }
                    excel = Activator.CreateInstance(classType);
                    ComHelper.ExcelVersion = float.Parse(ComHelper.GetObject(excel, "Version", null).ToString());
                    IntPtr hwnd = (IntPtr)uint.Parse(ComHelper.GetObject(excel, "Hwnd", null).ToString());
                    GetWindowThreadProcessId(hwnd, out processId); 

                    // Workbooksオブジェクト取得
                    workbooks = new WorkbooksClass(excel);
                }
                catch (Exception es)
                {
                    // Excelが見つからない
                    isFailed = true;
                    exceptionMessage = es.Message;
                }
            }

            /// <summary>
            /// リソースを開放します。
            /// </summary>
            public void Dispose()
            {
                // Workbooksオブジェクト解放
                if (workbooks != null)
                {
                    workbooks.Dispose();
                    workbooks = null;
                }

                // Excelアプリインスタンス開放
                if (excel != null)
                {
                    try
                    {
                        ComHelper.InvokeObject(excel, "Quit");
                    }
                    catch
                    {
                    }
                    Marshal.FinalReleaseComObject(excel);
                    excel = null;
                }
            }

            #endregion

            #region WIN32API

            [DllImport("user32.dll", SetLastError = true)]
            private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            private const int GWL_EXSTYLE = -20;
            private const int WS_EX_TOOLWINDOW = 128;

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            #endregion

            #region 変数

            /// <summary>
            /// Excelアプリケーションオブジェクト
            /// </summary>
            private object excel = null;

            #endregion

            #region プロパティ

            #region IsFailedプロパティ

            private bool isFailed = false;
            /// <summary>
            /// Excelアプリケーションのインスタンス取得に失敗したかを返します。
            /// </summary>
            [
                Category("情報"),
                Description("Excelアプリケーションのインスタンス取得に失敗したかを返します")
            ]
            public bool IsFailed
            {
                get
                {
                    return isFailed;
                }
            }

            #endregion

            #region ExceptionMessageプロパティ

            private string exceptionMessage = "";
            /// <summary>
            /// IsFailed が True の時、発生した例外メッセージを返します。
            /// </summary>
            [
                Category("情報"),
                Description("IsFailed が True の時、発生した例外メッセージを返します。")
            ]
            public string ExceptionMessage
            {
                get
                {
                    return exceptionMessage;
                }
            }

            #endregion

            #region Versionプロパティ

            /// <summary>
            /// Excelのバージョンを返します。
            /// </summary>
            [
                Category("情報"),
                Description("Excelのバージョンを返します。")
            ]
            public string Version
            {
                get
                {
                    string ver = "";
                    if (!isFailed)
                    {
                        ver = ComHelper.GetObject(excel, "Version", null).ToString();
                    }
                    return ver;
                }
            }

            #endregion

            #region ScreenUpdatingプロパティ

            /// <summary>
            /// 各メソッド呼び出し後に画面を更新するかを設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("各メソッド呼び出し後に画面を更新するかを設定します。")
            ]
            public bool ScreenUpdating
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(excel, "ScreenUpdating", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "ScreenUpdating", parms);
                    }
                }
            }

            #endregion

            #region SheetsInNewWorkbookプロパティ

            /// <summary>
            /// 新規ブックに自動的に挿入されるシートの数を設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(typeof(long), "3"),
                Description("新規ブックに自動的に挿入されるシートの数を設定します。")
            ]
            public long SheetsInNewWorkbook
            {
                get
                {
                    long rt = 0;
                    if (!isFailed)
                    {
                        rt = long.Parse(ComHelper.GetObject(excel, "SheetsInNewWorkbook", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "SheetsInNewWorkbook", parms);
                    }
                }
            }

            #endregion

            #region DisplayAlertsプロパティ

            /// <summary>
            /// シート削除や保存時などの確認メッセージを有効にするか無効にするかを設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("シート削除や保存時などの確認メッセージを有効にするか無効にするかを設定します。")
            ]
            public bool DisplayAlerts
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(excel, "DisplayAlerts", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "DisplayAlerts", parms);
                    }
                }
            }

            #endregion

            #region DisplayFullScreenプロパティ

            /// <summary>
            /// フルスクリーンモードで動作するかを設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(false),
                Description("フルスクリーンモードで動作するかを設定します。")
            ]
            public bool DisplayFullScreen
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(excel, "DisplayFullScreen", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "DisplayFullScreen", parms);
                    }
                }
            }

            #endregion

            #region Visibleプロパティ

            /// <summary>
            /// Excelアプリケーションを表示するかどうかを設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(false),
                Description("Excelアプリケーションを表示するかどうかを設定します。")
            ]
            public bool Visible
            {
                get
                {
                    bool rt = false;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(excel, "Visible", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "Visible", parms);
                    }
                }
            }

            #endregion

            #region Workbooksプロパティ

            private WorkbooksClass workbooks = null;
            /// <summary>
            /// ワークブックコレクションのインスタンスを参照します。
            /// </summary>
            [
                Category("情報"),
                Description("ワークブックコレクションのインスタンスを参照します。")
            ]
            public WorkbooksClass Workbooks
            {
                get
                {
                    return workbooks;
                }
            }

            #endregion

            #region ProcessIdプロパティ

            private uint processId = 0;
            /// <summary>
            /// ExcelアプリケーションのプロセスIDを返します。
            /// </summary>
            [
                Category("情報"),
                Description("ExcelアプリケーションのプロセスIDを返します。")
            ]
            public uint ProcessID
            {
                get
                {
                    return processId;
                }
            }

            #endregion

            #region WindowStateプロパティ

            /// <summary>
            /// Microsoft Excel ウィンドウの状態を設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(typeof(FormWindowState), "Normal"),
                Description("Microsoft Excel ウィンドウの状態を設定します。")
            ]
            public FormWindowState WindowState
            {
                get
                {
                    FormWindowState rt = FormWindowState.Normal;
                    if (!isFailed)
                    {
                        int ws = int.Parse(ComHelper.GetObject(excel, "WindowState", null).ToString());
                        switch (ws)
                        {
                            case -4137:
                                rt = FormWindowState.Maximized;
                                break;
                            case -4140:
                                rt = FormWindowState.Minimized;
                                break;
                            case -4143:
                                rt = FormWindowState.Normal;
                                break;
                        }
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        switch (value)
                        {
                            case FormWindowState.Maximized:
                                parms[0] = -4137;
                                break;
                            case FormWindowState.Minimized:
                                parms[0] = -4140;
                                break;
                            case FormWindowState.Normal:
                                parms[0] = -4143;
                                break;
                        }
                        ComHelper.SetObject(excel, "WindowState", parms);
                    }
                }
            }

            #endregion

            #region Leftプロパティ

            /// <summary>
            /// 物理画面の左端から Microsoft Excel ウィンドウの左端までのポイント単位の距離です。
            /// </summary>
            [
                Category("動作"),
                Description("物理画面の左端から Microsoft Excel ウィンドウの左端までのポイント単位の距離です。")
            ]
            public double Left
            {
                get
                {
                    double rt = 0;
                    if (!isFailed)
                    {
                        rt = double.Parse(ComHelper.GetObject(excel, "Left", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "Left", parms);
                    }
                }
            }

            #endregion

            #region Topプロパティ

            /// <summary>
            /// 物理画面の上端から、Microsoft Excel のウィンドウの上端までの距離です。
            /// </summary>
            [
                Category("動作"),
                Description("物理画面の上端から、Microsoft Excel のウィンドウの上端までの距離です。")
            ]
            public double Top
            {
                get
                {
                    double rt = 0;
                    if (!isFailed)
                    {
                        rt = double.Parse(ComHelper.GetObject(excel, "Top", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "Top", parms);
                    }
                }
            }

            #endregion

            #region Widthプロパティ

            /// <summary>
            /// アプリケーション ウィンドウの左端から右端までのポイント単位の距離です。
            /// </summary>
            [
                Category("動作"),
                Description("アプリケーション ウィンドウの左端から右端までのポイント単位の距離です。")
            ]
            public double Width
            {
                get
                {
                    double rt = 0;
                    if (!isFailed)
                    {
                        rt = double.Parse(ComHelper.GetObject(excel, "Width", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "Width", parms);
                    }
                }
            }

            #endregion

            #region Heightプロパティ

            /// <summary>
            /// アプリケーションのメイン ウィンドウの高さをポイントで表します。
            /// </summary>
            [
                Category("動作"),
                Description("アプリケーションのメイン ウィンドウの高さをポイントで表します。")
            ]
            public double Height
            {
                get
                {
                    double rt = 0;
                    if (!isFailed)
                    {
                        rt = double.Parse(ComHelper.GetObject(excel, "Height", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "Height", parms);
                    }
                }
            }

            #endregion

            #region Hwndプロパティ

            /// <summary>
            /// Microsoft Excel ウィンドウの最上位レベルのウィンドウ ハンドルを示す整数型 (Integer) の値を取得します。
            /// </summary>
            [
                Category("動作"),
                Description("Microsoft Excel ウィンドウの最上位レベルのウィンドウ ハンドルを示す整数型 (Integer) の値を取得します。")
            ]
            public IntPtr Hwnd
            {
                get
                {
                    IntPtr rt = (IntPtr)0;
                    if (!isFailed)
                    {
                        rt = (IntPtr)int.Parse(ComHelper.GetObject(excel, "Hwnd", null).ToString());
                    }
                    return rt;
                }
            }

            #endregion

            #region ShowInTaskbarプロパティ

            /// <summary>
            /// Windows タスクバーにアプリケーションを表示するかを設定します。再表示するまでタスクバーの状態は変わりません。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("Windows タスクバーにアプリケーションを表示するかを設定します。再表示するまでタスクバーの状態は変わりません。")
            ]
            public bool ShowInTaskbar
            {
                get
                {
                    bool rt = true;
                    if (!isFailed)
                    {
                        IntPtr hwnd = Hwnd;
                        int exstyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                        rt = (exstyle == (exstyle & ~WS_EX_TOOLWINDOW));
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        IntPtr hwnd = Hwnd;
                        int exstyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                        if (value)
                        {
                            exstyle = (exstyle & ~WS_EX_TOOLWINDOW);
                        }
                        else
                        {
                            exstyle = exstyle | WS_EX_TOOLWINDOW;
                        }
                        SetWindowLong(hwnd, GWL_EXSTYLE, exstyle);
                    }
                }
            }

            #endregion

            #region ShowWindowsInTaskbarプロパティ

            /// <summary>
            /// 開かれている各ブックごとに別々の Windows タスク バー ボタンを表示するかを設定します。
            /// </summary>
            [
                Category("動作"),
                DefaultValue(true),
                Description("開かれている各ブックごとに別々の Windows タスク バー ボタンを表示するかを設定します。")
            ]
            public bool ShowWindowsInTaskbar
            {
                get
                {
                    bool rt = true;
                    if (!isFailed)
                    {
                        rt = bool.Parse(ComHelper.GetObject(excel, "ShowWindowsInTaskbar", null).ToString());
                    }
                    return rt;
                }
                set
                {
                    if (!isFailed)
                    {
                        object[] parms = new object[1];
                        parms[0] = value;
                        ComHelper.SetObject(excel, "ShowWindowsInTaskbar", parms);
                    }
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region Runメソッド

            /// <summary>
            /// マクロを実行します。
            /// </summary>
            /// <param name="book">ブック名</param>
            /// <param name="module">シートまたはモジュール名</param>
            /// <param name="macro">マクロ名</param>
            /// <param name="param">引数配列[1-30]</param>
            /// <param name="refparam">書き換えられる引数配列[1-30]</param>
            /// <remarks>
            /// マクロ側の引数は、値渡し引数→参照渡し引数の順にまとめて定義してください。
            /// 値渡しの引数がparam配列、参照渡しの引数がrefparam配列に対応します。
            /// </remarks>
            public void Run(string book, string module, string macro, object[] param, ref object[] refparam)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                object[] parm = new object[31];
                int paramLen = 0;
                if (param != null) paramLen = param.Length;
                int refparamLen = 0;
                if (refparam != null) refparamLen = refparam.Length;
                ParameterModifier modp = new ParameterModifier(31);
                parm[0] = "'" + book + "'!" + module + "." + macro;
                modp[0] = false;
                for (int i = 0; i < 30; i++)
                {
                    if (i < paramLen)
                    {
                        parm[i + 1] = param[i];
                        modp[i + 1] = false;
                    }
                    else if (i < paramLen + refparamLen)
                    {
                        parm[i + 1] = refparam[i - paramLen];
                        modp[i + 1] = true;
                    }
                    else
                    {
                        parm[i + 1] = Type.Missing;
                        modp[i + 1] = false;
                    }
                }
                ParameterModifier[] mods = { modp };
                ComHelper.InvokeObject(excel, "Run", ref parm, mods);
                for (int i = 0; i < 30; i++)
                {
                    if (i < paramLen)
                    {
                    }
                    else if (i < paramLen + refparamLen)
                    {
                        refparam[i - paramLen] = parm[i + 1];
                    }
                    else
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// マクロを実行します。
            /// </summary>
            /// <param name="book">ブック名</param>
            /// <param name="module">シートまたはモジュール名</param>
            /// <param name="macro">マクロ名</param>
            /// <param name="param">引数配列[1-30]</param>
            public void Run(string book, string module, string macro, object[] param)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                object[] parm = new object[31];
                parm[0] = "'" + book + "'!" + module + "." + macro;
                for (int i = 0; i < 30; i++)
                {
                    if (i < param.Length)
                    {
                        parm[i + 1] = param[i];
                    }
                    else
                    {
                        parm[i + 1] = Type.Missing;
                    }
                }
                ComHelper.InvokeObject(excel, "Run", parm);
            }

            /// <summary>
            /// マクロを実行します。
            /// </summary>
            /// <param name="book">ブック名</param>
            /// <param name="module">シートまたはモジュール名</param>
            /// <param name="macro">マクロ名</param>
            public void Run(string book, string module, string macro)
            {
                if (isFailed) throw new ApplicationException(exceptionMessage);
                object[] parm = new object[1];
                parm[0] = Type.Missing;
                Run(book, module, macro, parm);
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.ExcelLink クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ExcelLink()
        {
        }

        /// <summary>
        /// インスタンスを破棄します。
        /// </summary>
        ~ExcelLink()
        {
            Dispose();
        }

        /// <summary>
        /// リソースを開放します。
        /// </summary>
        new void Dispose()
        {
            if (excelapp != null)
            {
                excelapp.Dispose();
                excelapp = null;
            }
        }

        #endregion

        #region メソッド

        #region GetInstanceメソッド

        private ApplicationClass excelapp = null;
        /// <summary>
        /// Excelアプリケーションのインスタンスを返します。
        /// </summary>
        /// <returns>Excelアプリケーションのインスタンス</returns>
        public ApplicationClass GetInstance()
        {
            // Excelアプリインスタンス取得
            if (excelapp == null) excelapp = new ApplicationClass();
            return excelapp;
        }

        #endregion

        #region ReleaseInstanceメソッド

        /// <summary>
        /// Excelアプリケーションをクローズします。
        /// </summary>
        /// <param name="excel">Excelアプリケーションのインスタンス</param>
        public void ReleaseInstance(ref ApplicationClass excel)
        {
            // Excelアプリインスタンス開放
            if (excel != null)
            {
                excel.Dispose();
                excel = null;
                excelapp = null;
            }
        }

        #endregion

        #endregion
    }
}
