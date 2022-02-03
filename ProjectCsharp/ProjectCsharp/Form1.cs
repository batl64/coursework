using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectCsharp
{
    public partial class Form1 : Form
    {
        const int mapSize = 7;
        const int cellSize = 50;
        List<Button> simpleSteps = new List<Button>();
        int currentPlayer;
        Button prevButton;
        Button pressedButton;
        bool isMoving;
        bool isContinue = false;
        int countEatSteps = 0;




        int[,] map = new int[mapSize, mapSize];
        Button[,] buttons = new Button[mapSize, mapSize];
        Image chicken;
        Image wolf;
        public Form1()
        {
            InitializeComponent();
            chicken = new Bitmap(new Bitmap(@"C:\Users\maksi\Downloads\Проект\К.png"), new Size(cellSize, cellSize));
            wolf = new Bitmap(new Bitmap(@"C:\Users\maksi\Downloads\Проект\Л.png"), new Size(cellSize, cellSize));
            this.Text = "Wolf and chicken";
            Init();
        }
        public void Init()
        {
            currentPlayer = 1;
            isMoving = false;
            prevButton = null;

            map = new int[mapSize, mapSize] {
            { 6, 6, 0, 0, 0, 6, 6 },
            { 6, 6, 0, 0, 0, 6, 6 },
            { 0, 0, 2, 0, 2, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 6, 6, 1, 1, 1, 6, 6 },
            { 6, 6, 1, 1, 1, 6, 6 },
            };

            CreateMap();
        }

        public void WinChicken()
        {
            int countzone = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 2; j < 5; j++)
                {
                    if (map[i, j] == 1)
                    {
                        countzone++;
                    }
                }
            }
            if (countzone == 9)
            {
                this.Controls.Clear();
                Init();
            }

        }
        public void FoxWin()
        {

            int countchiken = 0;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 1)
                    {

                        countchiken++;
                    }


                }
            }
            if (countchiken < 9)
            {
                this.Controls.Clear();
                Init();
            }
        }
        public void CreateMap()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize;

            for (int i = 0; i < mapSize; i++)
            {

                for (int j = 0; j < mapSize; j++)
                {

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Click += new EventHandler(OnFigurePress);
                    if (map[i, j] == 1)
                        button.Image = chicken;
                    else if (map[i, j] == 2) button.Image = wolf;
                    if (map[i, j] == 6)
                    {
                        button.Visible = false;
                    }
                    button.BackColor = Color.White;
                    button.ForeColor = Color.Red;

                    buttons[i, j] = button;

                    this.Controls.Add(button);
                }




            }
        }
        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1;
            FoxWin();
            WinChicken();
        }

        public void OnFigurePress(object sender, EventArgs e)
        {
            if (prevButton != null)
                prevButton.BackColor = Color.White;

            pressedButton = sender as Button;

            if (map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] != 0 && map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == currentPlayer)
            {
                CloseSteps();
                pressedButton.BackColor = Color.Red;
                DeactivateAllButtons();
                pressedButton.Enabled = true;
                countEatSteps = 0;
                ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);

                if (isMoving)
                {
                    CloseSteps();
                    pressedButton.BackColor = Color.White;
                    ShowPossibleSteps();
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    isContinue = false;
                    if (Math.Abs(pressedButton.Location.X / cellSize - prevButton.Location.X / cellSize) > 1 || Math.Abs(pressedButton.Location.Y / cellSize - prevButton.Location.Y / cellSize) > 1)
                    {
                        isContinue = true;
                        DeleteEaten(pressedButton, prevButton);
                    }
                    int temp = map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize];
                    map[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = map[prevButton.Location.Y / cellSize, prevButton.Location.X / cellSize];
                    map[prevButton.Location.Y / cellSize, prevButton.Location.X / cellSize] = temp;
                    pressedButton.Image = prevButton.Image;
                    prevButton.Image = null;
                    pressedButton.Text = prevButton.Text;
                    prevButton.Text = "";
                    countEatSteps = 0;
                    isMoving = false;
                    CloseSteps();
                    DeactivateAllButtons();
                    ShowSteps(pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize);
                    if (countEatSteps == 0 || !isContinue)
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }
                    else if (isContinue)
                    {
                        pressedButton.BackColor = Color.Red;
                        pressedButton.Enabled = true;
                        isMoving = true;
                    }

                }
            }
            prevButton = pressedButton;
        }
        public void ShowPossibleSteps()
        {
            bool isOneStep = true;
            bool isEatStep = false;
            DeactivateAllButtons();
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == currentPlayer)
                    {
                        if (IsButtonHasEatStep(i, j, isOneStep) && currentPlayer==2)
                        {
                            isEatStep = true;
                            buttons[i, j].Enabled = true;
                        }
                    }
                }
            }
            if (!isEatStep)
                ActivateAllButtons();

        }

        public void DeleteEaten(Button endButton, Button startButton)
        {
            if (currentPlayer == 2)
            {
                int count;
                if (endButton.Location.Y / cellSize - startButton.Location.Y / cellSize != 0)
                {
                    count = Math.Abs(endButton.Location.Y / cellSize - startButton.Location.Y / cellSize);
                }
                else
                {
                    count = Math.Abs(endButton.Location.X / cellSize - startButton.Location.X / cellSize);
                }
                int startIndexX = endButton.Location.Y / cellSize - startButton.Location.Y / cellSize;
                int startIndexY = endButton.Location.X / cellSize - startButton.Location.X / cellSize;

                int currCount = 0;

                int i = Math.Abs(startButton.Location.Y / cellSize + startIndexX / 2);
                int j = Math.Abs(startButton.Location.X / cellSize + startIndexY / 2);
                while (currCount < count - 1)
                {
                    map[i, j] = 0;
                    buttons[i, j].Image = null;
                    if (endButton.Location.Y / cellSize - startButton.Location.Y / cellSize != 0)
                    {
                        i += startIndexX;
                    }
                    else
                    {
                        j += startIndexY;

                    }
                    currCount++;
                }
            }

        }
        public void ShowSteps(int iCurrFigure, int jCurrFigure, bool isOneStep = true)
        {
            simpleSteps.Clear();
            Show(iCurrFigure, jCurrFigure, isOneStep);
            if (countEatSteps > 0)
            {
                CloseSimpleSteps(simpleSteps);
            }

        }

        public void Show(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {

            for (int i = IcurrFigure + 1; i < 7; i++)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }


                if (isOneStep)
                    break;
            }

            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }


                if (isOneStep)
                    break;
            }

            for (int j = JcurrFigure + 1; j < 7; j++)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }


                if (isOneStep)
                    break;
            }

            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }


                if (isOneStep)
                    break;
            }


            for (int i = IcurrFigure + 1; i < 7; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }


                if (isOneStep)
                    break;
            }

            for (int i = IcurrFigure - 1; i >= 0; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }


                if (isOneStep)
                    break;
            }
            for (int j = JcurrFigure + 1; j < 7; j++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }


                if (isOneStep)
                    break;
            }

            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }


                if (isOneStep)
                    break;
            }

        }

        public bool DeterminePath(int ti, int tj)
        {
            if (map[ti, tj] == 0 && !isContinue)
            {
                buttons[ti, tj].BackColor = Color.Yellow;
                buttons[ti, tj].Enabled = true;
                simpleSteps.Add(buttons[ti, tj]);
            }
            else
            {
                if (map[ti, tj] != currentPlayer)
                {                   
                    ShowProceduralEat(ti, tj);
                }
                return false;
            }
            return true;
        }
        public void CloseSimpleSteps(List<Button> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = Color.White;
                    simpleSteps[i].Enabled = false;
                }
            }
        }

        public void ShowProceduralEat(int i, int j, bool isOneStep = true)
        {
            if (currentPlayer == 2)
            {
                int dirX = i - pressedButton.Location.Y / cellSize;
                int dirY = j - pressedButton.Location.X / cellSize;


                int il = i;
                int jl = j;
                bool isEmpty = true;

                while (IsInsideBorders(il, jl))
                {

                    if (map[il, jl] != 0 && map[il, jl] != currentPlayer)
                    {
                        isEmpty = false;
                        break;
                    }
                    if (i - pressedButton.Location.Y / cellSize != 0)
                    {
                        il += dirX;
                    }
                    else
                    {
                        jl += dirY;
                    }
                    if (isOneStep)
                        break;

                }

                if (isEmpty)
                    return;
                List<Button> toClose = new List<Button>();
                bool closeSimple = false;
                int ik = il;
                int jk = jl;

                if (i - pressedButton.Location.Y / cellSize != 0)
                {
                    ik += dirX;
                }
                else
                {
                    jk += dirY;
                }

                while (IsInsideBorders(ik, jk))
                {
                    if (map[ik, jk] == 0)
                    {
                        if (IsButtonHasEatStep(ik, jk, isOneStep) && currentPlayer==2)
                        {
                            closeSimple = true;
                        }
                        else
                        {
                            toClose.Add(buttons[ik, jk]);
                        }
                        buttons[ik, jk].BackColor = Color.Yellow;
                        buttons[ik, jk].Enabled = true;
                        countEatSteps++;

                    }
                    else break;
                    if (isOneStep)
                        break;

                    if (i - pressedButton.Location.Y / cellSize != 0)
                    {
                        ik += dirX;
                    }
                    else
                    {
                        jk += dirY;
                    }



                }
                if (closeSimple && toClose.Count > 0)
                {
                    CloseSimpleSteps(toClose);
                }
            }
        }

        public bool IsButtonHasEatStep(int IcurrFigure, int JcurrFigure, bool isOneStep)
        {
            bool eatStep = false;
            for (int i = IcurrFigure + 1; i < 7; i++)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (map[i, JcurrFigure] != 0 && map[i, JcurrFigure] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, JcurrFigure))
                            eatStep = false;
                        else if (map[i + 1, JcurrFigure] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }


                if (isOneStep)
                    break;
            }

            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, JcurrFigure))
                {
                    if (map[i, JcurrFigure] != 0 && map[i, JcurrFigure] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, JcurrFigure))
                            eatStep = false;
                        else if (map[i - 1, JcurrFigure] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }


                if (isOneStep)
                    break;
            }

            for (int j = JcurrFigure + 1; j < 7; j++)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (map[IcurrFigure, j] != 0 && map[IcurrFigure, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(IcurrFigure, j + 1))
                            eatStep = false;
                        else if (map[IcurrFigure, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }


                if (isOneStep)
                    break;
            }

            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(IcurrFigure, j))
                {
                    if (map[IcurrFigure, j] != 0 && map[IcurrFigure, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(IcurrFigure, j - 1))
                            eatStep = false;
                        else if (map[IcurrFigure, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }

                if (isOneStep)
                    break;

            }
            return eatStep;
        }
        public void CloseSteps()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].BackColor = Color.White;
                }
            }
        }
        public bool IsInsideBorders(int ti, int tj)
        {
            if (ti >= mapSize || tj >= mapSize || ti < 0 || tj < 0 || ti < 2 && tj < 2 || ti > 4 && ti < mapSize && tj < 2 || ti < 2 && tj > 4 && tj < mapSize || tj > 4 && ti > 4 && tj < mapSize && ti < mapSize)
            {
                return false;
            }
            return true;
        }
        public void ActivateAllButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }
        public void DeactivateAllButtons()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }
    }
}