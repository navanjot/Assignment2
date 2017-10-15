using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
namespace Assignment2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        SqlConnection conn;
        SqlCommand cmd;
        protected void Page_Load(object sender, EventArgs e)
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            conn.Open();
            if (!IsPostBack)
            {
                DropDownList1.DataSource = FetchCategory();
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("Select a category", "-1"));
                 
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(File.Open("C:\\COURSES\\PROG37721\\VisualStudio\\Assignment2\\Items.txt", FileMode.Open));

            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            {
                conn.Open();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { ' ' });

                    string insert = "Insert into Items values(@ItemId, @Name, @Category, @UnitPrice)";
                    cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@ItemId", parts[0]);
                    cmd.Parameters.AddWithValue("@Name", parts[1]);
                    cmd.Parameters.AddWithValue("@Category", parts[2]);
                    cmd.Parameters.AddWithValue("@UnitPrice", parts[3]);
                    cmd.ExecuteNonQuery();

                }
                conn.Close();

            }
            Response.Redirect("WebForm1.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(File.Open("C:\\COURSES\\PROG37721\\VisualStudio\\Assignment2\\Transactions.txt", FileMode.Open));

            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            {
                conn.Open();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { ' ' });

                    string insert = "Insert into Transcation values(@Transcation, @ItemId, @quality)";
                    cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@Transcation", parts[0]);
                    cmd.Parameters.AddWithValue("@ItemId", parts[1]);
                    cmd.Parameters.AddWithValue("@quality", parts[2]);
                    cmd.ExecuteNonQuery();

                }
                conn.Close();

            }
            Response.Redirect("WebForm1.aspx");


        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
     
                FetchItem(DropDownList1.SelectedValue);

         }
        private DataSet FetchCategory()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            string query = "Select DISTINCT Category from Items";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            DropDownList1.DataTextField = "Category";
            da.Fill(ds);
            return ds;
        }
        private void FetchItem(string tempCategory)
        {

            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Insert Into CategoryTotal(ItemId,Item,Amount) Select t.ItemId,t.Name ,t.UnitPrice*Sum(quality) AS Amount from Items t INNER JOIN Transcation r ON t.ItemId = r.ItemId Where Category = @category Group BY t.ItemId,t.Name,t.UnitPrice";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Category", tempCategory);
            cmd.ExecuteNonQuery();

        }

        protected void Button3_Click(object sender, EventArgs e)
        {

            //Response.Write(DropDownList1.SelectedIndex);
            Response.Write(DropDownList1.SelectedIndex);
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Select * from CategoryTotal";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            GridView3.DataSource = ds;
            GridView3.DataBind();
            conn.Close();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Select t.ItemId, Name, Category, UnitPrice From Items t Where Not Exists (Select ItemId From Transcation r Where t.ItemId = r.ItemId) ";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            GridView4.DataSource = ds;
            GridView4.DataBind();
            conn.Close();
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Select t.UnitPrice*Sum(quality) AS Total_Cost from Items t INNER JOIN Transcation r ON t.ItemId = r.ItemId Group BY t.UnitPrice";
            double Total = 0.0;
            SqlCommand cmd = new SqlCommand(query, conn);
            using (SqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    Total += Convert.ToDouble(r["Total_Cost"]);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            Label2.Text = "The Total cost of all Transactions are " + Total;
            conn.Close();
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Select DISTINCT Name, Category From Items t Inner Join Transcation r ON t.ItemId = r.ItemId Where quality > 3 ";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            GridView5.DataSource = ds;
            GridView5.DataBind();
            conn.Close();
        }

        protected void Button6_Click1(object sender, EventArgs e)
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            conn.Open();
            string query = "Select DISTINCT Name, Category From Items t Inner Join Transcation r ON t.ItemId = r.ItemId Where quality > 3 ";
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            GridView5.DataSource = ds;
            GridView5.DataBind();
            conn.Close();
        }

    }
}