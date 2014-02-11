﻿using System.Windows;
using Demo.Core;
using BoonieBear.TinyMetro.WPF.Controller;

namespace Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // At first, a instance of the conrecte Kernel has to be created and set
            Kernel.Instance = new TestKernal();

            // The initialization of the kernel is optional, but maybe necessary for your concrete implementation
            Kernel.Instance.Controller.Init();
            base.OnStartup(e);
        }
    }
}
