﻿#pragma checksum "..\..\trialBalance.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B761F1E383CDF4859AE037C489CBF7FF3B3DACEA74883F057BBF9345F4BA9C37"
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


namespace accounts {
    
    
    /// <summary>
    /// trialBalance
    /// </summary>
    public partial class trialBalance : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button refresh_data;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox lblcr;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox lbldr;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid group_acc_grid;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_print;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_doc;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_excel;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_pdf;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\trialBalance.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_xps;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/bookkeeper;component/trialbalance.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\trialBalance.xaml"
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
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\trialBalance.xaml"
            ((accounts.trialBalance)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.refresh_data = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\trialBalance.xaml"
            this.refresh_data.Click += new System.Windows.RoutedEventHandler(this.refresh_data_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblcr = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.lbldr = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.group_acc_grid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 6:
            this.btn_print = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\trialBalance.xaml"
            this.btn_print.Click += new System.Windows.RoutedEventHandler(this.btn_print_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btn_doc = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\trialBalance.xaml"
            this.btn_doc.Click += new System.Windows.RoutedEventHandler(this.btn_doc_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btn_excel = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\trialBalance.xaml"
            this.btn_excel.Click += new System.Windows.RoutedEventHandler(this.btn_excel_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btn_pdf = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\trialBalance.xaml"
            this.btn_pdf.Click += new System.Windows.RoutedEventHandler(this.btn_pdf_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btn_xps = ((System.Windows.Controls.Button)(target));
            
            #line 73 "..\..\trialBalance.xaml"
            this.btn_xps.Click += new System.Windows.RoutedEventHandler(this.btn_xps_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

