using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Autolithium
{
    public static class Winform
    {
        public static void ALERT(string Msg)
        {
            System.Windows.Forms.MessageBox.Show(Msg);
        }
        public static string INPUTBOX(string Title, string Prompt, string Default, int? XPos, int? YPos)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(Prompt, Title, Default ?? "", XPos ?? -1, YPos ?? -1);
        }
    }
}
