﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Ex05.Logic;

namespace Ex05.WindowsUI
{
    public class Application
    {
        private TicTacToeRev m_Game;
        private FormGame m_GameUI;

        public Application(FormGameSettings i_FormGameSettings)
        {
            if(i_FormGameSettings.DialogResult == DialogResult.OK)
            {
                InitGame(i_FormGameSettings);
                InitFrontEnd(i_FormGameSettings);
            }
        }

        private void InitFrontEnd(FormGameSettings i_FormGameSettings)
        {
            m_GameUI = new FormGame(m_Game.Size);
            m_GameUI.SetGameFormLablesAndTheirSize(m_Game.Player1Name, m_Game.Player2Name);
            m_GameUI.AfterClick += afterClickOperations;
            m_GameUI.AfterClick += computerMove;
        }

        private void afterClickOperations(object sender, EventArgs e)
        {
            Button boardButton = sender as Button;
            Point coordinateToSetUp = (Point)boardButton.Tag;
            if (boardButton != null)
            {
                m_Game.SetCoordinate(coordinateToSetUp.X, coordinateToSetUp.Y);
            }
        }

        private void InitGame(FormGameSettings i_FormGameSettings)
        {
            string player1Name = i_FormGameSettings.Player1Name;
            string player2Name = i_FormGameSettings.Player2Name == "[Computer]" ? "Computer" : i_FormGameSettings.Player2Name;
            eGameDefinition gameMode = player2Name == "Computer" ? eGameDefinition.PlayerAgainstComputer : eGameDefinition.TwoPlayers;
            int boardSize = i_FormGameSettings.BoardSize;
            m_Game = new TicTacToeRev(boardSize, player1Name, player2Name, gameMode);
            m_Game.GameOperationAfterClick += updateUiBoard;
            m_Game.GameOperationAfterClick += checkTheBoardStatus;
        }

        private void computerMove(object sender, EventArgs e)
        {
            bool gameOver = m_Game.HasGameFinishedWithDrawFlag || m_Game.HasGameFinishedWithWinnerFlag;
            if (gameOver == false)
            {
                if (m_Game.GameMode == eGameDefinition.PlayerAgainstComputer && m_Game.CurrentPlayer.Name == "Computer")
                {
                    m_Game.ComputerMove();
                }
            }
        }

        private void updateUiBoard(object sender, EventArgs e)
        {
            Point coordinateToChange = new Point();
            if (sender.GetType() == typeof(Point))
            {
                coordinateToChange = (Point)sender;
                m_GameUI.ChangeButtonSignEnablementAndBold(coordinateToChange, m_Game.GetSignByIndex(coordinateToChange.X, coordinateToChange.Y));
            }
        }

        private void checkTheBoardStatus(object sender, EventArgs e)
        {
            StringBuilder messageToShow = new StringBuilder();

            if (m_Game.GameIsOver() == true)
            {

                if(m_Game.HasGameFinishedWithWinnerFlag)
                {
                    messageToShow.Append($@"The winner is {m_Game.GetTheWinner().Name}!");
                }
                else
                {
                    messageToShow.Append(@"Tie!");
                }

                messageToShow.Append(@"
Would you like to play another round?");

                int player1Score = m_Game.Players[0].Score;
                int player2Score = m_Game.Players[1].Score;

                m_GameUI.UpdateScoreLables(player1Score, player2Score);

                DialogResult dialog = MessageBox.Show(messageToShow.ToString(),"End Game",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if(dialog == DialogResult.Yes)
                {
                    NewRound();
                }
                else
                {
                    m_GameUI.Close();
                }
            }
        }

        private void NewRound()
        {
            m_Game.InitialNewRound();
            m_GameUI.RestartTheBoard();
        }

        internal void RunGame()
        {
            m_GameUI.ShowDialog();
        }
    }
}
