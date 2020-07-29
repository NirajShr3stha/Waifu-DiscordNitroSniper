using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Discord.Gateway;
using Discord;
using System.Diagnostics;

namespace DiscordNitroSniper
{
    public partial class frmDiscordNitroSniper : Form
    {
        private int succ = 0;

        private int fail = 0;

        private Config config;
        public static DiscordSocketClient Client { get; set; }
		private Color borderColor;
        private int borderWidth = 3;
		private Color headerColor;
        private Rectangle headerRect;
        private Rectangle clientRect;
        private Rectangle titleRect;
        private Rectangle miniBoxRect;
        private Rectangle maxBoxRect;
        private Rectangle closeBoxRect;
        private ButtonState miniState;
        private ButtonState maxState;
        private ButtonState closeState;
        private int x = 0;
        private int y = 0;
        private readonly Size BUTTON_BOX_SIZE = new Size(15, 15);

        public frmDiscordNitroSniper()
        {
			base.Paint += this.frmDiscordNitroSniper_Paint;
			base.Resize += this.frmDiscordNitroSniper_Resize;
			base.MouseDown += this.frmDiscordNitroSniper_MouseDown;
			base.MouseMove += this.frmDiscordNitroSniper_MouseMove;
			base.MouseUp += this.frmDiscordNitroSniper_MouseUp;
			base.MouseDoubleClick += this.frmDiscordNitroSniper_MouseDoubleClick;
			this.borderColor = Color.Black;
			this.borderWidth = 3;
			this.headerColor = Color.Black;
			//this.x = 0;
			//this.y = 0;
			//this.BUTTON_BOX_SIZE = new Size(15, 15);
			InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
			if (File.Exists("Config.json"))
			{
				Log("Config.json Located...", Color.Gray);
				lblStatus.Text = "Connecting...";
				lblStatus.ForeColor = Color.Gray;
				Login();
			}
			else
			{
				Log("Hello User! Please enter your token and click login to start sniping them nitros!", Color.Gray);
			}
		}

        private void Login()
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));

            Client = new DiscordSocketClient();
            Client.OnLoggedIn += Client_OnLoggedIn;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.Login(config.Token);
			tbToken.Text = config.Token;
			tbToken.Enabled = false;
			btnLogin.Enabled = false;
		}

        private void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            Text = "Discord Nitro Sniper - Logged into " + args.User + "!";
			Log("Logged into " + args.User + "!",Color.Green);
			lblStatus.Text = "Connected!";
			lblStatus.ForeColor = Color.Green;
			
		}
        private void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            int i = args.Message.Content.IndexOf("discord.gift/");

            if (i != -1)
            {
                string gift = args.Message.Content.Substring(i + 13);

                if (gift.Length == 16)
                {
                    try
                    {
                        Client.RedeemNitroGift(gift, args.Message.ChannelId);
						Log("Successfully Redeem Nitro", Color.Green);
						succ++;
                    }
                    catch (DiscordHttpException ex)
                    {
                        switch (ex.Code)
                        {
                            case DiscordError.NitroGiftRedeemed:
                                Log("Error Nitro gift already redeemed: " + gift,Color.Red);
								fail++;
                                break;
                            case DiscordError.UnknownGiftCode:
                                Log("Error Invalid nitro gift: " + gift, Color.Red);
								fail++;
								break;
                            default:
                                Log($"Error Unknown error: {ex.Code} | {ex.ErrorMessage}", Color.Red);
								fail++;
								break;
                        }
                    }
					lblFail.Text = fail.ToString();
					lblSucc.Text = succ.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Config.json", JsonConvert.SerializeObject(new Config { Token = tbToken.Text }));
            Login();
        }
		public void Log(string text, Color col, bool newline = true)
		{

			AddText("[", false, Color.Black);
			AddText(DateTime.Now.ToString("HH:mm:ss"), false, Color.Green);
			AddText("] [", false, Color.Black);
			AddText("NitroSniper", false, Color.Magenta);
			AddText("] ", false, Color.Black);
			AddText(text, newline, col);
		}

		private void AddText(string text, bool addNewLine, Color color)
		{
			rtbLog.SuspendLayout();
			rtbLog.SelectionColor = color;
			rtbLog.AppendText(addNewLine
				? $"{text}{Environment.NewLine}"
				: text);
			rtbLog.ScrollToCaret();
			rtbLog.ResumeLayout();
		}
		#region style
		private void frmDiscordNitroSniper_Paint(object sender, PaintEventArgs e)
		{
			using (Brush b = new SolidBrush(this.borderColor))
			{
				e.Graphics.FillRectangle(b, this.headerRect);
			}
			using (Brush b2 = new SolidBrush(Color.White))
			{
				e.Graphics.DrawString(this.Text, this.Font, b2, this.titleRect);
			}
			ControlPaint.DrawCaptionButton(e.Graphics, this.miniBoxRect, CaptionButton.Minimize, this.miniState);
			ControlPaint.DrawCaptionButton(e.Graphics, this.closeBoxRect, CaptionButton.Close, this.closeState);
			ControlPaint.DrawBorder(e.Graphics, this.clientRect, this.borderColor, this.borderWidth, ButtonBorderStyle.Solid, this.borderColor, this.borderWidth, ButtonBorderStyle.Solid, this.borderColor, this.borderWidth, ButtonBorderStyle.Solid, this.borderColor, this.borderWidth, ButtonBorderStyle.Solid);
		}

		private void frmDiscordNitroSniper_Resize(object sender, EventArgs e)
		{
			this.headerRect = new Rectangle(base.ClientRectangle.Location, new Size(base.ClientRectangle.Width, 25));
			checked
			{
				this.clientRect = new Rectangle(new Point(base.ClientRectangle.Location.X, base.ClientRectangle.Y + 25), (Size)new Point(base.ClientRectangle.Width, base.ClientRectangle.Height - 25));
				double yOffset = (double)(this.headerRect.Height + this.borderWidth - this.BUTTON_BOX_SIZE.Height) / 2.0;
				this.titleRect = new Rectangle((int)Math.Round(yOffset), (int)Math.Round(yOffset), (int)Math.Round(unchecked((double)(checked(base.ClientRectangle.Width - 3 * (this.BUTTON_BOX_SIZE.Width + 1))) - yOffset)), this.BUTTON_BOX_SIZE.Height);
				this.miniBoxRect = new Rectangle(base.ClientRectangle.Width - 2 * (this.BUTTON_BOX_SIZE.Width + 1), (int)Math.Round(yOffset), this.BUTTON_BOX_SIZE.Width, this.BUTTON_BOX_SIZE.Height);
				this.closeBoxRect = new Rectangle(base.ClientRectangle.Width - 1 * (this.BUTTON_BOX_SIZE.Width + 1), (int)Math.Round(yOffset), this.BUTTON_BOX_SIZE.Width, this.BUTTON_BOX_SIZE.Height);
				base.Invalidate();
			}
		}

		private void frmDiscordNitroSniper_MouseDown(object sender, MouseEventArgs e)
		{
			bool flag = this.titleRect.Contains(e.Location);
			if (flag)
			{
				this.x = e.X;
				this.y = e.Y;
			}
			Point mousePos = base.PointToClient(Control.MousePosition);
			bool flag2 = this.miniBoxRect.Contains(mousePos);
			if (flag2)
			{
				this.miniState = ButtonState.Pushed;
			}
			else
			{
				bool flag3 = this.maxBoxRect.Contains(mousePos);
				if (flag3)
				{
					this.maxState = ButtonState.Pushed;
				}
				else
				{
					bool flag4 = this.closeBoxRect.Contains(mousePos);
					if (flag4)
					{
						this.closeState = ButtonState.Pushed;
					}
				}
			}
		}

		private void frmDiscordNitroSniper_MouseMove(object sender, MouseEventArgs e)
		{
			bool flag = this.x != 0 & this.y != 0;
			if (flag)
			{
				base.Location = checked(new Point(base.Left + e.X - this.x, base.Top + e.Y - this.y));
				this.Refresh();
			}
		}

		private void frmDiscordNitroSniper_MouseUp(object sender, MouseEventArgs e)
		{
			this.x = 0;
			this.y = 0;
			bool flag = this.miniState == ButtonState.Pushed;
			if (flag)
			{
				base.WindowState = FormWindowState.Minimized;
				this.miniState = ButtonState.Normal;
			}
			else
			{
				bool flag2 = this.maxState == ButtonState.Pushed;
				if (flag2)
				{
					bool flag3 = base.WindowState == FormWindowState.Normal;
					if (flag3)
					{
						base.WindowState = FormWindowState.Maximized;
						this.maxState = ButtonState.Checked;
					}
					else
					{
						base.WindowState = FormWindowState.Normal;
						this.maxState = ButtonState.Normal;
					}
				}
				else
				{
					bool flag4 = this.closeState == ButtonState.Pushed;
					if (flag4)
					{
						base.Close();
					}
				}
			}
		}

		private void frmDiscordNitroSniper_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			bool flag = this.titleRect.Contains(e.Location);
			if (flag)
			{
				bool flag2 = base.WindowState == FormWindowState.Normal;
				if (flag2)
				{
					base.WindowState = FormWindowState.Maximized;
					this.maxState = ButtonState.Checked;
				}
				else
				{
					base.WindowState = FormWindowState.Normal;
					this.maxState = ButtonState.Normal;
				}
			}
		}
        #endregion
    }
}
