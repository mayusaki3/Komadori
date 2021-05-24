using Jaffa.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace Jaffa
{
    /// <summary>
    /// Jaffaフレームワーク・国際化対応
    /// </summary>
    public static class International
    {
        #region イベント

        #region カルチャー変更イベント (CultureChanged)

        /// <summary>
        /// CurrentCultureが変更されたことを通知します。
        /// </summary>
        public static event EventHandler<EventArgs> CultureChanged;

        #endregion

        #region カルチャー変更イベント呼び出し ([private] OnCultureChanged)

        /// <summary>
        /// カルチャー変更通知イベントを呼び出します。
        /// </summary>
        private static void OnCultureChanged()
        {
            // カルチャー変更を通知
            CultureChanged?.Invoke(currentCulture, new EventArgs());
        }

        #endregion

        #endregion

        #region メソッド

        #region 初期化処理 (Initialize)

        /// <summary>
        /// 初期化処理
        /// </summary>
        public static void Initialize()
        {
            // 起動時のカルチャー名で初期化
            GetResourceCultureName(CultureInfo.CurrentCulture.Name);
        }

        #endregion

        #region 利用可能な言語名リストを取得 (GetAvailableLanguageNameList)

        /// <summary>
        /// 利用可能な言語名リストを取得します。
        /// </summary>
        /// <remarks>
        /// JaffaCore / Resources / CultureList.resx に 文字列 SUPPORT_LIST を追加し、
        /// 次のようにサポートする{言語コード},{言語名} の行を設定します。
        /// 例. Auto,{CULTURE_AUTO}
        ///     en-US,English
        ///     ja-JP,日本語
        /// この例の「{CultureAuto}」部分は、言語コードのリソースで置き換わります。
        /// </remarks>
        public static string[] GetAvailableLanguageNameList()
        {
            string res = null;
            try
            {
                res = Core.Resource(Core.JaffaCulture).GetString("SUPPORT_LIST");
            }
            catch (Exception es)
            {
                Logging.Write(Logging.ExceptionToList(es), LogType.Error);
            }
            if (res == null)
            {
                return Array.Empty<string>();
            }
            string[] langs = res.Split(new char[] { '\n' });
            List<string> rt = new();
            foreach (string lang in langs)
            {
                string[] s = lang.Split(new char[] { ',', '\r', '\n' });
                if (s[1].Contains("{", StringComparison.CurrentCulture))
                {
                    string val = ConvertCurrentCultureResourceString(Core.Jaffa, s[1]);
                    if (val.Length > 0) s[1] = val;
                }
                rt.Add(s[1]);
            }
            return rt.ToArray();
        }

        #endregion

        #region 利用可能な言語コードリストを取得 (GetAvailableLanguageCodeList)

        /// <summary>
        /// 利用可能な言語コードリストを取得します。
        /// </summary>
        /// <remarks>
        /// JaffaCore / Resources / CultureList.resx に 文字列 SUPPORT_LIST を追加し、
        /// 次のようにサポートする{言語コード},{言語名} の行を設定します。
        /// 例. Auto,{CULTURE_AUTO}
        ///     en-US,English
        ///     ja-JP,日本語
        /// </remarks>
        public static string[] GetAvailableLanguageCodeList()
        {
            string res = null;
            try
            {
                res = Core.Resource(Core.JaffaCulture).GetString("SUPPORT_LIST");
            }
            catch (Exception es)
            {
                Logging.Write(Logging.ExceptionToList(es), LogType.Error);
            }
            if (res == null)
            {
                return Array.Empty<string>();
            }
            string[] langs = res.Split(new char[] { '\n' });
            List<string> rt = new();
            foreach (string lang in langs)
            {
                string[] s = lang.Split(new char[] { ',', '\r', '\n' });
                if (s[0].Equals("Auto") == false)
                {
                    rt.Add(s[0]);
                }
            }
            return rt.ToArray();
        }

        #endregion

        #region カルチャー名に対応する表示名を取得 (GetDisplayLanguageName)

        /// <summary>
        /// カルチャー名に対応する表示名を取得します。
        /// </summary>
        /// <param name="culture">カルチャー名</param>
        /// <returns>表示名</returns>
        public static string GetDisplayLanguageName(string culture)
        {
            string res = null;
            try
            {
                res = Core.Resource(Core.JaffaCulture).GetString("SUPPORT_LIST");
            }
            catch { }
            if (res == null)
            {
                return culture;
            }
            string[] langs = res.Split(new char[] { '\n' });
            string rt = culture;
            foreach (string lang in langs)
            {
                string[] s = lang.Split(new char[] { ',', '\r', '\n' });
                if (s[0].Equals(culture))
                {
                    if (s[1].Contains("{", StringComparison.CurrentCulture))
                    {
                        s[1] = ConvertCurrentCultureResourceString(Core.Jaffa, s[1]);
                    }
                    rt = s[1];
                }
            }
            return rt;
        }

        #endregion

        #region リソースとマッチするカルチャー名を取得 (GetResourceCultureName)

        /// <summary>
        /// Jaffaフレームワークのリソースからカルチャー名を検索し、
        /// 最もマッチするカルチャー名を取得します。
        /// </summary>
        /// <param name="name">マッチングするカルチャー名</param>
        /// <returns>リソース上のカルチャー名</returns>
        public static string GetResourceCultureName(string name)
        {
            string rt = "";
            string res = null;
            try
            {
                res = Core.Resource(Core.JaffaCulture).GetString("SUPPORT_LIST");
            }
            catch { }
            if (res == null)
            {
                rt = name;
            }
            else
            {
                string[] langs = res.Split(new char[] { '\n' });
                foreach (string lang in langs)
                {
                    string[] s = lang.Split(new char[] { ',', '\r', '\n' });
                    s[0] = s[0].Trim();
                    if (!s[0].Equals("Auto"))
                    {
                        if (rt.Equals(""))
                        {
                            rt = s[0];
                        }
                        if (s[0].Equals(name))
                        {
                            // 完全一致
                            rt = name;
                            break;
                        }
                        if (s[0].IndexOf(name + "-") == 0)
                        {
                            // 前方一致
                            rt = s[0];
                        }
                    }
                }

            }

            // 初回は起動時のカルチャー名として記憶
            if (startupCulture.Equals(""))
            {
                startupCulture = rt;
                currentCultureSetting = rt;
                currentCulture = rt;
            }

            return rt;
        }

        #endregion

        #region 現在のカルチャーを変更 (ChangeCultureFromDisplayLanguageName)

        /// <summary>
        /// 現在のカルチャーを言語名で変更します。
        /// </summary>
        /// <param name="name">表示名またはカルチャー名</param>
        public static void ChangeCultureFromDisplayLanguageName(string name)
        {
            // 表示名からカルチャー名を取得
            string res = null;
            try
            {
                res = Core.Resource(Core.JaffaCulture).GetString("SUPPORT_LIST");
            }
            catch { }
            if (res == null)
            {
                return;
            }
            string[] langs = res.Split(new char[] { '\n' });
            foreach (string lang in langs)
            {
                string[] s = lang.Split(new char[] { ',', '\r', '\n' });
                if (s[1].Contains("{", StringComparison.CurrentCulture))
                {
                    s[1] = ConvertCurrentCultureResourceString(Core.Jaffa, s[1]);
                }
                if (s[1].Equals(name) || s[0].Equals(name))
                {
                    string saveCulture = currentCulture;
                    currentCultureSetting = s[0];
                    if (s[0].Equals("Auto"))
                    {
                        currentCulture = startupCulture;
                    }
                    else
                    {
                        currentCulture = s[0];
                    }
                    if (saveCulture != currentCulture || isChangeCulture)
                    {
                        isChangeCulture = false;

                        // カルチャー変更通知イベント
                        OnCultureChanged();

                        break;
                    }
                }
            }
        }
        private static bool isChangeCulture = true;

        #endregion

        #region 文字列内のリソース指定を現在のカルチャーで変換 (ConvertCurrentCultureResourceString)

        /// <summary>
        /// 文字列内のリソース指定を現在のカルチャーで変換します。
        /// </summary>
        /// <param name="region">リソース領域</param>
        /// <param name="text">テキスト</param>
        /// <remarks>
        /// テキスト内の {resource-name} を現在のカルチャーに対応するリソースから変換します。
        /// </remarks>
        static public string ConvertCurrentCultureResourceString(string region, string text)
        {
            ResourceManager resManager = Core.Resource(region);
            string[] tl = text.Split(new char[] { '{' });
            StringBuilder rt = new();
            foreach (string t in tl)
            {
                int s = t.IndexOf('}');
                if (s < 0)
                {
                    if (rt.Length > 0) rt.Append('{');
                    rt.Append(t);
                }
                else
                {
                    try
                    {
                        rt.Append(resManager.GetString(t.Substring(0, s)));
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.Print(e.Message);
                    }
                    rt.Append(t[(s + 1)..]);
                }
            }
            return rt.ToString();
        }

        #endregion

        #endregion

        #region プロパティ

        #region 起動時のカルチャー名を取得 ([R] startupCulture) [Private]

        /// <summary>
        /// 起動時のカルチャー名を取得します。
        /// </summary>
        private static string startupCulture = "";

        #endregion

        #region 現在の設定カルチャー名を取得 ([R] CurrentCultureSetting)

        /// <summary>
        /// 現在の設定カルチャー名を取得します。CurrentCultureプロパティとの違いは "Auto" の有無です。
        /// </summary>
        public static string CurrentCultureSetting
        {
            get
            {
                return currentCultureSetting;
            }
        }
        private static string currentCultureSetting = "";

        #endregion

        #region 現在のカルチャー名を取得 ([R] CurrentCulture)

        /// <summary>
        /// 現在のカルチャー名を取得します。
        /// </summary>
        public static string CurrentCulture
        {
            get
            {
                return currentCulture;
            }
        }
        private static string currentCulture = "";

        #endregion

        #endregion
    }
}
