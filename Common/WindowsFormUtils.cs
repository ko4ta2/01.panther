using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Common
{
    public static class WindowsFormUtils
    {
        /// <summary>
        /// 対象コントロールのフォームからの絶対座標を取得します
        /// </summary>
        /// <param name="ctrl">コントロール</param>
        /// <param name="pt">絶対座標</param>
        public static void AbsolutePositionFromForm(Control ctrl, ref Point pt)
        {
            pt.X += ctrl.Left;
            pt.Y += ctrl.Top;

            // 親がFormになるまで再帰的処理します。
            if(ctrl.Parent != null && !( ctrl.Parent is Form ))
                AbsolutePositionFromForm(ctrl.Parent, ref pt);

            return;
        }
    }
}
