using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Problem
{
    public partial class Form1 : Form
    {
        #region Переменные
        private Control Place; //используем, чтобы узнавать новое место (реализация dragNdrop)
        private Point prevPos; //сохраняем прошлое место (реализация dragNdrop)
        bool RUS = false; //переменная, которая проверяет включена ли русская локализация
        int passingValue = 0; //переменная, которая указывает с какого места начинается путь
        int pos = -1; 
        int who = 0; //переменная, которая помогает определить какой элемент мы передвигали или нажимали
        Pen p = new Pen(Color.Blue, 2); //синий цвет ручки, которая рисует соединяющие линии (толщина 2 пикслея)
        Graphics g; //графика, чтобы можно было рисовать соединяющие линии
        List<Control> places = new List<Control>(); //список типа controls, который в себе будет содержать названия и местоположения точек (x,y)
        Point position; //переменная для сохранения позиции курсора, т.к. нам нужно знать (x,y) курсора не от экрана, а непосредственно от приложения
        #endregion

        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics(); //создаем графику для панели на которой рисуются линии
        }

        #region Ивенты панели: Движение Курсора и Нажатие Правой Кнопки Мыши На Панели
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            position = panel1.PointToClient(Cursor.Position); //сохраняем местоположение курсора относительно приложения, а именно относительно панели на которой рисуем точки
            label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
            label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)    
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) //если нажата правая кнопка мыши
            {
                contextMenuStrip1.Show(); //показать меню с выбором (добавить, обновить)
            }
        }
        #endregion

        #region Реализация dragNdrop
        //сохраняем x: знач y: знач, т.к. это совсем другой объект, который не относится к панели на которой мы ставим точки, а позиция все равно другая
        private void MyCon_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default; //делаем курсор мыши обычным

            label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
            label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)
        }

        private void MyCon_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand; //делаем курсор типа рука, чтобы обозначить, что этот предмент взаимодействуемый

            label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
            label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)
        }

        private void MyCon_MouseUp(object sender, MouseEventArgs e)
        {
            if (position.Y < 400 && position.Y > 38 && position.X<637 && position.X>13) //проверка коллизии, чтобы нельзя было случайно вывести объект из видимости панели, на которой рисуются точки
            {
                #region Обновление значений списка во время их движения 
                position = panel1.PointToClient(Cursor.Position); //сохраняем местоположение курсора относительно приложения, а именно относительно панели на которой рисуем точки
                listBox1.Items.RemoveAt(who); //удаляем объект в списке, который мы видим справа от панели
                places[who].Location = position; //присваиваем позицию объекта, который мы двигаем позиции курсора
                listBox1.Items.Insert(who, places[who].Name + " |     x: " + places[who].Location.X + "  y: " + places[who].Location.Y); //добавляем элемент назад в список с новыми координатами х и у
                #endregion
                Place = null; //присваиваем значение место пустым, т.к. мы уже отпустили кнопку мыши
                Cursor = Cursors.Default; //делаем курсор стандартным
                if (e.Button == MouseButtons.Right) //если мы отжали правую кнопку мыши
                {
                    for (int k = 0; k < places.Count; k++)
                    {                       
                        if (position.Y >= places[k].Location.Y - 15 && position.Y <= places[k].Location.Y + 13 && position.X >= places[k].Location.X - 15 && position.X <= places[k].Location.X + 13) //коллизия обхекта на который мы нажали правой кнопкой мыши
                        {
                            contextMenuStrip2.Show(contextMenuStrip1, panel1.Location); //показать меню с выбором (назначит нач поз)
                            who = k; //запоминаем кого мы сейчас трогали
                        }
                    }
                }
                label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
                label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)
            }
        }

        private void MyCon_MouseMove(object sender, MouseEventArgs e)
        {
            if (position.Y < 400 && position.Y > 38 && position.X < 637 && position.X > 13) //проверка коллизии, чтобы нельзя было случайно вывести объект из видимости панели, на которой рисуются точки
            {
                position = panel1.PointToClient(Cursor.Position); //сохраняем местоположение курсора относительно приложения, а именно относительно панели на которой рисуем точки
                label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
                label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)

                if (Place == null || Place != sender) //если мы отпустили мышь или если объект не является контроллером, то
                {
                    return; //ничего не делаем, т.е. не двигаем
                }
                var location = Place.Location; //позиция

                #region Обновление значений списка во время их движения
                listBox1.Items.RemoveAt(who); //удаляем объект в списке, который мы видим справа от панели
                places[who].Location = position; //присваиваем позицию объекта, который мы двигаем позиции курсора
                listBox1.Items.Insert(who, places[who].Name + " |     x: " + places[who].Location.X + "  y: " + places[who].Location.Y); //добавляем элемент назад в список с новыми координатами х и у
                #endregion
                location.Offset(e.Location.X - prevPos.X, e.Location.Y - prevPos.Y); //делаем так, чтобы был оффсет, т.е. объект передвигался
                Place.Location = location; //сохраняем позицию обекта 
            }
        }

        private void MyCon_MouseDown(object sender, MouseEventArgs e)
        {
            if (position.Y < 400 && position.Y > 38 && position.X < 637 && position.X > 13) //проверка коллизии, чтобы нельзя было случайно вывести объект из видимости панели, на которой рисуются точки
            {
                position = panel1.PointToClient(Cursor.Position); //сохраняем местоположение курсора относительно приложения, а именно относительно панели на которой рисуем точки

                for (int i = 0; i < places.Count; i++)
                {
                    if (position.Y >= places[i].Location.Y - 15 && position.Y <= places[i].Location.Y + 13 && position.X >= places[i].Location.X - 15 && position.X <= places[i].Location.X + 13) //коллизия круга (объекта) на которого мы нажали
                    {
                        #region Обновление значений списка во время их движения
                        who = i; //сохраянем значение, того, которого мы трогали
                        listBox1.Items.RemoveAt(who); //удаляем объект в списке, который мы видим справа от панели
                        places[who].Location = position; //присваиваем позицию объекта, который мы двигаем позиции курсора
                        listBox1.Items.Insert(who,places[who].Name + " |     x: " + places[who].Location.X + "  y: " + places[who].Location.Y); //добавляем элемент назад в список с новыми координатами х и у
                        #endregion
                    }

                }

                Place = sender as Control; //объект является контроллером
                prevPos = e.Location; //прошлая позиция становится локацией мыши
                Cursor = Cursors.Hand; //делаем курсор рукой, чтобы пользователь видел, что он интерактивен

                label2.Text = Convert.ToString(position.X); //сохраняем слева внизу на экране (x: знач)
                label4.Text = Convert.ToString(position.Y); //сохраняем слева внизу на экране (y: знач)
            }
        }
        #endregion

        #region Рисуем соединения и Считаем расстояния
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = null; //текст путей сначала пуст
            double[,] matrix = new double[places.Count, places.Count]; //матрица для сохранения расстояний
            double[] completed = new double[places.Count]; //массив для сохранения завершения пути           

            double length=0;
            double cost = 0;

            void mincost(int city)
            {
                int ncity; //конечный город

                completed[city] = 1; //завершили


                textBox1.Text = textBox1.Text+Convert.ToString(city) + ">"; //показываем, соединения
                ncity = least(city); //ищем короткий путь от города

                if (ncity == 999) // если меньше
                {
                    ncity = passingValue; //конечный город = значению, которое мы назначали
                    textBox1.Text = textBox1.Text + Convert.ToString(ncity); //и добавляем его
                    cost += matrix[city, ncity]; //записываем стоимость путей

                    return;
                }
                mincost(ncity);
            } //класс для нахождения мин затрат

            int least(int c)
            {
                int i, nc = 999;
                double min = 999, kmin = 0;

                for (i = 0; i < completed.Length; i++)
                {
                    if ((matrix[c, i] != 0) && (completed[i] == 0)) //если у нас значение не 0 и город еще не завершен
                        if (matrix[c, i] + matrix[i, c] < min) //складываем значения расстояний из матрицы и сравниваем с меньшим
                        {
                            min = matrix[i, 0] + matrix[c, i]; //меньшее записываем расстояние этих значений
                            kmin = matrix[c, i]; //коэффецент расстояния
                            nc = i; //просто для памяти
                        }
                }

                if (min != 999) //если не равно этому значению
                    cost += kmin; //складываем прошлое знач на память

                return nc;
            } //класс для нахождения меньшего

            if (checkBox1.Checked) //если мы нажали на кнопку Расчитать
            {
                #region Нет точек
                if (places.Count <= 1)
                {
                    if(RUS==true) MessageBox.Show("Добавьте места, чтобы начать тур!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); //показываем ошибку на русском языке
                    else MessageBox.Show("Add places to start a tour!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //показываем ошибку на английском языке
                }
                #endregion
                #region Есть 2 и более точки
                else
                {
                    panel1.Enabled = false; //панель больше не действительна, пользователь больше не может взаимодействовать с точками или добавлять их
                    if(RUS==true) checkBox1.Text = "   Новый   "; //при русском языке кнопка становится на новый
                    else checkBox1.Text = "    New   "; //при английском языке кнопка становится на новый
                    button2.Enabled = false; //кнопка очисти отключается

                    #region Находим длинны сторон и сохраняем в матрицу
                    for (int m = 0; m < places.Count; m++)
                        for(int f = 0; f < places.Count; f++)
                        {
                            length = Math.Sqrt(Math.Pow(places[m].Location.X - places[f].Location.X, 2) + Math.Pow(places[m].Location.Y - places[f].Location.Y, 2)); //находим расстоянием по школьной формуле l^2 = (x1-x2)^2 + (y1-y2)^2
                            matrix[m, f] = length; //сохраняем в матрицу                          
                        }
                    #endregion
                    mincost(passingValue); //передаем с чего начинаем     
                    label6.Text = Convert.ToString(cost); //показываем расстояние 

                    #region Запоминаем соединения
                    string s = textBox1.Text; //берем путь из текста, который мы записали
                    int[] a = new int[places.Count+1]; //массив будет на 1 больше, т.к. еще обратный путь

                    int lol = 0; //счётчик для того, чтобы вписать даннй по порядку
                    int i = 0, j = 0; //счётчики для форов

                    for (; i < s.Length; i++) //по длинну текста
                    {
                        if (i == 0) //находим первое соединение
                        {
                            for (int h = 0; h < s.Length; h++)
                                if (s[h] == '>') //если находим такой символ
                                {
                                    a[lol] = Convert.ToInt32(s.Substring(i, h)); //берем строку от позиции i длинной в h и переводим в переменную инт, сохраняем в массив соединений
                                    lol++; //увеличиваем счетчик, для дальнейших итераций
                                    i++;//выходим из ифа
                                    break; //покидаем этот фор
                                }

                        }
                        if (s[i] == '>') //если находим символ
                        {
                            for (j = i + 1; j < s.Length; j++)
                            {
                                if (s[j] == '>')
                                {
                                    a[lol] = Convert.ToInt32(s.Substring(i + 1, j - i - 1)); //берем строку от позиции i+1 длинной в j-i-1 и переводим в переменную инт, сохраняем в массив соединений
                                    lol++; //увеличиваем счетчик, для дальнейших итераций
                                    i = j; //сохраняем значение для i, чтобы дальше идти и не натыкаться на те же грабли
                                }
                                else if (j == s.Length - 1) //находим последнее соединение
                                    a[lol] = Convert.ToInt32(s.Substring(i + 1, s.Length - i - 1)); //берем строку от позиции i+1 длинной в s.Length - i - 1 и переводим в переменную инт, сохраняем в массив соединений
                            }
                        }
                    }
                    #endregion

                    #region Рисуем линии
                    for (int k = 0; k < a.Length-1; k++)
                        g.DrawLine(p, places[a[k]].Location, places[a[k+1]].Location); //берем информацию из списка и используем массив соединений, чтобы соединить эти точки линий
                    #endregion
                }
                #endregion
            }
            else //если мы нажали на кнопку Новый
            {
                #region Стираем рисованные линии и позволяем снова взаимодействовать
                label6.Text = "0000000000000"; //делаем текст расстояний 0
                button2.Enabled = true; //кнопка очистки теперь действительна
                panel1.Enabled = true; //мы можем взаимодействовать с панелью
                if(RUS==true) checkBox1.Text = "Расчитать"; //если стоит русский, то текст кнопки Расчитать
                else checkBox1.Text = "Calculate"; //если стоит английский, то текст кнопки Расчитать
                #endregion
            }
        }
        #endregion

        #region Кнопка Очистки
        private void button2_Click(object sender, EventArgs e)
        {
            label9.Text = "0";
            passingValue = 0;
            panel1.Controls.Clear(); //удаляем все объекты с панели 
            listBox1.Items.Clear(); //очищаем список справа
            places.Clear(); //очищаем список мест
            who = 0; //у нас никого нету
            pos = -1; //позиция возращается в нач положение
        }
        #endregion

        #region Всплывающее меню: Добавить и Старт Поз
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (position.Y < 399 && position.X < 639 && position.Y > 14 && position.X > 14) //проверка коллизии, чтобы нельзя было случайно вывести объект из видимости панели, на которой рисуются точки
            {
                #region Создаваемый объект
                Label name = new Label(); //мы создаем лейбел
                pos++; //счетчик для названий
                #region Внешний вид объекта
                name.Text = Convert.ToString(pos); //текст создаваемого объекта равен счетчику
                name.Left = position.X - 15; //для того, чтобы он ставился в центре курсора X
                name.Top = position.Y - 15; //для того, чтобы он ставился в центре курсора Y
                name.ForeColor = Color.White; //белый цвет текста
                name.BackColor = Color.Red; //красный фон
                name.Size = new Size(30, 30); //размер (30,30) пикселей
                name.TextAlign = ContentAlignment.MiddleCenter; //отцентрируем цвет
                GraphicsPath gp = new GraphicsPath(); //графический путь
                gp.AddEllipse(-1, 0, name.Width, name.Height); //рисуем круг размерами объекта, который мы создаем
                name.Region = new Region(gp); //область окна объекта
                name.Invalidate(); //перерисовываем всю графику на самом объекте
                #endregion
                #endregion

                panel1.Controls.Add(name);//рисуем этот объект на панели
                places.Add(new Control { Name = Convert.ToString(pos), Location = position }); //добавляем в список имя и местоположение объекта
                listBox1.Items.Add(places[pos].Name + " |     x: " + places[pos].Location.X + "  y: " + places[pos].Location.Y); //добавляем элемент назад в список с новыми координатами х и у

                #region Система ивентов мыши по объекту
                name.MouseDown += MyCon_MouseDown; //если клавиша мыши зажата на объекте
                name.MouseMove += new MouseEventHandler(MyCon_MouseMove); //если мышь двигается на объекте
                name.MouseUp += new MouseEventHandler(MyCon_MouseUp); //если клавиша мыши отжата на объекте
                name.MouseEnter += new EventHandler(MyCon_MouseEnter); //если мышь заходит на объект 
                name.MouseLeave += new EventHandler(MyCon_MouseLeave); //если мышь уходит с объекта
                #endregion
            }
        }

        private void setStartPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < places.Count; k++) //по кол-во объектов в списке
            {
                if (position.Y >= places[k].Location.Y - 15 && position.Y <= places[k].Location.Y + 13 && position.X >= places[k].Location.X - 15 && position.X <= places[k].Location.X + 13) //коллизия круга
                {
                    passingValue = k; //значение, с которого начинается путь (из него надо выйти и к нему надо будет в конце пути вернуться)
                    label9.Text = Convert.ToString(passingValue); //показываем, что у нас новое нач место
                }
            }        
        }
        #endregion

        #region Окна: О программе и Помощь
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RUS == true) MessageBox.Show("Данная программа является стандартным представлением задачи коммивояжера. TSP - продавец должен посещать все места один раз и возвращаться в исходную точку менее затратным способом. Пользователь может нарисовать собственную карту.                                          Создатель: Тарас Андриевский 2020 год.", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information); //показываем окно о программе, если включен русский язык
            else MessageBox.Show("This programm is standart representation of travelling salesman problem. TSP - salesman should visit all places once in a time and get back in the starting point using the less cost way. User is able to draw own map.                                         Creator: Taras Andrievskii 2020 year.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information); //показываем окно о программе, если включен английский язык
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(RUS==true) MessageBox.Show("Щелкните правой кнопкой мыши и выберите «Добавить». Вы можете перемещать точки и устанавливать начальную точку для путешествия. Нажмите кнопку «Рассчитать», чтобы увидеть путь с меньшими затратами.", "Помощь", MessageBoxButtons.OK, MessageBoxIcon.Question); //показываем окно помощи, если включен русский язык
            else MessageBox.Show("Click right mouse button and select «Add». You able to move dots around and set staring point for a journey. Click «Calculate» button to see a less cost path.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Question); //показываем окно помощи, если включен английский язык
        }
        #endregion

        #region Язык
        private void russianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RUS = true; //русский язык включен
            #region Присваиваем значения тексту, кнопка и т.д.
            label5.Text = "длинна:";
            label8.Text = "старт поз: ";
            languageToolStripMenuItem.Text = "Язык";
            russianToolStripMenuItem.Text = "Русский";
            englishToolStripMenuItem.Text = "Английский";
            aboutToolStripMenuItem.Text = "О программе";
            helpToolStripMenuItem.Text = "Справка";
            addToolStripMenuItem.Text = "Добавить";
            refreshToolStripMenuItem.Text = "Обновить";
            setStartPointToolStripMenuItem.Text = "Назначить старт поз";
            button2.Text = "Очистить";
            checkBox1.Text = "Расчитать";
            #endregion
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RUS = false; //русский язык выключен
            #region Присваиваем значения тексту, кнопка и т.д.
            label5.Text = "length:";
            label8.Text = "start point:";
            languageToolStripMenuItem.Text = "Language";
            russianToolStripMenuItem.Text = "Russian";
            englishToolStripMenuItem.Text = "English";
            aboutToolStripMenuItem.Text = "About";
            helpToolStripMenuItem.Text = "Help";
            addToolStripMenuItem.Text = "Add";
            refreshToolStripMenuItem.Text = "Refresh";
            setStartPointToolStripMenuItem.Text = "Set start point";
            button2.Text = "Clear";
            checkBox1.Text = "Calculate";
            #endregion
        }
        #endregion
    }
}