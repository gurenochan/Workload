   M S B A M L   `   `   `  ���� C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:WorkloadWorkload      Workload.TablePage   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     (localclr-namespace:Workload       N� Title$  	TablePage��.��� -    �Name$ MainGrid��
������� � RowDefinitions �� $��35 ��� �� $��35 � � ColumnDefinitions �� �� -    ��Name$ CreateEditColumn��$	��300 �.+�]� -    ]�Name$ ButtonsDock��$��0��$��0��$��2��$��Stretch=�$��Stretch=�
������ .+��� -    ��Name$ 	ImportBut��$��Імпортувати��$
��Left=�$��Center=�$��10,0,0,0q�$	��100���� -   $ 	ExportBut��$��Експортувати��$
��Left=�$��Center=�$��10,0,0,0q�$	��100��$y�	Collapsed=��� -   $ PrintBut��$y�	Collapsed=�$��Роздрукувати��$
��Left=�$��Center=�$��10,0,0,0q�$	��100���� -   $ DelBut��$��Видалити��$
��Left=�$��Center=�$��10,0,0,0q�$��75���� -   $ AddBut��$��Додати��$
��Left=�$��Center=�$��10,0,0,0q�$��75��&   System.Windows.Controls.DataGrid  -	     Name$ 	tableGrid��$��Stretch=�$��1��$��0��$��2��$��Stretch=�   SelectionMode$ Single=�	   
IsReadOnly	 . 
   CanUserDeleteRows
 .    AutoGenerateColumns .  
������� � -
     �Name$ CreateEditPanel��$��1��$��1��$��Stretch=�$��Stretch=�$
��5, 5q�]� -   $ OkCreateDock��$��1��$��2��$��Stretch=�$��Stretch=�.+��� -   $ OkBut��$'��!Застосувати зміни��$
��Left=�$��Center=�$��10,0,0,0q�$	��125��{�.  $��0��$��0���� -   $ CompleteCreateBut��$��Створити��$��Right=�$��Center=�$��0,0,10,0q�$��75��$��1��$��0��{�.  