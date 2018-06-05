using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueGame.Jatek.Jatekter;
using RogueGame.Jatek.Szabalyok;
using RogueGame.Jatek.Megjelenites;
using RogueGame.Jatek.Automatizmus;

namespace RogueGame.Jatek
{
    namespace Jatekter
    {
        abstract class JatekElem
        {
            private int x;
            private int y;

            protected JatekTer ter;

            public int X
            {
                get { return x; }
                set { x = value; }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }
            public abstract double Meret
            {
                get;
            }

            public JatekElem(int x, int y, JatekTer ter)
            {
                X = x;
                Y = y;
                this.ter = ter;
                ter.Felvetel(this);
            }

            public abstract void Utkozes(JatekElem elem);
        }
        abstract class RogzitettJatekElem : JatekElem
        {
            public RogzitettJatekElem(int x, int y, JatekTer ter) : base(x, y, ter)
            { }
        }
        abstract class MozgoJatekElem : JatekElem
        {
            private bool aktiv;

            public bool Aktiv
            {
                get { return aktiv; }
                set { aktiv = value; }
            }

            public MozgoJatekElem(int x, int y, JatekTer ter) : base(x, y, ter)
            {
                aktiv = true;
            }

            public void AtHelyez(int ujx, int ujy)
            {
                var ujHelyenLevok = ter.MegadottHelyenLevok(ujx, ujy);
                foreach (var elem in ujHelyenLevok)
                {
                    if (!this.aktiv)
                        return;
                    this.Utkozes(elem);
                    elem.Utkozes(this);
                }
                ujHelyenLevok = ter.MegadottHelyenLevok(ujx, ujy);
                double ujHelyenHely = 1;
                foreach (var elem in ujHelyenLevok)
                    ujHelyenHely -= elem.Meret;
                if (Meret <= ujHelyenHely)
                {
                    X = ujx;
                    Y = ujy;
                }
            }
        }

        class JatekTer : IMegjelenitheto
        {
            const int MAX_ELEMSZAM = 1000;

            private int elemN;
            private int meretX;
            private int meretY;

            private JatekElem[] elemek;

            public int MeretX
            {
                get { return meretX; }
            }
            public int MeretY
            {
                get { return meretY; }
            }
            public int[] MegjelenitendoMeret
            {
                get { return new int[] { meretX, meretY }; }
            }

            public JatekTer(int meretX, int meretY)
            {
                this.meretX = meretX;
                this.meretY = meretY;

                elemek = new JatekElem[MAX_ELEMSZAM];
                elemN = 0;
            }

            public void Felvetel(JatekElem elem)
            {
                elemek[elemN++] = elem;
            }
            public void Torles(JatekElem elem)
            {
                for (int i = 0; i < elemN; i++)
                {
                    if (elemek[i].Equals(elem))
                    {
                        //utolsó elem törölt elem helyére
                        elemek[i] = elemek[--elemN];
                        //utolsó elem törlése, hogy ne legyen duplikáció
                        elemek[elemN] = null;
                        return;
                    }
                }
            }
            public JatekElem[] MegadottHelyenLevok(int x, int y, int tavolsag)
            {
                int megfelel = 0;
                for (int i = 0; i < elemN; i++)
                {
                    //pitagorasz tétel alapján távmérés
                    if (((elemek[i].X - x) * (elemek[i].X - x) + (elemek[i].Y - y) * (elemek[i].Y - y)) <= tavolsag * tavolsag)
                        megfelel++;
                }
                var megfelelElemek = new JatekElem[megfelel];
                for (int i = 0; i < elemN; i++)
                    if (((elemek[i].X - x) * (elemek[i].X - x) + (elemek[i].Y - y) * (elemek[i].Y - y)) <= tavolsag * tavolsag)
                        megfelelElemek[--megfelel] = elemek[i];
                return megfelelElemek;
            }
            public JatekElem[] MegadottHelyenLevok(int x, int y)
            {
                return MegadottHelyenLevok(x, y, 0);
            }
            public IKirajzolhato[] MegjelenitendoElemek()
            {
                int count = 0;
                for (int i = 0; i<elemN; i++)
                    if (elemek[i] is IKirajzolhato)
                        count++;
                var megjelenithetok = new IKirajzolhato[count];
                for (int i = 0; i < elemN; i++)
                    if (elemek[i] is IKirajzolhato)
                        megjelenithetok[--count] = elemek[i] as IKirajzolhato;
                return megjelenithetok;

            }
        }
    }
    namespace Szabalyok
    {
        delegate void KincsFelvetelKezelo(Kincs kincs, Jatekos jatekos);
        delegate void JatekosValtozasKezelo(Jatekos jatekos, int ujPont, int ujElet);

        class Fal : RogzitettJatekElem, IKirajzolhato
        {
            public override double Meret
            {
                get { return 1; }
            }
            public char Alak
            {
                get { return '\u2593'; }
            }

            public Fal(int x, int y, JatekTer ter) : base(x, y, ter)
            { }

            public override void Utkozes(JatekElem elem)
            { }
        }
        class Jatekos : MozgoJatekElem, IKirajzolhato, IMegjelenitheto
        {
            private string nev;
            private int eletero;
            private int pontszam;

            public event JatekosValtozasKezelo JatekosValtozas;

            public string Nev
            {
                get { return nev; }
            }
            public override double Meret
            {
                get { return 0.2; }
            }
            public virtual char Alak
            {
                get { return Aktiv ? '\u263A' : '\u263B'; }
            }
            public int[] MegjelenitendoMeret
            {
                get { return ter.MegjelenitendoMeret; }
            }

            public Jatekos(string nev, int x, int y, JatekTer ter) : base(x, y, ter)
            {
                this.nev = nev;
                eletero = 100;
                pontszam = 0;
            }

            public override void Utkozes(JatekElem elem)
            { }
            public void Serul(int sebzes)
            {
                if (sebzes != 0 && this.Aktiv)
                    JatekosValtozas?.Invoke(this, pontszam, eletero -= eletero - sebzes > 0 ? sebzes : 0);
                if (eletero == 0)
                    Aktiv = false;
            }
            public void PontSzerez(int pont)
            {
                if (pont != 0)
                    JatekosValtozas?.Invoke(this, pontszam += pont, eletero);
            }
            public void Megy(int rx, int ry)
            {
                AtHelyez(X + rx, Y + ry);
            }
            public IKirajzolhato[] MegjelenitendoElemek()
            {
                var latott = ter.MegadottHelyenLevok(X, Y, 5);
                int count = 0;
                foreach (var elem in latott)
                    if (elem is IKirajzolhato)
                        count++;
                var kirajzolhato = new IKirajzolhato[count];
                foreach (var elem in latott)
                    kirajzolhato[--count] = elem as IKirajzolhato;
                return kirajzolhato;
            }
        }
        class Kincs : RogzitettJatekElem, IKirajzolhato
        {
            public event KincsFelvetelKezelo KincsFelvetel;

            public override double Meret
            {
                get { return 1; }
            }
            public char Alak
            {
                get { return '\u2666'; }
            }

            public Kincs(int x, int y, JatekTer ter) : base(x, y, ter)
            { }

            public override void Utkozes(JatekElem elem)
            {
                if (elem is Jatekos)
                {
                    KincsFelvetel?.Invoke(this, elem as Jatekos);
                    ter.Torles(this);
                }
            }
        }
        class GepiJatekos : Jatekos, IAutomatikusanMukodo
        {
            static Random RandomGenerator = new Random();

            public override char Alak
            {
                get { return '\u2640'; }
            }
            public int MukodesIntervallum
            { get { return 2; } }

            public GepiJatekos(string nev, int x, int y, JatekTer ter) : base(nev, x, y, ter)
            { }

            public void Mozgas()
            {
                int rng = RandomGenerator.Next(0, 4);       //szívesen így csinálnám: e^((n+1)/4*PI*i), ez visszaadná a 4 irányt complex számként...
                switch (rng)                                //vagy absztraktan (e^((n+1)/(felső határ)*PI*i), ez tetszőleges irányra működne
                {
                    case 0:
                        Megy(0, -1);
                        break;
                    case 1:
                        Megy(0, 1);
                        break;
                    case 2:
                        Megy(-1, 0);
                        break;
                    case 3:
                        Megy(1, 0);
                        break;
                    default:
                        break;
                }
            }
            public void Mukodik()
            {
                Mozgas();
            }
        }
        class GonoszGepiJatekos : GepiJatekos
        {
            public override char Alak
            {
                get { return '\u2642'; }
            }

            public GonoszGepiJatekos(string nev, int x, int y, JatekTer ter) : base(nev, x, y, ter)
            {   }

            public override void Utkozes(JatekElem elem)
            {
                base.Utkozes(elem);
                if (elem is Jatekos && this.Aktiv)
                    (elem as Jatekos).Serul(10);
            }
        }
    }
    namespace Keret
    {
        public class Keret
        {
            const int PALYA_MERET_X = 21;
            const int PALYA_MERET_Y = 11;
            const int KINCSEK_SZAMA = 10;

            private bool jatekVege;
            private int megtalaltKincsek;

            private JatekTer ter;
            private OrajelGenerator generator;

            public Keret()
            {
                ter = new JatekTer(PALYA_MERET_X, PALYA_MERET_Y);
                jatekVege = false;
                generator = new OrajelGenerator();
                PalyaGeneralas();
            }

            private void PalyaGeneralas()
            {
                Random r = new Random();
                for (int i = 0; i < PALYA_MERET_X; i++)
                {
                    new Fal(i, 0, ter);
                    new Fal(i, PALYA_MERET_Y - 1, ter);
                    if (i > 0 && i < PALYA_MERET_Y - 1)
                    {
                        new Fal(0, i, ter);
                        new Fal(PALYA_MERET_X - 1, i, ter);
                    }
                }
                for (int i = 0; i < KINCSEK_SZAMA;)
                {
                    int x = r.Next(1, PALYA_MERET_X - 1), y = r.Next(1, PALYA_MERET_Y - 1);
                    if (ter.MegadottHelyenLevok(x, y).Length == 0 && !(x == y && y == 1))
                    {
                        (new Kincs(x, y, ter)).KincsFelvetel += KincsFelvetelTortent;
                        i++;
                    }
                }
            }
            public void Futtatas() 
            {
                var jatekos = new Jatekos("Béla", 1, 1, ter);
                var gepiJatekos = new GepiJatekos("Kati", 2, 2, ter);
                var gonoszGepiJatekos = new GonoszGepiJatekos("Laci", 3, 3, ter);
                var megjelenito2 = new KonzolosMegjelenito(25, 0, jatekos);
                var megjelenito = new KonzolosMegjelenito(0, 0, ter);
                var eredmenyAblak = new KonzolosEredmenyAblak(0, 12, 5);
                jatekos.JatekosValtozas += JatekosValtozasTortent;
                eredmenyAblak.JatekosFeliratkozas(jatekos);
                generator.Felvetel(gepiJatekos);
                generator.Felvetel(gonoszGepiJatekos);
                generator.Felvetel(megjelenito);
                generator.Felvetel(megjelenito2);
                do
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            jatekos.Megy(-1, 0);
                            break;
                        case ConsoleKey.RightArrow:
                            jatekos.Megy(1, 0);
                            break;
                        case ConsoleKey.UpArrow:
                            jatekos.Megy(0, -1);
                            break;
                        case ConsoleKey.DownArrow:
                            jatekos.Megy(0, 1);
                            break;
                        case ConsoleKey.Escape:
                            jatekVege = true;
                            break;
                        default:
                            break;
                    }
                } while (!jatekVege);
            }
            private void KincsFelvetelTortent(Kincs kincs, Jatekos jatekos)
            {
                jatekos.PontSzerez(10);
                if (++megtalaltKincsek == KINCSEK_SZAMA)
                    jatekVege = true;
            }
            private void JatekosValtozasTortent(Jatekos jatekos, int ujPont, int ujElet)
            {
                if (ujElet == 0)
                    jatekVege = true;
            }
        }
    }
    namespace Megjelenites
    {
        public interface IKirajzolhato
        {
            int X
            { get; }
            int Y
            { get; }
            char Alak
            { get; }
        }
        public interface IMegjelenitheto
        {
            int[] MegjelenitendoMeret
            { get; }

            IKirajzolhato[] MegjelenitendoElemek();
        }

        public class KonzolosMegjelenito : IAutomatikusanMukodo
        {
            private int pozX;
            private int pozY;

            private IMegjelenitheto forras;

            public int MukodesIntervallum
            {
                get { return 1; }
            }

            public KonzolosMegjelenito(int pozX, int pozY, IMegjelenitheto forras)
            {
                this.pozX = pozX;
                this.pozY = pozY;
                this.forras = forras;
            }

            public void Megjelenites()
            {
                var meret = forras.MegjelenitendoMeret;     //meret[0] szélesség, meret[1] magasság
                var megjelenitendok = forras.MegjelenitendoElemek();
                for (int i = 0; i < meret[0]; i++)
                    for (int j = 0; j < meret[1]; j++)
                    {
                        SzalbiztosKonzol.KiirasXY(pozX + i, pozY + j, ' '); //töröljük a régi elem rajzát
                        foreach (var elem in megjelenitendok)
                            if (elem.X == i && elem.Y == j)
                                SzalbiztosKonzol.KiirasXY(pozX + i, pozY + j, elem.Alak);
                    }
            }

            public void Mukodik()
            {
                Megjelenites();
            }
        }
        class KonzolosEredmenyAblak
        {
            private int pozX;
            private int pozY;
            private int maxSorSzam;
            private int sor;

            public KonzolosEredmenyAblak(int pozX, int pozY, int maxSorSzam)
            {
                this.pozX = pozX;
                this.pozY = pozY;
                this.maxSorSzam = maxSorSzam;
            }

            private void JatekosValtozasTortent(Jatekos jatekos, int ujPont, int ujElet)
            {
                SzalbiztosKonzol.KiirasXY(pozX, pozY + sor, string.Format("Játékos neve: {0}, pontszáma: {1}, életereje: {2}", jatekos.Nev, ujPont, ujElet));
                sor = (sor + 1) % maxSorSzam;
            }

            public void JatekosFeliratkozas(Jatekos jatekos)
            {
                jatekos.JatekosValtozas += JatekosValtozasTortent;
            }
        }
    }
    namespace Automatizmus
    {
        public interface IAutomatikusanMukodo
        {
            void Mukodik();
            int MukodesIntervallum
            { get; }
        }
    }
}
