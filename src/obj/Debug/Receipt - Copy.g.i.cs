#pragma checksum "..\..\Receipt - Copy.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B70CE9919A67C96AD91F0E74BD3A4C825DA2DEA24FDCB2080EFD18F5A0ADA908"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using accounts;


namespace accounts
{


    /// <summary>
    /// Receipt
    /// </summary>
    public partial class payment : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {


#line 14 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_cramount;

#line default
#line hidden


#line 20 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_rno;

#line default
#line hidden


#line 21 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_narration;

#line default
#line hidden


#line 22 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_rnofind;

#line default
#line hidden


#line 23 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_find;

#line default
#line hidden


#line 24 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_save;

#line default
#line hidden


#line 25 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Reset;

#line default
#line hidden


#line 26 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_update;

#line default
#line hidden


#line 27 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_del;

#line default
#line hidden


#line 28 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmb_cashaccount;

#line default
#line hidden


#line 29 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmb_craccount;

#line default
#line hidden


#line 30 "..\..\Receipt - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dtp_rdate;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/accounts;component/receipt%20-%20copy.xaml", System.UriKind.Relative);

#line 1 "..\..\Receipt - Copy.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.txt_cramount = ((System.Windows.Controls.TextBox)(target));
                    return;
                case 2:
                    this.txt_rno = ((System.Windows.Controls.TextBox)(target));
                    return;
                case 3:
                    this.txt_narration = ((System.Windows.Controls.TextBox)(target));
                    return;
                case 4:
                    this.txt_rnofind = ((System.Windows.Controls.TextBox)(target));

#line 22 "..\..\Receipt - Copy.xaml"
                    this.txt_rnofind.KeyDown += new System.Windows.Input.KeyEventHandler(this.txt_rnofind_KeyDown);

#line default
#line hidden
                    return;
                case 5:
                    this.btn_find = ((System.Windows.Controls.Button)(target));
                    return;
                case 6:
                    this.btn_save = ((System.Windows.Controls.Button)(target));

#line 24 "..\..\Receipt - Copy.xaml"
                    this.btn_save.Click += new System.Windows.RoutedEventHandler(this.btn_save_Click);

#line default
#line hidden
                    return;
                case 7:
                    this.btn_Reset = ((System.Windows.Controls.Button)(target));

#line 25 "..\..\Receipt - Copy.xaml"
                    this.btn_Reset.Click += new System.Windows.RoutedEventHandler(this.btn_Reset_Click);

#line default
#line hidden
                    return;
                case 8:
                    this.btn_update = ((System.Windows.Controls.Button)(target));

#line 26 "..\..\Receipt - Copy.xaml"
                    this.btn_update.Click += new System.Windows.RoutedEventHandler(this.btn_update_Click);

#line default
#line hidden
                    return;
                case 9:
                    this.btn_del = ((System.Windows.Controls.Button)(target));

#line 27 "..\..\Receipt - Copy.xaml"
                    this.btn_del.Click += new System.Windows.RoutedEventHandler(this.btn_del_Click);

#line default
#line hidden
                    return;
                case 10:
                    this.cmb_cashaccount = ((System.Windows.Controls.ComboBox)(target));
                    return;
                case 11:
                    this.cmb_craccount = ((System.Windows.Controls.ComboBox)(target));
                    return;
                case 12:
                    this.dtp_rdate = ((System.Windows.Controls.DatePicker)(target));
                    return;
            }
            this._contentLoaded = true;
        }
    }
}

