   M S B A M L   `   `   `  ����G  CMessageBoard, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:MessageBoardMessageBoard  5   	       MessageBoard.MainWindow   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   >d2http://schemas.microsoft.com/expression/blend/2008    5   	   Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5   	   *localclr-namespace:MessageBoard    5   	     � Title$  
MainWindow��5   	   $	��650��6k   $	��500��6x   ��6   ��  �� Source# Zo� �� Path$ Height� � 	Resources 5	         
ModifyMode  btnAnsSubmit-  'System.Windows.Input.KeyboardNavigation   TabIndex �� ElementName  ansInputTextBox  
NormalMode  normalModeBorder  normalModeMsgPanel  normalModeMsgDetailPanel  normalModeDateTextBox �� StringFormat  normalModeUserTextBox d�Name 	 adminModeToggle
 � Tag	 � ContextMenu 
 normalModeMsgTextBlock  MessageTemplate
 ��Name �� Triggers �� Value �� Binding �� 
TargetName �� Value �� Property��%[  &	        &	 �    &	 �    7  7   7  �� 5
   
   .R�5      ]� $��LeftToRight=�6   .+�5      ]� $��Right=�6   .+�5      �� -   5        $��Bottom=�5      $��1��65   #���  6B   $��Cancel��6u   $ 2��6�   �� 5      -   5        $��0��5      #���  6*   $��Modify��6]   $ 1��6�   ��6�   �� $ updateAnswer��$
 Text�6�   5      �� 5      $��updateAnswer��6   #���  6.   $ 0��6~   ��6d   �� *
Messages+6~   5      5      �� 5   
   .R�5      �� #���  6   ._�5      ]� #���  6   .+�5      ]� #���  6    .+�5      �� #���  6$   ��6`   �� *CreatedDate+$ {0:dd MMMM yyyy}���� 5      #���  6$   ��6`   �� *GPName+$ Replied by {0}��5      ]� 5      .+�5      d� $ 	btnUpdate��6>   -   5        $��Right=�5   '   $��20��6R   $��20��6]   #��� 	 6i   ��6�   �� *Id+ 6�   �� *MessageThreadId+��5      �� 5   "   $��/images/dropdown.png��6(   	 5!      �� 5"   "   $��contextMenu��6.   .@�5#   &   m� -   5        $��Modify��5#   /   6?   m� 5$   &   -   5        $��Delete��5$   /   6?   5%   #   5'      �� 5(      #��� 
 6$   ��6a   �� *
Messages+5)      5*      5+      5,      �� 5.   
   .R�5/      �� $
 MessagePresenter��6   8��  69    50      �� 51      $
 true��6=    6   �� *
IsExpanded+.a�52      �� $ MessagePresenter��6   8   � ��6V   53      �� 54      $ false��6=    6   �� *
IsExpanded+.a�55      �� $ MessagePresenter��6   8   � ��6V   56      58      5:      .��5;      �  � RowDefinitions 5<   
   �� 5=      $
��Auto �6   �� 5>      $
��Auto �6   �� 5?      $
��Auto �6   �� 5@      $��* �6   .+�5C   
   d� -   5        $ btnNew��5C      $��0��6'    
 btnNew#���  64   $ 0��6a   ]� 5E   
   $��1��6     newTemplate#���  6!   y�6S   �� $ btnNew��$ 	IsChecked�  BoolToVisibilityConverter �� 	Converter# [ .+�5F      �� $	��Top=�6     newTemplateBorder#���  6*   ._�5G      ]� .+�5H      �� $
��Left=�6      textblockTitle#���  66   $��Title��6k   �� 5I      -   5        $��title��5I        textboxTitle#���  6+   $ 1��6^   5J      5K      ]� 5L      -   5         ]�Name$ click��5L      $��Bottom=�6'     submitPanel#���  6?   .+�5M      �� -	   5        $��Right=�5M        	btnSubmit#���  60   $
��Send��6`   $ 3��6�   5N      �� 5P      -
   5        $��message��5P        
newMessage#���  6%   $ 2��6V   5Q      ]� 5S   
   $��2��6     searchPanel1#���  6!   .+�5T      ��   searchBorder#���  6   ._�5U      ]�   searchPanel2#���  6   .+�5V      ��   
searchIcon#���  6   �� 5W      -   5         ��Name$ InputTextBox��5W       
 search#���  64   6�   5X      5Y      5Z      �� 5\   
   -   5        $��3��5\        	msgScroll#���  6$   .��5]      �� -   5        $��msgListView��5]        msgList#���  6*     ListViewControlTemplate#���  6X     msgListItem#���  6�   ��5^      �� 5_      .R�5`      ��   msgListItemBorder#���  6!   ._�5a      ��   msgThreadHiddenPanel#���  6)   .+�5b   "   ��    msgListItemTitleBorder#���   6)   ._�5c   &   ]� .+�5d   *   ��  ! gpImageBorder#��� ! 61   ._�5e   .   �� $
��Left=�69    " gpImagePanel#��� " 6O   .+�5f   2   ��  # gpImage#��� # 68   5g   /   5h   +   ]� 5i   *   $	��Top=�64    $ msgTitlePanel1#��� $ 6I   .+�5j   .   �� $��Right=�69    % 
replyPanel#��� % 6P   .+�5k   2   ��  & replyTextBlock#��� & 6<   ��6q   �� *Key.NumOfMessage+5l   /   �� 5m   .    ' 
titlePanel#��� ' 69   .+�5n   2   ��  ( titleTextBlock#��� ( 6<   ��6q   �� *	Key.Title+5o   /   5p   +   ]� 5q   *   $��Bottom=�64    ) msgTitlePanel2#��� ) 6L   .+�5r   .   �� $��Right=�69   .+�5s   2   ��  * dateTextBlock#��� * 6<   ��6p   �� *Key.CreatedDate+$ {0:dd MMMM yyyy}��5t   /   �� 5u   .   .+�5v   2   ��  + gpTextBlock#��� + 6<   ��6n   �� *
Key.GPName+5w   /   5x   +   5y   '   5z   #   �� 5{   "   $��Stretch=�6-   $��1q�6K   .+�5|   &   9� $��msgExpander��6/    , expander#��� , 6B    - ExpanderControlTemplate1#��� - 6q   .��5}   *   �� $��msgPanel��65   .+�5~   .   �� #��[ 6Y   ��6;   �� *Value+6Y   ]� 5   .    . ansInputPanel#��� . 68   .+�5�   2   �� -   5        $��Right=�5�   9   #���  6P   $
��Send��6�   $ 4��6  ��6�   �� $ answer��$
 Text� 6�   �� *Key.Id+��6�   �� *Key.NumOfMessage+6  �� 5�   2   $��answer��6:   #���  6H   $ 3��6~   5�   /   5�   +   5�   '   5�   #   5�      5�      5�      5�      5�      5�      5�      