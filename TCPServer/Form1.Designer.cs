
namespace TCPServer
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MessageTextBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.MessageListBox = new System.Windows.Forms.ListBox();
            this.userCheckList = new System.Windows.Forms.CheckedListBox();
            this.checkClients = new System.Windows.Forms.Timer(this.components);
            this.checkMessages = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.MessageTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sendBtn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.MessageListBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.userCheckList, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(109, 60);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(659, 255);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MessageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageTextBox.Location = new System.Drawing.Point(10, 10);
            this.MessageTextBox.Margin = new System.Windows.Forms.Padding(10);
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(441, 21);
            this.MessageTextBox.TabIndex = 0;
            this.MessageTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox_KeyUp);
            // 
            // sendBtn
            // 
            this.sendBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sendBtn.Location = new System.Drawing.Point(464, 10);
            this.sendBtn.Margin = new System.Windows.Forms.Padding(3, 10, 10, 10);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(185, 31);
            this.sendBtn.TabIndex = 1;
            this.sendBtn.Text = "SEND";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // MessageListBox
            // 
            this.MessageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessageListBox.FormattingEnabled = true;
            this.MessageListBox.ItemHeight = 12;
            this.MessageListBox.Location = new System.Drawing.Point(10, 61);
            this.MessageListBox.Margin = new System.Windows.Forms.Padding(10);
            this.MessageListBox.Name = "MessageListBox";
            this.MessageListBox.Size = new System.Drawing.Size(441, 184);
            this.MessageListBox.TabIndex = 2;
            // 
            // userCheckList
            // 
            this.userCheckList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userCheckList.FormattingEnabled = true;
            this.userCheckList.Items.AddRange(new object[] {
            "All"});
            this.userCheckList.Location = new System.Drawing.Point(464, 61);
            this.userCheckList.Margin = new System.Windows.Forms.Padding(3, 10, 10, 10);
            this.userCheckList.Name = "userCheckList";
            this.userCheckList.Size = new System.Drawing.Size(185, 184);
            this.userCheckList.TabIndex = 3;
            this.userCheckList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.userCheckList_ItemCheck);
            // 
            // checkClients
            // 
            this.checkClients.Enabled = true;
            this.checkClients.Tick += new System.EventHandler(this.checkClients_Tick);
            // 
            // checkMessages
            // 
            this.checkMessages.Enabled = true;
            this.checkMessages.Tick += new System.EventHandler(this.checkMessages_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox MessageTextBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.ListBox MessageListBox;
        private System.Windows.Forms.CheckedListBox userCheckList;
        private System.Windows.Forms.Timer checkClients;
        private System.Windows.Forms.Timer checkMessages;
    }
}

