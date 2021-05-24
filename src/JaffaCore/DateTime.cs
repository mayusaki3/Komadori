using Jaffa.Diagnostics;
using System.Globalization;

namespace Jaffa
{
    /// <summary>
    /// Jaffaフレームワーク・DateTime拡張
    /// </summary>
    public class DateTime
    {
        #region メソッド

        #region 現在時刻とのオフセット値を設定 (CalcDifferenceNow)

        /// <summary>
        /// 現在時刻とのオフセット値を設定します。
        /// </summary>
        /// <param name="reference">基準時刻</param>
        public static void CalcDifferenceNow(System.DateTime reference)
        {
            DifferenceNow = reference.Subtract(System.DateTime.Now);
        }

        #endregion

        #endregion

        #region プロパティ

        #region オフセットされた現在時刻を参照 ([R] Now)

        /// <summary>
        /// オフセットされた現在時刻を参照します。
        /// </summary>
        public static System.DateTime Now
        {
            get
            {
                return System.DateTime.Now.Add(differenceNow);
            }
        }

        #endregion

        #region 現在時刻のオフセット値を参照または設定 ([R/W] DifferenceNow)

        /// <summary>
        /// 現在時刻のオフセット値を参照または設定または設定します。
        /// </summary>
        public static System.TimeSpan DifferenceNow
        {
            get
            {
                return differenceNow;
            }
            set
            {
                var fp = new CultureInfo(International.CurrentCulture);
                string from = Now.ToString(Core.MakeMessage(Core.Jaffa, "{TIME_FORMAT}"), fp);
                differenceNow = value;
                string to = Now.ToString(Core.MakeMessage(Core.Jaffa, "{TIME_FORMAT}"), fp);

                Logging.Write(Core.MakeMessage(Core.Jaffa, Messages.JFWI0002, new string[] { from, to }));
            }
        }
        private static System.TimeSpan differenceNow = new();

        #endregion

        #endregion
    }
}
