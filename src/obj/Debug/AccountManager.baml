   M S B A M L   `   `   `  ����E  Abookkeeper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:accountsaccounts  5   	       accounts.AccountManager   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:accounts     5   	   -   5          � Title$  Account Manager��5   	   $��481.991��6!   $	��800��62    � WindowStartupLocation$ CenterOwner=�6>   
����İ� � 
ResizeMode$ NoResize=�6~    � WindowStyle$ SingleBorderWindow=�6�   .��5	      �  � RowDefinitions 5
   
   �� 5      $��40 �6   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      $��10 �6    � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $	��250 �6   �� 5      $��10 �6   �� 5      �� 5      �� 5      $��10 �6   .+�5   
   �� $��0��6   $��Center=�6!   $��1��6<   $��SELECT ACCOUNTS��6L      GoldeRodTextBlock#���   6c   
���   �$��DemiBold"�6�   �� 5   
   $��0��6   $��Center=�6!   $��3��6<   $��3��6L   $��TRANSACTIONS��6`    �� TextAlignment$ Center=�6t   #���   6�   
���   �$��DemiBold"�6�   �� 5   
   -   5         ��Name$ lst_ac��5   +    �� SelectionMode$ Multiple=�6     prent_and_groups_List#���  5      
������	 �� SelectedItem$	 SlectedAccount��6\   $��1��6�   $��1��6�   $��3��6�   ��6z   �� *
Accounts+��5!      �� 5#      .R�5)      �� .��5*      �� -   5        
 d� 	IsChecked
 5*   4   �� *	IsChecked+��6T   �� *
Short_Name+5+      5,      5.      &   System.Windows.Controls.DataGrid  50   
   -   5          Name$ 
detailGrid��50        PAYRGHeaderStyle#���  6'      
IsReadOnly . $��1��6d   $��2��6q   $��3��6�   $��5��6�      AutoGenerateColumns .   �� AlternationCount$ 2��6�      AlternatingRowBackground
 ����   Columns 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn   Header$ Account Name��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 6;   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6G   �� *CrAccount.Name+  53      $	 150 6%   $ 	Dr.Amount��61    6D   �� *	Dr_Amount+  54      $	 150 6%   $ 	Cr.Amount��61    6D   �� *	Cr_Amount+56      �� 58   
   $��Stretch=�6   $��1��59      $��2��6%    �� Orientation$ Vertical=�67   $��4��6N   .+�5:      �� $��5��6   d� 5;      -   5         d�Name$ tbtn_unselectall��5;      $��35��6T   $��14(�6`   $��Unselect all��6n   �� 5<      $��5��6   d� 5=      -   5        $ tbtn_��5=      $��35��6+   $��14(�67   $��
Select All��6E   6Z   �� 5>      $	��3.3��6   �� 5?      -   5         ��Name$ chk_clear_entries��5?      $��Clear Vouchers also��62   $��14(�6P   6a   �� 5@      -   5         ��Name$ 	btn_clear��5@      $��14(�6(   $��35��66   $��Clear Account��6B   6Z   5C      5G      5I      