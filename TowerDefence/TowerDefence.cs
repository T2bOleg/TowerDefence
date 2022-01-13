using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TowerDefence
{
    public partial class TowerDefence : Form
    {

        #region points
        Point[] pointsCheck = {new Point(1, 200),
                new Point(100, 200),
                new Point(100, 400),
                new Point(200, 400),
                new Point(200, 50),
                new Point(450, 50),
                new Point(450, 400),
                new Point(350, 400)};
        #endregion
        ObjectManager _obj;
        public TowerDefence()
        {
            InitializeComponent();
        }
        Graphics gf;
        private void TowerDefence_Paint(object sender, PaintEventArgs e)
        {

            g = CreateGraphics();
            gf = Graphics.FromImage(img);
            foreach (var n in _obj.buttons)
                _obj.objs.Add(n);
            Render();
        }
        Bitmap img;
        Point mousePoint;
        private void Restart()
        {
            _obj.checkpoints.Clear();
            _obj.objs.Clear();
            _obj.XP = 100;
            _obj.USD = 250;
            _obj.delList.Clear();
            img = new Bitmap(Size.Width, Size.Height);
            gf = Graphics.FromImage(img);
            _obj.buttons.Add(new Button(_obj, gf, new Point(124, Height - 100), Resources.PVOButton, 1));
            _obj.buttons.Add(new Button(_obj, gf, new Point(275, Height - 100), Resources.PressButton, 2));
            _obj.buttons.Add(new Button(_obj, gf, new Point(426, Height - 100), Resources.BahaButton, 3));

            for (int i = 0; i < 8; i++)
            {
                _obj.checkpoints.Add(new CheckPoint(_obj, gf, pointsCheck[i], i));
            }
            _obj.fort = new Fort(_obj, gf, new Point(350, 200));
            // for (int i = 0; i < _obj.checkpoints.Count - 1; i++)
            //_obj.checkpoints[i].SetCenterPoint(_obj.checkpoints[i].pos);
        }
        private void Render()
        {

            foreach (var v in _obj.objs) v.Step();
            if (!_obj.isButtonSel()) _obj.Rez = 0;
            gf.Clear(Color.FromArgb(1, 0, 45));
            gf.DrawString($"♥{_obj.XP}", new Font("Times New Roman", 16), new SolidBrush(Color.White), new Point(5, 1));
            gf.DrawString($"${_obj.USD}", new Font("Times New Roman", 16), new SolidBrush(Color.White), new Point(5, 21));
            gf.DrawString($"L{_obj.Lavle}", new Font("Times New Roman", 16), new SolidBrush(Color.White), new Point(5, 41));
            foreach (var v in _obj.objs) v.Render();


            int col = 0;
            foreach (var a in _obj.objs)
                if (a.Intersection(mousePoint))
                {
                    col++;
                }
            if (col == 0)
            {
                int dist = Data.dist[_obj.Rez, _obj.Rez];
                gf.DrawImage(Data.spr[_obj.Rez], mousePoint - new Size(15, 15));
                gf.DrawEllipse(new Pen(Color.FromArgb(50, 255, 255, 255)), mousePoint.X - dist, mousePoint.Y - dist, dist * 2, dist * 2);
            }

            g.DrawImage(img, 0, 0);
            foreach (var a in _obj.delList)
            {
                _obj.objs.Remove(a);
                _obj.enemies.Remove(a);
            }

            _obj.delList.Clear();
        }

        private void TowerDefence_Load(object sender, EventArgs e)
        {

            _obj = new ObjectManager();
            Restart();
        }
        Random rand = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            Render();
            _obj.sason = (_obj.sason + 1) % 300;
            if (_obj.sason == 299) _obj.Lavle += 1;

            if (_obj.sason % (8) == 1)
            {
                if (rand.Next(Math.Abs((5 - (_obj.Lavle / 3)) % 6)) == 0)
                    _obj.objs.Add(new Solder(_obj, gf, _obj.checkpoints[0].pos));
            }
            if (_obj.sason % 16 == 5)
            {
                if (rand.Next(Math.Abs((6 - (_obj.Lavle / 3)) % 7)) == 0)
                    _obj.objs.Add(new Vert(_obj, gf, _obj.checkpoints[0].pos));
            }
            if (_obj.XP <= 0) Restart();

        }

        private void TowerDefence_MouseDown(object sender, MouseEventArgs e)
        {
            int col = 0;
            foreach (var a in _obj.objs)
            {
                if (a.CheckPos(e.Location))
                {
                    a.Click();
                }
                if (a.Intersection(e.Location))
                    col++;
            }
            if (col == 0)
            {
                if (_obj.Rez > 0)
                {
                    _obj.objs.Add(new Waerpon(_obj, gf, e.Location, _obj.Rez));
                    if (Data.prise[_obj.Rez,0] > _obj.USD)
                        _obj.selected = null;

                }
                else
                {
                    _obj.selected=null;
                }
            }
        }

        private void TowerDefence_MouseMove(object sender, MouseEventArgs e)
        {
            mousePoint = e.Location;
        }
    }


    public class ObjectManager
    {
        public List<GameObject> objs = new List<GameObject>();
        public List<CheckPoint> checkpoints = new List<CheckPoint>();
        public List<Enemy> delList = new List<Enemy>();
        public List<Button> buttons = new List<Button>();
        public List<Enemy> enemies = new List<Enemy>();
        public Fort fort;
        public GameObject selected;
        public int sason = 0;
        public int Lavle = 1;
        public int XP = 100;
        public int USD = 250;
        public int Rez = 0;

        public bool isButtonSel() => (selected == buttons[0] || selected == buttons[1] || selected == buttons[2]);
    }
}
