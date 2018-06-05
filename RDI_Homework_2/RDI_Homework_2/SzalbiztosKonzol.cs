using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueGame.Jatek.Megjelenites
{
    public static class SzalbiztosKonzol
    {
        static char[,] buffer;
        static SzalbiztosKonzol()
        {
            buffer = new char[Console.WindowWidth, Console.WindowHeight];
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        }

        // a szinkronizacio miatt kell
        static object obj = new Object();

        // egy karakter kiirasa
        public static void KiirasXY(int x, int y, char betu)
        {
            lock (obj)
            {
                if (buffer[x, y] != betu)
                {
                    buffer[x, y] = betu;
                    Console.SetCursorPosition(x, y);
                    Console.Write(betu);
                }
            }
        }

        // egy sor kiirasa
        public static void KiirasXY(int x, int y, string szoveg)
        {
            for (int i = 0; i < szoveg.Length; i++)
                KiirasXY(x + i, y, szoveg[i]);
        }
    }
}
