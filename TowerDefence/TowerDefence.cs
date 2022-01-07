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

        private void Restart()
        {
            _obj.checkpoints.Clear();
            _obj.objs.Clear();
            _obj.XP = 100;
            _obj.delList.Clear();
            img = new Bitmap(Size.Width, Size.Height);
            gf = Graphics.FromImage(img);
            var n = new Button(_obj, gf, new Point(74, Height - 150), Resources.BahaButton, 1);
            _obj.buttons.Add(n);
            _obj.objs.Add(n);
            n = new Button(_obj, gf, new Point(225, Height - 150), Resources.PressButton, 2);
            _obj.buttons.Add(n);
            _obj.objs.Add(n);
            n = new Button(_obj, gf, new Point(376, Height - 150), Resources.PVOButton, 3);
            _obj.buttons.Add(n);
            _obj.objs.Add(n);
            for (int i = 0; i < 8; i++)
            {
                var a = new CheckPoint(_obj, gf, pointsCheck[i], i);
                _obj.checkpoints.Add(a);
                _obj.objs.Add(a);
            }

            _obj.fort = new Fort(_obj, gf, new Point(350, 200));
            _obj.objs.Add(_obj.fort);
            _obj.checkpoints.Add(_obj.fort);
        }
        private void Render()
        {

            foreach (var v in _obj.objs) v.Step();
            gf.Clear(Color.FromArgb(1, 0, 45));
            gf.DrawString(_obj.XP.ToString(), new Font("Times New Roman", 16), new SolidBrush(Color.White), new Point(5, 5));
            foreach (var v in _obj.objs) v.Render();
            g.DrawImage(img, 0, 0);
            foreach (var a in _obj.delList)
                _obj.objs.Remove(a);

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
            _obj.sason = (_obj.sason + 1) % 16;
            if (_obj.sason % 8 == 1)
            {
                if (rand.Next(1, 4) == 2)
                    _obj.objs.Add(new Solder(_obj, gf, _obj.checkpoints[0].pos));
            }
            if (_obj.sason == 5)
            {
                if (rand.Next(1, 6) == 2)
                    _obj.objs.Add(new Vert(_obj, gf, _obj.checkpoints[0].pos));
            }
            if (_obj.XP <= 0) Restart();

        }

        private void TowerDefence_MouseDown(object sender, MouseEventArgs e)
        {
            int col = 0;
            foreach (var a in _obj.objs)
                if (a.CheckPos(e.Location))
                {
                    a.Click();
                    col++;
                }
            if (col == 0 && _obj.Rez > 0)
            {
                _obj.objs.Add(new Waerpon(_obj, gf, e.Location, _obj.spr[_obj.Rez]));
            }
        }
    }


    public class ObjectManager
    {
        public List<GameObject> objs = new List<GameObject>();
        public List<GameObject> checkpoints = new List<GameObject>();
        public List<GameObject> delList = new List<GameObject>();
        public List<Button> buttons = new List<Button>();
        public Image[] spr = { null, Resources.BahaSprite, Resources.PressSprite, Resources.PVOSprite };
        public Fort fort;
        public int sason = 0;
        public int XP = 100;
        public int Rez = 0;

    }
}
