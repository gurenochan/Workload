   M S B A M L   `   `   `  ���� C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullm;clr-namespace:Workload.TabelWindow.CreateAndEditFieldsPages-Workload.TabelWindow.CreateAndEditFieldsPages  9!clr-namespace:Workload.PropertiesWorkload.Properties  @    :Workload.TabelWindow.CreateAndEditFieldsPages.WorkLoadEdit   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Mlocal;clr-namespace:Workload.TabelWindow.CreateAndEditFieldsPages     8
properties!clr-namespace:Workload.Properties       N� Title$  WorkLoadEdit�� � 	Resources =   7Workload.TabelWindow.CreateAndEditFieldsPages.FreeHours   	FreeHours:   4Workload.TabelWindow.CreateAndEditFieldsPages.Groups 
 Groups@   :Workload.TabelWindow.CreateAndEditFieldsPages.HoursOnTutor  HoursOnTutorN   HWorkload.TabelWindow.CreateAndEditFieldsPages.HoursOnTutorFormatedString  HoursOnTutorFormatedString��%<   &	        &	      &	 
     &	              .���  � RowDefinitions �� �� $��25 ���  � ColumnDefinitions �� $	��100 ��� $
��0.5* ��� $
��0.5* ��� $	��300 �.+� � -     �Name$ ParametersFrame��$��0��$��0��$��3��$��Stretch=�$��Stretch=�&   System.Windows.Controls.DataGrid  -     Name$ SubdetailsGrid��$��0��$��1��$��2��$��1.5,1.5q�   SelectionMode$ Single=�   AutoGenerateColumns .  {�.  .@�4  .System.Windows.Controls.DataGridTemplateColumn     CellEditingTemplate �� .R���   ValidationTip#��[ $
��Auto��$
��Autoq�	   Columns	 0  *System.Windows.Controls.DataGridTextColumn  -   	SubTutCol��,  &System.Windows.Controls.DataGridColumn
   Header$
 Викладач��   
IsReadOnly . 1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding ��  �� Path$ TEACHERS_TBL.TEACHER_NAME�  -   
SubHourCol��$
 Години�� .   �� $ HOURS�  $
 
Групи��-   GroupCol�� .  ��  �� 	Converter# [ �� $)��#Доступні викладачі��$��1��$��1��$��Stretch=�$��Center=�$��Center=�$��Stretch=�  -   $ AvaliebleTutors��$��2��$��1��$��1��$��1.5,1.5q�$ Single=�   
IsReadOnly .  .  	   $
 ПІБ�� .  �� $ TEACHER_NAME�  $
 Години�� .    MinWidth$ 80Q�   CellTemplate �� .R�� .+��  � Minimum$ 0Q�$��Stretch=�$��Stretch=� � Maximum �� "
  Workload.Properties.Settings 
  Default �� Source# Z $ MaxHoursPerTeacher� �� Mode$ OneWay=� � Value �� # [ $ OneWay=��� ���� # [ $ OneWay=��� $��Набори груп��$��1��$��2��$��Stretch=�$��Center=�$��Center=�$��Stretch=� � -   $ GroupSelectFrame��$��2��$��2��$��1.5,1.5q�{�.    -   $ 	MainsGrid��$��0��$��3��$��1.5,1.5q�$ Single=� .     CanUserAddRows .  	   $
 Дисципліна�� .  �� $ SUBJECTS_TBL.SUBJECT_NAME�  -	   
EduFormCol��$
 
Форма�� .  �� $ EDUFORMS_TBL.EDUFORM_NAME�  -
   
EduTypeCol��$
 Тип�� .  �� $ EDUTYPES_TBL.EDUTYPE_NAME�  -   	CourseCol��$
 Курс�� .  �� $ 	COURSE_NO�  -   SemesterCol��$
 Семестр�� .  �� $ SEMESTER_NO�  -   $ DetailsGrid��$��1��$��2��$��3��$��1.5,1.5q�$ Single=� .   .  	   $
 Вид роботи�� .  �� $ WORKS_TBL.WORK_NAME�  $
 Вільно�� .  �� # [  