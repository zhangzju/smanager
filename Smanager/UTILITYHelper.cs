using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smanager
{
    class UTILITYHelper
    {
        public static string ERROR = "Error Message";

        public static void ShowError(string error)
        {
            System.Windows.Forms.MessageBox.Show(error, UTILITYHelper.ERROR, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
        public static string getRandomizer(int length, bool useNum, bool useLow, bool useUpp, bool useSpe)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = null;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        public static bool checkMask(string mask)
        {
            string[] vList = mask.Split('.');
            if (vList.Length != 4) return false;

            bool vZero = false; // 出现0 
            for (int j = 0; j < vList.Length; j++)
            {
                int i;
                if (!int.TryParse(vList[j], out i)) return false;
                if ((i < 0) || (i > 255)) return false;
                if (vZero)
                {
                    if (i != 0) return false;
                }
                else
                {
                    for (int k = 7; k >= 0; k--)
                    {
                        if (((i >> k) & 1) == 0) // 出现0 
                        {
                            vZero = true;
                        }
                        else
                        {
                            if (vZero) return false; // 不为0 
                        }
                    }
                }
            }

            return true;
        } 
    }
}
