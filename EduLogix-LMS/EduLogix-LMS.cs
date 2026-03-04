namespace EduLogix_LMS
{
    public partial class LMSDashboard : Form
    {
        public LMSDashboard()
        {
            InitializeComponent();
        }

        private void LMSDashboard_Load(object sender, EventArgs e)
        {
            var ucDashboard = new ucDashboard();
            ucDashboard.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(ucDashboard);

            var ucSidebar = new ucSidebar();
            ucSidebar.Dock = DockStyle.Left;
            pnlSidebarContainer.Controls.Add(ucSidebar);

            var ucTopbar = new ucTopbar();
            ucTopbar.Dock = DockStyle.Fill;
            pnlTopbarContainer.Controls.Add(ucTopbar);
        }
    }
}
