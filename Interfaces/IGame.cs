using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rest_API_ReversiGame.Enums;

namespace Rest_API_ReversiGame.Interface
{
    public interface IGame
    {
        int ID { get; set; }
        string Description { get; set; }

        string Token { get; set; }
        string Player1Token { get; set; }
        string Player2Token { get; set; }
        
        ReversiColor [,] Board { get; set; }

        ReversiColor ColorTurn { get; set; }

        bool Pass();
        bool Ended();

        ReversiColor WhereasColor();

        bool MovePossible(int rowMove, int columnMove);
        bool PerformMove(int rowMove, int columnMove);

    }
}
