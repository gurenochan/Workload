   M S B A M L   `   `   `  ����C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Workload.TabelWindowWorkload.TabelWindow  5      '    !Workload.TabelWindow.ExcelReports   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:Workload.TabelWindow     5        N� Title$  Reports��5	      .��5
      �  � ColumnDefinitions 5   
   �� 5      $	��100 �6   �� 5       � RowDefinitions 5   
   �� 5      �� 5      $
��0.4* �6   .+�5   
    � -   5        $��
paramFrame��5   >   $��0��6   $��0��6   $��2��6-   6>   R� 5   
   -   5         R�Name$ ReportTypeTree��5      $��0��6+   $��1��68   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ReportChoosedItem��5      $��1��6.   $��1��6;   {�.     
IsReadOnly .    AutoGenerateColumns .     SelectionMode$ Single=�6�      SelectionUnit$ FullRow=�6�   5      5      