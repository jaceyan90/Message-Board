using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data.SqlClient;
using System.Windows.Controls.Primitives;

namespace MessageBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<MessageThread, List<Message>> listMessages = null;               // All detail of message threads and messages will be loaded to this variable
        private List<MessageThread> listThreads;                                            // All detail of message threads will be loaded to this variable
        private int updateThreadId = 0;                                                     // When admins modify/delete a message, selected thread's id will be stored to this variable
        private int updateMessageId = 0;                                                    // When admins modify/delete a message, selected message's id will be stored to this variable
        private ToggleButton btnPrevious;                                                   // When admins click toggle button to modify/delete a message, previous clicked button's status will be stored to this variable(ischecked or not)
        private Boolean isAdmin = false;                                                    // Check user Mode(true:admin / false:GP)
        private int cntThreads = 0;                                                         // Number of loaded message threads
        private int user_id = 0;                                                            // User's id who is using this system
        private String msgOrigin = "";                                                      // Original message before admin modifies.
        public MainWindow()
        {
            checkUser();
            InitializeComponent();
            loadData(0);
        }

        /// <summary>
        /// Select user mode in test system.
        /// Get user detail from MohioExpress in real system.
        /// </summary>
        private void checkUser()
        {
            string sMessageBoxText = "Admin Mode(Yes) / GP Mode(No)";
            string sCaption = "Select Mode";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    isAdmin = true;
                    user_id = 1;
                    break;

                case MessageBoxResult.No:
                    user_id = 6;
                    break;
            }
        }

        /// <summary>
        /// Add New Message and check whether it is empty or not.
        /// This function will be run when Button New Message clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewMessage(object sender, RoutedEventArgs e)
        {
            if (title.Text.Length <= 0 && message.Text.Length <= 0)
            {
                MessageBox.Show("Title and message cannot be empty.");
            }
            else if (title.Text.Length > 0 && message.Text.Length <= 0)
            {
                MessageBox.Show("Message cannot be empty.");
            }
            else if (title.Text.Length <= 0 && message.Text.Length > 0)
            {
                MessageBox.Show("Title cannot be empty.");
            }
            else
            {
                insertNewMsg();
            }
        }

        /// <summary>
        /// Store valid title and message to the database
        /// </summary>
        private void insertNewMsg()
        {
            DBConnect con = new DBConnect();                //DB Connect
            con.connect();

            con.conOpen();
            //Insert new message thread into message_threads table.
            SqlCommand comm1 = new SqlCommand("insert into message_threads(title, created_date, modified_date, created_by) values(@title, @created_date, @modified_date, @created_by)", con.con);
            comm1.Parameters.AddWithValue("@title", title.Text);
            comm1.Parameters.AddWithValue("@created_date", DateTime.Now);
            comm1.Parameters.AddWithValue("@modified_date", DateTime.Now);
            comm1.Parameters.AddWithValue("@created_by", user_id);

            comm1.ExecuteNonQuery();

            SqlCommand comm2 = new SqlCommand("select top 1 id from message_threads order by created_date desc", con.con);
            var thread_id = (Int32)comm2.ExecuteScalar();

            //Insert new message into messages table.
            SqlCommand comm3 = new SqlCommand("insert into messages(message_thread_id, user_id, message, created_date, created_by, message_type, hidden_type) values(@message_thread_id, @user_id, @message, @created_date, @created_by, @message_type, 0)", con.con);
            comm3.Parameters.AddWithValue("@message_thread_id", thread_id);
            comm3.Parameters.AddWithValue("@user_id", user_id);
            comm3.Parameters.AddWithValue("@message", message.Text);
            comm3.Parameters.AddWithValue("@created_date", DateTime.Now);
            comm3.Parameters.AddWithValue("@created_by", user_id);
            comm3.Parameters.AddWithValue("@message_type", 0);

            comm3.ExecuteNonQuery();

            con.conClose();

            btnNew.IsChecked = false;
            loadData(thread_id);
            title.Text = "";
            message.Text = "";
        }

        /// <summary>
        /// Load data from database and display data in ListView
        /// </summary>
        /// <param name="selected_thread"></param>
        private void loadData(int selected_thread)
        {
            DBConnect con = new DBConnect();                    //DB Connect
            con.connect();

            SqlCommand com = new SqlCommand();
            com.Connection = con.con;

            if (isAdmin == true)
            {
                //Load data from message_threads table in admin mode(undeleted message threads, deleted message threads)
                com.CommandText = "select a.id, a.title, a.created_date, a.modified_date, b.gpname," +
                    " (select count(*)-1 from messages where message_thread_id = a.id) as numofmsg" +
                    " from message_threads a" +
                    " left join users b on a.created_by = b.id" +
                    " left join messages c on a.id=c.message_thread_id" +
                    " where c.message_type=0" +
                    " order by a.modified_date desc";
            }
            else
            {
                //Load data from message_threads table in GP mode(only undeleted message threads)
                com.CommandText = "select a.id, a.title, a.created_date, a.modified_date, b.gpname," +
                    " (select count(*)-1 from messages where message_thread_id = a.id and hidden_type=0) as numofmsg" +
                    " from message_threads a" +
                    " left join users b on a.created_by = b.id" +
                    " left join messages c on a.id=c.message_thread_id" +
                    " where c.message_type=0 and hidden_type=0" +
                    " order by a.modified_date desc";
            }

            con.conOpen();

            SqlDataReader sdr = com.ExecuteReader();

            listMessages = null;
            listMessages = new Dictionary<MessageThread, List<Message>>();
            listThreads = new List<MessageThread>();

            while (sdr.Read())
            {
                MessageThread msgThread = new MessageThread();
                msgThread.Id = sdr.GetInt32(0);
                msgThread.Title = sdr.GetString(1);
                msgThread.CreatedDate = sdr.GetDateTime(2);
                msgThread.ModifiedDate = sdr.GetDateTime(3);
                msgThread.GPName = sdr.GetString(4);
                if (sdr.GetInt32(5) == 0)
                {
                    msgThread.NumOfMessage = sdr.GetInt32(5) + " Reply";
                }
                else
                {
                    msgThread.NumOfMessage = sdr.GetInt32(5) + " Replies";
                }
                if (selected_thread == 0)
                {
                    msgThread.IsExpanded = false;
                }
                else
                {
                    if (msgThread.Id == selected_thread)
                    {
                        msgThread.IsExpanded = true;
                    }
                    else
                    {
                        msgThread.IsExpanded = false;
                    }
                }
                msgThread.MsgSummary = "";
                if (isAdmin == true)
                {
                    msgThread.IsAdmin = true;
                }
                else
                {
                    msgThread.IsAdmin = false;
                }
                msgThread.IsHidden = false;

                listThreads.Add(msgThread);
            }

            con.conClose();

            int cntLoad = 0;
            if (listThreads.Count >= 10)
            {
                cntLoad = 10;
            }
            else
            {
                cntLoad = listThreads.Count;
            }
            for (int i = 0; i < cntLoad; i++)
            {
                if (isAdmin == true)
                {
                    //Load data from messages table in admin mode(undeleted messages, deleted messages)
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname, a.hidden_type" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " order by a.created_date";
                }
                else
                {
                    //Load data from messages table in GP mode(only undeleted messages)
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname, a.hidden_type" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " and a.hidden_type=0" +
                    " order by a.created_date";
                }
                con.conOpen();
                sdr = com.ExecuteReader();

                List<Message> listMsg = new List<Message>();

                while (sdr.Read())
                {
                    Message msg = new Message();
                    msg.Id = sdr.GetInt32(0);
                    msg.MessageThreadId = sdr.GetInt32(1);
                    msg.Messages = sdr.GetString(2);
                    msg.CreatedDate = sdr.GetDateTime(3);
                    msg.MessageType = sdr.GetInt32(4);
                    msg.GPName = sdr.GetString(5);
                    msg.IsExpanded = false;


                    if (msg.MessageType == 0)
                    {
                        String strSummary = msg.Messages;
                        strSummary = strSummary.Replace("\r\n", " ");
                        if (strSummary.Length <= 20)
                        {
                            listThreads[i].MsgSummary = strSummary + " .....";
                        }
                        else
                        {
                            listThreads[i].MsgSummary = strSummary.Substring(0, 20) + " .....";
                        }
                        if (sdr.GetInt32(6) == 1)
                        {
                            msg.IsHidden = true;
                            listThreads[i].IsHidden = true;
                        }
                    }
                    else
                    {
                        if (sdr.GetInt32(6) == 1)
                        {
                            msg.IsHidden = true;
                        }
                        else
                        {
                            msg.IsHidden = false;
                        }
                    }
                    if (isAdmin == true)
                    {
                        msg.IsAdmin = true;
                    }
                    else
                    {
                        msg.IsAdmin = false;
                    }

                    if (listThreads[i].IsHidden == true)
                    {
                        msg.IsHidden = true;
                    }

                    listMsg.Add(msg);
                }
                listMessages.Add(listThreads[i], listMsg);
                con.conClose();
            }

            msgListView.ItemsSource = null;
            msgListView.ItemsSource = listMessages;
            cntThreads = cntLoad;
        }

        /// <summary>
        /// Add New Reply and check whethere it is empty or not.
        /// This function will be run when Button New Message clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewAns(object sender, RoutedEventArgs e)
        {
            Button btnAns = (Button)sender;
            string answer = btnAns.CommandParameter.ToString();
            string thread_id = btnAns.Tag.ToString();

            if (answer.Length <= 0)
            {
                MessageBox.Show("Reply cannot be empty.");
            }
            else
            {
                insertAnswer(answer, thread_id);
            }
        }

        /// <summary>
        /// Valid reply will be stored to the database.
        /// </summary>
        /// <param name="answer">reply user entered</param>
        /// <param name="thread_id">selected message thread's id of reply</param>
        private void insertAnswer(string answer, string thread_id)
        {
            DBConnect con = new DBConnect();
            con.connect();
            con.conOpen();
            SqlCommand com = new SqlCommand("insert into messages(message_thread_id, user_id, message, created_date, created_by, message_type, hidden_type) values(@message_thread_id, @user_id, @message, @created_date, @created_by, 1, 0)", con.con);
            com.Parameters.AddWithValue("@message_thread_id", thread_id);
            com.Parameters.AddWithValue("@user_id", user_id);
            com.Parameters.AddWithValue("@message", answer);
            com.Parameters.AddWithValue("@created_date", DateTime.Now);
            com.Parameters.AddWithValue("@created_by", user_id);

            com.ExecuteNonQuery();

            com = new SqlCommand("update message_threads set modified_date = @modified_date where id=@id", con.con);
            com.Parameters.AddWithValue("@modified_date", DateTime.Now);
            com.Parameters.AddWithValue("@id", thread_id);

            com.ExecuteNonQuery();

            con.conClose();
            loadData(Int32.Parse(thread_id));
        }

        /// <summary>
        /// Load data from database with keyword which user entered in search text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void search(object sender, KeyEventArgs e)
        {
            cntThreads = 0;
            if (e.Key != System.Windows.Input.Key.Enter) return;

            string strSearch = InputTextBox.Text;

            DBConnect con = new DBConnect();
            con.connect();
            SqlCommand com = new SqlCommand();
            com.Connection = con.con;
            if (isAdmin == true)
            {
                com.CommandText = "select distinct a.id, a.title, a.created_date, a.modified_date, b.gpname," +
                " (select count(*)-1 from messages where message_thread_id = a.id) as numofmsg" +
                " from message_threads a" +
                " left join users b on a.created_by = b.id" +
                " left join messages c on a.id = c.message_thread_id" +
                " where (c.message_thread_id in (select distinct message_thread_id from messages where message like '%" + strSearch + "%')" +
                " or a.id in (select id from message_threads where title like '%" + strSearch + "%')" +
                " or b.gpname like '%" + strSearch + "%')" +
                " and c.message_type=0" +
                " order by a.modified_date desc";
            }
            else
            {
                com.CommandText = "select distinct a.id, a.title, a.created_date, a.modified_date, b.gpname," +
               " (select count(*)-1 from messages where message_thread_id = a.id and hidden_type=0) as numofmsg" +
               " from message_threads a" +
               " left join users b on a.created_by = b.id" +
               " left join messages c on a.id = c.message_thread_id" +
               " where (c.message_thread_id in (select distinct message_thread_id from messages where message like '%" + strSearch + "%')" +
               " or a.id in (select id from message_threads where title like '%" + strSearch + "%')" +
                " or b.gpname like '%" + strSearch + "%')" +
               " and (c.message_type=0 and c.hidden_type=0)" +
               " order by a.modified_date desc";
            }

            con.conOpen();

            SqlDataReader sdr = com.ExecuteReader();

            listMessages = new Dictionary<MessageThread, List<Message>>();
            listThreads = new List<MessageThread>();

            while (sdr.Read())
            {
                MessageThread msgThread = new MessageThread();
                msgThread.Id = sdr.GetInt32(0);
                msgThread.Title = sdr.GetString(1);
                msgThread.CreatedDate = sdr.GetDateTime(2);
                msgThread.ModifiedDate = sdr.GetDateTime(3);
                msgThread.GPName = sdr.GetString(4);
                if (sdr.GetInt32(5) == 0)
                {
                    msgThread.NumOfMessage = sdr.GetInt32(5) + " Reply";
                }
                else
                {
                    msgThread.NumOfMessage = sdr.GetInt32(5) + " Replies";
                }
                if (isAdmin == true)
                {
                    msgThread.IsAdmin = true;
                }
                else
                {
                    msgThread.IsAdmin = false;
                }
                msgThread.IsExpanded = false;
                msgThread.MsgSummary = "";

                listThreads.Add(msgThread);
            }

            con.conClose();
            int cntLoad = 0;
            if (listThreads.Count >= 10)
            {
                cntLoad = 10;
            }
            else
            {
                cntLoad = listThreads.Count;
            }
            for (int i = 0; i < cntLoad; i++)
            {
                if (isAdmin == true)
                {
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " order by a.created_date";
                }
                else
                {
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " and a.hidden_type=0" +
                    " order by a.created_date";
                }


                con.conOpen();
                sdr = com.ExecuteReader();

                List<Message> listMsg = new List<Message>();

                while (sdr.Read())
                {
                    Message msg = new Message();
                    msg.Id = sdr.GetInt32(0);
                    msg.MessageThreadId = sdr.GetInt32(1);
                    msg.Messages = sdr.GetString(2);
                    msg.CreatedDate = sdr.GetDateTime(3);
                    msg.MessageType = sdr.GetInt32(4);
                    msg.GPName = sdr.GetString(5);
                    msg.IsExpanded = false;


                    if (msg.MessageType == 0)
                    {
                        String strSummary = msg.Messages;
                        strSummary = strSummary.Replace("\r\n", " ");
                        if (strSummary.Length <= 20)
                        {
                            listThreads[i].MsgSummary = strSummary + " .....";
                        }
                        else
                        {
                            listThreads[i].MsgSummary = strSummary.Substring(0, 20) + " .....";
                        }
                    }
                    if (isAdmin == true)
                    {
                        msg.IsAdmin = true;
                    }
                    else
                    {
                        msg.IsAdmin = false;
                    }

                    listMsg.Add(msg);
                }
                listMessages.Add(listThreads[i], listMsg);
                con.conClose();
            }

            msgListView.ItemsSource = listMessages;
            cntThreads = cntLoad;
        }

        /// <summary>
        /// Add text 'Search' in the Text Box for search as a placeholder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddText(object sender, EventArgs e)
        {
            if (InputTextBox.Text == "")
            {
                InputTextBox.Text = "Search";
                InputTextBox.Foreground = Brushes.Gray;
            }
        }

        /// <summary>
        /// Remove placeholder in the TextBox for search when the Text Box gets mouse focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveText(object sender, MouseEventArgs e)
        {
            InputTextBox.Text = "";

        }

        /// <summary>
        /// Remove placeholder in the Text Box for search when the Text Box gets keyboard focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveText(object sender, KeyboardFocusChangedEventArgs e)
        {
            InputTextBox.Text = "";
        }

        /// <summary>
        /// Add placeholder in the Text Box for search when the Text Box lose keyboard focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddText(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (InputTextBox.Text == "")
            {
                InputTextBox.Text = "Search";
                InputTextBox.Foreground = Brushes.Gray;
            }
        }

        /// <summary>
        /// Add placehilder in the Text Box for search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddText(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text == "")
            {
                InputTextBox.Text = "Search";
                InputTextBox.Foreground = Brushes.Gray;
            }
        }

        /// <summary>
        /// Change message area to modify mode from readonly mode.
        /// When Modify menu in toggle button in admin mode clicked, this function will be run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifyMode(object sender, RoutedEventArgs e)
        {
            if (btnPrevious.IsChecked == true)
            {
                btnPrevious.IsChecked = false;
            }
            foreach (var tempList in listMessages)
            {
                MessageThread msgThread = tempList.Key;
                List<Message> msgList = tempList.Value;
                if (msgThread.Id == updateThreadId)
                {
                    msgThread.IsExpanded = true;
                }
                else
                {
                    msgThread.IsExpanded = false;
                }

                foreach (var tmpMsg in msgList)
                {
                    Message msg = tmpMsg;
                    if (msg.Id == updateMessageId)
                    {
                        msg.IsExpanded = true;
                        msgOrigin = msg.Messages;
                    }
                    else
                    {
                        msg.IsExpanded = false;
                    }
                }
            }
            msgListView.ItemsSource = null;
            msgListView.ItemsSource = listMessages;
        }

        /// <summary>
        /// Delete a message which is selected by admin.
        /// If it is a main message of the message thread, message thread and following messages will be deleted.
        /// Data will not be deleted in database, but users cannot access to the message/message thread in GP mode.
        /// When Delete menu in toggle button in admin mode clicked, this function will be run.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteMessage(object sender, RoutedEventArgs e)
        {
            //SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=dbMessage;Integrated Security=True;");
            DBConnect con = new DBConnect();
            con.connect();
            con.conOpen();
            SqlCommand com = new SqlCommand("update messages set hidden_type=1 where id=@id", con.con);
            com.Parameters.AddWithValue("@id", updateMessageId);

            com.ExecuteNonQuery();

            con.conClose();
            loadData(updateThreadId);
        }

        /// <summary>
        /// Control toggle button's status in admin mode for modify/delete messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToggleUpdateMsg(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;

            updateMessageId = Int32.Parse(btn.CommandParameter.ToString());
            updateThreadId = Int32.Parse(btn.Tag.ToString());

            btn.ContextMenu.IsOpen = false;

            if (btn.IsChecked.Value == true)
            {
                btn.ContextMenu.IsOpen = true;
            }

            if (btnPrevious != null && btnPrevious != btn)
            {
                if (btnPrevious.IsChecked == true)
                {
                    btnPrevious.ContextMenu.IsOpen = false;
                    btnPrevious.IsChecked = false;
                }
            }
            btnPrevious = btn;
        }

        /// <summary>
        /// Quit Modify mode and return to readonly mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelModify(object sender, RoutedEventArgs e)
        {
            if (btnPrevious.IsChecked == true)
            {
                btnPrevious.IsChecked = false;
            }
            foreach (var tempList in listMessages)
            {
                List<Message> msgList = tempList.Value;

                foreach (var tmpMsg in msgList)
                {
                    Message msg = tmpMsg;
                    if (msg.IsExpanded == true)
                    {
                        msg.IsExpanded = false;
                        msg.Messages = msgOrigin;
                    }
                }
            }
            msgListView.ItemsSource = null;
            msgListView.ItemsSource = listMessages;
        }

        /// <summary>
        /// Store updated message by admin to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifyMessage(object sender, RoutedEventArgs e)
        {
            Button btnModify = (Button)sender;
            string updatedMsg = btnModify.CommandParameter.ToString();

            //SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=dbMessage;Integrated Security=True;");
            DBConnect con = new DBConnect();
            con.connect();
            con.conOpen();
            SqlCommand com = new SqlCommand("update messages set modified_date = @modified_date, message = @message, modified_by=@modified_by where id=@id", con.con);
            com.Parameters.AddWithValue("@modified_date", DateTime.Now);
            com.Parameters.AddWithValue("@message", updatedMsg);
            com.Parameters.AddWithValue("@modified_by", 1);
            com.Parameters.AddWithValue("@id", updateMessageId);

            com.ExecuteNonQuery();

            con.conClose();
            loadData(updateThreadId);
        }

        /// <summary>
        /// When scroll bar placed 90% of the scroll height, call loadMoreData function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrChange(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sc = (ScrollViewer)sender;
            double scrHeight = sc.ScrollableHeight;
            double crntHeight = e.VerticalOffset;

            if (crntHeight > (scrHeight * 0.9))
            {
                loadMoreData();
            }
        }

        /// <summary>
        /// Load next 10 message threads from the database.
        /// </summary>
        private void loadMoreData()
        {
            DBConnect con = new DBConnect();
            con.connect();

            SqlCommand com = new SqlCommand();
            com.Connection = con.con;

            int cntLoad = 0;
            if (listThreads.Count - cntThreads >= 10)
            {
                cntLoad = 10;
            }
            else if (listThreads.Count - cntThreads < 10 && listThreads.Count - cntThreads > 0)
            {
                cntLoad = listThreads.Count - cntThreads;
            }
            for (int i = cntThreads; i < cntThreads + cntLoad; i++)
            {
                if (isAdmin == true)
                {
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname, a.hidden_type" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " order by a.created_date";
                }
                else
                {
                    com.CommandText = "select a.id, a.message_thread_id, a.message, a.created_date, a.message_type, b.gpname, a.hidden_type" +
                    " from messages a" +
                    " left join users b on a.created_by = b.id" +
                    " where a.message_thread_id = " + listThreads[i].Id +
                    " and a.hidden_type=0" +
                    " order by a.created_date";
                }
                con.conOpen();

                SqlDataReader sdr = com.ExecuteReader();

                List<Message> listMsg = new List<Message>();

                while (sdr.Read())
                {
                    Message msg = new Message();
                    msg.Id = sdr.GetInt32(0);
                    msg.MessageThreadId = sdr.GetInt32(1);
                    msg.Messages = sdr.GetString(2);
                    msg.CreatedDate = sdr.GetDateTime(3);
                    msg.MessageType = sdr.GetInt32(4);
                    msg.GPName = sdr.GetString(5);
                    msg.IsExpanded = false;
                    if (msg.MessageType == 0)
                    {
                        String strSummary = msg.Messages;
                        strSummary = strSummary.Replace("\r\n", " ");
                        if (strSummary.Length <= 20)
                        {
                            listThreads[i].MsgSummary = strSummary + " .....";
                        }
                        else
                        {
                            listThreads[i].MsgSummary = strSummary.Substring(0, 20) + " .....";
                        }
                        if (sdr.GetInt32(6) == 1)
                        {
                            msg.IsHidden = true;
                            listThreads[i].IsHidden = true;
                        }
                    }
                    else
                    {
                        if (sdr.GetInt32(6) == 1)
                        {
                            msg.IsHidden = true;
                        }
                        else
                        {
                            msg.IsHidden = false;
                        }
                    }
                    if (isAdmin == true)
                    {
                        msg.IsAdmin = true;
                    }
                    else
                    {
                        msg.IsAdmin = false;
                    }

                    if (listThreads[i].IsHidden == true)
                    {
                        msg.IsHidden = true;
                    }

                    listMsg.Add(msg);
                }
                listMessages.Add(listThreads[i], listMsg);
                con.conClose();
            }

            msgListView.ItemsSource = listMessages;
            cntThreads = cntThreads + cntLoad;
        }
    }
}
