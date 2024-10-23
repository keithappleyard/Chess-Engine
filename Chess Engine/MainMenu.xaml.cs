using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chess_Engine
{
    public partial class Window1 : Window
    {
        int Depth = 1;
        bool Checked = false;
        public Window1()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(true, Depth, Checked);
            mainWindow.Show();
            this.Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Depth = (int)Slider.Value;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Welcome to my chess game\n\nYou can adjust the difficulty slider to change the diffculty of the AI player with the left side being the easiest and the right side being the hardest\nYou can click the checkbox to turn on quiescence searching when you player versus an AI player but I would not recommend it because it can be slow\nYou can press the start button to get into a game\n\nThe rules of chess:\nThe aim of the game is to place the opponent in a position where they cannot do any moves where their king cannot be taken the next go\nThe pawns can move up by one square and twice on its first go but they can only capture pieces diagonally\nThe knights can only move two squares in front and one square to the side\nThe bishop can only move diagonally\nThe rook can only move in a straight line\nThe queen can move in a straight line or vertically\n The king can only move to its surrounding squares\nIf a piece moves to a spot where it is able to capture the king, the opponent must spot the piece from being able to capture it on their go\n\nSpecial moves:\nIf a pawn gets to the end of the board, it can turn into a queen\nIf a pawn does a double move adjacent to another colour pawn, the other colour pawn can move diagonally in front and take it\nIf the king has not moved, is not in check, and has not pieces between itself and the rook, it is able to do a castle move");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Checked = (bool)CBox.IsChecked;
        }
    }
}
