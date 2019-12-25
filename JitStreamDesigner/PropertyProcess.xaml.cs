﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tono;
using Tono.Gui.Uwp;
using Tono.Jit;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace JitStreamDesigner
{
    public sealed partial class PropertyProcess : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private JitProcess target;
        /// <summary>
        /// Target Jit Object
        /// </summary>
        public JitProcess Target
        {
            get => target;
            set
            {
                target = value;
                Name = target.ID;           // Control.Name to find chip
                InstanceID = target.ID;
                InstanceName = target.Name;
                X = $"{((Distance)target.ChildVriables["LocationX"].Value).m}m";
                Y = $"{((Distance)target.ChildVriables["LocationY"].Value).m}m";
                W = $"{((Distance)target.ChildVriables["Width"].Value).m}m";
                H = $"{((Distance)target.ChildVriables["Height"].Value).m}m";
            }
        }

        public string InstanceID
        {
            get => Target.ID;
            set => new NotSupportedException();
        }

        public Dictionary<string, object> PreviousValue { get; } = new Dictionary<string, object>();

        private string instanceName;
        /// <summary>
        /// ViewModel : Instance Name
        /// </summary>
        public string InstanceName
        {
            get => instanceName;
            set
            {
                if (instanceName != value)
                {
                    PreviousValue["InstanceName"] = instanceName;
                    instanceName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InstanceName"));
                }
            }
        }

        private string x = "0m";
        public string X
        {
            get => x;
            set
            {
                if (Distance.Parse(value) != Distance.Parse(x))
                {
                    PreviousValue["X"] = x;
                    x = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X"));
                }
            }
        }
        private string y = "0m";
        public string Y
        {
            get => y;
            set
            {
                if (Distance.Parse(value) != Distance.Parse(y))
                {
                    PreviousValue["Y"] = y;
                    y = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y"));
                }
            }
        }
        private string w = "0m";
        public string W
        {
            get => w;
            set
            {
                if (Distance.Parse(value) != Distance.Parse(w))
                {
                    PreviousValue["W"] = w;
                    w = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("W"));
                }
            }
        }
        private string h = "0m";
        public string H
        {
            get => h;
            set
            {
                if (Distance.Parse(value) != Distance.Parse(h))
                {
                    PreviousValue["H"] = h;
                    h = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("H"));
                }
            }
        }


        public PropertyProcess()
        {
            this.InitializeComponent();
        }

        private void Button_Round_Click(object sender, RoutedEventArgs e)
        {
            X = $"{Math.Round(Distance.Parse(X).m)}m";
            Y = $"{Math.Round(Distance.Parse(Y).m)}m";
            W = $"{Math.Round(Distance.Parse(W).m)}m";
            H = $"{Math.Round(Distance.Parse(H).m)}m";
        }
    }
}
