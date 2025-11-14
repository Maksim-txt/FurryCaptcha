using System.IO.Packaging;
using System.Resources;
using System.IO;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using Image = System.Drawing.Image;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Flurl.Http;
using System.Text;
using TestCaptcha.Models;

namespace TestCaptcha
{
    public partial class Form1 : Form
    {

        PictureBox[] boxes;
        PictureBox selected;
        StringBuilder winPoitions = new StringBuilder("");
        StringBuilder currentPositions = new StringBuilder("");

        private void SetRandomLocation(PictureBox[] boxes)
        {
            Random random = new();

            for (int i = 0; i < boxes.Length; i++)
            {
                int randomPB = random.Next(boxes.Length);
                Image temp = boxes[i].Image;
                boxes[i].Image = boxes[randomPB].Image;
                boxes[randomPB].Image = temp;
            }
        }
        public static PictureBox[] SetTags(PictureBox[] boxes)
        {

            for (int i = 0; i < 4; i++)
            {
                boxes[i].Image.Tag = i; // если заменить i на Random.Next(i) при выборе картинки, то и Tag должен быть равен значению, полученному из Random.Next(i)
            }

            return boxes;
        }

        private void CheckWin(PictureBox[] boxes)
        {
            currentPositions.Clear();

            if (winPoitions.Length < 4)
                return;

            for (int i = 0; i < boxes.Length; i++)
            {
                var temp = (Tag)boxes[i].Tag;
                currentPositions.Append(temp.Id);
                currentPositions.Append(temp.ClickCount);
            }
            Console.WriteLine($"{currentPositions} == {winPoitions}");

            if (currentPositions.ToString() == winPoitions.ToString())
            {
                MessageBox.Show("Капча пройдена", "Капча", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentPositions.Clear();
                return;
            }
            else
            {
                MessageBox.Show("Капча не пройдена", "Капча", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentPositions.Clear();
                return;
            }
        }

        public Form1()
        {

            InitializeComponent();

            boxes = [pictureBox1, pictureBox2, pictureBox3, pictureBox4];

            try
            {

            }
            catch (Exception)
            {
                MessageBox.Show("No images found", "No Images Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var box in boxes)
            {
                box.AllowDrop = true;
                box.DragDrop += PictureBox_DragDrop;
                box.DragEnter += PictureBox_DragEnter;
                box.MouseClick += PictureBox_MouseClick;
                box.MouseMove += PictureBox_MouseMove;
                box.DoubleClick += (_, _) => { };
                box.MouseDoubleClick += (_, _) => { };
            }
        }

        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            var target = (PictureBox)sender;
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                var source = (PictureBox)e.Data.GetData(typeof(PictureBox));
                if (source != target)
                {
                    Console.WriteLine("Do DragDrop from " + source.Name + " to " + target.Name);
                    // You can swap the images out, replace the target image, etc.
                    SwapImages(source, target);


                    selected = null;
                    SelectBox(target);
                    return;
                }
            }
            Console.WriteLine("Don't do DragDrop");
        }

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var pb = (PictureBox)sender;
            var temp = (Tag)pb.Tag;
            temp.ClickCount += 1;
            if (temp.ClickCount >= 4)
            {
                temp.ClickCount = 0;
            }

            pb.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pb.Tag = temp;
            pb.Refresh();
            SelectBox((PictureBox)sender);
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pb = (PictureBox)sender;
                if (pb.Image != null)
                {
                    pb.DoDragDrop(pb, DragDropEffects.Move);
                }
            }
        }


        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            var pb = (PictureBox)sender;
            pb.BackColor = Color.White;
            if (selected == pb)
            {
                ControlPaint.DrawBorder(e.Graphics, pb.ClientRectangle,
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Left
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Top
                   Color.Blue, 5, ButtonBorderStyle.Solid,  // Right
                   Color.Blue, 5, ButtonBorderStyle.Solid); // Bottom
            }
        }

        private void SelectBox(PictureBox pb)
        {
            if (selected != pb)
            {
                selected = pb;
            }
            else
            {
                selected = null;
            }

            // Cause each box to repaint
            foreach (var box in boxes) box.Invalidate();
        }

        private void SwapImages(PictureBox source, PictureBox target)
        {
            if (source.Image == null && target.Image == null)
            {
                return;
            }

            var temp = target.Image;
            target.Image = source.Image;
            source.Image = temp;
            var tempTag = source.Image.Tag;
            target.Tag = new Tag(target.Image.Tag, 0);
            source.Tag = new Tag(tempTag, 0);
        }
        

        private async void button1_Click(object sender, EventArgs e)
        {
            await FurryAPI.GetRandomImagesAsync(boxes);
            SetTags(boxes);
            SetRandomLocation(boxes);
            for (int i = 0; i < 4; i++)
            {
                boxes[i].Tag = new Tag(boxes[i].Image.Tag, 0); 
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CheckWin(boxes);
        }

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(pictureBox4, 1, 1);
            tableLayoutPanel1.Controls.Add(pictureBox3, 0, 1);
            tableLayoutPanel1.Controls.Add(pictureBox2, 1, 0);
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(218, 218);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(113, 113);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(100, 100);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 3;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(5, 113);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(100, 100);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(113, 5);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(100, 100);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(5, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(100, 100);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(324, 91);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Новая капча";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(324, 120);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "Проверить капчу";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            ClientSize = new Size(520, 325);
            Controls.Add(this.button2);
            Controls.Add(button1);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Load += Form1_Load;
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            bool isImageLoaded = await FurryAPI.GetRandomImagesAsync(boxes);
            
            if (isImageLoaded)
            {
                for (int i = 0; i < boxes.Length; i++)
                {
                    winPoitions.Append(i);
                    winPoitions.Append(0);
                }

                SetTags(boxes);
                SetRandomLocation(boxes);
                for (int i = 0; i < 4; i++)
                {
                    boxes[i].Tag = new Tag(boxes[i].Image.Tag, 0);
                }
            }

        }
    }
}
