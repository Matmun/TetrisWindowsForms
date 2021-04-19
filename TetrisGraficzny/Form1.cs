using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGraficzny
{
    public partial class Form1 : Form
    {
        public static readonly int SBW=25;//Singular Block Width
        public static readonly int BW =10,BH=16;//Board Widht, Board Height
        public static int XL=BW/2-1, YL=0;//X location, Y location
        public static int score;
        public static Queue<IBlock> QoB=new Queue<IBlock>();//Queue of Blocks
        public static IBlock CUB;//Currently Used Block
        public static short ColorId,ColorTemp;
        public short[,] Board;
        public static Random R = new Random();
        public static bool gameover;
        public Form1()
        {
            InitializeComponent();
            timer1.Start();

            gameover = false;
            Display.Width = SBW * BW;
            Display.Height = SBW * (BH-1);
            Display.BackColor = Color.Black;
            NextBlock.Width = SBW * 3;
            NextBlock.Height = SBW * 3;
            NextBlock.BackColor = Color.Black;
            Board = new short[BW, BH];
            for (int i = 0; i < BW; ++i)
                for (int j = 0; j < BH; ++j)
                    Board[i, j] = 0;
            Generate();
            GenerateNext();
            NextBlock.Refresh();

        }

        private void Display_Paint(object sender, PaintEventArgs e)
        {
            
            SolidBrush P = new SolidBrush(SetPrimaryColor(ColorId));
            SolidBrush S = new SolidBrush(SetSecondaryColor(ColorId));
            //SolidBrush t = new SolidBrush(Color.Gainsboro);
            SolidBrush B1 = new SolidBrush(Color.Gray);
            SolidBrush B2 = new SolidBrush(Color.Gray);
            Pen L = new Pen(Color.Gray, SBW / 6);
            for (int i = 0; i < Board.GetLength(0); ++i)
                for (int j = 0; j < Board.GetLength(1); ++j)
                {
                    e.Graphics.DrawLine(L, i * SBW, j * SBW, SBW * BW, j * SBW);
                    e.Graphics.DrawLine(L, i * SBW, 0, i * SBW, BH * SBW);
                }
            for (int i = 0; i < Board.GetLength(0); ++i)
                for (int j = 0; j < Board.GetLength(1); ++j)
                {
                    if (Board[i, j] != 0)
                    {
                        B1.Color = SetPrimaryColor(Board[i, j]);
                        e.Graphics.FillRectangle(B1, i * SBW, j * SBW - SBW, SBW, SBW);
                        B2.Color = SetSecondaryColor(Board[i, j]);
                        e.Graphics.FillRectangle(B2, i * SBW+ ((SBW / 4) / 2), j * SBW - SBW+((SBW / 4)/2), SBW -(SBW/4), SBW - (SBW / 4));
                    }
                }
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    if (Board[XL + i, YL + j] == 0 && QoB.First().Arr[i, j] == true)
                    {
                        e.Graphics.FillRectangle(P, (XL + i) * SBW, (YL + j) * SBW - SBW, SBW, SBW);
                        e.Graphics.FillRectangle(S, (XL + i) * SBW+ ((SBW / 4) / 2), (YL + j) * SBW - SBW+ ((SBW / 4) / 2), SBW- (SBW / 4), SBW- (SBW / 4));
                    }
                    //else e.Graphics.FillRectangle(t, (XL + i) * SBW, (YL + j) * SBW - SBW, SBW, SBW);

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'd':
                    if (CollisionCheck( QoB.First(), '>') == false)
                    {
                        XL++;
                        Display.Refresh();
                    }
                    break;
                case 'a':
                    if (CollisionCheck( QoB.First(), '<') == false)
                    {
                        XL--;
                        Display.Refresh();
                    }
                    break;
                case 'r':
                    QoB.First().Rotate();
                    Display.Refresh();
                    break;
                case 'p':
                    if (gameover == false) 
                    {
                        QoB.Clear();
                        timer1.Stop();
                        score = 0;
                        XL = BW / 2 - 1;
                        YL = 0;
                        Initializer();
                        Display.Refresh();

                    }
                    break;
                default:
                    break;
            }
        }

        private void NextBlock_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush P = new SolidBrush(SetPrimaryColor(ColorTemp));
            SolidBrush S = new SolidBrush(SetSecondaryColor(ColorTemp));

            for (int i = 0; i < QoB.First().Arr.GetLength(0); ++i)
                for (int j = 0; j < QoB.First().Arr.GetLength(1); ++j)
                {
                    if (QoB.Last().Arr[i, j] == true)
                    {
                        e.Graphics.FillRectangle(P,  i * SBW, j * SBW, SBW, SBW);
                        e.Graphics.FillRectangle(S,  i * SBW + ((SBW / 4) / 2), j * SBW + ((SBW / 4) / 2)+1, SBW - (SBW / 4), SBW - (SBW / 4));
                    }
                }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (CollisionCheck( QoB.First(), 'N') == false)
            {
                YL++;
            }
            else
            {
                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 3; ++j)
                        if (QoB.First().Arr[i, j] == true)
                            Board[XL + i, YL + j] = ColorId;
                if (YL + 3 == 3)
                    gameover = true;
                XL = BW / 2 - 1;
                YL = 0;
                StateChech();
                QoB.Dequeue();
                ColorId = ColorTemp;
                GenerateNext();
                NextBlock.Refresh();
                if (gameover)
                {
                    label1.Text = "GameOver";
                    timer1.Stop();
                }
                

            }
            Display.Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S) timer1.Interval = 100;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S) timer1.Interval = 700;
        }

        public void Generate()  
        {
            switch (R.Next(1, 8))
            {
                case 1:
                    ColorId = 1;
                    CUB = new IShape();
                    QoB.Enqueue(CUB);
                    break;
                case 2:
                    ColorId = 2;
                    CUB = new JShape();
                    QoB.Enqueue(CUB);
                    break;
                case 3:
                    ColorId = 3;
                    CUB = new LShape();
                    QoB.Enqueue(CUB);
                    break;
                case 4:
                    ColorId = 4;
                    CUB = new OShape();
                    QoB.Enqueue(CUB);
                    break;
                case 5:
                    ColorId = 5;
                    CUB = new SShape();
                    QoB.Enqueue(CUB);
                    break;
                case 6:
                    ColorId = 6;
                    CUB = new TShape();
                    QoB.Enqueue(CUB);
                    break;
                case 7:
                    ColorId = 7;
                    CUB = new ZShape();
                    QoB.Enqueue(CUB);
                    break;
            }
        } // Funkcja dodajaca bloki do kolejki. Odpalana raz.

        public void GenerateNext()
        {
            switch (R.Next(1, 8))
            {
                case 1:
                    ColorTemp = 1;
                    CUB = new IShape();
                    QoB.Enqueue(CUB);
                    break;
                case 2:
                    ColorTemp = 2;
                    CUB = new JShape();
                    QoB.Enqueue(CUB);
                    break;
                case 3:
                    ColorTemp = 3;
                    CUB = new LShape();
                    QoB.Enqueue(CUB);
                    break;
                case 4:
                    ColorTemp = 4;
                    CUB = new OShape();
                    QoB.Enqueue(CUB);
                    break;
                case 5:
                    ColorTemp = 5;
                    CUB = new SShape();
                    QoB.Enqueue(CUB);
                    break;
                case 6:
                    ColorTemp = 6;
                    CUB = new TShape();
                    QoB.Enqueue(CUB);
                    break;
                case 7:
                    ColorTemp = 7;
                    CUB = new ZShape();
                    QoB.Enqueue(CUB);
                    break;
            }
        } // Funkcja dodajaca bloki do kolejki. Odpalana wielokrotnie

        public void StateChech() 
        {
            int temp = 0;
            for (int i = BH - 1; i >= 0; --i)
            {
                for (int j = 0; j < BW; ++j)
                {
                    if (Board[j, i] != 0) temp++;
                    if (temp >= BW)
                    {
                        for (int k = 0; k < BW; ++k)
                            Board[k, i] = 0;
                        MoveBoardDown(i);
                        score += 100;
                        label1.Text = score.ToString();
                        label1.Refresh();
                        temp = 0;
                        j = -1;
                    }
                }
                temp = 0;
            }
        } // Funckja usuwaajaca linie oraz dodajaca punktacje

        public void MoveBoardDown(int y) 
        {
            for (int i = 0; i < BW; ++i)
                for (int j = y; j > 0; --j)
                    Board[i, j] = Board[i, j - 1];
        } // Funkcja przesuwajaca plansze w dol i 1 linie

        //public bool GameOverCheck() 
        //{
        //    for (int i = 0; i < 3; ++i)
        //        for(int j=0;j<3;j++)
        //        if (Board[XL + i, YL + j] == 1 && QoB.First().Arr[j,i] == true) return true;
        //    return false;
        //} // Funkcja sprawdzajaca czy nie doszlo do zakonczenia gry // nie działa z niezyndefikowanych powodów

        public Color SetPrimaryColor(short a) 
        {
            switch (a)
            {
                case 1:
                    return Color.FromArgb(112, 146, 190);
                case 2:
                    return Color.FromArgb(63, 72, 204);
                case 3:
                    return Color.FromArgb(223, 123, 9);
                case 4:
                    return Color.FromArgb(250, 214, 5);
                case 5:
                    return Color.FromArgb(65, 186, 24);
                case 6:
                    return Color.FromArgb(177, 61, 142);
                case 7:
                    return Color.FromArgb(237, 28, 36);
            }
            return Color.Black;
        } // Funkcja ustawiajaco główne kolory blokow

        public Color SetSecondaryColor(short a)
        {
            switch (a)
            {
                case 1:
                    return Color.FromArgb(153, 217, 234);
                case 2:
                    return Color.FromArgb(5, 143, 226);
                case 3:
                    return Color.Orange;
                case 4:
                    return Color.FromArgb(255, 242, 0);
                case 5:
                    return Color.FromArgb(146, 234, 26);
                case 6:
                    return Color.FromArgb(201, 99, 170);
                case 7:
                    return Color.FromArgb(243, 67, 71);
               
            }
            return Color.Black;
        }// Funkcja ustawiajaco poboczne kolory blokow

        public bool CollisionCheck(IBlock Item,char TypeOFMovement)
        {
            switch (TypeOFMovement) 
            {
                case 'N':
                    if (YL + 3 == BH)
                    {
                        for (int i = Item.Arr.GetLength(0) - 1; i >= 0; --i)
                        {
                            if (Item.Arr[i, Item.Arr.GetLength(1) - 1] == true)
                                return true;
                            if (Board[XL + i, YL + 2] != 0)
                                return true;
                        }
                        for (int i = Item.Arr.GetLength(0)-1; i >= 0; --i)
                            for (int j = Item.Arr.GetLength(1) - 1; j > 0; --j)
                                Item.Arr[i, j] = Item.Arr[i, j - 1];
                        Item.Arr[0, 0] = false;
                        Item.Arr[1, 0] = false;
                        Item.Arr[2, 0] = false;
                        return true;
                    }
                    for (int i = 0; i < 3; ++i)
                        for (int j = 0; j < 3; ++j)
                            if(Board[XL+i,(YL+1)+j]!=0&&Item.Arr[i,j]==true)
                                return true;
                    return false;
                case '>':
                    if (XL == BW - 3)
                    {
                        for (int i = 0; i < 3; ++i)
                            if (Board[2, i] == 1 || Item.Arr[2,i]==true)
                                return true;
                        Item.ChPos(true);
                        return true;
                    }
                    else
                    {
                        for (int i = 0; i < 3; ++i)
                            for (int j = 0; j < 3; ++j)
                                if (Board[(XL + i) + 1, YL + j] != 0 && Item.Arr[i, j] == true)
                                    return true;
                    }
                    return false;
                case '<':
                    if (XL == 0)
                    {
                        for (int i = 0; i < 3; ++i)
                            if (Board[0, i] == 1 || Item.Arr[0, i] == true)
                                return true;
                        Item.ChPos(false);
                        return true;
                    }
                    else
                    {
                        for (int i = 0; i < 3; ++i)
                            for (int j = 0; j < 3; ++j)
                                if (Board[(XL + i) - 1, YL + j] != 0 && Item.Arr[i, j] == true)
                                    return true;
                    }
                    return false;
                default:
                    return true;
            }
        } // Funkcja sprawdzajaca kolizje

        private void Initializer()
        {
            timer1.Start();

            gameover = false;
            Display.Width = SBW * BW;
            Display.Height = SBW * (BH - 1);
            Display.BackColor = Color.Black;
            NextBlock.Width = SBW * 3;
            NextBlock.Height = SBW * 3;
            NextBlock.BackColor = Color.Black;
            Board = new short[BW, BH];
            for (int i = 0; i < BW; ++i)
                for (int j = 0; j < BH; ++j)
                    Board[i, j] = 0;
            Generate();
            GenerateNext();
            NextBlock.Refresh();
        }
    }
    



}
