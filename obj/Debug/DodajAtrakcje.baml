   M S B A M L   `   `   `  ����H  DAplikacjaBest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:AplikacjaBestAplikacjaBest  5   	   !    AplikacjaBest.DodajAtrakcje   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:AplikacjaBest     5   	     � Title$  Dodaj Nową Atrakcje��5   	   $	��200��6%   $	��200��62   $	��200��6B   $	��300��6R   $	��300��6^   $	��300��6m   .��5	      � -   5        $��Atrakcja��5	      $��10q�6    � ColumnDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��lblNazwa��5      $��Nazwa:��6!   $��0��62   $��0��6B   $��Right=�6O   �� 5   
   -   5        $��lblUprawnienia��5      $��Uprawnienie:��6&   $��0��6=   $��1��6M   $��Right=�6Z   �� 5   
   -   5        $��txtNazwa��5   )   $��1��6:   $��0��6J   $��2q�6W   ��6   �� *Nazwa+6W   �� 5   
   -   5        $��txtUprawnienie��5   M   $��1��60   $��1��6@   $��	30,5,30,5q�6c    �� Text 6   �� *Uprawnienia+.@�5      �� $��1��6   6)   �� 5      $��2��6   6)   �� 5      $��3��6   6)   5      � 5   
   $��1��6   $��3��6%   $��Right=�62   .+�5       �� -   5        $��btnOK��5       $��Dodaj��6"   $��3q�62   $��60��6=   6H   �� 5!      -   5        $��	btnCancel��5!      $��Anuluj��6&   $��3q�67   $��60��6B   6M   5"      5#      5%      