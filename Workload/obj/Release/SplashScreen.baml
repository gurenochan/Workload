   M S B A M L   `   `   `  ���� C  ?Workload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Workload.PropertiesWorkload.Properties  #clr-namespace:WorkloadWorkload      Workload.SplashScreen   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8
properties!clr-namespace:Workload.Properties     (localclr-namespace:Workload     2gif"http://wpfanimatedgif.codeplex.com     -   $��Splash��  � AllowsTransparency  .  � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�
������ $
��0.01q� � Title$ SplashScreen��$	��350��$	��600�� � ShowInTaskbar .   � 	Resources    Workload.ColorToBrush   ColorToBrush��%   &	           � Triggers :�  :� RoutedEvent$ Loaded��[��� .���� 	 g� Duration$	 ForeverD�.��W� 
 �� 
TargetName$
 
grPrgName1��$	 0:0:1D� W� From$ 0X�	 W� To$ 1X� �� TargetProperty$ Offset� g� AutoReverse .   g� RepeatBehavior$ 1x�� W� EasingFunction /  )System.Windows.Media.Animation.CircleEase  7  1System.Windows.Media.Animation.EasingFunctionBase   
EasingMode$ EaseIn=�W� $	 0:0:1D�$ 0X�$ 1X�$ Offset�$
 
grPrgName2�� .  $ 1x�� 4  .System.Windows.Media.Animation.ExponentialEase  $ EaseOut=�W� $
 MadeByLabel��$ Opacity�$	 0.0X�$ 1X�$	 0:0:0.75D� .  $ 1x��W� $
 TalismanImage��$ Opacity�$	 0.0X�$ 1X�$	 0:0:0.1D� .  $ 1x��W� $

 Blur��$ Radius�$	 100X�$ 0X�$	 0:0:0.75D� .  $ 1x��   $ EaseOut=�t� $
 ProgramNameLabel��$ Margin� t� From$ 0,10,0,0X�	 t� To$ 	10,10,0,0X�$	 0:0:1D� .  $ 1x�� t� EasingFunction   $ EaseOut=��� $ 1x��.��t� $
 BackRect��$ Margin�$ 	0,0,600,0X�$ 100,0,100,0X�$	 0:0:0.75D� .     $ EaseIn=�t� $
 BackRect��$ Margin�$ 100,0,100,0X�$ 	600,0,0,0X�$	 0:0:0.75D� .   g� 	BeginTime$ 0:0:0.75X�   $ EaseOut=�t� $
 BackRect��$ Margin�$ 	0,0,600,0X�$ 0,0,0,0X�$	 0:0:0D� .  $ 0:0:1.5X�W� $
 BackRect��$ Opacity�$	 0.0X�$	 1.0X�$ 0:0:1.5X�$	 0:0:0.25D�W� $
 BackRect��$ Opacity�$	 1.0X�$	 0.0X�$ 0:0:3X�$	 0:0:0.5D�W� $
 BackRect��$ Opacity�$ 0X�$	 1.0X�$ 0:0:3.5X�$	 0:0:0D��� $ Forever��$ 0:0:3.5X�.��t� $
 BackRect��$ Margin�$ 	0,0,600,0X�$ 100,0,100,0X�$	 0:0:0.75D� .     $ EaseIn=�t� $
 BackRect��$ Margin�$ 100,0,100,0X�$ 	600,0,0,0X�$	 0:0:0.75D� .  $ 0:0:0.75X�   $ EaseOut=�.��� 
������� � ColumnDefinitions �� �� .+��� -    ��Name$ BackRect��$��60��$��2������ "  Workload.Properties.Settings   Default �� Source# Z  �� Path$ AccentColor� �� Mode$ OneWay=� �� 	Converter# [  �� -    ��Name$ ProgramNameLabel�� �� AcceptsReturn .  �� TextWrapping$
 Wrap=�  �� 
IsReadOnly  . $/��)Розподіл Навантаження��$
��Left=�$��	10,10,0,0q�$	��Top=�$	��110��$	��369��$��54(�$��0��$��Microsoft Sans Serif)�$��48(�$��2��$
��Bold"�$��0q���Z� .������ ! �� EndPoint$! 1,0.5&�" �� 
StartPoint$" 0,0.5&�.��� -   
grPrgName1��# � Color$# Black��$ � Offset$$ 1Q�� -   
grPrgName2��$# White��$$ 1Q�.���� -   % ��Name$% MadeByLabel��$��Кафедра АТЕП��$
��Left=�$��	10,0,0,10q�$��Bottom=�$��Microsoft Sans Serif)�$��16(��� -   & ��Name$& TalismanImage��$��0,0,10,0q�$��Bottom=�$��Stretch=�$0��*Media/talisman_cropped_and_back_erased.png��$��Uniform=�$��1��' J� Opacity$' 1Q�( J� Effect( -  'System.Windows.Media.Effects.BlurEffect  )   Radius$) 0Q�-   Blur��