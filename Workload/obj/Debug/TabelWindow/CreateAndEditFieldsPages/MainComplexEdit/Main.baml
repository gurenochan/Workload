   M S B A M L   `   `   `  ����C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullm;clr-namespace:Workload.TabelWindow.CreateAndEditFieldsPages-Workload.TabelWindow.CreateAndEditFieldsPages  5      @    :Workload.TabelWindow.CreateAndEditFieldsPages.MainEditForm   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Mlocal;clr-namespace:Workload.TabelWindow.CreateAndEditFieldsPages     5        N� Title$  MainEditForm��5	      .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      �� 5      $��25 �6   �� 5      �� 5      $��25 �6   .+�5   
   �� $��Предмет��6   $
��Left=�6"   $��Center=�6=   $��0��6X   $��0��6h   �� 5   
   -   5         ��Name$ SubjectNameLabel��5      $�� ��6*   $
��Left=�65   $��Center=�6P   $��1��6k   $��0��6{   �� 5   
   $��
Обсяг��6   $
��Left=�6    $��Center=�6;   $��0��6V   $��4��6f   �� 5   
   -   5         ��Name$ 
AmountText��5      $��1��6&   $��4��66   $��Center=�6C   $��Stretch=�6^      ValidationTip#��[  6|   .��6�   ��5      �� 5       �� Path$ Amount�6    �� UpdateSourceTrigger$ PropertyChanged=�6(    �� ValidatesOnDataErrors .  �� ValidationRules 5      5  /System.Windows.Controls.DataErrorValidationRule  5       5"      .��5#      5$      &   System.Windows.Controls.DataGrid  5&   
   -   5        	  Name$	 AppliedWorksList��5&      $��1��6-   $��0��6:   $��2��6J   $��Stretch=�6^   $��Stretch=�6z   
   AutoGenerateColumns
 .     CanUserAddRows .  .@�5'      4  .System.Windows.Controls.DataGridTemplateColumn     CellEditingTemplate 5(      �� 5)      .R�5*      �� #��[  6$   $
��Auto��6K   $
��Autoq�6Y   5+      5-         Columns 5.      0  *System.Windows.Controls.DataGridTextColumn  5/      ,  &System.Windows.Controls.DataGridColumn   Header$ Вид роботи��6%      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� $ WORKS_TBL.WORK_NAME�6f     50      $ к-ть годин��6%    .   69   �� $ HOURS�6X   52      �� 54   
   -   5         ��Name$ AddDetailButton��54      $��▲ Add ▲��6*   $��0��6<   $��2��6L   $��Stretch=�6Y   $��Stretch=�6w   $��25,2.5q�6�   {�.  6�   �� 55   
   -   5        $ RemoveDetailButton��55      $��▼ Remove ▼��6-   $��1��6B   $��2��6R   $��Stretch=�6_   $��Stretch=�6}   $��25,2.5q�6�   {�.  6�   �� 57   
   -   5         ��Name$ UnappliedWorksList��57      $��3��6/   $��0��6<   $��2��6L   $��Stretch=�6`   $��Stretch=�6|    �� DisplayMemberPath$ 	WORK_NAME��6�   58      59      