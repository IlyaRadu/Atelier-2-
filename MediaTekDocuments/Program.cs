using MediaTekDocuments.view;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormLogin login = new FormLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FrmMediatek());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
