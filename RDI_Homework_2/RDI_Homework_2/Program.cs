using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDI_Homework_2
{
    class Program
    {
        static void Main(string[] args)
        {
            System.IO.File.Delete("game.log");
            var keret = new RogueGame.Jatek.Keret.Keret();
            keret.Futtatas();
        }
    }
}
