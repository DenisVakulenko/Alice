   M S B A M L   `   `   `  ����@  <Alice, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:AliceAlice  5   	       Alice.taskMailBox   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   #myLibclr-namespace:Alice    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Почтовый ящик��5   	   $	��543��6   $	��319��6,   .��5      �  � ColumnDefinitions 5   
   �� 5       � RowDefinitions 5
   
   �� 5      $��45 �6   �� 5      .+�5   
   �� ���	#FFF0F0F0$��45��6%   $	��Top=�61   
�������$��
-1,-1,-1,0q�6\   �� 5   
   -   5        $��95,14,107,0q�5      $!��найди письма ...��6'   $��1q�6?    �� MaxLines$ 1��6S   $��19��6�   $	��Top=�6�   ��6`   Z� ��6w   Z� 6�   �� 5   
   -   5        $��написать��5      $
��Left=�6$   $��	10,10,0,0q�6?   $��70��6R   $��25��6r   $	��Top=�6~   �� 5   
   $
��Auto=�6   $��1��6:    J� IsHitTestVisible . |�.  $��0,10,0,0q�6q   .��5      �� -   5         ��Name$ 
stackNotes��5      5      �� 5   
   -   5        $��	вход.��5      $��	0,10,54,0q�6!   $	��Top=�64   $��25��6c   $��Right=�6o   $��39��6�   $y�Hidden=�6�   �� 5   
   -   5         ��Name$ btnInOut��5      $��исх.��6#   $��	0,10,10,0q�62   $	��Top=�6E   $��25��6t   $��Right=�6�   $��39��6�   5      5      