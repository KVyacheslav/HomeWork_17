   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   F BHomeWork_17, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:HomeWork_17HomeWork_17 5   	   %   HomeWork_17.WindowAddBankCredit   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   +localclr-namespace:HomeWork_17     5   	   -   5          � WindowStyle$
  None=�5	   	    � 
ResizeMode$ NoResize=�5
   	   $	��430��5   	   $	��300��5   	   $��Rostov)�5   	   ��5      �� 5   
    �� ImageSource$ background.jpg��6    � 	Resources 5      �� 5   
      Key
ArrayTimes   �� Type" ��6%   .s�5      ��1 год��6    ��5      
3 года��6!   ��5      5 лет��6    5      �� 5   
   ArrayPercentes  " ��6)   .s�5      ��30 %��6   ��5      &"20 % (Для VIP клиентов)��62   5      .��5       � .+�5!   
   �� .+�5"      �� $!��Расчётный счёт��6   $��Center=�5#      $��10q�5$      �� 5&      $��Срок��6   $��10q�5'      �� 5(      -   5         ��Name$ cbTimeCredit��5(        
ArrayTimes#��[ 5)       �� SelectedIndex$ 0��5*      $��10q�5,      �� 5.      $��Процент��6   $��10q�5/      �� 50      -   5        $ 	cbPercent��50        ArrayPercentes#��[ 51      $ 0��53      $��10q�54      �� 56      $#��Расчётные счета��6   $��10q�57      �� 58      -   5        $ cbBankAccounts��58      $��10q�5:      �� 5=       �� Orientation$ 
Horizontal=�6   .+�5>      �� $ ��Сумма кредита:��6   $��90��5?      $��10q�5@      �� 5A      -   5         ��Name$ 	tbBalance��5A      $
��1000��5B      $��10q�5C      5D      �� 5E      -   5        	 ��Name$	 	slBalance��5E      $��10q�5F      
 � Minimum$	
 500Q�5G       � Maximum 5H      �� *Sum+5I      �� 5K      -   5         ��Name$ btnAddBankCredit��5K      $��Center=�5L      $	��100��5M      $��25��5N      $
��10 5q�5O      $��Добавить��5P      5Q      �� 5S      -   5        $��Center=�5S      $	��100��5T      $��25��5U      $
��10 5q�5V      $��Закрыть��5W      5X      5Y      5Z      5[      