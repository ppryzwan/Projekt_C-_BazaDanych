   M S B A M L   `   `   `  ����H  DAplikacjaBest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:AplikacjaBestAplikacjaBest  5   	   *    $AplikacjaBest.HistoriaKarnetowKlient   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:AplikacjaBest     5   	     � Title$2  ,Historia Zakupionych Karnetów przez Klienta��5   	   $	��400��6=   $	��800��6J   $	��800��6V   $	��800��6e   $	��400��6t   $	��400��6�   .��5	      � $��Center=�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��AUTO �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6&   $��Center=�63   .+�5      �� -   5        $��lblNazwa��5      $��Wpisz numer klienta��6%   $��Right=�6C   {�.  6_   �� 5      -   5        $��txtWyszukaj��5      $��2q�6+   66   �� 5      -   5        $��btnZatwierdz��5      $��Zatwierdz wybor��6)   6C   5      �� 5   
   -   5        $��btnZmien��5      $��1��6!   $��0��61   $��Zmien na nazwisko��6>   $��20��6\   $	��120��6h   6t   �� 5   
   -   5        $��cmbdane��5      $��3q�6"   $��1��6-   $��0��6:    �� DisplayMemberPath$
 Dane��6J    �� SelectedValuePath$ 
ID_Klienta��6c   $y�Hidden=�6�   �� 5   
   -   5         ��Name$ 
lstKarnety��5      $��0��6'   $��2��67   $��3q�6D   {�.   �� View 5      �� 5       �� AllowsColumnReorder . .P�5      �� $��Nazwa��6%    �� DisplayMemberBinding 64   �� *Nazwa+�� 5       $
��Cena��6%    63   �� *Cena+�� 5!      $
��Opis��6%    63   �� *Opis+�� 5"      $��Data Zakupu Karnetu��6%    6B   �� *Data_Zakupu_Karnetu+�� 5#      $��Data Zakończenia Karnetu��6%    6G   �� *Data_Zakonczenia_Karnetu+5$      5&      5(      5*      