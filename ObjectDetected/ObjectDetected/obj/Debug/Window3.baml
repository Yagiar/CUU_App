   M S B A M L   `   `   `  ����I  EObjectDetected, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ObjectDetectedObjectDetected  5   	       ObjectDetected.Window3   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:ObjectDetected     5   	     � Title$  Window3��5   	   $	��450��6   $	��800��6&   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��97* �6   �� 5      $
��303* �6   .+�5   
   r� -   5         r�Name$ mediaPlayer��5   (    K� ZIndex$ 0��6   $��10,10,10,10q�6=    r� LoadedBehavior$ Manual=�6R   $��2��6j   �� 5   
   $ 2��6    �� Orientation$ 
Horizontal=�6&   $
��Left=�6?   $��Bottom=�6Z   $��
146,0,0,10q�6u   $��2��6�   .+�5      �� -   5        $ ��Воспроизвести��5      6-   �� 5      -   5        $��
Пауза��5      6%   �� 5      -   5        $��Стоп��5      6$   �� 5      -   5         ��Name$ progressSlider��5      $	��200��6-   69   �� 5      -   5         ��Name$ 
select_btn��5      $��select��6)   6:   �� 5      -   5         ��Name$ select_class��5   #   $	��120��6   .@�5      �� $��	No helmet��6   �� 5      $��No vest��6   �� 5      $��Person��6   �� 5      $��helmet��6   �� 5      $
��vest��6   5      5      �� 5   
   -   5        	 ��Name$	 images��5      $
��Left=�6    $	��227��6;   $��170,277,0,0q�6H   $	��Top=�6]   $	��313��6u   $��1��6�   �� 5    
   -	   5        
 ��Name$
 
paint_area��5    5   $ 1��6   
�������$��10,10,10,10q�6I   $��2��6�   5!      5"      