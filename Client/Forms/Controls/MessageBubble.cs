using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Client.Forms.Controls
{
    public class MessageBubble : UserControl
    {
        private FlowLayoutPanel _stack;
        private Label _lblName;
        private Label _lblText;
        private Label _lblMeta;
        private LinkLabel _lnkFile;

        public bool IsMine { get; private set; }
        private Color _bubbleColor;

        private const int Radius = 16;

        private static readonly Color MineBack = Color.FromArgb(56, 74, 175);
        private static readonly Color OtherBack = Color.FromArgb(42, 45, 55);

        public MessageBubble()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            BuildUI();
        }

        private void BuildUI()
        {
            _lblName = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 220, 220),
                Margin = new Padding(0, 0, 0, 6)
            };

            _lblText = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Margin = new Padding(0, 0, 0, 8)
            };

            _lnkFile = new LinkLabel
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(170, 210, 255),
                ActiveLinkColor = Color.FromArgb(200, 230, 255),
                VisitedLinkColor = Color.FromArgb(170, 210, 255),
                LinkBehavior = LinkBehavior.HoverUnderline,
                Margin = new Padding(0, 0, 0, 8),
                Visible = false
            };

            _lnkFile.LinkClicked += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(_lnkFile.Tag as string))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(_lnkFile.Tag.ToString())
                        {
                            UseShellExecute = true
                        });
                    }
                    catch { }
                }
            };

            _lblMeta = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(190, 190, 190),
                Margin = new Padding(0)
            };

            _stack = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            _stack.Controls.Add(_lblName);
            _stack.Controls.Add(_lblText);
            _stack.Controls.Add(_lnkFile);
            _stack.Controls.Add(_lblMeta);

            Controls.Add(_stack);

            Padding = new Padding(14, 12, 14, 10);
            MinimumSize = new Size(120, 0);
        }

        public void SetTextMessage(bool isMine, string name, string text, DateTime time, string? status = null)
        {
            IsMine = isMine;
            _bubbleColor = isMine ? MineBack : OtherBack;

            _lblName.Text = name;
            _lblText.Text = text;
            _lnkFile.Visible = false;

            string meta = time.ToString("HH:mm");

            if (isMine)
            {
                var s = status ?? "Sent";

                string marks = s switch
                {
                    "Sent" => " ✓",
                    "Delivered" => " ✓✓",
                    "Read" => " ✓✓",
                    _ => ""
                };

                meta += marks;

                // 🎨 Цвет галочек
                _lblMeta.ForeColor = s switch
                {
                    "Sent" => Color.FromArgb(170, 170, 170),        // светло-серый
                    "Delivered" => Color.FromArgb(170, 170, 170),   // серый
                    "Read" => Color.FromArgb(80, 170, 255),         // голубой
                    _ => Color.FromArgb(190, 190, 190)
                };
            }
            else
            {
                _lblMeta.ForeColor = Color.FromArgb(190, 190, 190);
            }

            _lblMeta.Text = meta;

            PerformLayout();
            Invalidate();
        }

        public void SetFileMessage(
    bool isMine,
    string name,
    string fileName,
    long sizeBytes,
    DateTime time,
    string? path,
    string? status = null)
        {
            IsMine = isMine;
            _bubbleColor = isMine ? MineBack : OtherBack;

            _lblName.Text = name;
            _lblText.Text = "📎 Файл";

            var kb = Math.Max(1, sizeBytes / 1024);
            _lnkFile.Text = $"{fileName} ({kb} KB) — открыть";
            _lnkFile.Tag = path;
            _lnkFile.Visible = true;

            // --- meta (time + colored checks for outgoing)
            string meta = time.ToString("HH:mm");

            if (isMine)
            {
                var s = status ?? "Sent";

                string marks = s switch
                {
                    "Sent" => " ✓",
                    "Delivered" => " ✓✓",
                    "Read" => " ✓✓",
                    _ => ""
                };

                meta += marks;

                _lblMeta.ForeColor = s switch
                {
                    "Sent" => Color.FromArgb(170, 170, 170),        // light gray
                    "Delivered" => Color.FromArgb(170, 170, 170),   // gray
                    "Read" => Color.FromArgb(80, 170, 255),         // blue-ish
                    _ => Color.FromArgb(190, 190, 190)
                };
            }
            else
            {
                _lblMeta.ForeColor = Color.FromArgb(190, 190, 190);
            }

            _lblMeta.Text = meta;

            PerformLayout();
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_stack == null) return;

            _stack.Location = new Point(Padding.Left, Padding.Top);

            int maxW = MaximumSize.Width > 0 ? MaximumSize.Width : 520;
            int innerW = Math.Max(120, maxW - Padding.Horizontal);

            _stack.MaximumSize = new Size(innerW, 0);
            _lblText.MaximumSize = new Size(innerW, 0);
            _lnkFile.MaximumSize = new Size(innerW, 0);

            Size = new Size(
                _stack.PreferredSize.Width + Padding.Horizontal,
                _stack.PreferredSize.Height + Padding.Vertical
            );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            using var path = RoundedRect(rect, Radius);
            using var brush = new SolidBrush(_bubbleColor);

            e.Graphics.FillPath(brush, path);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}