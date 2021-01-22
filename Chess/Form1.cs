using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        public Image chessSprites;

        // двовимірний масив для створення дошки
        public int[,] map = new int[8, 8]
        {
            {15, 14, 13, 12, 11, 13, 14, 15},
            {16, 16, 16, 16, 16, 16, 16, 16},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {26, 26, 26, 26, 26, 26, 26, 26},
            {25, 24, 23, 22, 21, 23, 24, 25},
        };

        public Button[,] butts = new Button[8, 8]; // можливі ходи для фігур

        public int currPlayer; // відповідає за гравця 

        public Button prevButton;

        public bool isMoving = false;

        public Form1()
        {
            InitializeComponent();
            
            chessSprites = new Bitmap("C:\\Users\\Andrew\\Desktop\\Programming\\C#\\Chess\\chess.png");

            Init();
        }

        public void Init()
        {
            map = new int[8, 8]
            {
                {15, 14, 13, 12, 11, 13, 14, 15},
                {16, 16, 16, 16, 16, 16, 16, 16},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0},
                {26, 26, 26, 26, 26, 26, 26, 26},
                {25, 24, 23, 22, 21, 23, 24, 25},
            };

            currPlayer = 1;

            CreateMap();
        }

        public void CreateMap()
        {
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j] = new Button(); // створюємо нову кнопку

                    Button butt = new Button();
                    butt.Size = new Size(60, 60);
                    butt.Location = new Point(j * 60, i * 60);

                    switch (map[i, j] / 10)
                    {
                        case 1:
                            Image part = new Bitmap(60, 60);
                            Graphics g = Graphics.FromImage(part);
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 70, 60), 0 + 150 * (map[i, j] % 10 - 1), 0, 220, 215, GraphicsUnit.Pixel);
                            butt.BackgroundImage = part;
                            break;
                        case 2:
                            Image part1 = new Bitmap(50, 50);
                            Graphics g1 = Graphics.FromImage(part1);
                            g1.DrawImage(chessSprites, new Rectangle(0, 0, 60, 60), 0 + 150 * (map[i, j] % 10 - 1), 150, 190, 215, GraphicsUnit.Pixel);
                            butt.BackgroundImage = part1;
                            break;
                    }
                    butt.BackColor = Color.White; 
                    butt.Click += new EventHandler(OnFigurePress);
                    this.Controls.Add(butt);

                    butts[i, j] = butt; // записуємо кнопку яку створили
                }
            }
        }

        public void DeactivateAllButtons() // виключаються всі кнопки
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
        }

        public void ActivateAllButtons() // включаються всі кнопки
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }
        }

        public void OnFigurePress(object sender, EventArgs e)
        {
            if(prevButton != null)
            {
                prevButton.BackColor = Color.White;
            }
            Button pressedButton = sender as Button;

            // Перевірка нажаття кнопки
            if (map[pressedButton.Location.Y / 60, pressedButton.Location.X / 60] != 0 && map[pressedButton.Location.Y / 60, pressedButton.Location.X / 60] / 10 == currPlayer)
            {
                CloseSteps();
                pressedButton.BackColor = Color.Red;
                DeactivateAllButtons();
                pressedButton.Enabled = true;
                ShowSteps(pressedButton.Location.Y / 60, pressedButton.Location.X / 60, map[pressedButton.Location.Y / 60, pressedButton.Location.X / 60]);

                if (isMoving) //якщо фігура нажата
                {
                    CloseSteps();
                    pressedButton.BackColor = Color.White;
                    ActivateAllButtons();
                    isMoving = false;
                }
                else
                {
                    isMoving = true;
                }
            }
            else
            {
                if (isMoving)
                {
                    int temp = map[pressedButton.Location.Y / 60, pressedButton.Location.X / 60];
                    map[pressedButton.Location.Y / 60, pressedButton.Location.X / 60] = map[prevButton.Location.Y / 60, prevButton.Location.X / 60];
                    map[prevButton.Location.Y / 60, prevButton.Location.X / 60] = temp;
                    pressedButton.BackgroundImage = prevButton.BackgroundImage;
                    prevButton.BackgroundImage = null;
                    isMoving = false;
                    CloseSteps();
                    ActivateAllButtons();
                    SwitchPlayer();
                }
            }

            prevButton = pressedButton;
        }

        public void ShowSteps(int IcurrFigure, int JcurrFigure, int currFigure) // показує варіанти ходів
        {
            int dir = currPlayer == 1 ? 1 : -1; // ходи для пішки
            switch (currFigure % 10) // отримуємо фігуру
            {
                case 6: //пішка
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure)) // якщо ми знаходимось на карті
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure] == 0) // якщо елемент нульовий
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow; // виділяємо жовтим
                            butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true; // включаємо кнопку
                        }
                    }

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) // перевіряємо чи є якась фігура по боках
                    {               // вгору           вправо по діагоналі
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer) // якщо на квадраті щось знаходиться і це не наша фігура
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].Enabled = true;
                        }
                    }
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1))
                    {                                    // вліво по діагоналі
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true;
                        }
                    }
                    break;
                case 5:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure); // ходи вверх, вліво, вправо, вниз
                    break;
                case 3:
                    ShowDiagonal(IcurrFigure, JcurrFigure); // ходи по діагоналі
                    break;
                case 2:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 1:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure, true);
                    ShowDiagonal(IcurrFigure, JcurrFigure, true);
                    break;
                case 4:
                    ShowHorseSteps(IcurrFigure, JcurrFigure);
                    break;
            }
        }

        public void ShowHorseSteps(int IcurrFigure, int JcurrFigure)
        {
            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure - 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure - 2);
            }
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1; //переініціалізовуємо змінну щоб рухатись по діагоналі
            for (int i = IcurrFigure - 1; i >= 0; i--) // права вверх діагональ
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j)) // зустріч фігури
                    {
                        break;
                    }
                }
                if (j < 7) // край карти
                {
                    j++;
                }
                else
                {
                    break;
                }
                if (isOneStep)
                {
                    break;
                }
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--) // ліва вверх діагональ
            {
                if (InsideBorder(i, j) && !DeterminePath(i, j))
                {
                    break;
                }
                if (j > 0)
                {
                    j--;
                }
                else
                {
                    break;
                }

                if (isOneStep)
                {
                    break;
                }
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++) // вниз ліва діагональ
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }
                if (j > 0)
                {
                    j--;
                }
                else
                {
                    break;
                }

                if (isOneStep)
                {
                    break;
                }
            }

            j = JcurrFigure + 1; // "+1" щоб показ відбувався не з місця фігури, а з наступного квадрата
            for (int i = IcurrFigure + 1; i < 8; i++) // вниз вправо діагональ
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                    {
                        break;
                    }
                }
                if (j < 7)
                {
                    j++;
                }
                else
                {
                    break;
                }

                if (isOneStep)
                {
                    break;
                }
            }
        }

        public void ShowVerticalHorizontal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            for (int i = IcurrFigure + 1; i < 8; i++) // всі можливі ходи вниз
            {
                if (InsideBorder(i, JcurrFigure)) // чи знаходяться в границі карти
                {
                    if (!DeterminePath(i, JcurrFigure)) // якщо функція вернула false закінчуємо показ ходів по цьому напрямку
                    {
                        break;
                    }
                }
                if (isOneStep) // всі можливі ходи не більше одного квадрата
                {
                    break;
                }
            }
            for (int i = IcurrFigure - 1; i >= 0; i--)// всі можливі ходи вверх
            {
                if (InsideBorder(i, JcurrFigure) && !DeterminePath(i, JcurrFigure))
                {
                    break;
                }
                if (isOneStep)
                {
                    break;
                }
            }
            for (int j = JcurrFigure + 1; j < 8; j++) // всі можливі ходи вправо
            {
                if (InsideBorder(IcurrFigure, j) && !DeterminePath(IcurrFigure, j))
                {
                    break;
                }
                if (isOneStep)
                {
                    break;
                }
            }
            for (int j = JcurrFigure - 1; j >= 0; j--) // всі можливі ходи вліво
            {
                if (InsideBorder(IcurrFigure, j) && !DeterminePath(IcurrFigure, j))
                {
                    break;
                }
                if (isOneStep)
                {
                    break;
                }
            }
        }

        public bool DeterminePath(int IcurrFigure, int j)
        {
            if (map[IcurrFigure, j] == 0) // якщо квадрат пустий то це можливий хід
            {
                butts[IcurrFigure, j].BackColor = Color.Yellow;
                butts[IcurrFigure, j].Enabled = true;
            }
            else // якщо квадрат зайнятий
            {
                if (map[IcurrFigure, j] / 10 != currPlayer) // якщо фігура не наша то показати як доступний хід
                {
                    butts[IcurrFigure, j].BackColor = Color.Yellow;
                    butts[IcurrFigure, j].Enabled = true;
                }
                return false;
            }
            return true;
        }

        public void SwitchPlayer() // допоміжна функція зміни користувача
        {
            if(currPlayer == 1)
            {
                currPlayer = 2;
            }
            else
            {
                currPlayer = 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            Init();
        }

        public bool InsideBorder(int ti, int tj) // перевіряє чи знаходиться значення всередині дошки
        {
            return ti < 8 && tj < 8 && ti >= 0 && tj >= 0;
        }

        public void CloseSteps() // скидує колір кнопок
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].BackColor = Color.White;
                }
            }
        }
    }
}