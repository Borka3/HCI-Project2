   M S B A M L   `   `   `  ����I  ENetworkService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:NetworkService.ViewsNetworkService.Views  5      *    $NetworkService.Views.NetworkEntities   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:NetworkService.Views     5        J� InputBindings  5
      �� 5   
    �� 	Modifiers$ Controlf�6   
 �� Key$ A��5       �� Command 5      �� *AddEntityCommand+�� 5   
   $ Controlf�6   $ B��5       5      �� *DeleteEntityCommand+�� 5   
   $ Controlf�6   $ C��5       5      �� *FilterEntityCommand+�� 5   
   $ Controlf�6   $ E��5       5      �� *ResetEntityCommand+.��5      � .+�5   
   ��  �� Orientation$ Vertical=�6   $��Center=�6,   .+�5      �� $ 
Horizontal=�6   $
��Left=�62   $	��790��6M   .+�5      �� $��/Fonts/#Rubik)�6   ���#CAE6B2$
��TYPE��6M   $
��Auto��6Y   $
��Auto��6g   $��Center=�6t   $��Center=�6�   $��	5,1,10,10q�6�   �� 5      -   5         ��Name$ comboBoxEntityType1��5      $	��150��6�   $��22��6�   $��	5,1,10,10q�6�   ���#8E3E63��68   �� *ComboBoxItems+ �� SelectedItem 6^   �� *SelectedEntityTypeToFilter+ �� Mode$ TwoWay=�6�   �� 5      $��/Fonts/#Rubik)�6   ���#CAE6B2$��ID��6L   $
��Auto��6V   $
��Auto��6d   $��Center=�6q   $��Center=�6�   $��	5,1,10,10q�6�   �� 5       $ Vertical=�6   $��	5,1,10,10q�64   .+�5!      	� -   5         	�Name$ radioButtonLessThan��5!   "   ���#CAE6B2$��<��6T   ���#8E3E63	 d� 	IsChecked	 6c   �� *IsLessThanRadioButtonSelected+$ TwoWay=�6�   	� 5"      -   5        $ radioButtonGreaterThan��5"   "   ���#CAE6B2$��>��6W   ���#8E3E63	 6f   �� *" IsGreaterThanRadioButtonSelected+$ TwoWay=�6�   	� 5#      -   5        $ radioButtonEqually��5#   "   ���#CAE6B2$��=��6S   ���#8E3E63	 6_   �� *IsEquallyRadioButtonSelected+$ TwoWay=�6�   5$      �� 5&      -   5        
 ��Name$
 textBoxIdToFilter��5&      $	��150��6p   $��20��6|   $��	5,1,10,10q�6�      Text_Box_Style#��[  6�   $��1q�6�   ���#8E3E63��65   �� *SelectedIdMarginToFilterText+$ TwoWay=�6�   �� 5(      -   5         ��Name$ buttonFilter��5(   D     ButtonStyleNormal#��[ 6    � ToolTip$ Filter entities��6Z   $��FILTER��6t   $	��100��6�   $��30��6�   $��	6,1,10,10q�6�   ��6�   �� *FilterEntityCommand+6�   �� 5)      -   5        $ buttonReset��5)   D   #��[ 6   $ Reset field��6Y   $��RESET��6o   $	��100��6�   $��30��6�   $��	6,1,10,10q�6�   ��6   �� *ResetFilterCommand+6�   �� 5*      -   5        $ 
buttonUndo��5*   D   #��[ 6   $ 	Step back��6X   $
��UNDO��6l   $	��100��6�   $��30��6�   $��	6,1,10,10q�6�   ��6{   �� *UndoCommand+6�   5+      �� 5-      
���  ��$	��115��6M    �� TextAlignment$ Center=�6Y   $
��Left=�6p   $��	295,0,0,0q�6�   ��6   �� *FilterErrorMessage+6�   &   System.Windows.Controls.DataGrid  5/      -	   5          Name$ dataGridEntities��5/      $��1q�6�   ���#8E3E63���#8E3E63$	��160��6�   $	��757��6�   $
��Left=�6�   $��	16,5,10,0q�6�      	RowHeight$ 20Q�6     
IsReadOnly .    SelectionMode$ Single=�6'     AutoGenerateColumns .     CanUserAddRows .     CanUserReorderColumns .     CanUserResizeColumns .  "   VerticalScrollBarVisibility$ Hidden=�6�  ��.  ��61   �� *EntitiesToShow+ 6X   �� *SelectedEntity+ � 	Resources 50      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value:  4System.Windows.Controls.Primitives.DataGridRowHeader*  $System.Windows.Controls.DataGridCell �� 
TargetType��%�  '        '  �     '  �     �� 51      "  6   .�52      ��  � �� �#D2649A67   �� 53       � ��$ Center=�6G   �� 54       � ��$ Center=�6E   55      �� 56      "  6   .�57      ��  � �� �#D2649A58   !   59      �� 5:      "  6   .�5;      ��  � �� 5<      �� 5=   "   "  62   .R�5>   &   � #��z��6+   .+�5?   *   �� $��Center=�6;   $��Center=�6V   5@   '   5A   #   5C      5D      5E         Columns 5F      0  *System.Windows.Controls.DataGridTextColumn  5G      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6)      
Foreground �#CAE6B2   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6a   1  +System.Windows.Controls.DataGridBoundColumn    Binding  6J   �� *Id+6a     5H      $
 Name��6)    �#CAE6B2$ * 6e     6L   �� *Name+6e     5I      $
 Type��6)    �#CAE6B2$ * 6p     6L   �� *TypeEntity.Name+6p     5J      $ Value��6)    �#CAE6B2$ * 6g     6M   �� *Value+6g   !   RowStyle! 5L      �� 5M      )	  #System.Windows.Controls.DataGridRow" 	 6   .�5N      ��  � �� �#D2649A5O   !   �� 5P       � ��$ Stretch=�5Q   !   �� 5R       � ��$ Stretch=�5S   !   �� 5T       � ��$ 16(�5U   !   �� 5V       � ��$ /Fonts/#Rubik)�5W   !   5X      5Z      �� 5\      $ Vertical=�6   $��Center=�60   $��0,15,0,0q�6M   .+�5]      �� $ 
Horizontal=�6   $
��Left=�66   .+�5^      �� $��/Fonts/#Rubik)�6    ���#CAE6B2$
��ID: ��6P   $
��Auto��6\   $
��Auto��6j   $��Center=�6w   $��Center=�6�   �� 5_      -
   5        $
 	textBoxId��5_      $	��150��6d   $��20��6p   #��[  6|   $��1q�6�   ���#8E3E63��61   �� *CurrentEntity.TextId+$ TwoWay=�6�   �� 5`      $��/Fonts/#Rubik)�6    ���#CAE6B2$��NAME: ��6P   $
��Auto��6^   $
��Auto��6l   $��Center=�6y   $��Center=�6�   $��
10,10,0,10q�6�   �� 5a      -   5        $
 textBoxName��5a      $	��150��6d   $��20��6p   #��[  6|   $��1q�6�   ���#8E3E63��63   �� *CurrentEntity.Name+$ TwoWay=�6�   �� 5b      $��/Fonts/#Rubik)�6    ���#CAE6B2$��TYPE: ��6P   $
��Auto��6^   $
��Auto��6l   $
��Left=�6y   $��Center=�6�   $��10,1,0,0q�6�   �� 5c      -   5        $ comboBoxEntityType2��5c      $	��150��6�   $��20��6�   ���#8E3E63��6<   �� *ComboBoxItems+ 6b   �� *CurrentEntityType.Name+$ TwoWay=�6�   5d      �� 5f      $ 
Horizontal=�6   $��Center=�66   $��
20,0,35,10q�6S   $	��535��6g   .+�5g      �� 
���  ��$	��150��6e   $��20��6q   $��1,1,0,0q�6}   ��6    �� *$"CurrentEntity.ValidationErrors[Id]+6}   �� 5h      
���  ��$	��150��6g   $��20��6s   $��	40,2,20,0q�6   ��6    �� *&$CurrentEntity.ValidationErrors[Name]+6   �� 5i      
���  ��$	��150��6k   $��20��6w   $��	20,2,50,0q�6�   ��6    �� **(CurrentEntityType.ValidationErrors[Name]+6�   5j      � 5l      $��80��6   $��80��6"   $��
10,0,10,15q�6.   
�������.+�5m      �� ���#8E3E63$��1q�63   $��Center=�6G   $��Center=�6d   ._�5n      �� $��Uniform=�6Y   ��6    �� *CurrentEntityType.ImgSrc+$ TwoWay=�6Y   5o      5p      �� 5r      $ 
Horizontal=�6   $��Center=�66   $��10,0,0,0q�6S   .+�5s      �� -   5        $ 	buttonAdd��5s   H   #��[ 6   $ 	Add valve��6[   $��	10,0,20,0q�6o   $	��ADD��6�   $	��100��6�   $��30��6�   ��6�   �� *AddEntityCommand+�� 5t      -   5        $ buttonDelete��5t   H   #��[ 6   $ Delete valve��6^   $��DELETE��6u   $	��100��6�   $��30��6�   $��	10,0,20,0q�6�   ��6�   �� *DeleteEntityCommand+6�   5u      5v      5y      5z      5{      