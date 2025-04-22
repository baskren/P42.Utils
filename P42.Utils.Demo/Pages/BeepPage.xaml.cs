using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace P42.Utils.Demo;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BeepPage : Page
{
    public BeepPage()
    {
        this.InitializeComponent();

        if (!P42.Utils.Uno.DeviceBeep.CanBeep)
        {
            TestBeepButton.IsEnabled = false;
            TestBeepButton.Content = "BEEP NOT AVAILABLE";
        }
    }

    async void TestBeepButton_Click(object sender, RoutedEventArgs e)
    {
        await P42.Utils.Uno.DeviceBeep.PlayAsync(4500, 300);
    }
}
