   M S B A M L   `   `   `  ����C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Workload.PropertiesWorkload.Properties  5   	   #clr-namespace:WorkloadWorkload  5   	       Workload.SplashScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   8
properties!clr-namespace:Workload.Properties     5   	   (localclr-namespace:Workload     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   -   5        $��Splash��5   	     � AllowsTransparency  .  � WindowStyle$
 None=�5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	   
������ $
��0.01q�5   	    � Title$ SplashScreen��5   	   $	��350��6   $	��600��6+    � ShowInTaskbar .   � 	Resources 5         Workload.ColorToBrush   ColorToBrush��%   &	          5   
   6   5       � Triggers 5      :� 5   
    :� RoutedEvent$ Loaded��6   [�5      �� 5      .��5      �� 	 g� Duration$	 ForeverD�6!   .��5       W� 
 �� 
TargetName$
 
grPrgName1��6*   $	 0:0:1D�6M    W� From$ 0X�6^   	 W� To$ 1X�6g    �� TargetProperty$ Offset�6n    g� AutoReverse .   g� RepeatBehavior$ 1x��6�    W� EasingFunction 5!      /  )System.Windows.Media.Animation.CircleEase  5"   "   7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode$ EaseIn=�6-   5$      W� 5%      $	 0:0:1D�6*   $ 0X�6;   $ 1X�6D   $ Offset�6K   $
 
grPrgName2��6n    .  $ 1x��6�    5&      4  .System.Windows.Media.Animation.ExponentialEase  5'   "   $ EaseOut=�62   5)      W� 5+      $
 MadeByLabel��6*   $ Opacity�6N   $	 0.0X�6r   $ 1X�6}   $	 0:0:0.75D�6�    .  $ 1x��6�   W� 5,      $
 TalismanImage��6*   $ Opacity�6P   $	 0.0X�6t   $ 1X�6   $	 0:0:0.1D�6�    .  $ 1x��6�   W� 5-      $

 Blur��6*   $ Radius�6G   $	 100X�6j   $ 0X�6u   $	 0:0:0.75D�6|    .  $ 1x��6�    5.        5/   "   $ EaseOut=�6-   51      t� 53      $
 ProgramNameLabel��6-   $ Margin�6V    t� From$ 0,10,0,0X�6y   	 t� To$ 	10,10,0,0X�6�   $	 0:0:1D�6�    .  $ 1x��6�    t� EasingFunction 54        55   "   $ EaseOut=�6-   57      �� 58      $ 1x��6%   .��59      t� $
 BackRect��61   $ Margin�6R   $ 	0,0,600,0X�6u   $ 100,0,100,0X�6�   $	 0:0:0.75D�6�    .   5:   "     5;   &   $ EaseIn=�61   5=      t� 5>      $
 BackRect��61   $ Margin�6R   $ 100,0,100,0X�6u   $ 	600,0,0,0X�6�   $	 0:0:0.75D�6�    .   g� 	BeginTime$ 0:0:0.75X�6�    5?   "     5@   &   $ EaseOut=�61   5B      t� 5C      $
 BackRect��61   $ Margin�6R   $ 	0,0,600,0X�6u   $ 0,0,0,0X�6�   $	 0:0:0D�6�    .  $ 0:0:1.5X�6�   W� 5D      $
 BackRect��6.   $ Opacity�6O   $	 0.0X�6s   $	 1.0X�6~   $ 0:0:1.5X�6�   $	 0:0:0.25D�6�   W� 5E      $
 BackRect��6.   $ Opacity�6O   $	 1.0X�6s   $	 0.0X�6~   $ 0:0:3X�6�   $	 0:0:0.5D�6�   W� 5F      $
 BackRect��6.   $ Opacity�6O   $ 0X�6s   $	 1.0X�6|   $ 0:0:3.5X�6�   $	 0:0:0D�6�   5G      �� 5H      $ Forever��6%   $ 0:0:3.5X�6>   .��5I      t� $
 BackRect��61   $ Margin�6R   $ 	0,0,600,0X�6u   $ 100,0,100,0X�6�   $	 0:0:0.75D�6�    .   5J   "     5K   &   $ EaseIn=�61   5M      t� 5N      $
 BackRect��61   $ Margin�6R   $ 100,0,100,0X�6u   $ 	600,0,0,0X�6�   $	 0:0:0.75D�6�    .  $ 0:0:0.75X�6�    5O   "     5P   &   $ EaseOut=�61   5R      5S      5T      5U      5W      .��5Z      � 
������� � ColumnDefinitions 5^   
   �� 5_      �� 5`      .+�5b   
   �� -   5         ��Name$ BackRect��5b      $��60��6&   $��2��62   ��6F   �� "  Workload.Properties.Settings   Default �� Source# Z  �� Path$ AccentColor� �� Mode$ OneWay=� �� 	Converter# [  �� 5c   
   -   5         ��Name$ ProgramNameLabel��5c   M    �� AcceptsReturn .  �� TextWrapping$
 Wrap=�6'     �� 
IsReadOnly  . $/��)Розподіл Навантаження��6g   $
��Left=�6�   $��	10,10,0,0q�6�   $	��Top=�6�   $	��110��6�   $	��369��6�   $��54(�6�   $��0��6�   $��Microsoft Sans Serif)�6  $��48(�6-  $��2��6;  $
��Bold"�6O  $��0q�6a  ��6u  Z� .��6�  ��5d      �� 5e      ! �� EndPoint$! 1,0.5&�6&   " �� 
StartPoint$" 0,0.5&�67   .��5f      � -   5        
grPrgName1��# � Color$# Black��5f   7   $ � Offset$$ 1Q�6E   � 5g      -   5        
grPrgName2��$# White��5g   7   $$ 1Q�6E   5h      .��5i   "   5j      �� 5k   
   -   5        % ��Name$% MadeByLabel��5k      $��Кафедра АТЕП��6%   $
��Left=�6<   $��	10,0,0,10q�6W   $��Bottom=�6j   $��Microsoft Sans Serif)�6�   $��16(�6�   �� 5l   
   -   5        & ��Name$& TalismanImage��5l      $��0,0,10,0q�6'   $��Bottom=�69   $��Stretch=�6T   $0��*Media/talisman_cropped_and_back_erased.png��6r   $��Uniform=�6�   $��1��6�   ' J� Opacity$' 1Q�6�   ( J� Effect( 5m      -  'System.Windows.Media.Effects.BlurEffect  5n      )   Radius$) 0Q�6   -   5        Blur��5n   (   5p      5q      5r      