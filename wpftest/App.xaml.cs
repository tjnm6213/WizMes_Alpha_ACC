using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WizMes_Alpha_JA;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //mfont = "fonts/#궁서체";
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);
            LoadINI loini = new LoadINI();
            loini.loadINI();
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            //We dont' care about System Assembies and so on...
            //if (!args.Name.ToLower().StartsWith("Test")) return null;

            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            //Get the Name of the AssemblyFile
            var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

            //Load form Embedded Resources - This Function is not called if the Assembly is in the Application Folder
            var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(name));
            if (resources.Count() > 0)
            {
                var resourceName = resources.First();
                using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    var block = new byte[stream.Length];
                    stream.Read(block, 0, block.Length);
                    return Assembly.Load(block);
                }
            }
            return null;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the startup window
            MainWindow wnd = new MainWindow();
            // Do stuff here, e.g. to the window
            // Show the window
            wnd.Show();
        }

        //
        private void TextBoxZero_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringZero(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        //
        private void TextBoxOne_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringOne(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        //
        private void TextBoxTwo_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringTwo(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        private void TextBoxZero_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringZero(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        private void TextBoxOne_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringTwoExceptDot(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        private void TextBoxTwo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb != null)
            {
                if (Lib.Instance.IsNumOrAnother(tb.Text))
                {
                    tb.Text = Lib.Instance.returnNumStringTwoExceptDot(tb.Text);
                    tb.SelectionStart = tb.Text.Length;
                    sender = tb;
                }
            }
        }

        private void MouseLeftDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                try
                {
                    UserControl userControl = Lib.Instance.GetParent<UserControl>(sender as DataGrid);
                    if (userControl != null)
                    {
                        object objUpdate = userControl.FindName("btnUpdate");
                        object objEdit = userControl.FindName("btnEdit");

                        if (objUpdate != null)
                        {
                            if ((objUpdate as Button).IsEnabled == true)
                            {
                                (objUpdate as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            }
                        }
                        else if (objEdit != null)
                        {
                            if ((objEdit as Button).IsEnabled == true)
                            {
                                (objEdit as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
