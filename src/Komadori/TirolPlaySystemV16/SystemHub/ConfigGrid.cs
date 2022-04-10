using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace XControls.UI
{
    /// <summary>
    /// PropertyGridの設定記録用拡張版コントロールです。
    /// </summary>
    /// <remarks>
    /// 次のアセンブリへの参照を追加してください。
    ///     System.Design
    /// </remarks>
    [DebuggerNonUserCode, Designer(typeof(ConfigGridDesigner))]
    public class ConfigGrid : PropertyGrid
    {
        #region インナークラス

        #region ItemSettingクラス

        /// <summary>
        /// ConfigGridで扱う項目の設定を扱います。
        /// </summary>
        [Designer(typeof(ItemSettingDesigner))]
        public class ItemSetting
        {
            #region インナークラス

            #region ItemSettingDesignerクラス

            /// <summary>
            /// ItemSetting用にデザイナをカスタマイズします。
            /// </summary>
            public class ItemSettingDesigner : System.Windows.Forms.Design.ControlDesigner
            {
                #region 構築・破棄

                /// <summary>
                /// XControls.ConfigGrid.ItemSetting.ItemSettingDesigner クラスの新しいインスタンスを初期化します。
                /// </summary>
                public ItemSettingDesigner()
                {
                }

                #endregion

                #region メソッド

                #region PostFilterPropertiesメソッド

                protected override void PostFilterProperties(IDictionary properties)
                {
                    // フィルタリングするプロパティ
                    properties.Remove("UpdateTimer");

                    base.PostFilterProperties(properties);
                }

                #endregion

                #endregion
            }

            #endregion

            #endregion

            #region プロパティ

            #region Categoryプロパティ

            private string category = "";
            /// <summary>
            /// この項目のカテゴリを参照したり設定します。
            /// </summary>
            [
                Category("設定"),
                DefaultValue("General"),
                Description("この項目のカテゴリを参照したり設定します。")
            ]
            public string Category
            {
                get
                {
                    return category;
                }
                set
                {
                    category = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region DisplayCategoryプロパティ

            private string displayCategory = "";
            /// <summary>
            /// この項目の表示用のカテゴリを参照したり設定します。省略時はCategoryの内容が使われます。
            /// </summary>
            [
                Category("設定"),
                DefaultValue("全般"),
                Description("この項目の表示用のカテゴリを参照したり設定します。省略時はCategoryの内容が使われます。")
            ]
            public string DisplayCategory
            {
                get
                {
                    return displayCategory;
                }
                set
                {
                    displayCategory = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region Nameプロパティ

            private string name = "";
            /// <summary>
            /// この項目の定義名を参照したり設定します。
            /// </summary>
            [
                Category("設定"),
                DefaultValue(""),
                Description("この項目の定義名を参照したり設定します。")
            ]
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region DisplayNameプロパティ

            private string displayName = "";
            /// <summary>
            /// この項目の表示用の名前を参照したり設定します。省略時はNameの内容が使われます。
            /// </summary>
            [
                Category("設定"),
                DefaultValue(""),
                Description("この項目の表示用の名前を参照したり設定します。省略時はNameの内容が使われます。")
            ]
            public string DisplayName
            {
                get
                {
                    return displayName;
                }
                set
                {
                    displayName = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region ValueTypeプロパティ

            private string valueType = "";
            /// <summary>
            /// この項目のタイプを参照したり設定します。特殊なタイプとして"Folder","FullFolder","Path","FullPath","IPAddress","Password"が使用できます。
            /// </summary>
            [
                Category("設定"),
                DefaultValue("string"),
                Description("この項目のタイプを参照したり設定します。特殊なタイプとして\"Folder\",\"FullFolder\",\"Path\",\"FullPath\",\"IPAddress\",\"Password\"が使用できます。")
            ]
            public string ValueType
            {
                get
                {
                    return valueType;
                }
                set
                {
                    valueType = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region DefaultValueプロパティ

            private string defaultValue = "";
            /// <summary>
            /// この項目の初期値を参照したり設定します。
            /// </summary>
            [
                Category("設定"),
                DefaultValue(""),
                Description("この項目の初期値を参照したり設定します。"),
                EditorAttribute(
                    typeof(MultilineStringEditor),
                    typeof(UITypeEditor))
            ]
            public string DefaultValue
            {
                get
                {
                    return defaultValue;
                }
                set
                {
                    defaultValue = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region Descriptionプロパティ

            private string description = "";
            /// <summary>
            /// この項目の説明を参照したり設定します。
            /// </summary>
            [
                Category("設定"),
                DefaultValue(""),
                Description("この項目の説明を参照したり設定します。")
            ]
            public string Description
            {
                get
                {
                    return description;
                }
                set
                {
                    description = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region Formatプロパティ

            private string format = "";
            /// <summary>
            /// この項目の書式を参照したり設定します。
            /// </summary>
            [
                Category("設定"),
                DefaultValue(""),
                Description("この項目の書式を参照したり設定します。")
            ]
            public string Format
            {
                get
                {
                    return format;
                }
                set
                {
                    format = value;
                    if (updateTimer != null) updateTimer.Enabled = true;
                }
            }

            #endregion

            #region UpdateTimerプロパティ

            private Timer updateTimer = null;
            /// <summary>
            /// 画面描画に使用するタイマーコントロールを参照または設定します。
            /// </summary>
            [Browsable(false)]
            public Timer UpdateTimer
            {
                get
                {
                    return updateTimer;
                }
                set
                {
                    updateTimer = value;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region ItemSettingCollectionクラス

        /// <summary>
        /// ItemSettingクラスインスタンスのコレクションを扱います。
        /// </summary>
        public class ItemSettingCollection : CollectionBase
        {
            #region プロパティ

            #region UpdateTimerプロパティ

            private Timer updateTimer = null;
            /// <summary>
            /// 画面描画に使用するタイマーコントロールを参照または設定します。
            /// </summary>
            public Timer UpdateTimer
            {
                get
                {
                    return updateTimer;
                }
                set
                {
                    updateTimer = value;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region this[]メソッド

            /// <summary>
            /// ItemSettingコレクションへのアクセスを提供します。
            /// </summary>
            /// <param name="index">コレクション内のインデックス</param>
            /// <returns>項目の設定を保持するItemSettingのインスタンス</returns>
            public ItemSetting this[int index]
            {
                get
                {
                    return ((ItemSetting)List[index]);
                }
                set
                {
                    List[index] = value;
                }
            }

            #endregion

            #region Addメソッド

            /// <summary>
            /// ItemSettingコレクションにItemSettingインスタンスを追加します。
            /// </summary>
            /// <param name="value">追加するItemSettingのインスタンス</param>
            /// <returns>追加された位置の0から始まるインデックス</returns>
            public int Add(ItemSetting value)
            {
                value.UpdateTimer = updateTimer;
                int index = List.Add(value);
                updateTimer.Enabled = true;
                return (index);
            }

            #endregion

            #region IndexOfメソッド

            /// <summary>
            /// ItemSettingコレクション内のインスタンスのインデックスを取得します。
            /// </summary>
            /// <param name="value">検索するItemSettingのインスタンス</param>
            /// <returns>見つかった位置の0から始まるインデックス</returns>
            public int IndexOf(ItemSetting value)
            {
                return (List.IndexOf(value));
            }

            #endregion

            #region Insertメソッド

            /// <summary>
            /// ItemSettingコレクションにインスタンスを挿入します。
            /// </summary>
            /// <param name="index">挿入する位置の0から始まるインデックス</param>
            /// <param name="value">項目の設定を保持するItemSettingのインスタンス</param>
            public void Insert(int index, ItemSetting value)
            {
                value.UpdateTimer = updateTimer;
                List.Insert(index, value);
                updateTimer.Enabled = true;
            }

            #endregion

            #region Removeメソッド

            /// <summary>
            ///  ItemSettingコレクションからインスタンスを削除します。
            /// </summary>
            /// <param name="value">削除するItemSettingのインスタンス</param>
            public void Remove(ItemSetting value)
            {
                List.Remove(value);
                updateTimer.Enabled = true;
            }

            #endregion

            #region Contains

            /// <summary>
            ///  ItemSettingコレクションにインスタンスが存在しているかを返します。
            /// </summary>
            /// <param name="value">検索するItemSettingのインスタンス</param>
            /// <returns>存在していたらtrueを返す</returns>
            public bool Contains(ItemSetting value)
            {
                return (List.Contains(value));
            }

            #endregion

            #region FindSettingメソッド

            /// <summary>
            /// ItemSettingコレクションから指定したカテゴリ名・項目名を持つインスタンスを取得します。
            /// </summary>
            /// <param name="category">カテゴリ名</param>
            /// <param name="name">項目名</param>
            /// <returns>見つかったItemSettingのインスタンス(見つからない場合はnullを返す)</returns>
            public ItemSetting FindSetting(string category, string name)
            {
                ItemSetting find = null;
                for (int i = 0; i < List.Count; i++)
                {
                    ItemSetting iset = (ItemSetting)List[i];
                    if (iset.Category.Equals(category) && iset.Name.Equals(name))
                    {
                        find = iset;
                        break;
                    }
                }
                return find;
            }

            #endregion

            #endregion

            #region イベント

            #region OnClearCompleteイベント

            protected override void OnClearComplete()
            {
                if (updateTimer != null)
                {
                    updateTimer.Enabled = false;
                    updateTimer.Enabled = true;
                }
            }

            #endregion

            #region OnInsertCompleteイベント

            protected override void OnInsertComplete(int index, Object value)
            {
                try
                {
                    CheckValue(index, ref value);
                    if (updateTimer != null)
                    {
                        updateTimer.Enabled = false;
                        updateTimer.Enabled = true;
                    }
                }
                catch (Exception es)
                {
                    throw es;
                }
            }

            #endregion

            #region OnRemoveCompleteイベント

            protected override void OnRemoveComplete(int index, Object value)
            {
                if (updateTimer != null)
                {
                    updateTimer.Enabled = false;
                    updateTimer.Enabled = true;
                }
            }

            #endregion

            #region OnSetCompleteイベント

            protected override void OnSetComplete(int index, Object oldValue, Object newValue)
            {
                try
                {
                    CheckValue(index, ref newValue);
                    if (updateTimer != null)
                    {
                        updateTimer.Enabled = false;
                        updateTimer.Enabled = true;
                    }
                }
                catch (Exception es)
                {
                    throw es;
                }
            }

            #endregion

            #region OnValidateイベント

            protected override void OnValidate(Object value)
            {
                if (value.GetType() != typeof(ItemSetting))
                {
                    throw new ArgumentException("タイプがItemSettingではありません。", "value");
                }
            }

            #endregion

            #endregion

            #region 内部処理

            #region カテゴリ・項目名存在チェック (FindName)

            private bool FindName(int index, string category, string name)
            {
                bool find = false;
                for (int i = 0; i < List.Count; i++)
                {
                    if (i != index)
                    {
                        ItemSetting iset = (ItemSetting)List[i];
                        if (iset.Category.Equals(category) && iset.Name.Equals(name))
                        {
                            find = true;
                            break;
                        }
                    }
                }
                return find;
            }

            #endregion

            #region 値チェック (CheckValue)

            private void CheckValue(int index, ref Object value)
            {
                ItemSetting iset = (ItemSetting)value;

                // カテゴリチェック
                if (iset.Category.Equals(""))
                {
                    iset.Category = "General";
                }

                // 項目名チェック
                if (iset.Name.Equals(""))
                {
                    for (int i = 1; ; i++)
                    {
                        string nam = "Item" + i.ToString();
                        if (FindName(index, iset.Category, nam) == false)
                        {
                            iset.Name = nam;
                            break;
                        }
                    }
                }
                else
                {
                    if (FindName(index, iset.Category, iset.Name) == true)
                    {
                        throw new ArgumentException("CategoryとNameの組み合わせはユニークである必要があります。",
                            "Category=" + iset.Category + ", Name=" + iset.Name);
                    }
                }

                // データ型チェック
                Type tp = TypeFromString(iset.ValueType);
                if (tp == null)
                {
                    throw new ArgumentException("ValueTypeが無効です。", "ValueType=" + iset.ValueType);
                }

                // IPAddressチェック
                if (iset.ValueType == "IPAddress")
                {
                    try
                    {
                        // 既定値
                        if (iset.DefaultValue.Length > 0)
                        {
                            iset.DefaultValue = IPAddress.Parse(iset.DefaultValue).ToString();
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("IPアドレスまたはサブネットマスクが無効です。", "DefaultValue=" + iset.DefaultValue);
                    }
                }
            }

            #endregion

            #endregion
        }

        #endregion
        
        #region ConfigItemクラス

        /// <summary>
        /// ItemSettingインスタンスに対応する、設定内容を扱います。
        /// </summary>
        [Serializable]
        public class ConfigItem
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.ConfigGrid.ConfigItem クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ConfigItem()
            {
            }

            /// <summary>
            /// 既存のインスタンスから XControls.ConfigGrid.ConfigItem クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="ci">既存のConfigItemインスタンス</param>
            public ConfigItem(ConfigItem ci)
            {
                this.category = ci.category;
                this.name = ci.name;
                this.textValue = ci.textValue;
                this.format = ci.format;
                this.valueType = ci.valueType;
            }

            #endregion

            #region プロパティ

            #region Categoryプロパティ

            private string category = "";
            /// <summary>
            /// 設定項目のカテゴリ名を参照または設定します。
            /// </summary>
            public string Category
            {
                get
                {
                    return category;
                }
                set
                {
                    category = value;
                }
            }

            #endregion

            #region Nameプロパティ

            private string name = "";
            /// <summary>
            /// 設定項目の項目名を参照または設定します。
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

            #region TextValueプロパティ

            private string textValue = "";
            /// <summary>
            /// 設定値を参照または設定します。
            /// </summary>
            public string TextValue
            {
                get
                {
                    return textValue;
                }
                set
                {
                    textValue = value;
                }
            }

            #endregion

            #region Formatプロパティ

            private string format = "";
            /// <summary>
            /// 設定値の文字列書式を参照または設定します。
            /// </summary>
            public string Format
            {
                get
                {
                    return format;
                }
                set
                {
                    format = value;
                }
            }

            #endregion

            #region ValueTypeプロパティ

            private string valueType = "";
            /// <summary>
            /// 設定値の型を参照または指定します。
            /// </summary>
            public string ValueType
            {
                get
                {
                    return valueType;
                }
                set
                {
                    valueType = value;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region ConvertTextValueメソッド

            /// <summary>
            /// データをtypeに合わせてテキストに変換します。
            /// </summary>
            /// <param name="type">データタイプ</param>
            /// <param name="value">変換元データ</param>
            /// <returns>テキストデータ</returns>
            /// <exception cref="System.ArgumentException"></exception>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException"></exception>
            public static string ConvertTextValue(string type, object value)
            {
                // Typeからインスタンス作成し、Typeを再設定  
                if (type.Length == 0) type = "String";
                Type t = Type.GetType(type);
                if (t == null) t = Type.GetType("System." + type);
                if (t == null) t = Type.GetType("System.Drawing." + type);
                if (t != null) type = t.ToString();
                if (type.Equals("Font")) type = "System.Drawing.Font";

                if (type.ToLower().IndexOf("string") < 0)
                {
                    if (value.ToString().IndexOf("\r") >= 0)
                    {
                        value = value.ToString().Replace("\r", "").Replace("\n", "");
                    }
                }
                if (value.ToString().Length == 0) return "";

                if (type.Equals("System.String[]"))
                {
                    StringBuilder sb = new StringBuilder();
                    string[] sa = (string[])value;
                    for (int i = 0; i < sa.Length; i++)
                    {
                        sb.Append(sa[i].ToString());
                        if ((i + 1) != sa.Length)
                        {
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();
                }
                if (type.Equals("System.Drawing.Color"))
                {
                    Color col = (Color)value;
                    string cx = ((UInt32)(col.A * 0x1000000 + col.R * 0x10000 + col.G * 0x100 + col.B)).ToString("x");
                    if (col.Name != cx)
                    {
                        return col.Name;
                    }
                    else
                    {
                        string strval = col.R.ToString() + ", " + col.G.ToString() + ", " + col.B.ToString();
                        if (col.A < 255)
                        {
                            strval = col.A.ToString() + ", " + strval;
                        }
                        return strval;
                    }
                }
                if (type.Equals("System.Drawing.Font"))
                {
                    Font fon = (Font)value;
                    if (fon == null)
                    {
                        return "";
                    }
                    string strval = fon.Name + ", " + fon.Size.ToString();
                    string styval = "";
                    switch (fon.Unit)
                    {
                        case GraphicsUnit.Document:
                            strval += "doc";
                            break;
                        case GraphicsUnit.Inch:
                            strval += "in";
                            break;
                        case GraphicsUnit.Millimeter:
                            strval += "mm";
                            break;
                        case GraphicsUnit.Pixel:
                            strval += "px";
                            break;
                        case GraphicsUnit.Point:
                            strval += "pt";
                            break;
                        case GraphicsUnit.World:
                            strval += "world";
                            break;
                    }
                    if (fon.Bold)
                    {
                        styval += ", Bold";
                    }
                    if (fon.Italic)
                    {
                        styval += ", Italic";
                    }
                    if (fon.Strikeout)
                    {
                        styval += ", Strikeout";
                    }
                    if (fon.Underline)
                    {
                        styval += ", Underline";
                    }
                    if (styval.Length > 0)
                    {
                        strval += ", style=" + styval.Substring(2);
                    }
                    return strval;
                }
                if (type.Equals("IPAddress"))
                {
                    try
                    {
                        if (value.ToString().Length > 0)
                        {
                            return IPAddress.Parse(value.ToString()).ToString();
                        }
                        else
                        {
                            return "";
                        }
                    }
                    catch
                    {
                        throw new ArgumentException("IPアドレスまたはサブネットマスクが無効です。", "Value=" + value.ToString());
                    }
                }
                if (type.ToLower().Equals("path") ||
                    type.ToLower().Equals("fullpath") ||
                    type.ToLower().Equals("password"))
                {
                    return value.ToString();
                }
                if (type.ToLower().Equals("folder") ||
                    type.ToLower().Equals("fullfolder"))
                {
                    string txt = value.ToString();
                    if (txt.Length > 0 && (!txt.Substring(txt.Length - 1, 1).Equals("\\"))) txt += "\\";
                    return txt;
                }

                // 一部のTypeについてパースできるかチェック
                if (type.ToLower().IndexOf("font") >= 0 ||
                    type.ToLower().IndexOf("color") >= 0)
                {
                    ConfigItem.ConvertObjectValue(type, value.ToString());
                    return value.ToString();
                }

                // パースできるかチェック  
                if (t == null)
                {
                    throw new ArgumentException("指定したタイプは無効です。", "ValueType=" + type);
                }
                System.Convert.ChangeType(value, t);

                return value.ToString();
            }

            #endregion

            #region ConvertObjectValueメソッド

            /// <summary>
            /// データをtypeに合わせてオブジェクトに変換します。
            /// </summary>
            /// <param name="type">データタイプ</param>
            /// <param name="value">変換元データ</param>
            /// <returns>オブジェクトデータ</returns>
            /// <exception cref="System.ArgumentException"></exception>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="System.FormatException"></exception>
            public static object ConvertObjectValue(string type, string value)
            {
                // Typeからインスタンス作成し、Typeを再設定  
                if (type.Length == 0) type = "String";
                Type t = Type.GetType(type);
                if (t == null) t = Type.GetType("System." + type);
                if (t == null) t = Type.GetType("System.Drawing." + type);
                if (t != null) type = t.ToString();
                if (type.Equals("Font")) type = "System.Drawing.Font";

                if (type.Equals("System.String[]"))
                {
                    return value.Split(SPLIT_CRLF, System.StringSplitOptions.None);
                }
                if (type.Equals("System.Drawing.Color"))
                {
                    Color col;
                    string nc = "";
                    int s = value.IndexOf("[");
                    if (s >= 0) value = value.Substring(s + 1);
                    s = value.IndexOf("]");
                    if (s >= 0) value = value.Substring(0, s);
                    if (value.IndexOf(",") < 0)
                    {
                        col = Color.FromName(value);
                        return col;
                    }
                    else
                    {
                        for (int i = 0; i < value.Length; i++)
                        {
                            string c = value.Substring(i, 1);
                            if (c.CompareTo("0") >= 0 && c.CompareTo("9") <= 0 || c == ",") nc += c;
                        }
                        if (nc.Length == 0) nc = value;
                        string[] p = nc.Split(',');
                        if (p.Length == 4)
                        {
                            col = Color.FromArgb(UInt16.Parse(p[0]), UInt16.Parse(p[1]), UInt16.Parse(p[2]), UInt16.Parse(p[3]));
                            return col;
                        }
                        if (p.Length == 3)
                        {
                            col = Color.FromArgb(UInt16.Parse(p[0]), UInt16.Parse(p[1]), UInt16.Parse(p[2]));
                            return col;
                        }
                    }
                }
                if (type.Equals("System.Drawing.Font"))
                {
                    if (value.Length < 1)
                    {
                        return null;
                    }
                    string[] p = value.Split(',');
                    Font fon = new Font(p[0], 9);
                    if (p.Length >= 1)
                    {
                        FontStyle fs = 0;
                        for (int i = 2; i < p.Length; i++)
                        {
                            if (i == 2)
                            {
                                p[i] = p[i].Replace("style=", "");
                            }
                            p[i] = p[i].ToLower().Trim();

                            if (p[i] == "bold")
                            {
                                fs = fs | FontStyle.Bold;
                            }
                            if (p[i] == "italic")
                            {
                                fs = fs | FontStyle.Italic;
                            }
                            if (p[i] == "strikeout")
                            {
                                fs = fs | FontStyle.Strikeout;
                            }
                            if (p[i] == "underline")
                            {
                                fs = fs | FontStyle.Underline;
                            }
                        }

                        int us;
                        if ((us = p[1].IndexOf("doc")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.Document);
                        }
                        if ((us = p[1].IndexOf("in")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.Inch);
                        }
                        if ((us = p[1].IndexOf("mm")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.Millimeter);
                        }
                        if ((us = p[1].IndexOf("px")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.Pixel);
                        }
                        if ((us = p[1].IndexOf("pt")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.Point);
                        }
                        if ((us = p[1].IndexOf("world")) > 0)
                        {
                            fon = new Font(p[0], Single.Parse(p[1].Substring(0, us)), fs, GraphicsUnit.World);
                        }
                    }
                    return fon;
                }
                return value;
            }

            #endregion

            #endregion
        }

        #endregion

        #region TempItemクラス

        /// <summary>
        /// プログラムから利用できる、一時的に利用可能な項目を扱います。
        /// </summary>
        [Serializable]
        public class TempItem
        {
            #region プロパティ

            #region Nameプロパティ

            private string name = "";
            /// <summary>
            /// 一時項目の項目名を参照または設定します。
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

            #region TextValueプロパティ

            private string textValue = "";
            /// <summary>
            /// 一時項目の値を参照または設定します。
            /// </summary>
            public string TextValue
            {
                get
                {
                    return textValue;
                }
                set
                {
                    textValue = value;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region ConfigDataクラス

        /// <summary>
        /// 取り扱う設定項目のリストです。
        /// </summary>
        [TypeConverter(typeof(ConfigDataTypeConverter))]
        internal class ConfigData
        {
            #region プロパティ

            #region ConfigItemプロパティ

            private ArrayList confItem = null;
            /// <summary>
            /// 設定項目のデータリストを参照または設定します。
            /// </summary>
            public ArrayList ConfigItem
            {
                get
                {
                    return confItem;
                }
                set
                {
                    confItem = value;
                    if (updateTimer != null)
                    {
                        updateTimer.Enabled = true;
                    }
                }
            }

            #endregion

            #region ItemSettingCollectionプロパティ

            private ItemSettingCollection itemSettingCollection = null;
            /// <summary>
            /// 設定項目の定義リストを参照または設定します。
            /// </summary>
            public ItemSettingCollection ItemSettingCollection
            {
                get
                {
                    return itemSettingCollection;
                }
                set
                {
                    itemSettingCollection = value;
                    if (itemSettingCollection != null)
                    {
                        itemSettingCollection.UpdateTimer = updateTimer;
                        if (updateTimer != null)
                        {
                            updateTimer.Enabled = true;
                        }
                    }
                }
            }

            #endregion

            #region UpdateTimerプロパティ

            private Timer updateTimer = null;
            /// <summary>
            /// 画面更新に使用するタイマーコントロールを参照または設定します。
            /// </summary>
            public Timer UpdateTimer
            {
                get
                {
                    return updateTimer;
                }
                set
                {
                    updateTimer = value;
                }
            }

            #endregion

            #region ConfigGridプロパティ

            private ConfigGrid xConfig = null;
            /// <summary>
            /// 暗号解読のために使用するConfigGridクラスのインスタンスを参照または設定します。
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public ConfigGrid ConfigGrid
            {
                get
                {
                    return xConfig;
                }
                set
                {
                    xConfig = value;
                }
            }

            #endregion

            #endregion
        }        

        #endregion

        #region ConfigDataタイプコンバータ

        /// <summary>
        /// ConfigDataクラスのプロパティに対する型変換を提供します。 
        /// </summary>
        internal class ConfigDataTypeConverter : TypeConverter
        {
            #region メソッド

            #region GetPropertiesメソッド

            /// <summary>
            /// 指定した条件に合致するプロパティコレクションを取得します。
            /// </summary>
            /// <param name="context">コンテキスト情報</param>
            /// <param name="value">プロパティ値</param>
            /// <param name="attributes">プロパティ属性リスト</param>
            /// <returns>見つかったプロパティコレクション</returns>
            public override PropertyDescriptorCollection GetProperties(
                ITypeDescriptorContext context, Object value, Attribute[] attributes
            )
            {
                ConfigData cd = (ConfigData)value;
                ArrayList clst = cd.ConfigItem;
                ItemSettingCollection isc = cd.ItemSettingCollection;
                PropertyDescriptorCollection pdcorg = TypeDescriptor.GetProperties(value, attributes, true);
                PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(null);
                if (clst != null)
                {
                    for (int i = 0; i < clst.Count; i++)
                    {
                        ConfigItem ci = (ConfigItem)clst[i];
                        ItemSetting iset = isc.FindSetting(ci.Category, ci.Name);
                        pdc.Add(new ConfigDataPropertyDescriptor(pdcorg[0], ci, iset, cd));
                    }
                }
                return pdc;
            }

            #endregion

            #region GetPropertiesSupportedメソッド

            /// <summary>
            /// 指定したコンテキスト情報のGetPropertiesをサポートしているかどうかを返します。
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            #endregion

            #endregion
        }

        #endregion

        #region ConfigDataタイプエディタ

        /// <summary>
        /// ConfigDataクラスのプロパティ編集に使用するエディタを提供します。 
        /// </summary>
        internal class ConfigDataTypeEditor : System.Drawing.Design.UITypeEditor
        {
            #region 変数

            private FolderBrowserDialog folderBrowser;
            private OpenFileDialog fileBrowser;
            private ConfigGridPassDlg passEditor;
            private string valtype;
            private ConfigData configData;

            #endregion

            #region プロパティ

            #region IsSupportプロパティ

            private bool isSupport;
            /// <summary>
            /// 独自のUITypeEditorを提供するかどうかを返します。
            /// </summary>
            /// <remarks>現状はDateTimeでのエディタ無効化のみ対応</remarks>
            public bool IsSupport
            {
                get
                {
                    return isSupport;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region ConfigDataTypeEditorメソッド

            /// <summary>
            /// 使用するエディタを設定します。
            /// </summary>
            /// <param name="ValueType">データタイプ</param>
            /// <param name="cd">編集データ</param>
            public ConfigDataTypeEditor(string ValueType, ConfigData cd)
            {
                valtype = ValueType.ToLower();
                configData = cd;
                if (valtype == "datetime")
                {
                    isSupport = true;
                    return;
                }
                if (valtype == "folder" ||
                    valtype == "fullfolder"||
                    valtype == "path" ||
                    valtype == "fullpath")
                {
                    isSupport = true;
                    return;
                }
                if (valtype == "password")
                {
                    isSupport = true;
                    return;
                }
                isSupport = false;
            }

            #endregion

            #region GetEditStyleメソッド

            /// <summary>
            /// プロパティエディタの表示をカスタマイズします。
            /// </summary>
            /// <param name="context">コンテキスト情報</param>
            /// <returns>適用するスタイル情報</returns>
            public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
            {
                if (valtype == "folder" ||
                    valtype == "fullfolder" ||
                    valtype == "path" ||
                    valtype == "fullpath" ||
                    valtype == "password")
                {
                    // [...]を表示
                    return UITypeEditorEditStyle.Modal;
                }
                // UITypeEditorのサポートなし
                return UITypeEditorEditStyle.None;
            }

            #endregion

            #region EditValueメソッド

            /// <summary>
            /// プロパティ値をエディタで編集します。
            /// </summary>
            /// <param name="context">コンテキスト情報</param>
            /// <param name="provider">サービスプロバイダ</param>
            /// <param name="value">編集するオブジェクト</param>
            /// <returns>編集結果のオブジェクト</returns>
            public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                if (valtype == "folder" || valtype == "fullfolder")
                {
                    // 初期化
                    if (folderBrowser == null)
                    {
                        folderBrowser = new FolderBrowserDialog();
                        folderBrowser.Description = "使用するフォルダを選択してください。";
                    }
                    // 初期パスを設定
                    if (value is string)
                    {
                        if (valtype == "fullfolder")
                        {
                            folderBrowser.SelectedPath = value.ToString();
                        }
                        else
                        {
                            Uri appUri = new Uri(Application.StartupPath + "\\");
                            Uri outUri = new Uri(appUri, value.ToString());
                            folderBrowser.SelectedPath = outUri.LocalPath;
                        }
                    }
                    // パスを選択
                    if (folderBrowser.ShowDialog() != DialogResult.OK)
                    {
                        return value;
                    }
                    if (valtype == "fullfolder")
                    {
                        return folderBrowser.SelectedPath;
                    }
                    else
                    {
                        Uri appUri = new Uri(Application.StartupPath + "\\");
                        Uri outUri = new Uri(appUri, folderBrowser.SelectedPath);
                        return Uri.UnescapeDataString(appUri.MakeRelativeUri(outUri).ToString().Replace("/", "\\"));
                    }
                }
                if (valtype == "path" || valtype == "fullpath")
                {
                    // 初期化
                    if (fileBrowser == null)
                    {
                        fileBrowser = new OpenFileDialog();
                        fileBrowser.Title = "使用するファイルを選択してください。";
                    }
                    // 初期パスを設定
                    if (value is string)
                    {
                        if (valtype == "fullpath")
                        {
                            try
                            {
                                fileBrowser.InitialDirectory = Path.GetDirectoryName(value.ToString());
                            }
                            catch { }
                        }
                        else
                        {
                            Uri appUri = new Uri(Application.StartupPath + "\\");
                            Uri outUri = new Uri(appUri, value.ToString());
                            try
                            {
                                fileBrowser.InitialDirectory = Path.GetDirectoryName(outUri.LocalPath);
                            }
                            catch { }
                        }
                    }
                    // パスを選択
                    fileBrowser.CheckFileExists = true;
                    fileBrowser.Multiselect = false;
                    if (fileBrowser.ShowDialog() != DialogResult.OK)
                    {
                        return value;
                    }
                    if (valtype == "fullpath")
                    {
                        return fileBrowser.FileName;
                    }
                    else
                    {
                        Uri appUri = new Uri(Application.StartupPath + "\\");
                        Uri outUri = new Uri(appUri, folderBrowser.SelectedPath);
                        return Uri.UnescapeDataString(appUri.MakeRelativeUri(outUri).ToString().Replace("/", "\\"));
                    }
                }
                if (valtype == "password")
                {
                    // 初期化
                    if (passEditor == null)
                    {
                        passEditor = new ConfigGridPassDlg();
                    }
                    // パスワードを編集
                    passEditor.ConfigGrid = configData.ConfigGrid;
                    passEditor.ShowEditPassword = false;
                    passEditor.PasswordData = value.ToString();
                    passEditor.TopMost = true;
                    if (passEditor.ShowDialog() != DialogResult.OK)
                    {
                        return value;
                    }
                    return passEditor.PasswordData;
                }
                return value;
            }

            #endregion

            #endregion
        }

        #endregion

        #region ConfigDataプロパティディスクリプタ

        /// <summary>
        /// ConfigDataクラスのプロパティを抽象化します。
        /// </summary>
        internal class ConfigDataPropertyDescriptor : PropertyDescriptor
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.ConfigGrid.ConfigDataPropertyDescriptor クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="pd"プロパティ識別子></param>
            /// <param name="ci">設定項目の内容</param>
            /// <param name="iset">設定項目の定義</param>
            /// <param name="cd">設定項目を保持するリスト</param>
            public ConfigDataPropertyDescriptor(PropertyDescriptor pd, ConfigItem ci, ItemSetting iset, ConfigData cd)
                : base(pd)
            {
                propertyDescriptor = pd;
                configItem = ci;
                itemSetting = iset;
                configEdit = new ConfigDataTypeEditor(ci.ValueType, cd);
            }

            #endregion

            #region 変数

            private PropertyDescriptor propertyDescriptor;
            private ConfigItem configItem;
            private ItemSetting itemSetting;
            private ConfigDataTypeEditor configEdit;

            #endregion

            #region プロパティ

            #region ComponentTypeプロパティ

            /// <summary>
            /// コンポーネントのタイプを参照します。
            /// </summary>
            public override Type ComponentType
            {
                get
                {
                    return this.GetType();
                }
            }

            #endregion

            #region Descriptionプロパティ

            /// <summary>
            /// プロパティのディスクプリションを参照します。
            /// </summary>
            public override string Description
            {
                get
                {
                    return itemSetting.Description;
                }
            }

            #endregion

            #region Categoryプロパティ

            /// <summary>
            /// 設定項目のカテゴリを参照します。
            /// </summary>
            public override string Category
            {
                get
                {
                    string cn = itemSetting.DisplayCategory;
                    if (cn.Equals(""))
                    {
                        cn = itemSetting.Category;
                    }
                    return cn;
                }
            }

            #endregion

            #region IsReadOnlyプロパティ

            /// <summary>
            /// プロパティが読み取り専用かどうかを参照します。
            /// </summary>
            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            #endregion

            #region PropertyTypeプロパティ

            /// <summary>
            /// プロパティのタイプを参照します。
            /// </summary>
            public override Type PropertyType
            {
                get
                {
                    Type tp = TypeFromString(itemSetting.ValueType);
                    return tp;
                }
            }


            #endregion

            #region DisplayNameプロパティ

            /// <summary>
            /// 設定項目の表示名を参照します。
            /// </summary>
            public override string DisplayName
            {
                get
                {
                    string dn = itemSetting.DisplayName;
                    if (dn.Equals(""))
                    {
                        dn = itemSetting.Name;
                    }
                    return dn;
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region CanResetValueメソッド

            /// <summary>
            /// 項目の設定内容をリセットできるかどうかを返します。
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public override bool CanResetValue(object component)
            {
                return true;
            }

            #endregion

            #region GetValueメソッド

            /// <summary>
            /// コンポーネントの値を返します。
            /// </summary>
            /// <param name="component">コンポーネント</param>
            /// <returns>格納されていた値</returns>
            public override object GetValue(object component)
            {
                return ConfigItem.ConvertObjectValue(PropertyType.ToString(), configItem.TextValue);
            }

            #endregion

            #region GetEditorメソッド

            /// <summary>
            /// 指定したエディタタイプのエディタを返します。
            /// </summary>
            /// <param name="editorBaseType">エディタタイプ</param>
            /// <returns>エディタ</returns>
            public override object GetEditor(Type editorBaseType)
            {
                if (configEdit.IsSupport)
                {
                    return configEdit;
                }
                return base.GetEditor(editorBaseType);
            }


            #endregion

            #region ResetValueメソッド

            /// <summary>
            /// プロパティリセット時の値を返します。
            /// </summary>
            /// <param name="component"></param>
            public override void ResetValue(object component)
            {
                configItem.TextValue = itemSetting.DefaultValue;
            }

            #endregion

            #region ShouldSerializeValueメソッド

            /// <summary>
            /// プロパティ値を永続化する必要があるかどうかを返します。
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            #endregion

            #region SetValueメソッド

            /// <summary>
            /// 値を設定します。
            /// </summary>
            /// <param name="component">コンポーネント</param>
            /// <param name="value">設定する値</param>
            public override void SetValue(object component, object value)
            {
                configItem.TextValue = ConfigItem.ConvertTextValue(configItem.ValueType, value);
                configItem.TextValue = FormattingFromString(configItem);
            }

            #endregion

            #endregion
        }

        #endregion

        #region ConfigGridDesignerクラス

        /// <summary>
        /// ConfigGrid用にデザイナをカスタマイズします。
        /// </summary>
        public class ConfigGridDesigner : System.Windows.Forms.Design.ControlDesigner
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.ConfigGrid.ConfigGridDesigner クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ConfigGridDesigner()
            {
            }

            #endregion

            #region メソッド

            #region PostFilterPropertiesメソッド

            protected override void PostFilterProperties(IDictionary properties)
            {
                // フィルタリングするプロパティ
                properties.Remove("ConfigLoadValue");
                properties.Remove("SelectedObject");

                base.PostFilterProperties(properties);
            }

            #endregion

            #endregion
        }

        #endregion

        #region StreamWriter2クラス

        /// <summary>
        /// 出力のコピーを持つStreamWriterです。
        /// </summary>
        private class StreamWriter2 : StreamWriter
        {
            #region 構築・破棄

            /// <summary>
            /// 指定したストリームを使用して、XControls.ConfigGrid.StreamWriter2 クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="stream">使用するストリーム</param>
            public StreamWriter2(Stream stream)
                : base(stream) { }

            /// <summary>
            /// 指定したファイル用に、XControls.ConfigGrid.StreamWriter2 クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="path">ファイルのパス</param>
            /// <param name="append">上書きならfalse, 追加ならtrue</param>
            /// <param name="encoding">使用するエンコーディング</param>
            public StreamWriter2(string path, bool append, Encoding encoding)
                : base(path, append, encoding) { }

            #endregion

            #region 変数

            private StringBuilder outvalue = new StringBuilder();

            #endregion

            #region プロパティ

            #region Valueプロパティ

            /// <summary>
            /// 書き込んだ内容を参照します。
            /// </summary>
            public string Value
            {
                get
                {
                    return outvalue.ToString();
                }
            }

            #endregion

            #endregion

            #region メソッド

            #region WriteLineメソッド

            /// <summary>
            /// ストリームに書き込みます。
            /// </summary>
            /// <param name="value">書き込む内容</param>
            public override void WriteLine(string value)
            {
                outvalue.AppendLine(value);
                base.WriteLine(value);
            }

            #endregion

            #endregion
        }

        #endregion

        #region ConfigPassDlgクラス

        /// <summary>
        /// パスワード項目の値の編集を行います。
        /// </summary>
        public partial class ConfigGridPassDlg : Form
        {
            #region 構築・破棄

            /// <summary>
            /// XControls.ConfigGridPassDlg クラスの新しいインスタンスを初期化します。
            /// </summary>
            public ConfigGridPassDlg()
            {
                InitializeComponent();
            }

            /// <summary>
            /// 必要なデザイナー変数です。
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// 使用中のリソースをすべてクリーンアップします。
            /// </summary>
            /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Windows フォーム デザイナーで生成されたコード

            /// <summary>
            /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
            /// コード エディターで変更しないでください。
            /// </summary>
            private void InitializeComponent()
            {
                this.btnOk = new System.Windows.Forms.Button();
                this.btnCancel = new System.Windows.Forms.Button();
                this.txtPass = new System.Windows.Forms.TextBox();
                this.chkShow = new System.Windows.Forms.CheckBox();
                this.SuspendLayout();
                // 
                // btnOk
                // 
                this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.btnOk.Location = new System.Drawing.Point(252, 70);
                this.btnOk.Name = "btnOk";
                this.btnOk.Size = new System.Drawing.Size(75, 23);
                this.btnOk.TabIndex = 0;
                this.btnOk.Text = "OK";
                this.btnOk.UseVisualStyleBackColor = true;
                this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
                // 
                // btnCancel
                // 
                this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.btnCancel.Location = new System.Drawing.Point(333, 70);
                this.btnCancel.Name = "btnCancel";
                this.btnCancel.Size = new System.Drawing.Size(75, 23);
                this.btnCancel.TabIndex = 1;
                this.btnCancel.Text = "キャンセル";
                this.btnCancel.UseVisualStyleBackColor = true;
                // 
                // txtPass
                // 
                this.txtPass.Location = new System.Drawing.Point(12, 23);
                this.txtPass.Name = "txtPass";
                this.txtPass.PasswordChar = '*';
                this.txtPass.Size = new System.Drawing.Size(396, 19);
                this.txtPass.TabIndex = 3;
                // 
                // chkShow
                // 
                this.chkShow.AutoSize = true;
                this.chkShow.Location = new System.Drawing.Point(16, 74);
                this.chkShow.Name = "chkShow";
                this.chkShow.Size = new System.Drawing.Size(107, 16);
                this.chkShow.TabIndex = 4;
                this.chkShow.Text = "見ながら入力する";
                this.chkShow.UseVisualStyleBackColor = true;
                this.chkShow.CheckedChanged += new System.EventHandler(this.chkShow_CheckedChanged);
                // 
                // PassEditorDialog
                // 
                this.AcceptButton = this.btnOk;
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.CancelButton = this.btnCancel;
                this.ClientSize = new System.Drawing.Size(420, 105);
                this.Controls.Add(this.chkShow);
                this.Controls.Add(this.txtPass);
                this.Controls.Add(this.btnCancel);
                this.Controls.Add(this.btnOk);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "PassEditorDialog";
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.Text = "パスワード編集";
                this.ResumeLayout(false);
                this.PerformLayout();
            }

            #endregion

            private System.Windows.Forms.Button btnOk;
            private System.Windows.Forms.Button btnCancel;
            private System.Windows.Forms.TextBox txtPass;
            private System.Windows.Forms.CheckBox chkShow;

            #endregion

            #region プロパティ

            #region ShowEditPasswordプロパティ

            private Boolean showEditPassword = false;
            /// <summary>
            /// パスワードを見ながら入力するかどうかを参照または設定します。
            /// </summary>
            public Boolean ShowEditPassword
            {
                get
                {
                    return showEditPassword;
                }
                set
                {
                    showEditPassword = value;
                    chkShow.Checked = showEditPassword;
                }
            }

            #endregion

            #region ConfigGridプロパティ

            private ConfigGrid xconf;
            /// <summary>
            /// ダイアログを使用するConfigGridクラスのインスタンスを参照または設定します。
            /// </summary>
            public ConfigGrid ConfigGrid
            {
                get
                {
                    return xconf;
                }
                set
                {
                    xconf = value;
                    this.Font = xconf.Font;
                    this.chkShow.Visible = xconf.allowEditShowPassword;
                }
            }

            #endregion

            #region PasswordDataプロパティ

            private String passwordData;
            /// <summary>
            /// 入力中のパスワードの内容を参照または設定します。
            /// </summary>
            public String PasswordData
            {
                get
                {
                    return passwordData;
                }
                set
                {
                    passwordData = value;

                    // 暗号解除
                    try
                    {
                        txtPass.Text = xconf.FromCrypt(passwordData);
                    }
                    catch (Exception es)
                    {
                        MessageBox.Show(es.Message, "内容が正しくないため復号できません", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPass.Text = "";
                    }
                }
            }

            #endregion

            #endregion

            #region イベント

            #region chkShow_CheckedChangedイベント

            private void chkShow_CheckedChanged(object sender, EventArgs e)
            {
                if (chkShow.Checked)
                {
                    txtPass.PasswordChar = new char();
                }
                else
                {
                    txtPass.PasswordChar = '*';
                }
            }

            #endregion

            #region btnOk_Clickイベント

            private void btnOk_Click(object sender, EventArgs e)
            {
                // 暗号化
                passwordData = xconf.ToCrypt(txtPass.Text);
                Close();
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region 構築・破棄

        /// <summary>
        /// XControls.ConfigGrid クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ConfigGrid()
            : base()
        {
            #region 初期化

            components = new System.ComponentModel.Container();
            timer = new System.Windows.Forms.Timer(this.components);
            timer.Tick += new System.EventHandler(this.timer_Tick);

            ItemSettings = new ItemSettingCollection();
            configData.UpdateTimer = timer;

            SetValueControl();

            #endregion
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            #region 後処理

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            #endregion
        }

        #endregion

        #region 定数

        /// <summary>
        /// 設定ファイルのエンコーディングです。
        /// </summary>
        private const string ENCODING = "Shift-JIS";

        /// <summary>
        /// 誤交換防止用のデータ交換用IDです。
        /// </summary>
        private const string EXCHANGE_ID = "CONFGRID_DATA_100";

        /// <summary>
        /// トリプルDES 初期ベクターテーブルです。
        /// </summary>
        private const string IVC_TEXT = "Cn%ts#xzl(hw!jq9";

        #endregion

        #region 変数

        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 表示更新用のタイマーです。
        /// </summary>
        private System.Windows.Forms.Timer timer = null;

        /// <summary>
        /// 設定項目編集用です。
        /// </summary>
        private ConfigData configData = new ConfigData();

        /// <summary>
        /// 設定項目保持用です。
        /// 内部にConfigItemを保持します。
        /// </summary>
        private ArrayList configValueList = new ArrayList();

        /// <summary>
        /// 一時項目保持用です。
        /// 内部にTempItemを保持します。
        /// </summary>
        private ArrayList tempValueList = new ArrayList();

        /// <summary>
        /// 初期化フラグです。
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// 行終端文字(LF)です。
        /// </summary>
        private char[] SPLIT_LF = new char[] { '\n' };

        /// <summary>
        /// 行終端文字(CRLF)です。
        /// </summary>
        private static string[] SPLIT_CRLF = new string[] { "\r\n" };

        #endregion

        #region プロパティ

        #region 追加のプロパティ

        #region AllowEditShowPasswordプロパティ

        private bool allowEditShowPassword = false;
        /// <summary>
        /// パスワード編集時に、見ながら編集を行えるかどうかを設定または取得します。
        /// </summary>
        [
            Category("動作"),
            DefaultValue(false),
            Description("パスワード編集時に、見ながら編集を行えるかどうかを設定または取得します。")
        ]
        public bool AllowEditShowPassword
        {
            get
            {
                return allowEditShowPassword;
            }
            set
            {
                allowEditShowPassword = value;
            }
        }

        #endregion

        #region Directoryプロパティ

        private string directory = "";
        /// <summary>
        /// 設定ファイルの格納ディレクトリを参照または設定します。
        /// </summary>
        [
            Category("設定ファイル"),
            DefaultValue(""),
            Description("設定ファイルの格納ディレクトリを参照または設定します。"),
            EditorAttribute(
                typeof(System.Windows.Forms.Design.FolderNameEditor),
                typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                directory = value;
                while(directory.Length > 0 && directory.Substring(directory.Length - 1, 1).Equals("\\"))
                {
                    directory = directory.Substring(0, directory.Length - 1);
                }
                if (directory.Length > 0)
                {
                    directory = directory + "\\";
                }
            }
        }

        #endregion

        #region FileNameプロパティ

        private string fileName = "setting.inix";
        /// <summary>
        /// 設定ファイル名を参照または設定します。
        /// </summary>
        [
            Category("設定ファイル"),
            DefaultValue("setting.inix"),
            Description("設定ファイル名を参照または設定します。"),
            EditorAttribute(
                typeof(System.Windows.Forms.Design.FileNameEditor),
                typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                if (fileName.Length > 0)
                {
                    if (fileName.LastIndexOf(".") < 1)
                    {
                        fileName = fileName + ".inix";
                    }
                }
            }
        }

        #endregion

        #region ItemSettingsプロパティ

        private ItemSettingCollection itemSettings = null;
        /// <summary>
        /// 設定ファイルの項目を参照または設定します。
        /// </summary>
        [
            Category("設定ファイル"),
            Description("設定ファイルの項目を参照または設定します。"),
            DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)
        ]
        public ItemSettingCollection ItemSettings
        {
            get
            {
                return itemSettings;
            }
            set
            {
                itemSettings = value;
                if (itemSettings != null)
                {
                    itemSettings.UpdateTimer = timer;
                }
            }
        }

        #endregion

        #region PublicKeyTextプロパティ

        private string publicKeyText = "SET KEY!";
        /// <summary>
        /// 暗号化に使用する共通鍵文字列を参照または設定します。この文字列は8文字で指定します。
        /// </summary>
        [
            Category("設定ファイル"),
            DefaultValue("SET KEY!"),
            Description("暗号化に使用する共通鍵文字列を参照または設定します。この文字列は8文字で指定します。")
        ]
        public string PublicKeyText
        {
            get
            {
                return publicKeyText;
            }
            set
            {
                if (value.Length != 8)
                {
                    throw new ArgumentException("共通鍵文字列は8文字で指定してください", value);
                }
                publicKeyText = value;
            }
        }

        #endregion

        #region ConfigSaveValueプロパティ

        private string configSaveValue = "";
        /// <summary>
        /// 直前のSaveSettingメソッドで保存したXML形式のデータを参照します。
        /// </summary>
        [
            Category("設定ファイル"),
            Description("直前のSaveSettingメソッドで保存したXML形式のデータを参照します。")
        ]
        public string ConfigSaveValue
        {
            get
            {
                return configSaveValue;
            }
        }

        #endregion

        #region ConfigLoadValueプロパティ

        private string configLoadValue = "";
        /// <summary>
        /// XML形式のデータを設定すると、LoadSettingメソッドで読み込まれます。読み込み後はクリアされます。
        /// </summary>
        [
            Category("設定ファイル"),
            Description("XML形式のデータを設定すると、LoadSettingメソッドで読み込まれます。読み込み後はクリアされます。")
        ]
        public string ConfigLoadValue
        {
            get
            {
                return configLoadValue;
            }
            set
            {
                configLoadValue = value;
            }
        }

        #endregion

        #endregion

        #endregion

        #region イベント

        #region OnPropertyValueChangedイベント

        /// <summary>
        /// XControls.ConfigGrid.PropertyValueChangedイベントを発生させます。
        /// </summary>
        protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            base.OnPropertyValueChanged(e);
            timer.Enabled = false;
            timer.Enabled = true;
        }

        #endregion

        #region SelectedGridItemChangedイベント

        private GridItem lastSelection = null;
        /// <summary>
        /// XControls.ConfigGrid.SelectedGridItemChangedイベントを発生させます。
        /// </summary>
        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            if (lastSelection != null)
            {
                if (lastSelection.Parent.Label.Equals(e.NewSelection.Parent.Label) &&
                    lastSelection.Label.Equals(e.NewSelection.Label))
                {
                    // カテゴリ・項目名が同じ
                    return;
                }
            }
            if (e.NewSelection.Parent == null)
            {
                // データ部
                return;
            }
            lastSelection = e.NewSelection;
            base.OnSelectedGridItemChanged(e);
        }

        #endregion

        #region timer_Tickイベント

        private void timer_Tick(object sender, EventArgs e)
        {
            SetValueControl();
            timer.Enabled = false;
        }

        #endregion

        #endregion

        #region メソッド

        #region 設定項目テキスト値設定 (SetConfigValue)

        /// <summary>
        /// 保持している設定項目のテキスト値を変更します。
        /// </summary>
        /// <param name="catename">カテゴリ名</param>
        /// <param name="valname">設定項目名</param>
        /// <param name="value">対応するテキスト値</param>
        public void SetConfigValue(string catename, string valname, string value)
        {
            while (initialized == false)
            {
                SetValueControl();
            }

            string nval = "";
            try
            {
                nval = value;
            }
            catch
            {
            }
            int idx = 0;
            string val = "";
            string fmt = "";
            string typ = "";
            findConfValue(catename, valname, ref idx, ref val, ref fmt, ref typ);

            if (idx < 0)
            {
                // 項目無し
                throw new ApplicationException("指定の項目は存在しません\r\nカテゴリ名:\t" + catename + "\r\n項目名:\t" + valname);
            }
            else
            {
                // 更新
                ConfigItem ci = (ConfigItem)configValueList[idx];
                ci.TextValue = nval;
                configValueList[idx] = ci;
                timer.Enabled = true;
            }
            return;
        }

        #endregion

        #region 設定項目テキスト値取得 (GetConfigValue)

        /// <summary>
        /// 保持している設定項目のテキスト値を取得します。
        /// </summary>
        /// <param name="catename">カテゴリ名</param>
        /// <param name="valname">設定項目名</param>
        /// <returns>対応するテキスト値(無い場合は空文字を返却)</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public string GetConfigValue(string catename, string valname)
        {
            int idx = 0;
            string val = "";
            string fmt = "";
            string typ = "";

            while (initialized == false)
            {
                SetValueControl();
            }

            findConfValue(catename, valname, ref idx, ref val, ref fmt, ref typ);
            if (idx < 0)
            {
                // 項目無し
                throw new ApplicationException("指定の項目は存在しません\r\nカテゴリ名:\t" + catename + "\r\n項目名:\t" + valname);
            }

            return val;
        }

        #endregion

        #region 設定項目テキスト値配列設定 (SetConfigValue)

        /// <summary>
        /// 保持している設定項目のテキスト値配列を変更します。
        /// </summary>
        /// <param name="catename">カテゴリ名</param>
        /// <param name="valname">設定項目名</param>
        /// <param name="value">対応するテキスト値配列</param>
        /// <exception cref="System.ApplicationException"></exception>
        public void SetConfigValue(string catename, string valname, string[] value)
        {
            String text = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) text += "\r\n";
                text += value[i];
            }
            SetConfigValue(catename, valname, text);
        }

        #endregion

        #region 設定項目テキスト値配列取得 (GetConfigValue)

        /// <summary>
        /// 保持している設定項目のテキスト値配列を指定したサイズで取得します。
        /// </summary>
        /// <param name="catname">カテゴリ名</param>
        /// <param name="valname">設定項目名</param>
        /// <param name="count">配列サイズ(1未満を指定すると指定を無視する)</param>
        /// <returns>対応するテキスト値配列</returns>
        public String[] GetConfigValue(string catname, string valname, int count)
        {
            String[] vs = GetConfigValue(catname, valname).Replace("\r", "").Split(SPLIT_LF);
            int acount = count;
            if (acount < 1) acount = vs.Length;
            String[] rt = new String[acount];
            for (int i = 0; i < acount; i++)
            {
                if (i < vs.Length)
                {
                    rt[i] = vs[i];
                }
                else
                {
                    rt[i] = "";
                }
            }
            return rt;
        }

        #endregion

        #region 一時項目テキスト値設定 (SetTempValue)

        /// <summary>
        /// 一時項目名を指定してテキスト値を設定します。
        /// </summary>
        /// <param name="valname">一時項目名</param>
        /// <param name="value">設定するテキスト値</param>
        public void SetTempValue(string valname, string value)
        {
            string nval = "";
            try
            {
                nval = value.Trim();
            }
            catch
            {
            }
            int idx = 0;
            string val = "";
            findTempValue(valname, ref idx, ref val);

            if (nval.Equals("") && idx == -1)
            {
                // 未登録を削除
                return;
            }
            if (nval.Equals("") && idx >= 0)
            {
                // 登録済みを削除
                tempValueList.RemoveAt(idx);
                return;
            }
            if (idx < 0)
            {
                // 追加
                TempItem ti = new TempItem();
                ti.Name = valname;
                ti.TextValue = value;
                tempValueList.Add(ti);
            }
            else
            {
                // 更新
                TempItem ti = (TempItem)tempValueList[idx];
                ti.TextValue = value;
                tempValueList[idx] = ti;
            }
            return;
        }

        #endregion

        #region 一時項目テキスト値取得 (GetTempValue)

        /// <summary>
        /// 保持している一時項目のテキスト値を取得します。
        /// </summary>
        /// <param name="valname">一時項目名</param>
        /// <returns>対応するテキスト値(無い場合は空文字を返却)</returns>
        public string GetTempValue(string valname)
        {
            int idx = 0;
            string val = "";
            findTempValue(valname, ref idx, ref val);
            return val;
        }

        #endregion

        #region テキスト復号化 (FromCrypt)

        /// <summary>
        /// 現在の設定でテキスト値を復号化した結果を取得します。
        /// </summary>
        /// <param name="text">復号化するテキスト</param>
        /// <returns>復号化されたテキスト</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public String FromCrypt(String text)
        {
            if (text.Length < 1) return "";

            // 暗号解除
            byte[] key = Encoding.Unicode.GetBytes(publicKeyText);
            byte[] ivc = Encoding.Unicode.GetBytes(IVC_TEXT);
            byte[] src = new byte[text.Length / 2];
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = (byte)int.Parse(text.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, ivc), CryptoStreamMode.Write);
            cs.Write(src, 0, src.Length);
            cs.Close();

            // 復号化されたデータのテキスト化
            byte[] dst = ms.ToArray();
            ms.Close();
            return Encoding.Unicode.GetString(dst);
        }

        #endregion

        #region テキスト暗号化 (ToCrypt)

        /// <summary>
        /// 現在の設定でテキスト値を暗号化した結果を取得します。
        /// </summary>
        /// <param name="text">暗号化するテキスト</param>
        /// <returns>暗号化されたテキスト</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public String ToCrypt(String text)
        {
            if (text.Length < 1) return "";

            // 暗号化
            byte[] key = Encoding.Unicode.GetBytes(publicKeyText);
            byte[] ivc = Encoding.Unicode.GetBytes(IVC_TEXT);
            byte[] src = Encoding.Unicode.GetBytes(text);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, ivc), CryptoStreamMode.Write);
            cs.Write(src, 0, src.Length);
            cs.Close();

            // 暗号化されたデータのテキスト化
            byte[] dst = ms.ToArray();
            ms.Close();

            StringBuilder txt = new StringBuilder(dst.Length * 2);
            for (int i = 0; i < dst.Length; i++)
            {
                txt.Append(dst[i].ToString("X2"));
            }

            return txt.ToString();
        }

        #endregion

        #region ファイルから設定を読込 (LoadSetting)

        /// <summary>
        /// ファイルから設定を読み込みます。
        /// ConfigLoadValue プロパティが設定されていた場合は、このプロパティから読み込みます。
        /// ConfigLoadValue プロパティは空文字列に設定されます。
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public void LoadSetting()
        {
            XmlDocument tempXml = new XmlDocument();
            ArrayList tempList = new ArrayList();

            SetValueControl();

            try
            {
                // 設定ファイルを読む
                if (configLoadValue.Length > 0)
                {
                    // ConfigLoadValueプロパティから
                    tempXml.LoadXml(configLoadValue);
                    configLoadValue = "";
                }
                else
                {
                    // ファイルから
                    tempXml.Load(System.Environment.ExpandEnvironmentVariables(directory + fileName));
                }

                // バージョンチェック -----------------------------
                XmlNode fmtver = tempXml.SelectSingleNode("/config/@fmt");
                if (fmtver == null)
                {
                    throw new ArgumentException("ファイル形式が不正です(format)", fileName);
                }
                if (fmtver.Value.Equals("1.0") == false)
                {
                    throw new ArgumentException("このファイルバージョンには対応していません", fileName);
                }

                // 内容読み取り -----------------------------------
                XmlNodeList categorys = tempXml.SelectNodes("/config/category");
                if (categorys == null)
                {
                    throw new ArgumentException("ファイル形式が不正です(category)", fileName);
                }
                for (int i = 0; i < categorys.Count; i++)
                {
                    XmlNode catnam = categorys.Item(i).SelectSingleNode("@name");
                    if (catnam == null)
                    {
                        throw new ArgumentException("ファイル形式が不正です(category-name)", fileName);
                    }
                    if (catnam.Value.Length < 1)
                    {
                        throw new ArgumentException("カテゴリが設定されていません", fileName);
                    }
                    XmlNodeList values = categorys.Item(i).SelectNodes("value");
                    if (values == null)
                    {
                        throw new ArgumentException("ファイル形式が不正です(value)", fileName);
                    }
                    if (values.Count < 1)
                    {
                        throw new ArgumentException("値がひとつも設定されていません", fileName);
                    }
                    for (int j = 0; j < values.Count; j++)
                    {
                        XmlNode valnam = values.Item(j).SelectSingleNode("@name");
                        if (valnam.Value.Length < 1)
                        {
                            throw new ArgumentException("項目名が設定されていません", fileName);
                        }
                        XmlNode valtxt = values.Item(j).FirstChild;

                        // 値を取得
                        string cat = catnam.Value.Trim();
                        string nam = valnam.Value.Trim();
                        string[] vals = new string[0];
                        if (valtxt != null) vals = valtxt.Value.Split(SPLIT_LF);
                        string val = "";
                        for (int k = 0; k < vals.Length; k++)
                        {
                            vals[k] = vals[k].Replace("\r", "").Trim();
                            int sp = vals[k].IndexOf('|');
                            if (sp >= 0)
                            {
                                val += vals[k].Substring(sp + 1) + "\r\n";
                            }
                        }
                        if (val.Length > 0)
                        {
                            val = val.Substring(0, val.Length - 2);
                        }

                        // 定義チェック
                        int idx = 0;
                        string defval = "";
                        string format = "";
                        string valtyp = "";
                        findConfValue(cat, nam, ref idx, ref defval, ref format, ref valtyp);
                        if (idx == -1)
                        {
                            // 定義なし → 格納しない
                        }
                        else
                        {
                            // 格納
                            ConfigItem ci = new ConfigItem();
                            ci.Category = cat;
                            ci.Name = nam;
                            ci.TextValue = val;
                            ci.Format = format;
                            ci.ValueType = valtyp;
                            tempList.Add(ci);
                        }
                    }
                }

                configValueList = tempList;
                SetValueControl();
                timer.Enabled = true;

            }
            catch (FileNotFoundException es)
            {
                throw es;
            }
            catch(UnauthorizedAccessException es)
            {
                throw es;
            }
            catch (DirectoryNotFoundException es)
            {
                throw new FileNotFoundException(es.Message);
            }
            catch (Exception es)
            {
                throw new ArgumentException("ファイル形式が不正です(xml形式)", fileName + ", 詳細:" + es.Message);
            }

            return;
        }

        #endregion

        #region ファイルに設定を書込 (SaveSetting)

        /// <summary>
        /// ファイルに設定を書き込みます。
        /// 書き込んだ内容のコピーが ConfigSaveValue プロパティに設定されます。
        /// FileName プロパティが空文字列の場合は、実際のファイルには書き込みません。
        /// </summary>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        public void SaveSetting()
        {
            string fname = "";
            if (fileName.Length > 0)
            {
                fname = System.Environment.ExpandEnvironmentVariables(directory + fileName);
            }
            StreamWriter2 mstream = null;
            ArrayList wlist = new ArrayList();

            while (initialized == false)
            {
                SetValueControl();
            }

            for (int n = 0; n < configValueList.Count; n++)
            {
                wlist.Add(new ConfigItem((ConfigItem)configValueList[n]));
            }

            try
            {
                if (fname.Length > 0)
                {
                    // ディレクトリを作成
                    if (directory.Length > 0)
                    {
                        System.IO.Directory.CreateDirectory(System.Environment.ExpandEnvironmentVariables(directory));
                    }

                    // ファイルを作成
                    mstream = new StreamWriter2(fname, false, Encoding.GetEncoding(ENCODING));
                }
                else
                {
                    // メモリストリームを作成

                    mstream = new StreamWriter2(new MemoryStream());
                }

                // XML宣言等組み立て ------------------------------
                mstream.WriteLine("<?xml version=\"1.0\" encoding=\"shift_jis\"?>");
                mstream.WriteLine("<config fmt=\"1.0\">");

                while (wlist.Count > 0)
                {
                    // カテゴリ部組み立て -----------------------------
                    string cat = ((ConfigItem)wlist[0]).Category;
                    mstream.WriteLine("  <category name=\"" + cat + "\">");

                    for (int i = 0; i < wlist.Count; )
                    {
                        ConfigItem ci = (ConfigItem)wlist[i];
                        if (ci.Category.Equals(cat))
                        {
                            // カテゴリ一致

                            // 項目部組み立て ---------------------------------
                            ItemSetting iset = itemSettings.FindSetting(ci.Category, ci.Name);
                            mstream.WriteLine("    <value name=\"" + ci.Name +
                                                "\" description=\"" + iset.Description + "\">");
                            ci.TextValue += "\r\n";
                            int p = -1;
                            do
                            {
                                p = ci.TextValue.IndexOf("\r\n");
                                if (p >= 0)
                                {
                                    String ld = ci.TextValue.Substring(0, p);
                                    ld = ld.Replace("&", "&amp;")
                                           .Replace("<", "&lt;")
                                           .Replace(">", "&gt;")
                                           .Replace("\"", "&quot;")
                                           .Replace("'", "&apos;");
                                    mstream.WriteLine("      |" + ld);
                                    ci.TextValue = ci.TextValue.Substring(p + "\r\n".Length);
                                }
                            } while (p >= 0);
                            mstream.WriteLine("    </value>");

                            wlist.RemoveAt(i);
                        }
                        else
                        {
                            // カテゴリ不一致
                            i++;
                        }
                    }

                    mstream.WriteLine("  </category>");
                }

                mstream.WriteLine("</config>");
            }
            catch (UnauthorizedAccessException es)
            {
                throw es;
            }
            catch (Exception es)
            {
                throw new ArgumentException(es.Message + " - ファイルの保存に失敗しました", fileName);
            }
            finally
            {
                if (mstream != null)
                {
                    configSaveValue = mstream.Value;
                    mstream.Close();
                }
            }

            return;
        }

        #endregion

        #region ConfigGrid間の全データ取得 (GetDataAll)

        /// <summary>
        /// ConfigGrid間のデータ交換用に全データを取得します。
        /// </summary>
        /// <returns>全データ</returns>
        public ArrayList GetDataAll()
        {
            ArrayList bag = new ArrayList();
            bag.Add(EXCHANGE_ID);
            bag.Add(configValueList);
            bag.Add(tempValueList);
            return bag;
        }

        #endregion

        #region ConfigGrid間の全データ設定 (SetDataAll)

        /// <summary>
        /// ConfigGrid間のデータ交換用に受け渡された全データを設定します。受け渡されなかった項目は初期値が設定されます。
        /// </summary>
        /// <param name="bag">全データ</param>
        /// <exception cref="System.ArgumentException"></exception>
        public void SetDataAll(ArrayList bag)
        {
            try
            {
                string eid = bag[0] as string;
                if (eid.Equals(EXCHANGE_ID))
                {
                    configValueList = new ArrayList();
                    foreach(object data in bag[1] as ArrayList)
                    {
                        ConfigItem item = data as ConfigItem;
                        ConfigItem newItem = new ConfigItem(item);
                        configValueList.Add(newItem);
                    }
                    tempValueList = new ArrayList();
                    foreach (object data in bag[2] as ArrayList)
                    {
                        TempItem item = data as TempItem;
                        TempItem newItem = new TempItem();
                        newItem.Name = item.Name;
                        newItem.TextValue = item.TextValue;
                        tempValueList.Add(newItem);
                    }
                    SetValueControl();
                    Refresh();
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("データ交換用のデータではありません。");
            }
        }

        #endregion

        #region ConfigGrid間の設定項目マージ (ItemSettingMerge)

        /// <summary>
        /// 指定したConfigGridの設定項目を取り込みます
        /// </summary>
        /// <param name="from">取り込み元のConfigGrid</param>
        /// <param name="isDup">true=内容を置き換える, false=内容に追加</param>
        public void ItemSettingMerge(ConfigGrid from, bool isDup)
        {
            // 設定削除
            if (isDup)
            {
                itemSettings = new ItemSettingCollection();
                itemSettings.UpdateTimer = this.timer;
            }

            // 設定反映
            foreach (ItemSetting fromISet in from.itemSettings)
            {
                if (itemSettings.FindSetting(fromISet.Category, fromISet.Name) == null)
                {
                    // 新規項目
                    ItemSetting newISet = new ItemSetting();
                    newISet.Category = fromISet.Category;
                    newISet.DisplayCategory = fromISet.DisplayCategory;
                    newISet.Name = fromISet.Name;
                    newISet.DisplayName = fromISet.DisplayName;
                    newISet.ValueType = fromISet.ValueType;
                    newISet.DefaultValue = fromISet.DefaultValue;
                    newISet.Description = fromISet.Description;
                    newISet.Format = fromISet.Format;
                    itemSettings.Add(newISet);
                }
            }

            SetValueControl();
        }

        #endregion

        #region ConfigGrid間の設定/一時項目テキスト値反映 (RefrectConfigValue)

        /// <summary>
        /// 指定したConfigGridの設定/一時項目のテキスト値を反映します。値のチェックはされません。
        /// </summary>
        /// <param name="from">取り込み元のConfigGrid</param>
        /// <param name="isTempMerge">true=from側に存在しない一時項目を残す, false=一時項目を置き換える</param>
        public void RefrectConfigValue(ConfigGrid from, bool isTempMerge)
        {
            // 項目反映
            foreach (ConfigItem fromCi in from.configValueList)
            {
                try
                {
                    SetConfigValue(fromCi.Category, fromCi.Name, fromCi.TextValue);
                }
                catch
                {
                }
            }

            // 一時項目削除
            if (!isTempMerge)
            {
                tempValueList = new ArrayList();
            }

            // 一時項目反映
            foreach (TempItem fromTi in from.tempValueList)
            {
                SetTempValue(fromTi.Name, fromTi.TextValue);
            }

            Refresh();
        }

        #endregion

        #endregion

        #region 内部処理

        #region Type取得 (TypeFromString)

        /// <summary>
        /// データ型文字列からTypeを取得します。
        /// </summary>
        /// <param name="typename">データ型文字列</param>
        /// <returns>Typeオブジェクト</returns>
        private static Type TypeFromString(string typename)
        {
            String tpname = typename;
            if (tpname.Equals(""))
            {
                tpname = "String";
            }
            Type tp = Type.GetType("System." + tpname);
            if (tp == null)
            {
                switch (tpname)
                {
                    case "Color":
                        tp = typeof(Color);
                        break;
                    case "Font":
                        tp = typeof(Font);
                        break;
                    case "FullFolder":
                    case "Folder":
                    case "FullPath":
                    case "Path":
                    case "IPAddress":
                    case "Password":
                        tp = Type.GetType("System.String");
                        break;
                }
            }
            return tp;
        }

        #endregion

        #region Format編集 (FormattingFromString)

        /// <summary>
        /// 設定内容を文字列に変換して、指定の書式に編集します。
        /// </summary>
        /// <param name="ci"></param>
        /// <returns></returns>
        private static string FormattingFromString(ConfigItem ci)
        {
            Type tp = TypeFromString(ci.ValueType);
            string strfmt = ci.Format;
            string strval = ci.TextValue;
            if (strval.Length > 0)
            {
                try
                {
                    // 基本データ型
                    if (tp.ToString().Equals("System.Byte"))
                    {
                        strval = Byte.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.DateTime"))
                    {
                        strval = DateTime.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Decimal"))
                    {
                        strval = Decimal.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Double"))
                    {
                        strval = Decimal.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Int32"))
                    {
                        strval = Int32.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Int64"))
                    {
                        strval = Int64.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.SByte"))
                    {
                        strval = SByte.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Int16"))
                    {
                        strval = Int16.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.Single"))
                    {
                        strval = Single.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.UInt32"))
                    {
                        strval = UInt32.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.UInt64"))
                    {
                        strval = UInt64.Parse(strval).ToString(strfmt);
                    }
                    if (tp.ToString().Equals("System.UInt16"))
                    {
                        strval = UInt16.Parse(strval).ToString(strfmt);
                    }
                }
                catch (Exception)
                {
                }
            }
            return strval;
        }

        #endregion

        #region 設定項目検索 (findConfValue)

        /// <summary>
        /// 保持している設定項目のテキスト値を取得します。
        /// </summary>
        /// <param name="catname">カテゴリ名</param>
        /// <param name="valname">設定項目名</param>
        /// <param name="idx">見つかった位置(無い場合は-1を返却)</param>
        /// <param name="value">対応するテキスト値(無い場合は空文字を返却)</param>
        /// <param name="format">書式(無い場合は空文字を返却)</param>
        /// <param name="valuetype">型</param>
        private void findConfValue(string catname, string valname, ref int idx, ref string value, ref string format, ref string valuetype)
        {
            for (int i = 0; i < configValueList.Count; i++)
            {
                ConfigItem ci = (ConfigItem)configValueList[i];
                if (catname.Equals(ci.Category) && valname.Equals(ci.Name))
                {
                    idx = i;
                    value = ci.TextValue;
                    format = ci.Format;
                    valuetype = ci.ValueType;
                    return;
                }
            }
            idx = -1;
            value = "";
            format = "";
            valuetype = "";
            return;
        }

        #endregion

        #region 一時項目検索 (findTempValue)

        /// <summary>
        /// 保持している一時項目のテキスト値を取得します。
        /// </summary>
        /// <param name="valname">一時項目名</param>
        /// <param name="idx">見つかった位置(無い場合は-1を返却)</param>
        /// <param name="value">対応するテキスト値(無い場合は空文字を返却)</param>
        private void findTempValue(string valname, ref int idx, ref string value)
        {
            for (int i = 0; i < tempValueList.Count; i++)
            {
                TempItem ti = (TempItem)tempValueList[i];
                if (valname.Equals(ti.Name))
                {
                    idx = i;
                    value = ti.TextValue;
                    return;
                }
            }
            idx = -1;
            value = "";
            return;
        }

        #endregion

        #region 内容をコントロールに反映 (SetValueControl)

        /// <summary>
        /// 設定値をコントロールに反映します。
        /// </summary>
        private void SetValueControl()
        {
            // 余分な設定を削除
            for (int i = configValueList.Count - 1; i >= 0; i--)
            {
                ConfigItem ci = (ConfigItem)configValueList[i];
                if (itemSettings.FindSetting(ci.Category, ci.Name) == null)
                {
                    configValueList.RemoveAt(i);
                }
            }

            // 未設定に初期値を設定
            for (int i = 0; i < itemSettings.Count; i++)
            {
                ItemSetting iset = itemSettings[i];
                bool find = false;
                for (int j = 0; j < configValueList.Count; j++)
                {
                    ConfigItem ci = (ConfigItem)configValueList[j];
                    if (ci.Category.Equals(iset.Category) && ci.Name.Equals(iset.Name))
                    {
                        // 既存項目 - Formatのみ更新（表示編集用）
                        ((ConfigItem)configValueList[j]).Format = iset.Format;
                        find = true;
                        break;
                    }
                }
                if (find == false)
                {
                    // 新規項目
                    ConfigItem ci = new ConfigItem();
                    ci.Category = iset.Category;
                    ci.Name = iset.Name;
                    ci.TextValue = iset.DefaultValue;
                    ci.Format = iset.Format;
                    ci.ValueType = iset.ValueType;
                    configValueList.Add(ci);
                }
            }

            // プロパティグリッドに設定
            configData.ConfigItem = configValueList;
            configData.ItemSettingCollection = itemSettings;
            configData.ConfigGrid = this;
            base.SelectedObject = configData;

            initialized = true;
        }

        #endregion

        #endregion
    }
}
