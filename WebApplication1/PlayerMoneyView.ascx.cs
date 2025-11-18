using System;
using System.Web.UI;

namespace WebApplication1
{
    public partial class PlayerMoneyView : UserControl
    {
        public Guid PlayerId { get; private set; }

        public int Stack { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BindPlayer(Guid playerId, int stack)
        {
            PlayerId = playerId;
            Stack = stack;

            litPlayerId.Text = playerId.ToString();
            litPlayerStack.Text = stack.ToString();
        }
    }
}
