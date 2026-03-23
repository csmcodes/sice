using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace WinServiceSICE
{
    [RunInstaller(true)]
    public partial class Instalador : System.Configuration.Install.Installer
    {
        public Instalador()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
