﻿using System;
using Tono.Gui.Uwp;
using Windows.UI.Xaml.Controls;

namespace JitStreamDesigner
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            // Welcome message
            var ver = Windows.ApplicationModel.Package.Current.Id.Version;
            LOG.AddMes(LLV.INF, "Start-Welcome", $"{ver.Build}.{ver.Major}.{ver.Minor}.{ver.Revision}", DateTime.Now.Year);
            LOG.AddMes(LLV.INF, new LogAccessor.Image { Key = "Lump16" }, "Start-Quickhelp");
        }
    }
}